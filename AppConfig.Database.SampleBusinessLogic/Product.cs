using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfig.Database.SampleBusinessLogic
{
    [Table("Products")]
    public class Product : DatabaseEntity
    {
        #region Database Columns
        [Column(1, IsIdentity = true)]
        public int ProductID { get; set; }

        [Column(2, Length = 40, Nullable = false)]
        public string ProductName { get; set; }

        //[Column(3, Nullable = false)]
        //public int SupplierID { get; set; }

        //[Column(4, Nullable = false)]
        //public int? CategoryID { get; set; }
        #endregion

        public override string ToString()
        {
            return ProductName;
        }

    }
}
