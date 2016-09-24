using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcSlab
    {

        public override bool Read(BaseObject baseObject)
        {
            InitializeAdditionalProperties();
            return true;
        }

        private double _Width;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_WIDTH_DISPLAYNAME", "Width")]
        public double Width // Qto_SlabBaseQuantities
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    string propertyName = this.PropertyName(() => this.Width);
                    UpdateIfcQuantityLength(propertyName, Width);
                    RaisePropertyChanged(this.PropertyName(() => this.Width));
                }
            }
        }

        private double _Length;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_LENGTH_DISPLAYNAME", "Length")]
        public double Length // Qto_SlabBaseQuantities
        {
            get { return _Length; }
            set
            {
                if (_Length != value)
                {
                    _Length = value;
                    string propertyName = this.PropertyName(() => this.Length);
                    UpdateIfcQuantityLength(propertyName, Length);
                    RaisePropertyChanged(this.PropertyName(() => this.Length));
                }
            }
        }

        private double _Depth;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_DEPTH_DISPLAYNAME", "Depth")]
        public double Depth // Qto_SlabBaseQuantities
        {
            get { return _Depth; }
            set
            {
                if (_Depth != value)
                {
                    _Depth = value;
                    string propertyName = this.PropertyName(() => this.Depth);
                    UpdateIfcQuantityLength(propertyName, Depth);
                    RaisePropertyChanged(this.PropertyName(() => this.Depth));
                }
            }
        }

        private double _Perimeter;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_PERIMETER_DISPLAYNAME", "Perimeter")]
        public double Perimeter // Qto_SlabBaseQuantities
        {
            get { return _Perimeter; }
            set
            {
                if (_Perimeter != value)
                {
                    _Perimeter = value;
                    string propertyName = this.PropertyName(() => this.Perimeter);
                    UpdateIfcQuantityLength(propertyName, Perimeter);
                    RaisePropertyChanged(this.PropertyName(() => this.Perimeter));
                }
            }
        }

        private double _GrossArea;
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_GROSSAREA_DISPLAYNAME", "Bruttogrundfläche [m²]")]
        public double GrossArea // Qto_SlabBaseQuantities
        {
            get { return _GrossArea; }
            set
            {
                if (_GrossArea != value)
                {
                    _GrossArea = value;
                    string propertyName = this.PropertyName(() => this.GrossArea);
                    UpdateIfcQuantityArea(propertyName, GrossArea);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossArea));
                }
            }
        }

        private double _NetArea;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_NETAREA_DISPLAYNAME", "Nettogrundfläche [m²]")]
        public double NetArea // Qto_SlabBaseQuantities
        {
            get { return _NetArea; }
            set
            {
                if (_NetArea != value)
                {
                    _NetArea = value;
                    string propertyName = this.PropertyName(() => this.NetArea);
                    UpdateIfcQuantityArea(propertyName, NetArea);
                    RaisePropertyChanged(this.PropertyName(() => this.NetArea));
                }
            }
        }

        private double _GrossVolume;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_GROSSVOLUME_DISPLAYNAME", "GrossVolume")]
        public double GrossVolume // Qto_SlabBaseQuantities
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
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_NETVOLUME_DISPLAYNAME", "NetVolume")]
        public double NetVolume // Qto_SlabBaseQuantities
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

        private double _GrossWeight;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_GROSSWEIGHT_DISPLAYNAME", "GrossWeight")]
        public double GrossWeight // Qto_SlabBaseQuantities
        {
            get { return _GrossWeight; }
            set
            {
                if (_GrossWeight != value)
                {
                    _GrossWeight = value;
                    string propertyName = this.PropertyName(() => this.GrossWeight);
                    UpdateIfcQuantityWeight(propertyName, GrossWeight);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossWeight));
                }
            }
        }

        private double _NetWeight;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSLAB_PROPERTY_NETWEIGHT_DISPLAYNAME", "NetWeight")]
        public double NetWeight // Qto_SlabBaseQuantities
        {
            get { return _NetWeight; }
            set
            {
                if (_NetWeight != value)
                {
                    _NetWeight = value;
                    string propertyName = this.PropertyName(() => this.NetWeight);
                    UpdateIfcQuantityWeight(propertyName, NetWeight);
                    RaisePropertyChanged(this.PropertyName(() => this.NetWeight));
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
        private void UpdateIfcQuantityWeight(string propertyName, double value)
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
        private List<IfcQuantityWeight> m_IfcQuantityWeightCollection;

        public void InitializeAdditionalProperties()
        {
            Ifc4.IfcElementQuantity ifcElementQuantity = this.GetIfcElementQuantityFromRelatingPropertyDefinition();
            IEnumerable<IfcQuantityLength> ifcQuantityLengthCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityLength>().ToList();
            IEnumerable<IfcQuantityArea> ifcQuantityAreaCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityArea>().ToList();
            IEnumerable<IfcQuantityVolume> ifcQuantityVolumeCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityVolume>().ToList();
            IEnumerable<IfcQuantityWeight> ifcQuantityWeightCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityWeight>().ToList();

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
            m_IfcQuantityWeightCollection = new List<IfcQuantityWeight>();

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
                else if (ifcPhysicalQuantityTmp.IsRef && ifcPhysicalQuantityTmp is IfcQuantityWeight)
                {
                    IfcQuantityWeight existingIfcQuantityWeight = ifcQuantityWeightCollection.FirstOrDefault(item => item.Id == ifcPhysicalQuantityTmp.Ref);
                    if (existingIfcQuantityWeight != null)
                    {
                        m_IfcQuantityWeightCollection.Add(existingIfcQuantityWeight);
                    }
                }
            }
            // ---------------------------------------------------------------------------------------
            string[] ifcQuantityLengthNames = new string[]
            {
                nameof(Width),
                nameof(Perimeter)
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
                nameof(GrossArea),
                nameof(NetArea)
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
            string[] ifcQuantityWeightNames = new string[]
            {
                nameof(GrossVolume),
                nameof(NetVolume)
            };
            foreach (var ifcQuantityWeightName in ifcQuantityWeightNames)
            {
                var ifcQuantityWeightPropertyInfo = this.GetType().GetProperty(ifcQuantityWeightName);
                if (ifcQuantityWeightPropertyInfo == null)
                    continue;

                var ifcQuantityWeight = m_IfcQuantityWeightCollection.FirstOrDefault(item => item.Name.Equals(ifcQuantityWeightName, StringComparison.OrdinalIgnoreCase));
                if (ifcQuantityWeight == null)
                {
                    ifcQuantityWeight = new IfcQuantityWeight()
                    {
                        Id = this.Document.GetNextSid(),
                        Name = ifcQuantityWeightName,
                        WeightValue = (double)ifcQuantityWeightPropertyInfo.GetValue(this, null),
                    };
                    m_IfcQuantityWeightCollection.Add(ifcQuantityWeight);
                    this.Document.IfcXmlDocument.Items.Add(ifcQuantityWeight);
                    ifcElementQuantity.Quantities.Items.Add(new Ifc4.IfcQuantityWeight() { Ref = ifcQuantityWeight.Id });
                }
                else
                {
                    // read
                    ifcQuantityWeightPropertyInfo.SetValue(this, ifcQuantityWeight.WeightValue, null);
                }
            }
            // ---------------------------------------------------------------------------------------


            if (ifcElementQuantity.Id == null)
            {
                ifcElementQuantity.Id = this.Document.GetNextSid();
                ifcElementQuantity.Name = "Qto_SlabBaseQuantities";
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
