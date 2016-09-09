using System;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ifc.NET.Interfaces;

namespace Ifc.NET
{
    public partial class Document : BaseObject
    {
        public delegate void MessageLoggedEventHandler(object sender, Ifc.NET.EventArgs.MessageLoggedEventArgs e);
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

        private Dictionary<string, Ifc.NET.IfcPropertySetTemplate> m_IfcPropertySetTemplateCollection;
        internal Dictionary<string, Ifc.NET.IfcPropertySetTemplate> IfcPropertySetTemplateCollection
        {
            get
            {
                if (m_IfcPropertySetTemplateCollection == null)
                {
                    m_IfcPropertySetTemplateCollection = new Dictionary<string, IfcPropertySetTemplate>();
                    foreach (var item in this.IfcXmlDocument.Items.OfType<Ifc.NET.IfcPropertySetTemplate>())
                        m_IfcPropertySetTemplateCollection.Add(item.Id, item);
                }
                return m_IfcPropertySetTemplateCollection;
            }
        }

        internal Ifc.NET.IfcPropertySetTemplate GetIfcPropertySetTemplate(string id)
        {
            if (String.IsNullOrEmpty(id))
                return null;

            //if (m_IfcPropertySetTemplateCollection == null)
            //{
            //    m_IfcPropertySetTemplateCollection = new Dictionary<string, IfcPropertySetTemplate>();
            //    foreach (var item in this.IfcXmlDocument.Items.OfType<Ifc.NET.IfcPropertySetTemplate>())
            //        m_IfcPropertySetTemplateCollection.Add(item.Id, item);
            //}

            //Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate;
            //if(m_IfcPropertySetTemplateCollection.TryGetValue(id, out ifcPropertySetTemplate))
            //    return ifcPropertySetTemplate;

            Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate;
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

        private IEnumerable<ABC> m_RelatedObjectsRefs = null;
        public IEnumerable<Ifc.NET.IfcPropertySetTemplate> GetIfcPropertySetTemplateCollection(CcFacility facility)
        {
            IEnumerable<Ifc.NET.IfcPropertySetTemplate> ifcPropertySetTemplateCollection = null;
            if (facility != null && !String.IsNullOrEmpty(facility.ObjectTypeId))
            {
                string objectTypeId = facility.ObjectTypeId;
                if (m_RelatedObjectsRefs == null)
                {
                    Ifc.NET.Document document = facility.GetParent<Ifc.NET.Document>();
                    IEnumerable<Ifc.NET.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcRelAssociatesClassification>().ToList();
                    m_RelatedObjectsRefs = (from ifcRelAssociatesClassification in ifcRelAssociatesClassificationCollection
                                            from relatedObject in ifcRelAssociatesClassification.RelatedObjects.Items
                                            where ifcRelAssociatesClassification.RelatingClassification.Item != null // && ifcRelAssociatesClassification.RelatingClassification.Item.Ref == objectTypeId
                                            select new ABC(ifcRelAssociatesClassification.RelatingClassification.Item, relatedObject)).ToList();

                    //m_IfcPropertySetTemplateCollection = document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcPropertySetTemplate>().ToList();
                }

                var relatedObjectsRefs = m_RelatedObjectsRefs.Where(item => item.A.Ref == objectTypeId).Select(item => item.B.Ref);
                ifcPropertySetTemplateCollection = IfcPropertySetTemplateCollection.Values.Where(item => relatedObjectsRefs.Contains(item.Id));
            }
            return ifcPropertySetTemplateCollection;
        }

        //private Dictionary<string, Ifc.NET.IfcPropertySetTemplate> m_IfcPropertySetTemplateCollectionByIfcPropertySetId;
        //internal Ifc.NET.IfcPropertySetTemplate GetIfcPropertySetTemplateFromIfcPropertySetId(string id)
        //{
        //    //<IfcRelDefinesByTemplate>
        //    //  <RelatedPropertySets>
        //    //    <IfcPropertySet ref="i9343" xsi:nil="true" />
        //    //  </RelatedPropertySets>
        //    //  <RelatingTemplate ref="i992" xsi:nil="true" />
        //    //</IfcRelDefinesByTemplate>

        //    if (String.IsNullOrEmpty(id))
        //        return null;

        //    if (m_IfcPropertySetTemplateCollectionByIfcPropertySetId == null)
        //    {
        //        m_IfcPropertySetTemplateCollectionByIfcPropertySetId = new Dictionary<string, IfcPropertySetTemplate>();

        //        IEnumerable<Ifc.NET.IfcRelDefinesByTemplate> ifcIfcRelDefinesByTemplateCollection = from item in this.IfcXmlDocument.Items.OfType<Ifc.NET.IfcRelDefinesByTemplate>()
        //                                                                                                          where item.RelatedPropertySets != null &&
        //                                                                                                          item.RelatedPropertySets.Items.OfType<Ifc.NET.IfcPropertySet>().Any()
        //                                                                                                          select item;

        //        string ifcPropertySetId;
        //        foreach (var ifcIfcRelDefinesByTemplate in ifcIfcRelDefinesByTemplateCollection)
        //        {
        //            IfcPropertySetTemplate tmp = ifcIfcRelDefinesByTemplate.RelatingTemplate;
        //            if (tmp.IsRef)
        //                tmp = this.GetIfcPropertySetTemplate(tmp.Ref);

        //            foreach (var item in ifcIfcRelDefinesByTemplate.RelatedPropertySets.Items)
        //            {
        //                ifcPropertySetId = item.IsRef ? item.Ref : item.Id;
        //                if (!m_IfcPropertySetTemplateCollectionByIfcPropertySetId.ContainsKey(ifcPropertySetId))
        //                    m_IfcPropertySetTemplateCollectionByIfcPropertySetId.Add(ifcPropertySetId, tmp);
        //            }
        //        }
        //    }

        //    Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate;
        //    if (m_IfcPropertySetTemplateCollectionByIfcPropertySetId.TryGetValue(id, out ifcPropertySetTemplate))
        //        return ifcPropertySetTemplate;

        //    return null;

        //}

        private Dictionary<string, Ifc.NET.IfcPropertySetTemplate> m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId;
        internal Ifc.NET.IfcPropertySetTemplate GetIfcPropertySetTemplateFromIfcClassificationReferenceId(string id)
        {
            if (String.IsNullOrEmpty(id))
                return null;

            if (m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId == null)
            {
                m_IfcPropertySetTemplateCollectionByIfcClassificationReferenceId = new Dictionary<string, IfcPropertySetTemplate>();

                IEnumerable<Ifc.NET.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = from item in this.IfcXmlDocument.Items.OfType<Ifc.NET.IfcRelAssociatesClassification>()
                                                                                                                  where item.RelatedObjects != null &&
                                                                                                                  item.RelatedObjects.Items.OfType<Ifc.NET.IfcPropertySetTemplate>().Any()
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

            Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate;
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
        //public static int gID = m_StartId;
        private static Queue<int> m_NextFreeRange = new Queue<int>();

        internal static void ResetId()
        {
            //gID = m_StartId;
            Ifc.NET.Entity.AllIds = new List<string>();
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
                    var ids = Ifc.NET.Entity.AllIds.Where(item => item != null && item.StartsWith("i")).Select(item => Convert.ToInt32(item.Substring(1))).ToList();
                    ids.Sort();
                    Enumerable.Range(m_StartId, Int32.MaxValue / 2).Except(ids).Take(10000).ToList().ForEach(id => m_NextFreeRange.Enqueue(id));
                }
                nextId = m_NextFreeRange.Dequeue();

                // wenn Dateien noch nicht alle Ifc... Objekte bereitstellen
                // ist das zu langsam
                // für Dateien, die mit diesem Tool erstellt wurden - kein Problem
                //
                //var l = Ifc.NET.Entity.AllIds.Where(item => item != null && item.StartsWith("i")).Select(item => Convert.ToInt32(item.Substring(1))).ToList();
                //l.Sort();
                //var a = l.ToArray();
                //int n = a.Length;

                //nextId = m_StartId;
                //if (n > 0)
                //{
                //    int minId = a[0];
                //    int maxId = a[n - 1];
                //    for (int i = 0; i < n; i++)
                //    {
                //        int id = a[i];
                //        if ((minId + i) < id)
                //        {
                //            nextId = (minId + i);
                //            break;
                //        }
                //        nextId = id + 1;
                //    }
                //}
            }
            return "i" + nextId.ToString();
        }

        [System.ComponentModel.Browsable(false)]
        public IfcProject Project
        {
            get
            {
                var instance = IfcXmlDocument.Items.OfType<IfcProject>().SingleOrDefault();
                if (instance == null)
                {
                    instance = AddNew<IfcProject>();
                }
                return instance;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public IEnumerable<Ifc.NET.IfcClassification> Classifications
        {
            get { return IfcXmlDocument.Items.OfType<Ifc.NET.IfcClassification>(); }
        }

        [System.ComponentModel.Browsable(false)]
        public IEnumerable<Ifc.NET.IfcClassificationReference> ClassificationReferences
        {
            get { return IfcXmlDocument.Items.OfType<Ifc.NET.IfcClassificationReference>(); }
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
                Organization = "eTASK Immobilien Software GmbH",
                PreprocessorVersion = "IFC.NET",
                OriginatingSystem = "IFC.NET Sample File",
                Authorization = "file created with IFC.NET",
                Documentation = "ViewDefinition [notYetAssigned]" // version 
            };

            BaseObject.UnlockEvents(eventType);
        }

        public void PopulateIndividualUosHeader(string author ,string organization, string originatingSystem, string authorization )
            
        {
            EventType eventType = BaseObject.LockEvents();

            this.IfcXmlDocument.Header = new UosHeader()
            {
                //<time_stamp>2013-04-17T14:34:08.5257209+02:00</time_stamp>
                Name = String.Empty, // System.IO.Path.GetFileNameWithoutExtension(fullName),
                // time_stamp = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                TimeStamp = DateTime.Now,
                Author = author,
                Organization = organization,
                PreprocessorVersion = "IFC.NET",
                OriginatingSystem = originatingSystem,
                Authorization = authorization,
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

        internal IEnumerable<System.ComponentModel.PropertyDescriptor> GetCustomPropertyDescriptorsFromIfcClassificationReferenceId(Ifc.NET.CcFacility facility, string id)
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
                                          //where Ifc.NET.CustomModel.CustomPropertyDescriptor.TryGetFormVisiblePositionFromObject(null, pd, out position) && !Ifc.NET.CustomModel.CustomPropertyDescriptor.HasXmlIgnoreAttribute(pd)
                                      let visiblePosition = position == 0 ? end++ : position
                                      orderby visiblePosition
                                      select pd;

                if (UseFacilityCache)
                    m_CustomPopertyDescriptorsFromIfcClassificationReferenceId.Add(id, propertyDescriptors);
            }

            return propertyDescriptors;

        }


        public Ifc.NET.Units Units
        {
            get
            {
                return Ifc.NET.Units.Current;
            }
        }

        //[System.ComponentModel.Browsable(false)]
        //public IfcProject Project { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcIfcOrganizations<IfcOrganization> Organizations { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcIfcClassifications<IfcClassification> Classifications { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcIfcRelAssociatesClassifications<IfcRelAssociatesClassification> RelAssociatesClassifications { get; private set; }



        //[System.ComponentModel.Browsable(false)]
        //public IfcPersons Persons { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcPersonAndOrganizations PersonAndOrganizations { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcApplications Applications { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcOwnerHistories OwnerHistories { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcRelAggregatesCollection RelAggregatesCollection { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcPropertySingleValues PropertySingleValues { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcQuantityAreas QuantityAreas { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcRelDefinesByPropertiesCollection RelDefinesByPropertiesCollection { get; private set; }

        ////[System.ComponentModel.Browsable(false)]
        ////public CcPropertySets PropertySets { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //public CcPostalAddresses PostalAddresses { get; private set; }

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
        public void SetDirty()
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
            Workspace.CurrentWorkspace.RaiseDocumentClosed(this, new Workspace.DocumentClosedEventArgs(this));
        }

        public string FullName { get; set; }

        //private void NamespaceCorrection(string fullName)
        //{
        //    // xmlns:xsd="http://www.w3.org/2001/XMLSchema" 

        //    XDocument xDocument = XDocument.Load(fullName);

        //    var a = xDocument.Root.Attributes();

        //    RaiseMessageLogged("Remove namespace...");
        //    xDocument.Root.Attributes(XName.Get("ifc", @"http://www.w3.org/2000/xmlns/")).Remove();

        //    RaiseMessageLogged(String.Format("Save file '{0}'...", fullName));
        //    xDocument.Save(fullName);

        //    //// ---------------------------------------
        //    //// manually correction!!!
        //    //RaiseMessageLogged("Namespace correction...");
        //    //string text = System.IO.File.ReadAllText(fullName);
        //    //text = text.Replace("<ifcXML ", String.Format("<ifcXML xmlns:ifc=\"{0}\" ", IfcNs.NamespaceName));
        //    //text = text.Replace(" xmlns=\"\"", "");
        //    //System.IO.File.WriteAllText(fullName, text);
        //    //// ---------------------------------------

        //}

        //private void Correction(string fullName)
        //{
        //    // Interne
        //    XDocument xDocument = XDocument.Load(fullName);

        //    RaiseMessageLogged("Remove namespace...");
        //    xDocument.Root.Attributes(XName.Get("ifc", @"http://www.w3.org/2000/xmlns/")).Remove();

        //    RaiseMessageLogged(String.Format("Save file '{0}'...", fullName));
        //    xDocument.Save(fullName);

        //    // ---------------------------------------
        //    // manually correction!!!
        //    RaiseMessageLogged("Namespace correction...");
        //    string text = System.IO.File.ReadAllText(fullName);
        //    text = text.Replace("<ifcXML ", String.Format("<ifcXML xmlns:ifc=\"{0}\" ", IfcNs.NamespaceName));
        //    text = text.Replace(" xmlns=\"\"", "");
        //    System.IO.File.WriteAllText(fullName, text);
        //    // ---------------------------------------

        //    List<string> lines = new List<string>();
        //    foreach (string lineTmp in System.IO.File.ReadAllLines(fullName))
        //    {
        //        string line = lineTmp;
        //        if (line.IndexOf(" xsi:nil=\"true\"") != -1 || line.IndexOf(" xsi:nil=\"false\"") != -1)
        //        {
        //        }
        //        else
        //        {
        //            if (line.IndexOf(" ref=\"") != -1)
        //                line = line.Replace(" ref=\"", " xsi:nil=\"true\" ref=\"");
        //        }
        //        lines.Add(line);
        //    }

        //    System.IO.File.WriteAllLines(fullName, lines.ToArray());

        //}

        public Ifc.NET.IfcXML IfcXmlDocument { get; internal set; }

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


            //List<string> lines = new List<string>();
            //foreach (var line in System.IO.File.ReadAllLines(fullName))
            //{
            //    if (line.IndexOf(" ref=\"") != -1 && line.IndexOf(" xsi:type=\"") != -1)
            //        lines.Add(line.Replace(" xsi:nil=\"true\"", ""));
            //    else
            //        lines.Add(line);
            //}
            //System.IO.File.WriteAllLines(fullName, lines.ToArray());

        }

