using System;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using Ifc4.Interfaces;
using System.Runtime.InteropServices;

namespace Ifc4
{
    public partial class Document : BaseObject
    {
        public delegate void MessageLoggedEventHandler(object sender, Ifc4.EventArgs.MessageLoggedEventArgs e);
        public event MessageLoggedEventHandler MessageLogged;
        private XNamespace IfcNs { get { return XNamespace.Get("http://www.buildingsmart-tech.org/ifcXML/IFC4/final"); } }

        internal Document(IBaseObject parent)
            : base(parent)
        {
            ResetId();

            IfcXmlDocument = new IfcXML();

            this.IfcXmlDocument.Id = "ifcXML4";

            PopulateDefaultUosHeader();

        }

        internal Dictionary<string, Dictionary<string, Entity>> Items { get; set; }

        private System.Diagnostics.Stopwatch m_Sw = new System.Diagnostics.Stopwatch();

        private Dictionary<string, List<IfcObjectDefinition>> m_IfcRelAggregatesCollection;
        internal IEnumerable<T> GetSpatialStructureChilds<T>(string id) where T : Entity
        {

            m_Sw.Start();

            if (m_IfcRelAggregatesCollection == null)
            {
                m_IfcRelAggregatesCollection = new Dictionary<string, List<IfcObjectDefinition>>();
                foreach (var item in this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null))
                {
                    if(!m_IfcRelAggregatesCollection.ContainsKey(item.RelatingObject.Ref))
                    {
                        m_IfcRelAggregatesCollection.Add(item.RelatingObject.Ref, item.RelatedObjects.Items);
                    }
                }
            }

            Dictionary<string, Entity> entities = null;
            if (!Document.Items.TryGetValue(typeof(T).Name, out entities))
                yield return default(T);

            List<IfcObjectDefinition> ifcObjectDefinitions;
            if (m_IfcRelAggregatesCollection.TryGetValue(id, out ifcObjectDefinitions))
            {
                foreach (var ifcObjectDefinition in ifcObjectDefinitions)
                {
                    Entity entity;
                    if (entities != null && entities.TryGetValue(ifcObjectDefinition.Ref, out entity))
                        yield return (T)entity;
                }

            }

            m_Sw.Stop();
            System.Diagnostics.Debug.WriteLine("GetSpatialStructureChilds:" + typeof(T).Name + " " + m_Sw.ElapsedMilliseconds + "ms");
        }

        public override bool Read(BaseObject parent)
        {
            EventType eventType = BaseObject.LockEvents();
            try
            {
                this.Parent = parent;

                Items = this.IfcXmlDocument.Items
                    .Where(item => item.Id != null)
                    .GroupBy(item => item.GetType().Name)
                    .ToDictionary(group => group.Key, group => group.ToDictionary(pair => pair.Id, pair => pair));

                // Reads all objects that can not be changed during editing
                bool result = this.Project.Read(this);
                if (result)
                    ReadChecksum(FullName);

                return result;
            }
            finally
            {
                BaseObject.UnlockEvents(eventType);
            }
        }

        private Dictionary<string, Ifc4.IfcPropertySetTemplate> m_IfcPropertySetTemplateCollection;
        internal Dictionary<string, Ifc4.IfcPropertySetTemplate> IfcPropertySetTemplateCollection
        {
            get
            {
                if (m_IfcPropertySetTemplateCollection == null)
                {
                    m_IfcPropertySetTemplateCollection = new Dictionary<string, IfcPropertySetTemplate>();
                    foreach (var item in this.IfcXmlDocument.Items.OfType<Ifc4.IfcPropertySetTemplate>())
                        m_IfcPropertySetTemplateCollection.Add(item.Id, item);
                }
                return m_IfcPropertySetTemplateCollection;
            }
        }

        internal Ifc4.IfcPropertySetTemplate GetIfcPropertySetTemplate(string id)
        {
            if (String.IsNullOrEmpty(id))
                return null;

            Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate;
            if (IfcPropertySetTemplateCollection.TryGetValue(id, out ifcPropertySetTemplate))
                return ifcPropertySetTemplate;

            return null;
        }

        public class ABC
        {
            public ABC(Entity a, Entity b)
            {
                A = a;
                B = b;
            }
            public Entity A { get; set; }
            public Entity B { get; set; }
        }

        public IEnumerable<Ifc4.IfcRelAssociatesDocument> GetIfcRelAssociatesDocumentCollection()
        {
            return this.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesDocument>();
        }

        public IEnumerable<Ifc4.IfcRelAssociatesDocument> GetIfcRelAssociatesDocumentCollection(Ifc4.Entity entity)
        {
            if (entity == null)
                return Enumerable.Empty<Ifc4.IfcRelAssociatesDocument>();

            return from ifcRelAssociatesDocument in GetIfcRelAssociatesDocumentCollection()
                   where ifcRelAssociatesDocument.RelatedObjects != null &&
                   ifcRelAssociatesDocument.RelatedObjects.Items.Any(item => item.Ref == entity.Id)
                   select ifcRelAssociatesDocument;
        }

        private IEnumerable<ABC> m_RelatedObjectsRefs = null;
        public IEnumerable<Ifc4.IfcPropertySetTemplate> GetIfcPropertySetTemplateCollection(CcFacility facility)
        {
            IEnumerable<Ifc4.IfcPropertySetTemplate> ifcPropertySetTemplateCollection = null;
            if (facility != null && !String.IsNullOrEmpty(facility.ObjectTypeId))
            {
                string objectTypeId = facility.ObjectTypeId;
                if (m_RelatedObjectsRefs == null)
                {
                    Ifc4.Document document = facility.GetParent<Ifc4.Document>();
                    IEnumerable<Ifc4.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesClassification>().ToList();
                    m_RelatedObjectsRefs = (from ifcRelAssociatesClassification in ifcRelAssociatesClassificationCollection
                                            from relatedObject in ifcRelAssociatesClassification.RelatedObjects.Items
                                            where ifcRelAssociatesClassification.RelatingClassification.Item != null // && ifcRelAssociatesClassification.RelatingClassification.Item.Ref == objectTypeId
                                            select new ABC(ifcRelAssociatesClassification.RelatingClassification.Item, relatedObject)).ToList();

                    //m_IfcPropertySetTemplateCollection = document.IfcXmlDocument.Items.OfType<Ifc4.IfcPropertySetTemplate>().ToList();
                }

                var relatedObjectsRefs = m_RelatedObjectsRefs.Where(item => item.A.Ref == objectTypeId).Select(item => item.B.Ref);
                ifcPropertySetTemplateCollection = IfcPropertySetTemplateCollection.Values.Where(item => relatedObjectsRefs.Contains(item.Id));
            }
            return ifcPropertySetTemplateCollection;
        }

