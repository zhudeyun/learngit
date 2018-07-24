using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frmDialog : Form
    {
        private string m_message = null;
        private string m_title = null;

        public string dgTitle
        {
            get { return m_title; }
            set { m_title = value; }
        }

        public string dgPrompt
        {
            get { return m_message; }
            set { m_message = value; }
        }

        public MessageBoxButtons dgPromptType
        {
            set
            {
                switch (value)
                {
                    case MessageBoxButtons.OK:
                        this.btnOk.Visible = true;
                        this.btnRetry.Visible = false;
                        this.btnCancel.Visible = false;
                        break;
                    case MessageBoxButtons.OKCancel:
                        this.btnOk.Visible = true;
                        this.btnRetry.Visible = false;
                        this.btnCancel.Visible = true;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        this.btnOk.Visible = true;
                        this.btnRetry.Visible = true;
                        this.btnCancel.Visible = true;
                        break;
                    case MessageBoxButtons.YesNo:
                        this.btnOk.Visible = true;
                        this.btnOk.Text = "Yes";
                        this.btnRetry.Visible = false;
                        this.btnCancel.Visible = true;
                        this.btnCancel.Text = "No";
                        break;
                    default:
                        this.btnOk.Visible = false;
                        this.btnRetry.Visible = false;
                        this.btnCancel.Visible = false;
                        break;
                }
                //if (this.Visible == false)
                //    this.ShowDialog();
                this.Refresh();
            }
        }

        public DialogResult dgResult
        {
            get { return this.DialogResult; }
            set { this.DialogResult = value; }
        }

        public frmDialog()
        {
            InitializeComponent();
        }

        public void InitButtons(string txtCancel, string txtOk, string txtRetry)
        {
            this.btnCancel.Text = txtCancel;
            this.btnOk.Text = txtOk;
            this.btnRetry.Text = txtRetry;
        }

        private void Dialog_Load(object sender, EventArgs e)
        {
            this.label1.Text = m_title;
            this.textBox1.Text = m_message;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Hide();
        }
   }
}