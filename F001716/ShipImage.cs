using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace F001716
{
    public class ShipImage
    {

        private enum CMDSTYLE
        {
            cmdMENU,
            cmdNONMENU,
            cmdRAW
        }

        private enum FILETYPE
        {
            fileBMP,
            fileJPEG
        }

        private struct WORKEROBJECTS_t
        {
            public string imgFn;
            public string imgShip;
            public string imgSnap;
        }
        private WORKEROBJECTS_t m_workerStruct;

        private string m_errMsg = "";
        private bool m_status = true;

        private SerialPort m_serport;
        private serialSettings_t m_settings;
        private frm_Progress m_progress = null;

        private byte[] m_buffer = null;
        private byte[] myBuffer = null;

        private int m_index = 0;
        private int m_imageWidth = 864;
        private int m_imageHeight = 640;
        private int m_nfileSize = 554038;
        private string m_filename;
        private string m_commandShip;
        private string m_commandSnap;
        //private FileStream m_streamWriter = null;

        //comm file
        private int m_startOfFile = 0;
        private int m_endOfFile = 0;

        private bool m_fImageShipped = false;
        private bool m_fBufferReadTimeExpired = false;
        private int m_bufferState = 0;

        //private const int BMP_HEADER = 0x40;
        //private const int PAD = 0x4B0; //0x436

        private const int BMP_HEADER = 0x36; //40 + 14
        private const int BMP_PAD = 0x400; //1024
        private const int PAD = 0x200; //512 pad for mtf response and imgshp command
        //private const int IMAGE_WIDTH = 864;
        //private const int IMAGE_HEIGHT = 640;

        //background worker
        private BackgroundWorker m_bkgdWorker;

        //events
        public delegate void ShipImageCallbackDelegate(bool res, string errm);
        public event ShipImageCallbackDelegate ev_ShipImageCallback;
        public delegate void ShipImageComStatusDelegate(double progress, int stat);
        public static event ShipImageComStatusDelegate ev_ShipImageComStatus;

        public serialSettings_t imgSettings 
        {
            get { return m_settings; }
            set { m_settings = value; }
        }

        public string imgFilename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }

        public ShipImage(SerialPort serport)
        {
            //m_progress = new Progress();
            m_serport = serport;
            m_bkgdWorker = new BackgroundWorker();
            m_bkgdWorker.WorkerSupportsCancellation = true;
            m_bkgdWorker.WorkerReportsProgress = true;

            //events
            m_bkgdWorker.DoWork += new DoWorkEventHandler(m_bkgdWorker_DoWork);
            m_bkgdWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_bkgdWorker_RunWorkerCompleted);
            m_bkgdWorker.ProgressChanged += new ProgressChangedEventHandler(m_bkgdWorker_ProgressChanged);
        }

        ~ShipImage()
        {
            int a = 0;
        }

        public void Init(int imgsizex, int imgsizey)
        {
            m_imageWidth = imgsizex; // 864;
            m_imageHeight = imgsizey; // 640;
            m_nfileSize = m_imageWidth * m_imageHeight + BMP_HEADER + BMP_PAD;
        }
        
        public void StartThread(string cmdSnap, string cmdShip)
        {
            if (m_progress == null)
            {
                m_progress = new frm_Progress();
            }

            m_workerStruct.imgFn = m_filename;
            m_workerStruct.imgSnap = cmdSnap;
            m_workerStruct.imgShip = cmdShip;
            while (m_bkgdWorker.IsBusy)
                System.Windows.Forms.Application.DoEvents();
            m_bkgdWorker.RunWorkerAsync(m_workerStruct);
            //m_bkgdWorker.RunWorkerAsync(fn);
        }

        public void CancelThread()
        {
            //CommClose();
            if (this.m_bkgdWorker != null)
            {
                //if (Me.m_bkgWorker.IsBusy)
                m_status = false;
                this.m_bkgdWorker.CancelAsync();
                //}
            }
        }

        public void HaltThread()
        {
            //CommClose();
            if (this.m_bkgdWorker != null)
                if (this.m_bkgdWorker.IsBusy)
                    this.m_bkgdWorker.CancelAsync();
        }

        public void CommClose()
        {
            if (m_serport.IsOpen)
            {
                m_serport.DiscardInBuffer();
                m_serport.DiscardOutBuffer();
                m_serport.Close();
                while (m_serport.IsOpen == true)
                    System.Windows.Forms.Application.DoEvents();
            }
            //m_bkgdWorker.ReportProgress(0, false);
        }

        private bool CommOpen()
        {
            if (m_serport.IsOpen)
                m_serport.Close();
            try
            {
                m_serport.PortName ="Com"+m_settings.PortNum.ToString();
                m_serport.BaudRate = m_settings.Baud;
                m_serport.DataBits = m_settings.DataBits;
                m_serport.Parity = m_settings.Parity;
                m_serport.StopBits = m_settings.StopBits;
                m_serport.ReadTimeout = 2000;
                m_serport.ReadBufferSize = 32768; //8192; //32768;
                m_serport.Open();
                m_serport.DiscardInBuffer();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Comport Open");
                return false;
            }
            //m_bkgdWorker.ReportProgress(0, true);
            return true;
        }

        private int CommWrite(CMDSTYLE cmdtype, string cmd)
        {
            switch (cmdtype)
            {
                case CMDSTYLE.cmdMENU:
                    cmd = frmMain.SYN + "M" + frmMain.CR + cmd;
                    break;
                case CMDSTYLE.cmdNONMENU:
                case CMDSTYLE.cmdRAW:
                    break;
            }
            m_serport.Write(cmd);

            return 0;
        }

        private int CommFileInit(FILETYPE ftype)
        {
            switch (ftype)
            {
                case FILETYPE.fileBMP:
                    m_filename = m_filename + ".bmp"; //"imgsnap.bmp"
                    break;
                case FILETYPE.fileJPEG:
                    m_filename = m_filename + ".jpg"; //"imgsnap.jpg"
                    break;
            }

            return 0;
        }

        private int CommWriteFile()
        {
            FileStream fs = new FileStream(m_filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            try
            {
                fs.Write(m_buffer, m_startOfFile, m_endOfFile - m_startOfFile);
                //fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Com Write File");
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                File.Copy(m_filename, "imgsnap.bmp", true);
            }

            return 0;
        }

        private void InitVariables()
        {
            m_fImageShipped = false;
            m_fBufferReadTimeExpired = false;
            m_index = 0;
            m_startOfFile = 0;
            m_endOfFile = 0;
            m_bufferState = 0;
            m_status = true;
            m_errMsg = "";
        }

        private void ManipulateBuffer2(int cnt)
        {
            //Static bufferst As Integer
            byte[] temp = new byte[cnt];
            //Array.ConstrainedCopy(m_buffer, m_index - cnt, temp, 0, cnt);
            System.Buffer.BlockCopy(m_buffer, m_index - cnt, temp, 0, cnt);

            string mystring;
            int m, n;
            switch (m_bufferState)
            {
                case 0:  //find start of file
                    //convert to string
                    mystring =clsUtils.ByteArrayToStr(temp);
                    n = mystring.IndexOf("BM6");
                    if (n > 0)
                    {
                        m_startOfFile += n;
                        m_bufferState = 1;
                        Console.WriteLine("StartOfFile: " + m_startOfFile.ToString());
                    }
                    else
                    {
                        m_startOfFile += cnt;
                        Console.WriteLine("StartOfFile: " + m_startOfFile.ToString());
                    }
                    break;

                case 1:  //find end of file
                    //convert to string
                    //Dim m As Integer
                    mystring = clsUtils.ByteArrayToStr(temp);
                    //m = mystring.IndexOf("IMG")
                    //if m > 0 Then Stop
                    //n = mystring.IndexOf("IMGSHP8F2P" + Chr(6) + ".")
                    n = mystring.IndexOf(m_commandShip.ToUpper() + frmMain.ACK + ".");
                    if (n > 0)
                    {
                        m_endOfFile = m_index - cnt + n;// -2 - m_commandSnap.Length; // - 8
                        m_bufferState = 2;
                        m_fImageShipped = true;
                        break;
                    }
                    // in case the image ship command gets fragmented
                    if (mystring.EndsWith(frmMain.ACK + ".") == true)
                    {
                        m = mystring.IndexOf(frmMain.ACK);
                        m_endOfFile = m_index - cnt + m;
                        m_bufferState = 2;
                        m_fImageShipped = true;
                        break;
                    }
                    break;

                default:
                    //m_bufferState = 0
                    break;
            }
        }

        private void ManipulateBuffer(int cnt)
        {
            //Static bufferst As Integer
            //byte[] temp = new byte[cnt];
            //Console.WriteLine("cnt: " + cnt.ToString());
            //Array.ConstrainedCopy(m_buffer, m_index - cnt, temp, 0, cnt);
            //System.Buffer.BlockCopy(m_buffer, m_index - cnt, temp, 0, cnt);

            //string mystring;
            int m, n;
            switch (m_bufferState)
            {
                case 0:  //find start of file
                    for (m = 0; m < m_index - 3; m++)
                    {
                        if (m_buffer[m] == 0x42 && m_buffer[m + 1] == 0x4d && m_buffer[m + 2] == 0x36)
                        {
                            m_startOfFile = m;
                            m_bufferState = 1;
                            Console.WriteLine("StartOfFile: " + m_startOfFile.ToString());
                            break;
                        }
                    }
                    break;

                case 1:  //find end of file
                    if (m_index < m_nfileSize)
                        break;

                    //for (m = m_index - 128; m < m_index; m++)
                    for (m = m_index - 32; m < m_index; m++)
                    {
                        //if (m_buffer[m] == 0x49)
                        //    n = 1;
                        if (m_buffer[m] == 0x49 && m_buffer[m + 1] == 0x4d && m_buffer[m + 2] == 0x47 && m_buffer[m + 3] == 0x52 && m_buffer[m + 4] == 0x41 && m_buffer[m + 5] == 0x57 && m_buffer[m + 6] == 0x6)
                        {
                            m_endOfFile = m;
                            m_bufferState = 2;
                            m_fImageShipped = true;
                            Console.WriteLine("EndOfFile: " + m_endOfFile.ToString());
                            break;
                        }
                    }
                    break;

                default:
                    //m_bufferState = 0
                    break;
            }
        }

        private void ReadBuffer(float WaitTimeSeconds)
        {
            int b;
            long lng_WaitTm;
            long tmr;
            int ctr = 0;
            double a = 0;
            int m = 0, n = 0;
            
            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 8;

            //m_buffer = new byte[IMAGE_WIDTH * IMAGE_HEIGHT + BMP_HEADER + BMP_PAD + PAD];
            m_buffer = new byte[m_imageWidth * m_imageHeight + BMP_HEADER + BMP_PAD + PAD + PAD];
            Array.Clear(m_buffer, 0, m_buffer.Length);

            //byte[] myBuffer = new byte[IMAGE_WIDTH * IMAGE_HEIGHT + BMP_HEADER + BMP_PAD + PAD];
            // insert code here to fill byte array 

            try
            {
                lng_WaitTm = Convert.ToInt64(WaitTimeSeconds * System.TimeSpan.TicksPerSecond);
                tmr = clsUtils.StartTimeInTicks();
                while (clsUtils.ElapseTimeInSeconds(tmr) < WaitTimeSeconds)
                {
                    b = m_serport.BytesToRead;
                    m += b;
                    if (b > 0)
                    {
                        n = m_serport.Read(m_buffer, m_index, b);
                        m_index += n;
                        ManipulateBuffer(n);
                        //else
                        //if (m_index > IMAGE_WIDTH * IMAGE_HEIGHT)
                        //    break;
                    }
                    System.Windows.Forms.Application.DoEvents();
                    //update progress
                    a = ((double)(m_index - m_startOfFile) / m_nfileSize) * 100;
                    //if (ctr == 30000)
                    //    break;
                    m_bkgdWorker.ReportProgress((int)a, ctr);
                    if ((m_fImageShipped == true) & (m_endOfFile - m_startOfFile == m_nfileSize))
                        break;
                    if (m_fBufferReadTimeExpired == true)
                        break;
                    ctr += 1;
                }
            }
            catch (Exception ex)
            {
                m_errMsg = "Error #452: " + ex.Message;
                m_status = false;
            }

            //Console.Write(Utils.ByteArrayToStr(m_buffer));
            //CommWriteFile();

            //IntPtr BufferAddress = Marshal.AllocHGlobal(IMAGE_WIDTH * IMAGE_HEIGHT + BMP_HEADER + PAD);
            //Marshal.Copy(myBuffer, 0, BufferAddress, IMAGE_WIDTH * IMAGE_HEIGHT + BMP_HEADER + PAD);
        }

        private bool StartImageSnap(BackgroundWorker worker, DoWorkEventArgs e)
        {
            string menucmd;
            bool stat = true;
            //FILETYPE filtype;

            // initialize variables
            InitVariables();

            // change all image snap and ship to 921600
            //CommClose();
            //m_serport.BaudRate = 921600;

            menucmd = m_commandSnap + ";" + m_commandShip + "."; //"IMGSNP;IMGSHP8F2P.";
            //filtype = FILETYPE.fileBMP;
            //CommFileInit(filtype);
            if (CommOpen() == true)
            {
                //float wt = (float)(m_nfileSize * 10 / m_settings.Baud) + 5;
                float wt = (float)(20);
                CommWrite(CMDSTYLE.cmdRAW, m_commandSnap + ".");
                clsUtils.Dly(.1);
                CommWrite(CMDSTYLE.cmdMENU, m_commandShip + ".");
                //m_serport.RtsEnable = true;
                //Utils.Dly(0.5);
                ReadBuffer(wt); // (15);
                CommClose();
                //debug file
                if (m_endOfFile > m_startOfFile)
                    CommWriteFile();
                else
                {
                    m_errMsg = "Error #453: ImageSnap, end of file not found";
                    m_status = false;
                    stat = false;
                }
            }
            else
            {
                m_errMsg = "Error #454: Cannot open " + m_serport.PortName;
                m_status = false;
                stat = false;
            }
            // restore baud rate
            //m_serport.BaudRate = m_settings.Baud;
            return stat;
        }



        //****************************************************
        //Purpose : This event gets raised when BkgW(BackgroundWorker).RunWorkerAsync is called 
        //Inputs  : Sender and event arguments that holds the test to run
        //Sets    : event arguments result
        //Returns : raises m_bkgdWorker_RunWorkerCompleted.
        //****************************************************
        private void m_bkgdWorker_DoWork(Object sender, DoWorkEventArgs e)
        {
            //This runs on a seperate thread

            // Get the BackgroundWorker object that raised this event.
            //BackgroundWorker bw = CType(sender, BackgroundWorker);
            BackgroundWorker bw = (BackgroundWorker)sender;

            //Dim arg As String = (e.Argument)

            // start the test thread
            //rsh        e.Result = RunTests(bw, e)
            //m_eventargs = e
            //m_operationCompleted = False

            //Dim pio As WORKEROBJECTS_t
            //pio = CType(arg, WORKEROBJECTS_t)
            WORKEROBJECTS_t wo;
            //wo = CType(e.Argument, WORKEROBJECTS_t);
            wo = (WORKEROBJECTS_t)e.Argument;
            m_filename = wo.imgFn;
            m_commandSnap = wo.imgSnap;
            m_commandShip = wo.imgShip;
            //
            //arg()
            //Select Case CInt(e.Argument)
            //    Case 0
            //e.Result = InitMotion(bw, e)
            //    Case Else
            e.Result = StartImageSnap(bw, e);
            //End Select
            // If the operation was canceled by the user, 
            // set the DoWorkEventArgs.Cancel property to true.
            if (bw.CancellationPending)
                e.Cancel = true;
        }

        //****************************************************
        //Purpose : This event gets raised when m_bkgdWorker(BackgroundWorker).ReportProgress is called 
        //Inputs  : Sender and ProgressChangedEventArgs that holds a TestProcessUpdate_t
        //Sets    : Raises event to update test instructions or test status info
        //Returns : 
        //****************************************************
        private void m_bkgdWorker_ProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int percentage = e.ProgressPercentage;
                int st;

                st = (int)e.UserState;
                if (st == 0)
                {
                    //if (m_progress == null)
                    //    m_progress = new Progress();
                    m_progress.prgFilename = m_filename;
                    m_progress.Show();
                }

                switch (percentage)
                {
                    case 100:
                        {
                            m_progress.Hide();
                            //bool st;
                            //st = CType(e.UserState, bool);
                            //st = (bool)e.UserState;
                            if (ev_ShipImageComStatus != null)
                                ev_ShipImageComStatus(percentage, st);
                            break;
                        }
                    default:
                        {
                            m_progress.UpdateProgress(percentage);
                            //bool st;
                            //st = CType(e.UserState, bool);
                            //st = (bool)e.UserState;
                            if (ev_ShipImageComStatus != null)
                                ev_ShipImageComStatus(percentage, st);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //****************************************************
        //Purpose : This event gets raised when m_bkgdWorker(BackgroundWorker).DoWork is finished 
        //Inputs  : Sender and RunWorkerCompletedEventArgs that holds a TestResultUpdate_t
        //Sets    : Raises event to update test status info
        //Returns : 
        //****************************************************
        private void m_bkgdWorker_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                //RaiseEvent ev_BackgroundUpdatePictureBox(Color.HotPink, m_scanindex)
                //RaiseEvent ev_BackgroundUpdatePictureBox2(3, m_scanindex)
                if (ev_ShipImageCallback != null)
                    ev_ShipImageCallback(false, "Failed: " + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled the 
                // operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                //HaltTest(m_TestList(m_ndxTest))
                //RaiseEvent ev_BackgroundUpdatePictureBox(Color.HotPink, m_scanindex)
                //RaiseEvent ev_BackgroundUpdatePictureBox2(1, m_scanindex)
                if (m_errMsg == "")
                {
                    if (ev_ShipImageCallback != null)
                        ev_ShipImageCallback(false, "Failed: Test Cancelled by operator");
                }
                else
                {
                    if (ev_ShipImageCallback != null)
                        ev_ShipImageCallback(false, m_errMsg);
                }
            }
            else
            {
                // Finally, handle the case where the operation succeeded.
                //Res = CType(e.Result, TESTRESULT_t)

                if (ev_ShipImageCallback != null)
                    ev_ShipImageCallback(m_status, m_errMsg);
            }
            System.Diagnostics.Debug.Print("thread complete");
            if (m_progress != null)
            {
                m_progress.Dispose();
                m_progress = null;
            }
        }


    }
}
