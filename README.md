# Ifc.NET
Simple c# classes to read and write a file in the format IFC 4 XML.

Start using Ifc.NET with NUGET (https://www.nuget.org/packages/Ifc.NET). To install Ifc.NET in your application, run the following command in the Package Manager Console in Visual Studio: "Install-Package Ifc.NET"

Your solution must run on the .NET Framework 4.6.1.

To read a file, please follow this steps:  


***

Download an ifcxml sample file from here: https://github.com/CAFM-Connect/Ifc.NET/tree/master/src/SampleReader/SampleFiles and copy it to your computer.  

Paste this code snippet into you code:  

`string Testfile = @"C:\Tmp\MyFirstIfcFile.ifcxml";`  
`Ifc.NET.Document ifcDocument = Ifc.NET.Workspace.CurrentWorkspace.OpenDocument(Testfile);`  

Adjust the path to the file to the location, where you copied the sample file.  
  
Run your application in Debug and look at the object `ifcDocument`.  

***

For further information about the file spectification please visit  http://www.buildingsmart-tech.org/ifc/IFC4/final/html/index.htm