using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public class Workspace : BaseObject
    {
        private Workspace()
        {
            Logging = true;
        }

        public bool Logging { get; set; }

        public delegate void BeforeCommandExecutedEventHandler(object sender, BeforeCommandExecutedEventArgs e);
        public event BeforeCommandExecutedEventHandler BeforeCommandExecuted;

        public delegate void AfterCommandExecutedEventHandler(object sender, AfterCommandExecutedEventArgs e);
        public event AfterCommandExecutedEventHandler AfterCommandExecuted;

        public delegate void DocumentOpenedEventHandler(object sender, DocumentOpenedEventArgs e);
        public event DocumentOpenedEventHandler DocumentOpened;

        public delegate void ReportCreatedEventHandler(object sender, ReportCreatedEventArgs e);
        public event ReportCreatedEventHandler ReportCreated;

        public delegate void DocumentSavedEventHandler(object sender, DocumentSavedEventArgs e);
        public event DocumentSavedEventHandler DocumentSaved;

        public delegate void ActiveDocumentChangedEventHandler(object sender, ActiveDocumentChangedEventArgs e);
        public event ActiveDocumentChangedEventHandler ActiveDocumentChanged;

        public delegate void DocumentClosedEventHandler(object sender, DocumentClosedEventArgs e);
        public event DocumentClosedEventHandler DocumentClosed;

        public delegate void DocumentModifiedEventHandler(object sender, DocumentModifiedEventArgs e);
        public event DocumentModifiedEventHandler DocumentModfied;

        public delegate void ObjectSelectedEventHandler(object sender, ObjectSelectedEventArgs e);
        public event ObjectSelectedEventHandler ObjectSelected;

        public delegate void PropertySelectedEventHandler(object sender, PropertySelectedEventArgs e);
        public event PropertySelectedEventHandler PropertySelected;

        public delegate void MessageLoggedEventHandler(object sender, Ifc4.EventArgs.MessageLoggedEventArgs e);
        public event MessageLoggedEventHandler MessageLogged; 

        public delegate void MessagesClearedEventHandler(object sender, MessageClearedEventArgs e);
        public event MessagesClearedEventHandler MessagesCleared;

        private static Workspace _Workspace;
        public static Workspace CurrentWorkspace
        {
            get
            {
                if (_Workspace == null)
                {
                    _Workspace = new Workspace();
                }
                return _Workspace;
            }
        }

        public void RaiseActiveDocumentChanged(object sender, Workspace.ActiveDocumentChangedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.ActiveDocumentChanged != null)
                Workspace.CurrentWorkspace.ActiveDocumentChanged(sender, e);
        }

        public void RaiseDocumentOpened(object sender, Workspace.DocumentOpenedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.DocumentOpened != null)
                Workspace.CurrentWorkspace.DocumentOpened(sender, e);
        }

        public void RaiseDocumentSaved(object sender, Workspace.DocumentSavedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.DocumentSaved != null)
                Workspace.CurrentWorkspace.DocumentSaved(sender, e);
        }

        public void RaiseDocumentClosed(object sender, Workspace.DocumentClosedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.DocumentClosed != null)
                Workspace.CurrentWorkspace.DocumentClosed(sender, e);
        }

        public void RaiseObjectSelected(object sender, Workspace.ObjectSelectedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.ObjectSelected != null)
                Workspace.CurrentWorkspace.ObjectSelected(sender, e);
        }

        public void RaisePropertySelected(object sender, Workspace.PropertySelectedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.PropertySelected != null)
                Workspace.CurrentWorkspace.PropertySelected(sender, e);
        }

        public void RaiseReportCreated(object sender, Workspace.ReportCreatedEventArgs e)
        {
            if (Workspace.CurrentWorkspace.ReportCreated != null)
                Workspace.CurrentWorkspace.ReportCreated(sender, e);
        }

        public void RaiseBeforeCommandExceuted(object sender, string command)
        {
            if (Workspace.CurrentWorkspace.BeforeCommandExecuted != null)
                Workspace.CurrentWorkspace.BeforeCommandExecuted(sender, new BeforeCommandExecutedEventArgs(command));
        }

        public void RaiseAfterCommandExceuted(object sender, string command)
        {
            if (Workspace.CurrentWorkspace.AfterCommandExecuted != null)
                Workspace.CurrentWorkspace.AfterCommandExecuted(sender, new AfterCommandExecutedEventArgs(command));
        }

        //public void RaiseMessageLogged(object sender, Ifc4.EventArgs.MessageLoggedEventArgs e)
        //{
        //    RaiseMessageLogged(sender, e.Message, String.Empty, false);
        //}

        public void RaiseMessageLogged(object sender, string message)
        {
            RaiseMessageLogged(sender, message, String.Empty, false, false);
        }

        public void RaiseMessageLogged(object sender, string message, bool addDateTime)
        {
            RaiseMessageLogged(sender, message, String.Empty, false, addDateTime);
        }

        public void RaiseMessageLogged(string message)
        {
            RaiseMessageLogged(null, message, String.Empty, false, false);
        }

        public void RaiseMessageLogged(string message, bool doEvents)
        {
            RaiseMessageLogged(null, message, String.Empty, doEvents, false);
        }

        public void RaiseMessageLogged(string message, string link)
        {
            RaiseMessageLogged(null, message, link, false, false);
        }

        public void RaiseMessageLogged(object sender, string message, string link, bool doEvents, bool addDateTime)
        {
            if (!Logging)
                return;

            if (CurrentWorkspace.MessageLogged != null)
            {
                if(addDateTime)
                    CurrentWorkspace.MessageLogged(sender, new Ifc4.EventArgs.MessageLoggedEventArgs(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(message), EventArgs.MessageLogType.Info, doEvents, false));
                else
                    CurrentWorkspace.MessageLogged(sender, new Ifc4.EventArgs.MessageLoggedEventArgs(message, EventArgs.MessageLogType.Info, doEvents, false));
            }
        }

        public void RaiseMessageLogged(object sender, Exception exc)
        {
            // Exception werden immer geloggt
            if (exc != null || Logging)
            {
                if (CurrentWorkspace.MessageLogged != null)
                    CurrentWorkspace.MessageLogged(sender, new Ifc4.EventArgs.MessageLoggedEventArgs(exc));
            }
        }

        public void RaiseMessageLogged(Exception exc)
        {
            // Exception werden immer geloggt
            if (exc != null || Logging)
            {
                if (CurrentWorkspace.MessageLogged != null)
                    CurrentWorkspace.MessageLogged(this, new Ifc4.EventArgs.MessageLoggedEventArgs(exc));
            }
        }

        public void RaiseMessagesCleared(object sender)
        {
            if (CurrentWorkspace.MessagesCleared != null)
                CurrentWorkspace.MessagesCleared(sender, new MessageClearedEventArgs());
        }

        public void RaiseDocumentModified(object sender, Workspace.DocumentModifiedEventArgs e)
        {
            e.Document.IsDirty = true;
            if (CurrentWorkspace.DocumentModfied != null)
                CurrentWorkspace.DocumentModfied(sender, e);
        }

        public class ObjectSelectedEventArgs : System.EventArgs
        {
            public ObjectSelectedEventArgs(BaseObject baseObject)
            {
                CcObject = baseObject;
            }
            public BaseObject CcObject { get; private set; }
        }

        public class PropertySelectedEventArgs : System.EventArgs
        {
            public PropertySelectedEventArgs(PropertyDescriptor propertyDescriptor)
            {
                PropertyDescriptor = propertyDescriptor;
            }
            public PropertyDescriptor PropertyDescriptor { get; private set; }
        }

        public class DocumentEventArgs : System.EventArgs
        {
            public DocumentEventArgs(Document document)
            {
                Document = document;
            }
            public Document Document { get; private set; }
            public string FullName { get { return Document == null ? String.Empty : Document.FullName; } }
        }

        public class DocumentOpenedEventArgs : DocumentEventArgs
        {
            public DocumentOpenedEventArgs(Document document)
                : base(document)
            {
            }
        }

        public class DocumentSavedEventArgs : DocumentEventArgs
        {
            public DocumentSavedEventArgs(Document document)
                : base(document)
            {
            }
        }

        public class DocumentModifiedEventArgs : DocumentEventArgs
        {
            public DocumentModifiedEventArgs(Document document)
                : base(document)
            {
            }
        }

        public class DocumentClosedEventArgs : DocumentEventArgs
        {
            public DocumentClosedEventArgs(Document document)
                : base(document)
            {
            }
        }

        public class CommandEventArgs : System.EventArgs
        {
            public CommandEventArgs(string command)
            {
                Command = command;
            }
            public string Command { get; private set; }
        }

        public class BeforeCommandExecutedEventArgs : CommandEventArgs
        {
            public BeforeCommandExecutedEventArgs(string command)
                : base(command)
            {
            }
        }

        public class AfterCommandExecutedEventArgs : CommandEventArgs
        {
            public AfterCommandExecutedEventArgs(string command)
                : base(command)
            {
            }
        }

        public class ActiveDocumentChangedEventArgs : DocumentEventArgs
        {
            public ActiveDocumentChangedEventArgs(Document document)
                : base(document)
            {
            }
        }

        public class ReportCreatedEventArgs : DocumentEventArgs
        {
            public ReportCreatedEventArgs(Document document, string reportFullName, Document.ReportFileType reportFileType)
                : base(document)
            {
                ReportType = reportFileType;
            }
            public Document.ReportFileType ReportType { get; private set; }
        }

        //public class MessageLoggedEventArgs : System.EventArgs
        //{
        //    public MessageLoggedEventArgs(Exception exception)
        //        : this((exception != null ? String.Concat("\r\n", exception.Message) : ""))
        //    {
        //        Exception = exception;
        //    }

        //    public MessageLoggedEventArgs(string message)
        //        : this(message, String.Empty, false)
        //    {
        //    }

        //    public MessageLoggedEventArgs(string message, string link)
        //        : this(message, link, false)
        //    {
        //    }

        //    public MessageLoggedEventArgs(string message, string link, bool doEvents)
        //        : base()
        //    {
        //        Exception = null;
        //        Message = message;
        //        DoEvents = doEvents;
        //        Hyperlink = link;
        //    }
        //    public string Message { get; private set; }
        //    public string Hyperlink { get; private set; }
        //    public Exception Exception { get; private set; }
        //    public bool DoEvents { get; private set; }
        //}

        public class MessageClearedEventArgs : System.EventArgs
        {
            public MessageClearedEventArgs()
                : base()
            {
            }
        }

        public Document ActiveDocument
        {
            get { return Documents.ActiveDocument; }
        }

        public Document OpenDocument(string fullName, Ifc4.Document.IfcFileType ifcFileType = Ifc4.Document.IfcFileType.Auto)
        {
            return Documents.Open(fullName, ifcFileType);
        }

        public Document CreateDocument(string fullName, bool overwriteIfExists)
        {
            return Documents.CreateDocument(fullName, overwriteIfExists);
        }

        public Ifc4.IfcXML CreateIfcXmlFromStream(Stream stream)
        {
            try
            {
                Ifc4.IfcXML externalIfcXML = JV.XmlProcessing<Ifc4.IfcXML>.Read(stream);
                return externalIfcXML;
            }
            catch (Exception exc)
            {
                return null;
            }
        }


        public bool CleanUpDocument()
        {
            if (Documents.ActiveDocument != null)
                return Documents.ActiveDocument.CleanUp();
            return false;
        }

        public class SaveResult
        {
            public SaveResultType Success { get; internal set; }
            public string Message { get; internal set; }
        }
        public class SaveOptions
        {
            public SaveOptions()
            {
                InternalOverwrite = false;
                InternalSaveAsNewFile = false;
            }

            internal bool InternalOverwrite { get; set; }
            internal bool InternalSaveAsNewFile { get; set; }
        }

        public enum SaveResultType
        {
            Error,
            Success,
            MissingFileName,
            NoActiveDocument,
            WrongFileExtension,
            ZipArchiveEntryAlreadyExists,
        }

        public bool SaveDocument()
        {
            SaveResult saveResult = SaveDocument(new SaveOptions());
            return saveResult.Success == SaveResultType.Success;

        }
        public SaveResult SaveDocument(SaveOptions saveOptions)
        {
            if (Documents.ActiveDocument != null)
                return Documents.ActiveDocument.Save(saveOptions);

            return new SaveResult()
            {
                Success = SaveResultType.NoActiveDocument,
                Message = "No active document found."
            };
        }

        public bool SaveDocument(string fullName)
        {
            SaveResult saveResult = SaveDocument(fullName, new SaveOptions());
            return saveResult.Success == SaveResultType.Success;
        }

        public SaveResult SaveDocument(string fullName, SaveOptions saveOptions)
        {
            SaveResult saveResult = new SaveResult();
            if (Documents.ActiveDocument == null)
            {
                saveResult.Success = SaveResultType.NoActiveDocument;
                saveResult.Message = "No active document found.";
            }
            else
            {
                //if (!String.IsNullOrWhiteSpace(Documents.ActiveDocument.FullName) && Documents.ActiveDocument.FullName != fullName)
                //{
                //    // Datei wird unter einem anderem Namen abgespeichert
                //    saveOptions.InternalSaveAsNewFile = true;
                //}
                if (String.IsNullOrWhiteSpace(Documents.ActiveDocument.FullName) || Documents.ActiveDocument.FullName != fullName)
                {
                    // Datei wird unter einem anderem Namen abgespeichert
                    saveOptions.InternalSaveAsNewFile = true;
                }
                saveOptions.InternalOverwrite = true;
                saveResult = Documents.ActiveDocument.SaveAs(fullName, saveOptions);
            }
            return saveResult;
        }


        public void CloseDocument()
        {
            Documents.CloseAll();
        }

        protected Documents _Documents;
        public Documents Documents
        {
            get
            {
                if (_Documents == null)
                    _Documents = new Documents(this);

                return _Documents;
            }
        }

        public static string AssemblyDirectoryFullName
        {
            get
            {
                FileInfo fileInfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return fileInfo.DirectoryName;
            }
        }

        public const string SCHEMA_EXP_IFC2X3_TC1 = "IFC2X3_TC1.exp";
        public const string SCHEMA_XSD_IFC2X3 = "IFC2X3.xsd";
        public const string SCHEMA_XSD_EX = "ex.xsd";

        private static string GetIfcSchemaFromResource(string resourceName, string schemaFileName)
        {
            //////// TODOJV: besser aus dll zusammenbauen!!!
            //////// string resourceName = "CafmConnect.Core.Resources.IFC2X3_TC1.exp";
            //////var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //////string[] schemaFullNames = new string[]
            //////{
            //////    System.IO.Path.Combine(AssemblyDirectoryFullName, schemaFileName), // // erster Versuch parallel zum Dll
            //////    System.IO.Path.Combine(Path.GetTempPath(), schemaFileName) // zweiter Versuch im Temp Folder
            //////};

            //////for (int i = 0; i < schemaFullNames.Length; i++)
            //////{
            //////    if (System.IO.File.Exists(schemaFullNames[i]))
            //////        return schemaFullNames[i];
            //////}

            //////var stream = assembly.GetManifestResourceStream(resourceName);
            //////if (stream != null)
            //////{
            //////    for (int i = 0; i < schemaFullNames.Length; i++)
            //////    {
            //////        try
            //////        {
            //////            using (FileStream fileStream = new FileStream(schemaFullNames[i], FileMode.Create))
            //////            {
            //////                Environment.CopyStream(stream, fileStream);
            //////            }
            //////            Workspace.CurrentWorkspace.RaiseMessageLogged("Workspace.GetIfcSchemaFromResource", String.Format("Schema '{0}' created from resource stream.", schemaFullNames[i]));
            //////            return schemaFullNames[i];
            //////        }
            //////        catch(Exception exc)
            //////        {
            //////            Workspace.CurrentWorkspace.RaiseMessageLogged("Workspace.GetIfcSchemaFromResource", exc);
            //////            continue;
            //////        }
            //////    }
            //////}

            return String.Empty;
        }

        public static string SchemaExpFullName
        {
            get
            {
                //// sollte hier sein
                //string fullName = Path.Combine(AssemblyDirectoryFullName, SCHEMA_EXP_IFC2X3_TC1);
                //if (File.Exists(fullName))
                //    return fullName;

                // Datei kann nicht gefunden werden oder fehlt
                // versuche die Datei aus den Resourcen zu erstellen
                string fullName = GetIfcSchemaFromResource("CafmConnect.Core.Resources.IFC2X3_TC1.exp", SCHEMA_EXP_IFC2X3_TC1);
                return fullName;
            }
        }

        public static string GetSchemaXsdFullName(string fileName)
        {
            // sollte hier sein
            string fullName = Path.Combine(AssemblyDirectoryFullName, fileName);
            if (File.Exists(fullName))
                return fullName;

            // Datei kann nicht gefunden werden oder fehlt
            // versuche die Datei aus den Resourcen zu erstellen
            fullName = GetIfcSchemaFromResource(String.Concat("CafmConnect.Core.Resources.", fileName), fileName);
            return fullName;
        }


        public bool SerializeTest(string ifcFullName)
        {
            if (String.IsNullOrEmpty(ifcFullName))
            {
                throw new ArgumentNullException("ifcFullName", "SerializeTest()");
                return false;
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(ifcFullName);
            if (!fileInfo.Exists)
            {
                RaiseMessageLogged(String.Format("{0} - File '{0}' not found!", "SerializeTest()", ifcFullName));
                return false;
            }

            try
            {
                string f0_Org = fileInfo.FullName;
                string f0 = System.IO.Path.ChangeExtension(f0_Org, ".tmp" + fileInfo.Extension);

                // bool validate = this.Validate(f0_Org);
                System.IO.File.Copy(f0_Org, f0, true);

                Ifc4.Document document = Ifc4.Workspace.CurrentWorkspace.OpenDocument(f0);
                Ifc4.IfcXML ifcXML = document.IfcXmlDocument;

                document.SaveAs(f0 + ".0", new SaveOptions());

                //Correction(f0 + ".0");
                //validate = this.Validate(f0 + ".0");

                RaiseMessageLogged(String.Format("Serialized file '{0}'", f0 + ".0"));
                RaiseMessageLogged("End");

                return true;

            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                Exception innerException = exc.InnerException;
                while (innerException != null)
                {
                    RaiseMessageLogged(innerException.Message);
                    innerException = innerException.InnerException;
                }
                return false;
            }
        }

        public bool CreateCafmConnectTestFile(string ifcFullName)
        {
            EventType eventType = BaseObject.LockEvents();
            bool result = InternalCreateCafmConnectTestFile(ifcFullName);
            BaseObject.UnlockEvents(eventType);
            return result;
        }

        private bool InternalCreateCafmConnectTestFile(string ifcFullName)
        {
            string schemaFullName;
            Ifc4.Document document;

            // create new ifc file
            document = Ifc4.Workspace.CurrentWorkspace.CreateDocument(ifcFullName, true);
            document.MessageLogged += document_MessageLogged;

            // add project info
            document.Project.GlobalId = document.GetNewGlobalId();
            document.Project.Name = "CEBP";
            document.Project.LongName = "Cologne, Ehrenfelder Business Park";
            document.Project.Description = "Ehrenfelder Business Park";

            Ifc4.IfcSite site;
            Ifc4.IfcBuilding building;
            Ifc4.IfcBuildingStorey buildingStorey;
            Ifc4.IfcSpace space;

            // --------------------------------------------------------------------------
            site = document.Project.Sites.AddNewSite();
            site.Name = "CEBP01";
            site.LongName = "Cologne, Ehrenfelder Tower";
            site.Description = "Cologne Ehrenfelder Tower";


            for (int nBuilding = 0; nBuilding < 10; nBuilding++) // buildings
            {
                building = site.Buildings.AddNewBuilding();
                building.Name = String.Format("Site:{0} Building:{1}", "1", nBuilding);
                building.LongName = "Tower 01";
                building.Description = "Tower 01";

                for (int nStorey = 0; nStorey < 10; nStorey++) // storeys
                {
                    buildingStorey = building.BuildingStoreys.AddNewBuildingStorey();
                    buildingStorey.Name = String.Format("Storey:{0}", nStorey);

                    for (int nSpace = 0; nSpace < 10; nSpace++) // rooms
                    {
                        space = buildingStorey.Spaces.AddNewSpace();
                        space.Name = String.Format("Storey:{0} Space:{1}", nStorey, nSpace);
                    }
                }
            }

            IfcPostalAddress postalAddress;
            postalAddress = new IfcPostalAddress();
            postalAddress.AddressLines = new IfcPostalAddressAddressLines();
            postalAddress.AddressLines.IfcLabelwrapper = new List<IfcLabelwrapper>();
            postalAddress.AddressLines.IfcLabelwrapper.Add(new IfcLabelwrapper()
            {
                Value = "eTASK Headquarter Wilhelm-Ruppert-Straße 38 Gebäude K15"
            });

            postalAddress.Id = document.GetNextSid();
            postalAddress.Region = "NRW";
            postalAddress.Town = "Cologne";
            postalAddress.Country = "Germany";
            postalAddress.PostalCode = "51147";

            document.IfcXmlDocument.Items.Add(postalAddress);

            postalAddress = new IfcPostalAddress();
            postalAddress.AddressLines = new IfcPostalAddressAddressLines();
            postalAddress.AddressLines.IfcLabelwrapper = new List<IfcLabelwrapper>();
            postalAddress.AddressLines.IfcLabelwrapper.Add(new IfcLabelwrapper()
            {
                Value = "Joachim Vollberg\r\nHadersleber Str. 28\r\n50825 Köln"
            });

            postalAddress.Id = document.GetNextSid();
            postalAddress.Region = "NRW";
            postalAddress.Town = "Cologne";
            postalAddress.Country = "Germany";
            postalAddress.PostalCode = "50825";

            document.IfcXmlDocument.Items.Add(postalAddress);

            try
            {
                document.Save(new SaveOptions());
                document.Close();
                return true;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                Exception innerException = exc.InnerException;
                while (innerException != null)
                {
                    RaiseMessageLogged(innerException.Message);
                    innerException = innerException.InnerException;
                }
                return false;
            }
            finally
            {
                document.MessageLogged -= document_MessageLogged;
            }
        }

        public bool CreateTestIfc(string ifcFullName)
        {

            Ifc4.Document document = Ifc4.Workspace.CurrentWorkspace.CreateDocument(ifcFullName, true);
            document.MessageLogged += document_MessageLogged;

            if (String.IsNullOrEmpty(ifcFullName))
            {
                throw new ArgumentNullException("ifcFullName", "CreateTestIfc()");
                return false;
            }

            try { System.IO.File.Delete(ifcFullName); }
            catch { }

            //ifcXml.express = new string[] { };
            //ifcXml.configuration = new string[] { };

            document.IfcXmlDocument.Id = "ifcXML4";
            document.IfcXmlDocument.Header = new UosHeader()
            {
                //<time_stamp>2013-04-17T14:34:08.5257209+02:00</time_stamp>

                Name = System.IO.Path.GetFileNameWithoutExtension(ifcFullName),
                // time_stamp = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                TimeStamp = DateTime.Now,
                Author = System.Environment.UserName,
                Organization = "eTASK Service-Management GmbH",
                PreprocessorVersion = ".NET API etask.ifc",
                OriginatingSystem = ".NET API etask.ifc",
                Authorization = "file created with .NET API etask.ifc",
                Documentation = "ViewDefinition [notYetAssigned]" // version 
            };


            document.Project.UnitsInContext = new IfcUnitAssignment()
            {
                Units = new IfcUnitAssignmentUnits()
                {
                    Items = new List<Entity>()
                    {
                        new IfcSIUnit()
                        {
                            Id = document.GetNextSid(),
                            Prefix = IfcSIPrefix.Milli,
                            Name = IfcSIUnitName.Metre,
                            NameSpecified = true,
                            UnitType = IfcUnitEnum.Lengthunit,
                            UnitTypeSpecified = true
                        },


                        new IfcDerivedUnit()
                        {
                                Elements = new IfcDerivedUnitElements()
                                {
                                    IfcDerivedUnitElement = new List<IfcDerivedUnitElement>()
                                    {
                                        new IfcDerivedUnitElement()
                                        {
                                            Id = document.GetNextSid(),
                                            Exponent = 1,
                                            Unit = new IfcSIUnit()
                                            {
                                                Name = IfcSIUnitName.Metre,
                                                NameSpecified = true,
                                                UnitType = IfcUnitEnum.Lengthunit,
                                                UnitTypeSpecified = true
                                            }
                                        },
                                        new IfcDerivedUnitElement()
                                        {
                                            Id = document.GetNextSid(),
                                            Exponent = -1,
                                            Unit = new IfcSIUnit()
                                            {
                                                Name = IfcSIUnitName.Second,
                                                NameSpecified = true,
                                                UnitType = IfcUnitEnum.Timeunit,
                                                UnitTypeSpecified = true
                                            }
                                        }
                                    }
                                }
                        },



                        new IfcDerivedUnit()
                        {
                            Id = document.GetNextSid(),
                            UnitType = IfcDerivedUnitEnum.Volumetricflowrateunit,
                            UnitTypeSpecified = true,
                                Elements = new IfcDerivedUnitElements()
                                {
                                    IfcDerivedUnitElement = new List<IfcDerivedUnitElement>()
                                    {
                                        new IfcDerivedUnitElement()
                                        {
                                            Id = document.GetNextSid(),
                                            Exponent = 3,
                                            ExponentSpecified = true,

                                            Unit = new IfcSIUnit()
                                            {
                                                Name = IfcSIUnitName.Metre,
                                                NameSpecified = true,
                                                UnitType = IfcUnitEnum.Lengthunit,
                                                UnitTypeSpecified = true
                                            },
                                        },
                                        new IfcDerivedUnitElement()
                                        {
                                            Id = document.GetNextSid(),
                                            Exponent = -1,
                                            ExponentSpecified = true,

                                        //Name = IfcSIUnitName.second,
                                        //NameSpecified = true,
                                        //UnitType = IfcUnitEnum.timeunit,
                                        //UnitTypeSpecified = true,

                                            Unit = new IfcConversionBasedUnit()
                                            {
                                                ConversionFactor = new IfcMeasureWithUnit()
                                            {
                                                ValueComponent = new IfcMeasureWithUnitValueComponent()
                                                {
                                                    Item = new IfcTimeMeasurewrapper()
                                                    {
                                                        Value = 3600.0
                                                    }
                                                },
                                                UnitComponent = new IfcMeasureWithUnitUnitComponent()
                                                {
                                                    Item = new IfcSIUnit()
                                                    {
                                                        Name = IfcSIUnitName.Second,
                                                        NameSpecified = true,
                                                        UnitType = IfcUnitEnum.Timeunit,
                                                        UnitTypeSpecified = true
                                                    }                                                        
                                                }
                                            }
                                            }
                                        }
                                    }
                                }
                        }
                    }

                }

            };

            Ifc4.IfcClassification ifcClassification = document.AddNew<IfcClassification>();
            //ifcClassification.id = document.GetNextSid();
            ifcClassification.Edition = "Version 1.06";
            ifcClassification.EditionDate = "2014-08-14T09:39:57";
            ifcClassification.Name = "CAFM-Connect Katalog 2014";

            Ifc4.IfcRelAssociatesClassification ifcRelAssociatesClassification = document.AddNew<IfcRelAssociatesClassification>();
            //ifcRelAssociatesClassification.id = document.GetNextSid();
            ifcRelAssociatesClassification.GlobalId = "1XiFKGEHP6PxQPxDU2E3PA";
            ifcRelAssociatesClassification.Name = "CAFM-Connect Katalog 2014 zu IfcClassification";
            ifcRelAssociatesClassification.RelatedObjects = new IfcRelAssociatesRelatedObjects()
            {
                Items = new List<IfcRoot>()
                {
                    new Ifc4.IfcProject(){@Ref = document.Project.Id}
                }
            };

            ifcRelAssociatesClassification.RelatingClassification = new IfcRelAssociatesClassificationRelatingClassification()
            {
                Item = new IfcClassification() { @Ref = ifcClassification.Id }
            };

            //<IfcClassification id="i120" Edition="Version 1.06" EditionDate="2014-08-14T09:39:57" Name="CAFM-Connect Katalog 2014" />
            //<IfcRelAssociatesClassification id="i121" GlobalId="1XiFKGEHP6PxQPxDU2E3PA" Name="CAFM-Connect Katalog 2014 zu IfcClassification">
            //  <RelatedObjects>
            //    <IfcProject ref="i100" xsi:nil="true" />
            //  </RelatedObjects>
            //  <RelatingClassification>
            //    <IfcClassification ref="i120" xsi:nil="true" />
            //  </RelatingClassification>
            //</IfcRelAssociatesClassification>

            try
            {

                document.Save(new SaveOptions());
                document.Close();

                string message;
                document.Open(ifcFullName, out message);
                document.SaveAs(ifcFullName + ".0", new SaveOptions());
                RaiseMessageLogged(this, System.IO.File.ReadAllText(document.FullName));

                return true;

            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                Exception innerException = exc.InnerException;
                while (innerException != null)
                {
                    RaiseMessageLogged(innerException.Message);
                    innerException = innerException.InnerException;
                }
                return false;
            }

        }

        void document_MessageLogged(object sender, Ifc4.EventArgs.MessageLoggedEventArgs e)
        {
            RaiseMessageLogged(sender, e.Message);
        }

    }

}



