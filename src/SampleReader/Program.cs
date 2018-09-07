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
            OpenFileDialog dlgFile = new OpenFileDialog();
            dlgFile.Filter = "Ifc xml files (*.ifcxml)|*.ifcxml|All files (*.*)|*.*";
            dlgFile.InitialDirectory = Path.GetDirectoryName(typeof(SampleIfcReader.Program).Assembly.Location);
            dlgFile.CheckFileExists = true;
            dlgFile.Multiselect = false;
            dlgFile.Title = "Select the file, you want to open...";


            FolderBrowserDialog dlgNewFolder = new FolderBrowserDialog();
            dlgNewFolder.Description = "Select the target folder, for your file";

            if (dlgFile.ShowDialog() == DialogResult.OK)
            {
                Ifc4.Document ifcDocument = Ifc4.Workspace.CurrentWorkspace.OpenDocument(dlgFile.FileName);

                string targetFolder=string.Empty;
                if (dlgNewFolder.ShowDialog() == DialogResult.OK)
                {
                    targetFolder = dlgNewFolder.SelectedPath;
                }
                var header = ifcDocument.IfcXmlDocument.Header;
                header.Organization = "My Organisation";
                header.Documentation = "https://github.com/CAFM-Connect/Ifc.NET";

                var ifcSite = ifcDocument.Project.Sites.AddNewSite();
                ifcSite.LongName = "Site";

                var ifcBuilding = ifcSite.Buildings.AddNewBuilding();
                ifcBuilding.LongName = "Building";

                var ifcBuildingStorey = ifcBuilding.BuildingStoreys.AddNewBuildingStorey();
                ifcBuildingStorey.LongName = "Storey";

                var ifcSpace = ifcBuildingStorey.Spaces.AddNewSpace();
                ifcSpace.LongName = "Room";

                ifcDocument.Workspace.SaveDocument(dlgFile.FileName);

                Console.WriteLine(string.Format("New file saved as {0}", Path.Combine(targetFolder,Path.GetFileName(dlgFile.FileName))));
                Console.ReadLine();
            }
        }
    }
}
