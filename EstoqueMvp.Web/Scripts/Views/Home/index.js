// ==========================================
// Home/index.js — Dashboard: KPIs, gráfico e últimas movimentações
// ==========================================
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        inicializarCardsClickable();
        inicializarDashboard();
    });

    /// Habilita navegação por clique e teclado nos cards do dashboard.
    function inicializarCardsClickable() {
        var cards = document.querySelectorAll('.card-clickable[data-href]');
        for (var i = 0; i < cards.length; i++) {
            (function (card) {
                var navegar = function () { window.location.href = card.getAttribute('data-href'); };
                card.addEventListener('click', navegar);
                card.addEventListener('keydown', function (e) {
                    if (e.key === 'Enter' || e.key === ' ') {
                        e.preventDefault();
                        navegar();
                    }
                });
            })(cards[i]);
        }
    }

    /// Carrega dados de produtos e movimentações e inicializa o dashboard.
    async function inicializarDashboard() {
        var produtos = [];
        var movimentacoes = [];

        try {
            produtos = await dataStore.carregar('produtos', '/produto');
            var produtosInativos = await apiGet('/produto/inativos');
            dataStore.definir('todosProdutos', produtos.concat(produtosInativos || []));
        } catch (e) {
            console.error('Erro Produtos:', e);
            document.getElementById('cardTotalProdutos').textContent = '!';
            document.getElementById('cardEstoqueBaixo').textContent = '!';
        }

        try {
            await dataStore.carregar('setores', '/setor');
            var setoresInativos = await apiGet('/setor/inativos');
            dataStore.definir('todosSetores', dataStore.obter('setores').concat(setoresInativos || []));
        } catch (e) { console.warn('Erro Setores:', e); }

        try {
            movimentacoes = await apiGet('/movimentacao-estoque');
        } catch (e) {
            console.warn('Erro Movimentações:', e);
            document.getElementById('cardEntradasMes').textContent = '!';
            document.getElementById('cardSaidasMes').textContent = '!';
        }

        atualizarCards(produtos, movimentacoes);
        renderizarGraficoReal(movimentacoes);
        renderizarUltimasMovimentacoes(movimentacoes);
    }

    /// Anima a contagem numérica de 0 até o valor final com easing.
    function animarContagem(el, valorFinal, duracao) {
        duracao = duracao || 600;
        var inicio = null;
        var valorInicial = 0;
        function step(timestamp) {
            if (!inicio) inicio = timestamp;
            var progresso = Math.min((timestamp - inicio) / duracao, 1);
            var eased = 1 - Math.pow(1 - progresso, 3);
            el.textContent = Math.floor(eased * valorFinal);
            if (progresso < 1) requestAnimationFrame(step);
        }
        if (valorFinal === 0) { el.textContent = '0'; return; }
        requestAnimationFrame(step);
    }

    /// Calcula e exibe os KPIs nos cards (total, estoque baixo, entradas, saídas).
    function atualizarCards(produtos, movimentacoes) {
        animarContagem(document.getElementById('cardTotalProdutos'), produtos.length);

        var baixo = produtos.filter(function (p) { return (p.QuantidadeTotalEstoque || 0) < ESTOQUE_BAIXO_LIMITE; }).length;
        animarContagem(document.getElementById('cardEstoqueBaixo'), baixo);

        var agora = new Date();
        var mesAtual = agora.getMonth();
        var anoAtual = agora.getFullYear();
        var movDoMes = movimentacoes.filter(function (m) {
            var raw = m.DataMovimentacao;
            if (!raw) return false;
            var d = new Date(raw);
            return d.getMonth() === mesAtual && d.getFullYear() === anoAtual;
        });

        var entradas = movDoMes.filter(function (m) {
            return m.TipoMovimentacaoId === TIPO_ENTRADA;
        }).length;
        animarContagem(document.getElementById('cardEntradasMes'), entradas);

        var consumos = movDoMes.filter(function (m) {
            return m.TipoMovimentacaoId === TIPO_CONSUMO;
        }).length;
        animarContagem(document.getElementById('cardConsumosMes'), consumos);

        var transferencias = movDoMes.filter(function (m) {
            return m.TipoMovimentacaoId === TIPO_ENVIO;
        }).length;
        animarContagem(document.getElementById('cardTransferenciasMes'), transferencias);
    }

    /// Renderiza o gráfico de movimentações dos últimos 7 dias com Chart.js.
    function renderizarGraficoReal(movimentacoes) {
        var canvas = document.getElementById('graficoMovimentacao');
        if (!canvas) return;

        if (typeof Chart === 'undefined') {
            canvas.parentElement.innerHTML = '<div class="d-flex align-items-center justify-content-center h-100 text-muted"><i class="fas fa-chart-line fa-2x me-2 opacity-50"></i> Não foi possível carregar o gráfico.</div>';
            return;
        }

        var ctx = canvas.getContext('2d');
        if (window.meuGrafico) window.meuGrafico.destroy();

        // Gerar chaves YYYY-MM-DD e labels de dia da semana para os últimos 7 dias
        var chavesDias = [];
        var labelsDias = [];
        for (var i = 0; i < 7; i++) {
            var d = new Date();
            d.setDate(d.getDate() - (6 - i));
            chavesDias.push(formatarDataCurta(d));
            labelsDias.push(d.toLocaleDateString('pt-BR', { weekday: 'short' }));
        }

        var dadosEntrada = [0, 0, 0, 0, 0, 0, 0];
        var dadosSaida = [0, 0, 0, 0, 0, 0, 0];

        movimentacoes.forEach(function (m) {
            var rawDate = m.DataMovimentacao;
            if (!rawDate) return;

            var chaveMov = formatarDataCurta(rawDate);
            var index = chavesDias.indexOf(chaveMov);

            if (index !== -1) {
                if (m.TipoMovimentacaoId === TIPO_ENTRADA) {
                    dadosEntrada[index] += 1;
                } else {
                    dadosSaida[index] += 1;
                }
            }
        });

        var isDark = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        var gridColor = isDark ? 'rgba(255,255,255,0.2)' : 'rgba(0,0,0,0.08)';
        var tickColor = isDark ? '#ffffff' : 'rgba(0,0,0,0.6)';
        var legendColor = isDark ? '#ffffff' : '#374151';

        window.meuGrafico = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labelsDias,
                datasets: [
                    {
                        label: 'Op. de Entrada',
                        data: dadosEntrada,
                        borderColor: '#10B981',
                        backgroundColor: 'rgba(16, 185, 129, 0.1)',
                        tension: 0.3,
                        fill: true
                    },
                    {
                        label: 'Op. de Saída',
                        data: dadosSaida,
                        borderColor: '#EF4444',
                        backgroundColor: 'rgba(239, 68, 68, 0.1)',
                        tension: 0.3,
                        fill: true
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: { color: legendColor }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1, color: tickColor },
                        grid: { color: gridColor },
                        border: { color: gridColor }
                    },
                    x: {
                        ticks: { color: tickColor },
                        grid: { color: gridColor },
                        border: { color: gridColor }
                    }
                }
            }
        });
    }

    /// Renderiza a tabela com as 5 últimas movimentações registradas.
    function renderizarUltimasMovimentacoes(movimentacoes) {
        var tbody = document.querySelector('#tabelaUltimasMov tbody');
        if (!tbody) return;

        var tiposMap = {};
        tiposMap[TIPO_ENTRADA] = { nome: 'ENTRADA', badge: 'bg-success' };
        tiposMap[TIPO_CONSUMO] = { nome: 'CONSUMO', badge: 'bg-warning text-dark' };
        tiposMap[TIPO_ENVIO] = { nome: 'ENVIO', badge: 'bg-info text-white' };
        tiposMap[TIPO_RECEBIMENTO] = { nome: 'RECEBIMENTO', badge: 'bg-primary' };

        var ultimas = movimentacoes.slice().sort(function (a, b) {
            return new Date(b.DataMovimentacao) - new Date(a.DataMovimentacao);
        }).slice(0, 5);

        if (ultimas.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted py-3"><i class="fas fa-inbox me-1"></i>Nenhuma movimentação registrada.</td></tr>';
            return;
        }

        function criarTdNomeInline(nome, ativo, tooltip) {
            var td = document.createElement('td');
            if (ativo) {
                td.textContent = nome;
            } else {
                var span = document.createElement('span');
                span.className = 'text-danger';
                span.textContent = nome;
                td.appendChild(span);
                var icon = document.createElement('i');
                icon.className = 'fas fa-ban text-danger ms-1 small';
                icon.title = tooltip;
                td.appendChild(icon);
            }
            return td;
        }

        tbody.innerHTML = '';
        ultimas.forEach(function (m) {
            var tr = document.createElement('tr');
            tr.appendChild(criarElemento('td', 'text-muted small', formatarData(m.DataMovimentacao)));

            var tdTipo = document.createElement('td');
            var tipoInfo = tiposMap[m.TipoMovimentacaoId] || { nome: 'OUTRO', badge: 'bg-secondary' };
            tdTipo.appendChild(criarElemento('span', 'badge ' + tipoInfo.badge, tipoInfo.nome));
            tr.appendChild(tdTipo);

            var prodAtivo = !!dataStore.buscarPorId('produtos', m.ProdutoId);
            var nomeProduto = dataStore.buscarNome('todosProdutos', m.ProdutoId, null) || dataStore.buscarNome('produtos', m.ProdutoId, 'Produto #' + m.ProdutoId);
            tr.appendChild(criarTdNomeInline(nomeProduto, prodAtivo, 'Produto inativo'));

            tr.appendChild(criarElemento('td', null, String(m.Quantidade || 0)));

            var setorAtivo = !!dataStore.buscarPorId('setores', m.SetorId);
            var nomeSetor = dataStore.buscarNome('todosSetores', m.SetorId, null) || dataStore.buscarNome('setores', m.SetorId, '-');
            tr.appendChild(criarTdNomeInline(nomeSetor, setorAtivo, 'Setor inativo'));

            tbody.appendChild(tr);
        });
    }
})();