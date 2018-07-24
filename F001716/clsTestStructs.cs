using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F001716
{
    public enum enAimerType
    {
        LED,
        Laser,
        Invalid
    }
    public enum Color
    {
        White = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        Black = 4,
        Blue_White = 5,//Blue+ White(>230)
        RGB_ALL,
    }
    public struct Image_t
    {
        // Threshold
        public Int32 Threshold_White;
        public Int32 Threshold_Black;
        public Int32 Threshold_Red;
        public Int32 Threshold_Green;
        public Int32 Threshold_Blue;

        // Pixel
        public Int32 CatchPoint;
        public Int32 White_Count;
        public Int32 Red_Count;
        public Int32 Green_Count;
        public Int32 Blue_Count;
        public Int32 Black_Count;
        // ROI
        public Int32 StartX;
        public Int32 StartY;
        public Int32 EndX;
        public Int32 EndY;

        // ROI_plus
        public Int32 StartX_plus;
        public Int32 StartY_plus;
        public Int32 EndX_plus;
        public Int32 EndY_plus;
    }
    public struct NetworkMapping_t
    {
        public string I_M_EDir;
        public string I_M_E_TransferDir;
        public string ProductionDataDir;
        public string LabelsDir;
        public string MastersDir;
    }
    public enum enIlluminationType
    {
        FourWire,
        Invalid
    }
    public struct AR66_Status
    {
        public int CurrentPosition;
        public string Error;
    }
    public struct ComPortSettings_t
    {
        public Int32 PortNum;
        public Int32 Baud;
        public string Parity;
        public Int32 DataBits;
        public Int32 StopBits;
    }

    public struct t_McConfigData
    {
        public string mcMCNumber;
        public string mcFamily;
        public string mcHwtoken;
        public string mcLedVoltage;
        public string mcIlluminationType;
        public string mcMemory;
        public string mcExpansion;
        public enUsageType mcUsageType;
        public enDirectionType mcDirectionType;
    }

    public struct TestStatus_t
    {
        public bool TestPassed;
        public string TestFailNumber;
        public string TestFailMessage;
        public string TestFailMessage_UI;
        public string TestFailMessage_Name;
        public string TestStatus;
    }

    public struct TestInfo_t
    {
        public string TestSoftwareNum;
        public string TestSoftwareRev;
        public string TestMCFNum;
        public string model;
        public string ItemNum;
        public string SWImageNumber;
        public string EMBEDDED_FW_REV;
        public string PCDownloadDirectory;
        public string QADLine;
        public string BomRev;
        public string Description;

    }

    public struct UnitInfo_t
    {
        public double total_time;


        public double Current;
        public string embedded_rev;
        public string str_SN;
        public double ShortOpen_Current;
        public double UnitCurrent;
        public double UnitCurrent2;
        public int APL;
        public string BT_MacAddress;

    }

    public enum AR66_Position
    {
        Home,
        Barcode1_70mm,
        Barcode1_65mm,
        Barcode1_55mm,
        Barcode1_20mm,
        Barcode1_135mm,
        Barcode1_110mm,
        Barcode1_5mil,

        Barcode2_13mil,
        Barcode2_10mil_C39,
        Barcode2_220mm,
        Barcode2_200mm,
        Barcode2_375mm,
        Barcode2_370mm,
        Barcode2_390mm,
        Barcode2_360mm,
        Barcode2_330mm,
        Barcode2_6mil,

        Barcode3_10mil,
        Barcode3_95mm,
        Barcode3_180mm,
        Barcode3_170mm,
        Barcode3_150mm,
        Barcode3_215mm,
        Barcode3_230mm,




        Triger0,
        Triger1,
        LaserTrigger_Home,
        LaserTrigger_Target,
        None,
    }

    public struct TestSaveData_t
    {
        public t_McConfigData Config;
        public AR66_Status AR66Status;
        public AR66_Status AR66Status1;
        public TestStatus_t Results;
        public TestInfo_t TestInfo;
        public UnitInfo_t UnitInfo;
        public NetworkMapping_t NetworkMapping;
    }

    class clsTestStructs
    {

    }

    public enum enDirectionType
    {
        Horizontal,
        Vertical
    }

    public enum enUsageType
    {
        General,
        Healthcare 
    }
}
