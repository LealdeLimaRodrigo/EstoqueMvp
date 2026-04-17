// ==========================================
// Usuario/index.js — CRUD de usuários
// ==========================================
(function () {
    'use strict';

    var crud = criarCrud({
        label: 'Usuário',
        endpoint: '/usuario',
        tabelaId: 'tabelaUsuarios',
        paginacaoId: 'paginacaoUsuarios',
        infoId: 'totalUsuariosInfo',
        formId: 'formUsuario',
        modalId: 'modalUsuario',
        erroId: 'modalUsuarioErro',
        tituloModalId: 'tituloModalUsuario',
        btnSalvarId: 'btnSalvarUsuario',
        colunas: 3,
        camposBusca: ['Nome', 'Cpf'],
        emptyIconClass: 'fas fa-users',
        tituloRemover: 'Remover Usuário',
        msgRemover: 'Deseja realmente remover este usuário?',
        msgRemovido: 'Usuário removido com sucesso.',
        msgErroRemover: 'Erro ao remover usuário.',

        renderRow: function (u, prepararEdicao, remover) {
            var tr = document.createElement('tr');
            tr.appendChild(criarElemento('td', null, u.Nome));

            var tdCpf = document.createElement('td');
            tdCpf.appendChild(criarElemento('code', 'fw-bold', formatarCpf(u.Cpf)));
            tr.appendChild(tdCpf);

            var tdAcoes = criarElemento('td', 'text-center');
            var btnGroup = criarElemento('div', 'd-inline-flex gap-1');
            btnGroup.appendChild(criarBtnAcao('fas fa-eye', 'btn-outline-secondary', 'Detalhes', 'Ver detalhes de ' + u.Nome, function () {
                var html = '<table class="table table-sm mb-0">'
                    + '<tr><th>Nome</th><td>' + u.Nome + '</td></tr>'
                    + '<tr><th>CPF</th><td><code>' + formatarCpf(u.Cpf) + '</code></td></tr>'
                    + '</table>';
                showDetailModal('Usuário: ' + u.Nome, html);
            }));
            btnGroup.appendChild(criarBtnAcao('fas fa-edit', 'btn-outline-primary', 'Editar', 'Editar usuário ' + u.Nome, prepararEdicao.bind(null, u.Id)));
            btnGroup.appendChild(criarBtnAcao('fas fa-trash', 'btn-outline-danger', 'Remover', 'Remover usuário ' + u.Nome, remover.bind(null, u.Id)));
            tdAcoes.appendChild(btnGroup);
            tr.appendChild(tdAcoes);
            return tr;
        },

        preencherForm: function (u) {
            document.getElementById('usuNome').value = u.Nome || '';
            document.getElementById('usuCpf').value = formatarCpf(u.Cpf);
            document.getElementById('usuCpf').readOnly = true;
            document.getElementById('usuSenha').value = '';
            document.getElementById('usuSenha').required = false;
            document.getElementById('senhaHint').classList.remove('d-none');
        },

        montarPayload: function () {
            return {
                Nome: document.getElementById('usuNome').value.trim(),
                Cpf: document.getElementById('usuCpf').value.replace(/\D/g, ''),
                Senha: document.getElementById('usuSenha').value
            };
        },

        antesValidar: function (edicaoId) {
            var cpfInput = document.getElementById('usuCpf');
            var cpfFeedback = document.getElementById('cpfFeedback');
            var senhaInput = document.getElementById('usuSenha');

            cpfInput.setCustomValidity('');
            var cpfNumeros = cpfInput.value.replace(/\D/g, '');
            if (!validarCpf(cpfNumeros)) {
                cpfInput.setCustomValidity('CPF inválido');
                cpfFeedback.textContent = 'CPF inválido. Verifique os dígitos.';
            }

            if (!edicaoId && senhaInput.value.length < 6) {
                senhaInput.setCustomValidity('Senha muito curta');
            } else {
                senhaInput.setCustomValidity('');
            }
        },

        aoAbrirNovo: function () {
            document.getElementById('usuSenha').required = true;
            document.getElementById('senhaHint').classList.add('d-none');
            document.getElementById('usuCpf').readOnly = false;
        },

        antesEnviar: function (payload, edicaoId) {
            if (edicaoId && !payload.Senha) delete payload.Senha;
            return payload;
        },

        aoErroSalvar: function (erro, payload, fecharModal, forcarCriacao) {
            if (erro.response && erro.response.status === 409 && erro.response.data && erro.response.data.UsuarioInativoId) {
                var d = erro.response.data;
                var cpfFmt = d.Cpf ? formatarCpf(d.Cpf) : 'N/A';
                showConfirmModal(
                    'Restaurar Usuário',
                    'Já existe um usuário inativo com este CPF:\n\n• Nome: ' + (d.Nome || 'N/A') + '\n• CPF: ' + cpfFmt + '\n\nDeseja restaurá-lo com os novos dados informados?',
                    function () {
                        apiPost('/usuario/' + d.UsuarioInativoId + '/restaurar', payload).then(function () {
                            showToast('Usuário restaurado com sucesso!', 'success');
                            fecharModal();
                        }).catch(function (e) {
                            showToast(extrairMsgErro(e, 'Erro ao restaurar usuário.'), 'danger');
                        });
                    },
                    'warning',
                    forcarCriacao
                );
                return true;
            }
            return false;
        }
    });

    document.addEventListener('DOMContentLoaded', function () {
        crud.carregar(1);
        crud.inicializarBusca('buscaUsuario');
        aplicarMascaraCpf(document.getElementById('usuCpf'));
        document.getElementById('btnNovoUsuario').addEventListener('click', crud.abrirModalNovo);
        document.getElementById('btnSalvarUsuario').addEventListener('click', crud.salvar);
    });
})();
