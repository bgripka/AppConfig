using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AppConfig.Windows.Forms.UI
{
    public partial class PropertyGridEditor : Form
    {
        public PropertyGridEditor(string FormTitle, object EditObject)
        {
            InitializeComponent();

            this.Text = FormTitle;
            this.pgMain.SelectedObject = EditObject;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveRequested == null)
                return;
            SaveRequested.Invoke(this, new SaveRequestEventArgs(pgMain.SelectedObject));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public EventHandler<SaveRequestEventArgs> SaveRequested;

    }

    public class SaveRequestEventArgs : EventArgs
    {
        public SaveRequestEventArgs(object DataObject)
        {
            this.DataObject = DataObject;
        }

        public object DataObject { get; private set; }
    }
}
