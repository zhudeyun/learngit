using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace F001716
{
    class clsUtilityTimer
    {
        private Timer m_Timer;
        public delegate void TimerElapsed(int index);
        public event TimerElapsed ev_TimerElapsed;

        private int m_index;

        public int utIndex
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public double utInterval
        {
            get { return m_Timer.Interval; }
            set { m_Timer.Interval = value; }
        }

        public bool utEnabled
        {
            get { return m_Timer.Enabled; }
            set { m_Timer.Enabled = value; }
        }

        public clsUtilityTimer()
        {
            m_Timer = new System.Timers.Timer();
            //AddHandler m_Timer.Elapsed, AddressOf OnTimer;
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimer);
            m_Timer.Enabled = false;
        }

        ~clsUtilityTimer()
        {
            if (m_Timer == null)
                return;
            else
            {
                m_Timer.Enabled = false;
                //RemoveHandler m_Timer.Elapsed, AddressOf OnTimer;
                m_Timer.Elapsed -= new ElapsedEventHandler(OnTimer);
                m_Timer = null;
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            ev_TimerElapsed(m_index);
        }


    }
}
