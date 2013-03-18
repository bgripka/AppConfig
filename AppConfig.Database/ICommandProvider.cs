using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;

namespace AppConfig.Database
{
    public interface ICommandProvider
    {
        string GetCreateTable(Type type);
        string GetCreateTableConstraints(Type type);
        IDbCommand GetTableSave(Type type);
        IDbCommand CreateSelectCommand<T>(Expression<Func<T, bool>> WhereClause, string OrderByClause, int Skip, int Take, params string[] Properties);
    }
}
