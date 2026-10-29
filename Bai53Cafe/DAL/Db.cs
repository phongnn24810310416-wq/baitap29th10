using System.Configuration;
using System.Data.SqlClient;
namespace Bai53Cafe.DAL
{
    public static class Db
    {
        public static SqlConnection TaoKetNoi()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
    }
}
