using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Management;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace F001716
{
    public partial class frmMain : Form
    {

        // Last Failure # for this document is 113
        #region variable
        private enum TestId
        {
            TestAll,
            TestPowerOn232,
            TestConnectRS232,
            TestUnitCurrent,
            TestReadMACAddresses,
            TestSetSerialNumber,
            TestReadSerialNumber,
            TestFirmwareVerify,
            TestRtsCts,
            TestConnectUSB,
            TestCradle,
            TestConnectBluetooth,
            TestHapConnectBluetooth,
            TestBattery,
            TestCharger,
            TestPager,
            TestIndicators,
            TestBeeper,
            TestSetStatus,
            TestDefault,
            TestCloseComms,
            TestPowerOff,
            TestNone,
        }
        public struct HIDDeviceDetect
        {
            public bool USBHIDDetect;
            public bool RS232Detect;
            public string RecievedData;
        }
        private serialSettings_t m_scannerSettings;
        private int mint_ImageShipComplete;
        private HighFrequencyNoise m_highFreqNoise;
        private frm_SoftwareInfo frm_SW;

        private t_McConfigData mtyp_McConfig;
        //private t_TestStatus TestData;
        private ComPortSettings_t mtyp_CommUUT;
        private serialSettings_t mySettings;
        private ComPortSettings_t m_typ_CommSLM;
        private ComPortSettings_t mtyp_CommAR66;
        

        private ComPortSettings_t m_typ_CommPackageLabelPrinter;
        private ComPortSettings_t m_typ_CommMacLabelPrinter;
        private TestSaveData_t TestData;
        private HIDDeviceDetect mtype_DeviceDetect;
        private frmDialog frm_Dialog;
        private frmInputBoxDialog frm_ScanMC;
        //private mfrm_BOMInfo  frm_BOMInfo
        private frm_LabelInfo frm_LabelInfo;
        private clsSLM m_obj_SLM;
        //private clsSaxComPortInterface mobj_Saxcomm;
        //private clsScanner m_scanner;
        private clsIncludes_NI6251.PortLine_t m_PortLine;
        private clsDaqmx_NI6251 m_cls_DAQ;
        private clsFixture m_fixctrl;
        private clsDiagnostics m_Diagnostics;
        private clsAgilentE3648A mobj_AgilentPS;
        private clsSaveData mobj_SaveData;
        private clsSaveDataMDCS mobj_SaveDataMDCS;

        private Bitmap b = null;
        private IntPtr m_ip = IntPtr.Zero;
        private int VIDEODEVICE = 0; // zero based index of video capture device to use
        private int VIDEOWIDTH = 640;//640;//1280; // Depends on video device caps 
        private int VIDEOHEIGHT = 480;//480;//720; // Depends on video device caps 
        private string ImagePath = "";
        private string ImagePath_temp = "";
        private short VIDEOBITSPERPIXEL = 24;
        private Honeywell.Testing.ImageProcess.imageProcess ProcessImage = new Honeywell.Testing.ImageProcess.imageProcess();
        //private Honeywell.Testing.AR66.cls_AR66Motor MotorAR66 = new Honeywell.Testing.AR66.cls_AR66Motor();
        private Honeywell.Testing.Camera.CameraSnapShot cam;
        private Image_t ImageParameter;

        private bool m_b_AR66_position = false;
        private bool m_b_AR66_position1 = false;
        public bool AR66_AlarmX = false;
        public bool AR66_AlarmY = false;
        public string str_AR66_AlarmX = "";
        public string str_AR66_AlarmY = "";
        private string FW_Version = "CZ000070HAA";
        #region Label
        private Honeywell.LabelPrintDLL.cls_PrintLabels mobj_Label1;
        public static Honeywell.LabelPrintDLL.TestData_t mudt_TestData = new Honeywell.LabelPrintDLL.TestData_t();
        public static Honeywell.LabelPrintDLL.ComPortSettings_t SerialPortSetting = new Honeywell.LabelPrintDLL.ComPortSettings_t();
        public static string sFirstSerialNumber = "";
        public static string sLastSerialNumber = "";
        public static string sPOFPath = "";

        public string[] MarkerItem = new string[10];
        public string[] MarkerValue = new string[10];
        #endregion

        #region command
        public const string FILE_CONSOLE_LOG = "./data/F001716log.txt";
        public const string BEGIN_TIME = "2014-01-01 00:00:00";
        public const string END_TIME = "2016-04-06 00:00:00";
        public const string ACK = "\x06";
        public const string NUL = "\x00";
        public const string CR = "\x0d";
        public const string LF = "\x0a";
        public const string CRLF = "\x0d\x0a";
        public const string SYN = "\x16";
        public const string DC1 = "\x11";
        public const string SPACE = "\x20";
        private const string CMD_ConnectRS232 = "MATCMD?";
        private const string CMD_FactionMode1 = "FACTON1";
        private const string CMD_ReadBenchNumber = "SERUUI?";
        private const string CMD_ReadSN = "SERNUM?";
        private string CMD_Enter_WriteCoinfigMode_pcfqry = "PCFQRY1!";
        private const string CMD_Enter_WriteCoinfigMode_pcfupd = "PCFUPD1!";
        private const string CMD_Exit_WriteCoinfigMode_pcfqry = "PCFQRY0!";
        private const string CMD_Exit_WriteCoinfigMode_pcfupd = "PCFUPD0!";
        private const string CMD_Read_WriteCoinfigMode_pcfqry = "PCFQRY?";
        private const string CMD_Read_WriteCoinfigMode_pcfupd = "PCFUPD?";
        private const string CMD_WriteSN = "SERNUM";
        private const string CMD_FWVersion_MOC = "REV_WA?";
        private const string CMD_FWVersion_SUF = "FIMPNM?";
        private const string CMD_Default = "Defalt";
        private const string CMD_ReedSwitch = "TSTSSW?";
        private const string CMD_ADCPort = "ACADC";
        private const string CMD_GoodLEDON = "GRLED1";
        private const string CMD_GoodLEDOFF = "GRLED0";
        private const string CMD_ERRLEDON = "ERLED1";
        private const string CMD_ERRLEDOFF = "ERLED0";
        private const string CMD_Beeper = "TSTBEP";
        private const string CMD_RTSCTS_Enable = "232CTS1!";
        private const string CMD_RTSCTS_Disable = "232CTS0!";
        private const string CMD_LEDON = "GRLED1";//Good Read LED ON[ CR][ LF]
        private const string CMD_LEDOFF = "GRLED0";//Good Read LED OFF[ CR][ LF]
        private const string CMD_EnterAPLMode_PWRLDC = "PWRLDC100";
        private const string CMD_EnterAPLMode_SCNLED = "SCNLED1";
        private const string CMD_EnterAPLMode_TRGSTO = "TRGSTO0";
        private const string CMD_EnterAPLMode_SDRTIM = "SDRTIM0";
        private const string CMD_EnterAPLMode_SCNDLY = "SCNDLY0";
        private const string CMD_EnterAPLMode_SCNAIM = "SCNAIM0";
        private const string CMD_EnterAPLMode_EXPMOD = "EXPMOD0";
        private const string CMD_EnterAPLMode_TRGMOD = "TRGMOD0";
        private const string CMD_EnterAPLMode_EXPFEX_CM5680 = "EXPFEX15";
        private const string CMD_EnterAPLMode_EXPFEX_CM2180 = "EXPFEX10";
        private const string CMD_EnterAPLMode_EXPFEX_CM3680 = "EXPFEX6000";
        private const string CMD_EnterAPLMode_EXPFGX = "EXPFGX1";
        private const string CMD_APL_CM5680 = "IMGSNP2P36E1G0T1L0A";
        private const string CMD_APL_CM3680 = "IMGSNP2P6000E1G0T1L0A";
        private const string CMD_APL_CM2180 = "IMGSNP2P150E1G0T1L0A";
        private const string CMD_USBETO300000 = "USBETO300000";
        #endregion
        private string mstr_swrev;
        private string mstr_mcb;
        private string mstr_pcbsub;
        private string mstr_workorder;
        private bool mbln_userStop;
        private bool mbln_enablePower;
        private bool mbln_enableAir;
        private bool m_b_EnablePressButton;
        private bool m_b_Waiver;
        private bool mbln_runInitialized;
        private bool mbln_warn;
        private bool iniLabel = false;
        //private int mint_sizeRam = 0;
        //private int mint_sizeFlash = 0;

        private string mstr_mdcsUrl;
        private bool mbln_mdcsEnabled;
        private string mstr_SolutionDir;

        //private string m_daqDevice = "";

        //resource
        private ResourceManager myResource;
        private ResourceManager m_Resource_Failcode;
        public static ResourceManager m_Resource_FailMessage;
        private string m_str_Message;
        private string mstr_PressStartToBeginTest;
        private string mstr_Beep;
        private string mstr_Testing;
        private string mstr_Setup;
        private string mstr_FixtureError;
        private string mstr_InsertAndBeginTest;
        private string mstr_InsertAndBeginDiagnostics;
        private string mstr_PowerSupply;
        private string mstr_Yes;
        private string mstr_Cancel;
        private string mstr_Ok;
        private string mstr_Retry;

        #region barcode
        public const string BARCODE1 = "Skaneateles Falls, NY 13153";//"5MIL80BW";//5mil code39 PT#100001542
        public const string BARCODE4 = "0.150 MM";//"5MIL80BW";//5mil code39 PT#100001542
        public const string BARCODE2 = "098765432105";//13mil UPC Far end Pt#100001513 
        public const string BARCODE3 = "100001520, 0123456789, abcdefghijklmnopqrstuvwxyz";//10mil DM Far end Pt#100001520
        public const string BARCODE5 = "A";//"10mil Code39

        #endregion

        #endregion

        #region "Menu"
        public frmMain()
        {
            InitializeComponent();
        }

        ~frmMain()
        {

        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About();
        }

        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string docpath;//, viewerpath;

            //viewerpath = "C:\\Program Files\\Windows NT\\Accessories\\wordpad.exe ";
            docpath = Application.StartupPath + "\\Data";

            try
            {
                //Shell(("" + viewerpath + docpath + ""), AppWinStyle.NormalFocus, false, 1000);
                OpenFileDialog fd;
                fd = new OpenFileDialog();
                fd.InitialDirectory = docpath;
                fd.FileName = "F000986log.txt";
                DialogResult dres = fd.ShowDialog();
                if (dres == DialogResult.OK)
                    System.Diagnostics.Process.Start(fd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file: " + docpath, "Log File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void openTestReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string docpath;//, viewerpath;

            // viewerpath = "C:\\Program Files\\Windows NT\\Accessories\\wordpad.exe ";
            docpath = Application.StartupPath + "\\Data";

            try
            {
                //Shell(("" + viewerpath + docpath + ""), AppWinStyle.NormalFocus, false, 1000);
                OpenFileDialog fd;
                fd = new OpenFileDialog();
                fd.InitialDirectory = docpath;
                fd.FileName = "F000986report.txt";
                DialogResult dres = fd.ShowDialog();
                if (dres == DialogResult.OK)
                    System.Diagnostics.Process.Start(fd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file: " + docpath, "Test Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void powerOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (powerOnToolStripMenuItem.Checked == false)
            {
                mbln_enablePower = true;
                powerOnToolStripMenuItem.Checked = true;
            }
            else
            {
                mbln_enablePower = false;
                powerOnToolStripMenuItem.Checked = false;
            }

            if (mbln_enablePower == true)
            {
                TestPowerOn();
                m_Diagnostics.dPortState = false;
            }
            else
            {
                TestPowerOff();
                m_Diagnostics.dPortState = false;
            }
        }

        private void enablePressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RS232.Checked == false)
            {
                mbln_enableAir = true;
                RS232.Checked = true;
                m_fixctrl.CableSel232();
            }
            else
            {
                mbln_enableAir = false;
                RS232.Checked = false;
            }

        }

        private void uSBToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (uSBToolStripMenuItem.Checked == false)
            {
                uSBToolStripMenuItem.Checked = true;
                m_fixctrl.CableSelUSB();
            }
            else
            {
                uSBToolStripMenuItem.Checked = false;
            }
        }

        private void diagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.diagsToolStripMenuItem.Checked)
            {
                if (!Login())
                    return;
                this.btn_Start.Enabled = false;
                this.diagsToolStripMenuItem.Checked = true;
                this.debugToolStripMenuItem.Enabled = true;
                this.testDiagsToolStripMenuItem.Enabled = true;
                ShowGraphics(false);
                this.lbl_diagMode.Visible = true;
                Instruction(mstr_InsertAndBeginDiagnostics);
            }
            else
            {
                this.testDiagsToolStripMenuItem.Enabled = false;
                this.debugToolStripMenuItem.Enabled = false;
                this.diagsToolStripMenuItem.Checked = false;
                this.powerOnToolStripMenuItem.Checked = false;
                m_fixctrl.InitFixture();
                ShowGraphics(true);
                this.lbl_diagMode.Visible = false;
                this.btn_Start.Enabled = true;
                Instruction(mstr_InsertAndBeginTest);
            }
        }

        private void toolShortOpenTestStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region "Events"

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseHW();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lbl_Rev.Text = Program.gstr_Software_Number + " Rev " + Program.gstr_Rev;

            // mbln_hideSplash = false;
            //mbln_diagMode = false;
            rbn_MDCSProduction.Checked = true;
            mbln_enablePower = false;
            mbln_enableAir = false;
            m_b_EnablePressButton = false;
            mbln_runInitialized = false;
            mbln_warn = false;
            m_b_Waiver = false;
            InitObjLoader();
            if (ReadTranslationSettings() == true)
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");

            SetResource();
            ReadINISettings();

            SetUIChanges();
            if (InitHW() == false)
                return;
            gbx_TestResults.Visible = false;
            gbx_TestResults.SendToBack();
            InitButtons(true);
            Instruction(mstr_InsertAndBeginTest);
            SNBox.Focus();
            //InitNewRun();

        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            mbln_userStop = true;
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < Program.FixtureLoopCount; i++)
            {
                InitButtons(false);
                ShowGraphics(false);
                RunTests(TestId.TestAll);
                ShowGraphics(true);
                InitButtons(true);
            }
            
        }

        private void cbx_ViewDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_ViewDebug.Checked == true)
            {
                gbx_Image.Visible = false;
                gbx_Image.SendToBack();
                gbx_TestResults.Visible = true;
                gbx_TestResults.BringToFront();
            }
            else
            {
                gbx_TestResults.Visible = false;
                gbx_TestResults.SendToBack();
                gbx_Image.Visible = true;
                gbx_Image.BringToFront();
            }
        }

        #region Waiver timer
        private void Waiver_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime now = new DateTime(Convert.ToDateTime(END_TIME).Ticks - System.DateTime.Now.Ticks);
                surplus_time.Text = (now.Month - 1).ToString() + "月" + (now.Day - 1).ToString() + "天" + now.Hour.ToString() + "小时" + now.Minute.ToString() + "分钟" + now.Second.ToString() + "妙";
            }
            catch (Exception err)
            {
                Waiver_timer.Enabled = false;
                m_b_Waiver = false;
                string error = err.Message;
            }
            System.Windows.Forms.Application.DoEvents();
        }
        #endregion

        #endregion

        #region Properties
        public string LabelSwRev
        {
            get
            {
                return mstr_swrev;
            }
            set
            {
                mstr_swrev = value;
                lbl_Rev.Text = mstr_swrev;
                lbl_Rev.Refresh();
            }
        }

        public string LabelMCB
        {
            get
            {
                return mstr_mcb;
            }
            set
            {
                mstr_mcb = value;
                lbl_configNum.Text = mstr_mcb;
                lbl_configNum.Refresh();
            }
        }

        public string LabelPcbSA
        {
            get
            {
                return mstr_pcbsub;
            }
            set
            {
                mstr_pcbsub = value;
                lbl_special.Text = mstr_pcbsub;
                lbl_special.Refresh();
            }
        }

        public string LabelWorkOrder
        {
            get
            {
                return mstr_workorder;
            }
            set
            {
                mstr_workorder = value;
                lbl_order.Text = mstr_workorder;
                lbl_order.Refresh();
            }
        }


        //public bool DiagMode
        //{
        //    get
        //    {
        //        return mbln_diagMode;
        //    }
        //    set
        //    {
        //        mbln_diagMode = value;
        //    }
        //}

        public bool UserStop
        {
            get
            {
                return mbln_userStop;
            }
            set
            {
                mbln_userStop = value;
            }
        }

        #endregion

        #region Function

        #region general funtion

        public string GetFileVersion(string DllName)
        {

            // Get the file version for the notepad. 
            string FileVersion = "";
            try
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(DllName);
                FileVersion = myFileVersionInfo.FileVersion;
                DisplayMessage("Version number: " + myFileVersionInfo.FileVersion);
            }
            catch (Exception err)
            {
                TestData.Results.TestFailMessage = "Read version fail:" + err.Message;
            }

            return FileVersion;
        }

        public void DisplayTestResults(string txtStr, bool pf)
        {
            // logSize ; int

            //logSize = rtb_TestResults.TextLength()
            rtb_TestResults.AppendText(txtStr + Convert.ToChar(13) + Convert.ToChar(10));
            rtb_TestResults.ScrollToCaret();
            rtb_TestResults.Refresh();
            Application.DoEvents();
        }

        public void Instruction(string strmes)
        {
            bool warn = false;

            mbln_warn = warn;
            if (warn == true)
            {
                lbl_Instruction.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                lbl_Instruction.BackColor = System.Drawing.Color.Beige;
            }
            lbl_Instruction.Text = strmes;
        }

        public void Instruction(string strmes, bool warn)
        {

            mbln_warn = warn;
            if (warn == true)
            {
                lbl_Instruction.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                lbl_Instruction.BackColor = System.Drawing.Color.Beige;
            }
            lbl_Instruction.Text = strmes;
        }

        public bool SaveTestReport()
        {
            string str_Path = "";
            string str_FileName;
            int logSize;

            str_Path = Application.StartupPath + "\\Data";
            str_FileName = "F000986report.txt";

            logSize = rtb_TestResults.TextLength;
            // see if directory is present
            if (System.IO.Directory.Exists(str_Path) == false)
            {
                System.IO.Directory.CreateDirectory(str_Path);
            }
            // save over old report
            if (System.IO.File.Exists(str_Path + "\\" + str_FileName))
            {
                try
                {
                    System.IO.File.Delete(str_Path + "\\" + str_FileName);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            try
            {
                rtb_TestResults.SaveFile(str_Path + "\\" + str_FileName, RichTextBoxStreamType.PlainText);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void About()
        {
            AboutBox m_frmAbout = new AboutBox();
            m_frmAbout.ShowDialog();
        }

        private void InitButtons(bool initFlag)
        {
            // Waiver
            DateTime dt_BeginTime = Convert.ToDateTime(BEGIN_TIME);
            DateTime dt_EndTime = Convert.ToDateTime(END_TIME);

            if ((dt_BeginTime < DateTime.Now) && (DateTime.Now < dt_EndTime))
            {
                m_b_Waiver = true;
            }
            else
            {
                m_b_Waiver = false;
            }

            if (m_b_Waiver == true)
            {
                Waiver.Visible = true;
                Waiver_Time.Visible = true;
                surplus.Visible = true;
                surplus_time.Visible = true;
                Waiver_Time.Text = BEGIN_TIME + " ~ " + END_TIME;
                Waiver_timer.Enabled = true;
                
            }
            else
            {
                Waiver.Visible = false;
                Waiver_Time.Visible = false;
                Waiver_timer.Enabled = false;
                surplus.Visible = false;
                surplus_time.Visible = false;
            }
            mbln_userStop = false;
            if (initFlag == true)
            {
                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
                btn_Exit.Enabled = true;
                SNBox.Text = "";
                btn_Start.Focus();
                SNBox.Focus();
                if (!mbln_warn)
                    Instruction(mstr_InsertAndBeginTest);
            }
            else
            {
                btn_Start.Enabled = false;
                btn_Stop.Enabled = true;
                btn_Exit.Enabled = false;
                rtb_TestResults.Clear();
                Instruction(mstr_Testing);
            }
        }

        private void ShowGraphics(bool initFlag)
        {
            if (initFlag == true)
            {
                //PictureBox2.Load(GetSolutionPath() + "5683.jpg")
                gbx_TestResults.Visible = false;
                gbx_TestResults.SendToBack();
                gbx_Image.Visible = true;
                gbx_Image.BringToFront();
                btn_Start.Focus();
            }
            else
            {
                gbx_Image.Visible = false;
                gbx_Image.SendToBack();
                gbx_TestResults.Visible = true;
                gbx_TestResults.BringToFront();
            }
            Application.DoEvents();
        }

        private bool Login()
        {
            frmLogin frm_Login = new frmLogin();
            frm_Login.lgPassword = "TECH";
            if (frm_Login.ShowDialog() == DialogResult.OK) { return true; }
            return false;
        }

        private void SetResource()
        {
            myResource = new ResourceManager("F001716.Resource", System.Reflection.Assembly.GetExecutingAssembly());
            m_Resource_Failcode = new ResourceManager("F001716.Failcode", System.Reflection.Assembly.GetExecutingAssembly());
            m_Resource_FailMessage = new ResourceManager("F001716.FailMessage", System.Reflection.Assembly.GetExecutingAssembly());
        }

        private void SetUIChanges()
        {
            try
            {
                this.grp_MDCS.Text = myResource.GetString("strMDCS");
                this.rbn_MDCSProduction.Text = myResource.GetString("strProductionMode");
                this.rbn_MDCSOther.Text = myResource.GetString("strTestMode");

                //this.grp_View.Text = myResource.GetString("strView");
                this.cbx_ViewDebug.Text = myResource.GetString("strViewDebug");

                this.btn_Start.Text = myResource.GetString("strStart");
                this.btn_Stop.Text = myResource.GetString("strStop");
                this.btn_Exit.Text = myResource.GetString("strExit");
                this.lbl_diagMode.Text = myResource.GetString("strDiagMode");

                mstr_PressStartToBeginTest = myResource.GetString("strPressStartToBeginTest");
                mstr_Beep = myResource.GetString("strBeep");
                mstr_Testing = myResource.GetString("strTesting");
                mstr_Setup = myResource.GetString("strSetup");
                mstr_FixtureError = myResource.GetString("strFixtureError");
                mstr_InsertAndBeginTest = myResource.GetString("strInsertAndBeginTest");
                mstr_InsertAndBeginDiagnostics = myResource.GetString("strInsertAndBeginDiagnostics");
                mstr_PowerSupply = myResource.GetString("strPowerSupply");
                mstr_Yes = myResource.GetString("strYes");
                mstr_Cancel = myResource.GetString("strCancel");
                mstr_Ok = myResource.GetString("strOk");
                mstr_Retry = myResource.GetString("strRetry");
                this.frm_Dialog.InitButtons(mstr_Cancel, mstr_Ok, mstr_Retry);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool InitObjLoader()
        {
            m_highFreqNoise = new HighFrequencyNoise();
            //  create new object instances
            mtyp_McConfig = new t_McConfigData();
            mtyp_CommUUT = new ComPortSettings_t();
            frm_SW = new frm_SoftwareInfo();
            m_typ_CommSLM = new ComPortSettings_t();
            m_typ_CommPackageLabelPrinter = new ComPortSettings_t();
            m_typ_CommMacLabelPrinter = new ComPortSettings_t();
            TestData = new TestSaveData_t();
            frm_LabelInfo = new frm_LabelInfo();
            mobj_SaveData = new clsSaveData();
            mobj_SaveDataMDCS = new clsSaveDataMDCS();
            m_Diagnostics = new clsDiagnostics();
            frm_ScanMC = new frmInputBoxDialog();
            frm_Dialog = new frmDialog();
            m_obj_SLM = new clsSLM();
            m_cls_DAQ = new clsDaqmx_NI6251();
            m_fixctrl = new clsFixture();
            m_PortLine = m_cls_DAQ.m_cls_NICard.PortLine;
            m_fixctrl.dqSetDaq = m_cls_DAQ;
            return true;
        }

        private bool ConstructTestList(TestId id, ref TestId[] tstList)
        {
            int i = 0;

            //  use i to easily arrange in any order
            switch (id)
            {
                case TestId.TestAll:
                    tstList[i++] = TestId.TestPowerOn232;
                    tstList[i++] = TestId.TestUnitCurrent;
                    tstList[i++] = TestId.TestFirmwareVerify;                  
                    tstList[i++] = TestId.TestReadMACAddresses;
                    tstList[i++] = TestId.TestCradle;
                    tstList[i++] = TestId.TestConnectBluetooth;
                    tstList[i++] = TestId.TestRtsCts;
                    tstList[i++] = TestId.TestConnectUSB;
                    tstList[i++] = TestId.TestBeeper;
                    tstList[i++] = TestId.TestPager;
                    tstList[i++] = TestId.TestIndicators;
                    tstList[i++] = TestId.TestSetSerialNumber;
                    tstList[i++] = TestId.TestSetStatus;
                    tstList[i++] = TestId.TestDefault;
                    tstList[i++] = TestId.TestNone;
                    break;

                default:
                    break;
            }

            return true;
        }

        private bool GetFailCode(string strin, ref string strout)
        {
            int idx;
            string cmdres;

            //  pull out error number, zero is reserved for MDCS passing, so use 1 as a default fail code
            cmdres = "";
            strout = "";

            idx = strin.IndexOf("#", 0);
            if (idx == -1)
            {
                strout = "1";
                return false;
            }
            cmdres = strin.Substring(idx + 1);

            idx = cmdres.IndexOf(":", 0);
            if (idx == -1)
            {
                strout = "1";
                return false;
            }
            cmdres = cmdres.Substring(0, idx);

            strout = cmdres.Trim().Replace(" ", "");

            return true;
        }
        private bool Ini_AR66MotorToHome()
        {
            //********AR66 Motor GoHome*********//
            //string Alarm = "";
            bool check = true;
            //long tmr = clsUtils.StartTimeInTicks();
            //check = MotorAR66.Serial_INIT();
            //DisplayMessage("AR66_X Go home");
            //m_b_AR66_position = false;
            //TestData.AR66Status.CurrentPosition = 0;
            //check = MotorAR66.AR66_HOME(1);
            //AR66_AlarmX = MotorAR66.AR66_GetAlarm(1, ref Alarm);
            //str_AR66_AlarmX = Alarm;
            //AR66Motor.Enabled = true;
            /////////////
            //DisplayMessage("AR66_Y Go home");
            //m_b_AR66_position1 = false;
            //TestData.AR66Status1.CurrentPosition = 0;
            //check = MotorAR66.AR66_HOME(2);
            //AR66_AlarmY = MotorAR66.AR66_GetAlarm(2, ref Alarm);
            //str_AR66_AlarmY = Alarm;
            //AR66Motor1.Enabled = true;
            return check;
        }
        private bool InitNewRun()
        {
            string reterr;
            string MDCSDLL_Version = "";
            MDCSDLL_Version = GetFileVersion("MDCS.dll");
            if (MDCSDLL_Version != "1.1.1.13") return false;
            mstr_mdcsUrl = "";
            mbln_mdcsEnabled = false;
            Instruction(mstr_Setup);
            frm_Dialog.Hide();
            TestData.TestInfo.TestSoftwareNum = Program.gstr_Software_Number;
            TestData.TestInfo.TestSoftwareRev = Program.gstr_Rev;
            ////for test 
            frm_ScanMC.FormCaption = "MCF Input";
            frm_ScanMC.FormPrompt = "Scan MCF Number";
            //frm_ScanMC.DefaultValue = "MCF-CM36800000000"; //for standard units
            frm_ScanMC.ShowDialog();
            if (frm_ScanMC.DialogResult != DialogResult.OK)
            {
                Instruction("Error scanning MCF Entry!", true);
                return false;
            }

            frm_ScanMC.Hide();
            TestData.TestInfo.TestMCFNum = frm_ScanMC.InputResponse.Trim().ToUpper();
            if (!ParseMCF(TestData.TestInfo.TestMCFNum))
            {
                Instruction("Invalid MCF Entry!", true);
                return false;
            }
            if (Ini_InputSWFiles() == false)
                return false;

            if (cbx_PrintLabelsYes.Checked == true && iniLabel == false)
            {
                frm_LabelInfo.NetworkMap_t = TestData.NetworkMapping;
                if (Ini_Labels() == false)
                    return false;
                else
                {
                    this.lab_KitLabelnumber.Visible = true;
                    this.lab_KitLabelnumber.Text = frm_LabelInfo.txt_KitLabel.Text;
                    //this.Group_Print.Size = new System.Drawing.Size(202, 138);
                }
            }
            //if (ReadSLMPortSettings() == false)
            //{
            //    m_str_Message = m_Resource_FailMessage.GetString("ER9001_EXTRA_READ_INI_SETTING");
            //    Instruction(m_str_Message + TestData.Results.TestFailMessage, true);
            //    return false;
            //}
            //if (InitSLMComm() == false)
            //{
            //    Instruction("UUT Comm Init Error", true);
            //    return false;
            //}

            //for test
            // display MCF test box here
            lbl_configNum.Text = TestData.TestInfo.TestMCFNum;
            lbl_configNum.Visible = true;

            if (ReadINISettings() == false)
            {
                m_str_Message = m_Resource_FailMessage.GetString("ER9001_EXTRA_READ_INI_SETTING");
                Instruction(m_str_Message + TestData.Results.TestFailMessage, true);
                return false;
            }
            //if (InitAR66Comm() == false)
            //{
            //    Instruction("AR66 Motor Comm Init Error", true);
            //    return false;
            //}
            //if (Ini_AR66MotorToHome() == false)
            //{
            //    Instruction("AR66 Motor Init To Home", true);
            //    return false;
            //}

            if (m_fixctrl.InitFixture() == false)
            {
                m_str_Message = m_Resource_FailMessage.GetString("ER9004_EXTRA_Fixture_INIT");
                reterr = m_fixctrl.Error;
                Instruction(m_str_Message + reterr, true);
                return false;
            }
            //try
            //{
            //    cam = new Honeywell.Testing.Camera.CameraSnapShot(0, VIDEOWIDTH, VIDEOHEIGHT, VIDEOBITSPERPIXEL, pictureBox2);
            //}
            //catch (Exception EX)
            //{
            //    TestData.Results.TestFailMessage = "Camera error." + EX.Message;
            //    Instruction(TestData.Results.TestFailMessage, true);
            //    return false;
            //}

            if (InitDecoderComm() == false)
            {
                Instruction("UUT Comm Init Error", true);
                return false;
            }

            if (!ReadMDCSSettings())
            {
                m_str_Message = m_Resource_FailMessage.GetString("ER9001_EXTRA_READ_INI_SETTING");
                Instruction(m_str_Message + TestData.Results.TestFailMessage, true);
                return false;
            }
            if (mbln_mdcsEnabled == false)
            {
                rbn_MDCSProduction.Enabled = false;
                rbn_MDCSOther.Enabled = false;
            }
            else
            {
                rbn_MDCSProduction.Enabled = true;
                rbn_MDCSOther.Enabled = true;
            }
            mstr_SolutionDir = GetSolutionPath();

            //TestData.TestInfo.tsRunInitialized = true;
            //TestData.TestInfo.tsFirstRun = true;

            //Enable debug capabilities
            this.diagsToolStripMenuItem.Enabled = true;
            mbln_runInitialized = true;
            return true;
        }

        //**********************************************
        //Purpose : Clears ALL Test Results Data for start of new test loop
        //Inputs  : None
        //Sets    :  
        //Returns : Boolean pass/fail
        //**********************************************
        private bool Ini_TestLoopVariables()
        {

            //Instruction(mstr_Testing);

            //  reset loop variables
            TestData.Results.TestPassed = true;
            TestData.Results.TestFailNumber = "";
            TestData.Results.TestFailMessage = "";
            TestData.Results.TestFailMessage_Name = "";
            TestData.Results.TestStatus = "";
            TestData.UnitInfo.APL = 0;
            TestData.UnitInfo.ShortOpen_Current = 0;
            TestData.UnitInfo.Current = 0;
            TestData.UnitInfo.UnitCurrent2 = 0;
            TestData.UnitInfo.embedded_rev = "";
            TestData.UnitInfo.str_SN = "";
            TestData.UnitInfo.BT_MacAddress = "";

            TestData.UnitInfo.total_time = 0;

            return true;
        }

        private bool InitDecoderComm()
        {

            
            try
            {
                mySettings.PortNum = mtyp_CommUUT.PortNum;
                mySettings.Baud = mtyp_CommUUT.Baud;
                mySettings.DataBits = mtyp_CommUUT.DataBits;
                switch (mtyp_CommUUT.Parity)
                {
                    case "n":
                    case "N":
                        mySettings.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "e":
                    case "E":
                        mySettings.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "o":
                    case "O":
                        mySettings.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    default:
                        mySettings.Parity = System.IO.Ports.Parity.None;
                        break;
                }

                switch (mtyp_CommUUT.StopBits)
                {
                    case 0:
                        mySettings.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case 1:
                        mySettings.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case 2:
                        mySettings.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        mySettings.StopBits = System.IO.Ports.StopBits.None;
                        break;
                }

                m_Diagnostics.dPortState = false;
                m_Diagnostics.dSettings = mySettings;

            }
            catch (Exception ex)
            {
                try
                {
                    m_Diagnostics.dPortState = false;
                }
                catch (Exception exx)
                {
                    return false;
                }
            }

            return true;
        }

        private bool InitSLMComm()
        {
            serialSettings_t mySettings;

            try
            {
                mySettings.PortNum = m_typ_CommSLM.PortNum;
                mySettings.Baud = m_typ_CommSLM.Baud;
                mySettings.DataBits = m_typ_CommSLM.DataBits;
                switch (m_typ_CommSLM.Parity)
                {
                    case "n":
                    case "N":
                        mySettings.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "e":
                    case "E":
                        mySettings.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "o":
                    case "O":
                        mySettings.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    default:
                        mySettings.Parity = System.IO.Ports.Parity.None;
                        break;
                }

                switch (m_typ_CommSLM.StopBits)
                {
                    case 0:
                        mySettings.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case 1:
                        mySettings.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case 2:
                        mySettings.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        mySettings.StopBits = System.IO.Ports.StopBits.None;
                        break;
                }
                m_obj_SLM.dSettings = mySettings;
                m_obj_SLM.dPortState = false;
            }
            catch (Exception ex)
            {
                try
                {
                    m_obj_SLM.dPortState = false;
                }
                catch (Exception exx)
                {
                    MessageBox.Show("Error: Init SLM comm fail" + "\n" + exx.Message);
                    return false;
                }
                MessageBox.Show("Error: Init SLM comm fail" + "\n" + ex.Message);
            }

            return true;
        }

        //private bool InitAR66Comm()
        //{
        //    try
        //    {
        //        MotorAR66.CommPortNumber = "COM"+mtyp_CommAR66.PortNum.ToString();
        //        MotorAR66.CommBaudRate = mtyp_CommAR66.Baud;
        //        MotorAR66.CommDataBits = mtyp_CommAR66.DataBits;
        //        switch (mtyp_CommAR66.Parity)
        //        {
        //            case "n":
        //            case "N":
        //                MotorAR66.CommParity = System.IO.Ports.Parity.None;
        //                break;
        //            case "e":
        //            case "E":
        //                MotorAR66.CommParity = System.IO.Ports.Parity.Even;
        //                break;
        //            case "o":
        //            case "O":
        //                MotorAR66.CommParity = System.IO.Ports.Parity.Odd;
        //                break;
        //            default:
        //                MotorAR66.CommParity = System.IO.Ports.Parity.None;
        //                break;
        //        }
        //        switch (mtyp_CommAR66.StopBits)
        //        {
        //            case 0:
        //                MotorAR66.CommStopBits = System.IO.Ports.StopBits.None;
        //                break;
        //            case 1:
        //                MotorAR66.CommStopBits = System.IO.Ports.StopBits.One;
        //                break;
        //            case 2:
        //                MotorAR66.CommStopBits = System.IO.Ports.StopBits.Two;
        //                break;
        //            default:
        //                MotorAR66.CommStopBits = System.IO.Ports.StopBits.None;
        //                break;
        //        }
        //        if (MotorAR66.Serial_INIT() == false)
        //        {
        //            Instruction("Serial port init fail for AR66.", true);
        //            return false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string errormessage1 = ex.Message;
        //        return false;
        //    }

        //    return true;
        //}


        private bool ParseMCF(string s_data)
        {
            string MCnum;
            string Family;
            string DirectionType;
            string UsageType;
            string RamSize;
            string Expansion;

            MCnum = s_data.Trim().ToUpper();
            if (MCnum.Length != 16)
            {
                MessageBox.Show("Invalid MCF number", "MCF Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (MCnum.StartsWith("MCF") == false)
            {
                MessageBox.Show("Invalid MCF number", "MCB Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            mtyp_McConfig.mcMCNumber = MCnum;
            Family = MCnum.Substring(4, 3);
            DirectionType = MCnum.Substring(7, 2);
            UsageType = MCnum.Substring(9, 1);
            RamSize = MCnum.Substring(10, 1);
            Expansion = MCnum.Substring(11, 5);

            switch (Family)
            {
                case "CCB": //CM3680
                    break;
                case "COB": //CM5680 SR-B0//CM5680 SR-CW0//CM5680 WA
                    break;
                default:
                    MessageBox.Show("Invalid family", "MCB Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
            }
            mtyp_McConfig.mcFamily = Family;
            TestData.TestInfo.model = Family;

            switch (RamSize)
            {
                case "0": //8MB Flash,32MB RAM
                    break;
                default:
                    MessageBox.Show("Invalid flash/ram", "MCB Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
            }

            switch (UsageType)
            {
                case "0": //General 
                    mtyp_McConfig.mcUsageType = enUsageType.General;
                    break;
                case "1": //Healthcare
                    mtyp_McConfig.mcUsageType = enUsageType.Healthcare;
                    break;
                default:
                    MessageBox.Show("Invalid UsageType", "MCB Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
            }

            switch (DirectionType)
            {
                case "00": //Horizontal
                    mtyp_McConfig.mcDirectionType = enDirectionType.Horizontal;
                    break;
                case "01": //Vertical
                    mtyp_McConfig.mcDirectionType = enDirectionType.Vertical;
                    break;
                default:
                    MessageBox.Show("Invalid DerectionType", "MCB Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
            }

            switch (Expansion)
            {
                case "00000": //None
                    break;
                default:
                    MessageBox.Show("Invalid future", "MCB Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
            }
            mtyp_McConfig.mcExpansion = Expansion;

            return true;
        }

        private bool PassFail()
        {
            if (TestData.Results.TestPassed == false)
            {
                frmFail frmfail = new frmFail();
                frmfail.faMessage = TestData.Results.TestFailMessage_UI;
                frmfail.ShowDialog();
                frmfail.Dispose();
            }
            else
            {
                frmPass frmPass = new frmPass();
                frmPass.SoftwareNumber = "SW:" + TestData.UnitInfo.embedded_rev;
                frmPass.paSerialNumber = "SN:" + TestData.UnitInfo.str_SN;
                frmPass.ShowDialog();
                frmPass.Dispose();

                //if (TestData.TestInfo.tsFirstRun == true)
                //{
                //    TestData.TestInfo.tsFirstRun = false;
                //}
            }
            SNBox.Text = "";
            SNBox.Focus();
            return true;
        }

        //Waits up to 4 seconds for TP76 FOCUS4 to go High
        private bool ReadEmbeddedStatusLine()
        {
            double anaval = 0;
            double WaitTimeSeconds;
            long tmr;
            bool blnret;

            // embedded status TP76 FOCUS_4
            blnret = false;
            WaitTimeSeconds = 4;
            tmr = clsUtils.StartTimeInTicks();
            while (clsUtils.ElapseTimeInSeconds(tmr) < WaitTimeSeconds)
            {
                //anaval = m_cls_DAQ.ReadSingleAnalogChannelNrse(m_PortLine.AnaInLine[12], 1);
                if (anaval < 2)
                //port = m_fixctrl.ReadSingleAnalogChannelRseConvertToDigital(m_PortLine.AnaInLine[12], 30);
                //if (port == 0)
                {
                    blnret = true;
                    break;
                }
                Application.DoEvents();
            }

            return blnret;
        }

        //Focus_3
        private bool SetFixtureReadyStatusLine(bool blnset)
        {
            if (blnset == true)
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][5].LineName, m_PortLine.Port[0][5].High);
            else
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][5].LineName, m_PortLine.Port[0][5].Low);

            return true;
        }

        #region Read ini file

        private bool ReadSLMPortSettings()
        {
            string strPath;
            string strRes;
            ReadIniSettings objIniSettings;
            objIniSettings = new ReadIniSettings();
            strPath = Application.StartupPath + "\\" + Program.gstr_Software_Number + ".ini";
            if (System.IO.File.Exists(strPath) == false)
            {
                TestData.Results.TestFailMessage = " incorrect path " + strPath;
                return false;
            }
            //***********SLM***********

            strRes = objIniSettings.ReadPrivateProfileStringKey("SLM.Comport", "ComPort", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[SLM.Comport] ComPort.";
                return false;
            }
            m_typ_CommSLM.PortNum = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("SLM.Comport", "Baud", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[SLM.Comport] Baud.";
                return false;
            }
            m_typ_CommSLM.Baud = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("SLM.Comport", "Parity", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[SLM.Comport] Parity.";
                return false;
            }
            m_typ_CommSLM.Parity = strRes;

            strRes = objIniSettings.ReadPrivateProfileStringKey("SLM.Comport", "DataBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[SLM.Comport] DataBits.";
                return false;
            }
            m_typ_CommSLM.DataBits = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("SLM.Comport", "StopBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[SLM.Comport] StopBits.";
                return false;
            }
            m_typ_CommSLM.StopBits = Convert.ToInt32(strRes);


            //strRes = objIniSettings.ReadPrivateProfileStringKey("SLM.Comport", "BeeperSpec", strPath);
            //if (strRes == "Error")
            //{
            //    TestData.Results.TestFailMessage = "[SLM.Comport] BeeperSpec.";
            //    return false;
            //}
            //BeeperSpec = Convert.ToDouble(strRes);
            return true;
        }

        private bool ReadINISettings()
        {
            string strPath;
            string strRes;
            ReadIniSettings objIniSettings;

            objIniSettings = new ReadIniSettings();
            strPath = Application.StartupPath + "\\" + Program.gstr_Software_Number + ".ini";
            if (System.IO.File.Exists(strPath) == false)
            {
                TestData.Results.TestFailMessage = " incorrect path " + strPath;
                return false;
            }
            strRes = objIniSettings.ReadPrivateProfileStringKey("FixtureConfig", "TestLoopCount", strPath);
            if (strRes == "Error")
            {
                //MsgBox("Error: With MDCS Path", MsgBoxStyle.Critical, "INI Error");
                return false;
            }
            Program.FixtureLoopCount = Convert.ToInt32(strRes);

            #region Decode1
            strRes = objIniSettings.ReadPrivateProfileStringKey("Decoder1.Comport", "ComPort", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " port setting";
                return false;
            }
            mtyp_CommUUT.PortNum = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("Decoder1.Comport", "Baud", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " baud setting";
                return false;
            }
            mtyp_CommUUT.Baud = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("Decoder1.Comport", "Parity", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " parity setting";
                return false;
            }
            mtyp_CommUUT.Parity = strRes;

            strRes = objIniSettings.ReadPrivateProfileStringKey("Decoder1.Comport", "DataBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " data bits setting";
                return false;
            }
            mtyp_CommUUT.DataBits = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("Decoder1.Comport", "StopBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " stop bits setting";
                return false;
            }
            mtyp_CommUUT.StopBits = Convert.ToInt32(strRes);
            #endregion
            #region image
            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageThreshold", "White", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read white of ImageThreshold .";
                return false;
            }
            ImageParameter.Threshold_White = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageThreshold", "Red", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read Red of ImageThreshold .";
                return false;
            }
            ImageParameter.Threshold_Red = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageThreshold", "Green", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read Green of ImageThreshold .";
                return false;
            }
            ImageParameter.Threshold_Green = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageThreshold", "Blue", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read Blue of ImageThreshold .";
                return false;
            }
            ImageParameter.Threshold_Blue = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImagePixel", "CatchPoint_White", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read CatchPoint_White of ImagePixel .";
                return false;
            }
            ImageParameter.White_Count = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImagePixel", "CatchPoint_Blue", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read CatchPoint_Blue of ImagePixel .";
                return false;
            }
            ImageParameter.Blue_Count = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImagePixel", "CatchPoint_Black", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read CatchPoint_Black of ImagePixel .";
                return false;
            }
            ImageParameter.Black_Count = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImagePixel", "CatchPoint_Green", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read CatchPoint_Green of ImagePixel .";
                return false;
            }
            ImageParameter.Green_Count = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImagePixel", "CatchPoint_Red", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read CatchPoint_Red of ImagePixel .";
                return false;
            }
            ImageParameter.Red_Count = Convert.ToInt32(strRes);


            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageROI", "StartX", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read StartX of ImageROI .";
                return false;
            }
            ImageParameter.StartX = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageROI", "StartY", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read StartY of ImageROI .";
                return false;
            }
            ImageParameter.StartY = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageROI", "EndX", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read EndX of ImageROI .";
                return false;
            }
            ImageParameter.EndX = Convert.ToInt32(strRes);


            strRes = objIniSettings.ReadPrivateProfileStringKey("ImageROI", "EndY", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "fail to read EndY of ImageROI .";
                return false;
            }
            ImageParameter.EndY = Convert.ToInt32(strRes);
            #endregion
            #region AR66
            strRes = objIniSettings.ReadPrivateProfileStringKey("AR66.Comport", "ComPort", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " port setting";
                return false;
            }
            mtyp_CommAR66.PortNum = Convert.ToInt32(strRes);


            strRes = objIniSettings.ReadPrivateProfileStringKey("AR66.Comport", "Baud", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " baud setting";
                return false;
            }
            mtyp_CommAR66.Baud = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("AR66.Comport", "Parity", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " parity setting";
                return false;
            }
            mtyp_CommAR66.Parity = strRes;

            strRes = objIniSettings.ReadPrivateProfileStringKey("AR66.Comport", "DataBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " data bits setting";
                return false;
            }
            mtyp_CommAR66.DataBits = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("AR66.Comport", "StopBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = " stop bits setting";
                return false;
            }
            mtyp_CommAR66.StopBits = Convert.ToInt32(strRes);
            #endregion

            #region Print
            strRes = objIniSettings.ReadPrivateProfileStringKey("PackageLabelPrinter.Comport", "ComPort", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[PackageLabelPrinter.Comport] ComPort.";
                return false;
            }
            m_typ_CommPackageLabelPrinter.PortNum = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("PackageLabelPrinter.Comport", "Baud", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[PackageLabelPrinter.Comport] Baud.";
                return false;
            }
            m_typ_CommPackageLabelPrinter.Baud = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("PackageLabelPrinter.Comport", "Parity", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[PackageLabelPrinter.Comport] Parity.";
                return false;
            }
            m_typ_CommPackageLabelPrinter.Parity = strRes;

            strRes = objIniSettings.ReadPrivateProfileStringKey("PackageLabelPrinter.Comport", "DataBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[PackageLabelPrinter.Comport] DataBits.";
                return false;
            }
            m_typ_CommPackageLabelPrinter.DataBits = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("PackageLabelPrinter.Comport", "StopBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[PackageLabelPrinter.Comport] StopBits.";
                return false;
            }
            m_typ_CommPackageLabelPrinter.StopBits = Convert.ToInt32(strRes);
            #endregion

            #region Print2
            strRes = objIniSettings.ReadPrivateProfileStringKey("MACLabelPrinter.Comport", "ComPort", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[MACLabelPrinter.Comport] ComPort.";
                return false;
            }
            m_typ_CommMacLabelPrinter.PortNum = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("MACLabelPrinter.Comport", "Baud", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[MACLabelPrinter.Comport] Baud.";
                return false;
            }
            m_typ_CommMacLabelPrinter.Baud = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("MACLabelPrinter.Comport", "Parity", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[MACLabelPrinter.Comport] Parity.";
                return false;
            }
            m_typ_CommMacLabelPrinter.Parity = strRes;

            strRes = objIniSettings.ReadPrivateProfileStringKey("MACLabelPrinter.Comport", "DataBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[MACLabelPrinter.Comport] DataBits.";
                return false;
            }
            m_typ_CommMacLabelPrinter.DataBits = Convert.ToInt32(strRes);

            strRes = objIniSettings.ReadPrivateProfileStringKey("MACLabelPrinter.Comport", "StopBits", strPath);
            if (strRes == "Error")
            {
                TestData.Results.TestFailMessage = "[MACLabelPrinter.Comport] StopBits.";
                return false;
            }
            m_typ_CommMacLabelPrinter.StopBits = Convert.ToInt32(strRes);
           
            #endregion
            return true;
        }

        private bool ReadMDCSSettings()
        {
            string str_Path;
            string str_Res;
            ReadIniSettings RdSettings;

            RdSettings = new ReadIniSettings();
            str_Path = Application.StartupPath + "\\" + Program.gstr_Software_Number + ".ini";
            if (!System.IO.File.Exists(str_Path))
            {
                //MsgBox("Error: Can not locate " + str_Path, MsgBoxStyle.Critical, "INI Error");
                return false;
            }

            str_Res = RdSettings.ReadPrivateProfileStringKey("MDCS", "Path", str_Path);
            if (str_Res == "Error")
            {
                //MsgBox("Error: With MDCS Path", MsgBoxStyle.Critical, "INI Error");
                return false;
            }
            mstr_mdcsUrl = str_Res;

            str_Res = RdSettings.ReadPrivateProfileStringKey("MDCS", "enabled", str_Path);
            if (str_Res == "Error")
            {
                //MsgBox("Error: With MDCS enabled", MsgBoxStyle.Critical, "INI Error");
                return false;
            }
            if (str_Res == "1")
            {
                mbln_mdcsEnabled = true;
            }
            else
            {
                mbln_mdcsEnabled = false;
            }


            ////National Instruments Device
            //str_Res = RdSettings.ReadPrivateProfileStringKey("National Instruments", "device", str_Path);
            //if (str_Res == "Error")
            //{
            //    MessageBox.Show("Error reading National Instruments Device Name", "Config file error");
            //    return false;
            //}
            //m_daqDevice = str_Res.ToString();

            return true;
        }

        #endregion
        private bool ReadTranslationSettings()
        {
            string str_Path;
            string str_Res;
            ReadIniSettings RdSettings;

            RdSettings = new ReadIniSettings();
            str_Path = Application.StartupPath + "\\" + Program.gstr_Software_Number + ".ini";
            if (!System.IO.File.Exists(str_Path))
            {
                MessageBox.Show("Error: Can not locate " + str_Path, "INI Error");
                return false;
            }

            str_Res = RdSettings.ReadPrivateProfileStringKey("Translate", "enabled", str_Path);
            if (str_Res == "Error")
            {
                MessageBox.Show("Error: Translate setting", "INI Error");
                return false;
            }

            if (str_Res == "1")
                return true;
            else
                return false;
        }

        private bool WriteTestReportHeader()
        {
            string stampDate, stampTime;

            stampDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            stampTime = System.DateTime.Now.ToString("HH:mm:ss");

            DisplayMessage("******************************************************");
            DisplayMessage("***    F001716 PRODUCT NAME PCB BOARD LEVEL TEST REPORT    ***");
            DisplayMessage("******************************************************");
            DisplayMessage("Timestamp: " + stampDate + " " + stampTime);

            return true;
        }

        private void DisplayMessage(string strmes)
        {
            DisplayTestResults(strmes, false);
        }

        private long StartTimeInTicks()
        {
            return System.DateTime.Now.Ticks;
        }

        private double ElapseTimeInSeconds(long StartTimeInTicks)
        {
            return (System.DateTime.Now.Ticks - StartTimeInTicks) / System.TimeSpan.TicksPerSecond;
        }

        private int CheckRange(double ltl, double utl, double reading, string str_Units)
        {
            int i_Res = 0;

            DisplayMessage("Range: " + ltl.ToString("##0.000") + str_Units + " < " + reading.ToString("##0.000") + str_Units + " < " + utl.ToString("##0.000") + str_Units);

            if (reading < ltl)
                i_Res = -1;
            else if (reading > utl)
                i_Res = 1;

            return i_Res;
        }

        private bool GetMenuInfo(string cmdstr, string cmdterm, string cmdres, ref string cinfo)
        {
            int idx;

            //  pull out string information from any menu cmd
            cinfo = "";
            idx = cmdres.IndexOf(cmdstr, 0);
            if (idx == -1)
            {
                return false;
            }
            cmdres = cmdres.Substring(idx);
            if (cmdstr.Length > 0)
            {
                cmdres = cmdres.Replace(cmdstr, "");
            }

            idx = cmdres.IndexOf(cmdterm, 0);
            if (idx == -1)
            {
                return false;
            }
            cmdres = cmdres.Substring(0, idx);
            cmdres = cmdres.Replace(cmdterm, "");

            cinfo = cmdres.Trim().Replace(" ", "");
            return true;
        }

        private string GetSolutionPath()
        {
            int n;
            string stpath;

            stpath = Application.StartupPath;
            n = stpath.IndexOf("bin");
            if (n > 0)
            {
                return stpath.Substring(0, n);
            }
            else
            {
                return stpath;
            }
        }
        private void OnShipImageCallback(bool res, string errm)
        {
            //m_fImageComplete = true;
            if (res)
                mint_ImageShipComplete = 0;
            else
            {
                mint_ImageShipComplete = 1;
            }
        }

        private bool GetImage_CM5680(string imgsnp, string imgshp, string str_Path, string fn)
        {
            bool res;
            SerialPort serport = new SerialPort();
            serport.PortName = "COM" + m_Diagnostics.CommPortNumber;
            serport.Parity = System.IO.Ports.Parity.None;
            serport.StopBits = System.IO.Ports.StopBits.One;
            serport.BaudRate = 9600;
            serport.DataBits = 8;
            mint_ImageShipComplete = -1;
            ShipImage si = new ShipImage(serport);
            try
            {
                //See if data directory is not present then make it 
                if (System.IO.Directory.Exists(str_Path) == false)
                {
                    System.IO.Directory.CreateDirectory(str_Path);
                }

                //See if file is present then delete it 
                if (System.IO.File.Exists(str_Path + fn) == true)
                {
                    System.IO.File.Delete(str_Path + fn);
                }


                //add in the ShipImage event handler
                si.ev_ShipImageCallback += new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

                si.imgFilename = str_Path + fn;
                m_scannerSettings.Baud = 9600;
                m_scannerSettings.PortNum = m_Diagnostics.CommPortNumber;
                m_scannerSettings.DataBits = 8;
                m_scannerSettings.StopBits = System.IO.Ports.StopBits.One;
                m_scannerSettings.Parity = System.IO.Ports.Parity.None;

                si.imgSettings = this.m_scannerSettings;
                si.Init(844, 640);
                si.StartThread(imgsnp, imgshp);
                //while (m_fImageComplete == false)
                while (mint_ImageShipComplete == -1)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                si.HaltThread();
                //m_fImageComplete = false;
                si.CommClose();
                //remove the ShipImage event handler
                si.ev_ShipImageCallback -= new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

            }
            catch (Exception EX)
            {
                return false;
            }

            switch (mint_ImageShipComplete)
            {
                case 0:
                    res = true;
                    break;
                default:
                    res = false;
                    break;
            }
            return res;

        }
        private bool GetImage_CM3680(string imgsnp, string imgshp, string str_Path, string fn)
        {
            bool res;
            SerialPort serport = new SerialPort();
            serport.PortName = "COM" + m_Diagnostics.CommPortNumber;
            serport.Parity = System.IO.Ports.Parity.None;
            serport.StopBits = System.IO.Ports.StopBits.One;
            serport.BaudRate = 9600;
            serport.DataBits = 8;
            mint_ImageShipComplete = -1;
            ShipImage si = new ShipImage(serport);
            try
            {
                //See if data directory is not present then make it 
                if (System.IO.Directory.Exists(str_Path) == false)
                {
                    System.IO.Directory.CreateDirectory(str_Path);
                }

                //See if file is present then delete it 
                if (System.IO.File.Exists(str_Path + fn) == true)
                {
                    System.IO.File.Delete(str_Path + fn);
                }


                //add in the ShipImage event handler
                si.ev_ShipImageCallback += new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

                si.imgFilename = str_Path + fn;
                m_scannerSettings.Baud = 9600;
                m_scannerSettings.PortNum = m_Diagnostics.CommPortNumber;
                m_scannerSettings.DataBits = 8;
                m_scannerSettings.StopBits = System.IO.Ports.StopBits.One;
                m_scannerSettings.Parity = System.IO.Ports.Parity.None;

                si.imgSettings = this.m_scannerSettings;
                si.Init(640, 480);
                si.StartThread(imgsnp, imgshp);
                //while (m_fImageComplete == false)
                while (mint_ImageShipComplete == -1)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                si.HaltThread();
                //m_fImageComplete = false;
                si.CommClose();
                //remove the ShipImage event handler
                si.ev_ShipImageCallback -= new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

            }
            catch (Exception EX)
            {
                return false;
            }

            switch (mint_ImageShipComplete)
            {
                case 0:
                    res = true;
                    break;
                default:
                    res = false;
                    break;
            }
            return res;

        }
        private bool GetImage_CM2180(string imgsnp, string imgshp, string str_Path, string fn)
        {
            bool res;
            SerialPort serport = new SerialPort();
            serport.PortName = "COM" + m_Diagnostics.CommPortNumber;
            serport.Parity = System.IO.Ports.Parity.None;
            serport.StopBits = System.IO.Ports.StopBits.One;
            serport.BaudRate = 9600;
            serport.DataBits = 8;
            mint_ImageShipComplete = -1;
            ShipImage si = new ShipImage(serport);
            try
            {
                //See if data directory is not present then make it 
                if (System.IO.Directory.Exists(str_Path) == false)
                {
                    System.IO.Directory.CreateDirectory(str_Path);
                }

                //See if file is present then delete it 
                if (System.IO.File.Exists(str_Path + fn) == true)
                {
                    System.IO.File.Delete(str_Path + fn);
                }


                //add in the ShipImage event handler
                si.ev_ShipImageCallback += new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

                si.imgFilename = str_Path + fn;
                m_scannerSettings.Baud = 9600;
                m_scannerSettings.PortNum = m_Diagnostics.CommPortNumber;
                m_scannerSettings.DataBits = 8;
                m_scannerSettings.StopBits = System.IO.Ports.StopBits.One;
                m_scannerSettings.Parity = System.IO.Ports.Parity.None;

                si.imgSettings = this.m_scannerSettings;
                si.Init(1280, 800);
                si.StartThread(imgsnp, imgshp);
                //while (m_fImageComplete == false)
                while (mint_ImageShipComplete == -1)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                si.HaltThread();
                //m_fImageComplete = false;
                si.CommClose();
                //remove the ShipImage event handler
                si.ev_ShipImageCallback -= new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

            }
            catch (Exception EX)
            {
                return false;
            }

            switch (mint_ImageShipComplete)
            {
                case 0:
                    res = true;
                    break;
                default:
                    res = false;
                    break;
            }
            return res;

        }
        private bool CommPollPowerup(Single tmout)
        {
            long tmr;
            string cmdres = "";
            bool check1 = false;
            tmr = StartTimeInTicks();
            while (ElapseTimeInSeconds(tmr) < tmout)
            {
                m_Diagnostics.dPortState = true;
                clsUtils.Dly(0.5);
                check1 = m_Diagnostics.HSM_Send_Menu_Command('M', CMD_ConnectRS232, "", '.', (char)0, ref cmdres);

                if (check1 == true)
                {
                    DisplayMessage(cmdres);
                    return true;
                }
                m_Diagnostics.dPortState = false;
                clsUtils.Dly(0.5);
                Application.DoEvents();
            }
            return check1;
        }


        #endregion

        #region All subtests listed
        // ******************************************************
        // * All subtests listed below
        // ******************************************************
        private bool RunTests(TestId id)
        {
            TestId[] tstList;
            const int TEST_LIST_MAX = 50;
            int tstIndex;
            long startTime = 0;

            if (mbln_runInitialized == false)
            {
                if (InitNewRun() == false)
                {
                    return false;
                }
                //MessageBox.Show("Please Load Unit into Fixture", "Load Unit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Instruction(mstr_Testing);
            Ini_TestLoopVariables();
            startTime = clsUtils.StartTimeInTicks();
            WriteTestReportHeader();
            tstList = new TestId[TEST_LIST_MAX];
            ConstructTestList(id, ref tstList);
            tstIndex = 0;
            if (SNBox.Text.Length != 10)
            {
                SNBox.Text = "";
                SNBox.Focus();
                return false;
            }
            TestData.UnitInfo.str_SN = SNBox.Text;
            m_fixctrl.RestoreFixture();

            while (tstList[tstIndex] != TestId.TestNone)
            {
                if (SubtestIterate(tstList[tstIndex]) == false)
                {
                    TestData.Results.TestPassed = false;
                    break;
                }

                if (UserStop == true)
                {
                    TestData.Results.TestFailMessage_Name = "ER9003_EXTRA_Stop_Test";
                    TestData.Results.TestPassed = false;
                    break;
                }
                tstIndex += 1;
                Application.DoEvents();
            }

            // Get total test time
            TestData.UnitInfo.total_time = clsUtils.ElapseTimeInSeconds(startTime);
            DisplayMessage(TestData.Results.TestFailMessage);

            TestPowerOff();

            //  set up for saving data to logfile and database
            if (TestData.Results.TestPassed == false)
            {
                TestData.Results.TestStatus = "0";
                if (TestData.Results.TestFailMessage.Length == 0)
                {
                    TestData.Results.TestFailNumber = TestData.Results.TestFailMessage_Name.Substring(2, 4);
                    TestData.Results.TestFailMessage = m_Resource_FailMessage.GetString(TestData.Results.TestFailMessage_Name);
                }

                TestData.Results.TestFailMessage_UI = TestData.Results.TestFailMessage;// m_Resource_FailMessage.GetString(TestData.Results.TestFailMessage_Name);

                if (TestData.Results.TestFailNumber == "")
                    TestData.Results.TestFailNumber = "0000";
            }
            else
            {
                TestData.Results.TestStatus = "1";
                TestData.Results.TestFailNumber = "0";
            }

            if (this.diagsToolStripMenuItem.Checked == false)
            {
                mobj_SaveData.p_TestData = TestData;
                mobj_SaveData.SaveTestData();
                if (mbln_mdcsEnabled == true)
                {
                    mobj_SaveDataMDCS.UseModeProduction = rbn_MDCSProduction.Checked;
                    mobj_SaveDataMDCS.ServerName = mstr_mdcsUrl;
                    mobj_SaveDataMDCS.p_TestData = TestData;

                    mobj_SaveDataMDCS.DeviceName = "F001716_EOL";
                    mobj_SaveDataMDCS.WriteAllData();
                    if (TestData.Results.TestPassed == true)
                        mobj_SaveDataMDCS.SaveDataToQCDCS(true, TestData.UnitInfo.str_SN);
                    //else
                    //    mobj_SaveDataMDCS.SaveDataToQCDCS(false, TestData.UnitInfo.str_SN);
                }
            }

            WriteTestReportHeader();
            SaveTestReport();


            string myError = "";
            if (TestData.Results.TestPassed == true && mobj_SaveDataMDCS.UseModeProduction == true && this.diagsToolStripMenuItem.Checked == false /*&& mbln_SkipSerialNumberAndLabelPrintOnce == false*/)
            {
                PrintLabel(ref myError);
            }
            if (this.diagsToolStripMenuItem.Checked == false)
                PassFail();

            return true;
        }

        private bool SubtestIterate(TestId selector)
        {
            bool retval = false;
            switch (selector)
            {
                case TestId.TestPowerOn232:
                    DisplayMessage("Start Test Power On");
                    retval = TestConnectRS232();
                    break;

                case TestId.TestUnitCurrent:
                    DisplayMessage("Start Test Unit Current");
                    retval = TestUnitCurrent();
                    break;
                
                case TestId.TestFirmwareVerify:
                    DisplayMessage("Test FirmwareVerify");
                    retval = TestFirmwareVerify();
                    break;
                case TestId.TestRtsCts:
                    DisplayMessage("Test RtsCts");
                    retval = TestRtsCts();
                    break;
                case TestId.TestReadMACAddresses:
                    DisplayMessage("Test Read MAC Addresses");
                    retval = TestReadMACAddresses();
                    break;

                case TestId.TestConnectUSB:
                    DisplayMessage("Test Connect USB");
                    retval = TestConnectUSB();
                    break;

                case TestId.TestCradle:
                    DisplayMessage("Test Cradle");
                    retval = TestCradle();
                    break;

                case TestId.TestConnectBluetooth:
                    DisplayMessage("Test Connect Bluetooth");
                    retval = TestConnectBluetooth();
                    break;

                case TestId.TestBeeper:
                    DisplayMessage("Test Beeper");
                    retval = TestBeeper();
                    break;

                case TestId.TestPager:
                    DisplayMessage("Test Pager");
                    retval = TestPager();
                    break;

                case TestId.TestIndicators:
                    DisplayMessage("Test Indicators");
                    retval = TestIndicators();
                    break;

                case TestId.TestSetSerialNumber:
                    DisplayMessage("Test SetSerial Number");
                    retval = TestSetSerialNumber();
                    break;

                case TestId.TestSetStatus:
                    DisplayMessage("Test Set Status");
                    retval = TestSetStatus();
                    break;

                case TestId.TestDefault:
                    DisplayMessage("Test Default");
                    retval = TestDefault();
                    break;

                default:
                    retval = false;
                    break;
            }

            if (retval == false)
            {
                DisplayMessage("Subtest Failed");
                return false;
            }
            DisplayMessage("Subtest Complete" + "\r\n");
            return true;
        }

        private bool WaitForPowerUP()
        {
            long st = 0;
            double Current = 0;
            bool check = false;
            st = clsUtils.StartTimeInTicks();
            double OnMinCurrent = 0.02F;
            while ((Current < OnMinCurrent) && (clsUtils.ElapseTimeInSeconds(st) < 10))
            {
                check = m_fixctrl.ReadCurrent(ref Current, 50);
                if (Current > OnMinCurrent) break;
            }
            if (Current < OnMinCurrent) return false;

            return true;
        }

        private bool SendCMD_HID(string input)
        {
            try
            {

                string hexOutput = "";
                char[] values = input.ToCharArray();
                mtype_DeviceDetect.RecievedData = "";
                foreach (char letter in values)
                {
                    int value = Convert.ToInt32(letter);
                    hexOutput = hexOutput + String.Format("0x{0:X}", value) + " ";
                }
                int size = input.Length + 5;
                string sendData = "0xfd " + String.Format("0x{0:X}", size) + " 0x16 m \r " + hexOutput;

                for (int i = size; i < this.usbHidPort1.SpecifiedDevice.OutputReportLength - 1; i++)
                {
                    sendData = sendData + "0x00 ";

                }
                string[] arrText = sendData.Split(' ');
                byte[] data = new byte[this.usbHidPort1.SpecifiedDevice.OutputReportLength];

                for (int i = 0; i < this.usbHidPort1.SpecifiedDevice.OutputReportLength; i++)
                {

                    if (arrText[i] != "")
                    {
                        int value = 0;

                        if (arrText[i].Contains("0x"))
                        {
                            arrText[i] = arrText[i].Replace("0x", "");
                            value = Int32.Parse(arrText[i], System.Globalization.NumberStyles.HexNumber);
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToChar(arrText[i]));
                        }
                        data[i] = (byte)Convert.ToByte(value);
                    }
                }
                if (this.usbHidPort1.SpecifiedDevice != null)
                {
                    this.usbHidPort1.SpecifiedDevice.SendData(data);
                }
                else
                {
                    MessageBox.Show("Sorry but your device is not present. Plug it in!! ");
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }

        private bool AR66_CheckArrivePosition1(int checkTime)
        {
            bool check = false;
            long tmr = clsUtils.StartTimeInTicks();
            while (true)
            {
                Application.DoEvents();
                if (m_b_AR66_position1 == true)
                {
                    check = true;
                    break;
                }
                if (AR66_AlarmY == false)
                {
                    check = false;
                    MessageBox.Show("AR66_AlarmY: " + str_AR66_AlarmY);
                    break;
                }
                if (clsUtils.ElapseTimeInSeconds(tmr) > checkTime)
                {
                    check = false;
                    break;
                }
            }
            if (check == false)
            {
                TestData.Results.TestFailMessage_Name = "ER4649_FIXTURE_FAILURE";
                TestData.Results.TestFailNumber = TestData.Results.TestFailMessage_Name.Substring(2, 4);
                TestData.Results.TestFailMessage = "Fixture Failure: Motor AR66_Y position";
                return false;
            }
            m_b_AR66_position1 = false;
            return check;
        }
        private bool AR66_CheckArrivePosition(int checkTime)
        {
            bool check = false;
            long tmr = clsUtils.StartTimeInTicks();
            while (true)
            {
                Application.DoEvents();
                if (m_b_AR66_position == true)
                {
                    check = true;
                    break;
                }
                if (AR66_AlarmX == false)
                {
                    check = false;
                    MessageBox.Show("AR66_AlarmX: " + str_AR66_AlarmX);
                    break;
                }
                if (clsUtils.ElapseTimeInSeconds(tmr) > checkTime)
                {
                    check = false;
                    break;
                }
            }
            m_b_AR66_position = false;
            if (check == false)
            {
                TestData.Results.TestFailMessage_Name = "ER4649_FIXTURE_FAILURE";
                TestData.Results.TestFailNumber = TestData.Results.TestFailMessage_Name.Substring(2, 4);
                TestData.Results.TestFailMessage = "Fixture Failure: Motor AR66_X position";
                return false;
            }
            return check;
        }


        private bool SaveDataToBMP(Int32[] ImageData)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - b.Width * 3;
                byte red, green, blue;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return true;
        }
        #endregion

        #region Fixture Control

        #region HW Init
        public bool InitHW()
        {
            string reterr;
            if (m_fixctrl.InitFixture() == false)
            {
                m_str_Message = m_Resource_FailMessage.GetString("ER9004_EXTRA_Fixture_INIT");
                reterr = m_fixctrl.Error;
                Instruction(m_str_Message + " " + reterr, true);
                return false;
            }
            return true;
        }
        #endregion

        #region HW Close
        public bool CloseHW()
        {
            if (m_Diagnostics != null) m_Diagnostics.dPortState = false;
            return true;
        }
        #endregion

        #region Pown off
        public bool TestPowerOff()
        {
            DisplayMessage("Powering unit off ");

            m_Diagnostics.dPortState = false;

            //Power off
            m_fixctrl.PowerControl(false);//powen on
            //m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][1].LineName, m_PortLine.Port[0][1].Low);  // power-ctl off
            clsUtils.Dly(2);

            DisplayMessage("OK");
            return true;
        }
        #endregion

        #region Pown on
        public bool TestPowerOn()
        {
            bool check = false;
            DisplayMessage("Powering unit up ");
            m_fixctrl.PowerControl(true);//powen on

            m_Diagnostics.dPortState = true;
            //Wait for unit to draw minimum current
            if (WaitForPowerUP() == false)
            {
                TestData.Results.TestFailMessage_Name = "ER1035_DEAD_UNIT";
                TestData.Results.TestFailNumber = TestData.Results.TestFailMessage_Name.Substring(2, 4);
                TestData.Results.TestFailMessage = "Fail to wait for power up.";
                return false;
            }
            DisplayMessage("OK");

            return true;
        }
        #endregion

        #region Agilent

        private bool AgilentOff()
        {
            mobj_AgilentPS.SelectOutput(PSPortEnum.PSOUT2);
            mobj_AgilentPS.OutputEnable(false);
            mobj_AgilentPS.SetVoltage(0.0F);
            clsUtils.Dly(0.15);

            return true;
        }

        private bool AgilentOn()
        {
            mobj_AgilentPS.SelectOutput(PSPortEnum.PSOUT2);
            mobj_AgilentPS.SetVoltage(5.0F);
            mobj_AgilentPS.OutputEnable(true);
            clsUtils.Dly(0.1);

            return true;
        }
        #endregion

        #endregion

        #region print function
        private bool Ini_InputSWFiles()
        {
            try
            {
                frm_SW.ShowDialog();
                if (frm_SW.Abort == true)
                {
                    TestData.Results.TestFailMessage = "User aborted software info!";
                    return false;
                }

                TestData.TestInfo.ItemNum = frm_SW.ItemNum;
                TestData.TestInfo.BomRev = frm_SW.BomRev;
                TestData.TestInfo.Description = frm_SW.Description;
                TestData.TestInfo.SWImageNumber = frm_SW.Image_SW_Num;
                TestData.TestInfo.EMBEDDED_FW_REV = TestData.TestInfo.SWImageNumber;
                TestData.TestInfo.PCDownloadDirectory = frm_SW.DownloadDirectory;
                TestData.TestInfo.model = frm_SW.MODEL;
                TestData.TestInfo.QADLine = frm_SW.QADLine;

            }
            catch (Exception ex)
            {
                TestData.Results.TestFailMessage = " Exception Error in SW entry: " + ex.Message;
                return false;
            }
            return true;
        }

        private bool Ini_Labels()
        {
            string BagLabelNum = "";

            frm_LabelInfo.ShowDialog();
            if (frm_LabelInfo.Abort == true)
            {
                TestData.Results.TestFailMessage = "User Aborted Label Entry Operation";
                this.cbx_PrintLabelsYes.CheckState = CheckState.Unchecked;
                iniLabel = false;
                return false;
            }
            BagLabelNum = frm_LabelInfo.KitLabelNum;
            this.lab_KitLabelnumber.Visible = true;
            this.lab_KitLabelnumber.Text = frm_LabelInfo.txt_KitLabel.Text;
            lbl_special.Visible = true;
            lbl_special.Text = frm_LabelInfo.txt_MACLabel.Text;
            iniLabel = true;
            return true;
        }

        private bool InitPackageLabelInfo(ref string sErrorMessage)
        {
            string str_Path = "";
            mudt_TestData.LabelPrintData.Label1ComportSettings.PortNum = m_typ_CommPackageLabelPrinter.PortNum;
            mudt_TestData.LabelPrintData.Label1ComportSettings.Baud = m_typ_CommPackageLabelPrinter.Baud;

            mudt_TestData.LabelPrintData.Label1ComportSettings.Parity = m_typ_CommPackageLabelPrinter.Parity;


            mudt_TestData.LabelPrintData.Label1ComportSettings.DataBits = m_typ_CommPackageLabelPrinter.DataBits;
            mudt_TestData.LabelPrintData.Label1ComportSettings.StopBits = m_typ_CommPackageLabelPrinter.StopBits;

            str_Path = System.IO.Directory.GetCurrentDirectory() + "\\Label";
            mudt_TestData.LabelPrintData.LabelPrintFile = "";
            mudt_TestData.LabelPrintData.LabelPrintFile = str_Path + "\\" + lab_KitLabelnumber.Text + ".pof";


            mudt_TestData.TestResult.TestFailMessage = "";
            mudt_TestData.TestResult.TestFailNumber = "";
            mudt_TestData.TestResult.TestPassed = false;

            return true;
        }

        private bool InitMacLabelInfo(ref string sErrorMessage)
        {
            string str_Path = "";
            mudt_TestData.LabelPrintData.Label1ComportSettings.PortNum = m_typ_CommMacLabelPrinter.PortNum;
            mudt_TestData.LabelPrintData.Label1ComportSettings.Baud = m_typ_CommMacLabelPrinter.Baud;

            mudt_TestData.LabelPrintData.Label1ComportSettings.Parity = m_typ_CommMacLabelPrinter.Parity;


            mudt_TestData.LabelPrintData.Label1ComportSettings.DataBits = m_typ_CommMacLabelPrinter.DataBits;
            mudt_TestData.LabelPrintData.Label1ComportSettings.StopBits = m_typ_CommMacLabelPrinter.StopBits;

            str_Path = System.IO.Directory.GetCurrentDirectory() + "\\Label";
            mudt_TestData.LabelPrintData.LabelPrintFile = "";
            mudt_TestData.LabelPrintData.LabelPrintFile = str_Path + "\\" + lbl_special.Text + ".pof";


            mudt_TestData.TestResult.TestFailMessage = "";
            mudt_TestData.TestResult.TestFailNumber = "";
            mudt_TestData.TestResult.TestPassed = false;

            return true;
        }

        private bool InitMarkValueandItem(ref string sErrormessage)
        {
            for (int i = 0; i < MarkerItem.Length; i++)
            {
                MarkerItem[i] = "";
                MarkerValue[i] = "";
            }

            return true;
        }

        public bool AddMarkandValue(string sMark, string sValue)
        {
            for (int i = 0; i < 10; i++)
            {
                if (MarkerValue[i].Length <= 0)
                {
                    MarkerItem[i] = sMark;
                    MarkerValue[i] = sValue;
                    break;
                }
            }

            return true;
        }

        private bool PrintPackageLabels(bool bDuplicatePrint)
        {
            bool retval;

            try
            {
                mobj_Label1.p_TestData = mudt_TestData;
                mobj_Label1.DuplicatePrint = bDuplicatePrint;
                //retval = mobj_Label1.PrintLabel(ref mudt_TestData.LabelPrintData.Label1ComportSettings, ref mudt_TestData.LabelPrintData.LabelPrintFile);
                retval = mobj_Label1.PrintLabel();
                if (retval == false)
                {
                    TestData.Results.TestFailNumber = "ER 0054";
                    mudt_TestData.TestResult.TestFailMessage = TestData.Results.TestFailNumber + ":Printing " + mudt_TestData.LabelPrintData.LabelPrintFile;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                TestData.Results.TestFailNumber = "ER 0053";
                mudt_TestData.TestResult.TestFailMessage = TestData.Results.TestFailNumber + ":Label Print Fail! " + ex.Message.ToString();
                return false;
            }
        }

        public bool PrintPackageLabelFunction(ref string sErrorMessage)
        {

            if (cbx_PrintLabelsYes.Checked == false)
            {
                return true;
            }

            if (InitPackageLabelInfo(ref sErrorMessage) == false)
            {
                return false;
            }
            mobj_Label1 = new Honeywell.LabelPrintDLL.cls_PrintLabels();
            InitMarkValueandItem(ref sErrorMessage);
            AddMarkandValue("BOMREV", frm_SW.BomRev);
            AddMarkandValue("DSN", TestData.UnitInfo.str_SN);
            AddMarkandValue("ITEM", frm_SW.ItemNum);
            AddMarkandValue("DESC", frm_SW.Description);
            AddMarkandValue("MODEL", TestData.TestInfo.model);

            mudt_TestData.LabelPrintData.ItemMarker = MarkerItem;
            mudt_TestData.LabelPrintData.ItemValue = MarkerValue;

            if (PrintPackageLabels(false) == false)
            {
                sErrorMessage = "PackageLabel Print Failure!";
                return false;
            }
            else
            {
                DisplayTestResults("PackageLabel Printed successfully!", true);
                return true;
            }
        }

        public bool PrintMacLabelFunction(ref string sErrorMessage)
        {

            if (cbx_PrintLabelsYes.Checked == false)
            {
                return true;

            }

            if (InitMacLabelInfo(ref sErrorMessage) == false)
            {
                return false;
            }
            mobj_Label1 = new Honeywell.LabelPrintDLL.cls_PrintLabels();
            InitMarkValueandItem(ref sErrorMessage);
            AddMarkandValue("MAC", TestData.UnitInfo.BT_MacAddress);

            //mudt_TestData.LabelPrintData.SerialNumber = TestData.UnitInfo.str_SN;

            mudt_TestData.LabelPrintData.ItemMarker = MarkerItem;
            mudt_TestData.LabelPrintData.ItemValue = MarkerValue;

            if (PrintPackageLabels(false) == false)
            {
                sErrorMessage = "Agency Label Print Failure!";
                return false;
            }
            else
            {
                DisplayTestResults("Agency Label Printed successfully!", true);
                return true;
            }
        }

        public bool PrintLabel(ref string sErrormessage)
        {
            if (PrintPackageLabelFunction(ref sErrormessage) == false)
            {
                TestData.Results.TestFailMessage_Name = "ER4098_LABEL";
                TestData.Results.TestFailNumber = TestData.Results.TestFailMessage_Name.Substring(2, 4);
                TestData.Results.TestFailMessage = " Print label Fail ";
                return false;
            }

            if (PrintMacLabelFunction(ref sErrormessage) == false)
            {
                TestData.Results.TestFailMessage_Name = "ER4098_LABEL";
                TestData.Results.TestFailNumber = TestData.Results.TestFailMessage_Name.Substring(2, 4);
                TestData.Results.TestFailMessage = " Print label Fail ";
                return false;
            }
            Thread.Sleep(500);
            return true;
        }

        private void BT_REPRINT_Click(object sender, EventArgs e)
        {

        }

        private void cbx_PrintLabelsYes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbx_PrintLabelsYes.Checked == true /*&& mbln_runInitialized == true*/)
            {
                if (iniLabel == false)
                {
                    if (Ini_Labels() == false) return;
                }
                this.lab_KitLabelnumber.Visible = true;
                this.lab_KitLabelnumber.Text = frm_LabelInfo.txt_KitLabel.Text;
                lbl_special.Visible = true;
                lbl_special.Text = frm_LabelInfo.txt_MACLabel.Text;
                this.Group_Print.Size = new System.Drawing.Size(202, 138);
            }

            else
            {
                lab_KitLabelnumber.Visible = false;
                this.Group_Print.Size = new System.Drawing.Size(202, 46);

            }

        }

        #endregion

        private void AR66Motor_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            //AR66Motor.Enabled = false;
            //string position = "";
            //string Alarm = "";
            //MotorAR66.AR66_ReadPositon(1, ref position);
            //AR66Position.Text = "AR66 Motor position: " + position;
            //if (position == TestData.AR66Status.CurrentPosition.ToString() && AR66_AlarmX == true)
            //{
            //    m_b_AR66_position = true;
            //}
            //else
            //{
            //    AR66Motor.Enabled = true;
            //}   
        }

        #endregion

        private void AR66Motor1_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            //AR66Motor1.Enabled = false;
            //string position = "";
            //string Alarm = "";
            //MotorAR66.AR66_ReadPositon(2, ref position);
            ////AR66_AlarmY = AR66StepMotor_AR1.getAlarm(2, ref Alarm);
            ////str_AR66_AlarmY = Alarm;
            //AR66Position.Text = "AR66 Motor position: " + position;
            //if (position == TestData.AR66Status1.CurrentPosition.ToString() && AR66_AlarmY == true)
            //{
            //    m_b_AR66_position1 = true;
            //}
            //else
            //{
            //    AR66Motor1.Enabled = true;
            //}  
        }


        #region"USB-HID"
        private void HIDPOS_Tick(object sender, EventArgs e)
        {
            try
            {
                this.usbHidPort1.ProductId = Int32.Parse("0E41", System.Globalization.NumberStyles.HexNumber);
                this.usbHidPort1.VendorId = Int32.Parse("0C2E", System.Globalization.NumberStyles.HexNumber);
                this.usbHidPort1.CheckDevicePresent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void usbHidPort1_OnDataRecieved(object sender, DataRecievedEventArgs args)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DataRecievedEventHandler(usbHidPort1_OnDataRecieved), new object[] { sender, args });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                string return_data = "";
                int datasize = Convert.ToInt32(Convert.ToChar(System.Text.Encoding.Default.GetString(args.data, 1, 1)));
                for (int i = 5; i < datasize + 5; i++)
                {
                    return_data += System.Text.Encoding.Default.GetString(args.data, i, 1);

                }
                this.rtb_TestResults.Text = this.rtb_TestResults.Text + return_data;
                mtype_DeviceDetect.RecievedData = return_data;
            }
        }

        private void usbHidPort1_OnDataSend(object sender, EventArgs e)
        {

        }

        private void usbHidPort1_OnDeviceArrived(object sender, EventArgs e)
        {
            this.rtb_TestResults.Text = rtb_TestResults.Text + "\r\nDetect USB_HID Device.\r\n";
            mtype_DeviceDetect.USBHIDDetect = true;
            mtype_DeviceDetect.RS232Detect = false;
        }

        private void usbHidPort1_OnDeviceRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usbHidPort1_OnDeviceRemoved), new object[] { sender, e });
            }
            else
            {
                this.rtb_TestResults.Text = rtb_TestResults.Text + "\r\nRemove USB_HID Device.\r\n";
                mtype_DeviceDetect.USBHIDDetect = false;
                mtype_DeviceDetect.RS232Detect = true;
            }
        }

        private void usbHidPort1_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            this.rtb_TestResults.Text = rtb_TestResults.Text + "\r\nDetect USB_HID Device.\r\n";
            mtype_DeviceDetect.USBHIDDetect = true;
            mtype_DeviceDetect.RS232Detect = false;
        }

        private void usbHidPort1_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usbHidPort1_OnSpecifiedDeviceRemoved), new object[] { sender, e });
            }
            else
            {
                this.rtb_TestResults.Text = rtb_TestResults.Text + "\r\nRemove USB_HID Device.\r\n";
                mtype_DeviceDetect.USBHIDDetect = false;
                mtype_DeviceDetect.RS232Detect = true;
            }
        }
        #endregion

        private void SNBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btn_Start.PerformClick();
        }

        private void testReadBarcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb_TestResults.Clear();

        }
        private void readBarcodeLoopTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void testCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb_TestResults.Clear();
        }

        private void testBeeperToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void testLEDToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void testAPLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cbx_PrintLabelsYes_CheckedChanged_1(object sender, EventArgs e)
        {
            if (this.cbx_PrintLabelsYes.Checked == true /*&& mbln_runInitialized == true*/)
            {
                if (iniLabel == false)
                {
                    if (Ini_Labels() == false) return;
                }
                this.lab_KitLabelnumber.Visible = true;
                this.lab_KitLabelnumber.Text = frm_LabelInfo.txt_KitLabel.Text;
                this.Group_Print.Size = new System.Drawing.Size(202, 127);
            }

            else
            {
                lab_KitLabelnumber.Visible = false;
                this.Group_Print.Size = new System.Drawing.Size(202, 46);

            }
        }

        private void BT_REPRINT_Click_1(object sender, EventArgs e)
        {
            string myError = "";
            if (mobj_SaveDataMDCS.UseModeProduction == true && this.diagsToolStripMenuItem.Checked == false /*&& mbln_SkipSerialNumberAndLabelPrintOnce == false*/)
            {
                PrintLabel(ref myError);
            }
        }


        /// <summary>
        /// //test function for N57
        /// </summary>
        /// <returns></returns>
        private bool TestShortOpenCurrent()
        {

            double Current = 0;
            bool b_ret;
            double utl, ltl;
            int range;

            utl = 0.5;
            ltl = 0.02;

            b_ret = m_fixctrl.ReadCurrent(ref Current, 50);

            range = CheckRange(ltl, utl, Current, "A");
            TestData.UnitInfo.ShortOpen_Current = Current;
            if (range != 0)
            {
                if (range == 1)
                {
                    TestData.Results.TestFailMessage_Name = "ER1025_HI_BD_CUR";
                }
                else
                {
                    TestData.Results.TestFailMessage_Name = "ER1024_LOW_BD_CUR";
                }

                return false;

            }

            return true;
        }
        private bool TestCurrent()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            double ltl, utl;
            double Current = 0;
            int range;

            DisplayMessage("Standby current");
            utl = 50;
            ltl = 0.01;

            m_fixctrl.ReadCurrent(ref Current, 50);
            TestData.UnitInfo.UnitCurrent = Current;
            range = CheckRange(ltl, utl, Current, "A");
            if (range != 0)
            {
                if (range == 1)
                {
                    TestData.Results.TestFailMessage_Name = "ER1025_HI_BD_CUR";
                }
                else
                {
                    TestData.Results.TestFailMessage_Name = "ER1024_LOW_BD_CUR";
                }

                return false;
            }

            DisplayMessage("Test Mode2 Current");
            strCmd = "SCNAIM1;SCNLED0";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', "SCNLED0\x06", ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strCmd = "\x16T\r";
            m_Diagnostics.SendCommand(strCmd, 1, "");
            clsUtils.Dly(3);

            utl = 50;
            ltl = 0.1;
            m_fixctrl.ReadCurrent(ref Current, 50);
            TestData.UnitInfo.UnitCurrent2 = Current;

            strCmd = "\x16U\r";
            m_Diagnostics.SendCommand(strCmd, 1, "");

            range = CheckRange(ltl, utl, Current, "A");
            if (range != 0)
            {
                if (range == 1)
                {
                    TestData.Results.TestFailMessage_Name = "ER1025_HI_BD_CUR";
                }
                else
                {
                    TestData.Results.TestFailMessage_Name = "ER1024_LOW_BD_CUR";
                }

                return false;

            }

            return true;
        }
        private bool TestConnectRS232()
        {
            bool b_ret = false;
            string cmdres = "";

            m_fixctrl.PowerControl(true);
            m_fixctrl.CableSel232();
            clsUtils.Dly(5);
            m_Diagnostics.dSettings = mySettings;
            m_Diagnostics.dPortState = true;
            m_Diagnostics.HSM_Send_Menu_Command('M', CMD_ConnectRS232, "", '.', (char)0, ref cmdres);
            b_ret = CommPollPowerup(15);

            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            return true;

        }

        private bool TestUnitCurrent()
        {
            double cur = 0;
            double ltl, utl;
            int iRange;

            ltl = 0;
            utl = 50;

            m_fixctrl.ReadCurrent(ref cur, 50);
            TestData.UnitInfo.ShortOpen_Current = cur;
            DisplayMessage("Read unit current: " + (cur*1000).ToString("0.00") + "mA");
            iRange = CheckRange(ltl, utl, cur, "A");

            if (iRange != 0)
            {
                if (iRange == -1)
                {
                    TestData.Results.TestFailMessage_Name = "ER1024_LOW_BD_CUR";
                    return false;
                }
                else
                {
                    TestData.Results.TestFailMessage_Name = "ER1025_HI_BD_CUR";
                    return false;
                }
            }

            return true;
        }
        private bool TestFirmwareVerify()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string strDname = "";
            string strUsageType = "";
            string strDirectionType = "";

            strCmd = "FIMPNM";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('Y', strCmd, "", '?', '!', ref strRes);
            DisplayMessage("Firmware Version: " + strRes);

            TestData.UnitInfo.embedded_rev = m_Diagnostics.GetMenuInfo(strCmd, strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }
            if (TestData.TestInfo.EMBEDDED_FW_REV.Contains(TestData.UnitInfo.embedded_rev ) == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2067_WRONG_FIRMWARE";
                return false;
            }


            strCmd = "D_NAME";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            DisplayMessage("Firmware Name: " + strRes);
            strDname = strRes;
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            switch(mtyp_McConfig.mcUsageType)
            {
                case enUsageType.General:
                    strUsageType = "General";
                    break;
                case enUsageType.Healthcare:
                    strUsageType = "Healthcare";
                    break;
            }

            switch(mtyp_McConfig.mcDirectionType)
            {
                case enDirectionType.Horizontal:
                    strDirectionType = "Horizontal";
                    break;
                case enDirectionType.Vertical:
                    strDirectionType = "Vertical";
                    break;
            }

            if (strDname.Contains(strUsageType) == false || strDname.Contains(strDirectionType) ==false)
            {
                TestData.Results.TestFailMessage_Name = "ER2067_WRONG_FIRMWARE";
                return false;
            }

            return true;
        }

        private bool TestRtsCts()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            bool b_sts;

            DisplayMessage("Set 232CTS1");
            strCmd = "232CTS1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            clsUtils.Dly(2);         
            b_sts = m_Diagnostics.cCtsState;
            DisplayMessage("Read Scanner CTS State: " + b_sts.ToString());

            if (b_sts == true)
            {
                TestData.Results.TestFailMessage_Name = "ER2311_RTS_CTS";
                return false;
            }

            //should not recive ack
            DisplayMessage("Set Scanner Rts disable");
            m_Diagnostics.cRtsEnable = false;
            clsUtils.Dly(0.5);
            strCmd = "MATCMD?";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == true)
            {
                TestData.Results.TestFailMessage_Name = "ER2311_RTS_CTS";
                return false;
            }

            if (m_Diagnostics.cCtsState == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2311_RTS_CTS";
                return false;
            }

            // should recive ack
            DisplayMessage("Set Scanner Rts enable");
            m_Diagnostics.cRtsEnable = true;
            clsUtils.Dly(0.5);
            strCmd = "MATCMD?";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2311_RTS_CTS";
                return false;
            }

            //set back
            DisplayMessage("Set 232CTS0");
            strCmd = "232CTS0";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            clsUtils.Dly(3);
            return true;
        }

        private bool TestReadMACAddresses()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            bool b_sts;

            strCmd = "BASLDA";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strRes = m_Diagnostics.GetMenuInfo(strCmd, strRes);
            TestData.UnitInfo.BT_MacAddress = strRes;
            DisplayMessage("Mac Address: " + strRes);

            return true;
        }

        private bool TestConnectUSB()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            bool b_sts;
            bool b_GetUSB;
            string strUSBCom = "";

            DateTime daTo;

            strCmd = "DEFALT;FACTON2;TRMUSB130;";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '.', "TRMUSB130\x06", ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            m_Diagnostics.dPortState = false;
            m_fixctrl.PowerControl(false);
            string[] strPorts1 = System.IO.Ports.SerialPort.GetPortNames();
            clsUtils.Dly(1);
            m_fixctrl.CableSelUSB();
            clsUtils.Dly(1);
            m_fixctrl.PowerControl(true);
            clsUtils.Dly(5);

            b_GetUSB = false;
            daTo = DateTime.Now.AddSeconds(50);
            while (daTo > DateTime.Now && b_GetUSB == false)
            {
                string[] strPorts2 = System.IO.Ports.SerialPort.GetPortNames();
                string portStrings = "";
                foreach (string strName1 in strPorts1)
                {
                    portStrings += strName1;
                }


                foreach (string strName2 in strPorts2)
                {
                    if (portStrings.Contains(strName2) == false)
                    {
                        strUSBCom = strName2.Replace("COM", "");
                        b_GetUSB = true;
                        DisplayMessage("Get USB serial com: " + strName2);
                        break;
                    }
                }

            }

            try
            {
                m_Diagnostics.CommPortNumber = int.Parse(strUSBCom);
            }
            catch(Exception ex)
            {
                TestData.Results.TestFailMessage_Name = "ER2308_USB";
                return false;
            }
            m_Diagnostics.dPortState = true;

            strCmd = "TRMUSB?";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            DisplayMessage("Read interface: " + strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2308_USB";
                return false;
            }

            return true;
        }

        private bool TestCradle()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            bool b_sts;
            System.Windows.Forms.DialogResult dlgRes;
            int itry = 0;
    re_con:
            strCmd = "BASCRD";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strRes = m_Diagnostics.GetMenuInfo(strCmd, strRes);

            if(strRes != "1")
            {
                itry += 1;

                if(itry > 1)
                {
                    TestData.Results.TestFailMessage_Name = "ER5120_BT_CONNECT";
                    return false;
                }
                frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
                frm_Dialog.dgTitle = myResource.GetString("strPutScannerInBase");
                frm_Dialog.dgPrompt = myResource.GetString("strPutScannerInBase");

                dlgRes = frm_Dialog.ShowDialog();
                if (dlgRes == System.Windows.Forms.DialogResult.Cancel)
                {
                    TestData.Results.TestFailMessage_Name = "ER5120_BT_CONNECT";
                    return false;
                }


                goto re_con;
            }




            strCmd = "BASLNK1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER5120_BT_CONNECT";
                return false;
            }
            return true;
        }

        private bool TestConnectBluetooth()
        {
            bool b_ret = false;
            DateTime daTo;
            string strCmd = "";
            string strRes = "";
            string scnMac = "";
            bool b_sts;
RE_BT:
            b_ret = false;
             daTo = DateTime.Now.AddSeconds(30);
             while (daTo > DateTime.Now && b_ret == false)
             {
                 //Send command to check link
                 strCmd = ":*:BT_ADR?";
                 b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);

                 clsUtils.Dly(3);
             }

             if (b_ret == false)
             {
                 frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
                 frm_Dialog.dgTitle = "重试";
                 frm_Dialog.dgPrompt = "重试?";
                 frm_Dialog.ShowDialog();

                 if (frm_Dialog.dgResult == DialogResult.OK)
                 {
                     goto RE_BT;
                 }
                 TestData.Results.TestFailMessage_Name = "ER5120_BT_CONNECT";
                 return false;
             }

            //TestData.UnitInfo.BT_MacAddress
            scnMac = m_Diagnostics.GetMenuInfo(strCmd, strRes);
            DisplayMessage("Check Scanner Mac Address Hard_Link: " + scnMac);

            if (TestData.UnitInfo.BT_MacAddress != scnMac)
            {
                frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
                frm_Dialog.dgTitle = "重试";
                frm_Dialog.dgPrompt = "重试?";
                frm_Dialog.ShowDialog();

                if (frm_Dialog.dgResult == DialogResult.OK)
                {
                    goto RE_BT;
                }
                TestData.Results.TestFailMessage_Name = "ER5120_BT_CONNECT";
                return false;
            }

            //disable hardlink
            strCmd = "BASLNK0";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strCmd = ":*:BT_ADR?";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
                frm_Dialog.dgTitle = "重试";
                frm_Dialog.dgPrompt = "重试?";
                frm_Dialog.ShowDialog();

                if (frm_Dialog.dgResult == DialogResult.OK)
                {
                    goto RE_BT;
                }
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strRes = m_Diagnostics.GetMenuInfo(strCmd, strRes);
            DisplayMessage(" Check Scanner Mac Address RE_Link: " + strRes);

            if(strRes != scnMac)
            {
                frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
                frm_Dialog.dgTitle = "重试";
                frm_Dialog.dgPrompt = "重试?";
                frm_Dialog.ShowDialog();

                if (frm_Dialog.dgResult == DialogResult.OK)
                {
                    goto RE_BT;
                }
                TestData.Results.TestFailMessage_Name = "ER5120_BT_CONNECT";
                return false;
            }

            return true;
        }

        private bool TestBeeper()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string scnMac = "";
            bool b_sts;
            System.Windows.Forms.DialogResult dlgRes;

        BEEP_RETRY:
            //Send command to check link

            if (m_Diagnostics.dPortState == false) m_Diagnostics.dPortState = true;
        clsUtils.Dly(1);
            strCmd = "BEPDUR1000;BEPFQ13000;BEPFQ23000;BEPGRX";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', "BEPGRX\x06", ref strRes);
            if (b_ret == false)
            {
                m_Diagnostics.dPortState = false;
                frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
                frm_Dialog.dgTitle = "重试,请工程师确认";
                frm_Dialog.dgPrompt = "重试?";
                frm_Dialog.ShowDialog();

                if (frm_Dialog.dgResult == DialogResult.OK)
                {
                    goto BEEP_RETRY;
                }
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            frm_Dialog.dgPromptType= MessageBoxButtons.YesNoCancel;
            frm_Dialog.dgTitle = myResource.GetString("strBeep");
            frm_Dialog.dgPrompt = myResource.GetString("strBeep");

            dlgRes = frm_Dialog.ShowDialog();
            if (dlgRes == System.Windows.Forms.DialogResult.Cancel)
            {
                TestData.Results.TestFailMessage_Name = "ER1280_NO_BEEP";
                return false;
            }
            else if (dlgRes == System.Windows.Forms.DialogResult.Retry)
            {
                goto BEEP_RETRY;
            }

            return true;
        }

        private bool TestPager()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string scnMac = "";
            string strTemp;
            bool b_sts;

            //Send command to check link
            strCmd = "PAGCLR";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            frm_Dialog.dgPromptType = MessageBoxButtons.YesNoCancel;
            frm_Dialog.dgTitle = myResource.GetString("strPressPage");
            frm_Dialog.dgPrompt = myResource.GetString("strPressPage");
            frm_Dialog.ShowDialog();

            strCmd = "PAG_RD";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            DisplayMessage(strRes);
            strTemp = strRes.Substring(0, strRes.IndexOf(","));
            strTemp = strTemp.Replace("Times=", "");
            strTemp = strTemp.Trim();
            if (int.Parse(strTemp) < 1)
            {
                TestData.Results.TestFailMessage_Name = "ER4353_PB_ALWAYS_OFF";
                return false;
            }

            return true;

        }

        private bool TestIndicators()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";

            //strCmd = "GRLED1";
            //b_ret = m_Diagnostics.HSM_Send_NoneMenu_Command(strCmd, ref strRes);
            //if (b_ret == false)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
            //    return false;
            //}

            //frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
            //frm_Dialog.dgTitle = myResource.GetString("strGRLEDON");
            //frm_Dialog.dgPrompt = myResource.GetString("strGRLEDON");
            //frm_Dialog.ShowDialog();

            //if(frm_Dialog.dgResult== DialogResult.No)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER1555_GREEN_ON";
            //    return false;
            //}

            //strCmd = "GRLED0";
            //b_ret = m_Diagnostics.HSM_Send_NoneMenu_Command(strCmd, ref strRes);
            //if (b_ret == false)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
            //    return false;
            //}
            frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
            frm_Dialog.dgTitle = myResource.GetString("strGreenLedBlink");
            frm_Dialog.dgPrompt = myResource.GetString("strGreenLedBlink");
            frm_Dialog.ShowDialog();

            if (frm_Dialog.dgResult == DialogResult.No)
            {
                TestData.Results.TestFailMessage_Name = "ER1539_GREEN_OFF";
                return false;
            }

            //strCmd = "ERLED1";
            //b_ret = m_Diagnostics.HSM_Send_NoneMenu_Command(strCmd, ref strRes);
            //if (b_ret == false)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
            //    return false;
            //}
            //frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
            //frm_Dialog.dgTitle = myResource.GetString("strERLEDON");
            //frm_Dialog.dgPrompt = myResource.GetString("strERLEDON");
            //frm_Dialog.ShowDialog();

            //if (frm_Dialog.dgResult == DialogResult.No)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER1554_RED_ON";
            //    return false;
            //}


            //strCmd = "ERLED0";
            //b_ret = m_Diagnostics.HSM_Send_NoneMenu_Command(strCmd, ref strRes);
            //if (b_ret == false)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
            //    return false;
            //}

            //frm_Dialog.dgPromptType = MessageBoxButtons.YesNo;
            //frm_Dialog.dgTitle = myResource.GetString("strERLEDOFF");
            //frm_Dialog.dgPrompt = myResource.GetString("strERLEDOFF");
            //frm_Dialog.ShowDialog();

            //if (frm_Dialog.dgResult == DialogResult.No)
            //{
            //    TestData.Results.TestFailMessage_Name = "ER1538_RED_OFF";
            //    return false;
            //}

            return true;
        }
        private bool TestSetSerialNumber()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string strSetSN;

            strCmd = "PCFUPD1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strCmd = "PCFQRY1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }


            strSetSN = SNBox.Text.Trim();
            strCmd = "SERNUM" + strSetSN;

            b_ret = m_Diagnostics.HSM_Send_Menu_Command('Y', strCmd, "", '\0', '.', ref strRes);
            DisplayMessage("Set Engine Serial Number: " + strSetSN);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            // CHECK
            strCmd = "SERNUM";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('Y', strCmd, "", '?', '!', ref strRes);
            TestData.UnitInfo.str_SN = m_Diagnostics.GetMenuInfo(strCmd, strRes);
            DisplayMessage("Read Engine Serial Number: " + TestData.UnitInfo.str_SN);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            if (strSetSN != TestData.UnitInfo.str_SN)
            {
                TestData.Results.TestFailMessage_Name = "ER4665_SERIAL_GEN";
                return false;
            }
            return true;
        }
        
        private bool TestSetStatus()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string strSetSN;


            strCmd = "PCFQRY1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strCmd = "PCFUPD1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strCmd = "TSTSTA1";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('Y', strCmd, "", '\0', '.', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            return true;
        }
        private bool TestDefault()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string strSetSN;

            strCmd = "DEFALT";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '.', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER1794_RECALL_DEFAULTS";
                return false;
            }

            return true;
        }
        private bool TestAPL()
        {
            bool b_ret = false;
            string strCmd;
            string strRes = "";
            string strtemp;

            strCmd = "SCNAIM0;SCNLED0;EXPMOD0;EXPFEX800";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', "EXPFEX800\x06", ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }

            strCmd = "\x16T\r";
            m_Diagnostics.SendCommand(strCmd, 1, "");

            clsUtils.Dly(1.5);
            strCmd = "\x16U\r";
            m_Diagnostics.SendCommand(strCmd, 1, "");


            strCmd = "IMGAPL";
            b_ret = m_Diagnostics.HSM_Send_Menu_Command('M', strCmd, "", '\0', '!', ref strRes);
            if (b_ret == false)
            {
                TestData.Results.TestFailMessage_Name = "ER2050_NO_COMM";
                return false;
            }
            strtemp = strRes.Substring(0, strRes.IndexOf(strCmd));
            strtemp = strtemp.Substring(strtemp.IndexOf("APL=") + 4);

            strtemp = strtemp.Replace("\r\n", "");
            strtemp = strtemp.Trim();
            TestData.UnitInfo.APL = (int)Convert.ToDouble(strtemp);
            DisplayMessage("APL= " + strtemp);

            return true;
        }


    }
}
