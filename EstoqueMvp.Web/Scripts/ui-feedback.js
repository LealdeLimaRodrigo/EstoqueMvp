// ==========================================
// ui-feedback.js — Componentes de feedback visual (toast, loading, confirmação)
// ==========================================

/// Exibe uma notificação toast não-bloqueante com ícone e auto-dismiss.
function showToast(mensagem, tipo) {
    tipo = tipo || 'success';
    var container = document.getElementById('toastContainer');

    var iconMap = {
        success: 'fas fa-check-circle',
        danger: 'fas fa-exclamation-circle',
        warning: 'fas fa-exclamation-triangle',
        info: 'fas fa-info-circle'
    };

    var toastEl = document.createElement('div');
    toastEl.className = 'toast align-items-center text-bg-' + tipo + ' border-0';
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');

    var flexDiv = document.createElement('div');
    flexDiv.className = 'd-flex';

    var body = document.createElement('div');
    body.className = 'toast-body';

    var icon = document.createElement('i');
    icon.className = (iconMap[tipo] || iconMap.info) + ' me-2';
    body.appendChild(icon);

    var span = document.createElement('span');
    span.textContent = mensagem;
    body.appendChild(span);

    var btnClose = document.createElement('button');
    btnClose.type = 'button';
    btnClose.className = 'btn-close btn-close-white me-2 m-auto';
    btnClose.setAttribute('data-bs-dismiss', 'toast');
    btnClose.setAttribute('aria-label', 'Fechar');

    flexDiv.appendChild(body);
    flexDiv.appendChild(btnClose);
    toastEl.appendChild(flexDiv);
    container.appendChild(toastEl);

    var toast = new bootstrap.Toast(toastEl, { delay: 4000 });
    toast.show();

    toastEl.addEventListener('hidden.bs.toast', function () {
        toastEl.remove();
    });
}

/// Alterna o estado de loading de um botão (spinner + disabled).
function setBtnLoading(btn, loading) {
    if (!btn) return;
    if (loading) {
        btn.disabled = true;
        btn.setAttribute('data-original-text', btn.innerHTML);
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>Aguarde...';
    } else {
        btn.disabled = false;
        var original = btn.getAttribute('data-original-text');
        if (original) btn.innerHTML = original;
    }
}

var _confirmCallback = null;
var _confirmRejectCallback = null;

/// Exibe o modal de confirmação global com título, mensagem, callback e tipo.
/// Se onReject for fornecido, exibe o botão "Não, criar novo".
function showConfirmModal(titulo, mensagem, callback, tipo, onReject, useHtml) {
    _confirmCallback = callback;
    _confirmRejectCallback = onReject || null;
    document.getElementById('confirmModalTitle').textContent = titulo;
    if (useHtml) {
        document.getElementById('confirmModalBody').innerHTML = mensagem;
    } else {
        document.getElementById('confirmModalBody').innerText = mensagem;
    }
    var header = document.querySelector('#modalConfirmacao .modal-header');
    var btnSim = document.getElementById('btnConfirmSim');
    var btnNao = document.getElementById('btnConfirmNao');
    if (tipo === 'warning') {
        header.className = 'modal-header bg-warning text-dark';
        btnSim.className = 'btn btn-warning';
    } else {
        header.className = 'modal-header bg-danger text-white';
        btnSim.className = 'btn btn-danger';
    }
    if (onReject) {
        btnNao.classList.remove('d-none');
    } else {
        btnNao.classList.add('d-none');
    }
    bootstrap.Modal.getOrCreateInstance(document.getElementById('modalConfirmacao')).show();
}

/// Inicializa o evento de confirmação do modal global.
function _initConfirmModal() {
    var btnConfirmSim = document.getElementById('btnConfirmSim');
    if (btnConfirmSim) {
        btnConfirmSim.addEventListener('click', function () {
            bootstrap.Modal.getInstance(document.getElementById('modalConfirmacao')).hide();
            if (typeof _confirmCallback === 'function') {
                var cb = _confirmCallback;
                _confirmCallback = null;
                _confirmRejectCallback = null;
                cb();
            }
        });
    }
    var btnConfirmNao = document.getElementById('btnConfirmNao');
    if (btnConfirmNao) {
        btnConfirmNao.addEventListener('click', function () {
            bootstrap.Modal.getInstance(document.getElementById('modalConfirmacao')).hide();
            if (typeof _confirmRejectCallback === 'function') {
                var cb = _confirmRejectCallback;
                _confirmCallback = null;
                _confirmRejectCallback = null;
                cb();
            }
        });
    }
}

/// Exibe o modal de detalhes global com título e conteúdo HTML.
function showDetailModal(titulo, htmlContent) {
    document.getElementById('detalhesModalTitle').innerHTML = '<i class="fas fa-eye me-1"></i> ' + titulo;
    document.getElementById('detalhesModalBody').innerHTML = htmlContent;
    bootstrap.Modal.getOrCreateInstance(document.getElementById('modalDetalhes')).show();
}

// Registrar no namespace App
App.showToast = showToast;
App.setBtnLoading = setBtnLoading;
App.showConfirmModal = showConfirmModal;
App.showDetailModal = showDetailModal;
App._initConfirmModal = _initConfirmModal;
