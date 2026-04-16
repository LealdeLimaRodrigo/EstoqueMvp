using EstoqueMvp.Api.Extensions;
using Servicos.Dtos;
using Servicos.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    /// <summary>
    /// Controller responsável por todas as operações de movimentação de estoque.
    /// O UsuarioId é sempre extraído do token JWT para garantir integridade e segurança.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/movimentacao-estoque")]
    public class MovimentacaoEstoqueController : ApiController
    {
        private readonly IMovimentacaoEstoqueServico _movimentacaoEstoqueServico;

        public MovimentacaoEstoqueController(IMovimentacaoEstoqueServico movimentacaoEstoqueServico)
        {
            _movimentacaoEstoqueServico = movimentacaoEstoqueServico;
        }

        /// <summary>
        /// Registra a entrada (aquisição) de um produto em um setor, creditando o estoque.
        /// </summary>
        [HttpPost]
        [Route("entrada")]
        public async Task<IHttpActionResult> EntradaProduto([FromBody] MovimentacaoEstoqueDto movimentacaoDto)
        {
            // Segurança: o UsuarioId é obtido do token JWT, nunca do corpo da requisição
            movimentacaoDto.UsuarioId = this.ObterUsuarioIdDoToken();

            await _movimentacaoEstoqueServico.EntradaProduto(movimentacaoDto);
            return Ok(new { Mensagem = "Entrada de produto registrada com sucesso." });
        }

        /// <summary>
        /// Registra o consumo interno de um produto, debitando o estoque do setor.
        /// </summary>
        [HttpPost]
        [Route("consumo")]
        public async Task<IHttpActionResult> ConsumoProduto([FromBody] MovimentacaoEstoqueDto movimentacaoDto)
        {
            movimentacaoDto.UsuarioId = this.ObterUsuarioIdDoToken();

            await _movimentacaoEstoqueServico.ConsumoProduto(movimentacaoDto);
            return Ok(new { Mensagem = "Consumo de produto registrado com sucesso." });
        }

        /// <summary>
        /// Realiza a transferência de um produto entre dois setores em transação atômica.
        /// </summary>
        [HttpPost]
        [Route("transferencia")]
        public async Task<IHttpActionResult> TransferenciaProduto([FromBody] TransferenciaProdutoDto transferenciaProdutoDto)
        {
            transferenciaProdutoDto.UsuarioId = this.ObterUsuarioIdDoToken();

            await _movimentacaoEstoqueServico.TransferirProduto(transferenciaProdutoDto);
            return Ok(new { Mensagem = "Transferência entre setores realizada com sucesso." });
        }

        /// <summary>
        /// Retorna todas as movimentações de estoque registradas no sistema.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos()
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterTodos();
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Retorna as movimentações filtradas por produto.
        /// </summary>
        [HttpGet]
        [Route("produto/{produtoId:int}")]
        public async Task<IHttpActionResult> ObterPorProdutoId(int produtoId)
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterPorProdutoId(produtoId);
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Retorna as movimentações filtradas por setor.
        /// </summary>
        [HttpGet]
        [Route("setor/{setorId:int}")]
        public async Task<IHttpActionResult> ObterPorSetorId(int setorId)
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterPorSetorId(setorId);
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Retorna as movimentações filtradas por usuário.
        /// </summary>
        [HttpGet]
        [Route("usuario/{usuarioId:int}")]
        public async Task<IHttpActionResult> ObterPorUsuarioId(int usuarioId)
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterPorUsuarioId(usuarioId);
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Retorna as movimentações filtradas por tipo (Entrada, Consumo, Envio, Recebimento).
        /// </summary>
        [HttpGet]
        [Route("tipo-movimentacao/{tipoMovimentacaoId:int}")]
        public async Task<IHttpActionResult> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId)
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterPorTipoMovimentacaoId(tipoMovimentacaoId);
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Retorna as movimentações que pertencem a uma mesma transação (ex: transferência entre setores).
        /// </summary>
        [HttpGet]
        [Route("transacao/{transacaoId:guid}")]
        public async Task<IHttpActionResult> ObterPorTransacaoId(Guid transacaoId)
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterPorTransacaoId(transacaoId);
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Retorna as movimentações filtradas por data específica.
        /// </summary>
        [HttpGet]
        [Route("data/{data:datetime}")]
        public async Task<IHttpActionResult> ObterPorData(DateTime data)
        {
            var movimentacoes = await _movimentacaoEstoqueServico.ObterPorData(data);
            return Ok(movimentacoes);
        }
    }
}