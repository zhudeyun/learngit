using System;
using System.Collections.Generic;
using System.Text;

namespace F001716
{
    class clsSaveDataMDCS
    {

        //Notes:
        //1 - MDCS"failcode"of"0"= passed, zero is a string here.
        //2 - Try to use numeric variables so MDCS can graph them: avg.,low, high, stddev, etc.
        //3 - In this class, we will need a struct for a single variable then add these to an array.
        //

        private MDCS.MDCSDeviceSetup  mdcsDevice;
        private MDCS.QCDCSDataInfo st_Data;
        private MDCS.QCDCS qdcdsDevice;
        private string mstr_error ; 
        private string mstr_ServerName ; 
        private string mstr_DeviceName ; 
        private bool mbln_modeProduction ; 
        private TestSaveData_t mobj_DataStruct ;        //Holds all relevent test data 

        public clsSaveDataMDCS()
        {

            mdcsDevice = new MDCS.MDCSDeviceSetup();

            mobj_DataStruct = new TestSaveData_t();
            mstr_error = "";

            mstr_ServerName = "http://hsm-mdcsws-ch3u.honeywell.com/MDCSWebService/MDCSService.asmx";
            mdcsDevice.ServerURL = mstr_ServerName;

            mstr_DeviceName = "XXXXMBFunctionalPRE";
            mdcsDevice.DeviceName = mstr_DeviceName;
                
            mbln_modeProduction = true;
            mdcsDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;
        }

        ~clsSaveDataMDCS()
        {

        }

        public string ServerName
        {
            get {
                return mstr_ServerName;
            } 
            set {
                mstr_ServerName = value;
            } 
        } 

        public bool UseModeProduction
        {
            get {
                return mbln_modeProduction;
            }
            set 
            {
                mbln_modeProduction = value;
            } 
        } 

        public string DeviceName
        {
            get {
                return mstr_DeviceName;
            } 
            set {
                mstr_DeviceName = value;
            } 
        } 

        public TestSaveData_t p_TestData
        {
            get {
                return mobj_DataStruct;
            } 
            set {
                mobj_DataStruct = value;
            } 
        }
        public bool SaveDataToQCDCS(bool R_esult, string SN)
        {
            bool Check = false;
            st_Data = new MDCS.QCDCSDataInfo();
            string str_ErrorMessage = "";
            st_Data.BF_Model = mobj_DataStruct.TestInfo.ItemNum;//mobj_DataStruct.TestInfo.ItemNum;// "Fail";//"1250G-2";
            st_Data.Model = mobj_DataStruct.TestInfo.ItemNum;//mobj_DataStruct.TestInfo.ItemNum;// "Fail";//"1250G-2";
            st_Data.QAD_Line = mobj_DataStruct.TestInfo.QADLine;//mobj_DataStruct.TestInfo.QADLine;// "7100";
            st_Data.Serial = SN;
            st_Data.Software = mobj_DataStruct.UnitInfo.embedded_rev;// "BO000033BAC";
            //str_Benchnumber = mobj_DataStruct.TestInfo.BenchNumber;
            if (R_esult == true)
            {
                st_Data.Post_Status = 2;
                st_Data.Pre_Status = 2;
                st_Data.FA_Status = 2;
                st_Data.XFR_Disp = 0;
            }
            else
            {
                st_Data.Post_Status = 0;
                st_Data.Pre_Status = 0;
                st_Data.FA_Status = 0;
                st_Data.XFR_Disp = 0;
            }
            qdcdsDevice = new MDCS.QCDCS();
            Check = qdcdsDevice.InsertQCDCS(st_Data, ref str_ErrorMessage);
            if (Check == false)
            {
                mstr_error = "Failed to send data to QCDCS";
                System.Windows.Forms.MessageBox.Show(mstr_error, "QCDCS",
                                                    System.Windows.Forms.MessageBoxButtons.OK,
                                                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                
            }

            return true;
        }
        public bool WriteAllData()
        {
            bool retVal  = false;
            #region Necessaryitems
            mdcsDevice.ServerURL = mstr_ServerName;
            mdcsDevice.DeviceName = mstr_DeviceName;
            if (mbln_modeProduction == true) {
                mdcsDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;
            } else {
                mdcsDevice.TestType = MDCS.MDCSTestModes.TESTMODE;
            }
            mdcsDevice.ItemType = "Standard";
            mdcsDevice.Key = mobj_DataStruct.UnitInfo.str_SN;
            mdcsDevice.AddStringVariable("Test_SW_Num", mobj_DataStruct.TestInfo.TestSoftwareNum);
            mdcsDevice.AddStringVariable("Test_SW_Rev", mobj_DataStruct.TestInfo.TestSoftwareRev);
            mdcsDevice.AddStringVariable("ShortOpen_Current", mobj_DataStruct.UnitInfo.ShortOpen_Current.ToString("0.000"));
            mdcsDevice.AddStringVariable("MCF", mobj_DataStruct.TestInfo.TestMCFNum);
            mdcsDevice.AddStringVariable("SerialNumber", mobj_DataStruct.UnitInfo.str_SN);
            mdcsDevice.AddStringVariable("FW_Version", mobj_DataStruct.UnitInfo.embedded_rev);
            mdcsDevice.AddStringVariable("BT_Address", mobj_DataStruct.UnitInfo.BT_MacAddress);
            mdcsDevice.AddStringVariable("PartNumber", mobj_DataStruct.TestInfo.ItemNum);


            mdcsDevice.AddStringVariable("Total_Test_Time", mobj_DataStruct.UnitInfo.total_time.ToString("0.000")); 
            mdcsDevice.AddStringVariable("Error_Message", mobj_DataStruct.Results.TestFailMessage);
            mdcsDevice.AddStringVariable("FailNumber", mobj_DataStruct.Results.TestFailNumber);
            mdcsDevice.AddStringVariable("Failcode", mobj_DataStruct.Results.TestFailNumber);
            #endregion

           
            //Send all varibles
            retVal = mdcsDevice.SendMDCSTestRecord();
            if (retVal == false) {
                mstr_error = "Failed to send data to MDCS";
                System.Windows.Forms.MessageBox.Show(mstr_error, "MDCS",
                                                    System.Windows.Forms.MessageBoxButtons.OK,
                                                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        } 

    }
}
