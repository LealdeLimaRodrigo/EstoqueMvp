// ==========================================
// pagination.js — Motor de paginação e filtro
// ==========================================

var ITENS_POR_PAGINA = 10;

/// Pagina um array localmente, retornando os itens da página solicitada.
function paginar(itens, pagina) {
    var total = itens.length;
    var totalPaginas = Math.max(1, Math.ceil(total / ITENS_POR_PAGINA));
    pagina = Math.max(1, Math.min(pagina, totalPaginas));
    var inicio = (pagina - 1) * ITENS_POR_PAGINA;
    return {
        dados: itens.slice(inicio, inicio + ITENS_POR_PAGINA),
        paginaAtual: pagina,
        totalPaginas: totalPaginas,
        totalItens: total
    };
}

/// Renderiza os controles de paginação com janela deslizante.
function renderizarPaginacao(containerId, paginaAtual, totalPaginas, onPageChange) {
    var container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = '';

    if (totalPaginas <= 1) return;

    var nav = document.createElement('nav');
    nav.setAttribute('aria-label', 'Paginação');
    var ul = criarElemento('ul', 'pagination pagination-sm justify-content-center mb-0');

    // Anterior
    var liPrev = criarElemento('li', 'page-item' + (paginaAtual <= 1 ? ' disabled' : ''));
    var aPrev = criarElemento('a', 'page-link', '‹');
    aPrev.href = '#';
    aPrev.setAttribute('aria-label', 'Página anterior');
    if (paginaAtual > 1) {
        aPrev.addEventListener('click', function (e) { e.preventDefault(); onPageChange(paginaAtual - 1); });
    }
    liPrev.appendChild(aPrev);
    ul.appendChild(liPrev);

    // Páginas com janela deslizante
    var inicio = Math.max(1, paginaAtual - 2);
    var fim = Math.min(totalPaginas, paginaAtual + 2);
    if (inicio > 1) {
        ul.appendChild(criarPaginaItem(1, paginaAtual, onPageChange));
        if (inicio > 2) {
            var liEllipsis = criarElemento('li', 'page-item disabled');
            liEllipsis.appendChild(criarElemento('span', 'page-link', '…'));
            ul.appendChild(liEllipsis);
        }
    }
    for (var p = inicio; p <= fim; p++) {
        ul.appendChild(criarPaginaItem(p, paginaAtual, onPageChange));
    }
    if (fim < totalPaginas) {
        if (fim < totalPaginas - 1) {
            var liEllipsis2 = criarElemento('li', 'page-item disabled');
            liEllipsis2.appendChild(criarElemento('span', 'page-link', '…'));
            ul.appendChild(liEllipsis2);
        }
        ul.appendChild(criarPaginaItem(totalPaginas, paginaAtual, onPageChange));
    }

    // Próximo
    var liNext = criarElemento('li', 'page-item' + (paginaAtual >= totalPaginas ? ' disabled' : ''));
    var aNext = criarElemento('a', 'page-link', '›');
    aNext.href = '#';
    aNext.setAttribute('aria-label', 'Próxima página');
    if (paginaAtual < totalPaginas) {
        aNext.addEventListener('click', function (e) { e.preventDefault(); onPageChange(paginaAtual + 1); });
    }
    liNext.appendChild(aNext);
    ul.appendChild(liNext);

    nav.appendChild(ul);
    container.appendChild(nav);
}

/// Cria um item de página individual para o componente de paginação.
function criarPaginaItem(numero, paginaAtual, onPageChange) {
    var li = criarElemento('li', 'page-item' + (numero === paginaAtual ? ' active' : ''));
    var a = criarElemento('a', 'page-link', numero);
    a.href = '#';
    if (numero === paginaAtual) {
        a.setAttribute('aria-current', 'page');
    }
    a.addEventListener('click', function (e) { e.preventDefault(); onPageChange(numero); });
    li.appendChild(a);
    return li;
}

/// Filtra um array de objetos por termo textual nos campos especificados.
function filtrarItens(itens, termo, campos) {
    if (!termo || !termo.trim()) return itens;
    var termoLower = termo.toLowerCase().trim();
    return itens.filter(function (item) {
        for (var i = 0; i < campos.length; i++) {
            var valor = item[campos[i]];
            if (valor !== undefined && valor !== null && String(valor).toLowerCase().indexOf(termoLower) !== -1) {
                return true;
            }
        }
        return false;
    });
}

/// Renderiza badge informativo de filtro ativo com link para limpar.
function renderizarInfoFiltro(container, badgeClass, label, totalItens, sufixo, limparHref) {
    container.textContent = '';
    var badge = criarElemento('span', 'badge ' + badgeClass);
    var icon = criarElemento('i', 'fas fa-filter me-1');
    badge.appendChild(icon);
    badge.appendChild(document.createTextNode(label));
    container.appendChild(badge);
    container.appendChild(document.createTextNode(' ' + totalItens + ' ' + sufixo + ' '));
    var link = criarElemento('a', 'ms-2 small', 'Limpar filtro');
    link.href = limparHref;
    container.appendChild(link);
}

// Registrar no namespace App
App.ITENS_POR_PAGINA = ITENS_POR_PAGINA;
App.paginar = paginar;
App.renderizarPaginacao = renderizarPaginacao;
App.criarPaginaItem = criarPaginaItem;
App.filtrarItens = filtrarItens;
App.renderizarInfoFiltro = renderizarInfoFiltro;
