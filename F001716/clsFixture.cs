using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace F001716
{
    public class clsFixture
    {
        public enum en_UP_Down
        {
            _up,
            _down
        }

        public enum en_On_Off
        {
            _on,
            _off
        }

        public enum en_3_State
        {
            _High,
            _Low,
            _Tristate
        }

        //private clsIncludes_NI6503.PortLine_t m_PortLine;
        private clsIncludes_NI6251.PortLine_t m_PortLine;
        //private clsIncludes_NI6259.PortLine_t m_PortLine;
        private clsDaqmx_NI6251 m_cls_DAQ;
        private string m_daqDevice;// = "Dev1";
        private string[] m_daqDevices;
        private bool m_daqInitialized;
        private string mstr_Error;

        public clsDaqmx_NI6251 dqSetDaq
        {
            set
            {
                m_cls_DAQ = value;
            }
        }

        public string Error
        {
            get { return mstr_Error; }
            set { mstr_Error = value; }
        }

        public clsFixture()
        {
        }

        ~clsFixture()
        {

        }

        public bool InitFixture()
        {
            try
            {
                m_PortLine = m_cls_DAQ.m_cls_NICard.PortLine;
                if (!m_daqInitialized)
                {
                    if (!(m_cls_DAQ.InitDaq()))
                        return false;
                }
                // all ports tristated after card reset
                RestoreFixture();
            }
            catch (Exception ex)
            {
                mstr_Error = "Error:Unknown error With DAQ Card #1 Initializing fixture.: " + "\x0d" + ex.Message;
                return false;
            }
            return true;
        }

        public bool CloseFixture()
        {
            try
            {
                // all ports tristated after card reset
                m_cls_DAQ.Close();
            }
            catch (Exception ex)
            {
                mstr_Error = "Error:Unknown error With DAQ Card #1 Initializing fixture.: " + "\x0d" + ex.Message;
                return false;
            }
            return true;
        }
        //****press button*****//
        public bool FixEnaPress(bool sel)
        {
            try
            {
                if (sel == true)
                {
                    // press on
                    m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][0].LineName, m_PortLine.Port[1][0].High);
                    m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][1].LineName, m_PortLine.Port[1][1].High);
                    clsUtils.Dly(1.0);
                }
                else
                {
                    // press off
                    m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][0].LineName, m_PortLine.Port[1][0].Low);
                    m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][1].LineName, m_PortLine.Port[1][1].Low);
                    clsUtils.Dly(0.3);
                }
                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;
        }

        //***  Turn on/off power *****//
        public bool PowerControl(bool sel)
        {
            try
            {
                if (sel == true)
                {
                    //m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][2].LineName, m_PortLine.Port[0][2].High);//power on
                    m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][0].LineName, m_PortLine.Port[0][0].High);//power on
                    clsUtils.Dly(1.0);
                }
                else
                {
                    // power off
                    //m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][2].LineName, m_PortLine.Port[0][2].Low);//power on
                    m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][0].LineName, m_PortLine.Port[0][0].Low);//power on
                    clsUtils.Dly(0.3);
                }
                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;
        }

        //***  Turn on/off power *****//
        public bool CableSel232()
        {
            try
            {

                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][6].LineName, m_PortLine.Port[0][6].Low);
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][7].LineName, m_PortLine.Port[0][7].High);
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][4].LineName, m_PortLine.Port[0][4].High);

                //close usb
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][1].LineName, m_PortLine.Port[0][1].Low);

                clsUtils.Dly(1.0);

                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;
        }

        public bool CableSelUSB()
        {
            try
            {

                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][6].LineName, m_PortLine.Port[0][6].High);
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][7].LineName, m_PortLine.Port[0][7].High);
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][1].LineName, m_PortLine.Port[0][1].High);

                //close 232
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][4].LineName, m_PortLine.Port[0][4].Low);

                clsUtils.Dly(1.0);

                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;
        }

        public bool ReadCurrent(ref double current,int count)
        {
            // Read AI1 
            double[] current_temp;
            double value = 0;
            current = 0;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    current_temp = m_cls_DAQ.ReadAnalogChannelRse(m_PortLine.AnaInLine[1]);
                    value =value+ current_temp[0];
                }
                    clsUtils.Dly(1.0);
              
                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
                current = value / count;
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;

        }

        public bool ReadAI(string AiName, ref double AiValue, int ReadCount)
        {
            double[] AiValue_temp;
            double value = 0;
            int AiNB = 0;
            try
            {
                // powen on
                AiName = AiName.Replace("AI", "");
                AiNB = Convert.ToInt32(AiName);
                for (int i = 0; i < ReadCount; i++)
                {
                    AiValue_temp = m_cls_DAQ.ReadAnalogChannelRse(m_PortLine.AnaInLine[AiNB]);//.WriteDigLine(m_PortLine.Port[0][0].LineName, m_PortLine.Port[0][0].High);
                    value = value + AiValue_temp[0];
                }
                clsUtils.Dly(1.0);

                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
                AiValue = value / ReadCount;
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;
        }

        public bool ReadVoltage(ref double Voltage, int count)
        {
            //Read AI0
            double[] Voltage_temp;
            double value=0;
            try
            {
                // powen on
                for (int i = 0; i < count; i++)
                {
                    Voltage_temp = m_cls_DAQ.ReadAnalogChannelRse(m_PortLine.AnaInLine[0]);//.WriteDigLine(m_PortLine.Port[0][0].LineName, m_PortLine.Port[0][0].High);
                    value=value+Voltage_temp[0];
                }
                clsUtils.Dly(1.0);
                
                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return false;
                }
                Voltage = value/count;
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return true;

        }

        public bool ReseSLM_RS232()
        {
            m_cls_DAQ.WriteDigLine(m_PortLine.Port[2][0].LineName, m_PortLine.Port[2][0].High);
            clsUtils.Dly(0.2);
            m_cls_DAQ.WriteDigLine(m_PortLine.Port[2][0].LineName, m_PortLine.Port[2][0].Low);
            clsUtils.Dly(0.2);
            return true;
        }

        public bool APLPanel(bool enable)
        {
            if(enable==true)
            {
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][0].LineName, m_PortLine.Port[1][0].High);
                clsUtils.Dly(0.2);
            }
            else
            {
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][0].LineName, m_PortLine.Port[1][0].Low);
                clsUtils.Dly(0.2);
            }
            
            return true;
        }

        public bool CameraPanel(bool enable)
        {
            if (enable == true)
            {
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][1].LineName, m_PortLine.Port[1][1].High);
                clsUtils.Dly(0.2);
            }
            else
            {
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[1][1].LineName, m_PortLine.Port[1][1].Low);
                clsUtils.Dly(0.2);
            }

            return true;
        }

        public bool ReseSLM_MaxHold()
        {
            m_cls_DAQ.WriteDigLine(m_PortLine.Port[2][7].LineName, m_PortLine.Port[2][7].High);
            clsUtils.Dly(0.2);
            m_cls_DAQ.WriteDigLine(m_PortLine.Port[2][7].LineName, m_PortLine.Port[2][7].Low);
            clsUtils.Dly(0.2);
            return true;
        }

        public void RestoreFixture()
        {
            try
            {
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][0].LineName, m_PortLine.Port[0][0].Low);//( P0_2 High ,3.3V)
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][2].LineName, m_PortLine.Port[0][2].Low);//( P0_2 High ,3.3V)
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][1].LineName, m_PortLine.Port[0][1].Low);//Low for USB off
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][4].LineName, m_PortLine.Port[0][4].Low);//High to enable RS232
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][6].LineName, m_PortLine.Port[0][6].Low);//cable_sel GND.
                m_cls_DAQ.WriteDigLine(m_PortLine.Port[0][7].LineName, m_PortLine.Port[0][7].Low);//cable_sel GND.

                if (m_cls_DAQ.dqErr != "")
                {
                    mstr_Error = m_cls_DAQ.dqErr;
                    return ;
                }
            }
            catch (Exception err)
            {
                mstr_Error = err.Message;
            }
            return ;
        }
    }
}
