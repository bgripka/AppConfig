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
            return Where(Filter, 0, -1, null);
        }
        public List<T> Where(Expression<Func<T, bool>> Filter, params string[] Properties)
        {
            return Where(Filter, 0, -1, Properties);
        }
        public List<T> Where(Expression<Func<T, bool>> Filter, int Skip, int Take)
        {
            return Where(Filter, Skip, Take, null);
        }
        public List<T> Where(Expression<Func<T, bool>> Filter, int Skip, int Take, params string[] Properties)
        {
            var command = this.dataSource.DataAdapter.CommandProvider.CreateSelectCommand<T>(
                Filter, null, Skip, Take, Properties);
            return Load(command.ExecuteReader());
        }
        #endregion

    }
}