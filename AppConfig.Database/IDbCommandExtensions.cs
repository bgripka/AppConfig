using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace AppConfig.Database
{
    public static class IDbCommandExtensions
    {
        #region Json
        public static string ToJson(this IDbCommand command)
        {
            var closeConnection = false;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var jsonWriter = new JsonTextWriter(sw);

            // read the row from the table
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    closeConnection = true;
                    command.Connection.Open();
                }
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    jsonWriter.WriteStartObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        jsonWriter.WritePropertyName(reader.GetName(i)); // column name
                        jsonWriter.WriteValue(reader.GetValue(i)); // value in column
                    }
                    jsonWriter.WriteEndObject();
                }

                reader.Close();
            }
            finally
            {
                if (closeConnection)
                    command.Connection.Close();
            }

            return sb.ToString();
        }
        #endregion

        #region AddParameter
        public static IDbDataParameter AddParameter(this IDbCommand command, string ParameterName, DbType DbType)
        {
            return addParameter(command, ParameterName, DbType, null);
        }
        public static IDbDataParameter AddParameter(this IDbCommand command, string ParameterName, DbType DbType, int Size)
        {
            return addParameter(command, ParameterName, DbType, Size);
        }
        private static IDbDataParameter addParameter(IDbCommand command, string ParameterName, DbType DbType, int? Size)
        {
            var rtn = command.CreateParameter();
            rtn.ParameterName = ParameterName;
            rtn.DbType = DbType;
            if (Size.HasValue)
                rtn.Size = Size.Value;
            command.Parameters.Add(rtn);
            return rtn;
        }
        #endregion

        #region Execute Managed
        /// <summary>
        /// Executes a non query command just like the ExecuteNonQuery function but manages opening and closing
        /// the connection if required and provides detailed error messages if the command fails.
        /// </summary>
        /// <param name="Command"></param>
        public static void ExecuteNonQueryManaged(this IDbCommand Command)
        {
            if (Command.Connection == null)
                throw new Exception("To execute this command a connection object is required.");

            bool closeConnection = false;
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

        /// <summary>
        /// Executes a reader command just like the ExecuteReader function but manages opening and closing
        /// the connection if required and provides detailed error messages if the command fails.
        /// </summary>
        /// <param name="Command"></param>
        public static IDataReader ExecuteReaderManaged(this IDbCommand Command)
        {
            if (Command.Connection == null)
                throw new Exception("To execute this command a connection object is required.");

            bool closeConnection = false;
            if (Command.Connection.State == ConnectionState.Closed)
            {
                Command.Connection.Open();
                closeConnection = true;
            }

            try
            {
                return Command.ExecuteReader();
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

        #region ExecuteDataTable
        public static DataSet ExecuteToDataSet(this IDbCommand command)
        {
            var rtn = new DataSet();
            var da = command.Connection.GetDataAdapter();
            da.SelectCommand = command;

            bool closeConnection = false;
            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                closeConnection = true;
            }
            try
            {
                da.Fill(rtn);
                return rtn;
            }
            finally
            {
                if (closeConnection)
                    command.Connection.Close();
            }
        }
        public static DataTable ExecuteToDataTable(this IDbCommand command)
        {
            var ds = ExecuteToDataSet(command);
            if (ds.Tables.Count == 0)
                return null;
            else if (ds.Tables.Count > 1)
                throw new Exception("More than one table was returned by the database command.  Consider using 'ExecuteDataTables' and returning a DataSet.");
            else
                return ds.Tables[0];
        }
        #endregion
    }
}
