using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AppConfig.Database
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(params string[] ColumnNames)
        {
            this.ColumnNames = ColumnNames;
        }

        public string[] ColumnNames { get; set; }
        public ColumnAttribute[] ColumnObjects { get; set; } 

        public static PrimaryKeyAttribute GetPrimaryKey(Type type)
        {
            var rtn = type.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).SingleOrDefault() as PrimaryKeyAttribute;
            
            if (rtn == null)
                return null;

            var columns = ColumnAttribute.GetColumns(type);

            rtn.ColumnObjects = new ColumnAttribute[rtn.ColumnNames.Length];
            for (int i = 0; i < rtn.ColumnNames.Length; i++)
            {
                var columnName = rtn.ColumnNames[i];
                var column = columns.SingleOrDefault(a => a.ColumnName == columnName);
                if (column == null)
                    throw new Exception("The primary key column '" + columnName + "' was not case sensitive matched to a column attribute on type '" + type.FullName + "'.");
                rtn.ColumnObjects[i] = column;
            }

            return rtn;
        }
    }
}
