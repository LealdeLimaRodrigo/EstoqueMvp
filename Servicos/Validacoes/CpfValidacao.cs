using System.Linq;

namespace Servicos.Validacoes
{
    public static class CpfValidacao
    {
        public static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) 
                return false;

            cpf = LimparCpf(cpf);

            if (cpf.Length != 11) 
                return false;

            // Rejeita CPFs com todos os números iguais (ex: 111.111.111-11)
            if (cpf != "00000000000" && cpf.Distinct().Count() == 1) return false;
            
            // Cálculo do 1º dígito
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            // Cálculo do 2º dígito
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf.EndsWith(digito1.ToString() + digito2.ToString());
        }

        public static string LimparCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return cpf;
            return new string(cpf.Where(char.IsDigit).ToArray());
        }
    }
}
