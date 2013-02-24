using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AppConfig.Database
{
    public interface ICommandProvider
    {
        string GetCreateTable(Type type);
        string GetCreateTableConstraints(Type type);
        IDbCommand GetTableSave(Type type);
    }
}
