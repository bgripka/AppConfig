using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database
{
    public class OneToManyRelationshipAttribute : Attribute
    {
        public OneToManyRelationshipAttribute() { }
        public OneToManyRelationshipAttribute(params string[] ForeignKeyColumns)
        {
            this.ForeignKeyColumns = ForeignKeyColumns;
        }

        public Type ParentType { get; private set; }
        public Type ChildType { get; private set; }
        public string[] ForeignKeyColumns { get; set; }
        public ColumnAttribute[] ForeignKeyColumnObjects { get; private set; }

        public static List<OneToManyRelationshipAttribute> GetOneToManyRelationships(Type type)
        {
            var rtn = new List<OneToManyRelationshipAttribute>();
            var props = type.GetProperties();
            var primaryKeyColumns = ColumnAttribute.GetPrimaryKeyColumns(type);

            foreach (var prop in props)
            {
                var relationship = prop.GetCustomAttributes(typeof(OneToManyRelationshipAttribute), true).SingleOrDefault() as OneToManyRelationshipAttribute;
                if (relationship == null)
                    continue;

                var propertyType = prop.PropertyType;
                if(!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(DatabaseEntityCollection<>))
                    throw new Exception("Property '" + prop.Name + " on type '" + type.FullName + "' is marked with attribute '" + typeof(OneToManyRelationshipAttribute).FullName + "' but doesn't use the required collection type '" + typeof(DatabaseEntityCollection<>).FullName + "'.");

                relationship.ParentType = type;
                relationship.ChildType = propertyType.GetGenericArguments()[0];

                //Create the relationship explicitly if it was left unspecified
                if (relationship.ForeignKeyColumns == null && primaryKeyColumns.Count == 1 && primaryKeyColumns[0].ColumnName == "Id" && relationship.ChildType.GetProperty(type.Name + "Id") != null)
                    relationship.ForeignKeyColumns = new string[] { type.Name + "Id" };

                //Check to see if any foreign key columns have been defined
                if (relationship.ForeignKeyColumns == null || relationship.ForeignKeyColumns.Length == 0)
                    throw new Exception("Foreign key column(s) have not been specified for property '" + prop.Name + "' on type '" + type.FullName + "' or the property '" + type.Name + "Id' is not specified on type '" + relationship.ChildType.FullName + "'.");

                //Check to make sure the same number of foreign key columns are present in the primary key
                bool passed = true;
                if(primaryKeyColumns.Count != relationship.ForeignKeyColumns.Length)
                    passed = false;
                else
                {
                    //Get all columns from the child type
                    var childTypeColumns = ColumnAttribute.GetColumns(relationship.ChildType);
                    relationship.ForeignKeyColumnObjects = new ColumnAttribute[primaryKeyColumns.Count];
                    for (int i = 0; i < primaryKeyColumns.Count; i++)
                    {
                        var parentProperty = primaryKeyColumns[i].Property;
                        var childColumn = childTypeColumns.SingleOrDefault(a => a.ColumnName == relationship.ForeignKeyColumns[i]);

                        if (childColumn == null)
                            throw new Exception("The foreign key column '" + relationship.ForeignKeyColumns[i] + "' specified for property '" + prop.Name + "' on type '" + type.FullName + "' doesn't exist as a property on type '" + relationship.ChildType.FullName + "'.  Property names are case sensitive.");
                        if (parentProperty.PropertyType != childColumn.Property.PropertyType)
                            passed = false;

                        relationship.ForeignKeyColumnObjects[i] = childColumn;
                    }
                }

                if (!passed)
                    throw new Exception("The one to many relationship property '" + prop.Name + "' on type '" + type.FullName + "' is invalid because the entity's primary key column(s) '" + string.Join(", ", primaryKeyColumns.Select(a => a.ColumnName)) + "' don't match the foreign key column(s) '" + string.Join(", ", relationship.ForeignKeyColumns) + "' in number, data type, and order.");

                rtn.Add(relationship);
            }

            return rtn;
        }
        public static HashSet<OneToManyRelationshipAttribute> GetOneToManyRelationships(IEnumerable<Type> types)
        {
            var rtn = new HashSet<OneToManyRelationshipAttribute>();
            foreach (var type in types)
                foreach (var oneToManyRelationship in GetOneToManyRelationships(type))
                    if (!rtn.Contains(oneToManyRelationship))
                        rtn.Add(oneToManyRelationship);
            return rtn;
        }
    }
}
