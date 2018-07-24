using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frmFail : Form
    {
        private string mstr_error;

        public frmFail()
        {
            InitializeComponent();
            mstr_error = "";
        }

        public string faMessage
        {
            get 
            {
                return mstr_error; 
            }
            set 
            {
                mstr_error = value;
                this.txtFailure.Text = mstr_error; 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

    }
}