using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace F001716
{
    public partial class frm_SoftwareInfo : Form
    {
        private enum enSW { App, Config, EngineID, Image }

        private string mstr_PC_Download_Dir;
        private string mstr_NetworkRootDir;


        private bool mbln_Abort;
        private string mstr_ItemNum;
        private string mstr_MODEL;
        private string mstr_BomRev;
        private string mstr_Description;
        private string mstr_QADLine;

//private string mstr_Product;
        private string mstr_ImageFile;
 

        private string mstr_Report_Checksum;
        private NetworkMapping_t m_NetworkMap;
        public NetworkMapping_t m_NetworkMapping
        {
            get { return m_NetworkMap; }
            set { m_NetworkMap = value; }
        }
        public bool Abort
        {
            get { return mbln_Abort; }
            set { mbln_Abort = value; }
        }
       
        public string BomRev
        {
            get { return mstr_BomRev; }
            set { mstr_BomRev = value; }
        }
        public string ItemNum
        {
            get { return mstr_ItemNum; }
            set { mstr_ItemNum = value; }
        }
        public string MODEL
        {
            get { return mstr_MODEL; }
            set { mstr_MODEL = value; }
        }
        public string Description
        {
            get { return mstr_Description; }
            set { mstr_Description = value; }
        }
        public string QADLine
        {
            get { return mstr_QADLine; }
            set { mstr_QADLine = value; }
        }
        public string Image_SW_Num
        {
            get { return mstr_ImageFile; }
            set { mstr_ImageFile = value; }
        }
        public string Report_Checksum
        {
            get { return mstr_Report_Checksum; }
            set { mstr_Report_Checksum = value; }
        }
        public string DownloadDirectory
        {
            get { return mstr_PC_Download_Dir; }
            set { mstr_PC_Download_Dir = value; }
        }



        public frm_SoftwareInfo()
        {
            InitializeComponent();
        }

        private void frm_SoftwareInfo_Load(object sender, EventArgs e)
        {
            ResetControls();
        }
        private void txt_ScanEntrys_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = new TextBox();
            tb = (TextBox)sender;
            switch (tb.Name)
            {//CM2180MP-BR0//CM3680SR-BW0//CM5680SR-BR0//CM5680SR-CW0
                case "txt_Item":
                    if (e.KeyCode == Keys.F1)
                    {
                        txt_Item.Text = "CM2180MP-BR0";
                    }
                    if (e.KeyCode == Keys.F2)
                    {
                        txt_Item.Text = "CM3680SR-BW0";
                    }
                    if (e.KeyCode == Keys.F3)
                    {
                        txt_Item.Text = "CM5680SR-BR0";
                    }
                    if (e.KeyCode == Keys.F4)
                    {
                        txt_Item.Text = "CM5680SR-CW0";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseItemNum() == true && btn_OK.Enabled == false)
                        {
                            txt_BomRev.Text = "";
                            lbl_BomRev.Visible = true;
                            txt_BomRev.Visible = true;
                            txt_BomRev.Focus();
                        }
                    }
                    break;
                
                case "txt_BomRev":
                    if (e.KeyCode == Keys.F1)
                    {
                        txt_BomRev.Text = "A";
                    }
                    else if (e.KeyCode == Keys.F2)
                    {
                        txt_BomRev.Text = "B";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseBomRev() == true && btn_OK.Enabled == false)
                        {

                            txt_Description.Text = "";
                            lbl_Description.Visible = true;
                            txt_Description.Visible = true;
                            txt_Description.Focus();
                        }
                    }
                    break;
                case "txt_Description"://CM2180MP-BR0//CM3680SR-BW0//CM5680SR-BR0//CM5680SR-CW0
                    if (e.KeyCode == Keys.F1)
                    {
                        txt_Description.Text = "CM2180MP-BR0 Barcode Scanner";
                    }
                    if (e.KeyCode == Keys.F2)
                    {
                        txt_Description.Text = "CM3680SR-BW0 Barcode Scanner ";
                    }
                     if (e.KeyCode == Keys.F3)
                    {
                        txt_Description.Text = "CM5680SR-BR0 Barcode Scanner";
                    }
                    if (e.KeyCode == Keys.F4)
                    {
                        txt_Description.Text = "CM5680SR-CW0 Barcode Scanner ";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseDescription() == true && btn_OK.Enabled == false)
                        {
                            txt_FlashImg.Text = "";
                            lbl_FlashImg.Visible = true;
                            txt_FlashImg.Visible = true;
                            txt_FlashImg.Focus();
                        }
                    }
                    break;
                case "txt_PSOC_REV":
                    break;
                case "txt_FlashImg":

                    if (e.KeyCode == Keys.F1)
                    {
                        txt_FlashImg.Text = "BI001054HAC";
                    }

                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseImage() == true && btn_OK.Enabled == false)
                        {
                            txt_Model.Text = "";
                            lbl_Model.Visible = true;
                            txt_Model.Visible = true;
                            txt_Model.Focus();
                        }
                    }
                    break;
                case "txt_Model":
                    if (e.KeyCode == Keys.F1)
                    {
                        txt_Model.Text = "HDLS_Cradle";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseModel() == true && btn_OK.Enabled == false)
                        {
                            Tex_QAD_Line.Text = "";
                            lbl_QAD_Line.Visible = true;
                            Tex_QAD_Line.Visible = true;
                            Tex_QAD_Line.Focus();
                        }
                    }
                    break;
                case "txt_EngIDCounter":

                    break;
                case "Tex_QAD_Line":
                    if (e.KeyCode == Keys.F1)
                    {
                        Tex_QAD_Line.Text = "CM2180";
                    }
                    if (e.KeyCode == Keys.F2)
                    {
                        Tex_QAD_Line.Text = "CM3680";
                    }
                    if (e.KeyCode == Keys.F3)
                    {
                        Tex_QAD_Line.Text = "CM5680";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseQADLine() == true && btn_OK.Enabled == false)
                        {
                            btn_OK.Enabled = true;
                            btn_OK.Focus();
                        }
                    }
                    break;
                default:
                    break;
            }


        }
        private bool ParseItemNum()
        {
            txt_Item.Text = txt_Item.Text.Trim().ToUpper();
            if (txt_Item.Text.Length > 0)
            {
                mstr_ItemNum = txt_Item.Text;
                return true;
            }
            else
            {
                MessageBox.Show("Scan Entry Error: Invalid Item Number!");
                txt_Item.Focus();
                SendKeys.Send("{Home}+{End}");
                return false;
            }
        }
       
        private bool ParseBomRev()
        {
            txt_BomRev.Text = txt_BomRev.Text.Trim().ToUpper();
            if (txt_BomRev.Text.Length > 0)
            {
                mstr_BomRev = txt_BomRev.Text;
                return true;
            }
            else
            {
                MessageBox.Show("Scan Entry Error: Invalid BOM Rev!");
                txt_BomRev.Focus();
                SendKeys.Send("{Home}+{End}");
                return false;
            }
        }
        private bool ParseDescription()
        {
            txt_Description.Text = txt_Description.Text.Trim().ToUpper();
            if (txt_Description.Text.Length > 0)
            {
                mstr_Description = txt_Description.Text;
                return true;
            }
            else
            {
                MessageBox.Show("Scan Entry Error: Invalid Description!");
                txt_Description.Focus();
                SendKeys.Send("{Home}+{End}");
                return false;
            }
        }
        private bool ParseQADLine()
        {
            Tex_QAD_Line.Text = Tex_QAD_Line.Text.Trim().ToUpper();
            if (Tex_QAD_Line.Text.Length > 0)
            {
                mstr_QADLine = Tex_QAD_Line.Text;
                return true;
            }
            else
            {
                MessageBox.Show("Scan Entry Error: Invalid QAD Line!");
                Tex_QAD_Line.Focus();
                SendKeys.Send("{Home}+{End}");
                return false;
            }
        }
        private bool ParseImage()
        {
            string str_File = "";
            txt_FlashImg.Text = txt_FlashImg.Text.Trim().ToUpper();
            if (txt_FlashImg.Text.Length < 1)
            {
                MessageBox.Show("Scan Entry Error:Invalid Flash Image Number!");
                txt_FlashImg.Focus();
                SendKeys.Send("{Home}+{End}");
                return false;
            }

            mstr_ImageFile = txt_FlashImg.Text;
            str_File = mstr_ImageFile;

            return true;
        }
        private bool ParseModel()
        {
            string str_File = "";
            txt_Model.Text = txt_Model.Text.Trim().ToUpper();
            if (txt_Model.Text.Length < 1)
            {
                MessageBox.Show("Scan Entry Error:Invalid Flash Image Number!");
                txt_FlashImg.Focus();
                SendKeys.Send("{Home}+{End}");
                return false;
            }

            mstr_MODEL = txt_Model.Text;


            return true;
        }

        public void ResetControls()
        {
            mbln_Abort = false;
            btn_OK.Enabled = false;
            mstr_PC_Download_Dir = System.IO.Directory.GetCurrentDirectory() + "\\Download";
            mstr_NetworkRootDir = m_NetworkMap.I_M_EDir + "1450";// "H:\\I-M-E\\1250G";

            mstr_ItemNum = "";
            mstr_BomRev = "";
            mstr_Description = "";
            mstr_ImageFile = "";
            mstr_Report_Checksum = "";

            lbl_BomRev.Visible = false;
            txt_BomRev.Visible = false;
            txt_BomRev.Text = "";


            lbl_Description.Visible = false;
            txt_Description.Visible = false;
            txt_Description.Text = "";



            mstr_QADLine = "";
            lbl_QAD_Line.Visible = false;
            Tex_QAD_Line.Visible = false;
            Tex_QAD_Line.Text = "";

       
            lbl_FlashImg.Visible = false;
            txt_FlashImg.Visible = false;
            txt_FlashImg.Text = "";

            lbl_Model.Visible = false;
            txt_Model.Visible = false;
            txt_Model.Text = "";

            lbl_ItemNumber.Visible = true;
            txt_Item.Visible = true;
            txt_Item.Text = "";
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (mstr_ItemNum != txt_Item.Text)
            {
                if (ParseItemNum() == false) return;
            }
            if (mstr_BomRev != txt_BomRev.Text)
            {
                if (ParseBomRev() == false) return;
            }

            if (mstr_Description != txt_Description.Text)
            {
                if (ParseDescription() == false) return;
            }

            if (mstr_QADLine != Tex_QAD_Line.Text)
            {
                if (ParseQADLine() == false) return;
            }

            if (mstr_ImageFile != txt_FlashImg.Text)
            {
                if (ParseImage() == false) return;
            }

            if (mstr_MODEL != txt_Model.Text)
            {
                if (ParseModel() == false) return;
            }
            txt_Item.Focus();
            this.Hide();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            ResetControls();
            txt_Item.Focus();
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            ResetControls();
            mbln_Abort = true;
            txt_Item.Focus();
            this.Hide();
        }
    }
}
