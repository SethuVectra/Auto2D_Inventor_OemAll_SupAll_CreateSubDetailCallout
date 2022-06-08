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
            ReadCallOutDetails = XmlHelper.DeserializeXmlFileToObject<CalloutDetails>(System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "CalloutDetails.xml"));
            //ReadCallOutDetails.Update();
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
                var oems = ReadCallOutDetails.Oem.Where(a=>a.Name.Equals(oem,StringComparison.InvariantCultureIgnoreCase));
                if (oems.Any())
                {
                    var suppliers = oems.First().Supplier.Where(a => a.Name.Equals(supplier, StringComparison.InvariantCultureIgnoreCase));
                    if (suppliers.Any())
                    {
                        var divisions = suppliers.First().Division.Where(a => a.Name.Equals(division, StringComparison.InvariantCultureIgnoreCase));
                        if (divisions.Any())
                            return divisions.First().InputType[0];
                        else
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
