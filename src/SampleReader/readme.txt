Hinweise von Joachim Vollberg:
Das Erstellen von Ifc... Gebäudestrukturen funktioniert anders
Wenn einzelne Ifc Instanzen erstellt werden, müsste man manuell dafür sorgen,
dass die Ifc Struktur in den z.B. IfcRelAggregates richtig befüllt werden.
Daher ist die Struktur in den Klassen abgebildet
und es können die Methoden .AddNew... an den Listen benutzt werden
 
Eine eigene Workspace Klasse wird nicht benötigt.
 
Das Beispiel muss in einem eigenem Projekt mit einer Referenz auf das Ifc dll prgrammiert werden,
sonst könnte man auch die als internal definierter Properties, Klassen, Methoden benutzen,
was zu Fehlern führen kann.
 
 
// *****************************************************************
//Beispiel erstellt eine Gebaeudestruktur
-Liegenschaft
-Gebaeude
  -Etage
   -Raum
 
string ifcFullName = @"C:\tmp\a.ifcxml";
  
// OHNE CafmConnectTemplate Datei
etask.Ifc4.Document document = etask.Ifc4.Workspace.CurrentWorkspace.CreateDocument(ifcFullName, true);
 
// MIT CafmConnectTemplate Datei
// string ifcTemplateFullName = @"C:\tmp\CAFM-ConnectFacilitiesViewTemplate.ifcxml";
// System.IO.File.Copy(ifcTemplateFullName, ifcFullName, true);
// document = etask.Ifc4.Workspace.CurrentWorkspace.OpenDocument(ifcTemplateFullName);
 
 
// so koennen die Properties am Header geändert werden.
var header = document.IfcXmlDocument.Header;
header.Organization = "eTASK";
 
// neue Liegenschaft
var ifcSite = document.Project.Sites.AddNewSite();
ifcSite.LongName = "A";
// ...
// neues Gebaeude
var ifcBuilding = ifcSite.Buildings.AddNewBuilding();
ifcBuilding.LongName = "B";
// ...
 
// neue Etage
var ifcBuildingStorey = ifcBuilding.BuildingStoreys.AddNewBuildingStorey();
ifcBuildingStorey.LongName = "C";
// ...
 
// neuer Raum
var ifcSpace = ifcBuildingStorey.Spaces.AddNewSpace();
ifcSpace.LongName = "D";
// ...
 
 
// IFC Datei speichern
document.Workspace.SaveDocument(ifcFullName);
// *****************************************************************
 
 
Noch ein anderes Thema - die Veröffentlichung https://github.com/CAFM-Connect/Ifc.NET
Der Namespace falsch und muss geändert werden. So wird die inneren Klassenstrukturen zerstört.
Die Ifc Klassen sind ein subfeature von Ifc und nicht von .NET.
Der Namespace .NET sollte auch nicht verwendet werden, da es eine System.Net gibt.
 
Namspace = CompanyName.TechnologyName[.Feature][.Design]
Für sämtliche Namen in .NET gilt die Camel Case Schreibweise.
Akronyme
- mit zwei Zeichen werden groß geschrieben
- mit mehr als zwei Zeichen werden nach Camel Case Verfahren geschrieben
 
z.B.
CafmConnect.Ifc.
CC.Ifc.
 
...
 
 