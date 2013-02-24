using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace AppConfig.Database.Npgsql
{
    public abstract class NpgsqlDataSource : DataSource
    {
        protected NpgsqlDataSource(string ConnectionString) 
            : base(new SqlConnection(ConnectionString), new NpgsqlCommandProvider()) { }
    }
}
