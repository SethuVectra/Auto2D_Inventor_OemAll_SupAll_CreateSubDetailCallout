using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD
{
    public interface ICADOperations
    {
        
        bool StartApplication();        
        List<string> GetDrawingFilePaths();
        bool UpdateSettings(CalloutConfig.InputType CalloutDetails);
        bool DeletePartList();

        object GetView();
        object PlacePartList();
        bool PlaceBalloonCallouts(object PartList, VctBalloons VctBalloons);

    }
}
