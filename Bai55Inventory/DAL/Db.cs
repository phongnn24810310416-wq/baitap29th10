using System.Configuration;
using System.Data.SqlClient;
namespace Bai55Inventory.DAL
{
    public static class Db { public static SqlConnection TaoKetNoi() { return new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString); } }
}
