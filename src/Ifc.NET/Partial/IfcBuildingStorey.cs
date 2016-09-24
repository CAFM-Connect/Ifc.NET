using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcBuildingStorey
    {
        private CcIfcSpaces<IfcSpace> m_Spaces;
        [System.Xml.Serialization.XmlIgnore]
        public CcIfcSpaces<IfcSpace> Spaces
        {
            get
            {
                if (m_Spaces == null)
                    m_Spaces = new CcIfcSpaces<IfcSpace>(this);
                return m_Spaces;
            }
        }

        public override bool CanAdd
        {
            get { return true; }
        }

        public override bool CanRemove
        {
            get { return (this.Spaces.Count() == 0); }
        }

        public override Type GetAddObjectType()
        {
            return typeof(Ifc4.IfcSpace);
        }

        public override bool Read(BaseObject baseObject)
        {
            InitializeAdditionalProperties();
            return true;
        }

        private double _GrossHeight;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_GROSSHEIGHT_DISPLAYNAME", "GrossHeight")]
        public double GrossHeight // [Qto_BuildingStoreyBaseQuantities]")]
        {
            get { return _GrossHeight; }
            set
            {
                if (_GrossHeight != value)
                {
                    _GrossHeight = value;
                    string propertyName = this.PropertyName(() => this.GrossHeight);
                    UpdateIfcQuantityLength(propertyName, GrossHeight);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossHeight));
                }
            }
        }
        private double _NetHeight;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_NETHEIGHT_DISPLAYNAME", "NetHeight")]
        public double NetHeight // [Qto_BuildingStoreyBaseQuantities]")]
        {
            get { return _NetHeight; }
            set
            {
                if (_NetHeight != value)
                {
                    _NetHeight = value;
                    string propertyName = this.PropertyName(() => this.NetHeight);
                    UpdateIfcQuantityLength(propertyName, NetHeight);
                    RaisePropertyChanged(this.PropertyName(() => this.NetHeight));
                }
            }
        }
        private double _GrossPerimeter;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_GROSSPERIMETER_DISPLAYNAME", "GrossPerimeter")]
        public double GrossPerimeter // [Qto_BuildingStoreyBaseQuantities]")]
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

        private double _GrossFloorArea;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_GROSSFLOORAREA_DISPLAYNAME", "GrossFloorArea")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_GROSSFLOORAREA_DISPLAYNAME", "Bruttogrundfläche [m²]")]
        public double GrossFloorArea // [Qto_BuildingBaseQuantities]")]
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
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_NETFLOORAREA_DISPLAYNAME", "NetFloorArea")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_NETFLOORAREA_DISPLAYNAME", "Nettogrundfläche [m²]")]
        public double NetFloorArea // [Qto_BuildingBaseQuantities]")]
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

        private double _GrossVolume;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_GROSSVOLUME_DISPLAYNAME", "GrossVolume")]
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
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREY_PROPERTY_NETVOLUME_DISPLAYNAME", "NetVolume")]
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

        private List<IfcQuantityLength> m_IfcQuantityLengthCollection;
        private List<IfcQuantityArea> m_IfcQuantityAreaCollection;
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
                nameof(GrossHeight),
                nameof(NetHeight),
                nameof(GrossPerimeter)
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
                nameof(NetFloorArea)
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
                ifcElementQuantity.Name = "Qto_BuildingStoreyBaseQuantities";
                this.Document.IfcXmlDocument.Items.Add(ifcElementQuantity);

                Ifc4.IfcRelDefinesByProperties ifcRelDefinesByProperties = new Ifc4.IfcRelDefinesByProperties();
                ifcRelDefinesByProperties.Id = this.Document.GetNextSid();
                ifcRelDefinesByProperties.RelatedObjects = this.RefInstance();
                ifcRelDefinesByProperties.RelatingPropertyDefinition = new Ifc4.IfcRelDefinesByPropertiesRelatingPropertyDefinition();
                ifcRelDefinesByProperties.RelatingPropertyDefinition.Item = new Ifc4.IfcElementQuantity() { Ref = ifcElementQuantity.Id };
                this.Document.IfcXmlDocument.Items.Add(ifcRelDefinesByProperties);
            }

        }



    }

}
