using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices; // DllImport
using System.Diagnostics;
using System.Windows.Forms;
namespace F001716
{
    class clsDiagnostics
    {

        #region Variables
        public const string ACK = "\x06";
        public const string SYN = "\x16";
        public const string CR = "\x0d";
        public const string DC1 = "\x11";
        private const string LF = "\x0a";
        private const string CRLF = "\x0d\x0a";
        private int m_iIntercharacterDelay = 10;

        private int m_iCommPortNumber = 1;
        private long m_lCommBaudRate = 9600;
        private System.IO.Ports.Parity m_pCommParity = System.IO.Ports.Parity.None;
        private int m_iCommDataBits = 8;
        private System.IO.Ports.StopBits m_sCommStopBits = System.IO.Ports.StopBits.One;

        public System.IO.Ports.SerialPort m_serport = null;
        //private System.IO.Ports.SerialPort Serial = null;

        private bool m_bShowErrors = true;
        private string mstr_error;
        
       // private System.Windows.Forms.RichTextBox m_tTerminalWindow;

        static readonly string[] LowNames = 
        {
        "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", 
        "BS", "HT", "LF", "VT", "FF", "CR", "SO", "SI",
        "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB",
        "CAN", "EM", "SUB", "ESC", "FS", "GS", "RS", "US"
        };

        //private SerialPort m_serport = null;
        private serialSettings_t m_settings;
        // private m_Utils Utils;

        public const string BEL = "\x07";

        #endregion


        #region Constructor

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>

        public clsDiagnostics()
        {
            m_serport = new System.IO.Ports.SerialPort();
           // m_tTerminalWindow = new System.Windows.Forms.RichTextBox();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>
        public clsDiagnostics(int SerialPortNumber, long SerialBaudRate)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;

            m_serport = new System.IO.Ports.SerialPort();
            //m_tTerminalWindow = new System.Windows.Forms.RichTextBox();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>
        /// <param name="SerialDataBits"></param>
        /// <param name="SerialParity"></param>
        /// <param name="SerialStopBits"></param>
        public clsDiagnostics(int SerialPortNumber, long SerialBaudRate, System.IO.Ports.Parity SerialParity, int SerialDataBits, System.IO.Ports.StopBits SerialStopBits)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;
            m_pCommParity = SerialParity;
            m_iCommDataBits = SerialDataBits;
            m_sCommStopBits = SerialStopBits;

            m_serport = new System.IO.Ports.SerialPort();
           // m_tTerminalWindow = new System.Windows.Forms.RichTextBox();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>
        /// <param name="SerialDataBits"></param>
        /// <param name="SerialParity"></param>
        /// <param name="SerialStopBits"></param>
        /// <param name="IntercharacterDelay"></param>
        public clsDiagnostics(int SerialPortNumber, long SerialBaudRate, System.IO.Ports.Parity SerialParity, int SerialDataBits, System.IO.Ports.StopBits SerialStopBits, int IntercharacterDelay)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;
            m_pCommParity = SerialParity;
            m_iCommDataBits = SerialDataBits;
            m_sCommStopBits = SerialStopBits;

            m_iIntercharacterDelay = IntercharacterDelay;

            m_serport = new System.IO.Ports.SerialPort();
           // m_tTerminalWindow = new System.Windows.Forms.RichTextBox();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>
        /// <param name="SerialDataBits"></param>
        /// <param name="SerialParity"></param>
        /// <param name="SerialStopBits"></param>
        /// <param name="IntercharacterDelay"></param>
        public clsDiagnostics(int SerialPortNumber, long SerialBaudRate, System.IO.Ports.Parity SerialParity, int SerialDataBits, System.IO.Ports.StopBits SerialStopBits, int IntercharacterDelay, System.Windows.Forms.RichTextBox TerminalWindow)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;
            m_pCommParity = SerialParity;
            m_iCommDataBits = SerialDataBits;
            m_sCommStopBits = SerialStopBits;

            m_iIntercharacterDelay = IntercharacterDelay;

            m_serport = new System.IO.Ports.SerialPort();
           // m_tTerminalWindow = TerminalWindow;
        }

        #endregion


        #region Properties

        public serialSettings_t dSettings
        {
            get { return m_settings; }
            set
            {
                m_settings = value;
                m_iCommPortNumber=value.PortNum;
                m_lCommBaudRate = value.Baud;
                m_pCommParity = value.Parity;
                m_iCommDataBits = value.DataBits;
                m_sCommStopBits = value.StopBits;
            }
        }

        public bool dPortState
        {
            get { return m_serport.IsOpen; }
            set
            {
                if (value == true)
                {
                    if (m_serport.IsOpen == true) Serial_END(m_serport);
                    //SetPortProperties();
                    Serial_INIT(m_serport);
                    if (m_serport.IsOpen == false) Serial_INIT(m_serport);
                }
                else
                {
                    if (m_serport.IsOpen == true)
                        Serial_END(m_serport);
                }
            }
        }
 

        public bool cCtsState
        {
            get
            {
                return m_serport.CtsHolding;
            }
        }

        public bool cRtsEnable
        {
            set
            {
                m_serport.RtsEnable = value;
            }
        }

        /// <summary>
        /// Get or Set the ability to show communication error messages
        /// </summary>
        public bool ShowErrors
        {
            get
            {
                return m_bShowErrors;
            }
            set
            {
                m_bShowErrors = value;
            }
        }

        /// <summary>
        /// Get error messages
        /// </summary>
        public string err
        {
            get
            {
                return mstr_error;
            }
        }

        /// <summary>
        /// Get or Set the Comm Port Number
        /// </summary>
        public int CommPortNumber
        {
            get
            {
                return m_iCommPortNumber;
            }
            set
            {
                m_iCommPortNumber = value;
            }
        }

        /// <summary>
        /// Get or Set the Comm Port Baud Rate
        /// </summary>
        public long CommBaudRate
        {
            get
            {
                return m_lCommBaudRate;
            }
            set
            {
                m_lCommBaudRate = value;
            }
        }

        /// <summary>
        /// Get or Set the Comm Parity
        /// </summary>
        public System.IO.Ports.Parity CommParity
        {
            get
            {
                return m_pCommParity;
            }
            set
            {
                m_pCommParity = value;
            }
        }

        /// <summary>
        /// Get or Set the Comm DataBits
        /// </summary>
        public int CommDataBits
        {
            get
            {
                return m_iCommDataBits;
            }
            set
            {
                m_iCommDataBits = value;
            }
        }

        /// <summary>
        /// Get or Set the Comm Stop Bits
        /// </summary>
        public System.IO.Ports.StopBits CommStopBits
        {
            get
            {
                return m_sCommStopBits;
            }
            set
            {
                m_sCommStopBits = value;
            }
        }

        /// <summary>
        /// Get or Set the Intercharacter Delay
        /// </summary>
        public int IntercharacterDelay
        {
            get
            {
                return m_iIntercharacterDelay;
            }
            set
            {
                m_iIntercharacterDelay = value;
            }
        }

        /// <summary>
        /// Get or Set the Terminal Window to control
        /// </summary>
        //public System.Windows.Forms.RichTextBox TerminalWindow
        //{
        //    get
        //    {
        //        return m_tTerminalWindow;
        //    }
        //    set
        //    {
        //        m_tTerminalWindow = value;
        //    }
        //}

        #endregion


        #region Public Functions

        #region General

        public bool ReadSerialNumberAndSoftwareNumber(ref string SerialNumber, ref string SoftwareNumber)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check = true;
            int value;
            string str_InBuffer;

            //Initialize Variables
            str_InBuffer = "";
            SerialNumber = "";
            SoftwareNumber = "";

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            //Send 999999
            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            Serial_FLUSH(m_serport);

