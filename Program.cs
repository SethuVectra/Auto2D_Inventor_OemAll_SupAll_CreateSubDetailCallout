using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Class_Files.VctBalloonCallOut vctBalloonCallOut  = new Class_Files.VctBalloonCallOut();
            vctBalloonCallOut.Start();
        }
    }
}
