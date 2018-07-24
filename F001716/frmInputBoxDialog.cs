using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frmInputBoxDialog : Form
    {
        public frmInputBoxDialog()
        {
            InitializeComponent();
        }

#region Private Variables
        string formCaption = string.Empty;
        string formPrompt = string.Empty;
        string inputResponse = string.Empty;
        string defaultValue = string.Empty;
        Boolean bln_Abort = false;
#endregion

#region Public Properties
        public string FormCaption
        {
            get { return formCaption; }
            set { formCaption = value; }
        } // property FormCaption
        public string FormPrompt
        {
            get { return formPrompt; }
            set { formPrompt = value; }
        } // property FormPrompt
        public string InputResponse
        {
            get { return inputResponse; }
            set { inputResponse = value; }
        } // property InputResponse
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        } // property DefaultValue

        public Boolean UserAbort
        {
            get { return bln_Abort; }
            set { bln_Abort = value; }
        } // property UserAbort


#endregion

#region Form and Control Events
        private void InputBox_Load(object sender, System.EventArgs e)
        {
            this.txtInput.Text = defaultValue;
            this.lblPrompt.Text = formPrompt;
            this.Text = formCaption;
            this.txtInput.SelectionStart = 0;
            this.txtInput.SelectionLength = this.txtInput.Text.Length;
            this.txtInput.Focus();
        }


        private void btnOK_Click(object sender, System.EventArgs e)
        {
            InputResponse = this.txtInput.Text;
            this.Hide();
            //this.Close();
        }
        private void btnOK_KeyPress(object sender, KeyPressEventArgs e)      
        {          
            if (e.KeyChar == 13)  //13表示回车键              
                btnOK.PerformClick();      
        }
        private void btncancel_Click(object sender, System.EventArgs e)
        {
            bln_Abort = true;
            this.Hide(); 
            //this.Close();
        }

        private void txtInputKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys .F1 )
                this.txtInput.Text = "MCF-CCB000000000";
            if (e.KeyCode == Keys.F2)
                this.txtInput.Text = "MCF-CCB010000000";

            if (e.KeyCode == Keys.Enter)
            {
                //InputResponse = this.txtInput.Text.Trim.ToUpper;
                if (this.txtInput.Text.Length!=16) 
                {
                    this.txtInput.Text = this.txtInput.Text + "Invalid MCB";
                    SendKeys.Send("{Home}+{End}");
                }
                else
                {
                    //InputResponse = this.txtInput.Text;
                    //this.Hide(); 
                    btnOK.Focus();
                }
            }

        }
#endregion
    }
}