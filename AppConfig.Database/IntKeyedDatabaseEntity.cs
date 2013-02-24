using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database
{
    [PrimaryKey("Id")]
    public class IntKeyedDatabaseEntity : DatabaseEntity
    {
        protected IntKeyedDatabaseEntity() { }
        protected IntKeyedDatabaseEntity(int Id) { this.Id = Id; }

        [Column(1, Nullable = false)]
        public int Id { get; protected set; }
    }

}
