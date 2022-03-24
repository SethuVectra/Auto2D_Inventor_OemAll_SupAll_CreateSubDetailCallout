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
            ReadCalloutDetails = XmlHelper.DeserializeXMLFileToObject<CalloutDetails>(System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "CalloutDetails.xml"));
        }

        private static CalloutDetailsReader instance;
        public static CalloutDetailsReader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CalloutDetailsReader();
                }
                return instance;
            }
        }

        private CalloutDetails ReadCalloutDetails { get; }

        public bool Refresh()
        {
            try
            {
                instance = new CalloutDetailsReader();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public InputType GetBallonCalloutsDetails(string Oem, string Supplier, string Division)
        {
            if (ReadCalloutDetails == null) return null;
            return null;
        }
    }
}
