using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;

namespace F001716
{
    public struct serialSettings_t
    {
        public int PortNum;
        public int Baud;
        public System.IO.Ports.Parity Parity;
        public int DataBits;
        public System.IO.Ports.StopBits StopBits;  //Int32
    }

    public class clsScanner
    {
        public const string ACK = "\x06";
        public const string SYN = "\x16";
        public const string CR = "\x0d";
        public const string DC1 = "\x11";

        private SerialPort m_serport = null;
        private serialSettings_t m_settings;
       // private m_Utils Utils;
        public serialSettings_t dSettings
        {
            get { return m_settings; }
            set
            {
                serialSettings_t s;
                s.Baud = value.Baud; //CInt(s(0))
                s.Parity = value.Parity; //CType(strParity.IndexOf(s(1).ToUpper), Parity)
                s.DataBits = value.DataBits; //CInt(s(2))
                s.StopBits = value.StopBits; //CType(s(3), StopBits)
                s.PortNum = value.PortNum;
                m_settings = s;
            }
        }

        public bool dPortState
        {
            get { return m_serport.IsOpen; }
            set
            {
                if (value == true)
                {
                    if (m_serport.IsOpen == true) m_serport.Close();
                    SetPortProperties();
                    if (m_serport.IsOpen == false) m_serport.Open();
                }
                else
                {
                    if (m_serport.IsOpen == true) m_serport.Close();
                }
            }
        }

        public bool cCtsState
        {
            get
            {
                bool st;
                if (m_serport.IsOpen == false)
                {
                    SetPortProperties();
                    m_serport.Open();
                }
                st = m_serport.CtsHolding;
                //m_ComPort.Close()
                return st;
            }
        }

        public bool cRtsEnable
        {
            get
            {
                bool st;
                if (m_serport.IsOpen == false)
                {
                    SetPortProperties();
                    m_serport.Open();
                }
                st = m_serport.RtsEnable;
                //'m_ComPort.Close()
                return st;
            }
            set
            {
                m_serport.RtsEnable = value;
            }
        }

        public clsScanner()
        {
            //m_Utils = new Utils();
            m_serport = new SerialPort();
            m_serport.DataReceived += new SerialDataReceivedEventHandler(m_serport_DataReceived);
        }

        ~clsScanner()
        {
            if (m_serport != null)
            {
                if (m_serport.IsOpen == true) m_serport.Close();
                m_serport = null;
            }
        }

        public void InitCom()
        {

        }

        private void SaveFile(string fname, byte[] buf)
        {
            FileStream fs = new FileStream(fname, System.IO.FileMode.Create, FileAccess.ReadWrite);
            fs.Write(buf, 0, buf.Length);
            fs.Close();
        }
        private void SetPortProperties()
        {
            m_serport.PortName = "COM" + m_settings.PortNum.ToString();
            m_serport.BaudRate = m_settings.Baud;
            m_serport.DataBits = m_settings.DataBits;
            m_serport.Parity = m_settings.Parity;
            m_serport.StopBits = m_settings.StopBits;
            m_serport.ReceivedBytesThreshold = 1;
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
                                commRead = true;
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

        public string SendCommand(string cmd, Single WaitTimeSeconds, string searchstring)
        {
            long tmr;

            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            //if (searchstring == "")
            //    searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(cmd);
            clsUtils.Dly(0.1);
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

        // overload #1
        public string SendMenuCommand(string cmd)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            menucmd = SYN + "M" + CR + cmd;
            Single WaitTimeSeconds = 2;
            string searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        // overload #2
        public string SendMenuCommand(string cmd, Single WaitTimeSeconds)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            menucmd = SYN + "M" + CR + cmd;
            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            string searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        // overload #3
        public string SendMenuCommand(string cmd, Single WaitTimeSeconds, string searchstring)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            menucmd = SYN + "M" + CR + cmd;
            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            if (searchstring == "")
                searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        // overload #1
        public string SendTestCommand(string cmd)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            //menucmd = SYN + "M" + CR + cmd;
            menucmd = SYN + DC1 + cmd + CR;
            Single WaitTimeSeconds = 2;
            string searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        // overload #2
        public string SendTestCommand(string cmd, Single WaitTimeSeconds)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            //menucmd = SYN + "M" + CR + cmd;
            menucmd = SYN + DC1 + cmd + CR;
            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            string searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        // overload #3
        public string SendTestCommand(string cmd, Single WaitTimeSeconds, string searchstring)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            //menucmd = SYN + "M" + CR + cmd;
            menucmd = SYN + DC1 + cmd + CR;
            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            if (searchstring == "")
                searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        public string SendNonMenuCommand(string cmd, Single WaitTimeSeconds, string searchstring)
        {
            long tmr;
            string menucmd;

            //menucmd = "\x16" + "M" + "\x0d" + cmd;
            //menucmd = SYN + "M" + CR + cmd;
            menucmd = SYN + "n," + cmd + CR;
            if (WaitTimeSeconds == 0)
                WaitTimeSeconds = 2;
            //if (searchstring == "")
            //    searchstring = ACK; // "\x06";

            //if (m_serport.IsOpen)
            //    m_serport.Close();
            //SetPortProperties();
            //m_serport.Open();

            m_serport.ReadTimeout = Convert.ToInt32(WaitTimeSeconds) * 1000;
            m_serport.DiscardInBuffer();
            //lng_WaitTm = Convert.ToInt64(timeout * System.TimeSpan.TicksPerSecond)

            //Dim bytes(m_serport.ReadBufferSize) As Byte
            byte[] bytes = new byte[m_serport.ReadBufferSize];
            string b = "";
            int cnt;
            int i = 0;
            m_serport.Write(menucmd);
            clsUtils.Dly(0.1);
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

            string msg = "menucommand: " + b + " loops: " + i.ToString();
            //Console.WriteLine(msg);
            Debug.Print(msg);
            return b;
        }

        //events
        private void m_serport_DataReceived(Object sender, System.IO.Ports.SerialDataReceivedEventArgs e) //Handles this.DataReceived
        {
            int a = 10;
        }

    }
}