            //Send 999986 to get serial # and software number
            if (Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999986" + "\x03");

                //Read the serial # in
                while (true)
                {
                    value = Serial_Wait_For_Char(m_serport, 2000);
                    if (value < 0 || value == 21)
                    {
                        Check = false;
                        str_InBuffer = "";
                        break;
                    }
                    else if (value == 6)
                    {
                        Check = true;
                        break;
                    }
                    else
                    {
                        //This means it's actually something to read
                        str_InBuffer = str_InBuffer + Convert.ToChar(value);
                    }
                }
            }

            if (Check)
            {
                SerialNumber = str_InBuffer.Substring(0, str_InBuffer.IndexOf(","));
                SoftwareNumber = str_InBuffer.Substring(str_InBuffer.IndexOf(",") + 1);
            }

            DelayMS(100);

            //Serial_END(Serial);

            return Check;
        }

        public bool EmbedSerialNumber(string serialnumber, int timeout)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            int addr_h;
            int addr_l;
            int j;
            string send;
            byte new_character;
            string str_Data;
            bool Check=true;

            addr_h = 2;
            addr_l = 5;

            //Invalid Serial #
            if (serialnumber.Length != 10 || serialnumber == "0000000000")
                return false;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            //Make sure we aren't in diagnostics
            Serial_FLUSH(m_serport);
            Serial_Write_String(m_serport, "x");
            Check = Serial_Wait_For_ACK(m_serport, 250);
            Serial_Write_String(m_serport, "\x04");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Check to see if it is an imager
            Serial_FLUSH(m_serport);
            Serial_Write_String(m_serport, "\x13");
            Check = Serial_Wait_For_String(m_serport, "DIAG>", 300);
            //If it is an imager then send the quit command
            if (Check)            
                Serial_Write_String(m_serport, "quit\x0D");

            System.Threading.Thread.Sleep(timeout);

            //Enter Programming Mode
            Serial_FLUSH(m_serport);
            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //if (Check)
            //{
            //    Serial_FLUSH(m_serport);
            //    Serial_Write_String(m_serport, "\x02" + "999973" + "\x03");
            //    Check = Serial_Wait_For_ACK(m_serport, 1000);
            //}

            if (Check)
            {
                //Send Serial #
                for (j = 0; j <= 9; j++)
                {
                    new_character = (byte)serialnumber[j];
                    str_Data = new_character.ToString("000");

                    send = "\x02" + "82" + (char)(addr_h + 48) + (char)(addr_l + 48) + str_Data[0] + str_Data[1] + str_Data[2] + "0" + "\x03";
                    Serial_FLUSH(m_serport);
                    Serial_Write_String(m_serport, send);
                    Check = Serial_Wait_For_ACK(m_serport, 2000);

                    if (!Check)
                    {
                        Serial_END(m_serport);
                        return Check;
                    }

                    addr_l = addr_l + 1;
                    if (j == 4)
                    {
                        addr_h = 3;
                        addr_l = 0;
                    }
                }

                //Exit Programming Mode
                Serial_FLUSH(m_serport);
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool EmbedSerialNumber(string serialnumber)
        {
            return EmbedSerialNumber(serialnumber, 10);
        }

        public bool EnterDiagnosticMode()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(m_serport);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            //First see if it is already in diagnostics
            Serial_Write_String(m_serport, "\x13");
            Check = Serial_Wait_For_String(m_serport, "DIAG>", 300);

            if (!Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);

                Serial_FLUSH(m_serport);

                if (Check)
                {
                    Serial_Write_String(m_serport, "\x02" + "998017" + "\x03");
                    Check = Serial_Wait_For_String(m_serport, "DIAG>", 3500);
                }
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool EnterTestMode()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            //First see if it is already in diagnostics
            Serial_Write_String(m_serport, "x");
            Check = Serial_Wait_For_ACK(m_serport, 100);
            if (!Check)
            {
                Serial_Write_String(m_serport, "D0");
                Check = Serial_Wait_For_ACK(m_serport, 250);

                //If it Acknowledged then it is already in test mode.  Get it back to the current state.
                if (Check)
                {
                    Serial_Write_String(m_serport, "x");
                    Check = Serial_Wait_For_ACK(m_serport, 100);
                }
            }

            if (!Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);

                Serial_FLUSH(m_serport);

                if (Check)
                {
                    Serial_Write_String(m_serport, "\x02" + "998017" + "\x03");
                    Check = Serial_Wait_For_ACK(m_serport, 1000);
                }
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool GlobalDefault()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            //Enter Programming Mode
            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            Serial_FLUSH(m_serport);

            if (Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999980" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            Serial_FLUSH(m_serport);

            //Exit Programming Mode
            if (Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool RecallDefault()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            //Enter Programming Mode
            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            Serial_FLUSH(m_serport);

            if (Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999998" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            Serial_FLUSH(m_serport);

            //Exit Programming Mode
            if (Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool ExitDiagnosticMode()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            //First see if it is already in diagnostics
            Serial_Write_String(m_serport, "\x13");
            Check = Serial_Wait_For_String(m_serport, "DIAG>", 300);

            Serial_Write_String(m_serport, "quit\x0D");
            DelayMS(1000);

            Serial_FLUSH(m_serport);

            //Serial_END(Serial);

            return true;
        }

        public bool ExitTestMode()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "x");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            Serial_Write_String(m_serport, "\x04");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return true;
        }

        public bool ExitTestMode(ref string SerialNumber, ref string SoftwareNumber)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            int value;
            int x;
            string temp = "";

            //Initialize Variables
            SerialNumber = "";
            SoftwareNumber = "";

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;
            Check = true;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "x");
            Check = Serial_Wait_For_ACK(m_serport, 500);

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "\x04");

            //Read all of the data in
            while (true)
            {
                value = Serial_Wait_For_Char(m_serport, 1000);
                if (value < 0 || value == 21)
                {
                    Check = false;
                    break;
                }
                else if (value == 6)
                {
                    Check = true;
                    break;
                }
                else
                {
                    //This means it's actually something to read
                    temp = temp + (char)value;
                }
            }

