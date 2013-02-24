﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using AppConfig.Database.Reflection;

namespace AppConfig.Database
{
    public abstract class DatabaseEntity
    {
        #region Constructors
        public DatabaseEntity() 
        {
            this.DataSource = DataSource.Current;
        }
        public DatabaseEntity(DataSource DataSource)
        {
            this.DataSource = DataSource;
        }
        #endregion

        #region Properties
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [System.Web.Script.Serialization.ScriptIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DataSource DataSource { get; internal protected set; }

        public static string[] ColumnNames
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Reflection
        public List<ColumnInfo> GetColumns<T>()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CreateSelectCommand
        public static IDbCommand CreateSelectCommand<T>()
        {
            return CreateSelectCommand<T>(ColumnNames);
        }
        public static IDbCommand CreateSelectCommand<T>(IEnumerable<string> Columns)
        {
            return CreateSelectCommand<T>(Columns, null);
        }
        public static IDbCommand CreateSelectCommand<T>(IEnumerable<string> Columns, string WhereClause)
        {
            return CreateSelectCommand<T>(Columns, WhereClause, DataSource.Current);
        }
        public static IDbCommand CreateSelectCommand<T>(IEnumerable<string> Columns, string WhereClause, DataSource DataSource)
        {
            return DataSource.CreateSelectCommand<T>(Columns, WhereClause, null, 0, -1);
        }
        #endregion

        #region Load
        protected static List<T> Load<T>(IDataReader dataReader) where T : DatabaseEntity
        {
            var type = typeof(T);
            var constructor = type.GetConstructor(new Type[] { typeof(IDataReader) });

            //Create a collection and construct an object for each row in the data reader
            var rtn = new List<T>();
            while (dataReader.Read())
                rtn.Add((T)constructor.Invoke(new object[] { dataReader }));
            return rtn;
        }

        protected internal void Load(IDataReader dataReader)
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

            var command = ds.CommandProvider.GetTableSave(type);
            command.Connection = ds.Connection;
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
                    ds.ExecuteNonQuery(command);
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