        private void RemoveComment(string sourceFullName, string targetFullName, int removetype = 0)
        {
            List<string> lines = new List<string>();
            foreach (var line in System.IO.File.ReadAllLines(sourceFullName))
            {
                if (line.Trim().StartsWith("<!--") && line.Trim().EndsWith("-->"))
                    continue;

                //if (line.IndexOf(" ref=\"") != -1)
                //    lines.Add(line.Replace(" xsi:nil=\"true\"", ""));

                //bool addLine = true;
                //lines.Add(line.Replace(" xsi:nil=\"true\"", ""));

                lines.Add(line);
            }
            System.IO.File.WriteAllLines(targetFullName, lines.ToArray());
        }

        public bool IsInOpenProcess { get; internal set; }

        public Ifc.NET.IfcXML Open(string fullName, out string message, IfcFileType ifcFileType = IfcFileType.IfcXml)
        {
            message = "";
            EventType eventType = BaseObject.LockEvents();

            try
            {

                Ifc.NET.IfcXML ifcXML = null;


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
                        ifcXML = JV.XmlProcessing<Ifc.NET.IfcXML>.Read(tempFullName);
                        // JV.XmlProcessing<Ifc.NET.IfcXML>.SerializeBinary("c:\\jv\\CAFM-Connect-Katalog.data", ifcXML);

                        try { System.IO.File.Delete(tempFullName); }
                        catch { }

                        break;

                    case IfcFileType.IfcXmlBin:
                        //tempFullName = System.IO.Path.GetTempFileName() + ".ifcxml";
                        //System.IO.File.Copy(fullName, tempFullName, true);
                        //ifcXML = JV.XmlProcessing<Ifc.NET.IfcXML>.ReadBinary(tempFullName);

                        ifcXML = JV.XmlProcessing<Ifc.NET.IfcXML>.ReadBinary(fullName);

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
                //Ifc.NET.Serialization.IfcXmlSerializer ifcXmlSerializer = new Ifc.NET.Serialization.IfcXmlSerializer(typeof(Ifc.NET.IfcXML));
                //ifcXmlSerializer.MessageLogged += ifcXmlSerializer_MessageLogged;


                //Ifc.NET.IfcXML ifcXML = null;
                //using (System.IO.FileStream fileStream = new System.IO.FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                //{
                //    ifcXML = ifcXmlSerializer.Deserialize(fileStream) as Ifc.NET.IfcXML;
                //}
                //IfcXmlDocument = ifcXML;
                //this.FullName = fullName;
                //this.Activate();
                //return ifcXML;
                // -------------------------------------------------------------
                //// 4. Möglichkeit
                //Ifc.NET.IfcXML ifcXML = JV.XmlProcessing<Ifc.NET.IfcXML>.Read(fullName);
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

        public enum IfcFileType
        {
            Undefined,
            IfcXml,
            IfcXmlBin
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

        public IfcFileType FileType
        {
            get
            {
                if (String.IsNullOrEmpty(FullName))
                    return IfcFileType.Undefined;

                string extension = System.IO.Path.GetExtension(FullName);
                if (String.IsNullOrEmpty(extension))
                    return IfcFileType.Undefined;

                if (extension.Equals(".ifcxml", StringComparison.OrdinalIgnoreCase))
                    return IfcFileType.IfcXml;

                //if (extension.Equals(IFCXMLTEMPEXTENSION, StringComparison.InvariantCultureIgnoreCase))
                //    return IfcFileType.IfcXmlTemp;

                //if (extension.Equals(".ifczip", StringComparison.InvariantCultureIgnoreCase))
                //    return IfcFileType.IfcXmlZip;


                // default .ifcxcml
                return IfcFileType.IfcXml;
                //return IfcFileType.Undefined;
            }
        }

        internal bool Save()
        {
            bool result;
            switch (FileType)
            {
                case IfcFileType.Undefined:
                    result = false;
                    break;
                case IfcFileType.IfcXml:
                    result = SaveAs(FullName);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        internal bool SaveAs(string fullName)
        {
            if (String.IsNullOrEmpty(fullName))
            {
                Workspace.CurrentWorkspace.RaiseMessageLogged("Missing file name.");
                return false;
            }

            FullName = fullName;
            this.ResetDirty();

            BaseObject.EnableEvent(EventType.None, false);

            try
            {
                // NUR wenn vorher ein anderes Dokument offen war
                if (!FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase))
                {
                    // TODO
                }

                // ReadChecksum(FullName);

                this.IfcXmlDocument.Header.Name = System.IO.Path.GetFileNameWithoutExtension(fullName);

                //System.Runtime.Serialization.DataContractSerializer sr1 = new System.Runtime.Serialization.DataContractSerializer(typeof(Ifc4.ifcXML));
                //using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(FullName))
                //{
                //    sr1.WriteObject(writer, this.IfcXmlDocument);
                //    writer.Close();
                //}

                Ifc.NET.Serialization.IfcXmlSerializer ifcXmlSerializer = new Ifc.NET.Serialization.IfcXmlSerializer(typeof(Ifc.NET.IfcXML));
                ifcXmlSerializer.MessageLogged += ifcXmlSerializer_MessageLogged;

                Workspace.CurrentWorkspace.RaiseMessageLogged(String.Format("Save document '{0}'...", FullName));

                string xmlSerializedString = ifcXmlSerializer.Serialize(this.IfcXmlDocument);

                // TODOJV
                // Schrott Lösung
                xmlSerializedString = xmlSerializedString.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", " xmlns:ifc=\"" + IfcNs.NamespaceName + "\"");

                System.IO.File.WriteAllText(FullName, xmlSerializedString);

                //Correction(f0 + ".0");
                //validate = this.Validate(f0 + ".0");

                Workspace.CurrentWorkspace.RaiseMessageLogged(String.Format("Document '{0}' saved.", FullName));
                this.ResetDirty();
                Ifc.NET.Workspace.CurrentWorkspace.RaiseDocumentSaved(this, new Ifc.NET.Workspace.DocumentSavedEventArgs(this));

                ReadChecksum(FullName);

                return true;
            }
            catch (Exception exc)
            {
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
                return false;
            }
            finally
            {
                BaseObject.EnableEvent(EventType.All, true);
            }


        }

        internal void ifcXmlSerializer_MessageLogged(object sender, Ifc.NET.EventArgs.MessageLoggedEventArgs e)
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
            using (FileStream fileStream = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(fileStream);
                Checksum = BitConverter.ToString(checksum).Replace("-", String.Empty);
                return Checksum;
            }
        }

        public bool Validate(string fullName)
        {
            Ifc.NET.Validation validation = new Ifc.NET.Validation();
            validation.Info += validation_Info;

            bool schemaValidationResult = validation.Validate(fullName, true);
            // custom validation
            CustomValidation customValidation = new Ifc.NET.Document.CustomValidation(this.Document);
            bool customValidationResult = customValidation.Validate();

            return (schemaValidationResult && customValidationResult);
        }

        void validation_Info(object sender, EventArgs.MessageLoggedEventArgs e)
        {
            Workspace.CurrentWorkspace.RaiseMessageLogged(e.Message);
        }


        private Ifc.NET.EventArgs.MessageLoggedEventArgs m_MessageLoggedEventArgs;
        private void WriteLog(string s)
        {
            if (m_MessageLoggedEventArgs == null)
                m_MessageLoggedEventArgs = new Ifc.NET.EventArgs.MessageLoggedEventArgs();

            m_MessageLoggedEventArgs.AddMessage(s);
        }



        internal class CustomValidation
        {

            public CustomValidation(Ifc.NET.Document document)
            {
                Document = document;
            }

            private Ifc.NET.Document Document { get; set; }

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

                var originalIfcSites = Document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcSite>().ToList();
                var originalIfcBuildings = Document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcBuilding>().ToList();
                var originalIfcBuildingStoreys = Document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcBuildingStorey>().ToList();
                var originalIfcSpaces = Document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcSpace>().ToList();

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


            private void FacilitiesCounter(Ifc.NET.CcFacilities<CcFacility> facilities)
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

            private void ValidateFacilities(Ifc.NET.CcFacilities<CcFacility> facilities, StringBuilder sb)
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

            private void ValidatePropertySet(Ifc.NET.CcFacility facility, StringBuilder messages)
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
                        messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("[PropertySetTemplate] für Objekt ${0}$ hat keine Property Templates!", facility.TempId)));
                    else
                        propertyTemplates = ifcPropertySetTemplate.HasPropertyTemplates.Items;

                    if (ifcPropertySet.HasProperties == null)
                    {
                        messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("[PropertySet] für Objekt ${0}$ hat keine Properties!", facility.TempId)));
                        m_ErrorCount++;
                    }
                    else
                        properties = ifcPropertySet.HasProperties.Items;

