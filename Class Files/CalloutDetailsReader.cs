using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public sealed class CalloutDetailsReader
    {
        private CalloutDetailsReader()
        {
            ReadCalloutDetails = XmlHelper.DeserializeXmlFileToObject<CalloutDetails>(System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "CalloutDetails.xml"));
        }

        private static CalloutDetailsReader _instance;
        public static CalloutDetailsReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CalloutDetailsReader();
                }
                return _instance;
            }
        }

        private CalloutDetails ReadCalloutDetails { get; }

        public bool Refresh()
        {
            try
            {
                _instance = new CalloutDetailsReader();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public InputType GetBallonCalloutsDetails(string oem, string supplier, string division)
        {
            if (ReadCalloutDetails == null) return null;
            return null;
        }
    }
}
