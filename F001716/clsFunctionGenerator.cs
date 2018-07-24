using System;
using System.Collections.Generic;
using System.Text;
using NationalInstruments.DAQmx;
using System.Diagnostics;

namespace F001716
{
    public enum WaveformType 
    {
        SineWave
    }

    class clsFunctionGenerator
    {
 
        static double[] _data;
        static double _resultingSampleClockRate;
        static double _resultingFrequency;
        static double _desiredSampleClockRate;
        static double _samplesPerCycle;
        
        public double[] fgData
        {
            get { return _data; }
        }

        public double fgResultingSampleClockRate
        {
            get { return _resultingSampleClockRate; }
        }

        public clsFunctionGenerator(Timing timingSubobject, string desiredFrequency, string samplesPerBuffer, string cyclesPerBuffer, string type, string amplitude)
        {
            WaveformType t = WaveformType.SineWave;
            if (type == "Sine Wave")
                t = WaveformType.SineWave;
            else
                Debug.Assert(false, "Invalid Waveform Type");

            Init(timingSubobject, Double.Parse(desiredFrequency), Double.Parse(samplesPerBuffer), Double.Parse(cyclesPerBuffer), t, Double.Parse(amplitude));
        }
        
        private void Init(Timing timingSubobject, double desiredFrequency, double samplesPerBuffer, double cyclesPerBuffer, WaveformType type, double amplitude)
        {
            if (desiredFrequency <= 0)
                throw new ArgumentOutOfRangeException("desiredFrequency", desiredFrequency, "This parameter must be a positive number");
            else if (samplesPerBuffer <= 0)
                throw new ArgumentOutOfRangeException("samplesPerBuffer", samplesPerBuffer, "This parameter must be a positive number");
            else if (cyclesPerBuffer <= 0)
                throw new ArgumentOutOfRangeException("cyclesPerBuffer", cyclesPerBuffer, "This parameter must be a positive number");

            // First configure the Task timing parameters
            if (timingSubobject.SampleTimingType == SampleTimingType.OnDemand)
                timingSubobject.SampleTimingType = SampleTimingType.SampleClock;

            _desiredSampleClockRate = (desiredFrequency * samplesPerBuffer) / cyclesPerBuffer;
            _samplesPerCycle = samplesPerBuffer / cyclesPerBuffer;

            // Determine the actual sample clock rate
            timingSubobject.SampleClockRate = _desiredSampleClockRate;
            _resultingSampleClockRate = timingSubobject.SampleClockRate;

            _resultingFrequency = _resultingSampleClockRate / (samplesPerBuffer / cyclesPerBuffer);

            if (type == WaveformType.SineWave)
                _data = GenerateSineWave(_resultingFrequency, amplitude, _resultingSampleClockRate, samplesPerBuffer);
        }

        public double[] GenerateSineWave(double frequency, double amplitude, double sampleClockRate, double samplesPerBuffer)
        {
            double deltaT;
            int intSamplesPerBuffer;

            deltaT = 1 / sampleClockRate; // sec./samp
            intSamplesPerBuffer = (int)(samplesPerBuffer - 1);
            double[] rVal = new double[intSamplesPerBuffer - 1];

            for (int i = 0; i < intSamplesPerBuffer - 1; i++)
                rVal[i] = amplitude * Math.Sin((2.0 * Math.PI) * frequency * (i * deltaT));
            return rVal;
        }

        public void InitComboBox(System.Windows.Forms.ComboBox box)
        {
            Object[] items = null;
            items[0] = "Sine Wave";

            box.Items.Clear();
            box.Items.AddRange(items);
            box.Sorted = false;
            box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            box.Text = "Sine Wave";
        }
    }
}