                    if (propertyTemplates != null && properties != null)
                    {
                        foreach (var name in propertyTemplates.Select(item => item.Name).Except(properties.Select(item => item.Name)))
                        {
                            messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("Objekt ${0}$ - [PropertySet] Property '{1}' nicht in IfcPropertySet id='{2}' enthalten! ", facility.TempId, name, ifcPropertySet.Id)));
                            m_ErrorCount++;
                        }

                        foreach (var name in properties.Select(item => item.Name).Except(propertyTemplates.Select(item => item.Name)))
                        {
                            messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("Objekt ${0}$ -[PropertySetTemplate] Property '{1}' nicht in IfcPropertySetTemplate id='{2}' enthalten!", facility.TempId, name, ifcPropertySetTemplate.Id)));
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
                                        messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("IfcPropertySet id='{0}' Property '{1}' Enum Wert '{2}' ist ungültig! ", ifcSimplePropertyTemplate.Id, ifcSimplePropertyTemplate.Name, ifcLabelWrapper.Value)));
                                        m_ErrorCount++;
                                    }
                                }
                            }
                        }

                    }
                }

            }

            private Dictionary<string, List<string>> m_IfcSimplePropertyTemplateEnumerationValues = new Dictionary<string, List<string>>();
            private List<string> GetEnumerationValues(Ifc.NET.IfcSimplePropertyTemplate ifcSimplePropertyTemplate, StringBuilder messages)
            {
                if (ifcSimplePropertyTemplate == null)
                    return new List<string>();

                if (
                        ifcSimplePropertyTemplate.Enumerators == null ||
                        ifcSimplePropertyTemplate.Enumerators.EnumerationValues == null
                    )
                {
                    messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("IfcSimplePropertyTemplate id='{0}' Property '{1}' hat keine enum Werte! ", ifcSimplePropertyTemplate.Id, ifcSimplePropertyTemplate.Name)));
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

            private void ValidateAttributeWithValue(Ifc.NET.CcFacility facility, StringBuilder messages)
            {
                if (facility == null)
                    return;

                foreach (Ifc.NET.CustomModel.CustomPropertyDescriptor customPropertyDescriptor in System.ComponentModel.TypeDescriptor.GetProperties(facility).OfType<Ifc.NET.CustomModel.CustomPropertyDescriptor>())
                {
                    var value = customPropertyDescriptor.GetValue(facility);

                    if (customPropertyDescriptor.LastValueError != null)
                    {
                        messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("{0}", customPropertyDescriptor.ToString("I"))));
                        if (customPropertyDescriptor.LastValueError.InnerException != null)
                        {
                            messages.Append
                                (
                                    Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage
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
                    //if (value != null)
                    //    m_AttributesWithValueCounter++;
                }
            }
        }

    }

}


