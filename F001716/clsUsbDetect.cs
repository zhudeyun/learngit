using System;  
using System.Management;  
using System.Text.RegularExpressions;  
using System.Collections.Generic;

namespace F001716
{  
    public struct PnPEntityInfo  
    {  
        public String PNPDeviceID;       
        public String Name;              
        public String Description;       
        public String Service;           
        public String Status;             
        public UInt16 VendorID;          
        public UInt16 ProductID;         
        public Guid ClassGuid;             
    }      
  
    public partial class clsUsbDetect  
    {        
        #region UsbDevice   
        /// <summary>   
        ///    
        /// </summary>   
        public static PnPEntityInfo[] AllUsbDevices  
        {  
            get  
           {  
                return WhoUsbDevice(UInt16.MinValue, UInt16.MinValue, Guid.Empty);  
            }  
        }  
  
        /// <summary>   
        /// Target Device(Need VID & PID) 
        /// </summary>   
        /// <param name="VendorID">VID</param>   
        /// <param name="ProductID">PID</param>   
        /// <param name="ClassGuid">Guid</param>   
        /// <returns>Device List</returns>   
        public static PnPEntityInfo[] WhoUsbDevice(UInt16 VendorID, UInt16 ProductID, Guid ClassGuid)  
        {  
            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();  
  
            //   
            ManagementObjectCollection USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get();  
            if (USBControllerDeviceCollection != null)  
            {  
                foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)  
                {   // Get DeviceID   
                    String Dependent = (USBControllerDevice["Dependent"] as String).Split(new Char[] { '=' })[1];

                    // Filtrate USB device which is without VID and PID.  
                    Match match = Regex.Match(Dependent, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");  
                    if (match.Success)  
                    {  
                        UInt16 theVendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Vender ID    
                        if (VendorID != UInt16.MinValue && VendorID != theVendorID) continue;  
  
                        UInt16 theProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product ID   
                        if (ProductID != UInt16.MinValue && ProductID != theProductID) continue;  
  
                        ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID=" + Dependent).Get();  
                        if (PnPEntityCollection != null)  
                        {  
                            foreach (ManagementObject Entity in PnPEntityCollection)  
                            {  
                                Guid theClassGuid = new Guid(Entity["ClassGuid"] as String);    // GUID   
                                if (ClassGuid != Guid.Empty && ClassGuid != theClassGuid) continue;  
  
                                PnPEntityInfo Element;  
                                Element.PNPDeviceID = Entity["PNPDeviceID"] as String;     
                                Element.Name = Entity["Name"] as String;                   
                                Element.Description = Entity["Description"] as String;   
                                Element.Service = Entity["Service"] as String;            
                                Element.Status = Entity["Status"] as String;              
                                Element.VendorID = theVendorID;       
                                Element.ProductID = theProductID;      
                                Element.ClassGuid = theClassGuid;      
  
                                UsbDevices.Add(Element);  
                            }  
                        }  
                    }  
                }  
            }  
  
            if (UsbDevices.Count == 0) return null; else return UsbDevices.ToArray();  
        }  
  
        /// <summary>   
        /// Target Device(Need VID & PID)    
        /// </summary>   
        /// <param name="VendorID">VID</param>   
        /// <param name="ProductID">PID</param>   
        /// <returns>Device List</returns>   
        public PnPEntityInfo[] WhoUsbDevice(UInt16 VendorID, UInt16 ProductID)  
        {
            return WhoUsbDevice(VendorID, ProductID, Guid.Empty);  
        }  
  
        /// <summary>   
        /// Target Device(Need VID & PID)    
        /// </summary>   
        /// <param name="ClassGuid">Guid，Empty Neglect</param>   
        /// <returns>Device List</returns>   
        public static PnPEntityInfo[] WhoUsbDevice(Guid ClassGuid)  
        {  
            return WhoUsbDevice(UInt16.MinValue, UInt16.MinValue, ClassGuid);  
        }  
  
