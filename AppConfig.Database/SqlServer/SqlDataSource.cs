using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace AppConfig.Database.SqlServer
{
    public abstract class SqlDataSource : DataSource
    {
        protected SqlDataSource(string ConnectionString) 
            : base(new SqlConnection(ConnectionString), new SqlCommandProivder()) { }

        public override IDbCommand  CreateSelectCommand<T>(IEnumerable<string> Columns, string WhereClause, string OrderByClause, int Skip, int Take)
        {
 	        throw new NotImplementedException();
        }
    }
}