            //Parse the data
            if (Check)
            {
                for (x = 0; x <= 19; x++)
                {
                    SerialNumber = SerialNumber + (char)(Convert.ToInt32(temp.Substring(x, 2), 16));
                    x++;
                }

                //SoftwareNumber = temp.Substring(21, 5);
                SoftwareNumber = temp.Substring(21);
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool ExitTestMode2(ref string SerialNumber, ref string SoftwareNumber)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            int value;

            //Initialize Variables
            SerialNumber = "";
            SoftwareNumber = "";

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "x");
            Check = Serial_Wait_For_ACK(m_serport, 500);

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "\x04");

            DelayMS(3000);

            //Send 999999
            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            Serial_FLUSH(m_serport);          

            //Send 999969 to get serial #
            if (Check)
            {
                Serial_Write_String(m_serport, "\x02" + "999969" + "\x03");

                //Read the serial # in
                while (true)
                {
                    value = Serial_Wait_For_Char(m_serport, 1000);
                    if (value < 0 || value == 21)
                    {
                        Check = false;
                        SerialNumber = "";
                        break;
                    }
                    else if (value == 6)
                    {
                        Check = true;
                        break;
                    }
                    else
                    {                        
                        //This means it's actually something to read
                        SerialNumber = SerialNumber + (char)value;

                        if (SerialNumber.Length >= 10) break;
                    }
                }             
            }
            

            //Send 999965 to get software #
            if (Check)
            {
                DelayMS(100);
                Serial_FLUSH(m_serport);

                Serial_Write_String(m_serport, "\x02" + "999965" + "\x03");

                //Read the software number in
                //Read the serial # in
                while (true)
                {
                    value = Serial_Wait_For_Char(m_serport, 1000);
                    if (value < 0)
                    {
                        if (SoftwareNumber == "")
                            Check = false;
                        else
                            Check = true;
                        break;
                    }
                    else if (value == 21)
                    {
                        Check = false;
                        SoftwareNumber = "";
                        break;
                    }
                    else if (value == 6)
                    {
                        Check = true;
                        break;
                    }
                    else
                    {                        
                        //This means it's actually something to read
                        SoftwareNumber = SoftwareNumber + (char)value;

                        //if (SoftwareNumber.Length >= 5) break;
                    }
                }
            }

            if (Check)
            {
                SoftwareNumber = SoftwareNumber.Trim();
            }

            //Send 999999
            DelayMS(100);
            Serial_FLUSH(m_serport);
            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 3000);

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region Metrologic Laser Based Products

        #region DAC

        public bool DAC_OpenChannel(string myChannel)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;
            Check = true;

            Serial_FLUSH(m_serport);

            //Send Dac Commands
            Serial_Write_String(m_serport, myChannel);
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_OpenChannel(string myChannel, int timeout)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            if (timeout < 0) timeout = 0;

            //Send Dac Commands
            Serial_Write_String(m_serport, myChannel);
            Check = Serial_Wait_For_ACK(m_serport, timeout);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_CloseChannel()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "x");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_Get(ref int i_Dac)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check = true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return 0;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "t");
            i_Dac = Serial_Wait_For_Char(m_serport, 500);
            Check = Serial_Wait_For_ACK(m_serport, 200);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_Set(int value)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "l");
            Serial_Write_Byte(m_serport, (byte)value);
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_Set2(int value)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "l");
            Serial_Write_String(m_serport, value.ToString("X2"));
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_Save()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "s");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_Increment()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "u");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool DAC_Decrement()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "d");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region IR

        public bool IR_Enable(bool Enable)
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            if(Enable)
                Serial_Write_String(Serial, "I");
            else
                Serial_Write_String(Serial, "i");

            //Serial_END(Serial);

            return Check;
        }

        public int IR_ObjectDetect()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            //bool Check=true;
            int value;

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "o");
            value = Serial_Wait_For_Char(m_serport, 250);

            //Serial_END(Serial);

            return value;
        }

        public bool IR_Far()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "f");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool IR_Near()
        {
         //  System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "n");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool IR_Increment()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "i");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool IR_Decrement()
        {
           // System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=false;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "d");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool IR_Increment16()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "I");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool IR_Decrement16()
        {
           // System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "D");
            Check = Serial_Wait_For_ACK(m_serport, 250);

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region Beeper

        public bool Beeper_Enable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check = true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_Write_String(m_serport, BEL); //Ctl + G

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region VLD

        public bool VLD_Enable(bool Enable)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            if (Enable)
                Serial_Write_String(m_serport, "L");
            else
                Serial_Write_String(m_serport, "l");

            //Serial_END(Serial);

            return Check;
        }

        public bool VLD_Enable(string enable_command, bool Enable)
        {
         //   System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            Check = Serial_INIT(m_serport);
            if (!Check) return false;

            if (Enable)
                Serial_Write_String(m_serport, enable_command);
            else
                Serial_Write_String(m_serport, "l");

            Serial_END(m_serport);

            return Check;
        }

        public double VLD_CurrentRead(string myChannel, double a2ddivider)
        {
            double VLDCurrent = 0;
            int Current;
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();

            System.DateTime myTime = new DateTime();
            string scanner_return;

            //Check = Serial_INIT(Serial);
            //if (!Check) return 0;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, myChannel);

            //Read the VLD Current
            myTime = System.DateTime.Now.AddMilliseconds(1000);

            while (true)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    DelayMS(100);
                    scanner_return = m_serport.ReadExisting();

                    //Put this error handling so it doesn't ever crash
                    try
                    {
                        Current = Convert.ToInt32(scanner_return.Substring(scanner_return.IndexOf(":") + 1, 6), 16);
                    }
                    catch (Exception ex)
                    {
                        //clsGlobals.log.Error("", ex);
                        mstr_error = "Error: " + "\n" + ex.Message;
                        VLDCurrent = -1;
                        break;
                    }

                    VLDCurrent = Current / a2ddivider;
                    break;
                }

                if (myTime < System.DateTime.Now) { VLDCurrent = -1; break; }
            }

            //Serial_END(Serial);

            return Math.Round(VLDCurrent, 2);
        }

        #endregion

        #region Flipper

        public bool Flipper_Enable(bool Enable)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            if (Enable)
                Serial_Write_String(m_serport, "M");
            else
                Serial_Write_String(m_serport, "m");

            //Serial_END(Serial);

            return Check;
        }

        public int Flipper_PWRead()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();

            System.DateTime myTime = new DateTime();
            string scanner_return;
            int PW;

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "P");

            myTime = System.DateTime.Now.AddMilliseconds(1500);
            while (true)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    DelayMS(100);
                    scanner_return = m_serport.ReadExisting();

                    //Put this error handling so it doesn't ever crash
                    try
                    {
                        PW = Convert.ToInt32(scanner_return.Substring(scanner_return.IndexOf("AVG=")+4, 5));
                    }
                    catch (Exception ex)
                    {
                        //clsGlobals.log.Error("", ex);
                        mstr_error = "Error: " + "\n" + ex.Message;
                        PW = -1;
                        break;
                    }
                    break;
                }

                if (myTime < System.DateTime.Now) { PW = -1; break; }
            }

            //Serial_END(Serial);

            return PW;
        }

        #endregion

        #region Motor

        public bool Motor_Enable(bool Enable)
        {
          //  System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            if (Enable)
                Serial_Write_String(m_serport, "M");
            else
                Serial_Write_String(m_serport, "m");

            //Serial_END(Serial);

            return Check;
        }

        public bool Motor_Enable(string enable_command, bool Enable)
        {
           // System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            if (Enable)
                Serial_Write_String(m_serport, enable_command);
            else
                Serial_Write_String(m_serport, "m");

            //Serial_END(Serial);

            return Check;
        }

        public bool Motor_SpeedRead2(ref int speed1, ref int speed2)
        {
         //   System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();

            System.DateTime myTime = new DateTime();
            int Dac = -1;
            string temp = "";
            int count = 0;
            int value = 0;
            int flagGotIt = 0;

            speed1 = -1;
            speed2 = -1;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "P");

            myTime = System.DateTime.Now.AddMilliseconds(2500);
            while (true)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    value = m_serport.ReadChar();
           
                    //It must be a digit to count
                    if (value >= 48 && value <= 57)
                    {
                        temp = temp + (char)value;
                        flagGotIt++;
                    }
                    else if (count == 0 && flagGotIt > 0)
                    {
                        count = 1;
                        Dac = Convert.ToInt32(temp);
                        temp = "";
                        flagGotIt = 0;
                    }
                    else if (count == 1 && flagGotIt > 0)
                    {
                        count = 2;
                        speed1 = Convert.ToInt32(temp);
                        temp = "";
                        flagGotIt = 0;
                    }
                    else if (count == 2 && flagGotIt > 0)
                    {
                        count = 3;
                        Dac = Convert.ToInt32(temp);
                        temp = "";
                        flagGotIt = 0;
                    }
                    else if (count == 3 && flagGotIt > 0)
                    {
                        count = 4;
                        speed2 = Convert.ToInt32(temp);
                        temp = "";
                        flagGotIt = 0;
                        break;
                    }
                }

                if (myTime < System.DateTime.Now) { speed1 = -1; speed2 = -1; break; }
            }

            //Serial_END(Serial);

            //If it failed return false
            if (speed1 == -1 && speed2 == -1)
                return false;

            return true;
        }

        public int Motor_SpeedRead()
        {
           // System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            //bool Check;
            System.DateTime myTime = new DateTime();
            int Speed = -1;
            string strspeed = "";
            int flagGotIt = 0;
            int value = 0;

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "P");

            myTime = System.DateTime.Now.AddMilliseconds(2500);
            while (true)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    value = m_serport.ReadChar();

                    //It must be a digit to count
                    if (value >= 48 && value <= 57)
                    {
                        strspeed = strspeed + (char)value;
                        flagGotIt++;
                    }
                    else if (flagGotIt > 0)
                    {
                        Speed = Convert.ToInt32(strspeed);
                        break;
                    }
                }

                if (myTime < System.DateTime.Now) { Speed = -1; break; }
            }

            //Serial_END(Serial);

            return Speed;
        }

        #endregion

        #region LED

        public bool LED_Enable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check = true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "E");

            //Serial_END(Serial);

            return Check;
        }

        public bool LED_Disable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "0");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool LED1_Enable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "1");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool LED2_Enable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "2");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool LED3_Enable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "3");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        public bool LED4_Enable()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "4");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region Unit ID

        public int Get_UnitID()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            int value;
            //bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "u");
            value = Serial_Wait_For_Char(m_serport, 250);

            //Serial_END(Serial);            

            return value;
        }

        public bool Set_UnitID(int unitid)
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "U");
            Serial_Write_Byte(m_serport, (byte)unitid);
            Check = Serial_Wait_For_ACK(m_serport, 2000);

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region Single Line

        public bool EnterSingleLineMode()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Get it out of test mode
            ExitTestMode();

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);           

            Serial_Write_String(Serial, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(Serial, 1000);

            Serial_FLUSH(Serial);

            if (Check)
                Serial_Write_String(Serial, "\x02" + "998047" + "\x03");

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Save()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "s");
            Check = Serial_Wait_For_ACK(Serial, 1000);

            if (Check)
            {
                Serial_Write_String(Serial, "S");
                Check = Serial_Wait_For_ACK(Serial, 1000);
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Start_Recall()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "R");
            Check = Serial_Wait_For_ACK(Serial, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Stop_Recall()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "r");
            Check = Serial_Wait_For_ACK(Serial, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Start_Increment()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "I");
            Check = Serial_Wait_For_ACK(Serial, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Start_Decrement()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "D");
            Check = Serial_Wait_For_ACK(Serial, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Stop_Increment()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "i");
            Check = Serial_Wait_For_ACK(Serial, 250);

            //Serial_END(Serial);

            return Check;
        }

        public bool SingleLine_Stop_Decrement()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            Serial_Write_String(Serial, "d");
            Check = Serial_Wait_For_ACK(Serial, 250);

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #region Scan

        public bool EnterScanCountMode()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            if (Check)
            {
                Serial_FLUSH(m_serport);
                Serial_Write_String(m_serport, "\x02" + "999980" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            if (Check)
            {
                Serial_FLUSH(m_serport);
                Serial_Write_String(m_serport, "\x02" + "118016" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            if (Check)
            {
                Serial_FLUSH(m_serport);
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            //Serial_END(Serial);

            return Check;
        }

        public bool ExitScanCountMode()
        {
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
            Check = Serial_Wait_For_ACK(m_serport, 1000);

            if (Check)
            {
                Serial_FLUSH(m_serport);
                Serial_Write_String(m_serport, "\x02" + "118006" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            if (Check)
            {
                Serial_FLUSH(m_serport);
                Serial_Write_String(m_serport, "\x02" + "999999" + "\x03");
                Check = Serial_Wait_For_ACK(m_serport, 1000);
            }

            //Serial_END(Serial);

            return Check;
        }

        #endregion

        #endregion

        #region Metrologic Image Based Products

        #region Upload Settings

        public string Imager_Get_Serial()
        {
            return Imager_Read_String_Until_DIAG("sn");
        }

        public string Imager_Get_SoftwareNumber()
        {
            return Imager_Read_String_Until_DIAG("swnum");
        }

        public string Imager_Get_Intsel()
        {
            return Imager_Read_String_Until_DIAG("intsel");
        }

        #endregion

        //#region Serial

        //public bool Imager_ReEmbedSerial(string serial, string oldserial)
        //{
        //    System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
        //    rijndael_metrologic.rijndael_metrologic.KeyInstance kInst = new rijndael_metrologic.rijndael_metrologic.KeyInstance();
        //    rijndael_metrologic.rijndael_metrologic.CipherInstance cInst = rijndael_metrologic.rijndael_metrologic.NewCipherInstance();
        //    string SNBuffer;
        //    byte[] SNPacket = new byte[16];
        //    byte[] EncPacket = new byte[16];
        //    byte[] bytKeyPhrase = new byte[16];
        //    int i;
        //    long pkt_crc;
        //    long lRet;
        //    string result;

        //    //Invalid Serial #
        //    if (serial.Length != 10 || serial == "0000000000")
        //        return false;

        //    //Append CRC to serial number packet
        //    for (i = 0; i < 10; i++)
        //        SNPacket[i] = (byte)serial[i];

        //    pkt_crc = rijndael_metrologic.rijndael_metrologic.cal_crcStdCall(ref SNPacket[0], 10);

        //    if (pkt_crc > 65535) pkt_crc = pkt_crc % 65536;

        //    SNPacket[10] = (byte)(pkt_crc / 256);
        //    SNPacket[11] = (byte)(pkt_crc % 256);

        //    for (i = 12; i <= 15; i++)
        //        SNPacket[i] = 0;

        //    //Encrypt Serial Number Packet
        //    for (i = 0; i < 10; i++)
        //        bytKeyPhrase[i] = (byte)oldserial[i];

        //    lRet = rijndael_metrologic.rijndael_metrologic.makeKeyPStdCall(ref kInst, (byte)rijndael_metrologic.rijndael_metrologic.Direction.DIR_ENCRYPT, 128, ref bytKeyPhrase[0], 10);
        //    if (lRet != 1)
        //    {
        //        Console.WriteLine("Encryption Error (10).");
        //        Serial_END(Serial);
        //        return false;
        //    }

        //    lRet = rijndael_metrologic.rijndael_metrologic.cipherInitStdCall(ref cInst, (byte)rijndael_metrologic.rijndael_metrologic.Modes.ECB, 0);
        //    if (lRet != 1)
        //    {
        //        Console.WriteLine("Encryption Error (11).");
        //        Serial_END(Serial);
        //        return false;
        //    }

        //    lRet = rijndael_metrologic.rijndael_metrologic.blockEncryptStdCall(ref cInst, ref kInst, ref SNPacket[0], 128, ref EncPacket[0]);
        //    if (lRet != 128)
        //    {
        //        Console.WriteLine("Encryption Error (12).");
        //        Serial_END(Serial);
        //        return false;
        //    }

        //    SNBuffer = "chgsn ";

        //    StringBuilder sb = new StringBuilder(24);

        //    sb.Append(SNBuffer);

        //    for (i = 0; i <= 15; i++)
        //        sb.AppendFormat("{0:x2}", EncPacket[i]);

        //    SNBuffer = sb.ToString() + "\x0D";

        //    result = Imager_Read_String_Until_DIAG(SNBuffer, 3000);

        //    if (result.Contains("SERIAL_NUMBER_UPDATED") == false)
        //    {
        //        Console.WriteLine("Unable to change serial number");
        //        return false;
        //    }

        //    Console.WriteLine("Successfully changed serial number");

        //    return true;
        //}

        //#endregion

        #region DAC

        public bool Imager_DAC_Set(string channel, int value)
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;

            //Set command
            command = "dac " + channel + " " + value.ToString();

            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return false; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Check for the DIAG> to come back
                Check = Serial_Wait_For_String(Serial, "DIAG>", 500);
            }

            //Serial_END(Serial);

            return Check;
        }

        public int Imager_DAC_Get(string channel)
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "dac " + channel;

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);
            
            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(3);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            //Serial_END(Serial);

            return DacValue;
        }

        #endregion

        #region IR

        public int Imager_IR_Check()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "ircheck";

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(1);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 200);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            //Serial_END(Serial);

            return DacValue;
        }

        public int Imager_IR_AutoSet_Far()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;
            int value;
            string tempdiag = "";
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "irconfig f";

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(30);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                        else
                        {
                            tempdiag = tempdiag + (char)value;

                            //Check to see if we get the DIAG back
                            if (tempdiag == "DIAG>") break;
                            if ("DIAG>".Contains(tempdiag) == false) tempdiag = "";                            
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            //Serial_END(Serial);

            return DacValue;
        }

        public int Imager_IR_AutoSet_Near()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;
            int value;
            string tempdiag = "";
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "irconfig n";

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(30);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                        else
                        {
                            tempdiag = tempdiag + (char)value;

                            //Check to see if we get the DIAG back
                            if (tempdiag == "DIAG>") break;
                            if ("DIAG>".Contains(tempdiag) == false) tempdiag = "";
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            //Serial_END(Serial);

            return DacValue;
        }

        #endregion

        #region Illumination

        public int Imager_Illummode_Linear()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illummode l";

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(1);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 200);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            //Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Illummode_Near()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check=true;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illummode n";

            //Check = Serial_INIT(Serial);
            //if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(1);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 200);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            //Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Illummode_Far()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illummode f";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(1);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 200);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Exposure_Check()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "exptest";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(2);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 200);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Illum_AutoSet_Area()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string tempdiag = "";
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illumcfg a";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(30);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                        else
                        {
                            tempdiag = tempdiag + (char)value;

                            //Check to see if we get the DIAG back
                            if (tempdiag == "DIAG>") break;
                            if ("DIAG>".Contains(tempdiag) == false) tempdiag = "";
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Illum_AutoSet_Far()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string tempdiag = "";
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illumcfg a f";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(30);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                        else
                        {
                            tempdiag = tempdiag + (char)value;

                            //Check to see if we get the DIAG back
                            if (tempdiag == "DIAG>") break;
                            if ("DIAG>".Contains(tempdiag) == false) tempdiag = "";
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Illum_AutoSet_Near()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string tempdiag = "";
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illumcfg a n";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(30);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                        else
                        {
                            tempdiag = tempdiag + (char)value;

                            //Check to see if we get the DIAG back
                            if (tempdiag == "DIAG>") break;
                            if ("DIAG>".Contains(tempdiag) == false) tempdiag = "";
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        public int Imager_Illum_AutoSet_Linear()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string tempdiag = "";
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "illumcfg l";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(30);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                        else
                        {
                            tempdiag = tempdiag + (char)value;

                            //Check to see if we get the DIAG back
                            if (tempdiag == "DIAG>") break;
                            if ("DIAG>".Contains(tempdiag) == false) tempdiag = "";
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        #endregion

        #region LED

        public bool Imager_LED_Test(string command)
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;

            Check = Serial_INIT(Serial);
            if (!Check) return false;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check && command != "")
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return false; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");
                Serial_FLUSH(Serial);
            }

            Serial_END(Serial);

            return Check;
        }

        #endregion

        #region Camera

        public int Imager_Camera_Check()
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            string command;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            int DacValue = -1;

            //Set command
            command = "camtest";

            Check = Serial_INIT(Serial);
            if (!Check) return -1;

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return -1; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(1);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 200);

                    if (value >= 0)
                    {
                        //It must be a number
                        if (value >= 48 && value <= 57)
                        {
                            temp = temp + (char)value;
                        }
                        else if (value == 45 && temp.Length == 0)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp == "-")       //If it got a negative but then get's garbage, then clear it and keep looking.
                        {
                            temp = "";
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    DacValue = Convert.ToInt32(temp);

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return DacValue;
        }

        #endregion

        #region Private Imager Functions

        public string Imager_Read_String_Until_DIAG(string command)
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            string result = "";

            Check = Serial_INIT(Serial);
            if (!Check) return "";

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, 300);
                if (!Check) { Serial_END(Serial); return ""; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(2);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a real character.  No Junk like CR or LF
                        if (value >= 32 && value <= 122)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    result = temp.Trim();

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return result;
        }

        public string Imager_Read_String_Until_DIAG(string command, int timeout)
        {
            System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            int value;
            string temp = "";
            bool flagGotNumber = false;
            DateTime myTimer;
            string result = "";

            Check = Serial_INIT(Serial);
            if (!Check) return "";

            Serial_FLUSH(Serial);

            //First see if it is already in diagnostics
            Serial_Write_String(Serial, "\x13");
            Check = Serial_Wait_For_String(Serial, "DIAG>", 500);

            if (Check)
            {
                Serial_FLUSH(Serial);

                //Send Command
                Serial_Write_String(Serial, command);

                //Check for the Echo First
                Check = Serial_Wait_For_String(Serial, command, timeout);
                if (!Check) { Serial_END(Serial); return ""; }

                //Send CR
                Serial_Write_String(Serial, "\x0D");

                //Read Integer value back
                myTimer = DateTime.Now.AddSeconds(timeout);
                while (myTimer > DateTime.Now)
                {
                    value = Serial_Wait_For_Char(Serial, 1000);

                    if (value >= 0)
                    {
                        //It must be a real character.  No Junk like CR or LF
                        if (value >= 32 && value <= 122)
                        {
                            temp = temp + (char)value;
                        }
                        else if (temp.Length > 0)
                        {
                            flagGotNumber = true;
                            break;
                        }
                    }
                }

                if (flagGotNumber)
                {
                    result = temp.Trim();

                    //Wait for the Diag Prompt back before continueing
                    Serial_Wait_For_String(Serial, "DIAG>", 500);
                }
            }

            Serial_END(Serial);

            return result;
        }

        #endregion

        #endregion

        #region HSM Based Products

        #region General

        public bool ClearInputBuffer()
        {
            m_serport.DiscardInBuffer();
            return true;
        }


        public string SendCommand(string cmd, Single WaitTimeSeconds, string searchstring)
        {
            long tmr;

            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            //if (searchstring == "")
            //    searchstring = ACK; // "\x06";

            if (!m_serport.IsOpen)
                m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(cmd);
            clsUtils.Dly(0.2);
            System.Windows.Forms.Application.DoEvents();
            if (searchstring != "")
            {
                try
                {
                    tmr = clsUtils.StartTimeInTicks();
                    while (clsUtils.ElapseTimeInSeconds(tmr) < WaitTimeSeconds)
                    {
                        if (m_serport.BytesToRead > 0)
                        {
                            cnt = m_serport.BytesToRead;
                            m_serport.Read(bytes, 0, cnt);
                            b = b + System.Text.Encoding.Default.GetString(bytes, 0, cnt);
                            i++;
                            if (b.Contains(searchstring))
                                break;
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    if (m_serport.IsOpen)
                        m_serport.Close();
                    //MsgBox(ex.Message)
                    return ex.Message + ": " + b;
                }
            }
            //if (m_serport.IsOpen)
            //    m_serport.Close();

            string msg = "command: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        public bool HSM_Send_Menu_Command(char type, string command, string parameters, char terminator1, char terminator2, ref string buffer)
        {
            int x;
            int value;
            //string echo = "";
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            //clsUtils.Dly(0.1);
            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_Byte(m_serport, 22);   //<SYN>
            Serial_Write_Byte(m_serport, (byte)type);
            Serial_Write_Byte(m_serport, 13);   //<CR>
            Serial_Write_String(m_serport, command);
            //If there are no parameters just skip this step
            if (parameters != null && parameters.Length > 0)
                Serial_Write_String(m_serport, parameters);
            if ((int)terminator1 != 0)
                Serial_Write_Byte(m_serport, (byte)terminator1);
            if ((int)terminator2 != 0)
                Serial_Write_Byte(m_serport, (byte)terminator2);

            //Wait for Echo
            //echo = command + parameters;
            //Check = Serial_Wait_For_String(Serial, echo, 3000);
            //if (!Check) { Serial_END(Serial); return false; }

            //Check for ACK
            buffer = "";
            Check = false;
            x = 0;
            DateTime to = DateTime.Now.AddMilliseconds(4000);
            while (to > DateTime.Now)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    value = m_serport.ReadByte();
                    if (x != 0 || value != 58)
                    {
                        buffer = buffer + (char)value;
                        x++;
                    }
                    if (value == 6) //ACK
                    {
                        Check = true;
                        break;
                    }
                    else if (value == 21) //NAK
                    {
                        Check = false;
                        break;
                    }
                    else if (value == 5) //ENQ
                    {
                        Check = false;
                        break;
                    }
                    //else
                    //{
                    //    //If the first character returned is a colon then ignore it
                    //    if (x != 0 || value != 58)
                    //    {
                    //        buffer = buffer + (char)value;
                    //        x++;
                    //    }
                    //}
                }
            }


            //Wait for 2mS and clear the buffer
            //DelayMS(2);
            Serial_FLUSH(m_serport);

            //Serial_END(Serial);

            return Check;
        }
        public bool HSM_Send_Menu_Command(char type, string command, string parameters, char terminator1, char terminator2,string strSearch,ref string buffer)
        {
            int x;
            int value;
            //string echo = "";
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            //clsUtils.Dly(0.1);
            //Check = Serial_INIT(Serial);
            //if (!Check) return false;

            Serial_FLUSH(m_serport);

            Serial_Write_Byte(m_serport, 22);   //<SYN>
            Serial_Write_Byte(m_serport, (byte)type);
            Serial_Write_Byte(m_serport, 13);   //<CR>
            Serial_Write_String(m_serport, command);
            //If there are no parameters just skip this step
            if (parameters != null && parameters.Length > 0)
                Serial_Write_String(m_serport, parameters);
            if ((int)terminator1 != 0)
                Serial_Write_Byte(m_serport, (byte)terminator1);
            if ((int)terminator2 != 0)
                Serial_Write_Byte(m_serport, (byte)terminator2);

            //Wait for Echo
            //echo = command + parameters;
            //Check = Serial_Wait_For_String(Serial, echo, 3000);
            //if (!Check) { Serial_END(Serial); return false; }
            buffer = "";
            //Check for ACK
            Check = false;
            x = 0;
            DateTime to = DateTime.Now.AddMilliseconds(4000);
            while (to > DateTime.Now)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    value = m_serport.ReadByte();
                    if (x != 0 || value != 58)
                    {
                        buffer = buffer + (char)value;
                        x++;
                    }
                    if (value == 6) //ACK
                    {
                        if (strSearch != "")
                        {
                            if (buffer.Contains(strSearch))
                            {
                                Check = true;
                                break;
                            }
                        }
                        else
                        {
                            Check = true;
                            break;
                        }
                    }
                    else if (value == 21) //NAK
                    {
                        Check = false;
                        break;
                    }
                    else if (value == 5) //ENQ
                    {
                        Check = false;
                        break;
                    }
                    //else
                    //{
                    //    //If the first character returned is a colon then ignore it
                    //    if (x != 0 || value != 58)
                    //    {
                    //        buffer = buffer + (char)value;
                    //        x++;
                    //    }
                    //}
                }
            }


            //Wait for 2mS and clear the buffer
            //DelayMS(2);
            Serial_FLUSH(m_serport);

            //Serial_END(Serial);

            return Check;
        }

        public bool HSM_Send_NoneMenu_Command(string command, ref string buffer)
        {
            int x;
            int value;
            //string echo = "";
            //System.IO.Ports.SerialPort Serial = new System.IO.Ports.SerialPort();
            bool Check;
            Serial_FLUSH(m_serport);
            Serial_Write_Byte(m_serport, 22);   //<SYN>
            Serial_Write_Byte(m_serport, (byte)'N');
            Serial_Write_String(m_serport, "," + command);
            Serial_Write_Byte(m_serport, 13);   //<CR>
            //If there are no parameters just skip this step
            //if (parameters != null && parameters.Length > 0)
            //    Serial_Write_String(m_serport, parameters);
            //if ((int)terminator1 != 0)
            //    Serial_Write_Byte(m_serport, (byte)terminator1);
            //if ((int)terminator2 != 0)
            //    Serial_Write_Byte(m_serport, (byte)terminator2);

            //Check for ACK
            Check = false;
            x = 0;
            DateTime to = DateTime.Now.AddMilliseconds(2000);
            while (to > DateTime.Now)
            {
                //Only read if there is something in the buffer
                if (m_serport.IsOpen && m_serport.BytesToRead > 0)
                {
                    value = m_serport.ReadByte();
                    if (x != 0 || value != 58)
                    {
                        buffer = buffer + (char)value;
                        x++;
                    }


                    if (value == 10) //ACK
                    {
                        Check = true;
                        break;
                    }
                    else if (value == 21) //NAK
                    {
                        Check = false;
                        break;
                    }
                    else if (value == 5) //ENQ
                    {
                        Check = false;
                        break;
                    }
                    //else
                    //{
                    //    //If the first character returned is a colon then ignore it
                    //    if (x != 0 || value != 58)
                    //    {
                    //        buffer = buffer + (char)value;
                    //        x++;
                    //    }
                    //}
                }
            }

            //Wait for 2mS and clear the buffer
            //DelayMS(2);
            Serial_FLUSH(m_serport);

            //Serial_END(Serial);

            return Check;
        }
       

        public bool HSM_FACTST(string device, string command, string parameters, ref string buffer)
        {
            string myparams = "";

            myparams = device + ":" + command + ":" + parameters;

            return HSM_Send_Menu_Command('Y', "FACTST", myparams, '.', (char)0, ref buffer);
        }

        public bool HSM_CALSET(string device, string paramid, string bytes)
        {
            string myparams = "";
            string dummy = "";

            myparams = device + ":" + paramid + ":" + bytes;

            return HSM_Send_Menu_Command('Y', "CALSET", myparams, '.', (char)0, ref dummy);
        }

        public bool HSM_CALGET(string device, string paramid, ref string buffer)
        {
            string myparams = "";

            myparams = device + ":" + paramid;

            return HSM_Send_Menu_Command('Y', "CALGET", myparams, '.', (char)0, ref buffer);
        }

        public bool GetBarcode(double d_Timeout, string str_Barcode, ref string buffer)
        {
            bool check = false;
            string str_Buffer = "";
            int idx;

            str_Buffer = ReadBuffer(d_Timeout, str_Barcode);

            if (str_Buffer.Length > 0)
            {
            //    idx = str_Buffer.IndexOf(CR, 0);
            //    if (idx == -1)
            //        return false;

            //    str_Buffer = str_Buffer.Substring(0, idx);

                if (str_Buffer.Contains(str_Barcode) == true)
                {
                    check = true;
                }
            }
            buffer = str_Buffer;
            return check;
        }
        public string ReadBuffer(double tmout, string terminator)
        {
            string commBuff;
            bool commRead;
            long timeTotal;
            long timeRef;
            int TermLength;

            TermLength = terminator.Length;

            //Debug.Print("Entering ReadInBuffer: " + m_serport.PortName);
            commBuff = "";
            commRead = false;
            try
            {
                //Convert it to 100 nanosecond increments
                timeTotal = Convert.ToInt64(tmout * System.TimeSpan.TicksPerSecond);
                timeRef = (System.DateTime.Now).Ticks;

                if (TermLength > 0)
                {
                    while (((System.DateTime.Now).Ticks - timeRef) < timeTotal)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        if (m_serport.BytesToRead > 0)
                        {
                            while (m_serport.BytesToRead > 0)
                            {
                                commBuff = commBuff + m_serport.ReadExisting();
                                if (commBuff.ToLower().LastIndexOf(terminator.ToLower()) != -1)
                                {
                                    commRead = true;
                                    break;
                                }
                            }
                        }
                        if (commRead == true)
                            break;
                    }
                }
                else
                {
                    while (((System.DateTime.Now).Ticks - timeRef) < timeTotal)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        if (m_serport.BytesToRead > 0)
                        {
                            while (m_serport.BytesToRead > 0)
                            {
                                commBuff = commBuff + m_serport.ReadExisting();

                                if (m_serport.BytesToRead < 1)
                                    clsUtils.Dly(0.3);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: ReadInBuffer function fail. " + ex.Message);
                //Debug.Print(ex.Message);
            }
            //Console.WriteLine(commBuff);
            //Debug.Print(commBuff);
            //Debug.Print("Exiting ReadInBuffer: " + m_serport.PortName);

            return commBuff;
        }

        public bool HSM_CALSAV()
        {
            string dummy = "";

            return HSM_Send_Menu_Command('Y', "CALSAV", "", '.', (char)0, ref dummy);
        }

        public bool HSM_ADC_GETMVOLTS(int index, ref double value)
        {
            string setindex = "";
            string buffer = "";
            bool Check;
            int tempvalue;

            value = 0;

            setindex = String.Format("{0:X2}", index);

            Check = HSM_FACTST("adc-simple", "3", setindex, ref buffer);
            if (!Check || buffer.Length < 2) return false;

            //Convert Buffer to Double Value
            tempvalue = Convert.ToInt32(FlipHexString(buffer), 16);
            value = Convert.ToDouble(tempvalue);

            return true;
        }

        public bool HSM_GPIO_SET(int index, int value)
        {
            string setparameters = "";
            string buffer = "";
            bool Check;

            //Make sure the value is either a 1 or a 0
            if (value != 0) value = 1;

            setparameters = String.Format("{0:X2}", index) + ":" + value.ToString();

            Check = HSM_FACTST("gpio", "5", setparameters, ref buffer);

            return Check;
        }

        public bool HSM_SET_SERIAL(string serial)
        {
            string temp = "";
            bool Check;

            if (serial.Length != 10) return false;

            for (int x = 0; x < 10; x++)
                temp = temp + String.Format("{0:X2}", (int)serial[x]);

            //Append the extra 6 bytes of space not used
            temp = temp + "000000000000";

            Check = HSM_CALSET("system", "1", temp);

            return Check;
        }

        public bool HSM_LEGACY_SET_SERIAL(string serial)
        {
            string temp = "";
            bool Check;

            if (serial.Length != 10) return false;

            Check = HSM_Send_Menu_Command('Y', "SERNUM", serial, '.', (char)0, ref temp);

            return Check;
        }

        public bool HSM_LEGACY_GET_SERIAL(ref string serial)
        {
            bool Check;
            string temp = "";

            serial = "";

            //Get Serial Number
            Check = HSM_Send_Menu_Command('Y', "SERNUM", "", '?', '.', ref temp);

            //Validate Serial # Size
            if (!Check || temp.Length != 10)
                return false;

            serial = temp;

            return true;
        }

        public bool HSM_GET_SERIAL(ref string serial)
        {
            bool Check;
            string temp = "";

            serial = "";

            //Get Serial Number
            Check = HSM_CALGET("system", "1", ref temp);

            //Validate Serial # Size
            if (!Check || temp.Length != 32)
                return false;

            //Convert Serial String from Hex to Characters
            for (int x = 0; x < 10; x++)
            {
                serial = serial + (char)ConvertHexCharToDecimal(temp[x * 2], temp[(x * 2) + 1]);
            }

            return true;
        }

        /*
        public bool HSM_GET_SWNUM(ref string swnum)
        {
            return HSM_Send_Menu_Command('M', "REV_SW", "", '?', '.', ref swnum);
        }
        */

        public bool HSM_GET_SWNUM(ref string swnum)
        {
            return HSM_Send_Menu_Command('M', "REV_WA", "", '?', '.', ref swnum);
        }

        public bool HSM_GET_SCANNER_BTADDRESS(ref string address)
        {
            bool Check;
            string temp = "";

            address = "";

            //Get Serial Number
            Check = HSM_Send_Menu_Command('M', "BT_LDA", "", '.', (char)0, ref temp);

            //Validate Address Size
            if (!Check || temp.Length != 12)
                return false;

            address = temp;

            return true;
        }

        public bool HSM_GET_SCANNER_BTCONNECT(ref string address)
        {
            bool Check;
            string temp = "";

            address = "";

            //Get Serial Number
            Check = HSM_Send_Menu_Command('M', "BT_ADR", "", '?', '.', ref temp);

            //Validate Address Size
            if (!Check || temp.Length != 12)
                return false;

            address = temp;

            return true;
        }

        public bool HSM_SET_SCANNER_BTCONNECT(string address)
        {
            bool Check;
            string temp = "";

            //Validate Address Size
            if (address.Length != 12)
                return false;

            //Get Serial Number
            Check = HSM_Send_Menu_Command('M', "BT_ADR", address, '.', (char)0, ref temp);

            return Check;
        }

        public bool HSM_GET_CRADLE_BTADDRESS(ref string address)
        {
            bool Check;
            string temp = "";

            address = "";

            //Get Serial Number
            Check = HSM_Send_Menu_Command('M', "BASLDA", "", '.', (char)0, ref temp);

            //Validate Address Size
            if (!Check || temp.Length != 12)
                return false;

            address = temp;

            return true;
        }

        public string GetMenuInfo(string strMenu,string strMenuString)
        {
            int index1;
            int index2;
            string strInfo = "";

            if (strMenu.Substring(0, 3) == ":*:") strMenu = strMenu.Replace(":*:", "");
            index1 = strMenuString.IndexOf(strMenu);
            index1 = index1 + strMenu.Length;
            index2 = strMenuString.IndexOf("\x06");

            if (index1 < 0 || index2 < 0) return "";
            strInfo = strMenuString.Substring(index1, index2 - index1);

            return strInfo;
        }

        #endregion

        #region Laser

        public bool HSM_Laser_Enable(int state, bool enterTM)
        {
            bool Check;
            string dummy = "";

            //Make sure we are in Flipper Test Mode
            if (enterTM)
            {
                Check = HSM_FACTST("nec_dac_flipper", "0", "01", ref dummy);
                if (!Check) return false;
            }

            //Toggle Laser Enable
            Check = HSM_FACTST("nec_dac_flipper", "1", String.Format("{0:X2}", state), ref dummy);

            return (Check);
        }

        #endregion

        #region Flipper

        public bool HSM_Flipper_Enable(int state, bool enterTM)
        {
            bool Check;
            string dummy = "";

            //Make sure we are in Flipper Test Mode
            if (enterTM)
            {
                Check = HSM_FACTST("nec_dac_flipper", "0", "01", ref dummy);
                if (!Check) return false;
            }

            //Toggle Laser Enable
            Check = HSM_FACTST("nec_dac_flipper", "3", String.Format("{0:X2}", state), ref dummy);

            return (Check);
        }

        public bool HSM_Flipper_Get_Width(string device, string command, ref int adc, ref int pwm)
        {
            bool Check;
            string temp = "";
            string flipstring = "";
            string adc_string = "";
            string pwm_string = "";
            int i;

            Check = HSM_FACTST(device, command, "", ref temp);
            if (!Check || temp.Length < 8) return false;

            flipstring = FlipHexString(temp);

            for (i = 0; i < 4; i++)
                adc_string = adc_string + flipstring[i];

            for (i = 4; i < 8; i++)
                pwm_string = pwm_string + flipstring[i];

            Check = Int32.TryParse(adc_string, System.Globalization.NumberStyles.HexNumber, null, out adc);
            if (!Check) 
            { 
                adc = 0; 
                //clsGlobals.log.Warn("Failed to Parse Hexadecimal Data");
                mstr_error = "Error: Failed to Parse Hexadecimal Data" + "\n";
                return false; 
            }

            Check = Int32.TryParse(pwm_string, System.Globalization.NumberStyles.HexNumber, null, out pwm);
            if (!Check) 
            { 
                pwm = 0; 
                //clsGlobals.log.Warn("Failed to Parse Hexadecimal Data"); 
                mstr_error = "Error: Failed to Parse Hexadecimal Data" + "\n";
                return false; 
            }

            return true;
        }

        public bool HSM_Flipper_Set_Width(string device, string command, int adc, int pwm)
        {
            bool Check;
            string temp = "";
            string parameters = "";
            int width;

            width = (adc<<16) + pwm;

            parameters = width.ToString("X8");

            Check = HSM_FACTST(device, command, parameters, ref temp);

            return Check;
        }

        #endregion

        #endregion

        #endregion


        #region Private Functions

        #region Serial Functions

        private bool Serial_INIT(System.IO.Ports.SerialPort Serial)
        {
            Serial.BaudRate = (int)m_lCommBaudRate;
            Serial.PortName = "COM" + m_iCommPortNumber.ToString();
            Serial.Parity = m_pCommParity;
            Serial.DataBits = m_iCommDataBits;

            if (m_sCommStopBits == System.IO.Ports.StopBits.None)            
                Serial.StopBits = System.IO.Ports.StopBits.One;
            else
                Serial.StopBits = m_sCommStopBits;

            try
            {
                if (!Serial.IsOpen)
                    Serial.Open();                            
            }
            catch (Exception ex)
            {
                //clsGlobals.log.Error("", ex);
                mstr_error = "Error: Serial open" + "\n" + ex.Message;
                //if(m_bShowErrors)
                //    System.Windows.Forms.MessageBox.Show("Error: Tried to open an already open Comm Port", "Comm Port Already Open", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void Serial_END(System.IO.Ports.SerialPort Serial)
        {
            try
            {
                if (Serial.IsOpen)
                    Serial.Close();
            }
            catch (Exception ex)
            {
                //clsGlobals.log.Error("", ex);
                mstr_error = "Error: Serial Close" + "\n" + ex.Message;
                if (m_bShowErrors)
                    Console.WriteLine("Error: Tried to close an already closed Comm Port");
            }
        }

        private void Serial_FLUSH(System.IO.Ports.SerialPort Serial)
        {
            try
            {
                if (Serial.IsOpen)
                {
                    Serial.ReadExisting();
                    Serial.DiscardInBuffer();
                }
            }
            catch (Exception ex)
            {
                //clsGlobals.log.Error("", ex);
                mstr_error = "Error: " + "\n" + ex.Message;
                if (m_bShowErrors)
                    Console.WriteLine("Error: Tried to flush a buffer that was already open");
            }
        }

        public string ReadInBuffer(double tmout, string terminator)
        {
            string commBuff;
            bool commRead;
            long timeTotal;
            long timeRef;

            Debug.Print("Entering ReadInBuffer: " + m_serport.PortName);
            commBuff = "";
            commRead = false;
            try
            {
                //Convert it to 100 nanosecond increments
                timeTotal = Convert.ToInt64(tmout * System.TimeSpan.TicksPerSecond);
                timeRef = (System.DateTime.Now).Ticks;
                while (((System.DateTime.Now).Ticks - timeRef) < timeTotal)
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (m_serport.BytesToRead > 0)
                    {
                        while (m_serport.BytesToRead > 0)
                        {
                            commBuff = commBuff + m_serport.ReadExisting();
                            if (commBuff.ToLower().LastIndexOf(terminator.ToLower()) != -1)
                            {
                                commRead = true;
                                break;
                            }
                        }
                    }
                    if (commRead == true)
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            //Console.WriteLine(commBuff);
            Debug.Print(commBuff);
            Debug.Print("Exiting ReadInBuffer: " + m_serport.PortName);

            return commBuff;
        }

        private bool Serial_Write_String(System.IO.Ports.SerialPort Serial, string myOutput)
        {
            int x;
 
            if (Serial.IsOpen)
            {
                for (x = 0; x < myOutput.Length; x++)
                {
                    try
                    {
                        Serial.Write(myOutput.Substring(x, 1));
                    }
                    catch (Exception ex)
                    {
                        //clsGlobals.log.Error("", ex);
                        mstr_error = "Error: Serial Write String" + "\n" + ex.Message;
                        return false;
                    }    
                
                    //Add the intercharacter Delay
                    DelayMS(m_iIntercharacterDelay);
                }
            }
            else
                return false;

            //After the transmission is done, wait a little longer
            DelayMS(m_iIntercharacterDelay);

            return true;
        }

        private bool Serial_Write_Byte(System.IO.Ports.SerialPort Serial, byte myOutput)
        {
            byte[] temp = new byte[2];
            

            temp[0] = myOutput;
            temp[1] = 0;

            if (Serial.IsOpen)
            {
                try
                {                        
                    Serial.Write(temp, 0, 1);
                }
                catch (Exception ex)
                {
                    //clsGlobals.log.Error("", ex);
                    mstr_error = "Error: Serial Write Byte" + "\n" + ex.Message;
                    return false;
                }

                //Add the intercharacter Delay
                DelayMS(m_iIntercharacterDelay);                
            }
            else
                return false;

            //After the transmission is done, wait a little longer
            DelayMS(m_iIntercharacterDelay);

            return true;
        }

        private bool Serial_Wait_For_ACK(System.IO.Ports.SerialPort Serial, double TimeoutmSeconds)
        {
            System.DateTime myTime = new DateTime();
            int myRead;

            myTime = System.DateTime.Now.AddMilliseconds(TimeoutmSeconds);

            while (myTime > System.DateTime.Now)
            {
                //Only read if there is something in the buffer
                if (Serial.IsOpen && Serial.BytesToRead > 0)
                {
                    myRead = Serial.ReadByte();
                     if (myRead == 6)            //ACK
                        return true;
                    else if (myRead == 21)      //NAK
                        return false;
                }
            }

            return false;
        }

        private int Serial_Wait_For_Char(System.IO.Ports.SerialPort Serial, double TimeoutmSeconds)
        {
            System.DateTime myTime = new DateTime();
            int myRead;

            myTime = System.DateTime.Now.AddMilliseconds(TimeoutmSeconds);

            while (myTime > System.DateTime.Now)
            {
                //Only read if there is something in the buffer
                if (Serial.IsOpen && Serial.BytesToRead > 0)
                {
                    myRead = Serial.ReadByte();

                    return (myRead);
                }
            }

            return -1;
        }

        private bool Serial_Wait_For_String(System.IO.Ports.SerialPort Serial, string MyString, double TimeoutmSeconds)
        {
            System.DateTime myTime = new DateTime();
            int myRead;
            string temp = "";

            myTime = System.DateTime.Now.AddMilliseconds(TimeoutmSeconds);

            while (myTime > System.DateTime.Now)
            {
                //Only read if there is something in the buffer
                if (Serial.IsOpen && Serial.BytesToRead > 0)
                {
                    myRead = Serial.ReadByte();

                    temp = temp + (char)myRead;

                    if (temp.Contains(MyString)) return true;
                }
            }

            return false;
        }

        #endregion

        #region Generic Functions

        private void DelayMS(int Timeout)
        {
            if (Timeout == 0) return;
            System.Threading.Thread.Sleep(Timeout);
            System.Windows.Forms.Application.DoEvents();
        }

        private int ConvertHexCharToDecimal(char character1, char character2)
        {
            string temp = "";

            temp = "" + character1 + character2;        

            return Convert.ToInt32(temp, 16);
        }

        private string FlipHexString(string startstring)
        {
            int x;
            string endstring = "";
            int length;

            length = startstring.Length;

            for (x = 0; x < length; x += 2)
            {
                endstring = endstring + startstring[length - (x + 2)];
                endstring = endstring + startstring[length - (x + 1)];
            }

            return endstring;
        }

        #endregion

        #endregion

    }
}
