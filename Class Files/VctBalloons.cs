using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public class VctBalloons
    {

        public VctBalloons()
        {
            PlaceBalloonCallouts();
        }

        public List<VctBalloon> BalloonCallouts { get; set; }

        public bool Update()//
        {
            return false;
        }

        public bool PlaceBalloonCallouts()
        {
            try
            {
                BalloonCallouts = StaticVariables.InventorOperations.AddBallonToView();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
