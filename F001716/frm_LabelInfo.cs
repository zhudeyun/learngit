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
    public partial class frm_LabelInfo : Form
    {
        private string mstr_PC_Label_Dir;
        private bool mbln_Abort;
        private string mstr_KitLabelPartNumber;
        private string mstr_MACLabelPartNumber;
        private NetworkMapping_t NetworkMap;
        public NetworkMapping_t NetworkMap_t
        {
            get { return NetworkMap; }
            set { NetworkMap = value; }
        }
        public bool Abort
        {
            get
            {
                return mbln_Abort;
            }
            set
            {
                mbln_Abort = value;
            }
        }

        public string MACLabelNum
        {
            get
            {
                return mstr_MACLabelPartNumber;
            }
            set
            {
                mstr_MACLabelPartNumber = value;
            }
        }
        public string KitLabelNum
        {
            get
            {
                return mstr_KitLabelPartNumber;
            }
            set
            {
                mstr_KitLabelPartNumber = value;
            }
        }
        public frm_LabelInfo()
        {
            InitializeComponent();
        }

        private void frm_LabelInfo_Load(object sender, EventArgs e)
        {
            ResetControls();
        }
        public void ResetControls()
        {
            mbln_Abort = false;
            btn_OK.Enabled = false;
            mstr_PC_Label_Dir = System.IO.Directory.GetCurrentDirectory() + "\\Label";//"C:\F0479\exe";
            mstr_KitLabelPartNumber = "";
            mstr_MACLabelPartNumber = "";
            txt_KitLabel.Text = "";
            txt_KitLabel.Focus();
            txt_KitLabel.Text = "";
            Productlabel1.Visible = false;
            
        }

        private void txt_ScanEntrys_KeyDown(object sender, KeyEventArgs e)
        {

            TextBox tb = new TextBox();
            tb = (TextBox)sender;
            switch (tb.Name)
            {
               
                case "txt_KitLabel":
                    if (e.KeyCode == Keys.F1)
                    {
                        txt_KitLabel.Text = "50137716-001";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseBagLabel() == true && btn_OK.Enabled == false)
                        {
                            Productlabel1.Visible = true;
                            txt_MACLabel.Visible = true;
                            txt_MACLabel.Focus();
                        }
                    }
                    break;
                 case "txt_MACLabel":
                    if (e.KeyCode == Keys.F1)
                    {
                        txt_MACLabel.Text = "50141994-001";
                    }
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ParseMACLabel() == true && btn_OK.Enabled == false)
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
      
        private bool ParseBagLabel()
        {
            bool Check = false;
            Check = Get_Label_POF_File(txt_KitLabel.Text);
            if (Check == true)
            {
                txt_KitLabel.Text=txt_KitLabel.Text.Replace("\r\n", "");
                mstr_KitLabelPartNumber = mstr_PC_Label_Dir + "\\" + txt_KitLabel.Text + ".POF";

            }
            return Check;
        }
        private bool ParseMACLabel()
        {
            bool Check = false;
            Check = Get_Label_POF_File(txt_MACLabel.Text);
            if (Check == true)
            {
                txt_MACLabel.Text = txt_MACLabel.Text.Replace("\r\n", "");
                mstr_MACLabelPartNumber = mstr_PC_Label_Dir + "\\" + txt_MACLabel.Text + ".POF";

            }
            return Check;
        }
        private bool Get_Label_POF_File(string LabelNum)
        {
            bool Network_Online;
            string NetworkLabelDir;
            string NetworkFile;
            string PcFile;
            //Dim res As MsgBoxResult
            string mstr_NetworkRootDir = "I:\\Barone\\";
            string mstr_PC_Label_Dir = System.IO.Directory.GetCurrentDirectory() + "\\Label";
            //'A catch all err routine
            try
            {
                NetworkLabelDir = mstr_NetworkRootDir + "POF";
                NetworkFile = mstr_NetworkRootDir + "POF" + "\\" + LabelNum + ".POF";
                PcFile = mstr_PC_Label_Dir + "\\" + LabelNum + ".POF";

                //'Check to see if Network is up
                Network_Online = System.IO.Directory.Exists(NetworkLabelDir);

                if (Network_Online)
                {
                    //'See if the file exists in the POF directory
                    if (System.IO.File.Exists(NetworkFile))
                    {
                        //'If So copy it to the HardDrive
                        try
                        {
                            //'See if Local Label directory is not present then make it 
                            if (System.IO.Directory.Exists(mstr_PC_Label_Dir) == false)
                            {
                                System.IO.Directory.CreateDirectory(mstr_PC_Label_Dir);
                            }
                            System.IO.File.Copy(NetworkFile, PcFile, true);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show("Error in Copying File " + NetworkFile + " Fom Network ," + ex.Message, "F001305", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else
                    {
                        //'File doesn't exist in the expected directory. Search the rest of the Label Root directory?
                        DialogResult result = System.Windows.Forms.MessageBox.Show("Unable to find file " + NetworkFile + "Do you want to use the Local Copy of the label file?", "File Not Found Error", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error);
                        if (result == DialogResult.No)
                            return false;
                        if (System.IO.File.Exists(PcFile))
                        {
                            return true;
                        }
                        else
                        {
                            //'Network down and File isn't on the Hard Drive
                            //MsgBox("The Network is down." & vbCrLf & "A local copy of the label file was not found." & vbCrLf & "Contact your leader.", vbCritical, "Label File Error")
                            System.Windows.Forms.MessageBox.Show("The Network is down." + "A local copy of the label file was not found." + "Contact your leader.", "F001305", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            return false;
                        }
                        

                    }

                }

                // 'If Network is not online then see if its on the Hard Drive
                if (Network_Online == false)
                {
                    // 'See if file is on Hard Drive
                    if (System.IO.File.Exists(PcFile))
                    {
                        //'See if operator wants to use it
                        //res = MsgBox("The Network appears to be down." & vbCrLf & "Do you want to use the Local Copy of the label file?", vbYesNo, "Network Access Error")
                        DialogResult result = System.Windows.Forms.MessageBox.Show("The Network appears to be down." + "Do you want to use the Local Copy of the label file?", "F001305", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error);
                        if (result == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        //'Network down and File isn't on the Hard Drive
                        //MsgBox("The Network is down." & vbCrLf & "A local copy of the label file was not found." & vbCrLf & "Contact your leader.", vbCritical, "Label File Error")
                        System.Windows.Forms.MessageBox.Show("The Network is down." + "A local copy of the label file was not found." + "Contact your leader.", "F001305", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //MsgBox("An Error occured during Label file Retrieval." & vbCrLf & ex.Message, vbCritical, "Label File Error")
                System.Windows.Forms.MessageBox.Show("An Error occured during Label file Retrieval." + ex.Message, "F001305", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                return false;
            }


        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            
            //if (ParseBagLabel() == false)
            //{
            //    txt_KitLabel.Focus();
            //    return;
            //}
        
            this.Hide();
        }
        private void btn_Abort_Click(object sender, EventArgs e)
        {
            ResetControls();
            mbln_Abort = true;
            this.Hide();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            ResetControls();
        }

        private void txt_KitLabel_TextChanged(object sender, EventArgs e)
        {

            if (txt_KitLabel.Text.Contains("\r") )
            {
                string temp=txt_KitLabel.Text;
             }
        }

        private void txt_MACLabel_TextChanged(object sender, EventArgs e)
        {

            if (txt_MACLabel.Text.Contains("\r"))
            {
                string temp = txt_MACLabel.Text;
            }
        }
            
        }
    
}
