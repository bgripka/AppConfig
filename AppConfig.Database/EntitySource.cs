using System;
using System.Collections.Generic;
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

        }

        private DataSource dataSource;

        #region Single
        public T Single(string Filter)
        {
            var rtn = SingleOrDefault(Filter);
            if (rtn == null)
                throw new Exception();
            return rtn;
        }
        public T SingleOrDefault(string Filter)
        {
            throw new NotImplementedException();
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
        }
        #endregion

    }
