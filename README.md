# Ifc4
Simple c# classes to read and write a file in the format IFC 4 XML.

Start using Ifc4 with NUGET (https://www.nuget.org/packages/Ifc4). To install Ifc4 in your application, run the following command in the Package Manager Console in Visual Studio: "Install-Package Ifc4"

Your solution must run on the .NET Framework 4.6.1.

To read a file, please follow this steps:  


***

Download an ifcxml sample file from here: https://github.com/CAFM-Connect/Ifc4/tree/master/src/SampleReader/SampleFiles and copy it to your computer.  

Paste this code snippet into you code:  

`string Testfile = @"C:\Tmp\MyFirstIfcFile.ifcxml";`  
`Ifc4.Document ifcDocument = Ifc4.Workspace.CurrentWorkspace.OpenDocument(Testfile);`  

Adjust the path to the file to the location, where you copied the sample file.  
  
Run your application in Debug and look at the object `ifcDocument`.  

***

For further information about the file spectification please visit  http://www.buildingsmart-tech.org/ifc/IFC4/final/html/index.htm


***

## Some more Samples   

### Sample for a spatial structure site/building/buildingstorey/room   

`string ifcFullName = @"C:\tmp\a.ifcxml";`  
`etask.Ifc4.Document document = etask.Ifc4.Workspace.CurrentWorkspace.CreateDocument(ifcFullName, true);`  

### Change some properties in header   

`var header = document.IfcXmlDocument.Header;`  
`header.Organization = "My Organisation";`  
 
### Add new Site   

`var ifcSite = document.Project.Sites.AddNewSite();`
`ifcSite.LongName = "A";`

### Add new building   

`var ifcBuilding = ifcSite.Buildings.AddNewBuilding();`
`ifcBuilding.LongName = "B";`
 
### Add new building storey

`var ifcBuildingStorey = ifcBuilding.BuildingStoreys.AddNewBuildingStorey();`   
`ifcBuildingStorey.LongName = "C";`   
 
### Add new room

`var ifcSpace = ifcBuildingStorey.Spaces.AddNewSpace();`  
`ifcSpace.LongName = "D";`  

 
### Save the IFC file   

`document.Workspace.SaveDocument(ifcFullName);`

***