// ==========================================
// Produto/index.js — CRUD de produtos
// ==========================================
(function () {
    'use strict';

    var filtroEspecialAtivo = false;

    /// Cria uma linha da tabela de produtos com SKU, preço, estoque e ações.
    function renderProdutoRow(p, prepararEdicao, remover) {
        var tr = document.createElement('tr');
        var qtdEstoque = p.QuantidadeTotalEstoque || 0;

        var tdSku = document.createElement('td');
        tdSku.appendChild(criarElemento('code', 'fw-bold', p.Sku || '---'));
        tr.appendChild(tdSku);

        tr.appendChild(criarElemento('td', null, p.Nome));
        tr.appendChild(criarElemento('td', null, formatarMoeda(p.Preco)));

        var tdEstoque = document.createElement('td');
        var badgeClass;
        if (qtdEstoque === 0) {
            badgeClass = 'badge bg-danger-subtle text-danger-emphasis';
        } else if (qtdEstoque < ESTOQUE_BAIXO_LIMITE) {
            badgeClass = 'badge bg-warning-subtle text-warning-emphasis';
        } else {
            badgeClass = 'badge bg-secondary-subtle text-secondary-emphasis';
        }
        tdEstoque.appendChild(criarElemento('span', badgeClass, qtdEstoque + ' un.'));
        tr.appendChild(tdEstoque);

        var tdAcoes = criarElemento('td', 'text-center');
        var btnGroup = criarElemento('div', 'd-inline-flex gap-1');
        btnGroup.appendChild(criarBtnAcao('fas fa-eye', 'btn-outline-secondary', 'Detalhes', 'Ver detalhes de ' + p.Nome, function () {
            var html = '<table class="table table-sm mb-0">'
                + '<tr><th>SKU</th><td><code>' + (p.Sku || '—') + '</code></td></tr>'
                + '<tr><th>Nome</th><td>' + p.Nome + '</td></tr>'
                + '<tr><th>Descrição</th><td>' + (p.Descricao || '—') + '</td></tr>'
                + '<tr><th>Preço</th><td>' + formatarMoeda(p.Preco) + '</td></tr>'
                + '<tr><th>Estoque Total</th><td>' + qtdEstoque + ' un.</td></tr>'
                + '</table>';
            showDetailModal('Produto: ' + p.Nome, html);
        }));
        btnGroup.appendChild(criarBtnAcao('fas fa-edit', 'btn-outline-primary', 'Editar', 'Editar produto ' + p.Nome, prepararEdicao.bind(null, p.Id)));
        btnGroup.appendChild(criarBtnAcao('fas fa-trash', 'btn-outline-danger', 'Remover', 'Remover produto ' + p.Nome, remover.bind(null, p.Id)));
        tdAcoes.appendChild(btnGroup);
        tr.appendChild(tdAcoes);
        return tr;
    }

    var crud = criarCrud({
        label: 'Produto',
        endpoint: '/produto',
        tabelaId: 'tabelaProdutos',
        paginacaoId: 'paginacaoProdutos',
        infoId: 'totalProdutosInfo',
        formId: 'formProduto',
        modalId: 'modalNovoProduto',
        erroId: 'modalErro',
        tituloModalId: 'tituloModalProduto',
        btnSalvarId: 'btnSalvarProduto',
        colunas: 5,
        camposBusca: ['Nome', 'Sku', 'Descricao'],
        emptyIconClass: 'fas fa-box-open',
        tituloRemover: 'Remover Produto',
        msgRemover: 'Deseja realmente remover este produto?',
        msgRemovido: 'Produto removido com sucesso.',
        msgErroRemover: 'Erro ao remover produto.',

        renderRow: function (p, prepararEdicao, remover) {
            return renderProdutoRow(p, prepararEdicao, remover);
        },

        preencherForm: function (p) {
            document.getElementById('prodNome').value = p.Nome || '';
            document.getElementById('prodDescricao').value = p.Descricao || '';
            document.getElementById('prodPreco').value = p.Preco || '';
        },

        montarPayload: function () {
            return {
                Nome: document.getElementById('prodNome').value.trim(),
                Descricao: document.getElementById('prodDescricao').value.trim(),
                Preco: parseFloat(document.getElementById('prodPreco').value)
            };
        },

        aoErroSalvar: function (erro, payload, fecharModal, forcarCriacao) {
            if (erro.response && erro.response.status === 409 && erro.response.data && erro.response.data.Registros) {
                var registros = erro.response.data.Registros;
                var selectedId = registros[0].Id;

                function buildDetalhes(r) {
                    return '<div class="small mt-2">'
                        + '<strong>SKU:</strong> ' + r.Sku + '<br>'
                        + '<strong>Nome:</strong> ' + r.Nome + '<br>'
                        + '<strong>Descrição:</strong> ' + (r.Descricao || '—') + '<br>'
                        + '<strong>Preço:</strong> R$ ' + (r.Preco ? r.Preco.toFixed(2) : '0,00')
                        + '</div>';
                }

                var html = '<p>Já existe um produto inativo com este nome:</p>';
                if (registros.length > 1) {
                    html += '<select class="form-select form-select-sm mb-2" id="selRestaurarProduto">';
                    registros.forEach(function (r) {
                        html += '<option value="' + r.Id + '">' + r.Sku + ' — ' + r.Nome + '</option>';
                    });
                    html += '</select>';
                }
                html += '<div id="detalhesRestaurar">' + buildDetalhes(registros[0]) + '</div>';
                html += '<p class="mt-2 mb-0">Deseja restaurá-lo com os novos dados? O SKU original será mantido.</p>';

                showConfirmModal('Restaurar Produto', html, function () {
                    apiPost('/produto/' + selectedId + '/restaurar', payload).then(function () {
                        showToast('Produto restaurado com sucesso!', 'success');
                        fecharModal();
                    }).catch(function (e) {
                        showToast(extrairMsgErro(e, 'Erro ao restaurar produto.'), 'danger');
                    });
                }, 'warning', forcarCriacao, true);

                // Bind dropdown change
                setTimeout(function () {
                    var sel = document.getElementById('selRestaurarProduto');
                    if (sel) {
                        sel.addEventListener('change', function () {
                            selectedId = parseInt(this.value, 10);
                            var r = registros.find(function (x) { return x.Id === selectedId; });
                            document.getElementById('detalhesRestaurar').innerHTML = buildDetalhes(r);
                        });
                    }
                }, 300);
                return true;
            }
            return false;
        }
    });

    var recarregarOriginal = crud.recarregarLista;
    crud.recarregarLista = function () {
        if (filtroEspecialAtivo) {
            carregarProdutosEstoqueBaixo();
        } else {
            recarregarOriginal();
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        var params = new URLSearchParams(window.location.search);
        if (params.get('filtro') === 'estoque-baixo') {
            filtroEspecialAtivo = true;
            carregarProdutosEstoqueBaixo();
        } else {
            crud.carregar(1);
        }

        crud.inicializarBusca('buscaProduto');
        document.getElementById('btnSalvarProduto').addEventListener('click', crud.salvar);
        document.getElementById('btnNovoProduto').addEventListener('click', crud.abrirModalNovo);
    });

    /// Carrega e exibe apenas produtos com estoque abaixo do limite.
    function carregarProdutosEstoqueBaixo() {
        var tbody = document.querySelector('#tabelaProdutos tbody');
        renderizarSkeletonLinhas(tbody, 7);
        apiGet('/produto').then(function (todosProdutos) {
            var filtrados = todosProdutos.filter(function (p) { return (p.QuantidadeTotalEstoque || 0) < ESTOQUE_BAIXO_LIMITE; });
            var resultado = paginar(filtrados, 1);

            var info = document.getElementById('totalProdutosInfo');
            renderizarInfoFiltro(info, 'bg-warning text-dark', 'Estoque Baixo', resultado.totalItens, 'item(ns)', '/Produto/Index');

            tbody.innerHTML = '';
            if (!resultado.dados || resultado.dados.length === 0) {
                renderizarEmptyState(tbody, 7, 'fas fa-box-open', 'Nenhum produto encontrado.');
                document.getElementById('paginacaoProdutos').innerHTML = '';
                return;
            }
            resultado.dados.forEach(function (p) {
                tbody.appendChild(renderProdutoRow(p, crud.prepararEdicao, crud.remover));
            });
        }).catch(function (error) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Erro ao carregar produtos.</td></tr>';
            console.error(error);
        });
    }
})();