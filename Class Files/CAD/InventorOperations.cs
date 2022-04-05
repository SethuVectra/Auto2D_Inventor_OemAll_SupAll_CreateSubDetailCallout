using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig;
using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD
{
    public class InventorOperations : ICADOperations
    {
        string ViewName = "B_LCS";
        private Inventor.Application Application;
        private PartsListLevelEnum PartsListLevelEnum;

        public bool DeletePartList()
        {
            try
            {
                foreach (PartsList item in ActiveSheet.PartsLists)
                    item.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Sheet ActiveSheet =>
             (Application.ActiveDocument as DrawingDocument).ActiveSheet;

        public List<string> GetDrawingFilePaths()
        {
            throw new NotImplementedException();
        }

        public object GetView()
        {
            if (Application == null) return null;
            foreach (DrawingView item in ActiveSheet.DrawingViews)
            {
                if (item.Name == ViewName) return item;
            }
            return null;
        }

        //public bool PlaceBalloonCallouts(object PartList, VctBalloons VctBalloons)
        //{
        //    ActiveSheet.Balloons.Add()
        //}

        public object PlacePartList()
        {
            return ActiveSheet.PartsLists.Add(GetView(), ActiveSheet.Border.RangeBox.MaxPoint, PartsListLevelEnum);
            //Application.TransientGeometry.CreatePoint2d(ActiveSheet.Width - 1, ActiveSheet.Height - 1), PartsListLevelEnum);
        }

        public bool StartApplication()
        {
            try
            {
                Application = System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                try
                {
                    Application = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("Inventor.Application")) as Inventor.Application;
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }



        public bool UpdateSettings(InputType CalloutDetails)
        {
            Inventor.Document oDocument = null;
            AssemblyDocument oAssemblyDocument = oDocument as AssemblyDocument;
            if (oAssemblyDocument == null) return false;

            TableSettings tableSettings = CalloutDetails.TableSettings;
            BOM oBOM = oAssemblyDocument.ComponentDefinition.BOM;

            if (tableSettings.BOMView == "Parts Only")
            {
                oBOM.PartsOnlyViewEnabled = true;
                oBOM.PartsOnlyViewNumberingScheme = tableSettings.Numbering == "Numeric" ? NumberingSchemeEnum.kNumericNumbering : NumberingSchemeEnum.kUppercaseAlphaNumbering;
                oBOM.PartsOnlyViewMinimumDigits = tableSettings.MinDigits;
                PartsListLevelEnum = PartsListLevelEnum.kPartsOnly;
            }
            else if (tableSettings.BOMView == "Structured")
            {
                oBOM.StructuredViewEnabled = true;
                oBOM.StructuredViewFirstLevelOnly = tableSettings.Levels == "First Level";
                oBOM.StructuredViewMinimumDigits = tableSettings.MinDigits;
                if (tableSettings.Levels == "First Level")
                    PartsListLevelEnum = PartsListLevelEnum.kStructured;
                else
                    PartsListLevelEnum = PartsListLevelEnum.kStructuredAllLevels;
            }

            DrawingDocument oDrawingDocument = oDocument as DrawingDocument;
            if (oDrawingDocument == null) return false;

            PartsList Partlist = oDrawingDocument.Sheets[0].PartsLists[0];

            //if (tableSettings.BOMView == "Parts Only")
            //{
            //    //Partlist.Level = PartsListLevelEnum.kPartsOnly;                
            //}

            bool IsAutoWrap = Convert.ToBoolean(tableSettings.AutomaticWrap);
            Partlist.WrapAutomatically = IsAutoWrap;
            Partlist.WrapLeft = tableSettings.TextWrapingDirection == "Left";
            if (IsAutoWrap)
            {
                Partlist.MaximumRows = tableSettings.MaximumRows;
                Partlist.NumberOfSections = tableSettings.NumberOfSections;
            }

            return true;
        }
    }
}
