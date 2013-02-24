using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppConfig.Windows.Forms.UI
{
    public partial class RichTextMessageBox : Form
    {
        private MessageBoxButtons buttons;

        private RichTextMessageBox()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string RichTextMessage, string StaticMessage, string WindowTitle, MessageBoxButtons Buttons)
        {
            RichTextMessageBox messageBox = new RichTextMessageBox();
            messageBox.rtbMessage.Text = RichTextMessage;
            messageBox.lblStaticMessage.Text = StaticMessage;
            messageBox.Text = WindowTitle;
            messageBox.buttons = Buttons;

            switch (Buttons)
            {
                case MessageBoxButtons.OK:
                    messageBox.button1.Visible = false;
                    messageBox.button2.Visible = false;
                    messageBox.button3.Text = "OK";
                    messageBox.button3.Tag = DialogResult.OK;
                    break;
                case MessageBoxButtons.OKCancel:
                    messageBox.button1.Visible = false;
                    messageBox.button2.Text = "OK";
                    messageBox.button2.Tag = DialogResult.OK;
                    messageBox.button3.Text = "Cancel";
                    messageBox.button3.Tag = DialogResult.Cancel;
                    break;
                default:
                    throw new NotImplementedException("Buttons option '" + Convert.ToString(Buttons) + "' is not supported.");
            }

            return messageBox.ShowDialog();
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)((Button)sender).Tag;
            this.Close();
        }
    }
}
