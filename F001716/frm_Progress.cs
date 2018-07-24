using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frm_Progress : Form
    {
        // The delegate
        //public delegate void UpdateProgressDelegate(int value);
        //string m_filename = "";

        public string prgFilename
        {
            //set { m_filename = value; }
            set { this.label1.Text = value; }
        }

        public frm_Progress()
        {
            InitializeComponent();
            Init();
        }

        ~frm_Progress()
        {
            int a = 0;
        }

        //public int prgValue
        //{
        //    get { return this.progressBar1.Value; }
        //    set
        //    {
        //        this.progressBar1.Value = value;
        //        this.progressBar1.Refresh();
        //    }
        //}

        public void UpdateProgress(int value)
        {
            this.progressBar1.Value = value;
        }
        
        private void Init()
        {
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.label1.Text = "";

            // The delegate member
            //UpdateProgressDelegate UpdateProgress = new UpdateProgressDelegate(UpdateProgressSafe);
        }

        private void Progress_Load(object sender, EventArgs e)
        {
            int a = 0;
        }

        private void Progress_FormClosing(object sender, FormClosingEventArgs e)
        {
            int a = 0;
        }

    }
}