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
        public VctPartList(string filePath, InputType callOutDetails)
        {
            this.CallOutDetails = callOutDetails;
            if(StaticVariables.InventorOperations.UpdateSettings(filePath, CallOutDetails))
                AddPartList();
            else
                LogWriter.LogWrite("Unable to find for the Welded Assembly : " + filePath);
        }
        public InputType CallOutDetails { get; set; }

        public void AddPartList()
        {
            //Delete PartList
            StaticVariables.InventorOperations.DeletePartList();

            //Insert new PartList
            StaticVariables.InventorOperations.PlacePartList(CallOutDetails.GeneralSettings.PartListType, CallOutDetails.TableData.PartListTopLeftPositionX, CallOutDetails.TableData.PartListTopLeftPositionY, CallOutDetails.TableSettings);

            Process();
        }

        public VctBalloons BalloonCallOuts { get; set; }

        public bool Process()
        {
            if (StaticVariables.InventorOperations.IsPartslistPlaced)
            {
                BalloonCallOuts = new VctBalloons();
            }
            return false;
        }
    }
}
