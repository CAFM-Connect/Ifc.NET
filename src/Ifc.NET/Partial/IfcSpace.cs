using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcSpace
    {
        public override bool CanRemove
        {
            get
            {
                return true;
            }
        }

        // Nutzungsart
        public override bool Read(BaseObject baseObject)
        {
            InitializeAdditionalProperties();
            return true;
        }

        [System.Xml.Serialization.XmlIgnore]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_NAME_DisplayName", "Raumnummer")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        private IfcClassificationReference m_IfcClassificationReference;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_IFCCLASSIFICATIONREFERENCE_DISPLAYNAME", "Nutzungsart")]
        public IfcClassificationReference IfcClassificationReference
        {
            get
            {
                if (m_IfcClassificationReference == null)
                    m_IfcClassificationReference = this.GetIfcClassificationReferenceFromIfcRelAssociatesClassification();
                return m_IfcClassificationReference;
            }
            set
            {
                if (m_IfcClassificationReference != value)
                {

                    DeleteIfcRelAssociatesClassification();

                    m_IfcClassificationReference = value;
                    string propertyName = this.PropertyName(() => this.IfcClassificationReference);

                    UpdateIfcRelAssociatesClassification(propertyName, m_IfcClassificationReference);

                    RaisePropertyChanged(propertyName);
                }
            }
        }

        private double _Height;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_HEIGHT_DISPLAYNAME", "Height")]
        public double Height // [Qto_SpaceBaseQuantities]")]
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    string propertyName = this.PropertyName(() => this.Height);
                    UpdateIfcQuantityLength(propertyName, Height);
                    RaisePropertyChanged(this.PropertyName(() => this.Height));
                }
            }
        }

        private double _FinishCeilingHeight;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_FINISHCEILINGHEIGHT_DISPLAYNAME", "FinishCeilingHeight")]
        public double FinishCeilingHeight // [Qto_SpaceBaseQuantities]")]
        {
            get { return _FinishCeilingHeight; }
            set
            {
                if (_FinishCeilingHeight != value)
                {
                    _FinishCeilingHeight = value;
                    string propertyName = this.PropertyName(() => this._FinishCeilingHeight);
                    UpdateIfcQuantityLength(propertyName, _FinishCeilingHeight);
                    RaisePropertyChanged(this.PropertyName(() => this._FinishCeilingHeight));
                }
            }
        }

        private double _FinishFloorHeight;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_FINISHFLOORHEIGHT_DISPLAYNAME", "FinishFloorHeight")]
        public double FinishFloorHeight // [Qto_SpaceBaseQuantities]")]
        {
            get { return _FinishFloorHeight; }
            set
            {
                if (_FinishFloorHeight != value)
                {
                    _FinishFloorHeight = value;
                    string propertyName = this.PropertyName(() => this.FinishFloorHeight);
                    UpdateIfcQuantityLength(propertyName, FinishFloorHeight);
                    RaisePropertyChanged(this.PropertyName(() => this.FinishFloorHeight));
                }
            }
        }

        private double _GrossPerimeter;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_GROSSPERIMETER_DISPLAYNAME", "GrossPerimeter")]
        public double GrossPerimeter // [Qto_SpaceBaseQuantities]")]
        {
            get { return _GrossPerimeter; }
            set
            {
                if (_GrossPerimeter != value)
                {
                    _GrossPerimeter = value;
                    string propertyName = this.PropertyName(() => this.GrossPerimeter);
                    UpdateIfcQuantityLength(propertyName, GrossPerimeter);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossPerimeter));
                }
            }
        }

        private double _NetPerimeter;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_NETPERIMETER_DISPLAYNAME", "NetPerimeter")]
        public double NetPerimeter // [Qto_SpaceBaseQuantities]")]
        {
            get { return _NetPerimeter; }
            set
            {
                if (_NetPerimeter != value)
                {
                    _NetPerimeter = value;
                    string propertyName = this.PropertyName(() => this.NetPerimeter);
                    UpdateIfcQuantityLength(propertyName, NetPerimeter);
                    RaisePropertyChanged(this.PropertyName(() => this.NetPerimeter));
                }
            }
        }

        private double _GrossFloorArea;
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_GROSSFLOORAREA_DISPLAYNAME", "Bruttogrundfläche [m²]")]
        public double GrossFloorArea // [Qto_SpaceBaseQuantities]")]
        {
            get { return _GrossFloorArea; }
            set
            {
                if (_GrossFloorArea != value)
                {
                    _GrossFloorArea = value;
                    string propertyName = this.PropertyName(() => this.GrossFloorArea);
                    UpdateIfcQuantityArea(propertyName, GrossFloorArea);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossFloorArea));
                }
            }
        }

        private double _NetFloorArea;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_NETFLOORAREA_DISPLAYNAME", "Nettogrundfläche [m²]")]
        public double NetFloorArea // [Qto_SpaceBaseQuantities]")]
        {
            get { return _NetFloorArea; }
            set
            {
                if (_NetFloorArea != value)
                {
                    _NetFloorArea = value;
                    string propertyName = this.PropertyName(() => this.NetFloorArea);
                    UpdateIfcQuantityArea(propertyName, NetFloorArea);
                    RaisePropertyChanged(this.PropertyName(() => this.NetFloorArea));
                }
            }
        }

        private double _GrossWallArea;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_GROSSWALLAREA_DISPLAYNAME", "GrossWallArea")]
        public double GrossWallArea // [Qto_SpaceBaseQuantities]")]
        {
            get { return _GrossWallArea; }
            set
            {
                if (_GrossWallArea != value)
                {
                    _GrossWallArea = value;
                    string propertyName = this.PropertyName(() => this.GrossWallArea);
                    UpdateIfcQuantityArea(propertyName, GrossWallArea);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossWallArea));
                }
            }
        }

        private double _NetWallArea;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_NETWALLAREA_DISPLAYNAME", "NetWallArea")]
        public double NetWallArea // [Qto_SpaceBaseQuantities]")]
        {
            get { return _NetWallArea; }
            set
            {
                if (_NetWallArea != value)
                {
                    _NetWallArea = value;
                    string propertyName = this.PropertyName(() => this.NetWallArea);
                    UpdateIfcQuantityArea(propertyName, NetWallArea);
                    RaisePropertyChanged(this.PropertyName(() => this.NetWallArea));
                }
            }
        }

        private double _GrossCeilingArea;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_GROSSCEILINGAREA_DISPLAYNAME", "GrossCeilingArea")]
        public double GrossCeilingArea // [Qto_SpaceBaseQuantities]")]
        {
            get { return _GrossCeilingArea; }
            set
            {
                if (_GrossCeilingArea != value)
                {
                    _GrossCeilingArea = value;
                    string propertyName = this.PropertyName(() => this.GrossCeilingArea);
                    UpdateIfcQuantityArea(propertyName, GrossCeilingArea);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossCeilingArea));
                }
            }
        }

        private double _NetCeilingArea;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_NETCEILINGAREA_DISPLAYNAME", "NetCeilingArea")]
        public double NetCeilingArea // [Qto_SpaceBaseQuantities]")]
        {
            get { return _NetCeilingArea; }
            set
            {
                if (_NetCeilingArea != value)
                {
                    _NetCeilingArea = value;
                    string propertyName = this.PropertyName(() => this.NetCeilingArea);
                    UpdateIfcQuantityArea(propertyName, NetCeilingArea);
                    RaisePropertyChanged(this.PropertyName(() => this.NetCeilingArea));
                }
            }
        }

        private double _GrossVolume;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_GROSSVOLUME_DISPLAYNAME", "GrossVolume")]
        public double GrossVolume // [Qto_BuildingBaseQuantities]")]
        {
            get { return _GrossVolume; }
            set
            {
                if (_GrossVolume != value)
                {
                    _GrossVolume = value;
                    string propertyName = this.PropertyName(() => this.GrossVolume);
                    UpdateIfcQuantityVolume(propertyName, GrossVolume);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossVolume));
                }
            }
        }
        private double _NetVolume;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACE_PROPERTY_NETVOLUME_DISPLAYNAME", "NetVolume")]
        public double NetVolume // [Qto_BuildingBaseQuantities]")]
        {
            get { return _NetVolume; }
            set
            {
                if (_NetVolume != value)
                {
                    _NetVolume = value;
                    string propertyName = this.PropertyName(() => this.NetVolume);
                    UpdateIfcQuantityVolume(propertyName, NetVolume);
                    RaisePropertyChanged(this.PropertyName(() => this.NetVolume));
                }
            }
        }

        private void UpdateIfcQuantityLength(string propertyName, double value)
        {
            if (m_IfcQuantityLengthCollection != null)
            {
                var ifcElementQuantity = m_IfcQuantityLengthCollection.FirstOrDefault(item => item.Name == propertyName);
                if (ifcElementQuantity != null)
                    ifcElementQuantity.LengthValue = value;
            }
        }
        private void UpdateIfcQuantityArea(string propertyName, double value)
        {
            if (m_IfcQuantityAreaCollection != null)
            {
                var ifcElementQuantity = m_IfcQuantityAreaCollection.FirstOrDefault(item => item.Name == propertyName);
                if (ifcElementQuantity != null)
                    ifcElementQuantity.AreaValue = value;
            }
        }
        private void UpdateIfcQuantityVolume(string propertyName, double value)
        {
            if (m_IfcQuantityVolumeCollection != null)
            {
                var ifcElementQuantity = m_IfcQuantityVolumeCollection.FirstOrDefault(item => item.Name == propertyName);
                if (ifcElementQuantity != null)
                    ifcElementQuantity.VolumeValue = value;
            }
        }

        private List<IfcQuantityArea> m_IfcQuantityAreaCollection;
        private List<IfcQuantityLength> m_IfcQuantityLengthCollection;
        private List<IfcQuantityVolume> m_IfcQuantityVolumeCollection;

        public void InitializeAdditionalProperties()
        {
            Ifc4.IfcElementQuantity ifcElementQuantity = this.GetIfcElementQuantityFromRelatingPropertyDefinition();
            IEnumerable<IfcQuantityLength> ifcQuantityLengthCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityLength>().ToList();
            IEnumerable<IfcQuantityArea> ifcQuantityAreaCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityArea>().ToList();
            IEnumerable<IfcQuantityVolume> ifcQuantityVolumeCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityVolume>().ToList();

            if (ifcElementQuantity == null)
            {
                ifcElementQuantity = new IfcElementQuantity();
            }

            if (ifcElementQuantity.Quantities == null)
            {
                ifcElementQuantity.Quantities = new IfcElementQuantityQuantities();
            }

            // ---------------------------------------------------------------------------------------
            m_IfcQuantityLengthCollection = new List<IfcQuantityLength>();
            m_IfcQuantityAreaCollection = new List<IfcQuantityArea>();
            m_IfcQuantityVolumeCollection = new List<IfcQuantityVolume>();

            foreach (IfcPhysicalQuantity ifcPhysicalQuantityTmp in ifcElementQuantity.Quantities.Items)
            {
                if (ifcPhysicalQuantityTmp.IsRef && ifcPhysicalQuantityTmp is IfcQuantityLength)
                {
                    IfcQuantityLength existingIfcQuantityLength = ifcQuantityLengthCollection.FirstOrDefault(item => item.Id == ifcPhysicalQuantityTmp.Ref);
                    if (existingIfcQuantityLength != null)
                    {
                        m_IfcQuantityLengthCollection.Add(existingIfcQuantityLength);
                    }
                }
                else if (ifcPhysicalQuantityTmp.IsRef && ifcPhysicalQuantityTmp is IfcQuantityArea)
                {
                    IfcQuantityArea existingIfcQuantityArea = ifcQuantityAreaCollection.FirstOrDefault(item => item.Id == ifcPhysicalQuantityTmp.Ref);
                    if (existingIfcQuantityArea != null)
                    {
                        m_IfcQuantityAreaCollection.Add(existingIfcQuantityArea);
                    }
                }
                else if (ifcPhysicalQuantityTmp.IsRef && ifcPhysicalQuantityTmp is IfcQuantityVolume)
                {
                    IfcQuantityVolume existingIfcQuantityVolume = ifcQuantityVolumeCollection.FirstOrDefault(item => item.Id == ifcPhysicalQuantityTmp.Ref);
                    if (existingIfcQuantityVolume != null)
                    {
                        m_IfcQuantityVolumeCollection.Add(existingIfcQuantityVolume);
                    }
                }
            }
            // ---------------------------------------------------------------------------------------
            string[] ifcQuantityLengthNames = new string[]
            {
                nameof(Height),
                nameof(FinishCeilingHeight),
                nameof(FinishFloorHeight),
                nameof(GrossPerimeter),
                nameof(NetPerimeter)
            };
            foreach (var ifcQuantityLengthName in ifcQuantityLengthNames)
            {
                var ifcQuantityLengthPropertyInfo = this.GetType().GetProperty(ifcQuantityLengthName);
                if (ifcQuantityLengthPropertyInfo == null)
                    continue;

                var ifcQuantityLength = m_IfcQuantityLengthCollection.FirstOrDefault(item => item.Name.Equals(ifcQuantityLengthName, StringComparison.OrdinalIgnoreCase));
                if (ifcQuantityLength == null)
                {
                    ifcQuantityLength = new IfcQuantityLength()
                    {
                        Id = this.Document.GetNextSid(),
                        Name = ifcQuantityLengthName,
                        LengthValue = (double)ifcQuantityLengthPropertyInfo.GetValue(this, null)
                    };
                    m_IfcQuantityLengthCollection.Add(ifcQuantityLength);
                    this.Document.IfcXmlDocument.Items.Add(ifcQuantityLength);
                    ifcElementQuantity.Quantities.Items.Add(new Ifc4.IfcQuantityLength() { Ref = ifcQuantityLength.Id });
                }
                else
                {
                    // read
                    ifcQuantityLengthPropertyInfo.SetValue(this, ifcQuantityLength.LengthValue, null);
                }
            }
            // ---------------------------------------------------------------------------------------
            string[] ifcQuantityAreaNames = new string[]
            {
                nameof(GrossFloorArea),
                nameof(NetFloorArea),
                nameof(GrossWallArea),
                nameof(NetWallArea),
                nameof(GrossCeilingArea),
                nameof(NetCeilingArea)
            };
            foreach (var ifcQuantityAreaName in ifcQuantityAreaNames)
            {
                var ifcQuantityAreaPropertyInfo = this.GetType().GetProperty(ifcQuantityAreaName);
                if (ifcQuantityAreaPropertyInfo == null)
                    continue;

                var ifcQuantityArea = m_IfcQuantityAreaCollection.FirstOrDefault(item => item.Name.Equals(ifcQuantityAreaName, StringComparison.OrdinalIgnoreCase));
                if (ifcQuantityArea == null)
                {
                    ifcQuantityArea = new IfcQuantityArea()
                    {
                        Id = this.Document.GetNextSid(),
                        Name = ifcQuantityAreaName,
                        AreaValue = (double)ifcQuantityAreaPropertyInfo.GetValue(this, null)
                    };
                    m_IfcQuantityAreaCollection.Add(ifcQuantityArea);
                    this.Document.IfcXmlDocument.Items.Add(ifcQuantityArea);
                    ifcElementQuantity.Quantities.Items.Add(new Ifc4.IfcQuantityArea() { Ref = ifcQuantityArea.Id });
                }
                else
                {
                    // read
                    ifcQuantityAreaPropertyInfo.SetValue(this, ifcQuantityArea.AreaValue, null);
                }
            }
            // ---------------------------------------------------------------------------------------
            string[] ifcQuantityVolumeNames = new string[]
            {
                nameof(GrossVolume),
                nameof(NetVolume)
            };
            foreach (var ifcQuantityVolumeName in ifcQuantityVolumeNames)
            {
                var ifcQuantityVolumePropertyInfo = this.GetType().GetProperty(ifcQuantityVolumeName);
                if (ifcQuantityVolumePropertyInfo == null)
                    continue;

                var ifcQuantityVolume = m_IfcQuantityVolumeCollection.FirstOrDefault(item => item.Name.Equals(ifcQuantityVolumeName, StringComparison.OrdinalIgnoreCase));
                if (ifcQuantityVolume == null)
                {
                    ifcQuantityVolume = new IfcQuantityVolume()
                    {
                        Id = this.Document.GetNextSid(),
                        Name = ifcQuantityVolumeName,
                        VolumeValue = (double)ifcQuantityVolumePropertyInfo.GetValue(this, null)
                    };
                    m_IfcQuantityVolumeCollection.Add(ifcQuantityVolume);
                    this.Document.IfcXmlDocument.Items.Add(ifcQuantityVolume);
                    ifcElementQuantity.Quantities.Items.Add(new Ifc4.IfcQuantityVolume() { Ref = ifcQuantityVolume.Id });
                }
                else
                {
                    // read
                    ifcQuantityVolumePropertyInfo.SetValue(this, ifcQuantityVolume.VolumeValue, null);
                }
            }
            // ---------------------------------------------------------------------------------------


            if (ifcElementQuantity.Id == null)
            {
                ifcElementQuantity.Id = this.Document.GetNextSid();
                ifcElementQuantity.Name = "Qto_SpaceBaseQuantities";
                this.Document.IfcXmlDocument.Items.Add(ifcElementQuantity);

                Ifc4.IfcRelDefinesByProperties ifcRelDefinesByProperties = new Ifc4.IfcRelDefinesByProperties();
                ifcRelDefinesByProperties.Id = this.Document.GetNextSid();
                ifcRelDefinesByProperties.RelatedObjects = this.RefInstance();
                ifcRelDefinesByProperties.RelatingPropertyDefinition = new Ifc4.IfcRelDefinesByPropertiesRelatingPropertyDefinition();
                ifcRelDefinesByProperties.RelatingPropertyDefinition.Item = new Ifc4.IfcElementQuantity() { Ref = ifcElementQuantity.Id };
                this.Document.IfcXmlDocument.Items.Add(ifcRelDefinesByProperties);
            }

        }

        private void UpdateIfcRelAssociatesClassification(string propertyName, IfcClassificationReference ifcClassificationReference)
        {

            if (ifcClassificationReference == null)
            {
                // delete
                DeleteIfcRelAssociatesClassification();
                return;
            }

            var existingIfcRelAssociatesClassificationCollection = GetIfcRelAssociatesClassificationCollection();
            if (existingIfcRelAssociatesClassificationCollection.Count() == 0)
            {
                // add
                IfcRelAssociatesClassification ifcRelAssociatesClassification = new IfcRelAssociatesClassification();
                ifcRelAssociatesClassification.RelatedObjects = new IfcRelAssociatesRelatedObjects();
                ifcRelAssociatesClassification.RelatedObjects.Items.Add(this.RefInstance());

                ifcRelAssociatesClassification.RelatingClassification = new IfcRelAssociatesClassificationRelatingClassification();
                ifcRelAssociatesClassification.RelatingClassification.Item = new IfcClassificationReference() { Ref = ifcClassificationReference.Id };
                this.Document.IfcXmlDocument.Items.Add(ifcRelAssociatesClassification);
                return;
            }

            foreach (var ifcRelAssociatesClassification in existingIfcRelAssociatesClassificationCollection)
            {
                // update
                ifcRelAssociatesClassification.RelatingClassification.Item = new IfcClassificationReference() { Ref = ifcClassificationReference.Id };
            }
        }

        private void DeleteIfcRelAssociatesClassification()
        {
            Ifc4.Document document = this.Document;
            foreach (var ifcRelAssociatesClassification in GetIfcRelAssociatesClassificationCollection().ToList())
            {
                document.IfcXmlDocument.Items.Remove(ifcRelAssociatesClassification);
            }
        }

        private IEnumerable<IfcRelAssociatesClassification> GetIfcRelAssociatesClassificationCollection()
        {
            Ifc4.Document document = this.Document;
            return from ifcRelAssociatesClassification in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesClassification>()
                   where ifcRelAssociatesClassification.RelatedObjects != null &&
                   ifcRelAssociatesClassification.RelatedObjects.Items.Count == 1 &&
                   ifcRelAssociatesClassification.RelatedObjects.Items[0].Ref == this.Id
                   select ifcRelAssociatesClassification;
        }

        private Ifc4.IfcClassificationReference GetIfcClassificationReferenceFromIfcRelAssociatesClassification()
        {
            Ifc4.Document document = this.Document;

            IfcClassificationReference ifcClassificationReference = (from ifcRelAssociatesClassification in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesClassification>()
                                                                     where ifcRelAssociatesClassification.RelatedObjects != null &&
                                                                     ifcRelAssociatesClassification.RelatedObjects.Items.Exists(item => item.Ref == this.Id) &&
                                                                     ifcRelAssociatesClassification.RelatingClassification != null &&
                                                                     ifcRelAssociatesClassification.RelatingClassification.Item is IfcClassificationReference
                                                                     select ifcRelAssociatesClassification.RelatingClassification.Item).FirstOrDefault() as IfcClassificationReference;

            if (ifcClassificationReference != null && ifcClassificationReference.IsRef)
            {
                ifcClassificationReference = this.Document.IfcXmlDocument.Items.OfType<IfcClassificationReference>().FirstOrDefault(item => item.Id == ifcClassificationReference.Ref);
            }
            return ifcClassificationReference;
        }

    }

}
