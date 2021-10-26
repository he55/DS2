using System;
using System.Windows.Forms;

namespace DreamScene2
{
    public partial class InputDialog : Form
    {
        public InputDialog()
        {
            InitializeComponent();
            this.Icon = DreamScene2.Properties.Resources.ico3;
        }

        public string URL => textBox1.Text;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
