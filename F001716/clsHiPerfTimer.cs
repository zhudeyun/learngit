using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace F001716
{
    internal class clsHiPerfTimer
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long m_startTime, m_stopTime;
        private long m_freq;

        // Returns the duration of the timer (in seconds)
        public double Duration
        {
            get { return (double)(m_stopTime - m_startTime) / (double)m_freq; }
        }

        public clsHiPerfTimer()
        {
            m_startTime = 0;
            m_stopTime = 0;
            if (QueryPerformanceFrequency(out m_freq) == false)
            {
                //high-performance counter not supported
                throw new Exception("high-performance counter not supported");
            }
        }

        //Start the timer
        public void StartTimer()
        {
            //m_startTime = 0;
            //m_stopTime = 0;
            //lets do the waiting threads there work
            System.Threading.Thread.Sleep(0);
            QueryPerformanceCounter(out m_startTime);
        }

        // Stop the timer
        public void StopTimer()
        {
            QueryPerformanceCounter(out m_stopTime);
        }


    }
}