//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using CafmConnect.Core.Interfaces;
//using System.Security.Cryptography;


//namespace CafmConnect.Core.Cc
//{
//    [CcDisplayName("CCDOCUMENT_DISPLAYNAME")]
//    public class CcDocument : CcObject
//    {


////OwnerHistory
////Entities derived from IfcRoot which includes all spatial and building elements and
////relationships have a mandatory ‘OwnerHistory’ attribute. The information held within this
////attribute is extensive and allows the data author, the time, and the application etc to be
////associated to the entity.
////It is the responsibility of every application to create an IfcOwnerHistory when it adds to or
////modifies the model. The IfcOrganization and IfcApplication entities need be created
////only once.


//        public const string IFCXMLTEMPEXTENSION = ".~ifcxml";

//        private bool _WriteHeader;

//        internal CcDocument(ICcObject parent)
//            : base(parent)
//        {
//            Model = 0;
//            FullName = String.Empty;
//            _WriteHeader = false;
//        }

//        private bool _InitializeDefaultValues = false;
//        public void InitializeDefaultValues()
//        {
//            if (_InitializeDefaultValues)
//                return;

//            // read reference
//            Organizations = new CcOrganizations(this);

//            Persons = new CcPersons(this);

//            PersonAndOrganizations = new CcPersonAndOrganizations(this);