        private Dictionary<string, Ifc4.IfcPropertySetTemplate> m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId;
        internal Ifc4.IfcPropertySetTemplate GetIfcPropertySetTemplateFromIfcClassificationReferenceId(string id)
        {
            if (String.IsNullOrEmpty(id))
                return null;

            if (m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId == null)
            {
                m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId = new Dictionary<string, IfcPropertySetTemplate>();

                IEnumerable<Ifc4.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = from item in this.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesClassification>()
                                                                                                                  where item.RelatedObjects != null &&
                                                                                                                  item.RelatedObjects.Items.OfType<Ifc4.IfcPropertySetTemplate>().Any()
                                                                                                                  select item;

                string ifcPropertySetTemplateId;
                foreach (var ifcRelAssociatesClassification in ifcRelAssociatesClassificationCollection)
                {
                    Entity relatingClassification = ifcRelAssociatesClassification.RelatingClassification.Item;
                    string relatingClassificationId = relatingClassification.IsRef ? relatingClassification.Ref : relatingClassification.Id;

                    foreach (var item in ifcRelAssociatesClassification.RelatedObjects.Items)
                    {
                        ifcPropertySetTemplateId = item.IsRef ? item.Ref : item.Id;

                        if (!m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId.ContainsKey(relatingClassificationId))
                        {
                            var tmp = this.GetIfcPropertySetTemplate(ifcPropertySetTemplateId);
                            if (tmp != null)
                                m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId.Add(relatingClassificationId, tmp);
                        }
                    }
                }
            }

            Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate;
            if (m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId.TryGetValue(id, out ifcPropertySetTemplate))
                return ifcPropertySetTemplate;

            return null;
        }

        public string GetNewGlobalId()
        {
            return GlobalId.ConvertToIfcGuid(Guid.NewGuid());
        }

        public T AddNew<T>() where T : Entity
        {
            T instance = Activator.CreateInstance<T>() as T;
            instance.Parent = this;
            instance.Id = this.GetNextSid();
            IfcXmlDocument.Items.Add(instance);
            return instance;
        }

        private static int m_StartId = 100;
        private static Queue<int> m_NextFreeRange = new Queue<int>();

        internal static void ResetId()
        {
            Ifc4.Entity.AllIds = new List<string>();
            m_NextFreeRange = new Queue<int>();
        }

        public string GetNextSid()
        {
            int nextId;

            if (IfcXmlDocument == null)
            {
                nextId = m_StartId;
            }
            else
            {
                if (!m_NextFreeRange.Any())
                {
                    var ids = Ifc4.Entity.AllIds.Where(item => item != null && item.StartsWith("i")).Select(item => Convert.ToInt32(item.Substring(1))).ToList();
                    ids.Sort();
                    Enumerable.Range(m_StartId, Int32.MaxValue / 2).Except(ids).Take(10000).ToList().ForEach(id => m_NextFreeRange.Enqueue(id));
                }
                nextId = m_NextFreeRange.Dequeue();
            }
            return "i" + nextId.ToString();
        }

