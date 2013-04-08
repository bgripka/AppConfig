using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AppConfig.Database.Npgsql
{
    public class NpgsqlCommandProvider : ICommandProvider
    {

        #region ICommandProvider Members

        public string GetCreateTable(Type type)
        {
            throw new NotImplementedException();
        }

        public string GetCreateTableConstraints(Type type)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDbCommand GetTableSave(Type type)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDbCommand CreateSelectCommand<T>(Expression<Func<T, bool>> WhereClause, string OrderByClause, int Skip, int Take, params string[] Properties)
        {
            throw new NotImplementedException();
        }

        public string TranslateWhereClause<T>(Expression<Func<T, bool>> WhereClause)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
