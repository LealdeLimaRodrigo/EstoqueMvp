// ==========================================
// crud-controller.js — Orquestrador CRUD genérico
// Extensível via hooks (aoAbrirNovo, antesValidar, antesEnviar)
// ==========================================

/// Cria uma instância de controlador CRUD genérico, configurável via hooks.
function criarCrud(config) {
    // Defaults derivados a partir do label
    var label = config.label || 'Registro';
    var labelMin = label.toLowerCase();

    config.tituloNovo = config.tituloNovo || 'Novo ' + label;
    config.tituloEditar = config.tituloEditar || 'Editar ' + label;
    config.tituloRemover = config.tituloRemover || 'Remover ' + label;
    config.msgRemover = config.msgRemover || 'Deseja realmente remover este ' + labelMin + '?';
    config.msgAtualizado = config.msgAtualizado || label + ' atualizado com sucesso!';
    config.msgCriado = config.msgCriado || label + ' cadastrado com sucesso!';
    config.msgRemovido = config.msgRemovido || label + ' removido com sucesso.';
    config.msgErroSalvar = config.msgErroSalvar || 'Erro ao salvar ' + labelMin + '.';
    config.msgErroRemover = config.msgErroRemover || 'Erro ao remover ' + labelMin + '.';
    config.labelPlural = config.labelPlural || labelMin + '(s)';
    config.emptyIcon = config.emptyIcon || '';

    var edicaoId = null;
    var paginaAtual = 1;
    var termoBuscaAtual = '';
    var formDirty = false;

    /// Rastreia alterações no formulário para aviso de dados não salvos.
    function inicializarFormDirtyTracking() {
        var form = document.getElementById(config.formId);
        if (!form) return;
        form.addEventListener('input', function () {
            formDirty = true;
            var modalEl = document.getElementById(config.modalId);
            if (modalEl) modalEl.classList.add('form-dirty');
        });

        var modalEl = document.getElementById(config.modalId);
        if (modalEl) {
            modalEl.addEventListener('hide.bs.modal', function (e) {
                if (formDirty) {
                    if (!confirm('Você tem alterações não salvas. Deseja realmente sair?')) {
                        e.preventDefault();
                    } else {
                        limparFormDirty();
                    }
                }
            });
            modalEl.addEventListener('hidden.bs.modal', function () {
                edicaoId = null;
                limparFormDirty();
                var form = document.getElementById(config.formId);
                if (form) {
                    form.reset();
                    form.classList.remove('was-validated');
                }
                if (config.erroId) document.getElementById(config.erroId).classList.add('d-none');
                if (config.tituloModalId) document.getElementById(config.tituloModalId).textContent = config.tituloNovo;
                if (config.aoAbrirNovo) config.aoAbrirNovo();
            });
        }
    }

    /// Limpa o estado de formulário sujo e remove o indicador visual.
    function limparFormDirty() {
        formDirty = false;
        var modalEl = document.getElementById(config.modalId);
        if (modalEl) modalEl.classList.remove('form-dirty');
    }

    /// Carrega os dados paginados do servidor e renderiza na tabela.
    function carregarPaginado(pagina) {
        var tbody = document.querySelector('#' + config.tabelaId + ' tbody');
        renderizarSkeletonLinhas(tbody, config.colunas);
        apiGetPaginado(config.endpoint, pagina).then(function (resultado) {
            paginaAtual = resultado.Pagina;
            renderizar(resultado.Itens, resultado.TotalRegistros, resultado.Pagina, resultado.TotalPaginas, false);
        }).catch(function (error) {
            tbody.innerHTML = '<tr><td colspan="' + config.colunas + '" class="text-center text-danger">Erro ao carregar dados.</td></tr>';
            console.error(error);
        });
    }

    /// Carrega dados filtrados por busca textual com paginação server-side.
    function carregarComBusca(termo) {
        var tbody = document.querySelector('#' + config.tabelaId + ' tbody');
        renderizarSkeletonLinhas(tbody, config.colunas);
        apiGetBuscaPaginada(config.endpoint, termo, paginaAtual).then(function (resultado) {
            paginaAtual = resultado.Pagina;
            renderizar(resultado.Itens, resultado.TotalRegistros, resultado.Pagina, resultado.TotalPaginas, false);
        }).catch(function (error) {
            tbody.innerHTML = '<tr><td colspan="' + config.colunas + '" class="text-center text-danger">Erro ao carregar dados.</td></tr>';
            console.error(error);
        });
    }

    /// Renderiza os itens na tabela e atualiza a paginação.
    function renderizar(itens, totalItens, pagina, totalPaginas, clientSide) {
        var tbody = document.querySelector('#' + config.tabelaId + ' tbody');

        if (config.infoId) {
            var info = document.getElementById(config.infoId);
            info.textContent = totalItens + (termoBuscaAtual ? ' resultado(s)' : ' ' + config.labelPlural);
        }

        tbody.innerHTML = '';

        if (!itens || itens.length === 0) {
            renderizarEmptyState(tbody, config.colunas, config.emptyIconClass || 'fas fa-inbox', 'Nenhum registro encontrado.');
            document.getElementById(config.paginacaoId).innerHTML = '';
            return;
        }

        itens.forEach(function (item) {
            tbody.appendChild(config.renderRow(item, prepararEdicao, remover));
        });

        renderizarPaginacao(config.paginacaoId, pagina, totalPaginas, function (p) {
            paginaAtual = p;
            if (termoBuscaAtual) {
                carregarComBusca(termoBuscaAtual);
            } else {
                carregarPaginado(p);
            }
        });
    }

    /// Busca os dados do registro e abre o modal no modo edição.
    function prepararEdicao(id) {
        edicaoId = id;
        limparFormDirty();
        apiGetRaw(config.endpoint + '/' + id).then(function (data) {
            config.preencherForm(data);
            document.getElementById(config.formId).classList.remove('was-validated');
            if (config.erroId) document.getElementById(config.erroId).classList.add('d-none');
            if (config.tituloModalId) document.getElementById(config.tituloModalId).textContent = config.tituloEditar;
            bootstrap.Modal.getOrCreateInstance(document.getElementById(config.modalId)).show();
        }).catch(function (e) {
            showToast('Erro ao buscar dados.', 'danger');
            console.error(e);
        });
    }

    /// Valida o formulário e envia os dados para criação ou atualização.
    function salvar() {
        var form = document.getElementById(config.formId);
        if (config.antesValidar) config.antesValidar(edicaoId);
        form.classList.add('was-validated');
        if (!form.checkValidity()) return;

        var payload = config.montarPayload();
        if (config.antesEnviar) payload = config.antesEnviar(payload, edicaoId) || payload;
        if (config.erroId) document.getElementById(config.erroId).classList.add('d-none');

        var btn = document.getElementById(config.btnSalvarId);
        setBtnLoading(btn, true);

        var promise;
        if (edicaoId) {
            payload.Id = edicaoId;
            promise = apiPut(config.endpoint + '/' + edicaoId, payload);
        } else {
            promise = apiPost(config.endpoint, payload);
        }

        promise.then(function () {
            showToast(edicaoId ? config.msgAtualizado : config.msgCriado, 'success');
            limparFormDirty();
            bootstrap.Modal.getInstance(document.getElementById(config.modalId)).hide();
            edicaoId = null;
            form.reset();
            form.classList.remove('was-validated');
            if (config.tituloModalId) document.getElementById(config.tituloModalId).textContent = config.tituloNovo;
            recarregarLista();
        }).catch(function (e) {
            var fecharModal = function () {
                limparFormDirty();
                bootstrap.Modal.getInstance(document.getElementById(config.modalId)).hide();
                form.reset();
                form.classList.remove('was-validated');
                recarregarLista();
            };
            var forcarCriacao = function () {
                apiPost(config.endpoint + '?forcar=true', payload).then(function () {
                    showToast(config.msgCriado, 'success');
                    fecharModal();
                }).catch(function (err) {
                    if (config.erroId) {
                        var erroDiv = document.getElementById(config.erroId);
                        erroDiv.textContent = extrairMsgErro(err, config.msgErroSalvar);
                        erroDiv.classList.remove('d-none');
                    }
                });
            };
            // Hook para interceptar erros específicos (ex: 409 Conflict)
            if (config.aoErroSalvar && config.aoErroSalvar(e, payload, fecharModal, forcarCriacao)) {
                return; // Hook tratou o erro
            }
            if (config.erroId) {
                var erroDiv = document.getElementById(config.erroId);
                erroDiv.textContent = extrairMsgErro(e, config.msgErroSalvar);
                erroDiv.classList.remove('d-none');
            }
            console.error(e);
        }).finally(function () {
            setBtnLoading(btn, false);
        });
    }

    /// Exibe confirmação e executa a remoção (soft delete) do registro.
    function remover(id) {
        showConfirmModal(
            config.tituloRemover,
            config.msgRemover,
            function () {
                apiDelete(config.endpoint + '/' + id).then(function () {
                    showToast(config.msgRemovido, 'success');
                    recarregarLista();
                }).catch(function (error) {
                    showToast(extrairMsgErro(error, config.msgErroRemover), 'danger');
                    console.error(error);
                });
            }
        );
    }

    /// Recarrega a listagem mantendo o estado atual de busca e página.
    function recarregarLista() {
        if (termoBuscaAtual) {
            carregarComBusca(termoBuscaAtual);
        } else {
            carregarPaginado(paginaAtual);
        }
    }

    /// Conecta o campo de busca ao carregamento server-side com debounce.
    function inicializarBusca(inputId) {
        document.getElementById(inputId).addEventListener('input', debounce(function () {
            paginaAtual = 1;
            termoBuscaAtual = this.value.trim();
            if (termoBuscaAtual) {
                carregarComBusca(termoBuscaAtual);
            } else {
                carregarPaginado(1);
            }
        }, 300));
    }

    /// Reseta o formulário e abre o modal no modo de criação.
    function abrirModalNovo() {
        edicaoId = null;
        limparFormDirty();
        var form = document.getElementById(config.formId);
        form.reset();
        form.classList.remove('was-validated');
        if (config.tituloModalId) document.getElementById(config.tituloModalId).textContent = config.tituloNovo;
        if (config.erroId) document.getElementById(config.erroId).classList.add('d-none');
        if (config.aoAbrirNovo) config.aoAbrirNovo();
        bootstrap.Modal.getOrCreateInstance(document.getElementById(config.modalId)).show();
    }

    // Inicializar rastreamento de alterações
    inicializarFormDirtyTracking();

    return {
        carregar: carregarPaginado,
        buscar: carregarComBusca,
        salvar: salvar,
        remover: remover,
        prepararEdicao: prepararEdicao,
        recarregarLista: recarregarLista,
        inicializarBusca: inicializarBusca,
        abrirModalNovo: abrirModalNovo,
        getEdicaoId: function () { return edicaoId; },
        setTermoBusca: function (t) { termoBuscaAtual = t; },
        getTermoBusca: function () { return termoBuscaAtual; },
        setPagina: function (p) { paginaAtual = p; }
    };
}

// Registrar no namespace App
App.criarCrud = criarCrud;
