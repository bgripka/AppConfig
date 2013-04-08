using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using AppConfig.Database.Reflection;
using System.Configuration;

namespace AppConfig.Database
{
    /// <summary>
    /// The base type for all objects derived from a database table.
    /// </summary>
    public abstract class DatabaseEntity
    {

        private static Dictionary<Type, DataSource> dataSourcesByType = new Dictionary<Type, DataSource>();

        #region Constructors
        public DatabaseEntity() 
        {
            DataSource dataSource;
            if (dataSourcesByType.TryGetValue(this.GetType(), out dataSource))
            {
                this.DataSource = dataSource;
                return;
            }
            
            //Since we didn't a DataSource object, loop through all connection string objects in the app.config
            //Find the first connection string for a DataSource that implements an EntitySource property for this type
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                var typeName = connectionString.Name;
                var dataSourceType = Type.GetType(typeName);

                //If this connection string isn't a type name then it isn't meant for us
                if (dataSourceType == null)
                    continue;

                //If this type doesn't inherit from DataSource then it isn't meant for us
                if (!dataSourceType.IsSubclassOf(typeof(DataSource)))
                    continue;

                //Create an EntitySource type object that uses the current object's type as the generic T parameter
                var entitySourceType = typeof(EntitySource<>).MakeGenericType(new Type[] { this.GetType() });

                //Determine if the current DataSource type implements a property returning the entitySourceType
                if (!dataSourceType.GetProperties().Any(a => a.PropertyType == entitySourceType))
                    continue;

                this.DataSource = DataSource.GetCurrentDataSource(dataSourceType);
                dataSourcesByType.Add(this.GetType(), this.DataSource);

                break;
            }
        }
        #endregion

        #region Properties
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [System.Web.Script.Serialization.ScriptIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DataSource DataSource { get; internal protected set; }
        #endregion

        #region Reflection
        public List<ColumnInfo> GetColumns<T>()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Load
        internal void Load(IDataReader dataReader)
        {
            var type = this.GetType();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                //Get the column name
                var name = dataReader.GetName(i);

                //Find the property matching this field
                var property = type.GetProperty(name);

                //Continue to the next field if there is no matching property or the property can't be set
                if (property == null || !property.CanWrite)
                    continue;

                //Set the value of this property
                var value = dataReader.GetValue(i);
                var propertyType = property.PropertyType;
                if (Convert.IsDBNull(value))
                    property.SetValue(this, null, null);
                else if (propertyType.IsEnum)
                {
                    if (value is string)
                        property.SetValue(this, Enum.Parse(propertyType, value as string), null);
                    else
                        property.SetValue(this, Enum.ToObject(propertyType, value), null);
                }
                else
                    property.SetValue(this, value, null);
            }
        }
        #endregion

        #region Save
        public void Save()
        {
            //Create an instance of a list of this type that will be passed to the generic save
            var gTypeShell = typeof(List<>);
            var gType = gTypeShell.MakeGenericType(new Type[] { this.GetType() });
            var instance = Activator.CreateInstance(gType);

            //Add this object to the list
            var addMethod = instance.GetType().GetMethod("Add", new Type[] { this.GetType() });
            addMethod.Invoke(instance, new object[] { this });

            //Invoke the static generic Save method
            MethodInfo method = typeof(DatabaseEntity).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(this.GetType());
            try
            {
                generic.Invoke(null, new object[] { instance });
            }
            catch (TargetInvocationException ex)
            {
                throw new Exception("An error occurred attempting to save an object of type '" + this.GetType().FullName + "'.", ex.InnerException);
            }
        }

        protected static void Save<T>(IEnumerable<T> Entities) where T : DatabaseEntity
        {
            if(Entities.Count() == 0)
                return;

            var type = typeof(T);
            
            //Ensure all items have the same data source
            var ds = Entities.First().DataSource;
            foreach (var entity in Entities)
                if (ds != entity.DataSource)
                    throw new Exception("All entities to be saved must be from the same data source");

            var command = ds.DataAdapter.CommandProvider.GetTableSave(type);
            command.Connection = ds.DataAdapter.Connection;
            command.Connection.Open();

            try
            {
                command.Prepare();

                var columns = ColumnAttribute.GetColumns(type);
                foreach (var entity in Entities)
                {
                    //Set parameter values before execution
                    foreach (var column in columns)
                    {
                        if (!column.CanInsert && !column.CanUpdate)
                            continue;
                        var param = command.Parameters[column.ColumnName] as IDbDataParameter;
                        var value = column.Property.GetValue(entity, null);
                        var propertyType = column.Property.PropertyType;
                        if (value == null)
                            param.Value = DBNull.Value;
                        else if (propertyType.IsEnum)
                        {
                            if (column.EnumStorageMethod == EnumStorageMethod.Text)
                                param.Value = Enum.GetName(propertyType, value);
                            else if (Enum.GetUnderlyingType(propertyType) == typeof(byte))
                                param.Value = Convert.ToByte(value);
                            else if (Enum.GetUnderlyingType(propertyType) == typeof(Int16))
                                param.Value = Convert.ToInt16(value);
                            else if (Enum.GetUnderlyingType(propertyType) == typeof(int))
                                param.Value = Convert.ToInt32(value);
                            else
                                throw new NotSupportedException("The underlying enum type '" + Enum.GetUnderlyingType(propertyType).FullName + "' is not supported as an enum type.");
                        }
                        else
                            param.Value = value;
                    }

                    //Execute the save command
                    command.ExecuteNonQueryManaged();
                }
            }
            finally
            {
                command.Connection.Close();
            }
        }
        #endregion

        #region Dependants
        public static HashSet<Type> GetRelatedTypes(Type type)
        {
            var rtn = new HashSet<Type>();

            return rtn;
        }
        #endregion
    }
}
