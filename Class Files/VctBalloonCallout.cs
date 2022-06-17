using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD;
using Inventor;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public class VctBalloonCallOut
    {
        public ToolFolder ToolFolder { get; set; }
        public CalloutConfig.InputType InputType { get; set; }
        public bool ReadDetails()
        {
            CallOutDetailsReader.Instance.Refresh();
            return false;
        }

        private ViewModel _viewModel;

        public bool Start()
        {
            ToolFolder = new ToolFolder();

            //For testing purpose
            ToolFolder.Oem = "GM";
            ToolFolder.Supplier = "VAlIANT";
            ToolFolder.Division = "N/A";

            InputType = CallOutDetailsReader.Instance.GetBalloonCallOutsDetails(ToolFolder.Oem, ToolFolder.Supplier,
                ToolFolder.Division);
            if (InputType == null) return false;

            ReadDetails();

            StaticVariables.InventorOperations = new InventorOperations();
            if (!StaticVariables.InventorOperations.StartApplication()) return false;
            List<string> openedDrawings = StaticVariables.InventorOperations.GetOpenedModels();

            if (openedDrawings.Count > 0)
            {
                _viewModel = new ViewModel(openedDrawings);
                Drawings drawings = new Drawings(_viewModel);
                drawings.Button.Click += (s, e) =>
                {
                    foreach (var item in _viewModel.Items.Where(a => a.IsSelected))
                    {
                        _ = new VctPartList(item.Tooltip, InputType);
                    }
                    (((s as System.Windows.Controls.Button)?.Parent as System.Windows.Controls.Grid)?.Parent as
                        System.Windows.Window)?.Close();
                };
                drawings.ShowDialog();
            }
            else
            {
                foreach (string item in StaticVariables.InventorOperations.GetDrawingFilePaths(ToolFolder.ToolFolderPath))
                {
                    _ = new VctPartList(item, InputType);
                }
            }
            return true;
        }
    }

    public class ToolFolder
    {
        public string ToolFolderPath { get; set; }
        public string ToolAssembly { get; set; }
        public string Oem { get; set; }
        public string Division { get; set; }
        public string Supplier { get; set; }
        public string InventorProjectFilePath { get; set; }

        public ToolFolder()
        {
            if (!System.IO.Directory.Exists(StaticVariables.ToolPath)) return;
            string[] lines = System.IO.File.ReadAllLines(StaticVariables.ToolPath);
            if (lines.Length == 6)
            {
                ToolFolderPath = lines[0];
                ToolAssembly = lines[1];
                Oem = lines[2];
                Division = lines[3];
                Supplier = lines[4];
                InventorProjectFilePath = lines[5];
            }
        }
    }
}
