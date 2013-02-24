using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;

namespace AppConfig.Database
{
    public class JsonResultBuilder<T> : IJsonResultBuilder //where T:DatabaseEntity,
    {
        public JsonResultBuilder()
        {
            Columns = new List<string>();
        }

        public List<string> Columns { get; private set; }
        public string WhereClause { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public string GenerateOutput()
        {
            return DatabaseEntity.CreateSelectCommand<T>(Columns, WhereClause).ToJson();
        }
    }

    public interface IJsonResultBuilder
    {
        List<string> Columns { get; }
        string WhereClause { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        string GenerateOutput();
    }
}