//            Applications = new CcApplications(this);

//            OwnerHistories = new CcOwnerHistories(this);

//            Classifications = new CcClassifications(this);

//            RelAggregatesCollection = new CcRelAggregatesCollection(this);

//            PropertySingleValues = new CcPropertySingleValues(this);

//            QuantityAreas = new CcQuantityAreas(this);

//            RelDefinesByPropertiesCollection = new CcRelDefinesByPropertiesCollection(this);

//            RelAssociatesClassifications = new CcRelAssociatesClassifications(this);

//            PostalAddresses = new CcPostalAddresses(this);

//            Project = new CcProject(this);

//            // new file
//            _WriteHeader = !this.HasFileName;
//            if (_WriteHeader)
//            {
//                CcOrganization organization = Organizations.AddNewOrganization();
//                organization.Name = "CAFM Ring e.V.";
//                organization.Description = "";

//                CcApplication application = Applications.AddNewApplication();
//                application.ApplicationDeveloper = organization;
//                application.ApplicationFullName = "CAFM-Connect-Editor";
//                try { application.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
//                catch { application.Version = "<unknown>"; }
//                application.ApplicationIdentifier = 1;
//            }

//        }

//        public override bool CanRemove
//        {
//            // nur abhängig von CcProject
//            get { return this.Project.CanRemove; }
//        }

