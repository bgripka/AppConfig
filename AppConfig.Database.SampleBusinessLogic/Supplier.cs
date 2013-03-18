using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfig.Database.SampleBusinessLogic
{
    [Table("Suppliers")]
    public class Supplier : DatabaseEntity
    {
        #region Database Columns
        [Column(1, IsIdentity=true)]
        public int SupplierID { get; set; }

        [Column(2, Length=40)]
        public string CompanyName { get; set; }

        [Column(3, Length = 30)]
        public string ContactName { get; set; }

        [Column(4, Length = 30)]
        public string ContactTitle { get; set; }
        #endregion
    }
}
