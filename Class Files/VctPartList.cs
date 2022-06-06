using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public class VctPartList
    {
        public VctPartList(string filePath, object view, string oem, string supplier, string division)
        {

        }
        public InputType CalloutDetails { get; set; }

        public object GetPartList()
        {
            return null;
        }

        public VctBalloons BallonCallouts { get; set; }

        public bool Process()
        {
            return false;
        }
    }
}