//        public string TempFullName
//        {
//            get
//            {
//                if (FileType == IfcFileType.IfcXmlZip)
//                    return System.IO.Path.ChangeExtension(FullName, IFCXMLTEMPEXTENSION);
//                return FullName;
//            }
//        }



//        internal bool SaveAsXml(string fullName)
//        {
//            FullName = fullName;
//            this.Write();
//            try
//            {

//                IfcConnection.sdaiSaveModelAsXmlBN(Model, FullName);

//                // IntPtr pFullName = (IntPtr)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(FullName);
//                // string s = GetStringFromPointer(pFullName.ToInt32());
//                // IfcConnection.sdaiSaveModelAsXmlBNUnicode(Model, pFullName);
//                // System.Runtime.InteropServices.Marshal.FreeHGlobal(pFullName);

//                //\X\E4\X\F6\X\FC\X\C4\X\D6\X\DC
//                // äöüÄÖÜ
//                //ConvertFileToUTF8(fullName);

//                ReadChecksum(FullName);
//            }
//            catch (Exception exc)
//            {
//                Cc.CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
//            }

//            this.ResetDirty();


//            CcWorkspace.CurrentWorkspace.RaiseDocumentSaved(this, new CcWorkspace.DocumentSavedEventArgs(this));
//            return true;
//        }



//        public void ConvertFileToUTF8(string fullName)
//        {
//            string s = System.IO.File.ReadAllText(FullName);
//            s = this.SpecialCharacterToXmlCode(s);
//            System.IO.File.WriteAllText(FullName, s, Encoding.ASCII);
//        }


//        //public string html_encode(string s)
//        //{
//        //    this.SpecialCharacterToUnicode(s)
//        //    s = s.Replace("'", "&apos;");
//        //    s = s.Replace("Ö", "&#214;");
//        //    s = s.Replace("Ü", "&#220;");
//        //    s = s.Replace("ä", "&#228; ");
//        //    s = s.Replace("ö", "&#246; ");
//        //    s = s.Replace("ü", "&#252; ");
//        //    s = s.Replace("ß", "&#223; ");
//        //    return s;
//        //}


//        internal bool SaveAsExp(string fullName)
//        {
//            FullName = fullName;
//            this.Write();
//            try
//            {
//                IfcConnection.sdaiSaveModelBN(Model, FullName);
//                ReadChecksum(FullName);
//            }
//            catch (Exception exc)
//            {
//                Cc.CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
//            }

//            this.ResetDirty();

//            CcWorkspace.CurrentWorkspace.RaiseDocumentSaved(this, new CcWorkspace.DocumentSavedEventArgs(this));
//            return true;
//        }



//        //public bool WriteXml(out string exportXmlFullName)
//        //{
//        //    CcDataImportExport importExport = new CcDataImportExport(this);
//        //    importExport.Write();
//        //    exportXmlFullName = importExport.ExportXmlFullName;
//        //    return true;
//        //}

//        private void DeleteDecompressedFile()
//        {
//            if (FileType == IfcFileType.IfcXmlTemp || FileType == IfcFileType.IfcXmlZip)
//            {
//                try { System.IO.File.Delete(TempFullName); }
//                catch (Exception exc) { Cc.CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc); }
//            }
//        }

//        internal void Close()
//        {

//            DeleteDecompressedFile();

//            ((ICcObjects<CcDocument>)Documents).Remove(this);
//            Documents.ActiveDocument = null;

//            if (Model != 0)
//            {
//                IfcConnection.sdaiCloseModel(Model);
//                Model = 0;
//            }


//            CcWorkspace.CurrentWorkspace.RaiseDocumentClosed(this, new CcWorkspace.DocumentClosedEventArgs(this));
//        }

