using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System;
using System.Collections.Generic;

namespace Servicos
{
    public class ProdutoServico : IProdutoServico
    {
        private readonly IProdutoRepositorio _produtoRepositorio;

        public ProdutoServico(IProdutoRepositorio produtoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
        }

        public int Adicionar(Produto produto)
        {
            var validator = new ProdutoValidator();
            validator.ValidateAndThrow(produto);

            produto.Sku = GerarSkuUnico();

            return _produtoRepositorio.Adicionar(produto);
        }

        private string GerarSkuUnico()
        {
            string hashCurto = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();            
            return $"PRD-{hashCurto}";
        }

        public void Atualizar(Produto produto)
        {
            var validator = new ProdutoValidator();
            validator.ValidateAndThrow(produto);

            _produtoRepositorio.Atualizar(produto);
        }

        public void Remover(int id)
        {
            var produto = _produtoRepositorio.ObterPorId(id);

            if (produto == null)
                throw new Exception("Produto não encontrado.");
            
            if (!produto.Ativo)
                throw new Exception("Produto já está inativo.");

            _produtoRepositorio.Remover(id);
        }

        public void Restaurar(int id)
        {
            var produto = _produtoRepositorio.ObterPorId(id);

            if (produto == null)
                throw new Exception("Produto não encontrado.");

            if (produto.Ativo)
                throw new Exception("Produto já está ativo.");

            _produtoRepositorio.Restaurar(id);
        }

        public Produto ObterPorId(int id) => _produtoRepositorio.ObterPorId(id);

        public Produto ObterPorSku(string sku) => _produtoRepositorio.ObterPorSku(sku);

        public IEnumerable<Produto> ObterTodos() => _produtoRepositorio.ObterTodos();

        public IEnumerable<Produto> ObterTodosInativos() => _produtoRepositorio.ObterTodosInativos();

    }
}
