using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;     // Needed for Application.DoEvents();  
using System.Diagnostics;       // Needed for Process();

namespace F001716
{
    class clsExecProcess
    {

        private string m_procName = "";
        private string m_procCmdargs = "";
        private string m_procWdir = "";
        private int m_procTimeout = 0;
        private int m_procExitCode = 0;
        private bool m_processComplete;
        private bool m_procRedirectStandardOut;

        public string epProcCmdArgs
        {
            get { return m_procCmdargs; }
            set { m_procCmdargs = value; }
        }

        public string epProcName
        {
            get { return m_procName; }
            set { m_procName = value; }
        }

        public int epProcTimeOut
        {
            get { return m_procTimeout; }
            set { m_procTimeout = value; }
        }

        public int epProcExitCode
        {
            get { return m_procExitCode; }
        }

        public bool epProcessComplete
        {
            get { return m_processComplete; }
            set { m_processComplete = value; }
        }

        public clsExecProcess(string procName, string procCmdargs, string procWdir, int procTimeout, bool procRedirectStandardOut)
        {
            m_processComplete = false;
            m_procName = procName;
            m_procCmdargs = procCmdargs;
            m_procWdir = procWdir;
            m_procTimeout = procTimeout;
            m_procRedirectStandardOut = procRedirectStandardOut;
        }

        public int ExecCmd(string cmdline, string cmdargs)
        {
            Process myProcess = null;
            ProcessStartInfo psi = null;
            int ec = -1;

            try
            {
                psi = new ProcessStartInfo(cmdline, cmdargs);
                psi.WindowStyle = ProcessWindowStyle.Minimized;
                if (m_procRedirectStandardOut)
                {
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;
                }
                myProcess = Process.Start(psi);
                if (m_procRedirectStandardOut)
                {
                    string strOutput = myProcess.StandardOutput.ReadToEnd();
                    Console.WriteLine(strOutput);
                }
                //psi = new ProcessStartInfo(cmdline,cmdargs);

                //myProcess.StartInfo.FileName = cmdline;
                //myProcess.StartInfo.Arguments = cmdargs;
                //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                //psi.WindowStyle = ProcessWindowStyle.Minimized;
                //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                if (m_procTimeout == 0)
                    myProcess.WaitForExit();
                else
                    myProcess.WaitForExit(m_procTimeout);
                //Wait for the shelled application to finish:
                do
                {
                    Application.DoEvents();
                }
                while (!myProcess.HasExited);
                //Application.DoEvents();
                ec = myProcess.ExitCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process failed to start: " + cmdline);
            }
            finally
            {
                if (myProcess != null)
                    myProcess.Close();
            }
            return ec;
        }

        public int ExecCmd2(string cmdline, string cmdargs)
        {
            Process myProcess = null;

            try
            {
                myProcess = new Process();
                myProcess.StartInfo.FileName = cmdline;
                myProcess.StartInfo.Arguments = cmdargs;
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                myProcess.Start();
                if (m_procTimeout == 0)
                    myProcess.WaitForExit();
                else
                    myProcess.WaitForExit(m_procTimeout);
                //Wait for the shelled application to finish:
                //Do
                //Windows.Forms.Application.DoEvents()
                //Loop While (myProcess.ExitCode = STILL_ACTIVE)
                Application.DoEvents();
                //return myProcess.ExitCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process failed to start: " + cmdline);
            }
            finally
            {
                if (myProcess != null)
                    myProcess.Close();
            }
            return myProcess.ExitCode;
        }

        public void ExecExternalProcess()
        {
            Process objProc = null;
            int procExit = -1;

            try
            {
                objProc = new Process();
                objProc.StartInfo.FileName = m_procName;
                objProc.StartInfo.Arguments = m_procCmdargs;
                objProc.StartInfo.WorkingDirectory = m_procWdir;

                objProc.StartInfo.UseShellExecute = true;
                objProc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                objProc.Start();
                if (m_procTimeout == 0)
                {
                    while (objProc.HasExited == false)
                    {
                        Application.DoEvents();
                        objProc.WaitForExit(300);
                    }
                }
                else
                    objProc.WaitForExit(m_procTimeout);

                procExit = objProc.ExitCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process failed to start: " + m_procName);
            }
            finally
            {
                if (objProc != null)
                    objProc.Close();
            }

            m_processComplete = true;
        }
    }
}
