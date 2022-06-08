using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public static class StaticVariables
    {
        public static InventorOperations InventorOperations { get; set; }

        public const string ViewName = "B_ISO";
        public const string PartType = "V_01_PART_TYPE";
        public const string WeldedAssembly = "WELDED_ASS";

        public const string ToolPath = @"C:\Vectra\Auto2D_Output_Files\Config\Tool_Folder_Path.txt";
    }
}
