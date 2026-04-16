using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicos.Validacoes;

namespace EstoqueMvp.Testes.Validacoes
{
    /// <summary>
    /// Testes unitários para a validação de CPF.
    /// Cobre cenários de CPFs válidos, inválidos, formatados e edge cases.
    /// </summary>
    [TestClass]
    public class CpfValidacaoTestes
    {
        [TestMethod]
        public void ValidarCpf_CpfValido_DeveRetornarTrue()
        {
            // CPF válido gerado para teste: 529.982.247-25
            Assert.IsTrue(CpfValidacao.ValidarCpf("52998224725"));
        }

        [TestMethod]
        public void ValidarCpf_CpfValidoComFormatacao_DeveRetornarTrue()
        {
            Assert.IsTrue(CpfValidacao.ValidarCpf("529.982.247-25"));
        }

        [TestMethod]
        public void ValidarCpf_CpfAdminSistema_DeveRetornarTrue()
        {
            // CPF especial do admin (00000000000) é aceito pelo sistema
            Assert.IsTrue(CpfValidacao.ValidarCpf("00000000000"));
        }

        [TestMethod]
        public void ValidarCpf_CpfComTodosDigitosIguais_DeveRetornarFalse()
        {
            Assert.IsFalse(CpfValidacao.ValidarCpf("11111111111"));
            Assert.IsFalse(CpfValidacao.ValidarCpf("22222222222"));
            Assert.IsFalse(CpfValidacao.ValidarCpf("99999999999"));
        }

        [TestMethod]
        public void ValidarCpf_CpfVazio_DeveRetornarFalse()
        {
            Assert.IsFalse(CpfValidacao.ValidarCpf(""));
            Assert.IsFalse(CpfValidacao.ValidarCpf(null));
            Assert.IsFalse(CpfValidacao.ValidarCpf("   "));
        }

        [TestMethod]
        public void ValidarCpf_CpfComTamanhoIncorreto_DeveRetornarFalse()
        {
            Assert.IsFalse(CpfValidacao.ValidarCpf("1234"));
            Assert.IsFalse(CpfValidacao.ValidarCpf("123456789012"));
        }

        [TestMethod]
        public void ValidarCpf_CpfComDigitosVerificadoresErrados_DeveRetornarFalse()
        {
            // CPF com dígitos errados (529.982.247-26 em vez de 25)
            Assert.IsFalse(CpfValidacao.ValidarCpf("52998224726"));
        }

        [TestMethod]
        public void LimparCpf_CpfFormatado_DeveRetornarApenasDigitos()
        {
            string resultado = CpfValidacao.LimparCpf("529.982.247-25");
            Assert.AreEqual("52998224725", resultado);
        }

        [TestMethod]
        public void LimparCpf_CpfLimpo_DeveRetornarIgual()
        {
            string resultado = CpfValidacao.LimparCpf("52998224725");
            Assert.AreEqual("52998224725", resultado);
        }

        [TestMethod]
        public void LimparCpf_ValorNulo_DeveRetornarNulo()
        {
            Assert.IsNull(CpfValidacao.LimparCpf(null));
        }
    }
}
