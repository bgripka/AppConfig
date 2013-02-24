using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute() { }
        public TableAttribute(string TableName)
        {
            this.SchemaName = "dbo";
            this.TableName = TableName;
        }
        public TableAttribute(string SchemaName, string TableName)
        {
            this.SchemaName = SchemaName;
            this.TableName = TableName;
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public Type EntityType { get; private set; }

        public static TableAttribute GetTable(Type type)
        {
            var rtn = type.GetCustomAttributes(type, true).SingleOrDefault() as TableAttribute;
            
            if (rtn == null)
                rtn = new TableAttribute(type.Name);

            rtn.EntityType = type;

            return rtn;
        }

        public static HashSet<TableAttribute> GetTables(IEnumerable<Type> types)
        {
            var rtn = new HashSet<TableAttribute>();
            foreach (var type in types)
            {
                var tableAttribute = GetTable(type);
                if (!rtn.Contains(tableAttribute))
                    rtn.Add(tableAttribute);
            }
            return rtn;
        }
    }
}
