using System;
using System.Collections.Generic;
using System.Text;
using NationalInstruments;
using NationalInstruments.DAQmx;
using System.Diagnostics;
using System.Windows.Forms;

namespace F001555
{
    public class clsDaqmx
    {
        public DaqSystem m_System;
        private string[] m_Devices;
        private Device[] m_Device = null;
        private int m_NumDevices;
        private int[] m_DigitalShadow = null;

        public clsIncludes_NI6503 m_cls_NICard;
        private string m_daqDevice = "";
        private string[] m_daqDevices;
        private int m_daqDeviceNumber = 0;
        private bool m_daqInitialized;

        private double m_d_MeasuredFrequency;
        private string mstr_Error = "";

        private Task Task_MeasureLowFreq;
        private CounterReader myCounterReader;
        private AsyncCallback myCallBack;

        //private m_digitalReadTask As Task
        //private m_digitalWriteTask As Task

        public double MeasuredFrequency
        {
            get
            {
                return m_d_MeasuredFrequency;
            }
        }

        public string dqErr
        {
            get { return mstr_Error; }
            set { mstr_Error = value; }
        }

        public clsDaqmx()
        {
            m_System = DaqSystem.Local;
            m_Devices = m_System.Devices;
            m_NumDevices = m_Devices.Length;
            m_cls_NICard = new clsIncludes_NI6503();

            m_daqInitialized = false;
            mstr_Error = "";

            //if (device != "")
            m_daqDevice = "";//device;

        }

        ~clsDaqmx()
        {
            if (m_Device[m_daqDeviceNumber] != null)
            {
                try
                {
                    m_Device[m_daqDeviceNumber].Reset();
                    m_Device[m_daqDeviceNumber] = null;
                }
                catch (Exception e)
                {
                }
            }

            this.Close();
        }

        public void DaqException(string ex)
        {
            System.Windows.Forms.MessageBox.Show(ex);
        }

        private void DaqError(string ex)
        {
            System.Windows.Forms.MessageBox.Show(ex, "Daq Exception");
        }

        private void CheckLines(ref bool[] dataArray)
        {
            dataArray[0] = true;
            dataArray[1] = false;
            dataArray[2] = true;
            dataArray[3] = false;
            dataArray[4] = true;
            dataArray[5] = false;
            dataArray[6] = true;
            dataArray[7] = false;
        }

        public string[] FindDevices()
        {
            m_Device = new Device[m_NumDevices];
            string[] id = new string[m_NumDevices];
            int i;
            for (i = 0; i < m_NumDevices; i++)
            {
                m_Device[i] = m_System.LoadDevice(m_Devices[i]);
                //m_Device[i].Reset();
                id[i] = m_Device[i].DeviceID;
            }
            return id;
        }

        public void Init(string device, int deviceNum)
        {
            m_daqDevice = device;
            m_daqDeviceNumber = deviceNum;
        }

        //***********************************************
        //Purpose : Initializes the DAQ Card 
        //Inputs  : None
        //Sets    : The state of the Initial state of the Daq io lines
        //returns : A bool indicating Pass.
        //***********************************************
        public bool InitDaq()
        {
            bool foundDevice = false;
            int j;

            try
            {
                if (!m_daqInitialized)
                {
                    string str_Path;
                    string str_Res;
                    ReadIniSettings RdSettings;

                    RdSettings = new ReadIniSettings();
                    str_Path = Application.StartupPath + "\\" + Program.gstr_Software_Number + ".ini";
                    if (!System.IO.File.Exists(str_Path))
                    {
                        mstr_Error = "Error:Can not locate " + str_Path;
                        return false;
                    }

                    //National Instruments Device
                    str_Res = RdSettings.ReadPrivateProfileStringKey("National Instruments", "device", str_Path);
                    if (str_Res == "Error")
                    {
                        mstr_Error = "Error: With NI_PCI_6259 Device# setting";
                        return false;
                    }
                    m_daqDevice = str_Res.ToString();

                    //Read all the Daq devices into a string array
                    m_daqDevices = FindDevices();
                    for (j = 0; j <= m_daqDevices.GetUpperBound(0); j++)
                    {
                        if (m_daqDevices[j] == m_daqDevice)
                        {
                            foundDevice = true;
                            break;
                        }
                    }
                    //if (m_daqDevices[0] != m_daqDevice)
                    if (foundDevice == false)
                    {
                        mstr_Error = "Error: Unable to Find Ni card" + m_daqDevice + ".";
                        return false;
                    }
                    Init(m_daqDevices[j], j);

                    //Init NI card all port
                    m_cls_NICard.Device = m_daqDevice;
                    m_cls_NICard.Init_Port_Line();
                }
            }
            catch (Exception ex)
            {
                mstr_Error = "Error: Unable to Find Ni card Dev #1.: " + "\x0d" + ex.ToString();
                return false;
            }

            m_daqInitialized = true;
            return true;
        }

