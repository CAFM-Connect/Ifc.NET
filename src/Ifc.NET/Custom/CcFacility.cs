using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCFACILITY_DISPLAYNAME", "Anlage")]
    public partial class CcFacility : Ifc4.BaseObject
    {
        private Dictionary<string, object> m_DynamicProperties = new Dictionary<string, object>();

        //- Nummer: Textfeld
        //- Kennzeichen: Textfeld
        //- Bezeichung: Textfeld
        //- Ort: Auswahl aus Liegenschaftsexplorer (Ein Raum, eine Etage, 
        //          ein Gebäude oder eine Liegenschaft), Mindestangabe: Liegenschaft
        //- Objekttyp: Auswahl aus Objekttypenkatalog	

        public static Ifc4.CustomModel.CustomTypeDescriptionProvider CustomTypeDescriptionProvider { get; private set; }

        public CcFacility() : base()
        {
            if (CustomTypeDescriptionProvider == null)
            {
                CustomTypeDescriptionProvider = new Ifc4.CustomModel.CustomTypeDescriptionProvider(typeof(Ifc4.CcFacility));
                System.ComponentModel.TypeDescriptor.AddProvider(CustomTypeDescriptionProvider, typeof(Ifc4.CcFacility));
            }
        }

        public static void ResetVariables()
        {
            m_AllIfc4Types = null;
            m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems = null;
        }

        private CcFacilities<CcFacility> m_Facilities = null;
        [Browsable(false)]
        public CcFacilities<CcFacility> Facilities
        {
            get
            {
                if (m_Facilities == null)
                {
                    m_Facilities = new CcFacilities<CcFacility>(this);
                }
                return m_Facilities;
            }
        }

        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_IFCSYSTEM_DisplayName", "IfcSystem")]
        [Browsable(false)]
        public Ifc4.IfcSystem IfcSystem
        {
            get;
            internal set;
        }


        private Ifc4.IfcObjectDefinition m_IfcObjectDefinition;
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_IFCSOBJECTDEFINITION_DisplayName", "IfcObjectDefinition")]
        [Browsable(false)]
        public Ifc4.IfcObjectDefinition IfcObjectDefinition
        {
            get { return m_IfcObjectDefinition; }
            internal set
            {
                if (m_IfcObjectDefinition != value)
                {
                    m_IfcObjectDefinition = value;
                    InitializeAdditionalProperties();
                }
            }
        }


        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_NUMBER_DisplayName", "Nummer")]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_NUMBER_DisplayName", "Barcode")]
        public string Number
        {
            get
            {
                if (this.IfcObjectDefinition is Ifc4.IfcElement)
                {
                    return ((Ifc4.IfcElement)this.IfcObjectDefinition).Tag;
                }
                else if (this.IfcObjectDefinition is Ifc4.IfcProxy)
                {
                    return ((Ifc4.IfcProxy)this.IfcObjectDefinition).Tag;
                }
                else if (this.IfcObjectDefinition is Ifc4.IfcTypeProduct)
                {
                    return ((Ifc4.IfcTypeProduct)this.IfcObjectDefinition).Tag;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if (this.IfcObjectDefinition is Ifc4.IfcElement)
                {
                    ((Ifc4.IfcElement)this.IfcObjectDefinition).Tag = value;
                }
                else if (this.IfcObjectDefinition is Ifc4.IfcProxy)
                {
                    ((Ifc4.IfcProxy)this.IfcObjectDefinition).Tag = value;
                }
                else if (this.IfcObjectDefinition is Ifc4.IfcTypeProduct)
                {
                    ((Ifc4.IfcTypeProduct)this.IfcObjectDefinition).Tag = value;
                }
            }
        }

        // Nummer: Textfeld
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_KEY_DisplayName", "Kennzeichen")]
        public string Key
        {
            get
            {
                if (this.IfcObjectDefinition != null)
                {
                    return this.IfcObjectDefinition.Name;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if (this.IfcObjectDefinition != null)
                {
                    this.IfcObjectDefinition.Name = value;
                }

            }
        }

        // Kennzeichen: Textfeld
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_DESCRIPTION_DisplayName", "Bezeichnung")]
        public string Description
        {
            get
            {
                if (this.IfcObjectDefinition != null)
                {
                    return this.IfcObjectDefinition.Description;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if (this.IfcObjectDefinition != null)
                {
                    this.IfcObjectDefinition.Description = value;
                }

            }
        }

        // Ort: Auswahl aus Liegenschaftsexplorer (Ein Raum, eine Etage, ein Gebäude oder eine Liegenschaft), Mindestangabe: Liegenschaft
        private IfcSpatialStructureElement m_Location;
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_LOCATION_DisplayName", "Ort")]
        public IfcSpatialStructureElement Location
        {
            get
            {
                if (m_Location == null)
                    m_Location = this.GetIfcSpatialStructureElement();
                return m_Location;
            }
            set
            {
                if ((this.m_Location != value))
                {
                    this.RaisePropertyChanging("Location");

                    DeleteIfcRelContainedInSpatialStructure();

                    this.m_Location = value;

                    UpdateIfcRelContainedInSpatialStructure();

                    this.RaisePropertyChanged("Location");
                }
            }
        }


        private IfcClassificationReference m_IfcClassificationReference;
        [Browsable(false)]
        public IfcClassificationReference IfcClassificationReference
        {
            get
            {
                if (String.IsNullOrEmpty(m_ObjectTypeId))
                    return null;

                if (m_IfcClassificationReference == null)
                    m_IfcClassificationReference = this.Document.IfcXmlDocument.Items.FirstOrDefault(item => item.Id == m_ObjectTypeId) as Ifc4.IfcClassificationReference;

                return m_IfcClassificationReference;
            }
        }

        private string m_ObjectTypeId;
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCDFACILITY_PROPERTY_OBJECTTYPE_DisplayName", "Objekttyp")]
        public string ObjectTypeId
        {
            get
            {
                if (String.IsNullOrEmpty(m_ObjectTypeId))
                {
                    var ifcClassificationReference = this.GetIfcClassificationReferenceFromIfcPropertySetTemplate(
                                                        this.GetRelatingTemplateFromIfcPropertySetDefinition(
                                                            this.GetIfcPropertySetFromRelatingPropertyDefinition()
                                                        )
                                                    );

                    m_ObjectTypeId = ifcClassificationReference != null ? ifcClassificationReference.Id : null;
                    m_IfcClassificationReference = ifcClassificationReference;
                }
                return m_ObjectTypeId;
            }
            set
            {

                if (this.m_ObjectTypeId != value)
                {
                    this.RaisePropertyChanging("ObjectTypeId");

                    if (CustomTypeDescriptionProvider != null)
                        CustomTypeDescriptionProvider.Reset(this);

                    if (!String.IsNullOrWhiteSpace(value) && value.StartsWith("i"))
                    {
                        DeletePropertiesFromDocument(m_ObjectTypeId);

                        this.m_ObjectTypeId = value;
                        this.m_IfcClassificationReference = null;

                        SetRuntimePropertiesFromFacility();
                    }

                    this.RaisePropertyChanged("ObjectTypeId");
                }
            }
        }

        [System.ComponentModel.ReadOnly(true)]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCROOT_PROPERTY_UNIQUEIDS_DisplayName", "IFC GUID / GUID")]
        public string UniqueIds
        {
            get
            {
                if (this.IfcObjectDefinition is IfcRoot)
                    return ((IfcRoot)this.IfcObjectDefinition).UniqueIds;

                return String.Empty;
            }
        }

        public override bool CanRemove
        {
            get
            {
                //return (this.Facilities.Count() == 0);
                return true;
            }
        }

        public override bool Remove()
        {

            // ---------------------------------------
            // hierachy delete
            RemoveRecursive();
            // ---------------------------------------

            System.Diagnostics.Debug.WriteLine(String.Format("Remove {0} {1} {2}:", this.Key, this.Number, this.Description));

            DeleteIfcRelContainedInSpatialStructure();

            if (this.IfcSystem != null)
            {
                this.IfcSystem.Remove();
                if (this.Parent is System.Collections.IList)
                {
                    ((System.Collections.IList)this.Parent).Remove(this);
                }
            }
            if (this.IfcObjectDefinition != null)
            {
                DeletePropertiesFromDocument(this.m_ObjectTypeId);

                this.IfcObjectDefinition.Remove();
                if (this.Parent is System.Collections.IList)
                {
                    ((System.Collections.IList)this.Parent).Remove(this);
                }
            }
            return true;
        }

        private void RemoveRecursive()
        {
            while (true)
            {
                var removeFacilities = this.Facilities.Descendants().Where(item => !item.Facilities.Any()).ToList();
                if (!removeFacilities.Any())
                    break;

                foreach (var facility in removeFacilities)
                {
                    facility.Remove();
                }
            }
        }

        public IEnumerable<CcFacility> All()
        {
            List<CcFacility> facilities = new List<CcFacility>();
            facilities.Add(this);
            facilities.AddRange(this.Facilities.Descendants());
            return facilities;
        }

        public override bool AllowMoving
        {
            get { return true; }
        }

        //public override bool HasSibling(ObjectPosition objectPosition)
        //{
        //    if (this.Parent == null) // Root
        //        return false;

        //    CcFacilities<CcFacility> parentCollection = this.Parent as CcFacilities<CcFacility>;
        //    if (parentCollection == null)
        //        return false;

        //    if (objectPosition == ObjectPosition.Previous)
        //    {
        //        int index = parentCollection.IndexOf(this);
        //        return (index > 0);
        //    }
        //    else if (objectPosition == ObjectPosition.Next)
        //    {
        //        int index = parentCollection.IndexOf(this);
        //        return (index < parentCollection.Count() - 1);
        //    }

        //    return false;
        //}

        //public override BaseObject GetSibling(ObjectPosition objectPosition)
        //{
        //    CcFacilities<CcFacility> parentCollection = this.Parent as CcFacilities<CcFacility>;
        //    if (parentCollection == null)
        //        return null;

        //    switch (objectPosition)
        //    {
        //        case ObjectPosition.Next:
        //            return parentCollection[parentCollection.IndexOf(this) + 1];
        //        //case ObjectPosition.NextLevel:
        //        //    throw new NotImplementedException("You must override in inherited class!");
        //        //    break;
        //        case ObjectPosition.Previous:
        //            return parentCollection[parentCollection.IndexOf(this) - 1];
        //        //case ObjectPosition.PreviousLevel:
        //        //    throw new NotImplementedException("You must override in inherited class!");
        //        //    break;
        //        default:
        //            break;
        //    }
        //    return null;
        //}

        public override bool Reposition(ObjectPosition objectPosition)
        {
            if (this.Parent == null) // Root
                return false;

            CcFacilities<CcFacility> parentCollection = this.Parent as CcFacilities<CcFacility>;
            if (parentCollection == null)
                return false;

            Ifc4.CcFacility siblingeFacility = GetSibling(objectPosition) as CcFacility;

            if (siblingeFacility != null)
            {
                if (siblingeFacility.IfcSystem != null)
                {
                    if (parentCollection.Swap(siblingeFacility, this))
                    {
                        return Swap(siblingeFacility.IfcSystem, this.IfcSystem);
                    }

                }
                else if (siblingeFacility.IfcObjectDefinition != null)
                {
                    if (parentCollection.Swap(siblingeFacility, this))
                    {
                        return Swap(siblingeFacility.IfcObjectDefinition, this.IfcObjectDefinition);
                    }
                }
            }
            return false;
        }

        private bool Swap(Entity a, Entity b)
        {
            int indexA = this.Document.IfcXmlDocument.Items.IndexOf(a);
            int indexB = this.Document.IfcXmlDocument.Items.IndexOf(b);

            if (indexA == -1 || indexB == -1)
                return false;

            if (indexA == indexB)
                return false;

            if (indexA >= this.Document.IfcXmlDocument.Items.Count())
                return false;

            if (indexB >= this.Document.IfcXmlDocument.Items.Count())
                return false;

            Entity tmp = this.Document.IfcXmlDocument.Items[indexA];
            this.Document.IfcXmlDocument.Items[indexA] = this.Document.IfcXmlDocument.Items[indexB];
            this.Document.IfcXmlDocument.Items[indexB] = tmp;
            return true;
        }

        public override Type GetAddObjectType()
        {
            return typeof(Ifc4.CcFacility);
        }

        public override bool CanAdd
        {
            get { return true; }
        }

        public override bool Read(BaseObject baseObject)
        {
            InitializeAdditionalProperties();
            return true;
        }

        public void AssignPropertiesFromFacility(CcFacility facility, bool oneToOneCopy = false)
        {
            if (facility == null)
                return;

            this.ObjectTypeId = facility.ObjectTypeId;
            this.Location = facility.Location;


            if (oneToOneCopy)
            {
                this.Number = facility.Number;
                this.Key = facility.Key;
                this.Description = facility.Description;
            }
            else
            {
                this.Key = String.Empty;
            }

            this.InitializeAdditionalProperties();

            var propertyDescriptors = from propertyDescriptor in System.ComponentModel.TypeDescriptor.GetProperties(this).Cast<PropertyDescriptor>().OfType<Ifc4.CustomModel.CustomPropertyDescriptor>()
                                      select propertyDescriptor;

            var clonePropertyDescriptors = from clonePropertyDescriptor in System.ComponentModel.TypeDescriptor.GetProperties(facility).Cast<PropertyDescriptor>().OfType<Ifc4.CustomModel.CustomPropertyDescriptor>()
                                           select clonePropertyDescriptor;

            foreach (var propertyDescriptor in propertyDescriptors)
            {
                var clonePropertyDescriptor = clonePropertyDescriptors.FirstOrDefault(item => item.Name == propertyDescriptor.Name);
                this.SetValue(propertyDescriptor, propertyDescriptor.GetValue(facility));
            }

        }

        private Ifc4.IfcSpatialStructureElement GetIfcSpatialStructureElement()
        {
            //<IfcRelContainedInSpatialStructure>
            //  <RelatedElements>
            //    <IfcBuildingElementProxy ref="i2466" xsi:nil="true" />
            //  </RelatedElements>
            //  <RelatingStructure xsi:type="IfcBuilding" ref="i141" xsi:nil="true" />
            //</IfcRelContainedInSpatialStructure>

            if (this.IfcObjectDefinition == null)
                return null;

            // TODOJV - Nimmt nur den ersten!!!

            Ifc4.Document document = this.Document;
            Ifc4.IfcSpatialStructureElement ifcSpatialStructureElement = (from ifcRelContainedInSpatialStructure in document.IfcXmlDocument.Items.OfType<IfcRelContainedInSpatialStructure>()
                                                                                from relatedElements in ifcRelContainedInSpatialStructure.RelatedElements.Items
                                                                                where relatedElements.Ref == this.IfcObjectDefinition.Id
                                                                                select ifcRelContainedInSpatialStructure.RelatingStructure).FirstOrDefault() as Ifc4.IfcSpatialStructureElement;

            if (ifcSpatialStructureElement != null && ifcSpatialStructureElement.IsRef)
                ifcSpatialStructureElement = this.Document.IfcXmlDocument.Items.OfType<IfcSpatialStructureElement>().FirstOrDefault(item => item.Id == ifcSpatialStructureElement.Ref);

            return ifcSpatialStructureElement;

        }

        private static Dictionary<string, object> m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems;

        private Ifc4.IfcPropertySet GetIfcPropertySetFromRelatingPropertyDefinition()
        {

            this.IfcPropertySet = null;

            //<IfcRelDefinesByProperties id="i4419">
            //  <RelatedObjects xsi:type="IfcSpaceHeater" ref="i4409" xsi:nil="true" />
            //  <RelatingPropertyDefinition>
            //    <IfcPropertySet ref="i4418" xsi:nil="true" />
            //  </RelatingPropertyDefinition>
            //</IfcRelDefinesByProperties>

            if (this.IfcObjectDefinition == null)
                return null;

            Ifc4.Document document = this.Document;

            object ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem = null;
            if (document.IsInOpenProcess)
            {

                if (m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems == null)
                {
                    var qc = (from ifcRelDefinesByProperties in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelDefinesByProperties>()
                              where ifcRelDefinesByProperties.RelatedObjects != null &&
                                    ifcRelDefinesByProperties.RelatedObjects.Ref != null &&
                                    ifcRelDefinesByProperties.RelatingPropertyDefinition != null &&
                                    ifcRelDefinesByProperties.RelatingPropertyDefinition.Item != null
                              select ifcRelDefinesByProperties);

                    m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems = new Dictionary<string, object>();
                    foreach (var q in qc)
                    {
                        if (!m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems.ContainsKey(q.RelatedObjects.Ref))
                            m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems.Add(q.RelatedObjects.Ref, q.RelatingPropertyDefinition.Item);
                    }

                }
                m_IfcRelDefinesByPropertiesRelatingPropertyDefinitionItems.TryGetValue(this.IfcObjectDefinition.Id, out ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem);
            }
            else
            {
                ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem = (from ifcRelDefinesByProperties in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelDefinesByProperties>()
                                                                           where ifcRelDefinesByProperties.RelatedObjects != null &&
                                                                                 ifcRelDefinesByProperties.RelatedObjects.Ref == this.IfcObjectDefinition.Id &&
                                                                                 ifcRelDefinesByProperties.RelatingPropertyDefinition != null &&
                                                                                 ifcRelDefinesByProperties.RelatingPropertyDefinition.Item != null
                                                                           select ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).FirstOrDefault();
            }

            if (ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem == null)
                return null;

            IfcPropertySet ifcPropertySet = null;
            if (ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem is Ifc4.IfcPropertySet)
            {
                ifcPropertySet = ((IfcPropertySet)ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem);
            }
            else if (ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem is Ifc4.IfcPropertySetDefinitionSetwrapper)
            {
                IfcPropertySetDefinitionSetwrapper ifcPropertySetDefinitionSetwrapper = (IfcPropertySetDefinitionSetwrapper)ifcRelDefinesByPropertiesRelatingPropertyDefinitionItem;
                // ich kann nur ein IfcPropertySet bearbeiten
                ifcPropertySet = ifcPropertySetDefinitionSetwrapper.Items.OfType<IfcPropertySet>().FirstOrDefault();
            }

            if (ifcPropertySet != null && ifcPropertySet.IsRef)
            {
                if (document.IsInOpenProcess)
                {
                    Dictionary<string, Entity> entities;
                    if (document.Items.TryGetValue(typeof(IfcPropertySet).Name, out entities))
                    {
                        Entity entity;
                        if (entities.TryGetValue(ifcPropertySet.Ref, out entity))
                            ifcPropertySet = entity as IfcPropertySet;
                    }
                    if (ifcPropertySet == null)
                        ifcPropertySet = this.Document.IfcXmlDocument.Items.OfType<IfcPropertySet>().FirstOrDefault(item => item.Id == ifcPropertySet.Ref);
                }
                else
                {
                    ifcPropertySet = this.Document.IfcXmlDocument.Items.OfType<IfcPropertySet>().FirstOrDefault(item => item.Id == ifcPropertySet.Ref);
                }
            }

            this.IfcPropertySet = ifcPropertySet;

            return ifcPropertySet;
        }

        private Ifc4.IfcPropertySetTemplate GetRelatingTemplateFromIfcPropertySetDefinition(IfcPropertySet ifcPropertySet)
        {
            //<IfcRelDefinesByTemplate>
            //  <RelatedPropertySets>
            //    <IfcPropertySet ref="i4418" xsi:nil="true" />
            //  </RelatedPropertySets>
            //  <RelatingTemplate ref="i992" xsi:nil="true" />
            //</IfcRelDefinesByTemplate>

            if (ifcPropertySet == null)
                return null;

            Ifc4.Document document = ifcPropertySet.Document;
            Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate = (from ifcRelDefinesByTemplate in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelDefinesByTemplate>()
                                                                        from relatedPropertySet in ifcRelDefinesByTemplate.RelatedPropertySets.Items
                                                                        where relatedPropertySet.Ref == ifcPropertySet.Id
                                                                        select ifcRelDefinesByTemplate.RelatingTemplate).FirstOrDefault();

            if (ifcPropertySetTemplate != null && ifcPropertySetTemplate.IsRef)
                ifcPropertySetTemplate = this.Document.IfcXmlDocument.Items.OfType<IfcPropertySetTemplate>().FirstOrDefault(item => item.Id == ifcPropertySetTemplate.Ref);

            return ifcPropertySetTemplate;

        }

        private Ifc4.IfcClassificationReference GetIfcClassificationReferenceFromIfcPropertySetTemplate(IfcPropertySetTemplate ifcPropertySetTemplate)
        {
            //<IfcRelAssociatesClassification id="i999" GlobalId="09Yj_c95H5iPvgszfwsNL6" Name="423.17 - Heizkörper zu IfcClassificationReference">
            //  <RelatedObjects>
            //    <IfcPropertySetTemplate ref="i992" xsi:nil="true" />
            //  </RelatedObjects>
            //  <RelatingClassification>
            //    <IfcClassificationReference ref="i991" xsi:nil="true" />
            //  </RelatingClassification>
            //</IfcRelAssociatesClassification>

            if (ifcPropertySetTemplate == null)
                return null;

            Document document = ifcPropertySetTemplate.Document;
            IfcClassificationReference ifcClassificationReference = (from ifcRelAssociatesClassification in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesClassification>()
                                                                     from relatedObjects in ifcRelAssociatesClassification.RelatedObjects.Items
                                                                     where relatedObjects.Ref == ifcPropertySetTemplate.Id
                                                                     select ifcRelAssociatesClassification.RelatingClassification.Item).FirstOrDefault() as IfcClassificationReference;

            if (ifcClassificationReference != null && ifcClassificationReference.IsRef)
                ifcClassificationReference = this.Document.IfcXmlDocument.Items.OfType<IfcClassificationReference>().FirstOrDefault(item => item.Id == ifcClassificationReference.Ref);

            return ifcClassificationReference;

        }

        private List<IfcPropertySingleValue> m_IfcPropertySingleValueCollection = null;
        private List<IfcPropertyEnumeratedValue> m_IfcPropertyEnumeratedValueCollection = null;

        [Browsable(false)]
        public IfcPropertySet IfcPropertySet { get; private set; }

        [Browsable(false)]
        public IfcPropertySetTemplate IfcPropertySetTemplate { get; private set; }

        public void InitializeAdditionalProperties()
        {
            m_IfcPropertySingleValueCollection = new List<IfcPropertySingleValue>();
            m_IfcPropertyEnumeratedValueCollection = new List<IfcPropertyEnumeratedValue>();
            m_DynamicProperties = new Dictionary<string, object>();

            Ifc4.IfcPropertySet ifcPropertySet = this.GetIfcPropertySetFromRelatingPropertyDefinition();
            if (
                    ifcPropertySet == null ||
                    ifcPropertySet.HasProperties == null
                )
            {
                return;
            }

            //IEnumerable<IfcProperty> ifcPropertyCollection = this.Document.IfcXmlDocument.Items.OfType<IfcProperty>().ToList();
            IEnumerable<IfcProperty> ifcPropertyCollection = this.Document.IfcXmlDocument.Items.OfType<IfcProperty>();

            foreach (IfcProperty ifcPropertyTmp in ifcPropertySet.HasProperties.Items)
            {
                IfcProperty ifcProperty = null;
                if (ifcPropertyTmp.IsRef)
                {
                    ifcProperty = ifcPropertyCollection.FirstOrDefault(item => item.Id == ifcPropertyTmp.Ref);
                }
                else
                {
                    ifcProperty = ifcPropertyTmp;
                }

                if (ifcProperty != null && ifcProperty is IfcPropertySingleValue)
                {
                    IfcPropertySingleValue ifcPropertySingleValue = (IfcPropertySingleValue)ifcProperty;
                    m_IfcPropertySingleValueCollection.Add(ifcPropertySingleValue);

                    var nominalValue = (IfcPropertySingleValueNominalValue)ifcPropertySingleValue.NominalValue;
                    if (nominalValue != null && nominalValue.Item is IfcLabelwrapper)
                    {
                        InternalSetValue<string>(ifcPropertySingleValue.Name, ((IfcLabelwrapper)nominalValue.Item).Value, false);
                    }
                }
                else if (ifcProperty != null && ifcProperty is IfcPropertyEnumeratedValue)
                {
                    IfcPropertyEnumeratedValue ifcPropertyEnumeratedValue = (IfcPropertyEnumeratedValue)ifcProperty;
                    m_IfcPropertyEnumeratedValueCollection.Add(ifcPropertyEnumeratedValue);

                    var ifcPropertyEnumeratedValueEnumerationValues = (IfcPropertyEnumeratedValueEnumerationValues)ifcPropertyEnumeratedValue.EnumerationValues;
                    if (ifcPropertyEnumeratedValueEnumerationValues != null)
                    {
                        foreach (var enumeratedValue in ifcPropertyEnumeratedValueEnumerationValues.Items)
                        {
                            if (enumeratedValue is IfcLabelwrapper)
                            {
                                string value = ((IfcLabelwrapper)enumeratedValue).Value;
                                InternalSetValue<string>(ifcPropertyEnumeratedValue.Name, value, false);
                                break; // not flag
                            }
                        }
                    }
                }
            }

        }

        //private bool Compare<T>(T x, T y) where T : class
        //{
        //    return x == y;
        //}

        public T GetValue<T>(Ifc4.CustomModel.CustomPropertyDescriptor customPropertyDescriptor)
        {

            string key;
            if (customPropertyDescriptor.PropertyItem != null)
                key = customPropertyDescriptor.PropertyItem.DisplayName;
            else
                key = customPropertyDescriptor.DisplayName;

            if (customPropertyDescriptor.PropertyType.BaseType == typeof(System.Enum))
            {
                string uniqueKey = GetUniqueKey(key);
                object value;
                if (m_DynamicProperties.TryGetValue(uniqueKey, out value))
                {
                    if (value == null)
                        return default(T);

                    foreach (var enumValue in System.Enum.GetValues(typeof(T)))
                    {
                        if (enumValue.ToString() == value.ToString().Trim())
                            return (T)enumValue;
                    }
                    return default(T);

                    // wieso funktioniert dies nicht?
                    // return (T)System.Enum.Parse(customPropertyDescriptor.PropertyType, value.ToString().Trim());
                }
            }

            return InternalGetValue<T>(key);
        }

        private T InternalGetValue<T>(string key)
        {
            key = GetUniqueKey(key);

            Type conversionType = typeof(T);

            object value;
            if (m_DynamicProperties.TryGetValue(key, out value))
            {
                return (T)CustomChangeType<T>(value, conversionType);
            }
            return default(T);
        }

        private T CustomChangeType<T>(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null || value.ToString() == "")
                    return default(T);

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            ////special case
            //if (
            //        conversionType == typeof (DateTime) &&
            //        value.ToString().Length == 4 &&
            //        value.ToString().ToCharArray().All(item => Char.IsNumber(item))
            //    )
            //{
            //    String dateTimeFormat = "yyyy";
            //    value = DateTime.ParseExact(value.ToString(), dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
            //    return (T)value;
            //}

            try
            {
                return (T)Convert.ChangeType(value, conversionType);
            }
            catch (Exception exc)
            {
                if (exc is FormatException)
                {
                    string v = value == null ? "<null>" : value.ToString();
                    throw new FormatException(String.Format("Die Eingabezeichenfolge '{0}' hat das falsche Format.", v));
                }

                throw;
            }
        }

        public void SetValue<T>(Ifc4.CustomModel.CustomPropertyDescriptor customPropertyDescriptor, T t)
        {
            string key;

            if (customPropertyDescriptor.PropertyItem != null)
                key = customPropertyDescriptor.PropertyItem.DisplayName;
            else
                key = customPropertyDescriptor.DisplayName;

            InternalSetValue<T>(key, t);
        }

        private string GetUniqueKey(string key)
        {
            if (key == null)
                return null;

            return String.Join("", key.Select(c => ((int)c).ToString("X2")));
        }

        private void InternalSetValue<T>(string key, T t, bool updatePropertyValue = true)
        {
            string ifcName = key;
            key = GetUniqueKey(ifcName);

            object value;
            if (m_DynamicProperties.TryGetValue(key, out value))
            {
                if (t is System.Enum)
                {
                    object a = t.ToString();
                    object b = Convert.ChangeType(value, typeof(System.String));
                    if (a != null && b != null && a.GetHashCode() == b.GetHashCode())
                        return;
                }
                else
                {
                    try
                    {
                        T u = (T)CustomChangeType<T>(value, typeof(T));
                        object a = t;
                        object b = u;
                        if (a != null && b != null && a.GetHashCode() == b.GetHashCode())
                            return;
                    }
                    catch { /* kein Vergleich alter und neuer Wert möglich. Vielleicht falscher Datentyp. Einfach Überschreiben. */ }
                }

            }

            if (!m_DynamicProperties.ContainsKey(key))
                m_DynamicProperties.Add(key, t);
            else
                m_DynamicProperties[key] = t;


            if (updatePropertyValue)
            {
                // ISENUM
                if (t is System.Enum)
                {
                    IfcPropertyEnumeratedValue ifcPropertyEnumeratedValue = m_IfcPropertyEnumeratedValueCollection.FirstOrDefault(item => item.Name == ifcName);
                    if (ifcPropertyEnumeratedValue == null)
                    {
                        Ifc4.IfcPropertySet ifcPropertySet = this.GetIfcPropertySetFromRelatingPropertyDefinition();
                        if (ifcPropertySet != null)
                        {
                            ifcPropertyEnumeratedValue = new Ifc4.IfcPropertyEnumeratedValue()
                            {
                                Id = this.Document.GetNextSid(),
                                Name = ifcName, // propertyDescriptor.PropertyItem.IfcPropertyName,
                                // EnumerationReference =  aus dem IfcPropertySetTemplate holen
                                EnumerationValues = new IfcPropertyEnumeratedValueEnumerationValues()
                            };

                            ifcPropertyEnumeratedValue.EnumerationValues.Items.Add(new Ifc4.IfcLabelwrapper());

                            m_IfcPropertyEnumeratedValueCollection.Add(ifcPropertyEnumeratedValue);

                            if (ifcPropertySet.HasProperties == null)
                                ifcPropertySet.HasProperties = new Ifc4.IfcPropertySetHasProperties();

                            ifcPropertySet.HasProperties.Items.Add(ifcPropertyEnumeratedValue);
                        }
                    }

                    if (ifcPropertyEnumeratedValue != null)
                    {
                        var ifcPropertyEnumeratedValueEnumerationValues = ifcPropertyEnumeratedValue.EnumerationValues;
                        if (ifcPropertyEnumeratedValueEnumerationValues != null)
                        {
                            ifcPropertyEnumeratedValueEnumerationValues.Items.Clear();
                            IfcLabelwrapper ifcLabelwrapper = new IfcLabelwrapper();
                            ifcLabelwrapper.Value = t.ToString().Trim();
                            ifcPropertyEnumeratedValueEnumerationValues.Items.Add(ifcLabelwrapper);
                        }
                    }
                }
                else
                {
                    IfcPropertySingleValue ifcPropertySingleValue = m_IfcPropertySingleValueCollection.FirstOrDefault(item => item.Name == ifcName);

                    if (ifcPropertySingleValue == null)
                    {
                        Ifc4.IfcPropertySet ifcPropertySet = this.GetIfcPropertySetFromRelatingPropertyDefinition();
                        if (ifcPropertySet != null)
                        {
                            ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                            {
                                Id = this.Document.GetNextSid(),
                                Name = ifcName, // propertyDescriptor.PropertyItem.IfcPropertyName,
                                NominalValue = new Ifc4.IfcPropertySingleValueNominalValue(),
                                // Unit = "";
                            };

                            ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcLabelwrapper();

                            m_IfcPropertySingleValueCollection.Add(ifcPropertySingleValue);

                            if (ifcPropertySet.HasProperties == null)
                                ifcPropertySet.HasProperties = new Ifc4.IfcPropertySetHasProperties();

                            ifcPropertySet.HasProperties.Items.Add(ifcPropertySingleValue);
                        }

                    }

                    if (ifcPropertySingleValue != null)
                    {
                        var nominalValue = (IfcPropertySingleValueNominalValue)ifcPropertySingleValue.NominalValue;
                        if (nominalValue.Item is IfcLabelwrapper)
                        {

                            if (t is DateTime)
                            {
                                ((IfcLabelwrapper)nominalValue.Item).Value = t == null
                                    ? String.Empty
                                    : DateTime.Parse(t.ToString()).ToShortDateString();
                            }
                            else
                            {
                                ((IfcLabelwrapper)nominalValue.Item).Value = t == null ? String.Empty : t.ToString();
                            }
                        }
                    }
                }

            }

            this.Document.SetDirty();

        }

        //private void UpdateIfcObjectDefinitionProperty(string propertyName)
        //{

        //}

        public Ifc4.IfcRelAssignsToGroup GetIfcRelAssignsToGroup()
        {
            if (this.IfcSystem == null)
                return null;

            IfcRelAssignsToGroup ifcRelAssignsToGroup = this.Document.IfcXmlDocument.Items.OfType<IfcRelAssignsToGroup>()
                                                        .FirstOrDefault(item => item.RelatingGroup != null && item.RelatingGroup.Ref == this.IfcSystem.Id);

            if (ifcRelAssignsToGroup == null)
            {
                ifcRelAssignsToGroup = new IfcRelAssignsToGroup()
                {
                    Id = Document.GetNextSid(),
                    GlobalId = Document.GetNewGlobalId(),
                    RelatingGroup = new IfcSystem() { Ref = this.IfcSystem.Id }
                };

                ifcRelAssignsToGroup.RelatedObjects = new IfcRelAssignsRelatedObjects();
                this.Document.IfcXmlDocument.Items.Add(ifcRelAssignsToGroup);
            }
            return ifcRelAssignsToGroup;
        }

        public Ifc4.IfcRelAggregates GetIfcRelAggregates()
        {
            if (this.IfcObjectDefinition == null)
                return null;

            IfcRelAggregates ifcRelAggregates = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>()
                                                    .FirstOrDefault(item => item.RelatingObject != null && item.RelatingObject.Ref == this.IfcObjectDefinition.Id);


            if (ifcRelAggregates == null)
            {
                ifcRelAggregates = new IfcRelAggregates()
                {
                    Id = Document.GetNextSid(),
                    GlobalId = Document.GetNewGlobalId(),
                    RelatingObject = this.IfcObjectDefinition.RefInstance()
                };

                ifcRelAggregates.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                this.Document.IfcXmlDocument.Items.Add(ifcRelAggregates);
            }

            return ifcRelAggregates;
        }



        //<IfcRelAssociatesClassification id="i999" GlobalId="09Yj_c95H5iPvgszfwsNL6" Name="423.17 - Heizkörper zu IfcClassificationReference">
        //  <RelatedObjects>
        //    <IfcPropertySetTemplate ref="i992" xsi:nil="true" />
        //  </RelatedObjects>
        //  <RelatingClassification>
        //    <IfcClassificationReference ref="i991" xsi:nil="true" />
        //  </RelatingClassification>
        //</IfcRelAssociatesClassification>

        private static Type[] m_AllIfc4Types;
        private Type GetTypeFromApplicableEntity(string typeName)
        {
            if (String.IsNullOrEmpty(typeName))
                return null;

            if (m_AllIfc4Types == null)
                m_AllIfc4Types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            return m_AllIfc4Types.FirstOrDefault(item => item.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        }

        private void DeletePropertiesFromDocument(string oldObjectTypeId)
        {

            //<IfcRelDefinesByTemplate>
            //  <RelatedPropertySets>
            //    <IfcPropertySet ref="i3478" xsi:nil="true" />
            //  </RelatedPropertySets>
            //  <RelatingTemplate ref="i3134" xsi:nil="true" />
            //</IfcRelDefinesByTemplate>

            //<IfcPropertySet id="i3478">
            //  <HasProperties>
            //    <IfcPropertySingleValue id="i3477" Name="Beschreibung">
            //      <NominalValue>
            //        <IfcLabel-wrapper></IfcLabel-wrapper>
            //      </NominalValue>
            //    </IfcPropertySingleValue>
            //  </HasProperties>
            //</IfcPropertySet>

            //<IfcRelDefinesByProperties id="i3479">
            //  <RelatedObjects xsi:type="IfcProxy" ref="i3475" xsi:nil="true" />
            //  <RelatingPropertyDefinition>
            //    <IfcPropertySet ref="i3478" xsi:nil="true" />
            //  </RelatingPropertyDefinition>
            //</IfcRelDefinesByProperties>


            if (String.IsNullOrEmpty(oldObjectTypeId))
                return;

            Ifc4.Document document = this.Document;


            if (
                    this.IfcObjectDefinition != null &&
                    this.IfcPropertySet != null
                )
            {

                //var ifcRelDefinesByPropertiesArray = (from ifcRelDefinesByProperties in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelDefinesByProperties>()
                //                                      where ifcRelDefinesByProperties.RelatedObjects != null &&
                //                                              ifcRelDefinesByProperties.RelatedObjects.Ref == this.IfcObjectDefinition.Id &&
                //                                              ifcRelDefinesByProperties.RelatingPropertyDefinition != null &&
                //                                              ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcPropertySet &&
                //                                              ((IfcPropertySet)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).Ref == this.IfcPropertySet.Id
                //                                      select ifcRelDefinesByProperties).ToArray();

                var ifcRelDefinesByPropertiesArray = (from ifcRelDefinesByProperties in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelDefinesByProperties>()
                                                      where ifcRelDefinesByProperties.RelatedObjects != null &&
                                                              ifcRelDefinesByProperties.RelatedObjects.Ref == this.IfcObjectDefinition.Id &&
                                                              ifcRelDefinesByProperties.RelatingPropertyDefinition != null &&
                                                              (
                                                                (ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcPropertySet && ((IfcPropertySet)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).Ref == this.IfcPropertySet.Id) ||
                                                                (ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcPropertySetDefinitionSetwrapper &&
                                                                    ((IfcPropertySetDefinitionSetwrapper)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).Items.Count == 1 &&
                                                                    ((IfcPropertySetDefinitionSetwrapper)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).Items[0].Id == this.IfcPropertySet.Id
                                                                )
                                                              )
                                                      select ifcRelDefinesByProperties).ToArray();


                for (int i = 0; i < ifcRelDefinesByPropertiesArray.Length; i++)
                {
                    document.IfcXmlDocument.Items.Remove(ifcRelDefinesByPropertiesArray[i]);
                }

                IfcPropertySetTemplate ifcPropertySetTemplate = document.GetIfcPropertySetTemplateFromIfcClassificationReferenceId(oldObjectTypeId);
                if (ifcPropertySetTemplate != null)
                {
                    var ifcRelDefinesByTemplateArray = (from ifcRelDefinesByTemplate in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelDefinesByTemplate>()
                                                        where ifcRelDefinesByTemplate.RelatedPropertySets.Items != null &&
                                                                ifcRelDefinesByTemplate.RelatedPropertySets.Items.Exists(item => item.Ref == this.IfcPropertySet.Id) &&
                                                                ifcRelDefinesByTemplate.RelatingTemplate != null &&
                                                                ifcRelDefinesByTemplate.RelatingTemplate.Ref == ifcPropertySetTemplate.Id
                                                        select ifcRelDefinesByTemplate).ToArray();

                    for (int i = 0; i < ifcRelDefinesByTemplateArray.Length; i++)
                    {
                        for (int j = 0; j < ifcRelDefinesByTemplateArray[i].RelatedPropertySets.Items.ToArray().Length; j++)
                        {
                            if (ifcRelDefinesByTemplateArray[i].RelatedPropertySets.Items[j].Ref == this.IfcPropertySet.Id)
                                ifcRelDefinesByTemplateArray[i].RelatedPropertySets.Items.Remove(ifcRelDefinesByTemplateArray[i].RelatedPropertySets.Items[j]);
                        }

                        if (ifcRelDefinesByTemplateArray[i].RelatedPropertySets.Items.Count == 0)
                        {
                            document.IfcXmlDocument.Items.Remove(ifcRelDefinesByTemplateArray[i]);
                        }
                    }

                }

                document.IfcXmlDocument.Items.Remove(this.IfcPropertySet);

                this.IfcPropertySet = null;
            }

        }

        public Ifc4.IfcPropertySetTemplate GetIfcPropertySetTemplate()
        {
            IfcPropertySetTemplate = null;

            Ifc4.Document document = this.Document; //.GetParent<Ifc4.Document>();
            if (this.IfcObjectDefinition == null)
                return null;

            if (String.IsNullOrEmpty(this.ObjectTypeId))
                return null;

            //<IfcRelAssociatesClassification id="i151" GlobalId="2wbIr$UIr6ouLAaQwpD$$M" Name="334a.01 - Türe, handbetätigt zu IfcClassificationReference">
            //  <RelatedObjects>
            //    <IfcPropertySetTemplate ref="i128" xsi:nil="true" />
            //  </RelatedObjects>
            //  <RelatingClassification>
            //    <IfcClassificationReference ref="i127" xsi:nil="true" />
            //  </RelatingClassification>
            //</IfcRelAssociatesClassification>

            string id = this.ObjectTypeId;
            IfcPropertySetTemplate = document.GetIfcPropertySetTemplateFromIfcClassificationReferenceId(id);

            return IfcPropertySetTemplate;
        }

        private void SetRuntimePropertiesFromFacility()
        {
            Ifc4.Document document = this.GetParent<Ifc4.Document>();
            Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate = null;

            if (this.IfcObjectDefinition == null)
                return;

            if (!String.IsNullOrEmpty(this.ObjectTypeId))
            {

                //<IfcRelAssociatesClassification id="i151" GlobalId="2wbIr$UIr6ouLAaQwpD$$M" Name="334a.01 - Türe, handbetätigt zu IfcClassificationReference">
                //  <RelatedObjects>
                //    <IfcPropertySetTemplate ref="i128" xsi:nil="true" />
                //  </RelatedObjects>
                //  <RelatingClassification>
                //    <IfcClassificationReference ref="i127" xsi:nil="true" />
                //  </RelatingClassification>
                //</IfcRelAssociatesClassification>

                string id = this.ObjectTypeId;
                ifcPropertySetTemplate = document.GetIfcPropertySetTemplateFromIfcClassificationReferenceId(id);

            }
            if (ifcPropertySetTemplate == null)
                return;

            IfcPropertySetTemplate = ifcPropertySetTemplate;

            // change IfcBuildingElementProxy
            // to
            // selected ObjectType -> IfcClassificationReference -> IfcRelAssociatesClassification -> IfcPropertySetTemplate -> ApplicableEntity
            if (!String.IsNullOrEmpty(ifcPropertySetTemplate.ApplicableEntity))
            {
                string applicableEntity = ifcPropertySetTemplate.ApplicableEntity.Trim();
                try
                {
                    Type type = GetTypeFromApplicableEntity(applicableEntity);
                    if (type != null)
                    {
                        // change IfcObjectDefinition
                        if (this.IfcObjectDefinition.GetType().Name != type.Name)
                        {
                            string id = this.IfcObjectDefinition.Id;
                            string globalId = this.IfcObjectDefinition.GlobalId;
                            string tmpKey = this.Key;
                            string tmpNumber = this.Number;
                            string tmpDescription = this.Description;

                            document.IfcXmlDocument.Items.Remove(this.IfcObjectDefinition);

                            this.IfcObjectDefinition = Activator.CreateInstance(type) as IfcObjectDefinition;
                            if (this.IfcObjectDefinition != null)
                            {
                                this.IfcObjectDefinition.Id = id;
                                this.IfcObjectDefinition.GlobalId = globalId;
                                this.Key = tmpKey;
                                this.Number = tmpNumber;
                                this.Description = tmpDescription;

                                document.IfcXmlDocument.Items.Add(this.IfcObjectDefinition);

                                var parentFacility = this.GetParent<CcFacility>();
                                if (parentFacility != null)
                                {
                                    if (parentFacility.IfcSystem != null)
                                    {
                                        // change IfcRelAssignsToGroup
                                        var ifcRelAssignsToGroupCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAssignsToGroup>()
                                                                                            .Where(item => item.RelatedObjects != null && item.RelatedObjects.Items.Exists(item2 => item2.Ref == id)).ToList();

                                        foreach (var ifcRelAssignsToGroup in ifcRelAssignsToGroupCollection)
                                        {
                                            var ifcObjectDefinition = ifcRelAssignsToGroup.RelatedObjects.Items.FirstOrDefault(item => item.Ref == id);
                                            if (ifcObjectDefinition != null)
                                            {
                                                ifcRelAssignsToGroup.RelatedObjects.Items.Remove(ifcObjectDefinition);
                                                ifcRelAssignsToGroup.RelatedObjects.Items.Add(this.IfcObjectDefinition.RefInstance());
                                            }
                                        }

                                        // change IfcRelAggregates
                                        var ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == id).ToList();
                                        foreach (var ifcRelAggregates in ifcRelAggregatesCollection)
                                        {
                                            ifcRelAggregates.RelatingObject = this.IfcObjectDefinition.RefInstance();
                                        }

                                    }
                                    else
                                    {

                                        // change IfcRelAggregates
                                        var ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == id).ToList();
                                        foreach (var ifcRelAggregates in ifcRelAggregatesCollection)
                                        {
                                            ifcRelAggregates.RelatingObject = this.IfcObjectDefinition.RefInstance();
                                        }

                                        ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatedObjects != null && item.RelatedObjects.Items.Exists(item2 => item2.Ref == id)).ToList();
                                        foreach (var ifcRelAggregates in ifcRelAggregatesCollection)
                                        {
                                            var ifcObjectDefinition = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == id);
                                            if (ifcObjectDefinition != null)
                                            {
                                                ifcRelAggregates.RelatedObjects.Items.Remove(ifcObjectDefinition);
                                                ifcRelAggregates.RelatedObjects.Items.Add(this.IfcObjectDefinition.RefInstance());
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception exc)
                {
                }
            }

            IEnumerable<PropertyDescriptor> propertyDescriptors = null;
            if (!String.IsNullOrEmpty(ObjectTypeId))
            {
                propertyDescriptors = document.GetCustomPropertyDescriptorsFromIfcClassificationReferenceId(this, ObjectTypeId);
            }

            // PropertySet already exits in document ?
            IfcPropertySet ifcPropertySet = (this.IfcObjectDefinition != null) ? this.IfcObjectDefinition.GetIfcPropertySetFromRelatingPropertyDefinition() : null;
            if (ifcPropertySet == null)
            {
                ifcPropertySet = new Ifc4.IfcPropertySet();
                if (propertyDescriptors != null)
                {
                    foreach (var propertyDescriptor in propertyDescriptors.OfType<Ifc4.CustomModel.CustomPropertyDescriptor>())
                    {
                        if (propertyDescriptor.PropertyType.IsEnum)
                        {
                            Ifc4.IfcPropertyEnumeratedValue ifcPropertyEnumeratedValue = new Ifc4.IfcPropertyEnumeratedValue()
                            {
                                Id = document.GetNextSid(),
                                Name = propertyDescriptor.PropertyItem.IfcPropertyName,
                                // EnumerationReference =  aus dem IfcPropertySetTemplate holen
                                EnumerationValues = new IfcPropertyEnumeratedValueEnumerationValues()
                            };

                            object value = propertyDescriptor.GetValue(this);
                            ifcPropertyEnumeratedValue.EnumerationValues.Items.Add(new Ifc4.IfcLabelwrapper()
                            {
                                Value = value == null ? String.Empty : value.ToString().Trim()
                            });

                            if (ifcPropertySet.HasProperties == null)
                                ifcPropertySet.HasProperties = new Ifc4.IfcPropertySetHasProperties();

                            ifcPropertySet.HasProperties.Items.Add(ifcPropertyEnumeratedValue);

                        }
                        //else if (propertyDescriptor.PropertyType == typeof(System.String))
                        //{
                        //    Ifc4.IfcPropertySingleValue ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                        //    {
                        //        Id = document.GetNextSid(),
                        //        Name = propertyDescriptor.PropertyItem.IfcPropertyName,
                        //        NominalValue = new Ifc4.IfcPropertySingleValueNominalValue()
                        //        // Unit = "";
                        //    };

                        //    object value = propertyDescriptor.GetValue(this);
                        //    ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcLabelwrapper()
                        //    {
                        //        Value = value == null ? String.Empty : value.ToString()
                        //    };

                        //    if (ifcPropertySet.HasProperties == null)
                        //        ifcPropertySet.HasProperties = new Ifc4.IfcPropertySetHasProperties();

                        //    ifcPropertySet.HasProperties.Items.Add(ifcPropertySingleValue);
                        //}
                        else
                        {
                            Ifc4.IfcPropertySingleValue ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                            {
                                Id = document.GetNextSid(),
                                Name = propertyDescriptor.PropertyItem.IfcPropertyName,
                                NominalValue = new Ifc4.IfcPropertySingleValueNominalValue()
                                // Unit = "";
                            };

                            object value = propertyDescriptor.GetValue(this);
                            ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcLabelwrapper()
                            {
                                Value = value == null ? String.Empty : value.ToString()
                            };

                            if (ifcPropertySet.HasProperties == null)
                                ifcPropertySet.HasProperties = new Ifc4.IfcPropertySetHasProperties();

                            ifcPropertySet.HasProperties.Items.Add(ifcPropertySingleValue);
                        }

                    }
                }
            }

            IfcPropertySet = ifcPropertySet;


            if (ifcPropertySet.HasProperties != null)
            {
                ifcPropertySet.Id = document.GetNextSid();
                document.IfcXmlDocument.Items.Add(ifcPropertySet);

                Ifc4.IfcRelDefinesByProperties ifcRelDefinesByProperties = new Ifc4.IfcRelDefinesByProperties();
                ifcRelDefinesByProperties.Id = document.GetNextSid();

                ifcRelDefinesByProperties.RelatedObjects = this.IfcObjectDefinition.RefInstance();
                ifcRelDefinesByProperties.RelatingPropertyDefinition = new Ifc4.IfcRelDefinesByPropertiesRelatingPropertyDefinition();
                ifcRelDefinesByProperties.RelatingPropertyDefinition.Item = new Ifc4.IfcPropertySet() { Ref = ifcPropertySet.Id };

                document.IfcXmlDocument.Items.Add(ifcRelDefinesByProperties);


                Ifc4.IfcRelDefinesByTemplate ifcRelDefinesByTemplate = new Ifc4.IfcRelDefinesByTemplate();
                ifcRelDefinesByTemplate.RelatingTemplate = new Ifc4.IfcPropertySetTemplate() { Ref = ifcPropertySetTemplate.Id };
                ifcRelDefinesByTemplate.RelatedPropertySets = new Ifc4.IfcRelDefinesByTemplateRelatedPropertySets();
                ifcRelDefinesByTemplate.RelatedPropertySets.Items.Add(new Ifc4.IfcPropertySet() { Ref = ifcPropertySet.Id });
                document.IfcXmlDocument.Items.Add(ifcRelDefinesByTemplate);

                UpdateIfcRelContainedInSpatialStructure();

            }
        }

        private void DeleteIfcRelContainedInSpatialStructure()
        {
            Document document = this.Document;
            IfcSpatialStructureElement ifcSpatialStructureElement = this.Location as IfcSpatialStructureElement;
            if (ifcSpatialStructureElement == null)
                return;

            var ifcRelContainedInSpatialStructureCollection = from ifcRelContainedInSpatialStructure in document.IfcXmlDocument.Items.OfType<IfcRelContainedInSpatialStructure>()
                                                              where ifcRelContainedInSpatialStructure.RelatingStructure != null && ifcRelContainedInSpatialStructure.RelatingStructure.Ref == ifcSpatialStructureElement.Id
                                                              select ifcRelContainedInSpatialStructure;

            List<IfcRelContainedInSpatialStructure> removeIfcRelContainedInSpatialStructureCollection = new List<IfcRelContainedInSpatialStructure>();
            foreach (var ifcRelContainedInSpatialStructure in ifcRelContainedInSpatialStructureCollection)
            {
                if (ifcRelContainedInSpatialStructure.RelatedElements != null)
                {
                    ifcRelContainedInSpatialStructure.RelatedElements.Items.RemoveAll(item => item.Ref == this.IfcObjectDefinition.Id);
                    if (ifcRelContainedInSpatialStructure.RelatedElements.Items.Count == 0)
                        removeIfcRelContainedInSpatialStructureCollection.Add(ifcRelContainedInSpatialStructure);
                }
            }
            foreach (var ifcRelContainedInSpatialStructure in removeIfcRelContainedInSpatialStructureCollection)
            {
                document.IfcXmlDocument.Items.Remove(ifcRelContainedInSpatialStructure);
            }
        }

        private void UpdateIfcRelContainedInSpatialStructure()
        {
            Document document = this.Document;

            IfcSpatialStructureElement ifcSpatialStructureElement = this.Location as IfcSpatialStructureElement;

            IfcSpatialStructureElement relatingStructure = null;
            if (
                    ifcSpatialStructureElement is IfcSite ||
                    ifcSpatialStructureElement is IfcBuilding ||
                    ifcSpatialStructureElement is IfcBuildingStorey ||
                    ifcSpatialStructureElement is IfcSpace)
            {
                relatingStructure = ifcSpatialStructureElement.RefInstance() as IfcSpatialStructureElement;
            }
            else
            {
                if (document.Project.Sites.Count() > 0)
                {
                    relatingStructure = document.Project.Sites[0] as IfcSpatialStructureElement;
                    relatingStructure = relatingStructure.RefInstance() as IfcSpatialStructureElement;
                }
            }

            if (relatingStructure != null)
            {
                //  <IfcRelContainedInSpatialStructure>
                //    <RelatedElements>
                //      <IfcProxy ref="i132" xsi:nil="true" />
                //      <IfcProxy ref="i165" xsi:nil="true" />
                //    </RelatedElements>
                //    <RelatingStructure xsi:type="IfcSite" ref="i130" xsi:nil="true" />
                //  </IfcRelContainedInSpatialStructure>

                IEnumerable<IfcRelContainedInSpatialStructure> ifcRelContainedInSpatialStructureCollection = from ifcRelContainedInSpatialStructure in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelContainedInSpatialStructure>()
                                                                                                             where ifcRelContainedInSpatialStructure.RelatingStructure != null && ifcRelContainedInSpatialStructure.RelatingStructure.Ref == relatingStructure.Ref
                                                                                                             select ifcRelContainedInSpatialStructure;

                if (ifcRelContainedInSpatialStructureCollection.Any())
                {
                    foreach (var ifcRelContainedInSpatialStructure in ifcRelContainedInSpatialStructureCollection)
                    {
                        // update
                        if (ifcRelContainedInSpatialStructure.RelatedElements == null)
                            ifcRelContainedInSpatialStructure.RelatedElements = new IfcRelContainedInSpatialStructureRelatedElements();

                        if (!ifcRelContainedInSpatialStructure.RelatedElements.Items.Exists(item => item.Ref == this.IfcObjectDefinition.Id))
                            ifcRelContainedInSpatialStructure.RelatedElements.Items.Add((IfcProduct)this.IfcObjectDefinition.RefInstance());
                    }
                }
                else
                {
                    // add
                    Ifc4.IfcRelContainedInSpatialStructure ifcRelContainedInSpatialStructure = new Ifc4.IfcRelContainedInSpatialStructure();
                    ifcRelContainedInSpatialStructure.RelatingStructure = relatingStructure;
                    ifcRelContainedInSpatialStructure.RelatedElements = new Ifc4.IfcRelContainedInSpatialStructureRelatedElements();
                    ifcRelContainedInSpatialStructure.RelatedElements.Items.Add((IfcProduct)this.IfcObjectDefinition.RefInstance());
                    document.IfcXmlDocument.Items.Add(ifcRelContainedInSpatialStructure);
                }
            }

        }

    }
}
