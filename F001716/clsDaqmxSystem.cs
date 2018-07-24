using System;
using System.Collections.Generic;
using System.Text;
using NationalInstruments.DAQmx;

namespace F001716
{
    class clsDaqmxSystem
    {
        //<StructLayout(LayoutKind.Sequential)> _
        public struct PhysicalDevice_t
        {
            public string[] m_AIChannels;
            public string[] m_AOChannels;
            public string[] m_CIChannels;
            public string[] m_COChannels;
            public string[] m_DILineChannels;
            public string[] m_DIPortChannels;
            public string[] m_DOLineChannels;
            public string[] m_DOPortChannels;
        }

        public static DaqSystem m_System;
        private static string[] m_Devices;
        private static Device[] m_Device = null;
        private static int m_NumDevices;
        public static PhysicalDevice_t m_PhysicalDevice;

        public static string[] dqAIChannels
        {
            get { return m_PhysicalDevice.m_AIChannels; }
        }

        public static string[] dqAOChannels
        {
            get { return m_PhysicalDevice.m_AOChannels; }
        }

        public static string[] dqCIChannels
        {
            get { return m_PhysicalDevice.m_CIChannels; }
        }

        public static string[] dqCOChannels
        {
            get { return m_PhysicalDevice.m_COChannels; }
        }

        public static string[] dqDILineChannels
        {
            get { return m_PhysicalDevice.m_DILineChannels; }
        }

        public static string[] dqDIPortChannels
        {
            get { return m_PhysicalDevice.m_DIPortChannels; }
        }

        public static string[] dqDOLineChannels
        {
            get { return m_PhysicalDevice.m_DOLineChannels; }
        }

        public static string[] dqDOPortChannels
        {
            get { return m_PhysicalDevice.m_DOPortChannels; }
        }

        public static string[] Init()
        {
            m_System = DaqSystem.Local;
            m_Devices = m_System.Devices;
            m_NumDevices = m_Devices.Length;
            m_PhysicalDevice.m_AIChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_AOChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_CIChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_COChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.CO, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_DILineChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.DILine, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_DIPortChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.DIPort, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_DOLineChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);
            m_PhysicalDevice.m_DOPortChannels = m_System.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External);

            //ReDim m_Device(m_NumDevices - 1)
            //Dim id(m_NumDevices - 1) As String
            Device[] m_Device = new Device[m_NumDevices];
            string[] id = new string[m_NumDevices];
            int i = 0;
            for (i=0;i<m_NumDevices;i++)
            {
                m_Device[i] = m_System.LoadDevice(m_Devices[i]);
                //m_Device[i].Reset();
                id[i] = m_Device[i].DeviceID;
            }
            return id;
        }

        public static void Reset(string deviceId)
        {
            //if we get nothing then reset all devices on the system
            //otherwise reset on the device we are told
            int i = 0;
            if (deviceId == "")
            {
                for (i=0;i<m_NumDevices - 1;i++)
                    m_Device[i].Reset();
            }
            else
            {
                for (i=0;i<m_NumDevices - 1;i++)
                    if (m_Device[i].DeviceID == deviceId)
                        m_Device[i].Reset();
            }
        }
    }
}
