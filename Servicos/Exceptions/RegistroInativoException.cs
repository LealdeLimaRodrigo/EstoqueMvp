using System;
using System.Collections.Generic;

namespace Servicos.Exceptions
{
    /// <summary>
    /// Exceção lançada ao tentar cadastrar um registro com nome/identificador
    /// que já pertence a um registro inativo. Contém dados dos registros para exibição.
    /// </summary>
    public class RegistroInativoException : InvalidOperationException
    {
        public List<RegistroInativoInfo> Registros { get; }

        public RegistroInativoException(string message, List<RegistroInativoInfo> registros)
            : base(message)
        {
            Registros = registros;
        }
    }

    public class RegistroInativoInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sku { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
