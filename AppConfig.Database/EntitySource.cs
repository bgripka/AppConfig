using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AppConfig.Database
{
    /// <summary>
    /// Associations a DatabaseEntity type with a DataSource through a public collection property of the DataSource.
    /// Define a property on the DataSource object that returns this type to add T to the database schema.
    /// </summary>
    /// <typeparam name="T">Any DatabaseEntity type</typeparam>
    public class EntitySource<T> where T : DatabaseEntity
    {
        public EntitySource(DataSource DataSource)
        {
            this.dataSource = DataSource;
        }

        private DataSource dataSource;

        #region Load
        public List<T> Load(IDataReader dataReader)
        {
            var type = typeof(T);
            //Get the empty constructor for this type
            var constructor = type.GetConstructor(Type.EmptyTypes);

            //Create a collection and construct an object for each row in the data reader
            var rtn = new List<T>();
            while (dataReader.Read())
            {
                T dataItem = (T)constructor.Invoke(new object[0]);
                dataItem.DataSource = dataSource;
                dataItem.Load(dataReader);
                rtn.Add(dataItem);
            }
            return rtn;
        }
        #endregion

        #region OrderBy
        public List<T> OrderBy(Expression<Func<T, dynamic>> OrderBy)
        {
            return Where(null, OrderBy, -1, -1);
        }
        public List<T> OrderBy(Expression<Func<T, dynamic>> OrderBy, int Skip, int Take)
        {
            return Where(null, OrderBy, Skip, Take);
        }
        #endregion

        #region Select
        public List<T> Select(Expression<Func<T, dynamic>> Properties)
        {
            return Where(null, Properties, null, -1, -1);
        }
        public List<T> Select(Expression<Func<T, dynamic>> Properties, Expression<Func<T, dynamic>> OrderBy, int Skip, int Take)
        {
            return Where(null, Properties, OrderBy, Skip, Take);
        }
        #endregion

        #region Single
        public T Single(Expression<Func<T, bool>> Filter)
        {
            var rtn = SingleOrDefault(Filter);
            if (rtn == null)
                throw new InvalidOperationException("The input sequence is empty.");
            return rtn;
        }
        public T SingleOrDefault(Expression<Func<T, bool>> Filter)
        {
            return SingleOrDefault(Filter, null);
        }
        public T SingleOrDefault(Expression<Func<T, bool>> Filter, T Default)
        {
            var result = Where(Filter);
            if (result.Count == 1)
                return result.First();
            else if (result.Count == 0)
                return null;
            else
                throw new InvalidOperationException("The input sequence contains more than one element.");
        }
        #endregion

        #region Where
        public List<T> Where(Expression<Func<T, bool>> Filter)
        {
            return Where(Filter, null, null, 0, -1);
        }
        public List<T> Where(Expression<Func<T, bool>> Filter, Expression<Func<T, dynamic>> Properties)
        {
            return Where(Filter, null, null, 0, -1);
        }
        public List<T> Where(Expression<Func<T, bool>> Filter, Expression<Func<T, dynamic>> OrderBy, int Skip, int Take)
        {
            return Where(Filter, null, OrderBy, Skip, Take);
        }
        public List<T> Where(Expression<Func<T, bool>> Filter, Expression<Func<T, dynamic>> Properties, Expression<Func<T, dynamic>> OrderBy, int Skip, int Take)
        {
            var command = this.dataSource.DataAdapter.CommandProvider.CreateSelectCommand<T>(
                Properties, Filter, OrderBy, Skip, Take);
            command.Connection = dataSource.DataAdapter.Connection;
            bool closeConnection = (command.Connection.State != ConnectionState.Open);
            try
            {
                if (closeConnection)
                    command.Connection.Open();

                var r = command.ExecuteReader();
                try
                {
                    return Load(r);
                }
                finally
                {
                    r.Close();
                }
            }
            finally
            {
                if (closeConnection)
                    command.Connection.Close();
            }
        }
        #endregion

    }
}