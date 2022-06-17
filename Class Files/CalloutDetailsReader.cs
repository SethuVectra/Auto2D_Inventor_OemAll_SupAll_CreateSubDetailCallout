using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public sealed class CallOutDetailsReader
    {
        private CallOutDetailsReader()
        {
            ReadCallOutDetails = XmlHelper.DeserializeXmlFileToObject<CalloutDetails>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "CalloutDetails.xml"));
            ReadCallOutDetails.Update();
        }

        private static CallOutDetailsReader _instance;
        public static CallOutDetailsReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CallOutDetailsReader();
                }
                return _instance;
            }
        }

        private CalloutDetails ReadCallOutDetails { get; }

        public bool Refresh()
        {
            try
            {
                _instance = new CallOutDetailsReader();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public InputType GetBalloonCallOutsDetails(string oem, string supplier, string division)
        {
            if (ReadCallOutDetails != null)
            {
                if (ReadCallOutDetails.Data.ContainsKey(oem))
                {
                    var suppliers = ReadCallOutDetails.Data[oem];
                    if (suppliers.Data.ContainsKey(supplier))
                    {
                        var divisions = suppliers.Data[supplier];
                        if (divisions.Data.ContainsKey(division))
                            return divisions.Data[division].InputType[0];
                        LogWriter.LogWrite("Division is not available in the Config file");
                    }
                    else
                        LogWriter.LogWrite("Supplier is not available in the Config file");
                }
                else
                    LogWriter.LogWrite("Oem is not available in the Config file");
            }
            return null;
        }
    }
}
