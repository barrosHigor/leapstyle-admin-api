using System.Data;
using System.Data.SqlClient;
using static Domain.Entities.Setting;

namespace DataAccess.Repository
{
    public class ProviderSystem
    {
        public static EmailSettings emailSettings { get; private set; } = getSettingsFromAppSetings();


        private static EmailSettings getSettingsFromAppSetings()
        {
            var Host = Environment.GetEnvironmentVariable("Email_Host");
            var Port = Environment.GetEnvironmentVariable("Email_Port");
            var NomeUsuarioEmail = Environment.GetEnvironmentVariable("Email_NomeUsuarioEmail");
            var Username = Environment.GetEnvironmentVariable("Email_Username");
            var Password = Environment.GetEnvironmentVariable("Email_Password");

            return new EmailSettings()
            {
                Host = Host,
                NomeUsuarioEmail = NomeUsuarioEmail,
                Password = Password,
                Port = Int32.Parse(Port),
                Username = Username
            };
        }


        public static string? GetStrConnectionSqlServer() {
            var strConnection = Environment.GetEnvironmentVariable("BaseConnection");
            return strConnection;
        }

        public static IDbConnection GetConexaoSqlServer()
        {
            try
            {
                var strConnection = Environment.GetEnvironmentVariable("BaseConnection");
                return new SqlConnection(strConnection);
            }
            catch (Exception ex)
            {
                Utils.Utils.salvaLog(ex, "GetConexaoSqlServer");
                return null;
            }
        }
    }
}