        public bool Reset(string deviceId)
        {
            //if we get nothing then reset all devices on the system
            //otherwise reset on the device we are told
            //int i;
            //if (deviceId == "")
            //{
            //    for (i = 0; i < m_NumDevices; i++)
            //    {
            //        if (m_Device[i] != null)
            //            m_Device[i].Reset();
            //    }
            //}
            //else
            //{
            //}
            try
            {
                m_Device[m_daqDeviceNumber].Reset();
                m_Device[m_daqDeviceNumber] = null;
            }
            catch (Exception e)
            {
            }
            return true;
        }

        public void Close()
        {
            Reset(""); //Should tristate all lines
        }

        public void WriteDigitialPulse(string counter, COPulseIdleState iState, double frequency, double dutyCycle)
        {    
            // This example uses the default source (or gate) terminal for 
            // the counter of your device.  To determine what the default 
            // counter pins for your device are or to set a different source 
            // (or gate) pin, refer to the Connecting Counter Signals topic
            // in the NI-DAQmx Help (search for "Connecting Counter Signals").

            Task myTask = new Task();
            COChannel CO;
            try
            {
                CO = myTask.COChannels.CreatePulseChannelFrequency(counter, "ContinuousPulseTrain", COPulseFrequencyUnits.Hertz, iState, 0.0, frequency, dutyCycle);
                myTask.Timing.ConfigureImplicit(SampleQuantityMode.ContinuousSamples, 1000);

                myTask.Start();
                myTask.Stop();
            }
            catch (System.Exception ex) {
                DaqError(ex.Message);
            }
            finally {
                myTask.Dispose();
            }
        }

        public double ReadDigitalPulseWidth(string counter, CIPulseWidthStartingEdge StartingEdge, double minPW_InSeconds, double maxPW_InSeconds)
        {
        // This example uses the default source (or gate) terminal for 
        // the counter of your device.  To determine what the default 
        // counter pins for your device are or to set a different source 
        // (or gate) pin, refer to the Connecting Counter Signals topic
        // in the NI-DAQmx Help (search for "Connecting Counter Signals").
        // Uses default PFI 9/P2.1 as the pulse input.
        // Returns PW in seconds.

        double pulseWidth = 0;
        Task myTask = null;
        NationalInstruments.DAQmx.CounterReader myCounterReader;
       
        try
        {
            myTask = new Task();
            myTask.CIChannels.CreatePulseWidthChannel(counter, 
                "Meas Pulse Width", minPW_InSeconds,
                maxPW_InSeconds, StartingEdge,
                CIPulseWidthUnits.Seconds);
            //RLC Wait up to 5 sec default was 10 sec
            myTask.Stream.Timeout = 5000;
            myCounterReader = new CounterReader(myTask.Stream);

           // For .NET Framework 2.0 or later, use SynchronizeCallbacks to specify that the object 
           // marshals callbacks across threads appropriately.
           myCounterReader.SynchronizeCallbacks = true;
           // For .NET Framework 1.1, set SynchronizingObject to the Windows Form to specify 
           // that the object marshals callbacks across threads appropriately.
           //myCounterReader.SynchronizingObject = this;


            //rlc AsyncCallback myCallback = new AsyncCallback(MeasurementCallback);
            //myCounterReader.BeginReadSingleSampleDouble(myCallback, null);
            IAsyncResult ar = null;
            ar = myCounterReader.BeginReadSingleSampleDouble(null, null);

            pulseWidth = myCounterReader.EndReadSingleSampleDouble(ar);
        }
        catch (Exception exception)
        {
            mstr_Error = exception.Message;
            pulseWidth = -1;
        }
        finally
        {
            if (myTask != null)
                myTask.Dispose();
        }

        return pulseWidth;
    }

