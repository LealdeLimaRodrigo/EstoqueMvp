using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Servicos
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public UsuarioServico(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        public UsuarioRetornoDto RealizarLogin(LoginDto loginDto)
        {
            var validator = new LoginDtoValidator();
            validator.ValidateAndThrow(loginDto);

            string cpfLimpo = CpfValidacao.LimparCpf(loginDto.Cpf); 

            var usuario = _usuarioRepositorio.ObterPorCpf(cpfLimpo);

            if (usuario == null || !usuario.Ativo)
                throw new UnauthorizedAccessException("Usuário não encontrado ou inativo.");

            string senhaCriptografada = GerarHashSenha(loginDto.Senha);

            if (usuario.SenhaHash != senhaCriptografada)
                throw new UnauthorizedAccessException("Senha incorreta.");

            return new UsuarioRetornoDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Cpf = usuario.Cpf
            };
        }

        public int Adicionar(Usuario usuario)
        {
            var validator = new UsuarioValidator();
            validator.ValidateAndThrow(usuario);

            usuario.Cpf = CpfValidacao.LimparCpf(usuario.Cpf);

            usuario.SenhaHash = GerarHashSenha(usuario.SenhaHash);

            if (_usuarioRepositorio.ObterPorCpf(usuario.Cpf) != null)
                throw new InvalidOperationException("Já existe um usuário com este CPF.");

            return _usuarioRepositorio.Adicionar(usuario);
        }

        public void Atualizar(Usuario usuario)
        {
            var validator = new UsuarioValidator();
            validator.ValidateAndThrow(usuario);

            usuario.Cpf = CpfValidacao.LimparCpf(usuario.Cpf);

            _usuarioRepositorio.Atualizar(usuario);
        }

        private string GerarHashSenha(string senhaPura)
        {
            if (string.IsNullOrWhiteSpace(senhaPura)) return senhaPura;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senhaPura));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Converte para Hexadecimal
                }
                return builder.ToString();
            }
        }

        public void Remover(int id)
        {
            var usuario = _usuarioRepositorio.ObterPorId(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");
            
            if (!usuario.Ativo)
                throw new Exception("Usuário já está inativo.");

            _usuarioRepositorio.Remover(id);
        }

        public void Restaurar(int id)
        {
            var usuario = _usuarioRepositorio.ObterPorId(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            if (usuario.Ativo)
                throw new Exception("Usuário já está ativo.");

            _usuarioRepositorio.Restaurar(id);
        }

        public Usuario ObterPorId(int id) => _usuarioRepositorio.ObterPorId(id);

        public IEnumerable<Usuario> ObterTodos() => _usuarioRepositorio.ObterTodos();

        public IEnumerable<Usuario> ObterTodosInativos() => _usuarioRepositorio.ObterTodosInativos();

        public Usuario ObterPorCpf(string cpf) => _usuarioRepositorio.ObterPorCpf(cpf);
    }
}
