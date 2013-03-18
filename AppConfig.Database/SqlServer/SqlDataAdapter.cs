using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace AppConfig.Database.SqlServer
{
    public class SqlDataAdapter : DataAdapter
    {
        public SqlDataAdapter(string ConnectionString) 
            : base(new SqlConnection(ConnectionString), new SqlCommandProivder()) { }


        //#region TranslateWhereClause
        //public override string TranslateWhereClause<T>(Expression<Func<T, bool>> WhereClause)
        //{
        //    WhereClause.Body.ToString();
        //    throw new NotImplementedException();
        //}
        //#endregion
    }
}