        /// <summary>   
        ///  Target Device(Need VID & PID)   
        /// </summary>   
        /// <param name="PNPDeviceID">Device ID</param>   
        /// <returns>Device List</returns>           
        public static PnPEntityInfo[] WhoUsbDevice(String PNPDeviceID)  
        {  
            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();

            // Get USB Controller and the corresponding Devices.
            ManagementObjectCollection USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get();  
            if (USBControllerDeviceCollection != null)  
            {  
                foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)
                {   //  Get DeviceID   
                    String Dependent = (USBControllerDevice["Dependent"] as String).Split(new Char[] { '=' })[1];  
                    if (!String.IsNullOrEmpty(PNPDeviceID))
                    {   // Notes:Neglect Capital/Lowercase letter   
                        if (Dependent.IndexOf(PNPDeviceID, 1, PNPDeviceID.Length - 2, StringComparison.OrdinalIgnoreCase) == -1) continue;  
                    }

                    // Filtrate USB Devices which do not have VID &PID
                    Match match = Regex.Match(Dependent, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");  
                    if (match.Success)  
                    {  
                        ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID=" + Dependent).Get();  
                       if (PnPEntityCollection != null)  
                        {                              
                            foreach (ManagementObject Entity in PnPEntityCollection)  
                           {  
                                PnPEntityInfo Element;  
                                Element.PNPDeviceID = Entity["PNPDeviceID"] as String;   
                                Element.Name = Entity["Name"] as String;                   
                                Element.Description = Entity["Description"] as String;    
                                Element.Service = Entity["Service"] as String;            
                                Element.Status = Entity["Status"] as String;               
                                Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);         
                                Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16);  
                                Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);             
  
                                UsbDevices.Add(Element);  
                            }  
                        }  
                    }  
                }  
            }  
  
            if (UsbDevices.Count == 0) return null; else return UsbDevices.ToArray();  
        }  
  
        /// <summary>   
        /// Locate USB device by service  
        /// </summary>   
        /// <param name="ServiceCollection">Service convene we will query</param>   
        /// <returns>Device List</returns>   
        public static PnPEntityInfo[] WhoUsbDevice(String[] ServiceCollection)  
        {  
            if (ServiceCollection == null || ServiceCollection.Length == 0)  
                return WhoUsbDevice(UInt16.MinValue, UInt16.MinValue, Guid.Empty);  
  
            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();  
  
            // Get USB Controller and the corresponding Devices.   
            ManagementObjectCollection USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get();  
            if (USBControllerDeviceCollection != null)  
            {  
                foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)  
                {   // Get DeviceID   
                    String Dependent = (USBControllerDevice["Dependent"] as String).Split(new Char[] { '=' })[1];                      
  
                    // Filtrate USB Devices which do not have VID &PID   
                    Match match = Regex.Match(Dependent, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");  
                    if (match.Success)  
                    {  
                        ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID=" + Dependent).Get();  
        if (PnPEntityCollection != null)  
                        {  
                            foreach (ManagementObject Entity in PnPEntityCollection)  
                            {  
                                String theService = Entity["Service"] as String;            
                                if (String.IsNullOrEmpty(theService)) continue;  
  
                                foreach (String Service in ServiceCollection)  
                                {   // Notes:Neglect Capital/Lowercase letter
                                    if (String.Compare(theService, Service, true) != 0) continue;  
  
                                    PnPEntityInfo Element;  
                                    Element.PNPDeviceID = Entity["PNPDeviceID"] as String;     
                                    Element.Name = Entity["Name"] as String;                 
                                    Element.Description = Entity["Description"] as String;     
                                    Element.Service = theService;                             
                                    Element.Status = Entity["Status"] as String;               
                                    Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);        
                                    Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16);    
                                    Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);               
  
                                    UsbDevices.Add(Element);  
                                    break;  
                                }  
                            }  
                        }  
                    }  
                }  
            }  
  
            if (UsbDevices.Count == 0) return null; else return UsbDevices.ToArray();  
        }  
        #endregion  
 
        #region PnPEntity   
        /// <summary>   
        ///   
        /// </summary>   
        public static PnPEntityInfo[] AllPnPEntities  
        {  
            get  
            {  
                return WhoPnPEntity(UInt16.MinValue, UInt16.MinValue, Guid.Empty);  
            }  
        }  
  
        /// <summary>   
        ///  
        /// </summary>   
        /// <param name="VendorID"></param>   
        /// <param name="ProductID"></param>   
        /// <param name="ClassGuid"></param>   
        /// <returns>Device List</returns>   
        /// <remarks>   
        /// HID：{745a17a0-74d3-11d0-b6fe-00a0c90f57da}   
        /// Imaging Device：{6bdd1fc6-810f-11d0-bec7-08002be2092f}   
        /// Keyboard：{4d36e96b-e325-11ce-bfc1-08002be10318}    
        /// Mouse：{4d36e96f-e325-11ce-bfc1-08002be10318}   
        /// Network Adapter：{4d36e972-e325-11ce-bfc1-08002be10318}   
        /// USB：{36fc9e60-c465-11cf-8056-444553540000}   
        /// </remarks>   
        public static PnPEntityInfo[] WhoPnPEntity(UInt16 VendorID, UInt16 ProductID, Guid ClassGuid)  
        {  
            List<PnPEntityInfo> PnPEntities = new List<PnPEntityInfo>();  
  
            // Enmu PnP Device 
            String VIDPID;  
            if (VendorID == UInt16.MinValue)  
            {  
                if (ProductID == UInt16.MinValue)  
                    VIDPID = "'%VID[_]____&PID[_]____%'";  
                else  
                    VIDPID = "'%VID[_]____&PID[_]" + ProductID.ToString("X4") + "%'";         
            }  
            else  
            {  
                if (ProductID == UInt16.MinValue)  
                    VIDPID = "'%VID[_]" + VendorID.ToString("X4") + "&PID[_]____%'";  
                else  
                    VIDPID = "'%VID[_]" + VendorID.ToString("X4") + "&PID[_]" + ProductID.ToString("X4") + "%'";  
            }  
  
            String QueryString;  
            if (ClassGuid == Guid.Empty)  
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE" + VIDPID;  
            else  
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE" + VIDPID + " AND ClassGuid='" + ClassGuid.ToString("B") + "'";  
  
            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();  
            if (PnPEntityCollection != null)  
            {  
                foreach (ManagementObject Entity in PnPEntityCollection)  
                {  
                    String PNPDeviceID = Entity["PNPDeviceID"] as String;  
                    Match match = Regex.Match(PNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");  
                    if (match.Success)  
                    {  
                        PnPEntityInfo Element;  
  
                        Element.PNPDeviceID = PNPDeviceID;                        
                        Element.Name = Entity["Name"] as String;                   
                        Element.Description = Entity["Description"] as String;     
                        Element.Service = Entity["Service"] as String;            
                        Element.Status = Entity["Status"] as String;              
                        Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);     
                        Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16);   
                        Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            
  
                        PnPEntities.Add(Element);  
                    }  
                }  
            }  
  
            if (PnPEntities.Count == 0) return null; else return PnPEntities.ToArray();  
        }    
        
        /// <summary>   
        ///    
       /// </summary>   
        /// <param name="VendorID"></param>   
        /// <param name="ProductID"></param>   
        /// <returns></returns>   
        public static PnPEntityInfo[] WhoPnPEntity(UInt16 VendorID, UInt16 ProductID)  
        {  
            return WhoPnPEntity(VendorID, ProductID, Guid.Empty);  
        }  
  
        /// <summary>   
        ///    
       /// </summary>   
        /// <param name="ClassGuid"></param>   
        /// <returns>Device List</returns>   
        public static PnPEntityInfo[] WhoPnPEntity(Guid ClassGuid)  
        {  
            return WhoPnPEntity(UInt16.MinValue, UInt16.MinValue, ClassGuid);  
        }  
  
       /// <summary>   
        /// Loacte Device by Device ID   
        /// </summary>   
        /// <param name="PNPDeviceID">DeviceID，Non-complate info</param>   
        /// <returns>Device List</returns>   
        /// <remarks>     
        /// </remarks>   
        public static PnPEntityInfo[] WhoPnPEntity(String PNPDeviceID)  
        {  
            List<PnPEntityInfo> PnPEntities = new List<PnPEntityInfo>();  
  
            // 
            String QueryString;  
            if (String.IsNullOrEmpty(PNPDeviceID))  
            {  
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '%VID[_]____&PID[_]____%'";  
            }  
            else  
            {   // 
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '%" + PNPDeviceID.Replace('\\', '_') + "%'";  
            }  
  
            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();  
            if (PnPEntityCollection != null)  
            {  
                foreach (ManagementObject Entity in PnPEntityCollection)  
                {  
                    String thePNPDeviceID = Entity["PNPDeviceID"] as String;  
                    Match match = Regex.Match(thePNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");  
                    if (match.Success)  
                    {  
                        PnPEntityInfo Element;  
  
                        Element.PNPDeviceID = thePNPDeviceID;                    
                        Element.Name = Entity["Name"] as String;                  
                        Element.Description = Entity["Description"] as String;   
                        Element.Service = Entity["Service"] as String;          
                        Element.Status = Entity["Status"] as String;              
                        Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);     
                        Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16);  
                        Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);              
  
                        PnPEntities.Add(Element);  
                    }  
                }  
            }  
  
            if (PnPEntities.Count == 0) return null; else return PnPEntities.ToArray();  
        }  
  
        /// <summary>   
        ///  
        /// </summary>   
       /// <param name="ServiceCollection"></param>   
        /// <returns>Device List</returns>   
        /// <remarks>   
        ///   
        ///     Win32_SystemDriverPNPEntity   
        ///     Win32_SystemDriver   
        /// </remarks>   
        public static PnPEntityInfo[] WhoPnPEntity(String[] ServiceCollection)  
        {  
            if (ServiceCollection == null || ServiceCollection.Length == 0)  
                return WhoPnPEntity(UInt16.MinValue, UInt16.MinValue, Guid.Empty);  
  
            List<PnPEntityInfo> PnPEntities = new List<PnPEntityInfo>();  
  
            // Enmu  
            String QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '%VID[_]____&PID[_]____%'";  
            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();  
            if (PnPEntityCollection != null)  
           {  
                foreach (ManagementObject Entity in PnPEntityCollection)  
                {  
                    String PNPDeviceID = Entity["PNPDeviceID"] as String;  
                    Match match = Regex.Match(PNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");  
                    if (match.Success)  
                    {  
                        String theService = Entity["Service"] as String;           
                        if (String.IsNullOrEmpty(theService)) continue;  
  
                       foreach (String Service in ServiceCollection)  
                        {    
                            if (String.Compare(theService, Service, true) != 0) continue;  
  
                            PnPEntityInfo Element;  
 
                           Element.PNPDeviceID = PNPDeviceID;                      
                            Element.Name = Entity["Name"] as String;                  
                            Element.Description = Entity["Description"] as String;    
                            Element.Service = theService;                           
                            Element.Status = Entity["Status"] as String;             
                            Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);  
                            Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16);  
                            Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            
  
                            PnPEntities.Add(Element);  
                            break;  
                        }  
                    }  
                }  
            }  
  
            if (PnPEntities.Count == 0) return null; else return PnPEntities.ToArray();  
        }  
        #endregion           
   }  
}  



