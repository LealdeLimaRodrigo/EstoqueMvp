// ==========================================
// app.js — Bootstrap da aplicação (auth, tema, navbar)
// ==========================================

// ==========================================\n// 1. TEMA (aplicar antes do DOM carregar)\n// ==========================================
(function () {
    var tema = localStorage.getItem('tema') || 'light';
    document.documentElement.setAttribute('data-bs-theme', tema);
})();

// ==========================================
// 2. GUARDA DE ROTA GLOBAL
// ==========================================
var paginaAtual = window.location.pathname.toLowerCase();
var estaNoLogin = paginaAtual.includes('/login');

// Esconder conteúdo até confirmar autenticação (evita flash de tela protegida)
if (!estaNoLogin) {
    document.documentElement.classList.add('auth-pending');
}

// Autenticação via cookie httpOnly: o JWT é enviado automaticamente pelo navegador.
// O frontend não armazena token no localStorage.
// Rotas protegidas retornam 401 se não autenticado.

// ==========================================
// 3. INTERCEPTORS AXIOS
// ==========================================

// O cookie httpOnly é enviado automaticamente pelo navegador.

axios.interceptors.response.use(function (response) { return response; }, function (error) {
    if (error.response && error.response.status === 401 && !estaNoLogin) {
        window.location.href = '/Login';
    }
    return Promise.reject(error);
});

// Verificação rápida de autenticação para desbloquear a tela
if (!estaNoLogin) {
    axios.get(API_URL + '/usuario/perfil').then(function () {
        document.documentElement.classList.remove('auth-pending');
    }).catch(function () {
        window.location.href = '/Login';
    });
} 

// ==========================================
// 4. LOGOUT
// ==========================================
function logout() {
    // Faz logout no backend, removendo o cookie httpOnly
    axios.post(API_URL + '/usuario/logout').finally(function () {
        window.location.href = '/Login';
    });
}

// ==========================================
// 5. NAVBAR + TEMA + CONFIRMAÇÃO
// ==========================================
document.addEventListener('DOMContentLoaded', function () {
    // Botão sair — sempre visível (se não está no login)
    if (!estaNoLogin) {
        var btnSair = document.getElementById('btnSair');
        if (btnSair) {
            btnSair.classList.remove('d-none');
            btnSair.addEventListener('click', logout);
        }

        // Buscar nome do usuário autenticado via endpoint de perfil
        axios.get(API_URL + '/usuario/perfil').then(function (response) {
            var nome = response.data && response.data.Nome;
            if (nome) {
                var saudacao = document.getElementById('saudacaoUsuario');
                var spanNome = document.getElementById('nomeUsuario');
                if (saudacao && spanNome) {
                    spanNome.textContent = 'Olá, ' + nome;
                    saudacao.classList.remove('d-none');
                }
            }
        }).catch(function () {
            // Silencia — se não autenticado, o interceptor 401 redireciona
        });
    }

    // Marcar nav-link ativo na sidebar
    var navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    for (var i = 0; i < navLinks.length; i++) {
        var href = navLinks[i].getAttribute('href');
        if (href && paginaAtual === href.toLowerCase()) {
            navLinks[i].classList.add('active');
            navLinks[i].setAttribute('aria-current', 'page');
        }
    }

    // Bind do modal de confirmação global
    _initConfirmModal();

    // Segurança: limpar dados sensíveis ao fechar modais
    document.querySelectorAll('.modal').forEach(function (modal) {
        modal.addEventListener('hidden.bs.modal', function () {
            var senhaInputs = modal.querySelectorAll('input[type="password"]');
            for (var i = 0; i < senhaInputs.length; i++) {
                senhaInputs[i].value = '';
            }
        });
    });

    // Toggle dark mode
    var btnTema = document.getElementById('btnTema');
    if (btnTema) {
        atualizarIconeTema(btnTema);
        btnTema.addEventListener('click', function () {
            var atual = document.documentElement.getAttribute('data-bs-theme');
            var novo = atual === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-bs-theme', novo);
            localStorage.setItem('tema', novo);
            atualizarIconeTema(btnTema);
        });
    }
});

/// Atualiza o ícone do botão de tema (sol/lua).
function atualizarIconeTema(btn) {
    var tema = document.documentElement.getAttribute('data-bs-theme');
    var icon = btn.querySelector('i');
    if (icon) {
        icon.className = tema === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    }
}

// ==========================================
// 6. PROTEÇÃO GLOBAL — beforeunload
// Previne perda de dados com modais abertos
// ==========================================
window.addEventListener('beforeunload', function (e) {
    var modaisAbertos = document.querySelectorAll('.modal.show .form-dirty, .modal.show');
    for (var i = 0; i < modaisAbertos.length; i++) {
        var inputs = modaisAbertos[i].querySelectorAll('input, textarea, select');
        for (var j = 0; j < inputs.length; j++) {
            if (inputs[j].value && inputs[j].value.trim()) {
                e.preventDefault();
                return;
            }
        }
    }
});