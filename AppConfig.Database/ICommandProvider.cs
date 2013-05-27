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
        IDbCommand CreateSelectCommand<T>(Expression<Func<T, dynamic>> SelectClause, Expression<Func<T, bool>> WhereClause, Expression<Func<T, dynamic>> OrderByClause, int Skip, int Take);
        string TranslateSelectClause<T>(Expression<Func<T, dynamic>> OrderByClause);
        string TranslateWhereClause<T>(Expression<Func<T, bool>> WhereClause);
        string TranslateOrderByClause<T>(Expression<Func<T, dynamic>> OrderByClause);
    }
}
