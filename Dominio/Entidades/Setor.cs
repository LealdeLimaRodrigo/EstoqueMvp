namespace Dominio.Entidades
{
    /// <summary>
    /// Entidade de domínio que representa um setor/departamento da organização.
    /// Cada setor possui seu próprio estoque de produtos.
    /// </summary>
    public class Setor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
