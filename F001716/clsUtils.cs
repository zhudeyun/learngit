using System;
using System.Collections.Generic;
using System.Text;

namespace F001716
{
    class clsUtils
    {
        //*********************************
        //Purpose : Causes a delay 
        //           (the system (Date.Time.Now).Ticks 
        //            has a 100 nanoseconds resolution)   
        //Inputs  : a single representing the delay time in seconds
        //Sets    : nothing
        //Returns : nothing
        //*********************************

        public static void Dly(double WaitTimeSeconds)
        {
            long waitTime = 0;
            long startTime = 0;

            //Convert it to 100 nanosecond increments - this was tested and is very accurate for precise delays - LRG
            waitTime = Convert.ToInt64(WaitTimeSeconds * TimeSpan.TicksPerSecond);
            startTime = System.DateTime.Now.Ticks;
            while ((System.DateTime.Now.Ticks - startTime) < waitTime)
            {
                System.Windows.Forms.Application.DoEvents();          //Process pending events while waiting - actually do something!
            }
        }

        //*********************************
        //Purpose : Return Time in Ticks
        //Inputs : none
        //Sets : nothing
        //Returns : a Long Indicating Ticks at time now
        //*********************************
        public static long StartTimeInTicks()
        {
            return (System.DateTime.Now).Ticks;
        }

        //*********************************
        //Purpose : Return elpas time
        //Inputs : a long (int64) representing the start time in ticks
        //Sets : nothing
        //Returns : a double indicating elaps time
        //*********************************
        public static double ElapseTimeInSeconds(long StartTimeInTicks)
        {
            return ((System.DateTime.Now).Ticks - StartTimeInTicks) / System.TimeSpan.TicksPerSecond;
        }

        //.NET to convert a string to a byte array
        public static byte[] StrToByteArray(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        //.NET to convert a byte array to a string
        public static string ByteArrayToStr(byte[] b)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(b);
        }

        //private static int Asc(char c)
        //{
        //    int converted = c;
        //    if (converted >= 0x80)
        //    {
        //        byte[] buffer = new byte[2];
        //        // if the resulting conversion is 1 byte in length, just use the value
        //        if (System.Text.Encoding.Default.GetBytes(new char[] { c }, 0, 1, buffer, 0) == 1)
        //        {
        //            converted = buffer[0];
        //        }
        //        else
        //        {
        //            // byte swap bytes 1 and 2;
        //            converted = buffer[0] << 16 | buffer[1];
        //        }
        //    }
        //    return converted;
        //}

        private static short Asc(string String)
        {
            return Encoding.Default.GetBytes(String)[0];
        }

        private static string Chr(int CharCode)
        {
            string cc = "";
            if (CharCode < 255)
                //throw new ArgumentOutOfRangeException("CharCode", CharCode, "CharCode must be between 0 and 255.");
                cc= Encoding.Default.GetString(new byte[] { (byte)CharCode });
            return cc;
        }

        public static string ConvertNonPrintables(string s)
        {
            try
            {
                string R = null;
                string C;

                for (int x = 0; x < s.Length - 1; x++)
                {
                    C = s.Substring(x, 1);
                    if (Asc(C) > 31)
                        R = R + C;
                    else if (C == Chr(0))
                        R = R + "<NUL>";
                    else if (C == Chr(1))
                        R = R + "<SOH>";
                    else if (C == Chr(2))
                        R = R + "<STX>";
                    else if (C == Chr(3))
                        R = R + "<ETX>";
                    else if (C == Chr(4))
                        R = R + "<EOT>";
                    else if (C == Chr(5))
                        R = R + "<ENQ>";
                    else if (C == Chr(6))
                        R = R + "<ACK>";
                    else if (C == Chr(7))
                        R = R + "<BEL>";
                    else if (C == Chr(8))
                        R = R + "<BS>";
                    else if (C == Chr(9))
                        R = R + "<HT>";
                    else if (C == Chr(10))
                        R = R + "<LF>";
                    else if (C == Chr(11))
                        R = R + "<VT>";
                    else if (C == Chr(12))
                        R = R + "<FF>";
                    else if (C == Chr(13))
                        R = R + "<CR>";
                    else if (C == Chr(14))
                        R = R + "<SO>";
                    else if (C == Chr(15))
                        R = R + "<SI>";
                    else if (C == Chr(16))
                        R = R + "<DLE>";
                    else if (C == Chr(17))
                        R = R + "<DC1>";
                    else if (C == Chr(18))
                        R = R + "<DC2>";
                    else if (C == Chr(19))
                        R = R + "<DC3>";
                    else if (C == Chr(20))
                        R = R + "<DC4>";
                    else if (C == Chr(21))
                        R = R + "<NAK>";
                    else if (C == Chr(22))
                        R = R + "<SYN>";
                    else if (C == Chr(23))
                        R = R + "<ETB>";
                    else if (C == Chr(24))
                        R = R + "<CAN>";
                    else if (C == Chr(25))
                        R = R + "<EM>";
                    else if (C == Chr(26))
                        R = R + "<SUB>";
                    else if (C == Chr(27))
                        R = R + "<ESC>";
                    else if (C == Chr(28))
                        R = R + "<FS>";
                    else if (C == Chr(29))
                        R = R + "<GS>";
                    else if (C == Chr(30))
                        R = R + "<RS>";
                    else if (C == Chr(31))
                        R = R + "<US>";
                }
                return R;
            }
            catch (Exception e)
            {
                 s = e.Message;
                return s;
            }
        }

        public static bool GetFailCode(string strin, ref string strout)
        {
            int idx;
            string cmdres = "";
            strout = "";

            if (strin == null)
                return true;

            // pull out error number, zero is reserved for MDCS passing, so use 1 as a default fail code
            idx = strin.IndexOf("#", 0);
            if (idx == -1)
            {
                strout = "1";
                return false;
            }
            cmdres = strin.Substring(idx + 1);

            idx = cmdres.IndexOf(":", 0);
            if (idx == -1)
            {
                strout = "1";
                return false;
            }
            cmdres = cmdres.Substring(0, idx);
            strout = cmdres.Trim().Replace(" ", "");
            return true;
        }

        //*********************************
        //Purpose : Generates a string for what a menue result should be
        //Inputs  : A menu command string minus the syn m CR
        //Sets    : nothing
        //Returns : A string
        //*********************************
        public static string GenerateAMenuResult(string cmd) 
        {
        string str_res = "";
        string str_Term = "";
        
            cmd = cmd.Trim();
        
            str_Term = cmd.Substring(cmd.Length - 1);
            str_res = cmd.Replace(";", "\x06" + ";");
            str_res = str_res.Replace(str_Term, "\x06" + str_Term);
            return str_res;
        }
        
    }
}
