using System.Configuration;
using System.Data.SqlClient;
namespace Bai54Hotel.DAL
{
    public static class Db { public static SqlConnection TaoKetNoi() { return new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString); } }
}
