using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace AppConfig.Database.Npgsql
{
    public class NpgsqlDataAdapter : DataAdapter
    {
        public NpgsqlDataAdapter(string ConnectionString) 
            : base(new SqlConnection(ConnectionString), new NpgsqlCommandProvider()) { }
    }
}
