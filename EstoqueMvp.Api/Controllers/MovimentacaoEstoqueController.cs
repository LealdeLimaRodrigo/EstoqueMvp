using Servicos.Dtos;
using Servicos.Interfaces;
using System;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/movimentacao-estoque")]
    public class MovimentacaoEstoqueController : ApiController
    {
        private readonly IMovimentacaoEstoqueServico _movimentacaoEstoqueServico;

        public MovimentacaoEstoqueController(IMovimentacaoEstoqueServico movimentacaoEstoqueServico)
        {
            _movimentacaoEstoqueServico = movimentacaoEstoqueServico;
        }

        [HttpPost]
        [Route("entrada")]
        public IHttpActionResult EntradaProduto(ProdutoDto produtoDto)
        {
            _movimentacaoEstoqueServico.EntradaProduto(produtoDto);
            return Ok();
        }

        [HttpPost]
        [Route("consumo")]
        public IHttpActionResult ConsumoProduto(ProdutoDto produtoDto)
        {
            _movimentacaoEstoqueServico.ConsumoProduto(produtoDto);
            return Ok();
        }

        [HttpPost]
        [Route("transferencia")]
        public IHttpActionResult TransferenciaProduto(TransferenciaProdutoDto transferenciaProdutoDto)
        {
            _movimentacaoEstoqueServico.TransferirProduto(transferenciaProdutoDto);
            return Ok();
        }

        // GET: MovimentacaoEstoque
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterTodos();
            return Ok(movimentacoes);
        }


        [HttpGet]
        [Route("produto/{produtoId:int}")]
        public IHttpActionResult ObterPorProdutoId(int produtoId)
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterPorProdutoId(produtoId);            
            return Ok(movimentacoes);
        }

        [HttpGet]
        [Route("setor/{setorId:int}")]
        public IHttpActionResult ObterPorSetorId(int setorId)
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterPorSetorId(setorId);            
            return Ok(movimentacoes);
        }

        [HttpGet]
        [Route("usuario/{usuarioId:int}")]
        public IHttpActionResult ObterPorUsuarioId(int usuarioId)
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterPorUsuarioId(usuarioId);            
            return Ok(movimentacoes);
        }

        [HttpGet]
        [Route("tipoMovimentacao/{tipoMovimentacaoId:int}")]
        public IHttpActionResult ObterPorTipoMovimentacaoId(int tipoMovimentacaoId)
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterPorTipoMovimentacaoId(tipoMovimentacaoId);            
            return Ok(movimentacoes);
        }


        [HttpGet]
        [Route("transacao/{transacaoId:int}")]
        public IHttpActionResult ObterPorTransacaoId(Guid transacaoId)
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterPorTransacaoId(transacaoId);            
            return Ok(movimentacoes);
        }


        [HttpGet]
        [Route("data/{data:datetime}")]
        public IHttpActionResult ObterPorData(DateTime data)
        {
            var movimentacoes = _movimentacaoEstoqueServico.ObterPorData(data);            
            return Ok(movimentacoes);
        }
    }
}