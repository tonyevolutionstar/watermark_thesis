using System;
using System.ComponentModel;


namespace WatermarkApp
{
    #region Enumeradores
   
    /// <summary>
    /// Possiveis formatos de um exemplar
    /// </summary>
    public enum Formato
    {
        Eletronico,
        Extraido
    }

    /// <summary>
    /// Possiveis estados de um exemplar
    /// </summary>
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

        /// <summary>
        /// numero de registo do documento
        /// </summary>
        public string NumeroRegisto { get => numeroRegisto; set => numeroRegisto = value; }
        /// <summary>
        /// numero exemplar do documento
        /// </summary>
        public int NumeroExemplar { get => numeroExemplar; set => numeroExemplar = value; }
        /// <summary>
        /// numero de cópia do documento
        /// </summary>
        public int NumeroCopia { get => numeroCopia; set => numeroCopia = value; }
        /// <summary>
        /// classificacao de segurança do documento
        /// </summary>
        public string ClassificacaoSeguranca { get => classificacaoSeguranca; set => classificacaoSeguranca = value; }
        /// <summary>
        /// utilizador que tem o documento ou assinou
        /// </summary>
        public string Utilizador { get => utilizador; set => utilizador = value; }
        /// <summary>
        /// data de alteração de algo em relação ao documento
        /// </summary>
        public DateTime DataOperacao { get => dataOperacao; set => dataOperacao = value; }
        /// <summary>
        /// qual é o estado do documento, se está ativo,...
        /// </summary>
        public Estado EstadoExemplar { get => estadoExemplar; set => estadoExemplar = value; }
        /// <summary>
        /// define o formato do documento
        /// </summary>
        public Formato FormatoExemplar { get => formatoExemplar; set => formatoExemplar = value; }
        /// <summary>
        /// sigla do documento
        /// </summary>
        public string SiglaPrincipal { get => siglaPrincipal; set => siglaPrincipal = value; }
        /// <summary>
        /// posto atual em que o documento se encontra
        /// </summary>
        public string PostoAtual { get => postoAtual; set => postoAtual = value; }
        /// <summary>
        /// dominio do documento
        /// </summary>
        public string Dominio { get => dominio; set => dominio = value; }
        #endregion

        #region Construtores
        /// <summary>
        /// Construtor
        /// </summary>
        public Metadata()
        {
        }

        /// <summary>
        /// Instancia um documento
        /// </summary>
        /// <param name="numeroRegisto"></param>
        /// <param name="numeroExemplar"></param>
        /// <param name="numeroCopia"></param>
        /// <param name="classificacaoSeguranca"></param>
        /// <param name="estadoExemplar"></param>
        /// <param name="formatoExemplar"></param>
        /// <param name="utilizador"></param>
        /// <param name="dataOperacao"></param>
        /// <param name="siglaPrincipal"></param>
        /// <param name="postoAtual"></param>
        /// <param name="dominio"></param>
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