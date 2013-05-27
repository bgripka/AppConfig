using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppConfig.Database.SampleBusinessLogic;

namespace AppConfig.Database.SampleApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void cmbCallFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbCallFunction.Text)
            {
                case "Get Product by Name":
                    BindToTreeView(Northwind.Current.Products.Where(a => new string[] { "ToFu", "Filo Mix" }.Contains(a.ProductName)));
                    break;
                case "Order By Product Name":
                    BindToTreeView(Northwind.Current.Products.Select(a => new { a.ProductID, a.ProductName }, a => a.ProductName, 0, 50));
                    break;
                case "Save New Product":
                    var product = new Product()
                    {
                        ProductName = "New Book"
                    };
                    product.Save();
                    break;
            }
        }

        private void BindToTreeView(IEnumerable<object> Data)
        {
            tvObjectView.BeginUpdate();
            try
            {
                tvObjectView.Nodes.Clear();

                foreach (var item in Data)
                    TreeViewAppendObject(item, tvObjectView.Nodes);
            }
            finally
            {
                tvObjectView.EndUpdate();
            }
        }
        private void BindToTreeView(object Data)
        {
            tvObjectView.BeginUpdate();
            try
            {
                tvObjectView.Nodes.Clear();
                TreeViewAppendObject(Data, tvObjectView.Nodes);
            }
            finally
            {
                tvObjectView.EndUpdate();
            }
        }
        private void TreeViewAppendObject(object Data, TreeNodeCollection Target)
        {
            Target.Add(new TreeNode()
            {
                Text = Data.ToString(),
                Tag = Data
            });
        }
    }
}
