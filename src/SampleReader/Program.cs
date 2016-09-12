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
            Ifc.NET.Document ifcDocument;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ifc xml files (*.ifcxml)|*.ifcxml|All files (*.*)|*.*";
            dlg.InitialDirectory = Path.GetDirectoryName(typeof(SampleIfcReader.Program).Assembly.Location);
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ifcDocument = Ifc.NET.Workspace.CurrentWorkspace.OpenDocument(dlg.FileName);
            }
        }
    }
}
