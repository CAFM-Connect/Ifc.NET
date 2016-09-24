using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcSite
    {
        private CcIfcBuildings<IfcBuilding> m_Buildings;
        [System.Xml.Serialization.XmlIgnore]
        public CcIfcBuildings<IfcBuilding> Buildings
        {
            get
            {
                if (m_Buildings == null)
                    m_Buildings = new CcIfcBuildings<IfcBuilding>(this);
                return m_Buildings;
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
                // nur wenn keine Gebäude vorhanden sind
                return (this.Buildings.Count() == 0);
            }
        }

        public override bool Remove()
        {
            bool result = true;

            IfcPostalAddress ifcPostalAddress = this.SiteAddress;
            if (ifcPostalAddress != null && ifcPostalAddress.IsRef)
                ifcPostalAddress = this.Document.IfcXmlDocument.Items.OfType<IfcPostalAddress>().FirstOrDefault(item => item.Id == ifcPostalAddress.Ref);

            if (ifcPostalAddress != null)
                result = ifcPostalAddress.Remove();

            result &= base.Remove();
            return result;
        }

        public override bool Read(BaseObject parent)
        {
            //int pInstance = this.InstanceReference;
            //int pAggrElement = pInstance;

            //base.Read(); // CcSpatialStructureElement

            //string value;
            //int pValue = 0;

            //IfcConnection.sdaiGetAttrBN(pAggrElement, "SiteAddress", (int)IfcConnection.ValueType.sdaiINSTANCE, ref pValue);
            //if (pValue != 0)
            //{
            //    SiteAddress = (from a in Document.PostalAddresses
            //                   where IfcConnection.engiGetInstanceLocalId(a.InstanceReference) == IfcConnection.engiGetInstanceLocalId(pValue)
            //                   select a).FirstOrDefault();
            //}
            //this.Buildings.Read();
            return true;

        }

        public override Type GetAddObjectType()
        {
            return typeof(Ifc4.IfcBuilding);
        }

    }


}
