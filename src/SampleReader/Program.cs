using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleIfcReader
{
    class Program
    {
        [STAThreadAttribute()]
        static void Main(string[] args)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ifc xml files (*.ifcxml)|*.ifcxml|All files (*.*)|*.*";
            dlg.InitialDirectory = Path.GetDirectoryName(typeof(SampleIfcReader.Program).Assembly.Location);
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Ifc4.Document ifcDocument = Ifc4.Workspace.CurrentWorkspace.OpenDocument(dlg.FileName);

                var header = ifcDocument.IfcXmlDocument.Header;
                header.Organization = "My Organisation";

                var ifcSite = ifcDocument.Project.Sites.AddNewSite();
                ifcSite.LongName = "Site";

                var ifcBuilding = ifcSite.Buildings.AddNewBuilding();
                ifcBuilding.LongName = "Building";

                var ifcBuildingStorey = ifcBuilding.BuildingStoreys.AddNewBuildingStorey();
                ifcBuildingStorey.LongName = "Storey";

                var ifcSpace = ifcBuildingStorey.Spaces.AddNewSpace();
                ifcSpace.LongName = "Room";

                ifcDocument.Workspace.SaveDocument(dlg.FileName);
            }
        }
    }
}
