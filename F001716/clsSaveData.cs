using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace F001716
{
    class clsSaveData
    {


        public enum DataFileType
        {
            LocalTestData,
            ProductionDatabaseData,
            DebugTestData
        }

        private string mstr_error;
        private TestSaveData_t mobj_TestData;  //Holds all relevent test data 

        public clsSaveData()
        {
            mstr_error = "";
            mobj_TestData = new TestSaveData_t();

        }

        ~clsSaveData()
        {

        }

        public string err
        {
            get
            {
                return mstr_error;
            }
        }

        //Pass the test struct using this property 
        public TestSaveData_t p_TestData
        {
            get
            {
                return mobj_TestData;
            }
            set
            {
                mobj_TestData = value;
            }
        }

        //**************************************************************
        //Purpose : Verifys Test Data File Size ,Moves and creates a new
        //            one if necessary. Saves test data.
        //Inputs  : TestSaveData Struct
        //Sets    : Nothing
        //Returns : A TestResult_t with pass fail data 
        //**************************************************************
        public bool SaveTestData()
        {
            string str_TestResults;
            string str_Path = "";
            string str_FileName;
            System.IO.StreamWriter sw;
            bool bln_IniFile;
            System.IO.FileInfo fi;
            string stampDate, stampTime;

            mstr_error = "";
            str_Path = System.IO.Directory.GetCurrentDirectory() + "\\Data";
            str_FileName = mobj_TestData.TestInfo.TestSoftwareNum + "log.txt";
            stampDate = System.DateTime.Now.ToString("MM/dd/yyyy");
            stampTime = System.DateTime.Now.ToString("HH:mm:ss");

            //See if data directory is not present then make it 
            if (System.IO.Directory.Exists(str_Path) == false)
            {
                System.IO.Directory.CreateDirectory(str_Path);
            }

            if (System.IO.File.Exists(str_Path + "\\" + str_FileName))
            {
                fi = new System.IO.FileInfo(str_Path + "\\" + str_FileName);
                //Depending on file size 
                //See if we need to put the header in it
                //Or rename it and create a new file 
                if (fi.Length > 900000)
                {
                    //Need to copy the file
                    try
                    {
                        System.IO.File.Copy((str_Path + "\\" + str_FileName), (str_Path + "\\" + System.DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "" + str_FileName));
                        System.IO.File.Delete(str_Path + "\\" + str_FileName);
                    }
                    catch (Exception ex)
                    {
                        mstr_error = "Error: File Move Error" + "\n" + ex.Message;
                        //MsgBox(mstr_error, MsgBoxStyle.Critical, "File Error");
                        return false;
                    }
                    bln_IniFile = true;
                }
                else
                {
                    if (fi.Length < 5)
                    {
                        //Need to write the header
                        bln_IniFile = true;
                    }
                    else
                    {
                        bln_IniFile = false;
                    }
                }
            }
            else
            {
                bln_IniFile = true;
            }

            if (bln_IniFile)
            {
                //Need to write the header
                str_TestResults = "Date".PadLeft(19) + "," +
                    "Test_Time".PadLeft(11) + "," +
                    "Test_SW_Num".PadLeft(9) + "," +
                    "Test_SW_Rev".PadLeft(7) + "," +
                    "OpenShortCurrent".PadLeft(20) + "," +
                    "Current".PadLeft(20) + "," +
                    "PerasentationCurrent".PadLeft(20) + "," +
                    "APL".PadLeft(20) + "," +
                    "FailNumber".PadLeft(10) + "," +
                    "Status".PadLeft(7) + "," +
                    "Error_Message" + Convert.ToChar(13) + Convert.ToChar(10);
                
                try
                {
                    sw = System.IO.File.CreateText(str_Path + "\\" + str_FileName);
                }
                catch (Exception ex)
                {
                    mstr_error = "Error: Data File Creation Error" + "\n" + ex.Message;
                    //MsgBox(mstr_error, MsgBoxStyle.Critical, "File Error");
                    return false;
                }

                try
                {
                    sw.Write(str_TestResults);
                    sw.Flush();
                }
                catch (Exception ex)
                {
                    mstr_error = "Error: Data File Write Error" + "\n" + ex.Message;
                    //MsgBox(mstr_error, MsgBoxStyle.Critical, "File Error");
                    sw.Close();
                    return false;
                }
                sw.Close();
            }

            //most error messages already contain crlf, we don't need 2 of them in the data file
            string ErrorMessage = mobj_TestData.Results.TestFailMessage;
            if (!ErrorMessage.EndsWith("\n"))
            {
                ErrorMessage += "\r\n";
            }

            //Write the test data to the file
            str_TestResults = (stampDate + " " + stampTime).PadLeft(19) + "," +
            mobj_TestData.UnitInfo.total_time.ToString("").PadLeft(11) + "," +
            mobj_TestData.TestInfo.TestSoftwareNum.PadLeft(9) + "," +
            mobj_TestData.TestInfo.TestSoftwareRev.PadLeft(7) + "," +
            mobj_TestData.UnitInfo.ShortOpen_Current.ToString("0.0").PadLeft(20) + "," +
            mobj_TestData.UnitInfo.Current.ToString("0.0").PadLeft(20) + "," +
            mobj_TestData.UnitInfo.UnitCurrent2.ToString("0.0").PadLeft(20) + "," +
            mobj_TestData.UnitInfo.APL.ToString().PadLeft(20) + "," +
            mobj_TestData.Results.TestFailNumber.PadLeft(10) + "," +
            mobj_TestData.Results.TestStatus.PadLeft(7) + "," +
            ErrorMessage;


            try
            {
                sw = System.IO.File.AppendText(str_Path + "\\" + str_FileName);
            }
            catch (Exception ex)
            {
                mstr_error = "Error: Data File Creation Error" + "\n" + ex.Message;
                //MsgBox(mstr_error, MsgBoxStyle.Critical, "File Error");
                return false;
            }

            try
            {
                sw.Write(str_TestResults);
                sw.Flush();
            }
            catch (Exception ex)
            {
                mstr_error = "Error: Data File Write Error" + "\n" + ex.Message;
                //MsgBox(mstr_error, MsgBoxStyle.Critical, "File Error");
                sw.Close();
                return false;
            }
            sw.Close();

            return true;
        }

    }
}
