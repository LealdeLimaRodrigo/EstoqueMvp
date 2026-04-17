// ==========================================
// data-store.js — Cache centralizado de dados
// ==========================================

var dataStore = (function () {
    var cache = {};

    /// Carrega dados de um endpoint e armazena no cache local.
    function carregar(chave, endpoint) {
        return apiGet(endpoint).then(function (dados) {
            cache[chave] = dados || [];
            return cache[chave];
        });
    }

    /// Retorna os dados em cache para a chave informada.
    function obter(chave) {
        return cache[chave] || [];
    }

    /// Define manualmente os dados em cache para uma chave.
    function definir(chave, dados) {
        cache[chave] = dados || [];
    }

    /// Busca um item no cache pelo ID.
    function buscarPorId(chave, id) {
        var lista = cache[chave] || [];
        return lista.find(function (item) { return item.Id === id; });
    }

    /// Retorna o nome de um item no cache pelo ID, ou o fallback.
    function buscarNome(chave, id, fallback) {
        var item = buscarPorId(chave, id);
        return item ? item.Nome : (fallback || '-');
    }

    return {
        carregar: carregar,
        obter: obter,
        definir: definir,
        buscarPorId: buscarPorId,
        buscarNome: buscarNome
    };
})();

// Registrar no namespace App
App.dataStore = dataStore;
