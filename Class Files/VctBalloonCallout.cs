using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public class VctBalloonCallout
    {
        public List<string> FilesPathToProcess { get; set; }
        public bool ReadDetails()
        {
            CalloutDetailsReader.Instance.Refresh();
            return false;
        }

        public bool Start()
        {
            ReadDetails();
            return false;
        }
    }
}
