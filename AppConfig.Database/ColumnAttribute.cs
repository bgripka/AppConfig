using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AppConfig.Database
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(int Order) 
        {
            this.Order = Order;

            //Set the default values
            this.CanInsert = true;
            this.CanUpdate = true;
            this.Nullable = true;
            this.LengthType = Database.LengthType.Variable;
            this.EnumStorageMethod = Database.EnumStorageMethod.Text;
            this.Percision = 19;
            this.Length = -1;
            this.Scale = 5;
        }
        public ColumnAttribute(int Order, string ColumnName) 
            : this(Order)
        {
            this.ColumnName = ColumnName;
        }

        public string ColumnName { get; set; }
        public PropertyInfo Property { get; private set; }
        public int Order { get; set; }
        public bool Nullable { get; set; }
        public int Length { get; set; }
        public LengthType LengthType { get; set; }
        public bool IsIdentity { get; set; }
        public bool CanInsert
        {
            get { return (IsIdentity) ? false : canInsert; }
            set { canInsert = value; }
        }
        private bool canInsert;
        public bool CanUpdate 
        {
            get { return (IsIdentity) ? false : canUpdate; }
            set { canUpdate = value; }  
        }
        private bool canUpdate;
        public EnumStorageMethod EnumStorageMethod { get; set; }
        public TimestampBehavior TimestampBehavior { get; set; }
        public int Percision { get; set; }
        public int Scale { get; set; }

        /// <summary>
        /// Gets all database column information for the requested type.
        /// </summary>
        /// <param name="type">The type containing column information</param>
        /// <returns></returns>
        public static List<ColumnAttribute> GetColumns(Type type)
        {
            var rtn = new List<ColumnAttribute>();
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                var column = prop.GetCustomAttributes(typeof(ColumnAttribute), true).SingleOrDefault() as ColumnAttribute;
                if (column == null)
                    continue;

                column.Property = prop;

                if (string.IsNullOrEmpty(column.ColumnName))
                    column.ColumnName = prop.Name;

                if (!column.Nullable)
                {

                }

                rtn.Add(column);
            }
            return rtn.OrderBy(a => a.Order).ToList();
        }
    }
}
