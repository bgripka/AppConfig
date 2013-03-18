using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppConfig.Database;
using AppConfig.Database.SqlServer;

namespace AppConfig.Database.SampleBusinessLogic
{
    public class Northwind : AppConfig.Database.DataSource
    {
        #region Static Properties
        public static Northwind Current
        {
            get
            {
                if (current == null)
                    current = new Northwind();
                return current;
            }
        }
        private static Northwind current;
        #endregion

        #region Constructors
        public Northwind()
        {
            Initialize();
        }
        public Northwind(DataAdapter DataAdapter)
            : base(DataAdapter)
        {
            Initialize();
        }
        private void Initialize()
        {
            Suppliers = new EntitySource<Supplier>(this);
            Products = new EntitySource<Product>(this);
        }
        #endregion

        #region DataEntities
        public EntitySource<Supplier> Suppliers { get; private set; }
        public EntitySource<Product> Products { get; private set; }
        #endregion
    }
}
