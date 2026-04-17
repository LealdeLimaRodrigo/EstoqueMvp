// ==========================================
// formatters.js — Formatação, máscara e validação
// ==========================================

/// Formata data/hora no padrão brasileiro (dd/mm/aaaa hh:mm:ss).
function formatarData(valor) {
    if (!valor) return '-';
    return new Date(valor).toLocaleString('pt-BR');
}

/// Formata data no padrão ISO curto (yyyy-MM-dd).
function formatarDataCurta(date) {
    var d = new Date(date);
    return d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0');
}

/// Formata valor numérico como moeda brasileira (R$ 0,00).
function formatarMoeda(valor) {
    var num = parseFloat(valor) || 0;
    return 'R$ ' + num.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

/// Formata CPF com pontuação (000.000.000-00).
function formatarCpf(cpf) {
    if (!cpf) return '-';
    cpf = String(cpf).replace(/\D/g, '');
    if (cpf.length !== 11) return cpf;
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
}

/// Aplica máscara de CPF em tempo real no campo de input.
function aplicarMascaraCpf(input) {
    input.addEventListener('input', function () {
        var v = this.value.replace(/\D/g, '').substring(0, 11);
        if (v.length > 9) {
            this.value = v.replace(/(\d{3})(\d{3})(\d{3})(\d{1,2})/, '$1.$2.$3-$4');
        } else if (v.length > 6) {
            this.value = v.replace(/(\d{3})(\d{3})(\d{1,3})/, '$1.$2.$3');
        } else if (v.length > 3) {
            this.value = v.replace(/(\d{3})(\d{1,3})/, '$1.$2');
        } else {
            this.value = v;
        }
    });
}

/// Valida CPF usando o algoritmo oficial dos dígitos verificadores.
/// Permite o CPF de teste 00000000000 (admin padrão do MVP).
function validarCpf(cpf) {
    cpf = String(cpf).replace(/\D/g, '');
    if (cpf.length !== 11) return false;
    if (cpf === '00000000000') return true;
    if (/^(\d)\1{10}$/.test(cpf)) return false;

    var soma = 0;
    for (var i = 0; i < 9; i++) soma += parseInt(cpf.charAt(i), 10) * (10 - i);
    var resto = (soma * 10) % 11;
    if (resto === 10) resto = 0;
    if (resto !== parseInt(cpf.charAt(9), 10)) return false;

    soma = 0;
    for (var j = 0; j < 10; j++) soma += parseInt(cpf.charAt(j), 10) * (11 - j);
    resto = (soma * 10) % 11;
    if (resto === 10) resto = 0;
    if (resto !== parseInt(cpf.charAt(10), 10)) return false;

    return true;
}

/// Parseia data no formato brasileiro (dd/MM/yyyy HH:mm:ss) retornando um Date local.
/// Aceita também formato ISO (yyyy-MM-ddTHH:mm:ss) como fallback.
function parseDateBR(valor) {
    if (!valor) return new Date(NaN);
    var s = String(valor);
    // Formato dd/MM/yyyy HH:mm:ss
    var m = s.match(/^(\d{2})\/(\d{2})\/(\d{4})\s+(\d{2}):(\d{2}):(\d{2})$/);
    if (m) return new Date(+m[3], +m[2] - 1, +m[1], +m[4], +m[5], +m[6]);
    // Formato dd/MM/yyyy
    var m2 = s.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
    if (m2) return new Date(+m2[3], +m2[2] - 1, +m2[1]);
    // Fallback ISO
    return new Date(s);
}

// Registrar no namespace App
App.formatarData = formatarData;
App.formatarDataCurta = formatarDataCurta;
App.formatarMoeda = formatarMoeda;
App.formatarCpf = formatarCpf;
App.aplicarMascaraCpf = aplicarMascaraCpf;
App.validarCpf = validarCpf;
App.parseDateBR = parseDateBR;
