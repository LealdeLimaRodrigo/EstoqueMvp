// ==========================================
// Movimentacao/index.js — Entrada, consumo e transferência de estoque
// ==========================================
(function () {
    'use strict';

    var todasMovimentacoes = [];
    var paginaAtualMov = 1;
    var filtroTipoQs = null;
    var filtroTipoBtn = null;
    var movFormDirty = false;
    var _estoqueSetoresCache = [];

    /// Atualiza o dropdown de destino excluindo o setor de origem selecionado.
    function atualizarDestinoFiltrado() {
        var selOrigem = document.getElementById('selOrigem');
        var selDestino = document.getElementById('selDestino');
        var origemId = parseInt(selOrigem.value, 10);
        selDestino.innerHTML = '';
        dataStore.obter('setores').forEach(function (s) {
            if (s.Id === origemId) return;
            var opt = document.createElement('option');
            opt.value = s.Id;
            var esSetor = _estoqueSetoresCache.find(function (es) { return es.SetorId === s.Id; });
            var qtdSetor = esSetor ? esSetor.QuantidadeEstoque : 0;
            opt.textContent = s.Nome + ' (' + qtdSetor + ' un.)';
            selDestino.appendChild(opt);
        });
    }
    var tiposMovimentacao = {};
    tiposMovimentacao[TIPO_ENTRADA] = 'ENTRADA';
    tiposMovimentacao[TIPO_CONSUMO] = 'CONSUMO';
    tiposMovimentacao[TIPO_ENVIO] = 'ENVIO';
    tiposMovimentacao[TIPO_RECEBIMENTO] = 'RECEBIMENTO';

    document.addEventListener('DOMContentLoaded', function () {
        var params = new URLSearchParams(window.location.search);
        filtroTipoQs = params.get('tipo');

        carregarDadosIniciais();

        document.getElementById('btnEntrada').addEventListener('click', function () { abrirModal('ENTRADA'); });
        document.getElementById('btnConsumo').addEventListener('click', function () { abrirModal('CONSUMO'); });
        document.getElementById('btnTransferencia').addEventListener('click', function () { abrirModal('TRANSFERENCIA'); });
        document.getElementById('btnConfirmar').addEventListener('click', executarMovimentacao);
        document.getElementById('btnExportarCsv').addEventListener('click', exportarCsv);
        document.getElementById('selProduto').addEventListener('change', atualizarSaldoModal);
        document.getElementById('selOrigem').addEventListener('change', function () {
            var tipo = document.getElementById('tipoMovimentacao').value;
            if (tipo === 'TRANSFERENCIA') atualizarDestinoFiltrado();
        });

        // Rastrear unsaved changes no modal de movimentação
        var formMov = document.getElementById('formMov');
        if (formMov) {
            formMov.addEventListener('input', function () { movFormDirty = true; });
        }
        var modalMov = document.getElementById('modalMovimentacao');
        if (modalMov) {
            modalMov.addEventListener('hide.bs.modal', function (e) {
                if (movFormDirty) {
                    if (!confirm('Você tem alterações não salvas. Deseja realmente sair?')) {
                        e.preventDefault();
                    } else {
                        movFormDirty = false;
                    }
                }
            });
            modalMov.addEventListener('hidden.bs.modal', function () {
                movFormDirty = false;
                document.getElementById('selProduto').value = '';
                document.getElementById('numQtd').value = '';
                document.getElementById('selProduto').classList.remove('is-invalid');
                document.getElementById('numQtd').classList.remove('is-invalid');
                document.getElementById('divSaldoAtual').classList.add('d-none');
                document.getElementById('btnConfirmar').disabled = false;
                document.getElementById('btnConfirmar').title = '';
                var collapseEl = document.getElementById('collapseEstoqueSetor');
                if (collapseEl && collapseEl.classList.contains('show')) {
                    bootstrap.Collapse.getOrCreateInstance(collapseEl).hide();
                }
                // Restaurar dropdowns de setor completos
                popularSelectsFormulario();
            });
        }

        document.getElementById('buscaMovimentacao').addEventListener('input', debounce(function () {
            paginaAtualMov = 1;
            renderizarTabela();
        }, 300));

        document.getElementById('filtroDataInicio').addEventListener('change', function () {
            paginaAtualMov = 1;
            renderizarTabela();
        });
        document.getElementById('filtroDataFim').addEventListener('change', function () {
            paginaAtualMov = 1;
            renderizarTabela();
        });
        document.getElementById('btnLimparDatas').addEventListener('click', function () {
            document.getElementById('filtroDataInicio').value = '';
            document.getElementById('filtroDataFim').value = '';
            paginaAtualMov = 1;
            renderizarTabela();
        });

        // Filtro por tipo via botões
        var btnsFilroTipo = document.querySelectorAll('.filtro-tipo');
        for (var i = 0; i < btnsFilroTipo.length; i++) {
            btnsFilroTipo[i].addEventListener('click', function () {
                for (var j = 0; j < btnsFilroTipo.length; j++) btnsFilroTipo[j].classList.remove('active');
                this.classList.add('active');
                filtroTipoBtn = this.getAttribute('data-tipo') || null;
                paginaAtualMov = 1;
                renderizarTabela();
            });
        }
    });

    /// Carrega produtos, setores e usuários em paralelo para popular selects e tabela.
    async function carregarDadosIniciais() {
        try {
            var results = await Promise.all([
                dataStore.carregar('produtos', '/produto'),
                dataStore.carregar('setores', '/setor'),
                dataStore.carregar('usuarios', '/usuario'),
                apiGet('/produto/inativos'),
                apiGet('/setor/inativos'),
                apiGet('/usuario/inativos')
            ]);
            // Merge inativos no cache para resolver nomes na tabela de movimentações
            var produtosInativos = results[3] || [];
            var setoresInativos = results[4] || [];
            var usuariosInativos = results[5] || [];
            var todosProdutos = dataStore.obter('produtos').concat(produtosInativos);
            var todosSetores = dataStore.obter('setores').concat(setoresInativos);
            var todosUsuarios = dataStore.obter('usuarios').concat(usuariosInativos);
            dataStore.definir('todosProdutos', todosProdutos);
            dataStore.definir('todosSetores', todosSetores);
            dataStore.definir('todosUsuarios', todosUsuarios);
        } catch (e) { console.error('Erro ao carregar dados iniciais.', e); }

        popularSelectsFormulario();
        carregarTabelaMovimentacoes();
    }

    /// Retorna o nome textual do tipo de movimentação pelo ID.
    function obterNomeTipo(tipoId) {
        return tiposMovimentacao[tipoId] || 'OUTRO';
    }

    /// Aplica filtros de tipo, texto e datas sobre as movimentações carregadas.
    function obterMovimentacoesFiltradas() {
        var lista = todasMovimentacoes;

        // Filtro via query string do dashboard
        if (filtroTipoQs === 'ENTRADA') {
            lista = lista.filter(function (m) { return m.TipoMovimentacaoId === TIPO_ENTRADA; });
        } else if (filtroTipoQs === 'SAIDA') {
            lista = lista.filter(function (m) { return m.TipoMovimentacaoId === TIPO_CONSUMO || m.TipoMovimentacaoId === TIPO_ENVIO; });
        } else if (filtroTipoQs === 'CONSUMO') {
            lista = lista.filter(function (m) { return m.TipoMovimentacaoId === TIPO_CONSUMO; });
        } else if (filtroTipoQs === 'TRANSFERENCIA') {
            lista = lista.filter(function (m) { return m.TipoMovimentacaoId === TIPO_ENVIO || m.TipoMovimentacaoId === TIPO_RECEBIMENTO; });
        }

        // Filtro via botões de tipo
        if (filtroTipoBtn) {
            var tipoMap = { 'ENTRADA': TIPO_ENTRADA, 'CONSUMO': TIPO_CONSUMO, 'ENVIO': TIPO_ENVIO, 'RECEBIMENTO': TIPO_RECEBIMENTO };
            var tipoId = tipoMap[filtroTipoBtn];
            if (tipoId) {
                lista = lista.filter(function (m) { return m.TipoMovimentacaoId === tipoId; });
            }
        }

        // Busca por texto — enriquece com nomes para permitir buscar
        var termo = document.getElementById('buscaMovimentacao').value;
        if (termo && termo.trim()) {
            var termoLower = termo.toLowerCase().trim();
            lista = lista.filter(function (m) {
                var nomeProduto = (dataStore.buscarNome('todosProdutos', m.ProdutoId, null) || dataStore.buscarNome('produtos', m.ProdutoId, '')).toLowerCase();
                var nomeSetor = (dataStore.buscarNome('todosSetores', m.SetorId, null) || dataStore.buscarNome('setores', m.SetorId, '')).toLowerCase();
                var nomeUsuario = (dataStore.buscarNome('todosUsuarios', m.UsuarioId, null) || dataStore.buscarNome('usuarios', m.UsuarioId, '')).toLowerCase();
                var nomeTipo = obterNomeTipo(m.TipoMovimentacaoId).toLowerCase();
                return nomeProduto.indexOf(termoLower) !== -1 ||
                       nomeSetor.indexOf(termoLower) !== -1 ||
                       nomeUsuario.indexOf(termoLower) !== -1 ||
                       nomeTipo.indexOf(termoLower) !== -1;
            });
        }

        // Filtro por intervalo de datas
        var dataInicio = document.getElementById('filtroDataInicio').value;
        var dataFim = document.getElementById('filtroDataFim').value;
        if (dataInicio) {
            var inicio = new Date(dataInicio + 'T00:00:00');
            lista = lista.filter(function (m) {
                return m.DataMovimentacao && parseDateBR(m.DataMovimentacao) >= inicio;
            });
        }
        if (dataFim) {
            var fim = new Date(dataFim + 'T23:59:59');
            lista = lista.filter(function (m) {
                return m.DataMovimentacao && parseDateBR(m.DataMovimentacao) <= fim;
            });
        }

        return lista;
    }

    /// Renderiza a tabela de movimentações com os filtros e paginação aplicados.
    function renderizarTabela() {
        var tbody = document.querySelector('#tabelaMovimentacoes tbody');
        var filtrados = obterMovimentacoesFiltradas();
        var resultado = paginar(filtrados, paginaAtualMov);
        var lista = resultado.dados;

        // Info
        var info = document.getElementById('totalMovInfo');
        if (filtroTipoQs) {
            var label = filtroTipoQs === 'ENTRADA' ? 'Entradas' : 'Saídas';
            renderizarInfoFiltro(info, 'bg-info', label, resultado.totalItens, 'registro(s)', '/Movimentacao/Index');
        } else if (document.getElementById('buscaMovimentacao').value) {
            info.textContent = resultado.totalItens + ' resultado(s)';
        } else {
            info.textContent = resultado.totalItens + ' movimentação(ões)';
        }

        tbody.innerHTML = '';

        if (!lista || lista.length === 0) {
            renderizarEmptyState(tbody, 7, 'fas fa-history', 'Nenhuma movimentação encontrada.');
            document.getElementById('paginacaoMovimentacoes').innerHTML = '';
            return;
        }

        /// Cria um td com nome e indicador de inativo (ícone + cor).
        function criarTdNome(nome, ativo, tooltip) {
            var td = document.createElement('td');
            if (ativo) {
                td.textContent = nome;
            } else {
                var span = document.createElement('span');
                span.className = 'text-danger';
                span.textContent = nome;
                td.appendChild(span);
                var icon = document.createElement('i');
                icon.className = 'fas fa-ban text-danger ms-1 small';
                icon.title = tooltip;
                td.appendChild(icon);
            }
            return td;
        }
        lista.forEach(function (m) {
            var tr = document.createElement('tr');
            var prodAtivo = !!dataStore.buscarPorId('produtos', m.ProdutoId);
            var setorAtivo = !!dataStore.buscarPorId('setores', m.SetorId);
            var usuarioAtivo = !!dataStore.buscarPorId('usuarios', m.UsuarioId);
            var nomeProduto = dataStore.buscarNome('todosProdutos', m.ProdutoId, null) || dataStore.buscarNome('produtos', m.ProdutoId, 'Produto #' + m.ProdutoId);
            var nomeSetor = dataStore.buscarNome('todosSetores', m.SetorId, null) || dataStore.buscarNome('setores', m.SetorId);
            var nomeUsuario = dataStore.buscarNome('todosUsuarios', m.UsuarioId, null) || dataStore.buscarNome('usuarios', m.UsuarioId);
            var tipo = obterNomeTipo(m.TipoMovimentacaoId);

            tr.appendChild(criarElemento('td', null, formatarData(m.DataMovimentacao)));

            var tdTipo = document.createElement('td');
            tdTipo.appendChild(criarElemento('span', 'badge ' + getBadgeTipo(tipo), tipo));
            tr.appendChild(tdTipo);

            tr.appendChild(criarTdNome(nomeProduto, prodAtivo, 'Produto inativo'));
            tr.appendChild(criarElemento('td', null, String(m.Quantidade || 0)));
            tr.appendChild(criarTdNome(nomeSetor, setorAtivo, 'Setor inativo'));
            tr.appendChild(criarTdNome(nomeUsuario, usuarioAtivo, 'Usuário inativo'));

            var tdAcoes = criarElemento('td', 'text-center');
            tdAcoes.appendChild(criarBtnAcao('fas fa-eye', 'btn-outline-secondary', 'Detalhes', 'Ver detalhes', (function (mov, tp, prod, setor, usu, pAtivo, sAtivo, uAtivo) {
                return function () {
                    var html = '<table class="table table-sm mb-0">'
                        + '<tr><th>Data</th><td>' + formatarData(mov.DataMovimentacao) + '</td></tr>'
                        + '<tr><th>Tipo</th><td><span class="badge ' + getBadgeTipo(tp) + '">' + tp + '</span></td></tr>'
                        + '<tr><th>Produto</th><td>' + prod + (pAtivo ? '' : ' <i class="fas fa-ban text-danger" title="Produto inativo"></i>') + '</td></tr>'
                        + '<tr><th>Quantidade</th><td>' + mov.Quantidade + '</td></tr>'
                        + '<tr><th>Setor</th><td>' + setor + (sAtivo ? '' : ' <i class="fas fa-ban text-danger" title="Setor inativo"></i>') + '</td></tr>'
                        + '<tr><th>Responsável</th><td>' + usu + (uAtivo ? '' : ' <i class="fas fa-ban text-danger" title="Usuário inativo"></i>') + '</td></tr>'
                        + '<tr><th>ID Transação</th><td><code class="small">' + (mov.TransacaoId || '—') + '</code></td></tr>'
                        + '</table>';
                    showDetailModal('Movimentação', html);
                };
            })(m, tipo, nomeProduto, nomeSetor, nomeUsuario, prodAtivo, setorAtivo, usuarioAtivo)));
            tr.appendChild(tdAcoes);

            tbody.appendChild(tr);
        });

        renderizarPaginacao('paginacaoMovimentacoes', resultado.paginaAtual, resultado.totalPaginas, function (p) {
            paginaAtualMov = p;
            renderizarTabela();
        });
    }

    /// Carrega todas as movimentações do servidor e renderiza a tabela.
    async function carregarTabelaMovimentacoes() {
        var tbody = document.querySelector('#tabelaMovimentacoes tbody');
        renderizarSkeletonLinhas(tbody, 6);
        try {
            todasMovimentacoes = await apiGet('/movimentacao-estoque');
            paginaAtualMov = 1;
            renderizarTabela();
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Erro ao carregar histórico.</td></tr>';
            console.error(error);
        }
    }

    var badgeTipoMap = {
        'ENTRADA': 'bg-success',
        'CONSUMO': 'bg-warning text-dark',
        'ENVIO': 'bg-info text-white',
        'RECEBIMENTO': 'bg-primary'
    };

    /// Retorna a classe CSS do badge para o tipo de movimentação.
    function getBadgeTipo(tipo) {
        return badgeTipoMap[tipo] || 'bg-secondary';
    }

    /// Popula os selects de produto, setor de origem e destino.
    function popularSelectsFormulario() {
        var selProd = document.getElementById('selProduto');
        selProd.innerHTML = '';
        var optDefault = document.createElement('option');
        optDefault.value = '';
        optDefault.textContent = 'Selecione o Produto...';
        selProd.appendChild(optDefault);

        dataStore.obter('produtos').forEach(function (p) {
            var qtd = p.QuantidadeTotalEstoque || 0;
            var opt = document.createElement('option');
            opt.value = p.Id;
            opt.textContent = p.Nome + ' (Qtd: ' + qtd + ')';
            selProd.appendChild(opt);
        });

        var selOrigem = document.getElementById('selOrigem');
        var selDestino = document.getElementById('selDestino');
        selOrigem.innerHTML = '';
        selDestino.innerHTML = '';

        dataStore.obter('setores').forEach(function (s) {
            var optO = document.createElement('option');
            optO.value = s.Id;
            optO.textContent = s.Nome;
            selOrigem.appendChild(optO);

            var optD = document.createElement('option');
            optD.value = s.Id;
            optD.textContent = s.Nome;
            selDestino.appendChild(optD);
        });
    }

    /// Configura e abre o modal de movimentação conforme o tipo (Entrada, Consumo, Transferência).
    function abrirModal(tipo) {
        var config = {
            'ENTRADA': { titulo: 'Dar Entrada', campos: ['dest'] },
            'CONSUMO': { titulo: 'Registrar Consumo', campos: ['orig'] },
            'TRANSFERENCIA': { titulo: 'Transferir entre Setores', campos: ['orig', 'dest'] }
        };
        var c = config[tipo];
        document.getElementById('tipoMovimentacao').value = tipo;
        document.getElementById('tituloModal').textContent = c.titulo;
        document.getElementById('headerModal').className = 'modal-header navbar-indigo text-white';
        document.getElementById('btnConfirmar').className = 'btn btn-success';
        document.getElementById('divOrigem').classList.toggle('d-none', !c.campos.includes('orig'));
        document.getElementById('divDestino').classList.toggle('d-none', !c.campos.includes('dest'));
        document.getElementById('divTransferArrow').classList.toggle('d-none', tipo !== 'TRANSFERENCIA');
        document.getElementById('divSaldoAtual').classList.add('d-none');
        document.getElementById('numQtd').value = '';
        document.getElementById('selProduto').classList.remove('is-invalid');
        document.getElementById('numQtd').classList.remove('is-invalid');
        movFormDirty = false;
        bootstrap.Modal.getOrCreateInstance(document.getElementById('modalMovimentacao')).show();
    }

    /// Exibe o saldo atual do produto selecionado, popula accordion com estoque por setor
    /// e filtra o dropdown de origem para exibir apenas setores com estoque.
    async function atualizarSaldoModal() {
        var idProd = document.getElementById('selProduto').value;
        var divSaldo = document.getElementById('divSaldoAtual');
        var lblSaldo = document.getElementById('lblSaldoAtual');
        var tbodyES = document.getElementById('tbodyEstoqueSetor');
        var collapseEl = document.getElementById('collapseEstoqueSetor');

        if (!idProd) {
            divSaldo.classList.add('d-none');
            return;
        }

        var prod = dataStore.buscarPorId('produtos', parseInt(idProd, 10));
        var saldo = prod ? (prod.QuantidadeTotalEstoque || 0) : 0;
        lblSaldo.textContent = saldo;
        divSaldo.classList.remove('d-none');

        // Buscar estoque por setor para o produto selecionado
        var estoqueSetores = [];
        try {
            estoqueSetores = await apiGet('/estoque-setor/produto/' + idProd);
        } catch (e) { console.warn('Erro ao buscar estoque por setor:', e); }

        // Popula accordion com a lista de setores e quantidades
        tbodyES.innerHTML = '';
        estoqueSetores.forEach(function (es) {
            var tr = document.createElement('tr');
            tr.appendChild(criarElemento('td', null, dataStore.buscarNome('setores', es.SetorId)));
            tr.appendChild(criarElemento('td', 'text-end fw-semibold', es.QuantidadeEstoque));
            tbodyES.appendChild(tr);
        });
        if (estoqueSetores.length === 0) {
            var tr = document.createElement('tr');
            tr.appendChild(criarElemento('td', 'text-muted', 'Nenhum setor com estoque.'));
            tr.appendChild(criarElemento('td', null, ''));
            tbodyES.appendChild(tr);
        }

        // Filtrar dropdown de origem: apenas setores que possuem este produto
        var tipo = document.getElementById('tipoMovimentacao').value;
        if (tipo === 'CONSUMO' || tipo === 'TRANSFERENCIA') {
            var selOrigem = document.getElementById('selOrigem');
            selOrigem.innerHTML = '';
            estoqueSetores.filter(function (es) { return es.QuantidadeEstoque > 0; }).forEach(function (es) {
                var opt = document.createElement('option');
                opt.value = es.SetorId;
                opt.textContent = dataStore.buscarNome('setores', es.SetorId) + ' (' + es.QuantidadeEstoque + ' un.)';
                selOrigem.appendChild(opt);
            });
            if (selOrigem.options.length === 0) {
                var opt = document.createElement('option');
                opt.value = '';
                opt.textContent = 'Nenhum setor com estoque';
                opt.disabled = true;
                selOrigem.appendChild(opt);
            }
        }

        // Atualizar dropdown de destino com estoque do produto em cada setor
        if (tipo === 'TRANSFERENCIA') {
            _estoqueSetoresCache = estoqueSetores;
            atualizarDestinoFiltrado();
        }

        // Poka-Yoke: desabilitar confirmar se estoque 0 em consumo/transferência
        var btnConfirmar = document.getElementById('btnConfirmar');
        if ((tipo === 'CONSUMO' || tipo === 'TRANSFERENCIA') && saldo === 0) {
            btnConfirmar.disabled = true;
            btnConfirmar.title = 'Estoque zerado — não é possível realizar esta operação.';
        } else {
            btnConfirmar.disabled = false;
            btnConfirmar.title = '';
        }
    }

    /// Valida os campos e envia a movimentação para o backend.
    async function executarMovimentacao() {
        var idProd = document.getElementById('selProduto').value;
        var qtd = parseFloat(document.getElementById('numQtd').value);
        var tipo = document.getElementById('tipoMovimentacao').value;

        var selProduto = document.getElementById('selProduto');
        var numQtd = document.getElementById('numQtd');
        var valido = true;

        selProduto.classList.remove('is-invalid');
        numQtd.classList.remove('is-invalid');

        if (!idProd) { selProduto.classList.add('is-invalid'); valido = false; }
        if (isNaN(qtd) || qtd <= 0) { numQtd.classList.add('is-invalid'); valido = false; }

        // Validação: origem e destino não podem ser o mesmo setor
        if (tipo === 'TRANSFERENCIA') {
            var origemId = document.getElementById('selOrigem').value;
            var destinoId = document.getElementById('selDestino').value;
            if (origemId === destinoId) {
                showToast('Setor de origem e destino não podem ser iguais.', 'danger');
                return;
            }
        }

        if (!valido) return;

        var prod = dataStore.buscarPorId('produtos', parseInt(idProd, 10));
        var estoqueAtual = prod ? (prod.QuantidadeTotalEstoque || 0) : 0;

        if ((tipo === 'CONSUMO' || tipo === 'TRANSFERENCIA') && qtd > estoqueAtual) {
            numQtd.classList.add('is-invalid');
            showToast('Estoque insuficiente! Saldo atual: ' + estoqueAtual, 'danger');
            return;
        }

        var btn = document.getElementById('btnConfirmar');
        setBtnLoading(btn, true);

        try {
            var payload = { ProdutoId: parseInt(idProd, 10), Quantidade: qtd };

            if (tipo === 'CONSUMO') payload.SetorId = parseInt(document.getElementById('selOrigem').value, 10);
            if (tipo === 'ENTRADA') payload.SetorId = parseInt(document.getElementById('selDestino').value, 10);
            if (tipo === 'TRANSFERENCIA') {
                payload.SetorOrigemId = parseInt(document.getElementById('selOrigem').value, 10);
                payload.SetorDestinoId = parseInt(document.getElementById('selDestino').value, 10);
            }

            await apiPost('/movimentacao-estoque/' + tipo.toLowerCase(), payload);

            movFormDirty = false;
            bootstrap.Modal.getInstance(document.getElementById('modalMovimentacao')).hide();
            showToast('Operação registrada com sucesso!', 'success');
            try { await dataStore.carregar('produtos', '/produto'); } catch (ex) { }
            popularSelectsFormulario();
            carregarTabelaMovimentacoes();
        } catch (e) {
            showToast(extrairMsgErro(e, 'Falha ao salvar.'), 'danger');
            console.error(e);
        } finally {
            setBtnLoading(btn, false);
        }
    }

    // ==========================================
    /// Exporta as movimentações filtradas para arquivo CSV.
    function exportarCsv() {
        var lista = obterMovimentacoesFiltradas();
        if (!lista || lista.length === 0) {
            showToast('Nenhuma movimentação para exportar.', 'warning');
            return;
        }

        var linhas = lista.map(function (m) {
            return [
                formatarData(m.DataMovimentacao),
                obterNomeTipo(m.TipoMovimentacaoId),
                dataStore.buscarNome('produtos', m.ProdutoId, 'ID ' + m.ProdutoId),
                String(m.Quantidade || 0),
                dataStore.buscarNome('setores', m.SetorId),
                dataStore.buscarNome('usuarios', m.UsuarioId)
            ];
        });

        exportarParaCsv(
            'movimentacoes_' + formatarDataCurta(new Date()) + '.csv',
            ['Data', 'Tipo', 'Produto', 'Quantidade', 'Setor', 'Responsavel'],
            linhas
        );

        showToast('Arquivo CSV exportado com sucesso!', 'success');
    }
})();