        private void CounterInCallback(IAsyncResult ar)
        {
            // Read the measured value
            try
            {
                m_d_MeasuredFrequency = myCounterReader.EndReadSingleSampleDouble(ar);
            }
            catch (DaqException exception)
            {
                mstr_Error = exception.Message;
            }
            finally
            {
                Task_MeasureLowFreq.Dispose();
            }
        }

        public bool ReadDigitalLowFrequency(string lines, string name, double min, double max, double time)
        {
            // This example uses the default source (or gate) terminal for 
            // the counter of your device.  To determine what the default 
            // counter pins for your device are or to set a different source 
            // (or gate) pin, refer to the Connecting Counter Signals topic
            // in the NI-DAQmx Help (search for "Connecting Counter Signals").
            // Uses default PFI 9/P2.1 as the pulse input.
            try
            {
                m_d_MeasuredFrequency = 0;

                Task_MeasureLowFreq = new Task();

                Task_MeasureLowFreq.CIChannels.CreateFrequencyChannel(lines, name, min, max, CIFrequencyStartingEdge.Rising,
                    CIFrequencyMeasurementMethod.LowFrequencyOneCounter, time, 4, CIFrequencyUnits.Hertz);

                myCounterReader = new CounterReader(Task_MeasureLowFreq.Stream);

                // For .NET Framework 2.0 or later, use SynchronizeCallbacks to specify that the object 
                // marshals callbacks across threads appropriately.
                myCounterReader.SynchronizeCallbacks = true;

                myCallBack = new AsyncCallback(CounterInCallback);
                myCounterReader.BeginReadSingleSampleDouble(myCallBack, null);
            }
            catch (Exception exception)
            {
                mstr_Error = exception.Message;
                Task_MeasureLowFreq.Dispose();

                return false;
            }

            return true;
        }

        public double Meas2EdgeSeparation(string lines, double min, double max, CITwoEdgeSeparationFirstEdge firstEdge, CITwoEdgeSeparationSecondEdge secondEdge, int Timeout)
        {
            // This example uses the default source (or gate) terminal for 
            // the counter of your device.  To determine what the default 
            // counter pins for your device are or to set a different source 
            // (or gate) pin, refer to the Connecting Counter Signals topic
            // in the NI-DAQmx Help (search for "Connecting Counter Signals").
            // Uses default PFI 9/P2.1 as the pulse input.

            double pulseGap = 0;
            Task myTask = null;
            NationalInstruments.DAQmx.CounterReader myCounterReader;

            try
            {
                myTask = new Task();

                myTask.Stream.Timeout = Timeout;

                myTask.CIChannels.CreateTwoEdgeSeparationChannel(
                    lines, "", min, max,
                    firstEdge, secondEdge, CITwoEdgeSeparationUnits.Seconds);

                myCounterReader = new CounterReader(myTask.Stream);
                pulseGap = myCounterReader.ReadSingleSampleDouble();
            }
            catch (DaqException exception)
            {
                mstr_Error = exception.Message;
                pulseGap = 0;
            }
            finally
            {
                if (myTask != null)
                    myTask.Dispose();
            }

            return pulseGap;
        }

        public int ReadDigChannel(string lines, string name)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            Task digitalReadTask = new Task();
            int val = 0;

            try
            {
                //Create channel
                digitalReadTask.DIChannels.CreateChannel(lines, "DigRead", ChannelLineGrouping.OneChannelForEachLine);

                bool[] data;
                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);

                digitalReadTask.Start();
                data = reader.ReadSingleSampleMultiLine();
                digitalReadTask.Stop();


