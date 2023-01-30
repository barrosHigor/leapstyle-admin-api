using Dapper.Contrib.Extensions;
using DataAccess.Repository;
using Domain.Entities;
using System;
using System.IO;

namespace DataAccess.Utils
{
    public class Utils
    {   
        public static void salvaLog(Exception ex, string metodo = "", string query = "")
        {
            var erro = $"Exception: {ex.Message}{Environment.NewLine}Stacktrace: {ex.StackTrace}";
            salvaLog(erro, metodo, query);
        }

        public static void salvaLog(string erro, string metodo = "", string query = "")
        {
            try
            {
                var rep = ProviderSystem.GetConexaoSqlServer();

                var logErro = new LogErro()
                {
                    erro = erro,
                    query = query,
                    metodo = metodo,
                    data_erro = DateTime.Now
                };

                rep.Insert<LogErro>(logErro);
            }
            catch (Exception ex)
            {
                gerarLogTxt(ex, metodo, query);
            }
        }

        public static void gerarLogTxt(Exception Erro, string Metodo = "", string Query = "")
        {
            var erro = "";

            if (Metodo != "")
                erro += $"Método: {Metodo}";
            if (Query != "")
                erro += $"{Environment.NewLine}Query: {Query}";
            if (erro != null)
                erro += $"Exception: {Erro.Message}{Environment.NewLine}Exception: {Erro.StackTrace}";

            gerarLog(erro);
        }

        private static void gerarLog(string AErro)
        {
            try
            {
                var caminho = $@"{AppDomain.CurrentDomain.BaseDirectory}Erros";
                var arquivoErro = $"{caminho}\\Erro{DateTime.Now:dd.MM.yyyy.HH.mm.ss}.txt";
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);
                File.WriteAllText(arquivoErro, AErro);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao gerar log de erro{Environment.NewLine}Exption: {ex.Message}{Environment.NewLine}Caminho: {ex.StackTrace}");
            }
        }
    }
}
