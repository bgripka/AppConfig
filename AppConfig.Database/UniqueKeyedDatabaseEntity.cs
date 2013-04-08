using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AppConfig.Database
{
    public abstract class UniqueKeyedDatabaseEntity : DatabaseEntity
    {
        protected UniqueKeyedDatabaseEntity() { Id = Guid.NewGuid(); }
        protected UniqueKeyedDatabaseEntity(Guid Id) { this.Id = Id; }

        [Column(1, IsInPrimaryKey=true, Nullable = false)]
        public Guid Id { get; protected set; }
    }
}
