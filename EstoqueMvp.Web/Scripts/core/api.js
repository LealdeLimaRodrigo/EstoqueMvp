// ==========================================
// api.js — Camada de comunicação HTTP com a API
// ==========================================

// Enviar cookies (jwt_token httpOnly) em requisições cross-origin
axios.defaults.withCredentials = true;

/// Executa GET e retorna os itens da resposta (ou o objeto completo).
async function apiGet(url) {
    var response = await axios.get(API_URL + url);
    var data = response.data;
    return data.Itens || data;
}

/// Executa GET e retorna a resposta bruta sem tratamento.
async function apiGetRaw(url) {
    var response = await axios.get(API_URL + url);
    return response.data;
}

/// Executa GET paginado, enviando página e tamanho como query string.
async function apiGetPaginado(url, pagina, tamanhoPagina) {
    tamanhoPagina = tamanhoPagina || ITENS_POR_PAGINA;
    var sep = url.indexOf('?') !== -1 ? '&' : '?';
    var response = await axios.get(API_URL + url + sep + 'pagina=' + pagina + '&tamanhoPagina=' + tamanhoPagina);
    return response.data;
}

/// Executa GET com busca textual e paginação server-side.
async function apiGetBuscaPaginada(url, termo, pagina, tamanhoPagina) {
    tamanhoPagina = tamanhoPagina || ITENS_POR_PAGINA;
    var sep = url.indexOf('?') !== -1 ? '&' : '?';
    var response = await axios.get(API_URL + url + sep + 'busca=' + encodeURIComponent(termo) + '&pagina=' + pagina + '&tamanhoPagina=' + tamanhoPagina);
    return response.data;
}

/// Executa POST enviando o payload como JSON.
function apiPost(url, payload) {
    return axios.post(API_URL + url, payload);
}

/// Executa PUT enviando o payload como JSON.
function apiPut(url, payload) {
    return axios.put(API_URL + url, payload);
}

/// Executa DELETE no endpoint informado.
function apiDelete(url) {
    return axios.delete(API_URL + url);
}

/// Extrai a mensagem de erro da resposta HTTP ou retorna o fallback.
function extrairMsgErro(error, fallback) {
    if (error.response && error.response.data) {
        var data = error.response.data;
        return data.Message || data.message || data.Mensagem || fallback;
    }
    return fallback;
}

// Registrar no namespace App
App.apiGet = apiGet;
App.apiGetRaw = apiGetRaw;
App.apiGetPaginado = apiGetPaginado;
App.apiGetBuscaPaginada = apiGetBuscaPaginada;
App.apiPost = apiPost;
App.apiPut = apiPut;
App.apiDelete = apiDelete;
App.extrairMsgErro = extrairMsgErro;
