using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frmLogin : Form
    {
        private string m_Password = "";
        private string m_PasswordEntered = "";

        public string lgPassword
        {
            set
            {
                m_Password = value;
                m_PasswordEntered = "";
            }
        }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            // Make sure the user entered something.
            if (UsernameTextBox.Text.Length == 0)
            {
                MessageBox.Show("You must enter a user name");
                UsernameTextBox.Focus();
            }
            else if (PasswordTextBox.Text.Length == 0)
            {
                MessageBox.Show("You must enter a password");
                PasswordTextBox.Focus();
            }
            else if (!PasswordValid(UsernameTextBox.Text, PasswordTextBox.Text))
            {
                // The user name/password is invalid.
                MessageBox.Show("User name/password invalid");
                UsernameTextBox.Focus();
            }
            else
            {
                // The user name/password is valid.
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }            
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool PasswordValid(string user_name, string psword)
        {
            m_PasswordEntered = psword;
            return ((user_name.ToLower() == "tech") & (psword.ToLower() == m_Password.ToLower()));
        }
    }
}