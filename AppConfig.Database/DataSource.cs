using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using AppConfig.Database.Reflection;
using System.Reflection;
using System.Linq.Expressions;

namespace AppConfig.Database
{
    /// <summary>
    /// The primary source for delivering data between the database and CLR class objects.
    /// </summary>
    public abstract class DataSource
    {
        #region Constructors
        protected DataSource()
        {
            var type = this.GetType();
            var ConnectionStringName = type.FullName + ", " + type.Assembly.GetName().Name;

            //Read the connection string object from the configuration file
            var connectionStringObject = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connectionStringObject == null || string.IsNullOrEmpty(connectionStringObject.ConnectionString))
                throw new Exception("The required connection string object is not defined in the application configuration file.  To correct this error define a connection string object in 'configuration\\connectionStrings' with the name '" + ConnectionStringName + "'.");

            var connectionType = Type.GetType(connectionStringObject.ProviderName);
            DataAdapter = Activator.CreateInstance(connectionType, connectionStringObject.ConnectionString) as DataAdapter;
        }
        protected DataSource(DataAdapter DataAdapter)
        {
            this.DataAdapter = DataAdapter;
        }
        #endregion

        #region Static
        public static T GetCurrentDataSource<T>() where T : DataSource
        {
            return (T)GetCurrentDataSource(typeof(T));
        }
        /// <summary>
        /// Gets or creates and returns the current instance of a DataSource object for the type given.
        /// </summary>
        /// <param name="DataSourceType"></param>
        /// <returns></returns>
        public static DataSource GetCurrentDataSource(Type DataSourceType)
        {
            DataSource rtn;
            //Return the current instance if it exists
            if (currentDataSources.TryGetValue(DataSourceType, out rtn))
                return rtn;

            //Get the constructor for this DataSourceType
            var constructor = DataSourceType.GetConstructor(Type.EmptyTypes);
            if(constructor == null)
                throw new NotImplementedException("The DataSource Type '" + DataSourceType.FullName + "' requires an empty constructor to use this function.");

            //Create the new object using the empty constructor
            rtn = (DataSource)constructor.Invoke(new object[] { });
            
            //Add the new data source to the collection so it isn't created again
            currentDataSources.Add(DataSourceType, rtn);

            return rtn;
        }
        private static Dictionary<Type, DataSource> currentDataSources = new Dictionary<Type, DataSource>();
        #endregion

        #region Properties
        public DataAdapter DataAdapter { get; private set; }
        #endregion

        #region Connection Methods
        public IDbCommand CreateCommand()
        {
            return DataAdapter.Connection.CreateCommand();
        }
        public IDbCommand CreateCommandFromText(string SqlCommandText)
        {
            var rtn = CreateCommand();
            rtn.CommandType = System.Data.CommandType.Text;
            rtn.CommandText = SqlCommandText;
            return rtn;
        }

        /// <summary>
        /// Loads an assembly resource file as the CommandText into a new Command object for the current data source.
        /// </summary>
        /// <param name="ResourceName">Assembly qualified name of the resource file</param>
        /// <returns>Database command object ready for execution</returns>
        public IDbCommand CreateCommandFromResource(string ResourceName)
        {
            throw new NotImplementedException();
            //return CreateCommandFromText([ResourceString]);
        }
        #endregion

        #region Where Clause
        /// <summary>
        /// Takes a where statement expressed in property names and validates and translates to a where clause valid for the data source.
        /// </summary>
        /// <param name="WhereClause"></param>
        /// <returns></returns>
        public string TranslateWhereClause<T>(Expression<Func<T, bool>> WhereClause) where T : DatabaseEntity
        {
            return WhereClause.Body.ToString();
        }

        /// <summary>
        /// Takes a where statement expressed in property names and checks that it is valid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereClause"></param>
        /// <returns>True if the statement is valid or false if it is not</returns>
        public bool ValidateWhereClause<T>(Expression<Func<T, bool>> WhereClause) where T : DatabaseEntity
        {
            try
            {
                TranslateWhereClause<T>(WhereClause);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
