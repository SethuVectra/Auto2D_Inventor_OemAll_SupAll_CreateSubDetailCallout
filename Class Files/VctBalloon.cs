using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public class VctBalloon
    {
        public VctBalloon()
        {

        }

        public string PartNumber { get; set; }

        public double[] BalloonPosition { get; set; }

        public double[] LeaderPoint { get; set; }

        public double CircleSize { get; set; }

        public VctBalloonZone Zone { get; set; }

        public object DrawingCurve { get; set; }
    }
}
