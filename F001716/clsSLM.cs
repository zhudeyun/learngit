using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices; // DllImport
using System.Diagnostics;

namespace F001716
{
    class clsSLM
    {

        #region Variables

        private int m_iIntercharacterDelay = 10;

        private int m_iCommPortNumber = 1;
        private long m_lCommBaudRate = 9600;
        private System.IO.Ports.Parity m_pCommParity = System.IO.Ports.Parity.None;
        private int m_iCommDataBits = 8;
        private System.IO.Ports.StopBits m_sCommStopBits = System.IO.Ports.StopBits.One;

        private System.IO.Ports.SerialPort m_serport = null;

        private bool m_bShowErrors = true;
        private string mstr_error;

        private serialSettings_t m_settings;

        #endregion


        #region Constructor

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>

        public clsSLM()
        {
            m_serport = new System.IO.Ports.SerialPort();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>
        public clsSLM(int SerialPortNumber, long SerialBaudRate)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;

            m_serport = new System.IO.Ports.SerialPort();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="SerialPortNumber"></param>
        /// <param name="SerialBaudRate"></param>
        /// <param name="SerialDataBits"></param>
        /// <param name="SerialParity"></param>
        /// <param name="SerialStopBits"></param>
        public clsSLM(int SerialPortNumber, long SerialBaudRate, System.IO.Ports.Parity SerialParity, int SerialDataBits, System.IO.Ports.StopBits SerialStopBits)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;
            m_pCommParity = SerialParity;
            m_iCommDataBits = SerialDataBits;
            m_sCommStopBits = SerialStopBits;

            m_serport = new System.IO.Ports.SerialPort();
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
        public clsSLM(int SerialPortNumber, long SerialBaudRate, System.IO.Ports.Parity SerialParity, int SerialDataBits, System.IO.Ports.StopBits SerialStopBits, int IntercharacterDelay)
        {
            m_iCommPortNumber = SerialPortNumber;
            m_lCommBaudRate = SerialBaudRate;
            m_pCommParity = SerialParity;
            m_iCommDataBits = SerialDataBits;
            m_sCommStopBits = SerialStopBits;

            m_iIntercharacterDelay = IntercharacterDelay;

            m_serport = new System.IO.Ports.SerialPort();
        }

        #endregion


        #region Properties

        public System.IO.Ports.SerialPort SerialPort
        {
            get { return m_serport; }
            set
            {
                m_serport = value;
            }
        }

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
                    Serial_FLUSH(m_serport);
                    if (m_serport.IsOpen == false) Serial_INIT(m_serport);
                }
                else
                {
                    if (m_serport.IsOpen == true)
                        Serial_END(m_serport);
                }
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

        #endregion


        #region Public Functions

        #region General

        public bool SendString(string str_Send)
        {
            bool Check = true;

            Check = true;

            Serial_FLUSH(m_serport);

            Serial_Write_String(m_serport, str_Send);

            return Check;
        }

        public bool GetVolume(ref string str_Volume)
        {
            bool check = false;
            string str_InBuffer = "";

            m_serport.DtrEnable = true;

            str_InBuffer = ReadInBuffer(2, "");

            if (GetValidInfo("8A", "8A", str_InBuffer, ref str_Volume) == true)
            {
                check = true;
                str_Volume = str_InBuffer.Substring(0, str_InBuffer.IndexOf("8A"));
            }

            return check;
        }

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
                if(m_bShowErrors)
                    System.Windows.Forms.MessageBox.Show("Error: Tried to open an already open Comm Port", "Comm Port Already Open", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
        public  bool discardBuffer()
        {
            m_serport.DiscardInBuffer();
            return true;
        }
        public string ReadInBuffer(double tmout, string terminator)
        {
            string commBuff;
            int myRead;
            bool commRead;
            long timeTotal;
            long timeRef;
            int TermLength;
            string str_HexString;

            TermLength = terminator.Length;

            //Debug.Print("Entering ReadInBuffer: " + m_serport.PortName);
            commBuff = "";
            str_HexString = "";
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
                                myRead = m_serport.ReadByte();
                                commBuff = commBuff + ((char)myRead).ToString();
                                str_HexString = ConvertToHexString(commBuff);

                                if (str_HexString.LastIndexOf(terminator) != -1)
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
                                myRead = m_serport.ReadByte();
                                commBuff = commBuff + ((char)myRead).ToString();
                                str_HexString = ConvertToHexString(commBuff);

                                if (m_serport.BytesToRead < 1)
                                    DelayMS(300);
                                if (((System.DateTime.Now).Ticks - timeRef) >timeTotal)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.Message);
                mstr_error = "Error: Read in buffer" + "\n" + ex.Message;
            }

            return str_HexString;
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

        private string ConvertToHexString(string str_StartString)
        {
            string str_HexString = "";

            for (int i = 0; i < str_StartString.Length; i++)
            {
                str_HexString = str_HexString + String.Format("{0:X2}", (int)str_StartString[i]);
            }

            return str_HexString;
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

        private bool GetValidInfo(string cmdstr, string cmdterm, string cmdres, ref string cinfo)
        {
            int idx;

            //  pull out string information from any menu cmd
            cinfo = "";
            idx = cmdres.IndexOf(cmdstr, 0);
            if (idx == -1)
            {
                return false;
            }
            cmdres = cmdres.Substring(idx + cmdstr.Length);

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

        #endregion

        #endregion

    }
}