        [System.ComponentModel.Browsable(false)]
        public IfcProject Project
        {
            get
            {
                var instance = IfcXmlDocument.Items.OfType<IfcProject>().FirstOrDefault();
                if (instance == null)
                {
                    instance = AddNew<IfcProject>();
                }
                return instance;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public IEnumerable<Ifc4.IfcClassification> Classifications
        {
            get { return IfcXmlDocument.Items.OfType<Ifc4.IfcClassification>(); }
        }

        [System.ComponentModel.Browsable(false)]
        public IEnumerable<Ifc4.IfcClassificationReference> ClassificationReferences
        {
            get { return IfcXmlDocument.Items.OfType<Ifc4.IfcClassificationReference>(); }
        }

        public void PopulateDefaultUosHeader()
        {
            EventType eventType = BaseObject.LockEvents();

            this.IfcXmlDocument.Header = new UosHeader()
            {
                //<time_stamp>2013-04-17T14:34:08.5257209+02:00</time_stamp>
                Name = String.Empty, // System.IO.Path.GetFileNameWithoutExtension(fullName),
                // time_stamp = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                TimeStamp = DateTime.Now,
                Author = System.Environment.UserName,
                Organization = "eTASK Service-Management GmbH",
                PreprocessorVersion = ".NET API etask.ifc",
                OriginatingSystem = ".NET API etask.ifc",
                Authorization = "file created with .NET API etask.ifc",
                Documentation = "ViewDefinition [notYetAssigned]" // version 
            };
            BaseObject.UnlockEvents(eventType);
        }

        [System.ComponentModel.Browsable(false)]
        public IEnumerable<IfcOrganization> Organizations
        {
            get { return IfcXmlDocument.Items.OfType<IfcOrganization>(); }
        }

        [System.ComponentModel.Browsable(false)]
        public IEnumerable<IfcRelAssociatesClassification> RelAssociatesClassifications
        {
            get { return IfcXmlDocument.Items.OfType<IfcRelAssociatesClassification>(); }
        }

        private static Dictionary<string, IfcPropertySetTemplate> m_IfcPropertySetTemplateFromIfcClassificationReference = new Dictionary<string, IfcPropertySetTemplate>();
        private static Dictionary<string, IEnumerable<System.ComponentModel.PropertyDescriptor>> m_CustomPopertyDescriptorsFromIfcClassificationReferenceId = new Dictionary<string, IEnumerable<System.ComponentModel.PropertyDescriptor>>();

        private bool m_UseFacilityCache;
        public bool UseFacilityCache
        {
            get
            {
                return m_UseFacilityCache;
            }
            set
            {
                m_UseFacilityCache = value;
                if (!m_UseFacilityCache)
                {
                    m_IfcPropertySetTemplateFromIfcClassificationReference = new Dictionary<string, IfcPropertySetTemplate>();
                    m_CustomPopertyDescriptorsFromIfcClassificationReferenceId = new Dictionary<string, IEnumerable<System.ComponentModel.PropertyDescriptor>>();
                }
            }
        }

        internal IEnumerable<System.ComponentModel.PropertyDescriptor> GetCustomPropertyDescriptorsFromIfcClassificationReferenceId(Ifc4.CcFacility facility, string id)
        {
            IEnumerable<System.ComponentModel.PropertyDescriptor> propertyDescriptors = null;

            bool found = false;
            if (UseFacilityCache)
            {
                found = m_CustomPopertyDescriptorsFromIfcClassificationReferenceId.TryGetValue(id, out propertyDescriptors);
            }

            if (!found)
            {
                int position = 0;
                int end = 1000;
                propertyDescriptors = from pd in System.ComponentModel.TypeDescriptor.GetProperties(facility).Cast<System.ComponentModel.PropertyDescriptor>()
                                          //where Ifc4.CustomModel.CustomPropertyDescriptor.TryGetFormVisiblePositionFromObject(null, pd, out position) && !Ifc4.CustomModel.CustomPropertyDescriptor.HasXmlIgnoreAttribute(pd)
                                      let visiblePosition = position == 0 ? end++ : position
                                      orderby visiblePosition
                                      select pd;

                if (UseFacilityCache)
                    m_CustomPopertyDescriptorsFromIfcClassificationReferenceId.Add(id, propertyDescriptors);
            }
            return propertyDescriptors;
        }

        public Ifc4.Units Units
        {
            get
            {
                return Ifc4.Units.Current;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public Workspace Workspace
        {
            get { return this.GetParent<Workspace>(); }
        }

        [System.ComponentModel.Browsable(false)]
        internal Documents Documents
        {
            get { return Parent as Documents; }
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsActive
        {
            get { return System.Object.Equals(Documents.ActiveDocument, this); }
        }

        internal void Activate()
        {
            Documents.ActiveDocument = this;
        }

        public bool IsDirty { get; internal set; }
        internal void SetDirty()
        {
            IsDirty = true;
            if (BaseObject.IsEventEnabled(EventType.All))
                Workspace.CurrentWorkspace.RaiseDocumentModified(this, new Workspace.DocumentModifiedEventArgs(this));
        }

        internal void ResetDirty()
        {
            IsDirty = false;
        }

        public bool CleanUp()
        {
            return true;
        }

        public bool HasFileName
        {
            get { return !String.IsNullOrEmpty(FullName); }
        }

        internal void Close()
        {
            ((IBaseObjects<Document>)Documents).Remove(this);
            Documents.ActiveDocument = null;
            CleanUpTempDocumentPath();
            Workspace.CurrentWorkspace.RaiseDocumentClosed(this, new Workspace.DocumentClosedEventArgs(this));
        }

        private void CleanUpTempDocumentPath()
        {
            try
            {
                if (m_TempDocumentPath == null)
                    return;

                foreach(var fullName in System.IO.Directory.GetFiles(m_TempDocumentPath))
                {
                    try { System.IO.File.Delete(fullName); }
                    catch { }
                }
                System.IO.Directory.Delete(m_TempDocumentPath, true);
            }
            catch { }
        }

        public string FullName { get; set; }

        public Ifc4.IfcXML IfcXmlDocument { get; internal set; }

        private void RemoveXsi(string fullName)
        {
            List<string> lines = new List<string>();
            foreach (var line in System.IO.File.ReadAllLines(fullName))
            {
                if (line.IndexOf(" ref=\"") != -1)
                    lines.Add(line.Replace(" xsi:nil=\"true\"", ""));
                else
                    lines.Add(line);
            }
            System.IO.File.WriteAllLines(fullName, lines.ToArray());
        }

        private void RemoveComment(string sourceFullName, string targetFullName, int removetype = 0)
        {
            List<string> lines = new List<string>();
            foreach (var line in System.IO.File.ReadAllLines(sourceFullName))
            {
                if (line.Trim().StartsWith("<!--") && line.Trim().EndsWith("-->"))
                    continue;

                lines.Add(line);
            }
            System.IO.File.WriteAllLines(targetFullName, lines.ToArray());
        }

        public bool IsInOpenProcess { get; internal set; }


        private string m_TempDocumentPath;
        private string GetTempDocumentPath()
        {
            if (m_TempDocumentPath == null)
                m_TempDocumentPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N").ToString());

            if (!System.IO.Directory.Exists(m_TempDocumentPath))
                System.IO.Directory.CreateDirectory(m_TempDocumentPath);

            return m_TempDocumentPath;
        }
        public bool ExtractToTemp(string entryName, out string tempFullName)
        {
            if (String.IsNullOrWhiteSpace(entryName))
                throw new ArgumentNullException(nameof(entryName));

            if (!System.IO.File.Exists(FullName))
                throw new System.IO.FileNotFoundException();

            tempFullName = String.Empty;

            try
            {
                using (ZipArchive zipArchive = ZipFile.Open(FullName, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault(item => item.FullName.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                    if (zipArchiveEntry != null)
                    {
                        tempFullName = System.IO.Path.Combine(GetTempDocumentPath(), System.IO.Path.GetFileName(entryName));
                        zipArchiveEntry.ExtractToFile(tempFullName, true);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public Ifc4.IfcXML Open(string fullName, out string message, IfcFileType ifcFileType = IfcFileType.Undefined)
        {
            message = "";

            if (ifcFileType == IfcFileType.Auto)
            {
                string extension = System.IO.Path.GetExtension(fullName);
                if (String.IsNullOrWhiteSpace(extension))
                    ifcFileType = IfcFileType.Undefined;

                else if (extension.Equals(".ifcxml", StringComparison.OrdinalIgnoreCase))
                    ifcFileType = IfcFileType.IfcXml;

                else if (extension.Equals(".ifczip", StringComparison.OrdinalIgnoreCase))
                    ifcFileType = IfcFileType.IfcZip;
            }

            EventType eventType = BaseObject.LockEvents();
            try
            {
                Ifc4.IfcXML ifcXML = null;
                string tempFullName = null;

                // -------------------------------------------------------------
                switch (ifcFileType)
                {
                    case IfcFileType.Undefined:
                        break;

                    case IfcFileType.IfcXml:
                        tempFullName = System.IO.Path.GetTempFileName() + ".ifcxml";
                        System.IO.File.Copy(fullName, tempFullName, true);
                        RemoveXsi(tempFullName);
                        ifcXML = JV.XmlProcessing<Ifc4.IfcXML>.Read(tempFullName);
                        try { System.IO.File.Delete(tempFullName); }
                        catch { }

                        break;

                    case IfcFileType.IfcZip:

                        // open file from zip

                        // <project>.ifczip
                        // <project>.zip
                        //      <project>.ifcxml
                        //      <Document>.pdf
                        //      <Document>.docx
                        //      <Document>.xlsx
                        //      <Document>.txt
                        //      <Document>.*

                        tempFullName = System.IO.Path.GetTempFileName() + ".ifcxml";

                        using (ZipArchive zipArchive = ZipFile.Open(fullName, ZipArchiveMode.Read))
                        {
                            string entryName = System.IO.Path.ChangeExtension(System.IO.Path.GetFileNameWithoutExtension(fullName), ".ifcxml");
                            ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault(item => item.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                            if (zipArchiveEntry != null)
                            {
                                zipArchiveEntry.ExtractToFile(tempFullName, true);
                            }
                        }

                        RemoveXsi(tempFullName);
                        ifcXML = JV.XmlProcessing<Ifc4.IfcXML>.Read(tempFullName);
                        try { System.IO.File.Delete(tempFullName); }
                        catch { }

                        break;

                    default:
                        break;

                }

                IfcXmlDocument = ifcXML;
                this.FullName = fullName;
                this.Activate();
                return ifcXML;

                //// -------------------------------------------------------------
                //// 3. Möglichkeit
                //Ifc4.Serialization.IfcXmlSerializer ifcXmlSerializer = new Ifc4.Serialization.IfcXmlSerializer(typeof(Ifc4.IfcXML));
                //ifcXmlSerializer.MessageLogged += ifcXmlSerializer_MessageLogged;


                //Ifc4.IfcXML ifcXML = null;
                //using (System.IO.FileStream fileStream = new System.IO.FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                //{
                //    ifcXML = ifcXmlSerializer.Deserialize(fileStream) as Ifc4.IfcXML;
                //}
                //IfcXmlDocument = ifcXML;
                //this.FullName = fullName;
                //this.Activate();
                //return ifcXML;
                // -------------------------------------------------------------
                //// 4. Möglichkeit
                //Ifc4.IfcXML ifcXML = JV.XmlProcessing<Ifc4.IfcXML>.Read(fullName);
                //IfcXmlDocument = ifcXML;

                //this.FullName = fullName;
                //this.Activate();
                //return ifcXML;


            }
            catch (Exception exc)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(exc.Message);
                Exception innerException = exc.InnerException;
                while (innerException != null)
                {
                    sb.AppendLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                message = sb.ToString();

                Workspace.CurrentWorkspace.RaiseMessageLogged(exc);
                return null;
            }
            finally
            {
                BaseObject.UnlockEvents(eventType);
            }
        }

        public delegate void LineCallback(string line);

        public void ReadAllLines(LineCallback lineCallback)
        {
            if (!this.HasFileName)
                return;

            if (FileType == IfcFileType.IfcXml)
            {
                using (StreamReader streamReader = new StreamReader(FullName))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (lineCallback != null)
                            lineCallback(line);
                    }
                }
            }
            else if (FileType == IfcFileType.IfcZip)
            {
                using (ZipArchive zipArchive = ZipFile.Open(FullName, ZipArchiveMode.Read))
                {
                    string entryName = System.IO.Path.ChangeExtension(System.IO.Path.GetFileNameWithoutExtension(FullName), ".ifcxml");
                    ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault(item => item.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                    if (zipArchiveEntry != null)
                    {

                        //var s = "";
                        //using (StreamReader streamReader = new StreamReader(zipArchiveEntry.Open()))
                        //{
                        //    XDocument xDocument = XDocument.Load(streamReader);
                        //    s = xDocument.ToString();
                        //}

                        //if (lineCallback != null)
                        //    lineCallback(s);

                        //var x = XDocument.Load(zipArchiveEntry.Open());
                        //if (lineCallback != null)
                        //    lineCallback(x.ToString());

                        //XmlReader xr = XmlReader.Create(zipArchiveEntry.Open());
                        //System.Xml.Xsl.XslCompiledTransform xslCompiledTransform = new System.Xml.Xsl.XslCompiledTransform();
                        //xslCompiledTransform.Load(xr);

                        //StringBuilder sb = new StringBuilder();
                        //XmlWriter xmlWriter = XmlWriter.Create(sb);
                        //xslCompiledTransform.Transform(value, xmlWriter);


                        //XmlReader xr = XmlReader.Create(zipArchiveEntry.Open());
                        //xr.Read();
                        //xr.ReadOuterXml();
                        //lineCallback(xr.ToString());

                        //using (var xmlReader = XmlReader.Create(zipArchiveEntry.Open()))
                        //{
                        //    while (xmlReader.Read())
                        //    {
                        //        if (lineCallback != null)
                        //            lineCallback(xmlReader.ToString());
                        //    }
                        //}

                        //using (StreamReader streamReader = new StreamReader(zipArchiveEntry.Open()))
                        //{
                        //    string line;
                        //    while ((line = streamReader.ReadLine()) != null)
                        //    {
                        //        if (lineCallback != null)
                        //            lineCallback(line);
                        //    }
                        //}
                    }
                }
            }

            
        }

        public enum IfcFileType
        {
            Undefined,
            Auto,
            IfcXml,
            IfcZip
        }

        public enum IfcVersion
        {
            Unknown,
            IFC4
        }

        public enum ReportFileType
        {
            Pdf,
            Xps
        }

        internal IfcFileType GetFileType(string fullName)
        {
            if (String.IsNullOrWhiteSpace(fullName))
                return IfcFileType.Undefined;

            string extension = System.IO.Path.GetExtension(fullName);
            if (String.IsNullOrWhiteSpace(extension))
                return IfcFileType.Undefined;

            if (extension.Equals(".ifcxml", StringComparison.OrdinalIgnoreCase))
                return IfcFileType.IfcXml;

            //if (extension.Equals(IFCXMLTEMPEXTENSION, StringComparison.InvariantCultureIgnoreCase))
            //    return IfcFileType.IfcXmlTemp;

            if (extension.Equals(".ifczip", StringComparison.OrdinalIgnoreCase))
                return IfcFileType.IfcZip;

            return IfcFileType.Auto;
        }

        public IfcFileType FileType => GetFileType(FullName);

        internal Ifc4.Workspace.SaveResult Save(Workspace.SaveOptions saveOptions)
        {
            Ifc4.Workspace.SaveResult result;
            switch (FileType)
            {
                case IfcFileType.Undefined:
                    result = new Workspace.SaveResult()
                    {
                        Success = Workspace.SaveResultType.WrongFileExtension,
                        Message = "Missing file extension."
                    };
                    break;
                case IfcFileType.IfcXml:
                    result = SaveAs(FullName, saveOptions);
                    break;
                case IfcFileType.IfcZip:
                    result = SaveAs(FullName, saveOptions);
                    break;
                default:
                    result = new Workspace.SaveResult()
                    {
                        Success = Workspace.SaveResultType.WrongFileExtension,
                        Message = "File extension not supported."
                    };
                    break;
            }
            return result;
        }

        private void TransferZipArchiveEntriesToNewFile(string oldFullName, string newFullName)
        {
            if (oldFullName == newFullName)
                return; // nothing to do

            if (String.IsNullOrWhiteSpace(oldFullName) || !System.IO.File.Exists(oldFullName))
            {
                System.IO.File.Delete(newFullName);
                return;
            }

            if (System.IO.Path.GetExtension(oldFullName).Equals(System.IO.Path.GetExtension(newFullName), StringComparison.OrdinalIgnoreCase))
            {
                System.IO.File.Copy(oldFullName, newFullName, true);
            }
            else
            {
                System.IO.File.Delete(newFullName);
            }
        }

        private void BeforeSaveZipArchive(ZipArchive zipArchive)
        {
            if (zipArchive == null)
                return;

            // ---------------------------------------------
            // delete ifcxml files
            List<ZipArchiveEntry> zipArchiveEntries;
            zipArchiveEntries =
                zipArchive.Entries.Where(
                    item =>
                        System.IO.Path.GetExtension(item.FullName).Equals(".ifcxml", StringComparison.OrdinalIgnoreCase))
                    .ToList();

            foreach (var zipArchiveEntry in zipArchiveEntries)
            {
                zipArchiveEntry.Delete();
            }
            // ---------------------------------------------
            // delete unused document information files
            var allDocumentContainerLocations = this.Document.Project.DocumentContainer.Select(item => item.Location.ToLowerInvariant()).ToArray();
            var allZipArchiveEntryFullNames = zipArchive.Entries.Select(item => item.FullName.ToLowerInvariant()).ToArray();

            var removeZipArchiveEntryNames = allZipArchiveEntryFullNames.Except(allDocumentContainerLocations);
            var removeZipArchiveEntries = zipArchive.Entries.Where(item => removeZipArchiveEntryNames.Contains(item.FullName.ToLowerInvariant()));

            foreach (var removeZipArchiveEntry in removeZipArchiveEntries.ToList())
            {
                removeZipArchiveEntry.Delete();
            }
        }

        internal const string RelativeDocumentsFolderName = "Documents";

        internal string GetZipFileLocation(string fullName)
        {
            return System.IO.Path.Combine(RelativeDocumentsFolderName, System.IO.Path.GetFileName(fullName));
        } 

        internal Ifc4.Workspace.SaveResult SaveAs(string fullName, Ifc4.Workspace.SaveOptions saveOptions)
        {
            if (String.IsNullOrWhiteSpace(fullName))
            {
                Workspace.CurrentWorkspace.RaiseMessageLogged("Missing file name.");
                return new Workspace.SaveResult()
                {
                    Success = Workspace.SaveResultType.MissingFileName,
                    Message = "Missing file name."
                };
            }

            if (saveOptions.InternalSaveAsNewFile)
            {
                // -------------------------------------------------
                // transfer all ZipArchiveEntries to the new file
                string oldFullName = this.FullName;
                string newFullName = fullName;
                TransferZipArchiveEntriesToNewFile(oldFullName, newFullName);
            }

            FullName = fullName;
            
            if (FileType == IfcFileType.IfcXml)
            {
                if (this.IfcXmlDocument.Items.OfType<IfcDocumentInformation>().Any())
                {
                    return new Workspace.SaveResult()
                    {
                        Success = Workspace.SaveResultType.WrongFileExtension,
                        Message = "The document has 'IfcDocumentInformation' entities and can't be saved with the file extension '.ifcxml'. Please save the file as '.ifczip'."
                    };
                }
            }

            this.ResetDirty();

            BaseObject.EnableEvent(EventType.None, false);

            try
            {
                // Header Name wird immer überschrieben
                this.IfcXmlDocument.Header.Name = System.IO.Path.GetFileNameWithoutExtension(fullName);

                //System.Runtime.Serialization.DataContractSerializer sr1 = new System.Runtime.Serialization.DataContractSerializer(typeof(Ifc4.ifcXML));
                //using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(FullName))
                //{
                //    sr1.WriteObject(writer, this.IfcXmlDocument);
                //    writer.Close();
                //}

                Ifc4.Serialization.IfcXmlSerializer ifcXmlSerializer = new Ifc4.Serialization.IfcXmlSerializer(typeof(Ifc4.IfcXML));
                ifcXmlSerializer.MessageLogged += ifcXmlSerializer_MessageLogged;

                Workspace.CurrentWorkspace.RaiseMessageLogged($"Save document '{FullName}'...");

                string xmlSerializedString = ifcXmlSerializer.Serialize(this.IfcXmlDocument);
                // TODOJV
                // Schrott Lösung
                xmlSerializedString = xmlSerializedString.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", " xmlns:ifc=\"" + IfcNs.NamespaceName + "\"");


                if (FileType == IfcFileType.IfcZip)
                {
                    try
                    {

                        // save as unter einem neuem namen
                        // vorher kopieren


                        FileMode fileMode = System.IO.File.Exists(fullName) ? FileMode.Open : FileMode.CreateNew;
                        using (var fileStream = new FileStream(fullName, fileMode))
                        {
                            using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Update))
                            {

                                string entryName;
                                ZipArchiveEntry zipArchiveEntry;

                                BeforeSaveZipArchive(zipArchive);

                                // save document information files
                                foreach (var ifcDocumentInformation in this.IfcXmlDocument.Items.OfType<Ifc4.IfcDocumentInformation>()
                                                                        .Where(item => !String.IsNullOrWhiteSpace(item.InternalFullName)))
                                {
                                    entryName = this.Document.GetZipFileLocation(ifcDocumentInformation.InternalFullName);
                                    zipArchiveEntry = zipArchive.Entries.FirstOrDefault(item => item.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                                    if (zipArchiveEntry == null)
                                    {
                                        string sourceFullName = ifcDocumentInformation.InternalFullName;
                                        CompressionLevel compressionLevel = GetCompressionLevel(sourceFullName);
                                        zipArchiveEntry = zipArchive.CreateEntryFromFile(sourceFullName, entryName, compressionLevel);
                                    }
                                    else
                                    {
                                        // file already exists
                                        return new Workspace.SaveResult()
                                        {
                                            Success = Workspace.SaveResultType.Error,
                                            Message = $"Zip archive entry '{entryName}' already exists."
                                        };
                                    }
                                }
                                // -----------------------------------------


                                entryName = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(fullName), ".ifcxml");
                                zipArchiveEntry = zipArchive.Entries.FirstOrDefault(item => item.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                                if (zipArchiveEntry == null)
                                    zipArchiveEntry = zipArchive.CreateEntry(entryName);

                                using (StreamWriter writer = new StreamWriter(zipArchiveEntry.Open()))
                                {
                                    writer.Write(xmlSerializedString);

#if DEBUG
                                    var abc = "debug.ifcxml";
                                    System.IO.File.WriteAllText(abc, xmlSerializedString);
#endif
                                }

                            }
                        }


                    }
                    catch (Exception exc)
                    {
                        return new Workspace.SaveResult()
                        {
                            Success = Workspace.SaveResultType.Error,
                            Message = exc.Message
                        };
                    }

                }
                else
                {
                    System.IO.File.WriteAllText(FullName, xmlSerializedString);
                }


                foreach (
                    var ifcDocumentInformation in this.IfcXmlDocument.Items.OfType<Ifc4.IfcDocumentInformation>()
                        .Where(item => !String.IsNullOrWhiteSpace(item.InternalFullName)))
                {
                    ifcDocumentInformation.InternalFullName = null;
                }


                Workspace.CurrentWorkspace.RaiseMessageLogged($"Document '{FullName}' saved.");
                this.ResetDirty();
                Ifc4.Workspace.CurrentWorkspace.RaiseDocumentSaved(this, new Ifc4.Workspace.DocumentSavedEventArgs(this));

                ReadChecksum(FullName);

                return new Workspace.SaveResult()
                {
                    Success = Workspace.SaveResultType.Success,
                    Message = String.Empty
                };

            }
            catch (Exception exc)
            {
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
                return new Workspace.SaveResult()
                {
                    Success = Workspace.SaveResultType.Error,
                    Message = exc.Message
                };
            }
            finally
            {
                BaseObject.EnableEvent(EventType.All, true);
            }
        }

        private static string[] NoCompressionFileExtensions = new string[]
        {
            ".zip",
            ".z",
            ".ifczip",
            ".pdf",
            ".jpeg",
            ".jpg",
            ".exe"
        };

        internal CompressionLevel GetCompressionLevel(string sourceFullName)
        {
            return NoCompressionFileExtensions.Contains(System.IO.Path.GetExtension(sourceFullName).ToLowerInvariant())
                ? CompressionLevel.NoCompression
                : CompressionLevel.Fastest;
        }

        internal void ifcXmlSerializer_MessageLogged(object sender, Ifc4.EventArgs.MessageLoggedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            Workspace.CurrentWorkspace.RaiseMessageLogged(e.Message);
        }

        // Checksum kann nur nach dem Speichern oder direkt nach dem Öffnen der Datei ermittelt werden
        private string _Checksum;
        public string Checksum
        {
            get
            {
                if (String.IsNullOrEmpty(_Checksum))
                    return "File not saved!";

                return _Checksum;
            }
            set { _Checksum = value; }
        }

        private string ReadChecksum(string fullName)
        {
            var fileType = GetFileType(fullName);
            if (fileType == IfcFileType.IfcXml)
            {
                using (FileStream fileStream = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                {
                    SHA256Managed sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(fileStream);
                    Checksum = BitConverter.ToString(checksum).Replace("-", String.Empty);
                    return Checksum;
                }
            }
            else if (fileType == IfcFileType.IfcZip)
            {
                using (ZipArchive zipArchive = ZipFile.Open(fullName, ZipArchiveMode.Read))
                {
                    string entryName = System.IO.Path.ChangeExtension(System.IO.Path.GetFileNameWithoutExtension(fullName), ".ifcxml");
                    ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault(item => item.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                    if (zipArchiveEntry != null)
                    {
                        using (Stream stream = zipArchiveEntry.Open())
                        {
                            SHA256Managed sha = new SHA256Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            Checksum = BitConverter.ToString(checksum).Replace("-", String.Empty);
                            return Checksum;
                        }
                    }
                }
            }
            return String.Empty;
        }

        public bool Validate(string fullName)
        {
            if (this.FileType == Document.IfcFileType.IfcZip)
            {
                fullName = System.IO.Path.ChangeExtension(fullName, ".ifcxml");
                string tempFullName;
                if(this.ExtractToTemp(System.IO.Path.GetFileName(fullName), out tempFullName))
                {
                    fullName = tempFullName;
                }
            }

            Ifc4.Validation validation = new Ifc4.Validation();
            validation.Info += validation_Info;

            bool schemaValidationResult = validation.Validate(fullName, true);
            // custom validation
            CustomValidation customValidation = new Ifc4.Document.CustomValidation(this.Document);
            bool customValidationResult = customValidation.Validate();

            return (schemaValidationResult && customValidationResult);
        }

        void validation_Info(object sender, EventArgs.MessageLoggedEventArgs e)
        {
            Workspace.CurrentWorkspace.RaiseMessageLogged(e.Message);
        }

        private Ifc4.EventArgs.MessageLoggedEventArgs m_MessageLoggedEventArgs;
        private void WriteLog(string s)
        {
            if (m_MessageLoggedEventArgs == null)
                m_MessageLoggedEventArgs = new Ifc4.EventArgs.MessageLoggedEventArgs();

            m_MessageLoggedEventArgs.AddMessage(s);
        }

        internal class CustomValidation
        {
            public CustomValidation(Ifc4.Document document)
            {
                Document = document;
            }

            private Ifc4.Document Document { get; set; }

            private bool m_IfcSystemValidationInitialized = false;
            private bool m_FacilityValidationInitialized = false;
            private int m_FacilitiesCounter = 0;
            private int m_FacilitiesIfcSystemCounter = 0;
            private int m_FacilitiesIfcObjectDefinitionCounter = 0;
            private int m_AttributesWithValueCounter = 0;

            private void ResetVariables()
            {
                m_IfcSystemValidationInitialized = false;
                m_FacilityValidationInitialized = false;
                m_FacilitiesCounter = 0;
                m_FacilitiesIfcSystemCounter = 0;
                m_FacilitiesIfcObjectDefinitionCounter = 0;
                m_AttributesWithValueCounter = 0;
            }

            private int m_ErrorCount = 0;

            public bool Validate()
            {
                m_ErrorCount = 0;
                int n = 60;

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged("Datei wird überprüft.", true);
                Workspace.CurrentWorkspace.RaiseMessageLogged("Bitte warten...", true);

                StringBuilder sbAll = new StringBuilder();

                var originalIfcSites = Document.IfcXmlDocument.Items.OfType<Ifc4.IfcSite>().ToList();
                var originalIfcBuildings = Document.IfcXmlDocument.Items.OfType<Ifc4.IfcBuilding>().ToList();
                var originalIfcBuildingStoreys = Document.IfcXmlDocument.Items.OfType<Ifc4.IfcBuildingStorey>().ToList();
                var originalIfcSpaces = Document.IfcXmlDocument.Items.OfType<Ifc4.IfcSpace>().ToList();

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Räumliche Struktur Objekte"));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Liegenschaften: {0}", originalIfcSites.Count));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Gebäude: {0}", originalIfcBuildings.Count));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Etagen: {0}", originalIfcBuildingStoreys.Count));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Räume: {0}", originalIfcSpaces.Count));

                int nSites = 0;
                bool siteValidationInitialized = false;

                int nBuildings = 0;
                bool buildingValidationInitialized = false;

                int nBuildingStoreys = 0;
                double buildingStoreyGrossFloorArea = 0;
                bool buildingStoreyValidationInitialized = false;

                int nSpaces = 0;
                double spaceNetFloorArea = 0;
                bool spacesValidationInitialized = false;

                Document.ResetValidationProperties();

                int errorCount = 0;

                foreach (var site in Document.Project.Sites)
                {
                    nSites++;
                    if (!siteValidationInitialized)
                    {
                        site.AddValidationProperty(Document.PropertyName(() => site.Name), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                        site.AddValidationProperty(Document.PropertyName(() => site.LongName), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                        //site.AddValidationProperty(Document.PropertyName(() => site.Description), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                        siteValidationInitialized = true;
                    }
                    originalIfcSites.Remove(site);
                    site.Validate(sbAll, out errorCount);
                    m_ErrorCount += errorCount; // site.ValidationProperties.Count(item => item.Error);

                    foreach (var building in site.Buildings)
                    {
                        nBuildings++;
                        if (!buildingValidationInitialized)
                        {
                            building.AddValidationProperty(Document.PropertyName(() => building.Name), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            building.AddValidationProperty(Document.PropertyName(() => building.LongName), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            //building.AddValidationProperty(Document.PropertyName(() => building.Description), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            buildingValidationInitialized = true;
                        }
                        originalIfcBuildings.Remove(building);
                        building.Validate(sbAll, out errorCount);
                        m_ErrorCount += errorCount; // building.ValidationProperties.Count(item => item.Error);

                        foreach (var buildingStorey in building.BuildingStoreys)
                        {
                            nBuildingStoreys++;
                            if (!buildingStoreyValidationInitialized)
                            {
                                buildingStorey.AddValidationProperty(Document.PropertyName(() => buildingStorey.Name), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                                buildingStorey.AddValidationProperty(Document.PropertyName(() => buildingStorey.LongName), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                                //buildingStorey.AddValidationProperty(Document.PropertyName(() => buildingStorey.Description), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                                buildingStorey.AddValidationProperty(Document.PropertyName(() => buildingStorey.GrossFloorArea), typeof(Double), ValidationEnumType.GreaterThanOrEqual, 0);
                                buildingStoreyValidationInitialized = true;
                            }

                            buildingStoreyGrossFloorArea += buildingStorey.GrossFloorArea;

                            originalIfcBuildingStoreys.Remove(buildingStorey);
                            buildingStorey.Validate(sbAll, out errorCount);
                            m_ErrorCount += errorCount; // buildingStorey.ValidationProperties.Count(item => item.Error);

                            foreach (var space in buildingStorey.Spaces)
                            {
                                nSpaces++;
                                if (!spacesValidationInitialized)
                                {
                                    space.AddValidationProperty(Document.PropertyName(() => space.Name), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                                    space.AddValidationProperty(Document.PropertyName(() => space.LongName), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                                    //space.AddValidationProperty(Document.PropertyName(() => space.Description), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                                    space.AddValidationProperty(Document.PropertyName(() => space.NetFloorArea), typeof(Double), ValidationEnumType.GreaterThanOrEqual, 0);
                                    space.AddValidationProperty(Document.PropertyName(() => space.IfcClassificationReference), typeof(Object), ValidationEnumType.IsNotNull);
                                    spacesValidationInitialized = true;
                                }

                                spaceNetFloorArea += space.NetFloorArea;

                                originalIfcSpaces.Remove(space);
                                space.Validate(sbAll, out errorCount);
                                m_ErrorCount += errorCount; // space.ValidationProperties.Count(item => item.Error);
                            }
                        }
                    }
                }

                ResetVariables();

                FacilitiesCounter(Document.Project.Facilities);

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Räumliche Struktur Objekte MIT Verknüpfung (Hierarchie)"));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Liegenschaften: {0}", nSites));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Gebäude: {0}", nBuildings));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Etagen: {0}", nBuildingStoreys));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Räume: {0}", nSpaces));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Summe Raumfläche: {0}[m²]", buildingStoreyGrossFloorArea));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty);
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Objekte"));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl der Objekte [IfcSystem]: {0}", m_FacilitiesIfcSystemCounter));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl der Objekte [IfcObjectDefinition]: {0}", m_FacilitiesIfcObjectDefinitionCounter));

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Räumliche Struktur Objekte OHNE Verknüpfung"));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Liegenschaften: {0}", originalIfcSites.Count));
                foreach (var item in originalIfcSites)
                {
                    Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("\tLiegenschaft: id={0} !", item.Id));
                }
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Gebäude: {0}", originalIfcBuildings.Count));
                foreach (var item in originalIfcBuildings)
                {
                    Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("\tGebäude: id={0} !", item.Id));
                }
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Etagen: {0}", originalIfcBuildingStoreys.Count));
                foreach (var item in originalIfcBuildingStoreys)
                {
                    Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("\tEtage: id={0} !", item.Id));
                }
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Räume: {0}", originalIfcSpaces.Count));
                foreach (var item in originalIfcSpaces)
                {
                    Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("\tRaum: id={0} !", item.Id));
                }


                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Nicht gefüllte Felder"));
                Workspace.CurrentWorkspace.RaiseMessageLogged(sbAll.ToString());
                sbAll.Clear();

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Überprüfe Objekte..."));

                Document.UseFacilityCache = true;

                //bool facilityValidationStatus = ValidateFacilities(Document.Project.Facilities, sbAll);
                ValidateFacilities(Document.Project.Facilities, sbAll);

                Document.UseFacilityCache = false;
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("{0} Objekte überprüft", m_FacilitiesCounter));
                //Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Anzahl Attribute mit Wert: {0}", m_AttributesWithValueCounter));
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, sbAll.ToString(), false);
                // -----------------------------------------------------------------------------

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));
                Workspace.CurrentWorkspace.RaiseMessageLogged("Überprüfung beendet.", true);

                if (m_ErrorCount > 0)
                {
                    Workspace.CurrentWorkspace.RaiseMessageLogged(String.Format("Error(s): {0}", m_ErrorCount));
                }

                Workspace.CurrentWorkspace.RaiseMessageLogged(this, String.Empty.PadRight(n, '*'));

                return (m_ErrorCount == 0);

            }

            private void FacilitiesCounter(Ifc4.CcFacilities<CcFacility> facilities)
            {
                foreach (var facility in facilities)
                {
                    if (facility.IfcSystem != null)
                        m_FacilitiesIfcSystemCounter++;

                    if (facility.IfcObjectDefinition != null)
                        m_FacilitiesIfcObjectDefinitionCounter++;

                    FacilitiesCounter(facility.Facilities);
                }
            }

            private void ValidateFacilities(Ifc4.CcFacilities<CcFacility> facilities, StringBuilder sb)
            {
                int errorCount = 0;

                foreach (var facility in facilities)
                {
                    m_FacilitiesCounter++;

                    if (m_FacilitiesCounter % 100 == 0 && m_FacilitiesCounter != 0)
                        Workspace.CurrentWorkspace.RaiseMessageLogged(String.Format("Objekt {0}/{1}...", m_FacilitiesCounter, m_FacilitiesIfcSystemCounter + m_FacilitiesIfcObjectDefinitionCounter), true);

                    if (facility.IfcSystem != null)
                    {
                        if (!m_IfcSystemValidationInitialized)
                        {
                            facility.IfcSystem.AddValidationProperty(Document.PropertyName(() => facility.IfcSystem.Name), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            facility.IfcSystem.AddValidationProperty(Document.PropertyName(() => facility.IfcSystem.Description), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            m_IfcSystemValidationInitialized = true;
                        }
                        facility.IfcSystem.Validate(sb, out errorCount);
                        m_ErrorCount += errorCount;
                    }
                    if (facility.IfcObjectDefinition != null)
                    {
                        if (!m_FacilityValidationInitialized)
                        {
                            facility.AddValidationProperty(Document.PropertyName(() => facility.Number), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            facility.AddValidationProperty(Document.PropertyName(() => facility.Key), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            facility.AddValidationProperty(Document.PropertyName(() => facility.Description), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            facility.AddValidationProperty(Document.PropertyName(() => facility.Location), typeof(Object), ValidationEnumType.IsNotNull);
                            facility.AddValidationProperty(Document.PropertyName(() => facility.ObjectTypeId), typeof(String), ValidationEnumType.IsNotNullOrNotEmpty);
                            m_FacilityValidationInitialized = true;
                        }
                        facility.Validate(sb, out errorCount);
                        m_ErrorCount += errorCount;

                        ValidatePropertySet(facility, sb);

                        ValidateAttributeWithValue(facility, sb);

                    }

                    ValidateFacilities(facility.Facilities, sb);

                }
            }

            private void ValidatePropertySet(Ifc4.CcFacility facility, StringBuilder messages)
            {
                if (facility == null)
                    return;

                //IfcPropertySetTemplate ifcPropertySetTemplate = facility.GetIfcPropertySetTemplate();
                //IfcPropertySet ifcPropertySet = (facility.IfcObjectDefinition != null) ? facility.IfcObjectDefinition.GetIfcPropertySetFromRelatingPropertyDefinition() : null;

                IfcPropertySetTemplate ifcPropertySetTemplate;
                IfcPropertySet ifcPropertySet;


                ifcPropertySet = facility.IfcPropertySet;
                ifcPropertySetTemplate = facility.IfcPropertySetTemplate;

                if (ifcPropertySet != null)
                {
                    if (ifcPropertySetTemplate == null)
                    {
                        ifcPropertySetTemplate = facility.GetIfcPropertySetTemplate();
                    }
                }

                if (ifcPropertySetTemplate != null && ifcPropertySet != null)
                {
                    IEnumerable<IfcPropertyTemplate> propertyTemplates = null;
                    IEnumerable<IfcProperty> properties = null;

                    if (ifcPropertySetTemplate.HasPropertyTemplates == null)
                        messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("[PropertySetTemplate] für Objekt ${0}$ hat keine Property Templates!", facility.TempId)));
                    else
                        propertyTemplates = ifcPropertySetTemplate.HasPropertyTemplates.Items;

                    if (ifcPropertySet.HasProperties == null)
                    {
                        messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("[PropertySet] für Objekt ${0}$ hat keine Properties!", facility.TempId)));
                        m_ErrorCount++;
                    }
                    else
                        properties = ifcPropertySet.HasProperties.Items;

                    if (propertyTemplates != null && properties != null)
                    {
                        foreach (var name in propertyTemplates.Select(item => item.Name).Except(properties.Select(item => item.Name)))
                        {
                            messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("Objekt ${0}$ - [PropertySet] Property '{1}' nicht in IfcPropertySet id='{2}' enthalten! ", facility.TempId, name, ifcPropertySet.Id)));
                            m_ErrorCount++;
                        }

                        foreach (var name in properties.Select(item => item.Name).Except(propertyTemplates.Select(item => item.Name)))
                        {
                            messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("Objekt ${0}$ -[PropertySetTemplate] Property '{1}' nicht in IfcPropertySetTemplate id='{2}' enthalten!", facility.TempId, name, ifcPropertySetTemplate.Id)));
                            m_ErrorCount++;
                        }


                        List<string> validNames = propertyTemplates.Select(item => item.Name).Intersect(properties.Select(item => item.Name)).ToList();
                        foreach (var ifcSimplePropertyTemplate in ifcPropertySetTemplate.HasPropertyTemplates.Items.OfType<IfcSimplePropertyTemplate>().Where(item => item.TemplateType == IfcSimplePropertyTemplateTypeEnum.PEnumeratedvalue && validNames.Contains(item.Name)))
                        {
                            IfcPropertyEnumeratedValue ifcPropertyEnumeratedValue = properties.OfType<IfcPropertyEnumeratedValue>().FirstOrDefault(item => item.Name == ifcSimplePropertyTemplate.Name);
                            if (ifcPropertyEnumeratedValue != null && ifcPropertyEnumeratedValue.EnumerationValues != null)
                            {
                                IfcLabelwrapper ifcLabelWrapper = ifcPropertyEnumeratedValue.EnumerationValues.Items.OfType<IfcLabelwrapper>().FirstOrDefault();
                                if (ifcLabelWrapper != null && !String.IsNullOrEmpty(ifcLabelWrapper.Value))
                                {
                                    if (!GetEnumerationValues(ifcSimplePropertyTemplate, messages).Contains(ifcLabelWrapper.Value))
                                    {
                                        messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("IfcPropertySet id='{0}' Property '{1}' Enum Wert '{2}' ist ungültig! ", ifcSimplePropertyTemplate.Id, ifcSimplePropertyTemplate.Name, ifcLabelWrapper.Value)));
                                        m_ErrorCount++;
                                    }
                                }
                            }
                        }

                    }
                }

            }

            private Dictionary<string, List<string>> m_IfcSimplePropertyTemplateEnumerationValues = new Dictionary<string, List<string>>();
            private List<string> GetEnumerationValues(Ifc4.IfcSimplePropertyTemplate ifcSimplePropertyTemplate, StringBuilder messages)
            {
                if (ifcSimplePropertyTemplate == null)
                    return new List<string>();

                if (
                        ifcSimplePropertyTemplate.Enumerators == null ||
                        ifcSimplePropertyTemplate.Enumerators.EnumerationValues == null
                    )
                {
                    messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("IfcSimplePropertyTemplate id='{0}' Property '{1}' hat keine enum Werte! ", ifcSimplePropertyTemplate.Id, ifcSimplePropertyTemplate.Name)));
                    return new List<string>();
                }

                List<string> enumerationValues;
                if (!m_IfcSimplePropertyTemplateEnumerationValues.TryGetValue(ifcSimplePropertyTemplate.Id, out enumerationValues))
                {
                    enumerationValues = ifcSimplePropertyTemplate.Enumerators.EnumerationValues.Items.OfType<IfcLabelwrapper>().Select(item => item.Value).ToList();
                    m_IfcSimplePropertyTemplateEnumerationValues.Add(ifcSimplePropertyTemplate.Id, enumerationValues);
                }

                return enumerationValues;

            }

            private void ValidateAttributeWithValue(Ifc4.CcFacility facility, StringBuilder messages)
            {
                if (facility == null)
                    return;

                foreach (Ifc4.CustomModel.CustomPropertyDescriptor customPropertyDescriptor in System.ComponentModel.TypeDescriptor.GetProperties(facility).OfType<Ifc4.CustomModel.CustomPropertyDescriptor>())
                {
                    var value = customPropertyDescriptor.GetValue(facility);

                    if (customPropertyDescriptor.LastValueError != null)
                    {
                        messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("{0}", customPropertyDescriptor.ToString("I"))));
                        if (customPropertyDescriptor.LastValueError.InnerException != null)
                        {
                            messages.Append
                                (
                                    Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage
                                        (
                                            String.Format("{0} Objekt ${1}$",
                                                customPropertyDescriptor.LastValueError.InnerException.Message,
                                                facility.TempId)
                                        )
                                );
                        }
                        customPropertyDescriptor.ResetLastValueError();
                        m_ErrorCount++;
                    }
                }
            }
        }


        [DllImport("urlmon.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = false)]
        private static extern int FindMimeFromData(
                                                    IntPtr pBC,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
                                                    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
                                                    int cbSize,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
                                                    int dwMimeFlags,
                                                    out IntPtr ppwzMimeOut,
                                                    int dwReserved);

        //private static string GetMimeFromByteArray(byte[] dataBytes)
        //{
        //    string mimeRet = "unknown/unknown";
        //    try
        //    {
        //        IntPtr outPtr;
        //        int ret = FindMimeFromData(IntPtr.Zero, null, dataBytes, dataBytes.Length, null, 0, out outPtr, 0);
        //        if (ret == 0 && outPtr != IntPtr.Zero)
        //        {
        //            mimeRet = Marshal.PtrToStringUni(outPtr);
        //            Marshal.FreeCoTaskMem(outPtr);
        //        }
        //    }
        //    catch { }
        //    return mimeRet;
        //}

        public string GetMimeFromFile(string fullName)
        {
            string mimeRet = "unknown/unknown";
            if (!File.Exists(fullName))
                throw new System.IO.FileNotFoundException();

            int nBytes = 256;
            byte[] buffer = new byte[nBytes];
            try
            {
                using (FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length < nBytes)
                        nBytes = (int)fs.Length;

                    fs.Read(buffer, 0, nBytes);
                }

                IntPtr outPtr;
                int ret = FindMimeFromData(IntPtr.Zero, null, buffer, nBytes, null, 0, out outPtr, 0);
                if (ret == 0 && outPtr != IntPtr.Zero)
                {
                    mimeRet = Marshal.PtrToStringUni(outPtr);
                    Marshal.FreeCoTaskMem(outPtr);
                }
            }
            catch { }
            return mimeRet;
        }


    }

}
