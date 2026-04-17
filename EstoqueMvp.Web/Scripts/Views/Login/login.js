// ==========================================
// Login/login.js — Autenticação com CPF e senha
// ==========================================
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var inputCpf = document.getElementById('inputCpf');
        aplicarMascaraCpf(inputCpf);

        document.getElementById('btnLogin').addEventListener('click', fazerLogin);

        // Submeter com Enter
        document.getElementById('inputSenha').addEventListener('keydown', function (e) {
            if (e.key === 'Enter') fazerLogin();
        });
        inputCpf.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') fazerLogin();
        });
    });

    /// Valida CPF e senha, envia credenciais ao backend e redireciona ao dashboard.
    async function fazerLogin() {
        var cpf = document.getElementById('inputCpf').value.replace(/\D/g, '');
        var senha = document.getElementById('inputSenha').value;
        var alertErro = document.getElementById('alertErro');
        var btn = document.getElementById('btnLogin');

        alertErro.classList.add('d-none');

        if (!validarCpf(cpf)) {
            alertErro.textContent = 'Informe um CPF válido.';
            alertErro.classList.remove('d-none');
            return;
        }

        if (!senha || !senha.trim()) {
            alertErro.textContent = 'Informe a senha.';
            alertErro.classList.remove('d-none');
            return;
        }

        setBtnLoading(btn, true);

        try {
            var response = await apiPost('/usuario/login', {
                Cpf: cpf,
                Senha: senha
            });

            // O backend emite o JWT em cookie httpOnly. Apenas redireciona.
            window.location.href = '/';
        } catch (error) {
            alertErro.textContent = 'CPF ou Senha inválidos!';
            alertErro.classList.remove('d-none');
        } finally {
            setBtnLoading(btn, false);
        }
    }
})();
