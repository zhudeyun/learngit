using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F001716
{
    class clsPrinter
    {
        public Honeywell.LabelPrintDLL.TestData_t m_TestData = new Honeywell.LabelPrintDLL.TestData_t();
        
        //public static Honeywell.LabelPrintDLL.ComPortSettings_t m_PrinterCom = new Honeywell.LabelPrintDLL.ComPortSettings_t();

        private string[] MarkerItem  ;
        private string[] MarkerValue ;
        private  MarkDetail []obj_MarkDetail;
        private string str_Err;

        public struct MarkDetail
        {
            public string Item;
            public string Value;
        }

        public string Eorr
        {
            get
            {
                return str_Err;
            }

        }
        public MarkDetail []DataDetail
        {
            set
            {
                obj_MarkDetail = value;
            }
        }
        public Honeywell.LabelPrintDLL.ComPortSettings_t ComSetting
        {
            set
            {
                m_TestData.LabelPrintData.Label1ComportSettings = value;
            }
        }

        public string POFFile
        {
            set
            {
                if (value.IndexOf(".pof") < 0) value = value + ".POF";
                m_TestData.LabelPrintData.LabelPrintFile = value;
            }
        }


        public bool PrintLabel()
        {
            str_Err = "";
            string[] dataItem = new string[obj_MarkDetail.Length];
            string[] dataValue = new string[obj_MarkDetail.Length];

            Honeywell.LabelPrintDLL.cls_PrintLabels m_obj_Printer = new Honeywell.LabelPrintDLL.cls_PrintLabels();
            for(int i = 0 ;i < obj_MarkDetail.Length ; i++)
            {
                dataItem[i] = obj_MarkDetail[i].Item;
                dataValue[i] = obj_MarkDetail[i].Value;
            }

            m_TestData.LabelPrintData.ItemMarker = dataItem;
            m_TestData.LabelPrintData.ItemValue = dataValue;


            m_obj_Printer.p_TestData = m_TestData;

            if (System.IO.File.Exists(m_obj_Printer.p_TestData.LabelPrintData.LabelPrintFile) == false)
            {
                str_Err = "File :" + m_obj_Printer.p_TestData.LabelPrintData.LabelPrintFile + " not exist";
                return false;
            }

            if (m_obj_Printer.PrintLabel() == false)
            {
                str_Err = "Lable priter Fail";
                return false;
            }

            return true;
        }


    }
}
