using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System;
using System.Collections.Generic;

namespace Servicos
{
    public class SetorServico : ISetorServico
    {
        private readonly ISetorRepositorio _setorRepositorio;

        public SetorServico(ISetorRepositorio setorRepositorio)
        {
            _setorRepositorio = setorRepositorio;
        }

        public int Adicionar(Setor setor)
        {
            var validator = new SetorValidator();
            validator.ValidateAndThrow(setor);

            return _setorRepositorio.Adicionar(setor);
        }

        public void Atualizar(Setor setor)
        {
            var validator = new SetorValidator();
            validator.ValidateAndThrow(setor);

            _setorRepositorio.Atualizar(setor);
        }

        public void Remover(int id)
        {
            var setor = _setorRepositorio.ObterPorId(id);

            if (setor == null)
                throw new Exception("Setor não encontrado.");
            
            if (!setor.Ativo)
                throw new Exception("Setor já está inativo.");

            _setorRepositorio.Remover(id);
        }

        public void Restaurar(int id)
        {
            var setor = _setorRepositorio.ObterPorId(id);

            if (setor == null)
                throw new Exception("Setor não encontrado.");

            if (setor.Ativo)
                throw new Exception("Setor já está ativo.");

            _setorRepositorio.Restaurar(id);
        }

        public Setor ObterPorId(int id) => _setorRepositorio.ObterPorId(id);

        public IEnumerable<Setor> ObterTodos() => _setorRepositorio.ObterTodos();

        public IEnumerable<Setor> ObterTodosInativos() => _setorRepositorio.ObterTodosInativos();

    }
}
