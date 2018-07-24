using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace F001716
{

    public enum PSPortEnum
    {
        PSNONE = 0,
        PSOUT1 = 1,
        PSOUT2 = 2,
        PSBOTH = 3
    }

    public class clsAgilentE3648A
    {

        private string mstr_Error;
        private System.IO.Ports.SerialPort m_ComPort;
        //private cls_utils Utils  ;

        private Single msng_StartVoltageLevel;
        private Single msng_OverVoltageLevel;
        private Single msng_UnderVoltageLevel;
        private Single msng_LastSetVoltageLevel;
        private Single msng_IcrementLevel;


        public clsAgilentE3648A()
        {
            mstr_Error = "";
            //Utils = new cls_utils();
            msng_StartVoltageLevel = 4.0F;
            msng_LastSetVoltageLevel = msng_StartVoltageLevel;
            msng_OverVoltageLevel = 6.0F; //9.0
            msng_UnderVoltageLevel = 1.0F;
            msng_IcrementLevel = 0.005F;
        }

        ~clsAgilentE3648A()
        {


        }

        // Properties
        public Single LastSetVoltage
        {
            get
            {
                return msng_LastSetVoltageLevel;
            }
        }

        public string Err
        {
            get
            {
                return mstr_Error;
            }
        }

        // *******************************************************
        // * Comm Port functions
        // *******************************************************

        public bool InitComm(ComPortSettings_t Settings)
        {

            if (m_ComPort == null)
            {
                m_ComPort = new System.IO.Ports.SerialPort();
            }

            PortClose();
            try
            {
                m_ComPort.PortName = "COM" + Settings.PortNum.ToString(); 
                m_ComPort.BaudRate = Settings.Baud;
                switch (Settings.Parity)
                {
                    case "N":
                    case "n":
                        m_ComPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "O":
                    case "o":
                        m_ComPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case "S":
                    case "s":
                        m_ComPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                    case "E":
                    case "e":
                        m_ComPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "M":
                    case "m":
                        m_ComPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                }
                m_ComPort.DataBits = Settings.DataBits;

                switch (Settings.StopBits)
                {
                    case 0:
                        m_ComPort.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case 1:
                        m_ComPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case 2:
                        m_ComPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                }

                m_ComPort.Handshake = System.IO.Ports.Handshake.None;
                m_ComPort.DtrEnable = true;
                PortOpen();
                PortDiscard();

            }
            catch (Exception ex)
            {
                try
                {
                    PortClose();
                }
                catch (Exception exx)
                {
                }
                mstr_Error = "Error: Initializing Power Supply on COM " + m_ComPort.PortName + Convert.ToChar(13) + Convert.ToChar(10) + ex.Message;
                return false;
            }

            return true;
        }

        private bool PortClose()
        {

            try
            {
                if (m_ComPort.IsOpen == true) m_ComPort.Close();
            }
            catch
            {
            }
            if (m_ComPort.IsOpen == true)
            {
                return false;
            }

            return true;
        }

        private bool PortOpen()
        {

            try
            {
                if (m_ComPort.IsOpen == false) m_ComPort.Open();
            }
            catch
            {
            }
            if (m_ComPort.IsOpen == false)
            {
                return false;
            }

            return true;
        }

        private bool PortDiscard()
        {

            m_ComPort.DiscardInBuffer();
            return true;
        }

        private bool PSWrite(string cmd)
        {

            PortDiscard();
            m_ComPort.Write(cmd);
            return true;
        }

        private string PSRead(string terminator, Single WaitTimeSeconds)
        {
            Int64 lng_WaitTm;
            string data;
            Int32 TermL;
            long tmr;

            if (terminator == null) terminator = "";
            if (WaitTimeSeconds == 0) WaitTimeSeconds = 2;

            data = "";
            TermL = terminator.Length;
            lng_WaitTm = Convert.ToInt64(WaitTimeSeconds * System.TimeSpan.TicksPerSecond);

            tmr = clsUtils.StartTimeInTicks();
            while (clsUtils.ElapseTimeInSeconds(tmr) < WaitTimeSeconds)
            {
                if (m_ComPort.BytesToRead > 0)
                {
                    while (m_ComPort.BytesToRead > 0)
                    {
                        data = data + Convert.ToChar(m_ComPort.ReadByte());
                    }
                    System.Windows.Forms.Application.DoEvents();

                    if (m_ComPort.BytesToRead == 0)
                    {
                        if (TermL > 0)
                        {
                            if (data.EndsWith(terminator) == true)
                            {
                                break;
                            }
                        }
                    }
                }
                System.Windows.Forms.Application.DoEvents();
            }

            return data;
        }

        public bool PSCommandWr(string cmd, out string outres)
        {
            long tmr;
            Single WaitTimeSeconds;
            string term;
            int idx;
            string res;

            outres = "";
            WaitTimeSeconds = 2;
            term = "1" + Convert.ToChar(13) + Convert.ToChar(10);
            res = "";

            PSWrite(cmd + Convert.ToChar(10)); //write the cmd
            clsUtils.Dly(0.3);  // do not overload buffer
            //PSWrite("*OPC" + Convert.ToChar(10)) //allow command sequence compleat flag
            //PSWrite("*STB?" + Convert.ToChar(10))
            PSWrite("*OPC?" + Convert.ToChar(10)); //allow command sequence compleat flag
            tmr = clsUtils.StartTimeInTicks();
            while (clsUtils.ElapseTimeInSeconds(tmr) < WaitTimeSeconds)
            {
                res = res + PSRead(term, 0.2F);
                if (res.EndsWith(term) == true)
                {
                    break;
                }
                clsUtils.Dly(0.1);
            }

            //PSWrite("*CLS" + Convert.ToChar(10)) //Clear status
            idx = res.IndexOf(term, 0);
            if (idx == -1)
            {
                mstr_Error = "Error: Failed to send command :" + cmd;
                return false;
            }
            outres = res.Substring(0, idx);

            return true;
        }

        public bool PSCommandRd(string cmd, out string outres)
        {
            long tmr;
            Single WaitTimeSeconds;
            string term;
            int idx;
            string res;

            outres = "";
            WaitTimeSeconds = 3;
            term = "\r\n"; // Convert.ToString(Convert.ToChar(13) + Convert.ToChar(10));
            res = "";

            PSWrite(cmd + Convert.ToChar(10));//write the cmd
            clsUtils.Dly(0.3); // do not overload buffer
            //PSWrite("*OPC?" + Convert.ToChar(10)) //allow command sequence compleat flag
            //PSWrite("*OPC" + Convert.ToChar(10)) //allow command sequence compleat flag
            //PSWrite("*STB?" + Convert.ToChar(10))
            tmr = clsUtils.StartTimeInTicks();
            while (clsUtils.ElapseTimeInSeconds(tmr) < WaitTimeSeconds)
            {
                res = res + PSRead(term, 0.2F);
                if (res.EndsWith(term) == true)
                {
                    break;
                }
                clsUtils.Dly(0.1);
            }

            //PSWrite("*CLS" + Convert.ToChar(10)) //Clear status
            idx = res.IndexOf(term, 0);
            if (idx == -1)
            {
                mstr_Error = "Error: Failed to send command :" + cmd;
                return false;
            }
            outres = res.Substring(0, idx);

            return true;
        }

        // *******************************************************
        // * Power Supply functions
        // *******************************************************

        private string ReadError()
        {
            string res = "";
            PSCommandRd("SYST:ERR?", out res);
            return res;
        }

        public bool InitSupply()
        {
            string res = "";

            // system cmds
            // PSWrite("*IDN?" + Convert.ToChar(10))
            PSWrite("*RST" + Convert.ToChar(10));
            clsUtils.Dly(1);
            PSWrite("*CLS" + Convert.ToChar(10));
            clsUtils.Dly(0.2);
            PSWrite("*ESE 1" + Convert.ToChar(10));
            clsUtils.Dly(0.2);
            PSWrite("*OPC?" + Convert.ToChar(10));
            clsUtils.Dly(0.2);
            res = PSRead("\r\n", 0);
            if (res != ("1" + Convert.ToChar(13) + Convert.ToChar(10)))
            {
                mstr_Error = "Error: Failed to Reset PS " + Convert.ToChar(13) + Convert.ToChar(10) + ReadError();
                return false;
            }

            PSCommandWr("DISP:STAT ON", out res);
            PSCommandWr("SYST:REM", out res);
            if (!OutputEnable(false))
            { //Turn output off
                mstr_Error = "Error: Failed to toggle output: " + Convert.ToChar(13) + Convert.ToChar(10) + ReadError();
                return false;
            }

            if (!SelectOutput(PSPortEnum.PSOUT1))
            {
                mstr_Error = "Error: Failed to select output: " + Convert.ToChar(13) + Convert.ToChar(10) + ReadError();
                return false;
            }
            PSCommandWr("VOLT:PROT:CLE", out res);
            OutputEnable(false);
            PSCommandWr("VOLT:PROT " + msng_OverVoltageLevel.ToString().Trim(), out res);

            PSCommandWr("VOLT:RANG P8V", out res);
            PSCommandWr("CURR:LEV:IMM:AMPL MAX", out res);

            if (!SetVoltage(0))
            {
                mstr_Error = "Error: Failed to set voltage: " + Convert.ToChar(13) + Convert.ToChar(10) + ReadError();
                return false;
            }

            if (!SelectOutput(PSPortEnum.PSOUT2))
            {
                mstr_Error = "Error: Failed to select output: " + Convert.ToChar(13) + Convert.ToChar(10) + ReadError();
                return false;
            }
            PSCommandWr("VOLT:PROT " + msng_OverVoltageLevel.ToString().Trim(), out res);
            PSCommandWr("VOLT:RANG P8V", out res);
            PSCommandWr("CURR:LEV:IMM:AMPL MAX", out res);
            PSCommandWr("VOLT:PROT:CLE", out res);
            if (!SetVoltage(0))
            {
                mstr_Error = "Error: Failed to set voltage: " + Convert.ToChar(13) + Convert.ToChar(10) + ReadError();
                return false;
            }

            //res = ReadError()
            //if (! res.Contains("No error")) {
            //    mstr_Error = "Agilent Error: "
            //    while (! res.Contains("No error")) //if an error occured read all errors in
            //        mstr_Error = mstr_Error + Convert.ToChar(13) + Convert.ToChar(10) + res
            //        res = ReadError()
            //    } while
            //    return false
            //}

            return true;
        }

        public bool SelectOutput(PSPortEnum psport)
        {

            string res = "";

            switch (psport)
            {
                case PSPortEnum.PSOUT1:
                    return PSCommandWr("INST:SEL OUT1", out res);
                    break;
                case PSPortEnum.PSOUT2:
                    return PSCommandWr("INST:SEL OUT2", out res);
                    break;

                default:
                    mstr_Error = "Error: Invalid Supply #";
                    return false;
                    break;
            }

            return false;
        }

        public bool OutputEnable(bool enaout)
        {
            string res = "";

            if (enaout == true) return PSCommandWr("OUTP:STAT ON", out res);
            return PSCommandWr("OUTP:STAT OFF", out res);

        }

        public bool SetVoltage(Single Level)
        {
            string res = "";

            if (Level >= msng_OverVoltageLevel)
            {
                return false;
            }
            msng_LastSetVoltageLevel = Level;

            return PSCommandWr("VOLT:LEV:IMM:AMPL " + msng_LastSetVoltageLevel.ToString().Trim(), out res);
        }

        public Single ReadVoltage()
        {
            string res = "";

            PSCommandRd("MEAS:VOLT:DC?", out res);
            if (res == "") return 0;

            return Convert.ToSingle(res);
        }

        public Single ReadCurrent()
        {
            string res = "";

            PSCommandRd("MEAS:CURR:DC?", out res);
            if (res == "") return 0;

            return Convert.ToSingle(res);
        }

        public bool Close()
        {
            if (m_ComPort != null)
            {
                OutputEnable(false);
                PortClose();
            }

            return true;
        }

    }
}
