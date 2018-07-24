using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace F001716
{
    class ReadIniSettings
    {
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileIntA")]
        public static extern int GetPrivateProfileInt(string sectionName,
        string keyName, int defaultVal, string fileName);

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA",
        SetLastError = true)]
        private static extern int GetPrivateProfileStringKey(string sectionName,
        string keyName, string defaultVal, StringBuilder returnVal, int returnSize,
        string fileName);

        //Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" _
        //(ByVal lpApplicationName As String, ByVal lpKeyName As String, _
        //ByVal lpDefault As String, ByVal lpReturnedString As String, _
        //ByVal nSize As Int32, ByVal lpFileName As String) As Int32

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA",
        SetLastError = true)]
        private static extern int WritePrivateProfileString(string sectionName,
        string keyName, string defaultVal, string fileName);

        //Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" _
        //(ByVal lpApplicationName As String, ByVal lpKeyName As String, _
        //ByVal lpString As String, ByVal lpFileName As String) As Int32

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA",
        SetLastError = true)]
        private static extern int FlushPrivateProfileString(int sectionName,
        int keyName, int defaultVal, string fileName);

        //Private Declare Ansi Function FlushPrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" _
        //(ByVal lpApplicationName As Integer, ByVal lpKeyName As Integer, ByVal lpString As Integer, _
        //ByVal lpFileName As String) As Integer

        public ReadIniSettings()
        {

        }

        ~ReadIniSettings()
        {

        }

        //***********************************************
        //Purpose : retrieves data from the ini file
        //Inputs : None
        //Sets :
        //Returns : A string.
        //***********************************************
        public string ReadPrivateProfileStringKey(string section, string key, string path)
        {
            Int32 i32_NumOfElementsReturned;
            string str_DefaultResult = "Error";
            StringBuilder buffer = new StringBuilder(256);

            i32_NumOfElementsReturned = GetPrivateProfileStringKey(section, key, str_DefaultResult,
                buffer, buffer.Capacity, path);

            if (i32_NumOfElementsReturned > 0)
                return buffer.ToString();
            else
                return str_DefaultResult;
        }

        //***********************************************
        //Purpose : retrieves data from the ini file
        //Inputs : None
        //Sets :
        //Returns : An integer.
        //***********************************************
        public int ReadPrivateProfileInt(string section, string key, string path)
        {
            int nResult;
            int nDefault = -1;

            nResult = GetPrivateProfileInt(section, key, nDefault, path);
            return nResult;
        }

        public void WriteString(string section, string key, string value, string path)
        {
            // Writes a string to your INI file
            WritePrivateProfileString(section, key, value, path);
            Flush(path);
        }

        private void Flush(string strFilename)
        {
            // Stores all the cached changes to your INI file
            FlushPrivateProfileString(0, 0, 0, strFilename);
        }

    }
}