                for (int index=0;index<data.Length;index++)
                {    
                    if (data[index] == true)
                    {
                        //if bit is true
                        //add decimal value of bit
                        val += 1 << index;
                    }

                }
                //Debug.Print(String.Format(lines + ": " + "0x{0:X}", val));
                //return val;
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
            }
            finally {
                //dispose task
                digitalReadTask.Dispose();
            }
            return val;
        }

        public int WriteDigChannel2(string lines, string name, uint val, uint mask)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            int res = 0;
            Task digitalWriteTask = new Task();

            try
            {
                //Create channel
                digitalWriteTask.DOChannels.CreateChannel(lines, "port0", ChannelLineGrouping.OneChannelForEachLine);

                bool[] dataArray = new bool[7];
                CheckLines(ref dataArray);
                
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                digitalWriteTask.Start();
                writer.WriteSingleSampleMultiLine(false, dataArray);
                digitalWriteTask.Stop();
                //UpdateDigitalShadows(lines, (int)val);

            }
            catch (DaqException ex)
            {
                DaqError(ex.Message);
                res = -1;
            }
            finally {
                //dispose task
                digitalWriteTask.Dispose();
            }
            return res;
        }

        public uint ReadDigPort(string lines, string name)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            Task digitalReadTask = new Task();
            uint data = 0;

            try
            {
                //Create channel
                digitalReadTask.DIChannels.CreateChannel(lines, "port0", ChannelLineGrouping.OneChannelForAllLines);

                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);

                digitalReadTask.Start();
                data = reader.ReadSingleSamplePortUInt32();
                digitalReadTask.Stop();
                Debug.Print(String.Format("0x{0:X}", data));
                //return data;
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
            }
            finally {
                //dispose task
                digitalReadTask.Dispose();
            }
            return data;
        }

        public void WriteDigPortLine(string lines, string name, int val)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            Task digitalWriteTask = new Task();
            DOChannel ch;

            try
            {
                //Create a Digital Output channel and name it.
                ch = digitalWriteTask.DOChannels.CreateChannel(lines, "line0", ChannelLineGrouping.OneChannelForAllLines);

                //get the states of the lines before we get started
                //Verify the Task
                //m_digitalWriteTask.Control(TaskAction.Verify)
                //Dim st As DOLineStatesStartState
                //st = ch.LineStatesStartState()
                //Dim data As UInt32
                //Dim reader As DigitalSingleChannelReader = New DigitalSingleChannelReader(digitalReadTask.Stream)

                //digitalReadTask.Start()
                //data = reader.ReadSingleSamplePortUInt32()
                //digitalReadTask.Stop()

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);

                DaqBuffer buf;
                buf = digitalWriteTask.Stream.Buffer;
                digitalWriteTask.Start();
                //st = ch.LineStatesStartState
                writer.WriteSingleSamplePort(false, val);
                //st = ch.LineStatesStartState
                digitalWriteTask.Stop();
                buf = digitalWriteTask.Stream.Buffer;
                //UpdateDigitalShadows(lines, (int)val);
            }
            catch (System.Exception ex) {
                DaqError(ex.Message);
            }
            finally {
                //dispose task
                digitalWriteTask.Dispose();
            }
        }

        public int WriteDigChannel(string lines, string name, int val)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            int res = 0;
            Task digitalWriteTask = new Task();
            DOChannel ch;

            try
            {
                //Create channel
                ch = digitalWriteTask.DOChannels.CreateChannel(lines, "port0", ChannelLineGrouping.OneChannelForEachLine);

                //Dim st As DOLineStatesStartState
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalWriteTask.Stream);

                bool[] data;
                data = reader.ReadSingleSampleMultiLine();

                digitalWriteTask.Start();
                //st = ch.LineStatesStartState
                if (ch.Tristate == true)
                    ch.Tristate = false;
                writer.WriteSingleSamplePort(false, val);
                digitalWriteTask.Stop();
                //UpdateDigitalShadows(lines, (int)val);
            }
            catch (DaqException ex)
            {
                DaqError(ex.Message);
                res = -1;
            }
            finally {
                //dispose task
                digitalWriteTask.Dispose();
            }
            return res;
        }

        public int WriteDigChannelGroup(string lines, string name, uint val)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            int res = 0;
            Task digitalWriteTask = new Task();
            DOChannel ch;

            try
            {
                //Create channel
                ch = digitalWriteTask.DOChannels.CreateChannel(lines, "digwrite", ChannelLineGrouping.OneChannelForAllLines);

                //Dim st As DOLineStatesStartState
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalWriteTask.Stream);

                bool[] data;
                data = reader.ReadSingleSampleMultiLine();

                digitalWriteTask.Start();
                //st = ch.LineStatesStartState
                if (ch.Tristate == true)
                    ch.Tristate = false;
                writer.WriteSingleSamplePort(false, val);
                digitalWriteTask.Stop();
                //UpdateDigitalShadows(lines, CInt(val));
            }
            catch (DaqException ex)
            {
                DaqError(ex.Message);
                res = -1;
            }
            finally
            {
                //dispose task
                digitalWriteTask.Dispose();
            }
            return res;
        }

        public int ReadAnalogChannel(string lines, string name, AITerminalConfiguration config , ref double[] channelData, double min, double max)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            Task analogReadTask = new Task();
            AIChannel ch = null;
            //channelData = new double[analogReadTask.AIChannels.Count];
            //double[] channelData2 = new double[analogReadTask.AIChannels.Count];
            double[] channelData2 = null;

            if (max == 0)
                max = 10;

            try
            {
                //If min > ai.Minimum And max < ai.Maximum Then
                //Create a virtual channel
                ch = analogReadTask.AIChannels.CreateVoltageChannel(lines, "", config, min, max, AIVoltageUnits.Volts);

                //Verify the Task
                analogReadTask.Control(TaskAction.Verify);
                //InitializeDataTable(myTask.AIChannels, DataTable)

                channelData = new double[analogReadTask.AIChannels.Count];
                channelData2 = new double[analogReadTask.AIChannels.Count];

                AnalogMultiChannelReader reader = new AnalogMultiChannelReader(analogReadTask.Stream);

                analogReadTask.Start();
                channelData2 = reader.ReadSingleSample();
                analogReadTask.Stop();

                for (int i = 0; i < analogReadTask.AIChannels.Count; i++ )
                {
                    Debug.Print(channelData2[0].ToString("#0.00"));
                }
                //return 0;
                //Else

                //End If

                //Update the Acquired Sample Table
                //dataToDataTable(data, DataTable)
                //acquisitionDataGrid.DataSource = DataTable
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
            }   
            finally {
                //analogReadTask.Dispose();
                Array.Copy(channelData2, channelData, analogReadTask.AIChannels.Count);
                channelData = channelData2;
            }
            return 0;
        }

        public int ReadAnalogChannelConvertToDigital(string lines, string name, AITerminalConfiguration config, ref double[] channelData, double min, double max)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            int val = 0;
            Task analogReadTask = new Task();
            AIChannel ch = null;
            //channelData = new double[analogReadTask.AIChannels.Count];
            //double[] channelData2 = new double[analogReadTask.AIChannels.Count];
            //double[] channelData2 = null;

            if (max == 0)
                max = 10;

            try
            {
                //If min > ai.Minimum And max < ai.Maximum Then
                //Create a virtual channel
                ch = analogReadTask.AIChannels.CreateVoltageChannel(lines, "", config, min, max, AIVoltageUnits.Volts);

                //Verify the Task
                analogReadTask.Control(TaskAction.Verify);
                //InitializeDataTable(myTask.AIChannels, DataTable)

                channelData = new double[analogReadTask.AIChannels.Count];
                //channelData2 = new double[analogReadTask.AIChannels.Count];

                AnalogMultiChannelReader reader = new AnalogMultiChannelReader(analogReadTask.Stream);

                analogReadTask.Start();
                channelData = reader.ReadSingleSample();
                analogReadTask.Stop();

                //Debug.Print(String.Format("0x{0:X}", channelData));
                for (int i = 0; i < analogReadTask.AIChannels.Count; i++)
                {
                    Debug.Print(channelData[0].ToString("#0.00"));
                }

                for (int i = 0; i < channelData.Length; i++)
                {
                    if (channelData[i] > 2.0)
                    {
                        //if bit is > 2.0V (logical high) add decimal value of bit
                        val += 1 << i;
                    }
                }
                //return 0;
                //Else

                //End If

                //Update the Acquired Sample Table
                //dataToDataTable(data, DataTable)
                //acquisitionDataGrid.DataSource = DataTable
            }
            catch (DaqException ex)
            {
                DaqError(ex.Message);
            }
            //finally
            //{
            //    //analogReadTask.Dispose();
            //    Array.Copy(channelData2, channelData, analogReadTask.AIChannels.Count);
            //    channelData = channelData2;
            //}
            return val;
        }

        public int ReadWaveformAnalogChannel(string lines, string name, AITerminalConfiguration config, ref AnalogWaveform<double>[] channelData, double min, double max, int nsamples)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            Task analogReadTask = new Task();
            AIChannel ch = null;
            AnalogWaveform<double>[] channelData2 = new AnalogWaveform<double>[analogReadTask.AIChannels.Count];

            if (max == 0)
                max = 10;
            if (nsamples == 0)
                nsamples = -1;

            try
            {
                //If min > ai.Minimum And max < ai.Maximum Then
                //Create a virtual channel
                ch = analogReadTask.AIChannels.CreateVoltageChannel(lines, "", config, min, max, AIVoltageUnits.Volts);

                //Verify the Task
                analogReadTask.Control(TaskAction.Verify);
                //InitializeDataTable(myTask.AIChannels, DataTable)

                AnalogMultiChannelReader reader = new AnalogMultiChannelReader(analogReadTask.Stream);

                analogReadTask.Start();
                channelData2 = reader.ReadWaveform(nsamples);
                analogReadTask.Stop();

                Debug.Print(String.Format("0x{0:X}", channelData2));
                return 0;
                //Else

                //End If

                //Update the Acquired Sample Table
                //dataToDataTable(data, DataTable)
                //acquisitionDataGrid.DataSource = DataTable
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
            }
            finally {
                analogReadTask.Dispose();
                Array.Copy(channelData2, channelData, analogReadTask.AIChannels.Count);
                channelData = channelData2;
            }
            return 0;
        }

        public void TristateDigChannel(string lines, string name)
        {
            //Create a task such that it will be disposed after
            //we are done using it.
            Task digitalWriteTask = new Task();
            DOChannel ch;

            try
            {
                //Create channel
                ch = digitalWriteTask.DOChannels.CreateChannel(lines, "port0", ChannelLineGrouping.OneChannelForEachLine);

                digitalWriteTask.Start();
                ch.Tristate = true;
                digitalWriteTask.Stop();
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
            }
            finally {
                //dispose task
                digitalWriteTask.Dispose();
            }
        }

        public void WriteWaveformAnalogChannel(string lines, string name, int freq, int samples, int cycles, string sigtype, double ampl, double min, double max)
        {
            Task analogWriteTask = new Task();
            AOChannel ch;
            if (max == 0)
                max = 10;

            try
            {
                // Create the task and channel
                //myTask = New Task()
                ch = analogWriteTask.AOChannels.CreateVoltageChannel(lines, "", min, max, AOVoltageUnits.Volts);

                // Verify the task before doing the waveform calculations
                analogWriteTask.Control(TaskAction.Verify);

                // Calculate some waveform parameters and generate data
                clsFunctionGenerator fGen = new clsFunctionGenerator(analogWriteTask.Timing, freq.ToString(), samples.ToString(), cycles.ToString(), sigtype, ampl.ToString());

                // Configure the sample clock with the calculated rate
                analogWriteTask.Timing.ConfigureSampleClock("", fGen.fgResultingSampleClockRate, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

                // Write the data to the buffer
                AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(analogWriteTask.Stream);
                writer.WriteMultiSample(false, fGen.fgData);
                IAsyncResult res;
                res = writer.BeginWriteMultiSample(false, fGen.fgData, _WriteAnalogChannelComplete, 0);

                //Start writing out data
                analogWriteTask.Start();
                analogWriteTask.Stop();
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
            }
            finally {
                analogWriteTask.Dispose();
            }
        }

        public int WriteAnalogChannel(string lines, string name, int freq, int samples, int cycles, string sigtype, double ampl, double min, double max)
        {
            int res = 0;
            Task analogWriteTask = new Task();
            AOChannel ch;
            
            if (max == 0)
                max = 10;

            try
            {
                // Create the task and channel
                //myTask = New Task()
                ch = analogWriteTask.AOChannels.CreateVoltageChannel(lines, "", min, max, AOVoltageUnits.Volts);

                // Verify the task before doing the waveform calculations
                analogWriteTask.Control(TaskAction.Verify);

                // Calculate some waveform parameters and generate data
                //Dim fGen As New FunctionGenerator( _
                //    analogWriteTask.Timing, _
                //    freq.ToString, _
                //    samples.ToString, _
                //    cycles.ToString, _
                //    sigtype, _
                //    ampl.ToString)

                //Configure the sample clock with the calculated rate
                //analogWriteTask.Timing.ConfigureSampleClock( _
                //    "", _
                //    fGen.ResultingSampleClockRate, _
                //    SampleClockActiveEdge.Rising, _
                //    SampleQuantityMode.ContinuousSamples, 1000)

                // Write the data to the buffer
                AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(analogWriteTask.Stream);
                //writer.WriteMultiSample(False, fGen.Data)
                //Dim res As IAsyncResult
                //res = writer.BeginWriteMultiSample(False, fGen.Data, AddressOf _WriteAnalogChannelComplete, 0)
                analogWriteTask.Start();
                writer.WriteSingleSample(false, 2.5);

                //Start writing out data
                //analogWriteTask.Start()
                analogWriteTask.Stop();
            }
            catch (DaqException ex) {
                DaqError(ex.Message);
                res = -1;
            }
            finally {
                analogWriteTask.Dispose();
            }
            return res;
        }

        //public DOChannel CreateDigitalField(string lines, string name)
        //{
        //    Task digitalWriteTask = new Task();
        //    DOChannel ch;
        //    //Create channel
        //    ch = digitalWriteTask.DOChannels.CreateChannel(lines, "port0", ChannelLineGrouping.OneChannelForAllLines);
        //    return ch;
        //}

        //private void CreateDigitalShadows(int ports)
        //{
        //    int[] m_DigitalShadow = new int[ports];

        //    for (int i = 0; i < ports - 1; i++)
        //        m_DigitalShadow[i] = 0;
        //}

        //private void UpdateDigitalShadows(string lines, int mask)
        //{
        //    int i;
        //    if (lines.ToLower().Contains("port0"))
        //        i = 0;
        //    else if (lines.ToLower().Contains("port1"))
        //        i = 1;
        //    else if (lines.ToLower().Contains("port2"))
        //        i = 2;
        //    else
        //        throw new DaqException("Invalid digital shadow port: " + lines, -99);
        //    m_DigitalShadow[i] += mask;
        //    m_DigitalShadow[i] = m_DigitalShadow[i] & (0xFF ^ mask);
        //}

        private void _WriteAnalogChannelComplete(IAsyncResult ar)
        {
            int a = 1;
        }

        // ***********************************************************************
        // * below functions are for generate use which moved from Fixture class
        // ***********************************************************************
        #region Generate Functions

        public void WriteDigLine(string lines, int portVal)
        {
            WriteDigPortLine(lines, "", portVal);
            clsUtils.Dly(0.1);
        }

        public void WriteDigPort(string lines, int portVal)
        {
            WriteDigChannel(lines, "", portVal);
            clsUtils.Dly(0.1);
        }

        public int ReadDigLine(string lines)
        {
            int val;

            clsUtils.Dly(0.1);
            val = ReadDigChannel(lines, "");

            return val;
        }

        public double[] ReadAnalogChannelRse(string lines)
        {
            //read analog channel and return
            int res;
            //int val = 0;
            double[] value = null; //= new double[1];
            res = ReadAnalogChannel(lines, "Rse Analog Channel", AITerminalConfiguration.Rse, ref value, -10, 10);

            return value;
        }

        public double[] ReadAnalogChannelDiff(string lines)
        {
            //read analog channel and return
            int res;
            //int val = 0;
            double[] value = null; //= new double[1];
            res = ReadAnalogChannel(lines, "Diff Analog Channel", AITerminalConfiguration.Differential, ref value, -10, 10);

            return value;
        }

        public double ReadSingleAnalogChannelDiff(string lines, int cnt)
        {
            //read analog channel and average counts
            int res, i;
            double[] value = null;
            double anaout = 0;

            if (cnt == 0) cnt = 30;

            for (i = 0; i < cnt; i++)
            {
                res = ReadAnalogChannel(lines, "Diff Analog Channel", AITerminalConfiguration.Differential, ref value, -10, 10);
                anaout += value[0];
            }
            anaout = anaout / cnt;

            return anaout;
        }

        public double ReadSingleAnalogChannelNrse(string lines, int cnt)
        {
            //read analog channel and average counts
            int res, i;
            double[] value = null;
            double anaout = 0;

            //if (cnt == 0) cnt = 30;

            for (i = 0; i < cnt; i++)
            {
                res = ReadAnalogChannel(lines, "Nrse Analog Channel", AITerminalConfiguration.Nrse, ref value, -10, 10);
                anaout += value[0];
            }
            anaout = anaout / cnt;

            return anaout;
        }

        // overload #1
        public double ReadSingleAnalogChannelRse(string lines)
        {
            //read analog channel and return
            int res;
            double[] value = null;

            res = ReadAnalogChannel(lines, "Rse Analog Channel", AITerminalConfiguration.Rse, ref value, -10, 10);

            return value[0];
        }

        // overload #2
        public double ReadSingleAnalogChannelRse(string lines, int cnt)
        {
            //read analog channel and average counts
            int res, i;
            double[] value = null;
            double anaout = 0;

            //if (cnt == 0) cnt = 30;

            for (i = 0; i < cnt; i++)
            {
                res = ReadAnalogChannel(lines, "Rse Analog Channel", AITerminalConfiguration.Rse, ref value, -10, 10);
                anaout += value[0];
            }
            anaout = anaout / cnt;

            return anaout;
        }

        public int ReadSingleAnalogChannelRseConvertToDigital(string lines, int cnt)
        {
            //read analog channel and average counts
            int res, i;
            double[] value = null;
            int dig = 0;

            //if (cnt == 0) cnt = 30;

            for (i = 0; i < cnt; i++)
            {
                res = ReadAnalogChannelConvertToDigital(lines, "Rse Analog Channel", AITerminalConfiguration.Rse, ref value, -10, 10);
                dig += res; //(int)value[0];
            }
            dig = dig / cnt;

            return dig;
        }

        //Measures the PW on PFI 9/P2_1
        public double ReadPulseWidth(string lines, CIPulseWidthStartingEdge StartingEdge)
        {
            double PW = 0;
            double minPW_InSeconds = 0.000000100;
            double maxPW_InSeconds = 0.830000000;
            PW = ReadDigitalPulseWidth(lines, StartingEdge, minPW_InSeconds, maxPW_InSeconds);

            return PW;
        }

        //Measures the separation of two PW on PFI 9(P2_1) and PFI10(P2_2)
        public double Meas2EdgeSeparation(string lines, CITwoEdgeSeparationFirstEdge firstEdge, CITwoEdgeSeparationSecondEdge secondEdge)
        {
            double PW = 0;
            int i_Timeout = 2000;          //unit:ms
            double minPW_InSeconds = 0.00000010;
            double maxPW_InSeconds = 0.838860750;
            PW = Meas2EdgeSeparation(lines, minPW_InSeconds, maxPW_InSeconds, firstEdge, secondEdge, i_Timeout);

            return PW;
        }

        //Measures the Frequency on PFI 9/P2_1 or PFI 10/P2_2
        public bool MeasureLowFrequency(string lines)
        {
            bool b_res;
            string str_Channel = "Measure Dig Freq Low Frequency";
            double d_MinFrequency = 1.192093;    //Hz
            double d_MaxFrequency = 10000000.0;  //Hz
            double d_MeasureTime = 3;            //s

            b_res = ReadDigitalLowFrequency(lines, str_Channel, d_MinFrequency, d_MaxFrequency, d_MeasureTime);
            clsUtils.Dly(0.2);

            return b_res;
        }

        #endregion
    }
}