//        [System.ComponentModel.Browsable(false)]
//        public CcProject Project { get; internal set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcOrganizations Organizations { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcPersons Persons { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcPersonAndOrganizations PersonAndOrganizations { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcApplications Applications { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcOwnerHistories OwnerHistories { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcClassifications Classifications { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcRelAssociatesClassifications RelAssociatesClassifications { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcRelAggregatesCollection RelAggregatesCollection { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcPropertySingleValues PropertySingleValues { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcQuantityAreas QuantityAreas { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcRelDefinesByPropertiesCollection RelDefinesByPropertiesCollection { get; private set; }

//        //[System.ComponentModel.Browsable(false)]
//        //public CcPropertySets PropertySets { get; private set; }

//        [System.ComponentModel.Browsable(false)]
//        public CcPostalAddresses PostalAddresses { get; private set; }

//        public override bool Read()
//        {

//            System.Diagnostics.Stopwatch sw = new Stopwatch();
//            try
//            {
//                sw.Start();
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Read document '{0}' ...", this.FullName));

//                InitializeDefaultValues();

//                if (!this.Organizations.Read())
//                    return false;

//                if (!this.Persons.Read())
//                    return false;

//                if (!this.PersonAndOrganizations.Read())
//                    return false;

//                if (!this.Applications.Read())
//                    return false;

//                if (!this.OwnerHistories.Read())
//                    return false;

//                if (!this.Classifications.Read())
//                    return false;

//                if (!this.PostalAddresses.Read())
//                    return false;

//                if (!this.RelAggregatesCollection.Read())
//                    return false;

//                if (!this.RelAssociatesClassifications.Read())
//                    return false;

//                if (!this.PropertySingleValues.Read())
//                    return false;

//                if (!this.QuantityAreas.Read())
//                    return false;

//                if (!this.RelDefinesByPropertiesCollection.Read())
//                    return false;

//                if (!this.Project.Read())
//                    return false;

//                ReadChecksum(Document.FullName);

//                return true;

//            }
//            catch (Exception exc)
//            {
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
//                return false;
//            }
//            finally
//            {
//                sw.Stop();
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Read... End {0}ms", sw.ElapsedMilliseconds));
//            }
//        }

//        public bool HasFileName
//        {
//            get { return !String.IsNullOrEmpty(FullName); }
//        }

//        public override bool Write()
//        {

//            // http://www.buildingsmart-tech.org/implementation/ifc-implementation

//            System.Diagnostics.Stopwatch sw = new Stopwatch();
//            try
//            {
//                sw.Start();
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Write document '{0}' ...", this.FullName));

//                //Project muss immer zuerst geschrieben werden
//                if (!this.Project.Write(false))
//                    return false;

//                if (!this.Organizations.Write())
//                    return false;

//                if (!this.Persons.Write())
//                    return false;

//                if (!this.PersonAndOrganizations.Write())
//                    return false;

//                if (!this.Applications.Write())
//                    return false;

//                if (!this.OwnerHistories.Write())
//                    return false;

//                if (!this.Classifications.Write())
//                    return false;

//                if (!this.PostalAddresses.Write())
//                    return false;

//                // ---------------------------------------
//                // new
//                // update old ifcxml files
//                if (!this.RelAggregatesCollection.Write())
//                    return false;

//                if (!this.RelAssociatesClassifications.Write())
//                    return false;

//                if (!this.RelDefinesByPropertiesCollection.Write())
//                    return false;
//                // ---------------------------------------

//                if (!this.Project.Write())
//                    return false;


//                if(_WriteHeader)
//                {
//                    try
//                    {

//                        string version;
//                        try
//                        {
//                            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
//                            version = v.Major + "." + v.MajorRevision + "." + v.Revision;

//                        }
//                        catch {version = "<unknown>"; }

//                        string timeStamp = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

//                        // http://buildingsmart-tech.org/implementation/ifc-implementation/ifc-header




//                        // Validate
//                        // http://gtds.buildingsmart.com/
//                        // wenn dieser String fehlt, wird der Header als Fehler in QuickTest ausgegeben
//                        string viewDefinition = "ViewDefinition [CoordinationView_V2.0]";

//                        IfcConnection.SetSPFFHeader
//                            (
//                                Document.Model,
//                                "CAFM-Connect-Editor generated IFC file., " + version + " - " + viewDefinition, // description
//                                "2;1", // implementationLevel
//                                System.IO.Path.GetFileNameWithoutExtension(Document.FullName), // name
//                                timeStamp, // timeStamp,
//                                "http://www.cafmring.de", // Author
//                                "CAFM Ring e.V.", // organization
//                                "TNO Building and Construction Research", // preprocessorVersion
//                                "CAFM-Connect-Editor", // originatingSystem,
//                                "http://www.cafmring.de", // authorization,
//                                "IFC2X3" // fileSchema
//                            );

//                        _WriteHeader = false;

//                    }
//                    catch { }
//                }

//                return true;

//            }
//            catch (Exception exc)
//            {
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
//                return false;
//            }
//            finally
//            {
//                sw.Stop();
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Write... End. {0}ms", sw.ElapsedMilliseconds));
//            }

//        }

//        public override string ObjectText
//        {
//            get { return base.DisplayText; }
//        }

//        public bool CleanUp()
//        {

//            try
//            {
//                CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, String.Format("Clean up document '{0}' ...", this.FullName));

//                CcDocument newDocument = CcWorkspace.CurrentWorkspace.CreateDocument("", true);

//                newDocument.Project.GlobalId = Document.Project.GlobalId;
//                newDocument.Project.Name = Document.Project.Name;
//                newDocument.Project.LongName = Document.Project.LongName;
//                newDocument.Project.Description = Document.Project.Description;

