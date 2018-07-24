using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frmPass : Form
    {
        public frmPass()
        {
            InitializeComponent();
        }

        public string SoftwareNumber
        {
            set { lblSoftwareNumber.Text = value; }
        }

        public string paSerialNumber
        {
            set { lblSerialNumber.Text = value; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}