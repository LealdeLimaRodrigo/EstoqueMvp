// ==========================================
// dom-utils.js — Utilitários DOM genéricos
// ==========================================

/// Cria um elemento HTML com classes CSS e texto opcionais.
function criarElemento(tag, classes, texto) {
    var el = document.createElement(tag);
    if (classes) el.className = classes;
    if (texto !== undefined && texto !== null) el.textContent = String(texto);
    return el;
}

/// Cria um botão de ação com ícone, tooltip e evento de clique.
function criarBtnAcao(icon, classe, titulo, ariaLabel, onClick) {
    var btn = document.createElement('button');
    btn.className = 'btn btn-sm ' + classe;
    btn.title = titulo;
    btn.setAttribute('aria-label', ariaLabel);
    btn.innerHTML = '<i class="' + icon + '"></i>';
    btn.addEventListener('click', onClick);
    return btn;
}

/// Retorna uma função com atraso (debounce) para limitar execuções frequentes.
function debounce(fn, ms) {
    var timer;
    return function () {
        clearTimeout(timer);
        timer = setTimeout(fn.bind(this), ms);
    };
}

/// Renderiza linhas de skeleton loading animadas na tabela.
function renderizarSkeletonLinhas(tbody, colunas, linhas) {
    linhas = linhas || 5;
    tbody.innerHTML = '';
    for (var i = 0; i < linhas; i++) {
        var tr = document.createElement('tr');
        for (var c = 0; c < colunas; c++) {
            var td = document.createElement('td');
            var span = criarElemento('span', 'placeholder-wave');
            var placeholder = criarElemento('span', 'placeholder col-' + (4 + Math.floor(Math.random() * 5)));
            span.appendChild(placeholder);
            td.appendChild(span);
            tr.appendChild(td);
        }
        tbody.appendChild(tr);
    }
}

/// Renderiza estado vazio com ícone e mensagem na tabela.
function renderizarEmptyState(tbody, colunas, iconClass, mensagem, actionHref, actionLabel) {
    var tr = document.createElement('tr');
    var td = document.createElement('td');
    td.colSpan = colunas;

    var wrapper = criarElemento('div', 'empty-state');

    var icon = document.createElement('div');
    icon.className = 'empty-state-icon';
    var iconEl = document.createElement('i');
    iconEl.className = iconClass || 'fas fa-inbox';
    icon.appendChild(iconEl);
    wrapper.appendChild(icon);

    wrapper.appendChild(criarElemento('p', 'empty-state-text', mensagem || 'Nenhum registro encontrado.'));

    if (actionHref && actionLabel) {
        var actionDiv = criarElemento('div', 'empty-state-action');
        var link = criarElemento('a', 'btn btn-sm btn-outline-primary', actionLabel);
        link.href = actionHref;
        actionDiv.appendChild(link);
        wrapper.appendChild(actionDiv);
    }

    td.appendChild(wrapper);
    tr.appendChild(td);
    tbody.innerHTML = '';
    tbody.appendChild(tr);
}

// Registrar no namespace App
App.criarElemento = criarElemento;
App.criarBtnAcao = criarBtnAcao;
App.debounce = debounce;
App.renderizarSkeletonLinhas = renderizarSkeletonLinhas;
App.renderizarEmptyState = renderizarEmptyState;
