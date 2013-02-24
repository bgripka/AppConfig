using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace AppConfig.Database
{
    public static class IDbConnectionExtensions
    {
        public static IDbDataAdapter GetDataAdapter(this IDbConnection connection)
        {
            if (connection is System.Data.SqlClient.SqlConnection)
                return new System.Data.SqlClient.SqlDataAdapter();
            else
                throw new NotSupportedException();
        }
    }
}
