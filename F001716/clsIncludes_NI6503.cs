using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F001716
{
    public class clsIncludes_NI6503
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
            //NiDioPortA~C
            public Line_t [][]Port;
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

        public clsIncludes_NI6503()
        {
            m_PortLine = new PortLine_t();

            //NiDioPort
            m_PortLine.Port = new Line_t[3][];
            m_PortLine.Port[0] = new Line_t[8];
            m_PortLine.Port[1] = new Line_t[8];
            m_PortLine.Port[2] = new Line_t[8];

            m_daqDevice = "Dev1";
        }

        ~clsIncludes_NI6503()
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
        }

        #endregion
    }
}
