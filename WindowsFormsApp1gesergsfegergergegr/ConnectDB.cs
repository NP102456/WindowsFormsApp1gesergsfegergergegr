using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace WindowsFormsApp1gesergsfegergergegr
{
    internal class ConnectDB
    {
        public static SqlConnection ConnectMinimart()
        {
            string server = @"LAPTOP-IQ8TJ0SI\SQLEXPRESS";
            string db = "Minimart";

            string strCon = string.Format("Data Source={0};Initial Catalog={1};" +
                "Integrated Security=True;Encrypt=False", server, db);

            SqlConnection con = new SqlConnection(strCon);

            con.Open();
            return con;
        }

    }
}
