using System;
using System.ComponentModel;

namespace ConsoleNet
{
    #region Enumeradores

    public enum Formato
    {
        Eletronico,
        Extraido
    }

    public enum Estado
    {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Transferido")]
        Transferido = 2,
        [Description("Arquivado")]
        Arquivado = 3,
        [Description("Destruido")]
        Destruido = 4,
        [Description("Em Transferencia")]
        EmTransferencia = 5,
        [Description("Em Destruição")]
        EmDestruicao = 6
    }

    #endregion

    /// <summary>
    /// Classe destinada a informações relativas a documentos
    /// </summary>
    public class Metadata
    {
        #region Propriedades
        private string numeroRegisto;
        private int numeroExemplar;
        private int numeroCopia;
        private string classificacaoSeguranca;
        private Estado estadoExemplar;
        private Formato formatoExemplar;
        private string utilizador;
        private DateTime dataOperacao;
        private string siglaPrincipal;
        private string postoAtual;
        private string dominio;

     
        public string NumeroRegisto { get => numeroRegisto; set => numeroRegisto = value; }

        public int NumeroExemplar { get => numeroExemplar; set => numeroExemplar = value; }
       
        public int NumeroCopia { get => numeroCopia; set => numeroCopia = value; }
  
        public string ClassificacaoSeguranca { get => classificacaoSeguranca; set => classificacaoSeguranca = value; }

        public string Utilizador { get => utilizador; set => utilizador = value; }

        public DateTime DataOperacao { get => dataOperacao; set => dataOperacao = value; }

        public Estado EstadoExemplar { get => estadoExemplar; set => estadoExemplar = value; }
   
        public Formato FormatoExemplar { get => formatoExemplar; set => formatoExemplar = value; }
      
        public string SiglaPrincipal { get => siglaPrincipal; set => siglaPrincipal = value; }
      
        public string PostoAtual { get => postoAtual; set => postoAtual = value; }
     
        public string Dominio { get => dominio; set => dominio = value; }
        #endregion

        #region Construtores
   
        public Metadata()
        {
        }

        public Metadata(string numeroRegisto, int numeroExemplar, int numeroCopia, string classificacaoSeguranca, Estado estadoExemplar, Formato formatoExemplar, string utilizador, DateTime dataOperacao, string siglaPrincipal, string postoAtual, string dominio)
        {
            this.numeroRegisto = numeroRegisto;
            this.numeroExemplar = numeroExemplar;
            this.numeroCopia = numeroCopia;
            this.classificacaoSeguranca = classificacaoSeguranca;
            this.estadoExemplar = estadoExemplar;
            this.formatoExemplar = formatoExemplar;
            this.utilizador = utilizador;
            this.dataOperacao = dataOperacao;
            this.siglaPrincipal = siglaPrincipal;
            this.postoAtual = postoAtual;
            this.dominio = dominio;
        }
        #endregion
    }
}
