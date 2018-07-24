using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
namespace F001716
{
    
    class HighFrequencyNoise
    {
        //public struct SNRDATA_t
        //{
        //    public double snrAvgPixelLevel;
        //    public double snrAvgPixelLevelExposure;
        //    public double snrNoiseAverage;
        //    public double snrRMSNoise;
        //    public double snrSignalToNoise;
        //    public double snrOpticalRolloff;
        //}

        //public struct DIRTDATA_t
        //{
        //    public bool dcResult;
        //    public int dcDefects;
        //    public int dcMinorBlemish;
        //    public int dcMajorBlemish;
        //    public int dcOutOfSpecBlemish;
        //    public int dcDirtCounter;
        //    //public int dcExposure;
        //}

        public struct CIRCLEDATA_t
        {
            public int[] cdXmajor;
            public int[] cdYmajor;
            public int cdNmajor;
            public int[] cdXminor;
            public int[] cdYminor;
            public int cdNminor;
            public int[] cdXbad;
            public int[] cdYbad;
            public int cdNbad;
        }

        private ShipImage m_shipImage;

        private SerialPort m_serport;
        private serialSettings_t m_settings;
                
        private bool m_status;
        private string m_errmsg;
        private bool m_abort = false;

        //private int m_imageWidth;
        //private int m_imageHeight;
        private int m_exposure;
        private int m_lastExposure = 0;

        //private bool m_fSaveImages;
        //private bool m_fPauseOnImages;
        //private bool m_fImageComplete;
        private int m_nImageComplete;
        private string m_imageFileName;

        //perf spec 500026040
        //RLC--
        //private int DEFECTPIXELS = 30;
        //private int MINORBLEMISH = 3;
        //private int MAJORBLEMISH = 0;
        //private int OUTOFSPECBLEMISH = 0;
        
        //private const double MAXHIFREQUENCYNOISE = 4.5;
        //private const double MINRELATIVECORNERSIGNAL = 50;
        //private const int MINCORNERSIGNAL = 50;
        
        //private const int MAX_STORAGE_BAD_PIXEL = 1024;
        //private const int MAX_STORAGE_DISCONTINUITY = 8192;
        //private const int MAX_STORAGE_MAJOR_BLEMISH = 128;
        //private const int MAX_STORAGE_MINOR_BLEMISH = 128;
        //private const int APL_TOL = 10;
        //--RLC
        private const string CRLF = "\x0d\x0a";

        //events
        public delegate void HighFreqNoiseCallback(bool res, string errm);
        public static event HighFreqNoiseCallback ev_HighFreqNoiseCallback;
        public delegate void HighFreqNoiseStatus(string msg);
        public static event HighFreqNoiseStatus ev_HighFreqNoiseStatus;
        public delegate void HighFreqNoiseGui(string fn, ref byte[] bigstr, CIRCLEDATA_t dt);
        public static event HighFreqNoiseGui ev_HighFreqNoiseGui;

     
        //public HighFrequencyNoise(ImageTests it, ShipImage si)
        //public HighFrequencyNoise(serialSettings_t settings)
        public HighFrequencyNoise()
        {
            m_serport = new SerialPort();
            //m_settings = settings;
            m_shipImage = new ShipImage(m_serport); // si;
        }

        ~HighFrequencyNoise()
        {

        }

        public string p_Err
        {
            get { return m_errmsg; }
        
        }


        public serialSettings_t p_Settings
        {
            set 
            {
                m_settings = value;
            }
        }


        public void HaltSNR()
        {
            m_abort = true;
            m_errmsg = "Error #332:User Halted Test";
        }


        private void CopyFailedImage()
        {
            bool CopyImage = false;
            //bool cp = false;
       
            if (CopyImage == true)
            {
                DateTime stamp;
                string seqnum = "";
                string woy = "";
                string fd = System.Windows.Forms.Application.StartupPath + "\\Failed_Images\\";
                string[] d = m_imageFileName.Split("\\".ToCharArray());

                try
                {
                    stamp = System.DateTime.Now;
                    // fff is the same as milliseconds
                    seqnum = stamp.ToString("HHmmssfff");
                    woy = WeekOfYear().ToString("00");

                    if (System.IO.Directory.Exists(fd) == false)
                    {
                        System.IO.Directory.CreateDirectory(fd);
                    }
                    fd = fd + "Week_" + woy + "\\";
                    if (System.IO.Directory.Exists(fd) == false)
                    {
                        System.IO.Directory.CreateDirectory(fd);
                    }
                    System.IO.File.Copy(m_imageFileName, fd + "i" + seqnum + d[d.GetUpperBound(0)], true);

                }
                catch (Exception ex)
                {
                    //if (sw != null) sw.Close();
                }
            }
        }

     
        //private bool GetImage(string imgsnp, string imgshp, string fn)
        public bool GetImage(string imgsnp, string imgshp)
        {
            bool res;
            m_nImageComplete = -1;
            DateTime myTimer;
            //add in the ShipImage event handler
            m_shipImage.ev_ShipImageCallback += new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);

