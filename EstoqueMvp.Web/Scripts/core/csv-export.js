// ==========================================
// csv-export.js — Exportação CSV genérica
// ==========================================

/// Gera e faz download de um arquivo CSV a partir de cabeçalhos e linhas.
function exportarParaCsv(nomeArquivo, cabecalhos, linhas, separador) {
    separador = separador || ';';

    var conteudo = [cabecalhos.join(separador)];

    linhas.forEach(function (campos) {
        var linha = campos.map(function (v) {
            return '"' + String(v).replace(/"/g, '""') + '"';
        }).join(separador);
        conteudo.push(linha);
    });

    var bom = '\uFEFF';
    var blob = new Blob([bom + conteudo.join('\r\n')], { type: 'text/csv;charset=utf-8;' });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = nomeArquivo;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

// Registrar no namespace App
App.exportarParaCsv = exportarParaCsv;