//                foreach (CcPostalAddress postalAddress in Document.PostalAddresses)
//                {
//                    CcPostalAddress newPostalAddress = newDocument.PostalAddresses.AddNewPostalAddress();
//                    newPostalAddress.InternalLocation = postalAddress.InternalLocation;
//                    newPostalAddress.AddressLines = postalAddress.AddressLines;
//                    newPostalAddress.PostalBox = postalAddress.PostalBox;
//                    newPostalAddress.Town = postalAddress.Town;
//                    newPostalAddress.Region = postalAddress.Region;
//                    newPostalAddress.PostalCode = postalAddress.PostalCode;
//                    newPostalAddress.Country = postalAddress.Country;
//                }

//                foreach (CcSite site in this.Document.Project.Sites)
//                {
//                    CcSite newSite = newDocument.Project.Sites.AddNewSite();
//                    newSite.GlobalId = site.GlobalId;
//                    newSite.Name = site.Name;
//                    newSite.Description = site.Description;
//                    newSite.LongName = site.LongName;

//                    if (site.SiteAddress != null)
//                    {
//                        var postalAddress = (from pa in newDocument.PostalAddresses where pa.Equals(site.SiteAddress) select pa).FirstOrDefault();
//                        if (postalAddress == null)
//                        {
//                            CcPostalAddress newPostalAddress = newDocument.PostalAddresses.AddNewPostalAddress();

//                            newPostalAddress.InternalLocation = site.SiteAddress.InternalLocation;
//                            newPostalAddress.AddressLines = site.SiteAddress.AddressLines;
//                            newPostalAddress.PostalBox = site.SiteAddress.PostalBox;
//                            newPostalAddress.Town = site.SiteAddress.Town;
//                            newPostalAddress.Region = site.SiteAddress.Region;
//                            newPostalAddress.PostalCode = site.SiteAddress.PostalCode;
//                            newPostalAddress.Country = site.SiteAddress.Country;

//                            newSite.SiteAddress = newPostalAddress;
//                        }
//                        else
//                        {
//                            newSite.SiteAddress = postalAddress;
//                        }
//                    }

//                    foreach (CcBuilding building in site.Buildings)
//                    {
//                        CcBuilding newBuilding = newSite.Buildings.AddNewBuilding();
//                        newBuilding.GlobalId = building.GlobalId;

//                        newBuilding.Name = building.Name;
//                        newBuilding.Description = building.Description;
//                        newBuilding.LongName = building.LongName;
//                        newBuilding.YearOfConstruction = building.YearOfConstruction;
//                        newBuilding.NumberOfStoreys = building.NumberOfStoreys;
//                        newBuilding.GrossFloorArea = building.GrossFloorArea;
//                        newBuilding.NetFloorArea = building.NetFloorArea;

//                        if (building.BuildingAddress != null)
//                        {
//                            var postalAddress = (from pa in newDocument.PostalAddresses where pa.Equals(building.BuildingAddress) select pa).FirstOrDefault();
//                            if (postalAddress == null)
//                            {
//                                CcPostalAddress newPostalAddress = newDocument.PostalAddresses.AddNewPostalAddress();

//                                newPostalAddress.InternalLocation = building.BuildingAddress.InternalLocation;
//                                newPostalAddress.AddressLines = building.BuildingAddress.AddressLines;
//                                newPostalAddress.PostalBox = building.BuildingAddress.PostalBox;
//                                newPostalAddress.Town = building.BuildingAddress.Town;
//                                newPostalAddress.Region = building.BuildingAddress.Region;
//                                newPostalAddress.PostalCode = building.BuildingAddress.PostalCode;
//                                newPostalAddress.Country = building.BuildingAddress.Country;

//                                newBuilding.BuildingAddress = newPostalAddress;
//                            }
//                            else
//                            {
//                                newBuilding.BuildingAddress = postalAddress;
//                            }
//                        }

//                        foreach (CcBuildingStorey buildingStorey in building.BuildingStoreys)
//                        {
//                            CcBuildingStorey newBuildingStorey = newBuilding.BuildingStoreys.AddNewStorey();

//                            newBuildingStorey.GlobalId = buildingStorey.GlobalId;
//                            newBuildingStorey.Name = buildingStorey.Name;
//                            newBuildingStorey.LongName = buildingStorey.LongName;
//                            newBuildingStorey.GrossFloorArea = buildingStorey.GrossFloorArea;

//                            foreach (CcSpace space in buildingStorey.Spaces)
//                            {
//                                CcSpace newSpace = newBuildingStorey.Spaces.AddNewSpace();
//                                newSpace.GlobalId = space.GlobalId;
//                                newSpace.Name = space.Name;
//                                newSpace.LongName = space.LongName;
//                                newSpace.Height = space.Height;
//                                newSpace.NetFloorArea = space.NetFloorArea;


//                                if (space.ClassificationReference != null)
//                                {
//                                    CcClassificationReference classificationReference =
//                                        (from c in Document.Classifications.GetDefaultClassificationReferences
//                                         where c.Name == space.ClassificationReference.Name
//                                         select c).FirstOrDefault();
//                                    if (classificationReference != null)
//                                        newSpace.ClassificationReference = classificationReference;
//                                }

//                            }
//                        }
//                    }

//                }

//                //string fullName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Document.FullName), "COPY") + System.IO.Path.GetExtension(Document.FullName);
//                string fullName = Document.FullName;
//                newDocument.SaveAs(fullName);
//                CcWorkspace.CurrentWorkspace.CloseDocument();
//                // reopen
//                CcWorkspace.CurrentWorkspace.OpenDocument(fullName, CcWorkspace.SchemaExpFullName);
//                return true;

//            }
//            catch (Exception exc)
//            {
//                Cc.CcWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
//                return false;
//            }
//        }
//    }
//}
