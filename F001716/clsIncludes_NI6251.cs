using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F001716
{
    public class clsIncludes_NI6251
    {
        #region Struct

        public struct Line_t
        {
            //NiDioLine
            public string LineName;
            public int High;
            public int Low;
        }

        public struct PortLine_t
        {
            //NiDioPort0~2
            public Line_t [][]Port;

            //NiAnaInLine
            public string[] AnaInLine;

            //NiAnaOutLine
            public string[] AnaOutLine;

            //NiCounter
            public string[] Counter;
        }

        #endregion

        #region Variables

        private PortLine_t m_PortLine;
        private string m_daqDevice;

        #endregion

        #region "Properties"

        public String Device
        {
            set
            {
                m_daqDevice = value;
            }
        }

        public PortLine_t PortLine
        {
            get
            {
                return m_PortLine;
            }
        }

        #endregion

        #region Constructor

        public clsIncludes_NI6251()
        {
            m_PortLine = new PortLine_t();

            //NiDioPort
            m_PortLine.Port = new Line_t[3][];
            m_PortLine.Port[0] = new Line_t[8];
            m_PortLine.Port[1] = new Line_t[8];
            m_PortLine.Port[2] = new Line_t[8];

            //NiAnaInLine
            m_PortLine.AnaInLine = new string[16];

            //NiAnaOutLine
            m_PortLine.AnaOutLine = new string[2];

            //NiCounter
            m_PortLine.Counter = new string[2];

            m_daqDevice = "Dev1";
        }

        ~clsIncludes_NI6251()
        {

        }

        #endregion

        #region Public Function

        public void Init_Port_Line()
        {
            int i_PortIndex = 0;
            int i_LineIndex = 0;

            //NiDioPort0~2
            for (i_PortIndex = 0; i_PortIndex < 3; i_PortIndex++)
            {
                for (i_LineIndex = 0; i_LineIndex < 8; i_LineIndex++)
                {
                    m_PortLine.Port[i_PortIndex][i_LineIndex].LineName = m_daqDevice + "/port" + i_PortIndex.ToString() + "/line" + i_LineIndex.ToString();
                    m_PortLine.Port[i_PortIndex][i_LineIndex].High = 1 << i_LineIndex;
                    m_PortLine.Port[i_PortIndex][i_LineIndex].Low = 0;
                }
            }

            //NiAnaInLine
            for (i_LineIndex = 0; i_LineIndex < 16; i_LineIndex++)
            {
                m_PortLine.AnaInLine[i_LineIndex] = m_daqDevice + "/ai" + i_LineIndex.ToString();
            }

            //NiAnaOutLine
            for (i_LineIndex = 0; i_LineIndex < 2; i_LineIndex++)
            {
                m_PortLine.AnaOutLine[i_LineIndex] = m_daqDevice + "/ao" + i_LineIndex.ToString();
            }

            //NiCounter
            m_PortLine.Counter[0] = m_daqDevice + "/ctr0";
            m_PortLine.Counter[1] = m_daqDevice + "/ctr1";
        }

        #endregion
    }
}
