// ==========================================
// Setor/index.js — CRUD de setores
// ==========================================
(function () {
    'use strict';

    var crud = criarCrud({
        label: 'Setor',
        endpoint: '/setor',
        tabelaId: 'tabelaSetores',
        paginacaoId: 'paginacaoSetores',
        infoId: 'totalSetoresInfo',
        formId: 'formSetor',
        modalId: 'modalSetor',
        erroId: 'modalSetorErro',
        tituloModalId: 'tituloModalSetor',
        btnSalvarId: 'btnSalvarSetor',
        colunas: 2,
        camposBusca: ['Nome'],
        emptyIconClass: 'fas fa-building',

        renderRow: function (s, prepararEdicao, remover) {
            var tr = document.createElement('tr');
            tr.appendChild(criarElemento('td', null, s.Nome));

            var tdAcoes = criarElemento('td', 'text-center');
            var btnGroup = criarElemento('div', 'd-inline-flex gap-1');
            btnGroup.appendChild(criarBtnAcao('fas fa-eye', 'btn-outline-secondary', 'Detalhes', 'Ver detalhes de ' + s.Nome, function () {
                var html = '<table class="table table-sm mb-0">'
                    + '<tr><th>Nome</th><td>' + s.Nome + '</td></tr>'
                    + '</table>';
                showDetailModal('Setor: ' + s.Nome, html);
            }));
            btnGroup.appendChild(criarBtnAcao('fas fa-edit', 'btn-outline-primary', 'Editar', 'Editar setor ' + s.Nome, prepararEdicao.bind(null, s.Id)));
            btnGroup.appendChild(criarBtnAcao('fas fa-trash', 'btn-outline-danger', 'Remover', 'Remover setor ' + s.Nome, remover.bind(null, s.Id)));
            tdAcoes.appendChild(btnGroup);
            tr.appendChild(tdAcoes);
            return tr;
        },

        preencherForm: function (s) {
            document.getElementById('setNome').value = s.Nome || '';
        },

        montarPayload: function () {
            return { Nome: document.getElementById('setNome').value.trim() };
        },

        aoErroSalvar: function (erro, payload, fecharModal, forcarCriacao) {
            if (erro.response && erro.response.status === 409 && erro.response.data && erro.response.data.Registros) {
                var registros = erro.response.data.Registros;
                var selectedId = registros[0].Id;

                var html = '<p>Já existe um setor inativo com este nome:</p>';
                if (registros.length > 1) {
                    html += '<select class="form-select form-select-sm mb-2" id="selRestaurarSetor">';
                    registros.forEach(function (r) {
                        html += '<option value="' + r.Id + '">' + r.Nome + ' (ID: ' + r.Id + ')</option>';
                    });
                    html += '</select>';
                } else {
                    html += '<div class="small"><strong>Nome:</strong> ' + registros[0].Nome + '</div>';
                }
                html += '<p class="mt-2 mb-0">Deseja restaurá-lo?</p>';

                showConfirmModal('Restaurar Setor', html, function () {
                    apiPost('/setor/' + selectedId + '/restaurar', payload).then(function () {
                        showToast('Setor restaurado com sucesso!', 'success');
                        fecharModal();
                    }).catch(function (e) {
                        showToast(extrairMsgErro(e, 'Erro ao restaurar setor.'), 'danger');
                    });
                }, 'warning', forcarCriacao, true);

                setTimeout(function () {
                    var sel = document.getElementById('selRestaurarSetor');
                    if (sel) {
                        sel.addEventListener('change', function () { selectedId = parseInt(this.value, 10); });
                    }
                }, 300);
                return true;
            }
            return false;
        }
    });

    document.addEventListener('DOMContentLoaded', function () {
        crud.carregar(1);
        crud.inicializarBusca('buscaSetor');
        document.getElementById('btnNovoSetor').addEventListener('click', crud.abrirModalNovo);
        document.getElementById('btnSalvarSetor').addEventListener('click', crud.salvar);
    });
})();