            m_shipImage.imgFilename = m_imageFileName;
            m_shipImage.imgSettings = m_settings;
            m_shipImage.Init(640, 480);
            m_shipImage.StartThread(imgsnp, imgshp);
            //while (m_fImageComplete == false)
            myTimer = DateTime.Now.AddSeconds(25);
            while (m_nImageComplete == -1 && m_abort == false)
            {
                System.Windows.Forms.Application.DoEvents();
                if (m_abort == true)
                    m_shipImage.CancelThread();
                //if (myTimer < DateTime.Now)
                //{
                //    MessageBox.Show("Save image time out");
                //    m_nImageComplete = 1;
                //    break;
                //}
            }
            m_shipImage.HaltThread();
            //m_fImageComplete = false;
            m_shipImage.CommClose();
            //remove the ShipImage event handler
            m_shipImage.ev_ShipImageCallback -= new ShipImage.ShipImageCallbackDelegate(OnShipImageCallback);
            
            //RLC--
            //LensFocus.LensFocusImageDelegate ic = LensFocus.LensFocusImageCallback;
            //if (ic != null) LensFocus.LensFocusImageCallback(m_imageFileName);
            //--RLC
            switch (m_nImageComplete)
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

        private void InitVariables()
        {
            m_status = true;
            m_errmsg = "";
            m_abort = false;
            //m_fImageComplete = false;
            m_nImageComplete = -1;
        }


        private bool CreateDirtImage(string fn,string ImageCirclefile, CIRCLEDATA_t cd,int offsetX, int offsetY)
        {

            //DisplayCleanResults();
            //Bitmap b = new Bitmap(fn);
            //Image b = Image.FromFile(fn);

            //MemoryStream newstr = new MemoryStream(bb);
            //Image imag = Image.FromStream(newstr);
            //Bitmap b = new Bitmap(imag);
            //Graphics g = Graphics.FromImage(b);
            try
            {
                int width;
                int height;
                System.Drawing.Bitmap b1 = new System.Drawing.Bitmap(fn);

                width = b1.Size.Width;
                height = b1.Size.Height;
                System.Drawing.Bitmap b2 = new System.Drawing.Bitmap(b1);
                System.Drawing.Graphics g2 = System.Drawing.Graphics.FromImage(b2);
                g2.DrawImage(b1, 0, 0, width, height);

                //int offsetX = 30; //100;
                //int offsetY = 30; //100;

                int i;
               
                //RLC--
                //for (i = 0; i < cd.cdNmajor; i++)
                //    g2.DrawArc(System.Drawing.Pens.Blue, cd.cdXmajor[i] + offsetX - 10, cd.cdYmajor[i] + offsetY - 10, 20, 20, 0, 360);

                //for (i = 0; i < cd.cdNminor; i++)
                //    g2.DrawArc(System.Drawing.Pens.BlueViolet, cd.cdXminor[i] + offsetX - 10, cd.cdYminor[i] + offsetY - 10, 20, 20, 0, 360);

                //for (i = 0; i < cd.cdNbad; i++)
                //    g2.DrawArc(System.Drawing.Pens.Yellow, cd.cdXbad[i] + offsetX - 10, cd.cdYbad[i] + offsetY - 10, 20, 20, 0, 360);
                //--RLC

                for (i = 0; i < cd.cdNmajor; i++)
                    g2.DrawArc(System.Drawing.Pens.Blue, cd.cdXmajor[i] + offsetX - 10, cd.cdYmajor[i] + offsetY, 20, 20, 0, 360);

                for (i = 0; i < cd.cdNminor; i++)
                    g2.DrawArc(System.Drawing.Pens.BlueViolet, cd.cdXminor[i] + offsetX - 10, cd.cdYminor[i] + offsetY, 20, 20, 0, 360);

                for (i = 0; i < cd.cdNbad; i++)
                    g2.DrawArc(System.Drawing.Pens.Yellow, cd.cdXbad[i] + offsetX - 10, cd.cdYbad[i] + offsetY, 20, 20, 0, 360);

                //this.pctImage.Image = b2;
                b2.Save(ImageCirclefile, System.Drawing.Imaging.ImageFormat.Bmp);
                b1.Dispose();
                b2.Dispose();
                g2.Dispose();
            }
            catch (Exception ex) 
            {
                m_status = false;
                m_errmsg = "Error #334: Failed Dirt Image file creation: " + ex.Message ;
                return false;
            }
            return true;

        }


        private int WeekOfYear()
        {
            // Gets the Calendar instance associated with a CultureInfo.
            System.Globalization.CultureInfo myCI = new System.Globalization.CultureInfo("en-US");
            System.Globalization.Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            System.Globalization.CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            // Displays the number of the current week relative to the beginning of the year.
            //Console.WriteLine("The CalendarWeekRule used for the en-US culture is {0}.", myCWR);
            //Console.WriteLine("The FirstDayOfWeek used for the en-US culture is {0}.", myFirstDOW);
            //Console.WriteLine("Therefore, the current week is Week {0} of the current year.", myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW));

            // Displays the total number of weeks in the current year.
            //System.DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            //Console.WriteLine("There are {0} weeks in the current year ({1}).", myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW), LastDay.Year);

            return myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
        }

        private void OnShipImageCallback(bool res, string errm)
        {
            //m_fImageComplete = true;
            if (res)
                m_nImageComplete = 0;
            else
            {
                m_nImageComplete = 1;
                m_errmsg = errm;
            }
        }
    }
}
