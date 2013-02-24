using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using AppConfig.Database.Reflection;

namespace AppConfig.Database
{
    public abstract class DataSource
    {
        #region Static
        public static DataSource Current
        {
            get 
            {
                if (current != null)
                    return current;

                var connectionStringObject = ConfigurationManager.ConnectionStrings["AppConfig.Database.DataSource.ConnectionString"];
                if (connectionStringObject == null || string.IsNullOrEmpty(connectionStringObject.ConnectionString))
                    throw new Exception("The required connection string object is not defined in the application configuration file.  To correct this error define a connection string object in 'configuration\\connectionStrings' with the name 'AppConfig.Database.DataSource.ConnectionString' and the DataSource CRL type name in the 'providerName' setting.");

                var connectionType = Type.GetType(connectionStringObject.ProviderName);
                current = Activator.CreateInstance(connectionType, connectionStringObject.ConnectionString) as DataSource;
                return current;
            }
        }
        private static DataSource current;

        public static string GetConnectionString(Type type)
        {
            var ConnectionStringName = type.FullName + ".ConnectionString";

            //Read the connection string object from the configuration file
            var connectionStringObject = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connectionStringObject == null || string.IsNullOrEmpty(connectionStringObject.ConnectionString))
                throw new Exception("The required connection string object is not defined in the application configuration file.  To correct this error define a connection string object in 'configuration\\connectionStrings' with the name '" + ConnectionStringName + "'.");

            return connectionStringObject.ConnectionString;
        }
        #endregion

        #region Constructors
        protected DataSource(IDbConnection Connection, ICommandProvider CommandProvider)
        {
            this.Connection = Connection;
            if(string.IsNullOrEmpty(Connection.ConnectionString))
                Connection.ConnectionString = GetConnectionString(this.GetType());
            this.CommandProvider = CommandProvider;
        }
        #endregion

        #region Properties
        public IDbConnection Connection { get; private set; }
        public ICommandProvider CommandProvider { get; private set; }

        public HashSet<Type> EntityTypes
        {
            get
            {
                if (entityTypes != null)
                    return entityTypes;

                entityTypes = new HashSet<Type>();

                //Get all properties that implement the EntitySouce generic type
                var entitySources = this.GetType().GetProperties()
                    .Where(a => a.PropertyType.IsGenericType && a.PropertyType.GetGenericTypeDefinition() == typeof(EntitySource<>));

                foreach (var entitySouce in entitySources)
                {
                    var entityType = entitySouce.PropertyType.GetGenericArguments()[0];

                    //Check to see if this entity has already been added.
                    if (entityTypes.Contains(entityType))
                        continue;

                    //Add this entity type to the collection
                    entityTypes.Add(entityType);

                    //Add all dependant entities
                    var dependantTypes = DatabaseEntity.GetRelatedTypes(entityType);
                    foreach (var dependantType in dependantTypes)
                        if (!entityTypes.Contains(dependantType))
                            entityTypes.Add(dependantType);
                }

                return entityTypes;
            }
        }
        private HashSet<Type> entityTypes;

        public HashSet<OneToManyRelationshipAttribute> OneToManyRelationships
        {
            get
            {
                if (oneToManyRelationships != null)
                    return oneToManyRelationships;

                oneToManyRelationships = OneToManyRelationshipAttribute.GetOneToManyRelationships(EntityTypes);
                return oneToManyRelationships;
            }
        }
        private HashSet<OneToManyRelationshipAttribute> oneToManyRelationships;
        #endregion

        #region Create
        public string GetCreateSchemaCommandText()
        {
            var sb = new StringBuilder();

            //Create all tables
            foreach (var entityType in entityTypes)
                sb.AppendLine(CommandProvider.GetCreateTable(entityType));

            //Constraints go after all tables have been created
            foreach (var entityType in entityTypes)
                sb.AppendLine(CommandProvider.GetCreateTableConstraints(entityType));

            return sb.ToString();
        }
        #endregion

        #region Get
        public List<E> GetEntities<E>(IDbCommand Command) where E : DatabaseEntity
        {
            bool closeConnection = false;

            //Get the constructor that takes an IDbReader as its only parameter
            var constructorInfo = typeof(E).GetConstructor(new Type[] { this.GetType(), typeof(IDataReader) });
            
            //Set the connection object for this command to this connection
            Command.Connection = Connection;

            if (Command.Connection.State == ConnectionState.Closed)
            {
                Command.Connection.Open();
                closeConnection = true;
            }

            try
            {
                var reader = Command.ExecuteReader();

                //Create the return collection and invoke the constructor for each result in the reader
                var rtn = new List<E>();
                while (reader.Read())
                {
                    var entity = Activator.CreateInstance(typeof(E)) as E;
                    entity.DataSource = this;
                    entity.Load(reader);
                    rtn.Add(entity);
                }
                return rtn;
            }
            catch (Exception ex)
            {
                throw new DbCommandException(Command, ex);
            }
            finally
            {
                if (closeConnection)
                    Command.Connection.Close();
            }
        }
        #endregion

        #region Connection Methods
        public IDbCommand CreateCommand()
        {
            return Connection.CreateCommand();
        }
        public IDbCommand CreateCommandFromText(string SqlCommandText)
        {
            var rtn = CreateCommand();
            rtn.CommandType = System.Data.CommandType.Text;
            rtn.CommandText = SqlCommandText;

            return rtn;
        }

        public IDbCommand CreateCommandFromResource(string ResourceName)
        {
            var rtn = Connection.CreateCommand();

            return rtn;
        }

        public void ExecuteNonQuery(IDbCommand Command)
        {
            bool closeConnection = false;

            //Set the connection object for this command to this connection
            Command.Connection = Connection;

            if (Command.Connection.State == ConnectionState.Closed)
            {
                Command.Connection.Open();
                closeConnection = true;
            }

            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DbCommandException(Command, ex);
            }
            finally
            {
                if (closeConnection)
                    Command.Connection.Close();
            }
        }
        #endregion

        #region Create Commands
        public abstract IDbCommand CreateSelectCommand<T>(IEnumerable<string> Columns, string WhereClause, string OrderByClause, int Skip, int Take);        
        #endregion
    }
}
