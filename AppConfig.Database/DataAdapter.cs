using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfig.Database
{
    public abstract class DataAdapter
    {
        #region Static
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
        protected DataAdapter(IDbConnection Connection, ICommandProvider CommandProvider)
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


    }
}
