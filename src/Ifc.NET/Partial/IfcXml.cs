using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFC4 = Ifc4;

namespace Ifc4
{

    // http://msdn.microsoft.com/en-us/data/jj613116.aspx

    public partial class IfcXML : Uos
    {
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "schemaLocation", Namespace = System.Xml.Schema.XmlSchema.InstanceNamespace)]
        public string xsiSchemaLocation = @"http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd";

        private Dictionary<string, IfcClassificationReference> m_IfcClassificationReferenceFromIfcPropertySetTemplateId = new Dictionary<string, IfcClassificationReference>();

        private bool m_PopulateIfcPropertySetTemplate = false;

        public bool TryGetIfcClassificationReferenceFromIfcPropertySetTemplate(string ifcGlobalId, out Ifc4.IfcClassificationReference ifcClassificationReference)
        {
            Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate = GetIfcPropertySetTemplate(ifcGlobalId);
            ifcClassificationReference = GetIfcClassificationReferenceFromIfcPropertySetTemplate(ifcPropertySetTemplate);
            return (ifcClassificationReference != null);
        }

        private Ifc4.IfcClassificationReference GetIfcClassificationReferenceFromIfcPropertySetTemplate(Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate)
        {

            //<IfcRelAssociatesClassification id="i999" GlobalId="09Yj_c95H5iPvgszfwsNL6" Name="423.17 - Heizkörper zu IfcClassificationReference">
            //  <RelatedObjects>
            //    <IfcPropertySetTemplate ref="i992" xsi:nil="true" />
            //    <IfcPropertySetTemplate ref="i993" xsi:nil="true" />
            //  </RelatedObjects>
            //  <RelatingClassification>
            //    <IfcClassificationReference ref="i991" xsi:nil="true" />
            //  </RelatingClassification>
            //</IfcRelAssociatesClassification>
            Ifc4.IfcClassificationReference ifcClassificationReference;

            if (ifcPropertySetTemplate == null)
                return null;

            if (m_IfcClassificationReferenceFromIfcPropertySetTemplateId.TryGetValue(ifcPropertySetTemplate.Id, out ifcClassificationReference))
                return ifcClassificationReference;

            IEnumerable<Ifc4.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = Get<Ifc4.IfcRelAssociatesClassification>();
            var relatingClassification = (from ifcRelAssociatesClassification in ifcRelAssociatesClassificationCollection
                                          from relatedObjects in ifcRelAssociatesClassification.RelatedObjects.Items.OfType<Ifc4.IfcPropertySetTemplate>()
                                          where relatedObjects.Ref == ifcPropertySetTemplate.Id
                                          select ifcRelAssociatesClassification.RelatingClassification.Item).FirstOrDefault();

            ifcClassificationReference = relatingClassification as Ifc4.IfcClassificationReference;
            if (ifcClassificationReference != null)
            {
                ifcClassificationReference = Get<Ifc4.IfcClassificationReference>().FirstOrDefault(item => item.Id == ifcClassificationReference.Ref);
            }
            m_IfcClassificationReferenceFromIfcPropertySetTemplateId.Add(ifcPropertySetTemplate.Id, ifcClassificationReference);
            return ifcClassificationReference;

        }

        public IEnumerable<T> Get<T>()
        {
            return this.Items.OfType<T>();
        }

        private IEnumerable<IfcClassificationReference> GetIfcClassificationReferenceCollection()
        {
            return Get<IfcClassificationReference>();
        }

        private IEnumerable<IfcPropertySetTemplate> GetIfcPropertySetTemplateCollection()
        {
            return Get<IfcPropertySetTemplate>();
        }

        private Dictionary<string, IfcPropertySetTemplate> m_IfcPropertySetTemplateDictionary = new Dictionary<string, IfcPropertySetTemplate>();
        private void PopulateIfcPropertySetTemplateCollection()
        {
            if (!m_PopulateIfcPropertySetTemplate)
            {
                foreach (var ifcPropertySetTemplate in Get<IfcPropertySetTemplate>())
                {
                    m_IfcPropertySetTemplateDictionary.Add(ifcPropertySetTemplate.GlobalId, ifcPropertySetTemplate);
                }
                m_PopulateIfcPropertySetTemplate = true;
            }
        }

        public IfcPropertySetTemplate GetIfcPropertySetTemplate(string ifcGlobalId)
        {
            if (String.IsNullOrEmpty(ifcGlobalId))
                return null;

            PopulateIfcPropertySetTemplateCollection();

            Ifc4.IfcPropertySetTemplate ifcPropertySetTemplate;
            if (m_IfcPropertySetTemplateDictionary.TryGetValue(ifcGlobalId, out ifcPropertySetTemplate))
                return ifcPropertySetTemplate;

            return null;
        }

    }


}
