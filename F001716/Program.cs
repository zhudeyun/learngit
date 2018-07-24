using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

/********************************************************************************
 *
 * F001716 "Product Name" Fixture Test
 *
 * $Id: F001716.cs
 *
 * Documentation:
 * Test Configuration (MCB/MCF)
 * Test plan & specification
 * User Manual
 * Performance Specification
 *
 * Requirements:
 * Fixture Electrical Schematic
 * "File Number" "Product Name" Scanner Schematic
 *
 * Installation:
 * 1. Install National Instruments NI-DAQmx 9.3..
 * 3. Screen resolution 1024x768 normal size fonts.
 * 4. Unzip the latest F001716 release from H:\Masters\F001716.
 * 5. Build F001716. 
 *******************************************************************************/

/*******************************************************************************
 *
 * History Log:
 * REV AUTHOR DATE COMMENTS
 *
 * A   Sam xue    01/04/2017    
 *     1.Create a template for fixture software development.
 *******************************************************************************/

namespace F001716
{
    
    static class Program
    {
        public static int FixtureLoopCount;
        public static string gstr_Software_Number;
        public static string gstr_Rev;
        private static System.Threading.Mutex mutex;
        public static string versionDescription;
        private const string CRLF = "\x0d\x0a";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            mutex = new System.Threading.Mutex(false, "F001716 SINGLE_INSTANCE_MUTEX"); 
            if ( ! mutex.WaitOne(0,false))
            {
                mutex.Close();
                mutex = null;
            }
            if (mutex != null)
            {
                gstr_Software_Number = "F001716";
                gstr_Rev = "A2";
                versionDescription = "Rev       Author   Date     " + CRLF +
                                           "A   Jiaquan    05/31/2018   " + CRLF +
                                           "First version " + CRLF +
                                            "A2   Jiaquan    07/05 2018 add firmware check";
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                mutex.ReleaseMutex();
            }   
           else 
            {
                MessageBox.Show("F001716 is already running !!!!");
            }
                      
        }
    }
}
