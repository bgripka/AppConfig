using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database
{
    public class EntitySource<T> where T : DatabaseEntity
    {
        public List<T> Where(string Filter)
        {
            throw new NotImplementedException();
        }
        public List<T> Where(string Filter, params string[] Columns)
        {
            throw new NotImplementedException();
        }
        public List<T> Where(string Filter, int Skip, int Take)
        {
            throw new NotImplementedException();
        }
    }
}
