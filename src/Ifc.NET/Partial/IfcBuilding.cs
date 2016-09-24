using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcBuilding
    {
        public IfcBuilding()
        {
            //PropertySetCollection = new List<IfcPropertySet>();
            //ElementQuantityCollection = new List<IfcElementQuantity>();
        }

        //[System.ComponentModel.Browsable(false)]
        //[System.Xml.Serialization.XmlIgnore]
        //public List<IfcPropertySet> PropertySetCollection { get; private set; }

        //[System.ComponentModel.Browsable(false)]
        //[System.Xml.Serialization.XmlIgnore]
        //public List<IfcElementQuantity> ElementQuantityCollection { get; private set; }

        public override bool Read(BaseObject baseObject)
        {
            InitializeAdditionalProperties();
            return true;
        }

        private CcIfcBuildingStoreys<IfcBuildingStorey> m_BuildingStoreys;
        [System.Xml.Serialization.XmlIgnore]
        public CcIfcBuildingStoreys<IfcBuildingStorey> BuildingStoreys
        {
            get
            {
                if (m_BuildingStoreys == null)
                    m_BuildingStoreys = new CcIfcBuildingStoreys<IfcBuildingStorey>(this);

                return m_BuildingStoreys;
            }
        }

        private List<IfcPropertySingleValue> m_IfcPropertySingleValueCollection;
        private List<IfcQuantityArea> m_IfcQuantityAreaCollection;

        public void InitializeAdditionalProperties()
        {
            // default value
            this.IsLandmarked = false;
            this.YearOfConstruction = "";
            this.NumberOfStoreys = 0;
            this.OccupancyType = "";

            Ifc4.IfcPropertySet ifcPropertySet = this.GetIfcPropertySetFromRelatingPropertyDefinition();

            IEnumerable<IfcProperty> ifcPropertyCollection = this.Document.IfcXmlDocument.Items.OfType<IfcProperty>().ToList();

            if (ifcPropertySet == null)
            {
                ifcPropertySet = new IfcPropertySet();
            }

            if (ifcPropertySet.HasProperties == null)
            {
                ifcPropertySet.HasProperties = new IfcPropertySetHasProperties();
            }

            m_IfcPropertySingleValueCollection = new List<IfcPropertySingleValue>();
            foreach (IfcProperty ifcPropertyTmp in ifcPropertySet.HasProperties.Items)
            {
                if (ifcPropertyTmp.IsRef)
                {
                    IfcProperty ifcProperty = ifcPropertyCollection.FirstOrDefault(item => item.Id == ifcPropertyTmp.Ref);
                    if (ifcProperty != null && ifcProperty is IfcPropertySingleValue)
                    {
                        m_IfcPropertySingleValueCollection.Add((IfcPropertySingleValue)ifcProperty);
                    }
                }
            }

            IfcPropertySingleValue ifcPropertySingleValue = null;
            ifcPropertySingleValue = m_IfcPropertySingleValueCollection.FirstOrDefault(item => item.Name.Equals("IsLandmarked", StringComparison.OrdinalIgnoreCase));
            if (ifcPropertySingleValue == null)
            {
                ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                {
                    Id = this.Document.GetNextSid(),
                    Name = "IsLandmarked",
                    NominalValue = new Ifc4.IfcPropertySingleValueNominalValue(),
                };

                ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcLogicalwrapper()
                {
                    Value = IsLandmarked ? Ifc4.IfcLogical.True : Ifc4.IfcLogical.False
                };

                m_IfcPropertySingleValueCollection.Add(ifcPropertySingleValue);
                this.Document.IfcXmlDocument.Items.Add(ifcPropertySingleValue);
                ifcPropertySet.HasProperties.Items.Add(new Ifc4.IfcPropertySingleValue() { Ref = ifcPropertySingleValue.Id });
            }
            else
            {
                // read
                Ifc4.IfcLogicalwrapper ifcLogicalwrapper = ifcPropertySingleValue.NominalValue.Item as Ifc4.IfcLogicalwrapper;
                if (ifcLogicalwrapper != null)
                    this.IsLandmarked = ifcLogicalwrapper.Value == IfcLogical.True ? true : false;
            }

            ifcPropertySingleValue = m_IfcPropertySingleValueCollection.FirstOrDefault(item => item.Name.Equals("YearOfConstruction", StringComparison.OrdinalIgnoreCase));
            if (ifcPropertySingleValue == null)
            {
                ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                {
                    Id = this.Document.GetNextSid(),
                    Name = "YearOfConstruction",
                    NominalValue = new Ifc4.IfcPropertySingleValueNominalValue(),
                };

                ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcLabelwrapper()
                {
                    Value = this.YearOfConstruction
                };

                m_IfcPropertySingleValueCollection.Add(ifcPropertySingleValue);
                this.Document.IfcXmlDocument.Items.Add(ifcPropertySingleValue);
                ifcPropertySet.HasProperties.Items.Add(new Ifc4.IfcPropertySingleValue() { Ref = ifcPropertySingleValue.Id });
            }
            else
            {
                // read
                Ifc4.IfcLabelwrapper ifcLabelwrapper = ifcPropertySingleValue.NominalValue.Item as Ifc4.IfcLabelwrapper;
                if (ifcLabelwrapper != null)
                    this.YearOfConstruction = ifcLabelwrapper.Value;
            }

            ifcPropertySingleValue = m_IfcPropertySingleValueCollection.FirstOrDefault(item => item.Name.Equals("NumberOfStoreys", StringComparison.OrdinalIgnoreCase));
            if (ifcPropertySingleValue == null)
            {
                ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                {
                    Id = this.Document.GetNextSid(),
                    Name = "NumberOfStoreys",
                    NominalValue = new Ifc4.IfcPropertySingleValueNominalValue(),
                };

                ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcIntegerwrapper()
                {
                    Value = this.NumberOfStoreys
                };


                m_IfcPropertySingleValueCollection.Add(ifcPropertySingleValue);
                this.Document.IfcXmlDocument.Items.Add(ifcPropertySingleValue);
                ifcPropertySet.HasProperties.Items.Add(new Ifc4.IfcPropertySingleValue() { Ref = ifcPropertySingleValue.Id });
            }
            else
            {
                // read
                Ifc4.IfcIntegerwrapper ifcIntegerwrapper = ifcPropertySingleValue.NominalValue.Item as Ifc4.IfcIntegerwrapper;
                if (ifcIntegerwrapper != null)
                    this.NumberOfStoreys = ifcIntegerwrapper.Value;
            }

            ifcPropertySingleValue = m_IfcPropertySingleValueCollection.FirstOrDefault(item => item.Name.Equals("OccupancyType", StringComparison.OrdinalIgnoreCase));
            if (ifcPropertySingleValue == null)
            {
                ifcPropertySingleValue = new Ifc4.IfcPropertySingleValue()
                {
                    Id = this.Document.GetNextSid(),
                    Name = "OccupancyType",
                    NominalValue = new Ifc4.IfcPropertySingleValueNominalValue(),
                };

                ifcPropertySingleValue.NominalValue.Item = new Ifc4.IfcLabelwrapper()
                {
                    Value = this.OccupancyType
                };

                m_IfcPropertySingleValueCollection.Add(ifcPropertySingleValue);
                this.Document.IfcXmlDocument.Items.Add(ifcPropertySingleValue);
                ifcPropertySet.HasProperties.Items.Add(new Ifc4.IfcPropertySingleValue() { Ref = ifcPropertySingleValue.Id });
            }
            else
            {
                // read
                Ifc4.IfcLabelwrapper ifcLabelwrapper = ifcPropertySingleValue.NominalValue.Item as Ifc4.IfcLabelwrapper;
                if (ifcLabelwrapper != null)
                    this.OccupancyType = ifcLabelwrapper.Value;
            }


            if (ifcPropertySet.Id == null)
            {
                ifcPropertySet.Id = this.Document.GetNextSid();
                ifcPropertySet.Name = "Pset_BuildingCommon";
                this.Document.IfcXmlDocument.Items.Add(ifcPropertySet);

                Ifc4.IfcRelDefinesByProperties ifcRelDefinesByProperties = new Ifc4.IfcRelDefinesByProperties();
                ifcRelDefinesByProperties.Id = this.Document.GetNextSid();
                //ifcRelDefinesByProperties.RelatedObjects = new Ifc4.IfcBuilding() { Ref = this.Id };
                ifcRelDefinesByProperties.RelatedObjects = this.RefInstance();
                ifcRelDefinesByProperties.RelatingPropertyDefinition = new Ifc4.IfcRelDefinesByPropertiesRelatingPropertyDefinition();
                ifcRelDefinesByProperties.RelatingPropertyDefinition.Item = new Ifc4.IfcPropertySet() { Ref = ifcPropertySet.Id };
                this.Document.IfcXmlDocument.Items.Add(ifcRelDefinesByProperties);
            }

            // -----------------------------------------
            Ifc4.IfcElementQuantity ifcElementQuantity = this.GetIfcElementQuantityFromRelatingPropertyDefinition();
            if (ifcElementQuantity != null && ifcElementQuantity.IsRef)
            {
                ifcElementQuantity = this.Document.IfcXmlDocument.Items.OfType<IfcElementQuantity>().FirstOrDefault(item => item.Id == ifcElementQuantity.Ref);
            }

            IEnumerable<IfcQuantityArea> ifcQuantityAreaCollection = this.Document.IfcXmlDocument.Items.OfType<IfcQuantityArea>();

            if (ifcElementQuantity == null)
            {
                ifcElementQuantity = new IfcElementQuantity();
            }

            if (ifcElementQuantity.Quantities == null)
            {
                ifcElementQuantity.Quantities = new IfcElementQuantityQuantities();
            }


            m_IfcQuantityAreaCollection = new List<IfcQuantityArea>();
            foreach (IfcQuantityArea ifcQuantityAreaTmp in ifcElementQuantity.Quantities.Items)
            {
                if (ifcQuantityAreaTmp.IsRef)
                {
                    IfcQuantityArea existingIfcQuantityArea = ifcQuantityAreaCollection.FirstOrDefault(item => item.Id == ifcQuantityAreaTmp.Ref);
                    if (existingIfcQuantityArea != null)
                    {
                        m_IfcQuantityAreaCollection.Add(existingIfcQuantityArea);
                    }
                }
            }

            IfcQuantityArea ifcQuantityArea = null;
            ifcQuantityArea = m_IfcQuantityAreaCollection.FirstOrDefault(item => item.Name.Equals("GrossFloorArea", StringComparison.OrdinalIgnoreCase));
            if (ifcQuantityArea == null)
            {
                ifcQuantityArea = new IfcQuantityArea()
                {
                    Id = this.Document.GetNextSid(),
                    Name = "GrossFloorArea",
                    AreaValue = GrossFloorArea
                };

                m_IfcQuantityAreaCollection.Add(ifcQuantityArea);
                this.Document.IfcXmlDocument.Items.Add(ifcQuantityArea);
                ifcElementQuantity.Quantities.Items.Add(new Ifc4.IfcQuantityArea() { Ref = ifcQuantityArea.Id });
            }
            else
            {
                // read
                this.GrossFloorArea = ifcQuantityArea.AreaValue;
            }

            ifcQuantityArea = m_IfcQuantityAreaCollection.FirstOrDefault(item => item.Name.Equals("NetFloorArea", StringComparison.OrdinalIgnoreCase));
            if (ifcQuantityArea == null)
            {
                ifcQuantityArea = new IfcQuantityArea()
                {
                    Id = this.Document.GetNextSid(),
                    Name = "NetFloorArea",
                    AreaValue = NetFloorArea
                };

                m_IfcQuantityAreaCollection.Add(ifcQuantityArea);
                this.Document.IfcXmlDocument.Items.Add(ifcQuantityArea);
                ifcElementQuantity.Quantities.Items.Add(new Ifc4.IfcQuantityArea() { Ref = ifcQuantityArea.Id });
            }
            else
            {
                // read
                this.NetFloorArea = ifcQuantityArea.AreaValue;
            }

            if (ifcElementQuantity.Id == null)
            {
                ifcElementQuantity.Id = this.Document.GetNextSid();
                ifcElementQuantity.Name = "Qto_BuildingBaseQuantities";
                this.Document.IfcXmlDocument.Items.Add(ifcElementQuantity);

                Ifc4.IfcRelDefinesByProperties ifcRelDefinesByProperties = new Ifc4.IfcRelDefinesByProperties();
                ifcRelDefinesByProperties.Id = this.Document.GetNextSid();
                ifcRelDefinesByProperties.RelatedObjects = this.RefInstance();
                ifcRelDefinesByProperties.RelatingPropertyDefinition = new Ifc4.IfcRelDefinesByPropertiesRelatingPropertyDefinition();


                // old
                // ifcRelDefinesByProperties.RelatingPropertyDefinition.Item = new Ifc4.IfcElementQuantity() { Ref = ifcElementQuantity.Id };

                // new
                ifcRelDefinesByProperties.RelatingPropertyDefinition.Item = ((IfcPropertySetDefinition)(new Ifc4.IfcElementQuantity() { Ref = ifcElementQuantity.Id }));

                this.Document.IfcXmlDocument.Items.Add(ifcRelDefinesByProperties);
            }

        }

        public override bool CanAdd
        {
            get { return true; }
        }

        public override bool CanRemove
        {
            get
            {
                return (this.BuildingStoreys.Count() == 0);
            }
        }

        public override bool Remove()
        {
            bool result = true;

            IfcPostalAddress ifcPostalAddress = this.BuildingAddress;
            if (ifcPostalAddress != null && ifcPostalAddress.IsRef)
                ifcPostalAddress = this.Document.IfcXmlDocument.Items.OfType<IfcPostalAddress>().FirstOrDefault(item => item.Id == ifcPostalAddress.Ref);

            if (ifcPostalAddress != null)
                result = ifcPostalAddress.Remove();

            result &= base.Remove();
            return result;
        }

        public override Type GetAddObjectType()
        {
            return typeof(Ifc4.IfcBuildingStorey);
        }

        //private void WriteElementQuantity()
        //{
        //    string propertyType = "IfcQuantityArea";
        //    ElementQuantity.AddOrUpdateElement(propertyType, "GrossFloorArea", System.Convert.ToString(GrossFloorArea, System.Globalization.CultureInfo.InvariantCulture), IfcConnection.sdaiREAL);
        //    ElementQuantity.AddOrUpdateElement(propertyType, "NetFloorArea", System.Convert.ToString(NetFloorArea, System.Globalization.CultureInfo.InvariantCulture), IfcConnection.sdaiREAL);
        //    ElementQuantity.Write();
        //    Document.RelDefinesByPropertiesCollection.AddOrUpdateRelDefinesByPropertiesInstance("PropertyContainer", "SiteContainerForElementQuantity", ElementQuantity.InstanceReference, this.InstanceReference);
        //}

        private void UpdateIfcQuantityArea(string propertyName, double value)
        {
            if (m_IfcQuantityAreaCollection != null)
            {
                Ifc4.IfcQuantityArea ifcQuantityArea = m_IfcQuantityAreaCollection.FirstOrDefault(item => item.Name == propertyName);
                ifcQuantityArea.AreaValue = value;
            }
        }

        private void UpdateIfcPropertySingleValue(string propertyName, object value)
        {
            if (m_IfcPropertySingleValueCollection != null)
            {
                Ifc4.IfcPropertySingleValue ifcPropertySingleValue = m_IfcPropertySingleValueCollection.FirstOrDefault(item => item.Name == propertyName);
                UpdateIfcPropertySingleValue(ifcPropertySingleValue, value);
            }
        }

        private T Get<T>(object instance) where T : class
        {
            return (T)Convert.ChangeType(instance, typeof(T));
        }

        private void UpdateIfcPropertySingleValue(Ifc4.IfcPropertySingleValue ifcPropertySingleValue, object value)
        {
            if (ifcPropertySingleValue == null || ifcPropertySingleValue.NominalValue == null)
                return;

            object item = ifcPropertySingleValue.NominalValue.Item;

            if (value is Boolean)
            {
                if (item is IfcLogicalwrapper)
                {
                    var valueItem = Get<IfcLogicalwrapper>(item);
                    valueItem.Value = ((Boolean)value) ? IfcLogical.True : IfcLogical.False;
                    valueItem.InvariantStringValue = ((Boolean)value) ? Boolean.TrueString : Boolean.FalseString;
                }
                else if (item is IfcBooleanwrapper)
                {
                    var valueItem = Get<IfcBooleanwrapper>(item);
                    valueItem.Value = (Boolean)Convert.ChangeType(value, typeof(Boolean));
                }
            }
            else if (value is Int32)
            {
                var valueItem = Get<IfcIntegerwrapper>(item);
                valueItem.Value = (Int32)Convert.ChangeType(value, typeof(Int32));
            }
            else if (value is Int64)
            {
                var valueItem = Get<IfcIntegerwrapper>(item);
                valueItem.Value = (Int64)Convert.ChangeType(value, typeof(Int64));
            }
            else if (value is Double)
            {
                var valueItem = Get<IfcRealwrapper>(item);
                valueItem.Value = (Double)Convert.ChangeType(value, typeof(Double));
            }
            else
            {
                var valueItem = Get<IfcLabelwrapper>(item);
                valueItem.Value = (value == null) ? String.Empty : value.ToString();
            }
        }

        private bool _IsLandmarked;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_ISLANDMARKED_DISPLAYNAME", "IsLandmarked")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_ISLANDMARKED_DISPLAYNAME", "Denkmalschutz")]
        public bool IsLandmarked // [Pset_BuildingCommon]
        {
            get { return _IsLandmarked; }
            set
            {
                if (_IsLandmarked != value)
                {
                    _IsLandmarked = value;
                    string propertyName = this.PropertyName(() => this.IsLandmarked);
                    UpdateIfcPropertySingleValue(propertyName, _IsLandmarked);
                    RaisePropertyChanged(propertyName);
                }
            }
        }

        private string _YearOfConstruction;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_YEAROFCONSTRUCTION_DISPLAYNAME", "YearOfConstruction")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_YEAROFCONSTRUCTION_DISPLAYNAME", "Baujahr")]
        public string YearOfConstruction // [Pset_BuildingCommon]
        {
            get { return _YearOfConstruction; }
            set
            {
                if (_YearOfConstruction != value)
                {
                    _YearOfConstruction = value;
                    string propertyName = this.PropertyName(() => this.YearOfConstruction);
                    UpdateIfcPropertySingleValue(propertyName, _YearOfConstruction);
                    RaisePropertyChanged(propertyName);
                }
            }
        }
        private long _NumberOfStoreys;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_NUMBEROFSTOREYS_DISPLAYNAME", "NumberOfStoreys")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_NUMBEROFSTOREYS_DISPLAYNAME", "Anzahl der Etagen")]
        //[System.ComponentModel.TypeConverter(typeof(Ifc4.CustomModel.CustomLongNullableTypeConverter))]
        public long NumberOfStoreys // [Pset_BuildingCommon]
        {
            get { return _NumberOfStoreys; }
            set
            {
                if (_NumberOfStoreys != value)
                {
                    _NumberOfStoreys = value;
                    string propertyName = this.PropertyName(() => this.NumberOfStoreys);
                    UpdateIfcPropertySingleValue(propertyName, _NumberOfStoreys);
                    RaisePropertyChanged(propertyName);
                }
            }
        }

        private string _OccupancyType;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_OCCUPANCYTYPE_DISPLAYNAME", "OccupancyType")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_OCCUPANCYTYPE_DISPLAYNAME", "Nutzungsart")]
        public string OccupancyType // [Pset_BuildingCommon]
        {
            get { return _OccupancyType; }
            set
            {
                if (_OccupancyType != value)
                {
                    _OccupancyType = value;
                    string propertyName = this.PropertyName(() => this.OccupancyType);
                    UpdateIfcPropertySingleValue(propertyName, _OccupancyType);
                    RaisePropertyChanged(propertyName);
                }
            }
        }

        private double _GrossFloorArea;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_GROSSFLOORAREA_DISPLAYNAME", "GrossFloorArea")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_GROSSFLOORAREA_DISPLAYNAME", "Bruttogrundfläche [m²]")]
        public double GrossFloorArea // [Qto_BuildingBaseQuantities]")]
        {
            get { return _GrossFloorArea; }
            set
            {
                if (_GrossFloorArea != value)
                {
                    _GrossFloorArea = value;
                    string propertyName = this.PropertyName(() => this.GrossFloorArea);
                    UpdateIfcQuantityArea(propertyName, _GrossFloorArea);
                    RaisePropertyChanged(this.PropertyName(() => this.GrossFloorArea));
                }
            }
        }

        private double _NetFloorArea;
        [System.Xml.Serialization.XmlIgnore()]
        //[Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_NETFLOORAREA_DISPLAYNAME", "NetFloorArea")]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDING_PROPERTY_NETFLOORAREA_DISPLAYNAME", "Nettogrundfläche [m²]")]
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

        //<IfcQuantityArea id="i99">
        //    <Name>GrossFloorArea</Name>
        //    <AreaValue>33.44</AreaValue>
        //</IfcQuantityArea>
        //<IfcQuantityArea id="i100">
        //    <Name>NetFloorArea</Name>
        //    <AreaValue>55.66</AreaValue>
        //</IfcQuantityArea>
        //<IfcElementQuantity id="i101">
        //    <GlobalId>2JITMKxs910AHJ2DszmOZL</GlobalId>
        //    <OwnerHistory>
        //        <IfcOwnerHistory xsi:nil="true" ref="i2"/>
        //    </OwnerHistory>
        //    <Name>Qto_BuildingBaseQuantities</Name>
        //    <Description/>
        //    <Quantities ex:cType="set">
        //        <IfcQuantityArea pos="0" xsi:nil="true" ref="i99"/>
        //        <IfcQuantityArea pos="1" xsi:nil="true" ref="i100"/>
        //    </Quantities>
        //</IfcElementQuantity>
        //<IfcRelDefinesByProperties id="i102">
        //    <GlobalId>2mZGDAhbDA7fyfyRig66rm</GlobalId>
        //    <OwnerHistory>
        //        <IfcOwnerHistory xsi:nil="true" ref="i2"/>
        //    </OwnerHistory>
        //    <Name>PropertyContainer</Name>
        //    <Description>SiteContainerForElementQuantity</Description>
        //    <RelatedObjects ex:cType="set">
        //        <IfcBuilding pos="0" xsi:nil="true" ref="i88"/>
        //    </RelatedObjects>
        //    <RelatingPropertyDefinition>
        //        <IfcElementQuantity xsi:nil="true" ref="i101"/>
        //    </RelatingPropertyDefinition>
        //</IfcRelDefinesByProperties>

        // ------------------------------

        //<IfcPostalAddress id="i82">
        //    <InternalLocation/>
        //    <AddressLines ex:cType="list">
        //        <IfcLabel>eTASK Headquarter Wilhelm-Ruppert-Stra\X\DFe 38</IfcLabel>
        //        <IfcLabel>Building K15</IfcLabel>
        //    </AddressLines>
        //    <PostalBox/>
        //    <Town>Cologne</Town>
        //    <Region>NRW</Region>
        //    <PostalCode>51147</PostalCode>
        //    <Country>Germany</Country>
        //</IfcPostalAddress>

        //<IfcBuilding id="i88">
        //    <GlobalId>28tbf6Cp58GBazzofhZWIV</GlobalId>
        //    <OwnerHistory>
        //        <IfcOwnerHistory xsi:nil="true" ref="i2"/>
        //    </OwnerHistory>
        //    <Name>B</Name>
        //    <Description>BB</Description>
        //    <ObjectPlacement>
        //        <IfcLocalPlacement xsi:nil="true" ref="i89"/>
        //    </ObjectPlacement>
        //    <LongName>BBB</LongName>
        //    <CompositionType>element</CompositionType>
        //    <BuildingAddress>
        //        <IfcPostalAddress xsi:nil="true" ref="i82"/>
        //    </BuildingAddress>
        //</IfcBuilding>



    }
}
