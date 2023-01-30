    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FiltroAPI
    {
        public List<Filtro> filtros { get; set; }
        public string aliasTabelaPrincipal { get; set; }
        public string ordenacao { get; set; }
        public bool flagSomenteAtivo { get; set; } = true;
        public bool flagMostrarExcluido { get; set; }

        public FiltroAPI() { }
    }

    public class Filtro
    {
        public Filtro() { }

        public string nomeCampo { get; set; }
        public string valorCampo { get; set; }
        public Tipo tipo { get; set; }
        public TipoFiltro tipoFiltro { get; set; }
        public Condicao condicao { get; set; }
        public string alias { get; set; }

        public enum Tipo
        {   
            [Description("AND")]
            AND = 1,
            [Description("OR")]
            OR = 2
        }

        public enum TipoFiltro
        {
            [Description("String")]
            STRING = 1,
            [Description("Number")]
            NUMBER = 2,
            [Description("BOOLEAN")]
            BOOLEAN = 3,
            [Description("DATE")]
            DATE = 4
        }

        public enum Condicao
        {
            [Description(">")]
            Maior = 1,
            [Description(">=")]
            MaiorIgual = 2,
            [Description("<")]
            Menor = 3,
            [Description("<=")]
            MenorIgual = 4,
            [Description("=")]
            Igual = 5,
            [Description("!=")]
            Diferente = 6,
            [Description("like")]
            Like = 7
        }

        public static string getTipoFiltro(Tipo tipoFiltro)
        {
            if (tipoFiltro == Tipo.OR)
                return "OR";
            return "AND";
        }

        public static string getCondicaoFiltro(Condicao condicao)
        {
            if (condicao == Condicao.Igual)
                return "=";
            else if (condicao == Condicao.Maior)
                return ">";
            else if (condicao == Condicao.MaiorIgual)
                return ">=";
            else if (condicao == Condicao.Menor)
                return "<";
            else if (condicao == Condicao.MenorIgual)
                return "<=";
            else if (condicao == Condicao.Diferente)
                return "!=";
            else if (condicao == Condicao.Like)
                return "like";

            return "=";
        }
    }
}
