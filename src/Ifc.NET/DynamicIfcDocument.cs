using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using IFC4 = Ifc.NET;

namespace Ifc.NET
{

    // http://msdn.microsoft.com/en-us/data/jj613116.aspx

    public partial class IfcXML : Uos
    {
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "schemaLocation", Namespace = System.Xml.Schema.XmlSchema.InstanceNamespace)]
        public string xsiSchemaLocation = @"http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd";

        private Dictionary<string, IfcClassificationReference> m_IfcClassificationReferenceFromIfcPropertySetTemplateId = new Dictionary<string, IfcClassificationReference>();

        private bool m_PopulateIfcPropertySetTemplate = false;

        public bool TryGetIfcClassificationReferenceFromIfcPropertySetTemplate(string ifcGlobalId, out Ifc.NET.IfcClassificationReference ifcClassificationReference)
        {
            Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate = GetIfcPropertySetTemplate(ifcGlobalId);
            ifcClassificationReference = GetIfcClassificationReferenceFromIfcPropertySetTemplate(ifcPropertySetTemplate);
            return (ifcClassificationReference != null);
        }

        private Ifc.NET.IfcClassificationReference GetIfcClassificationReferenceFromIfcPropertySetTemplate(Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate)
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
            Ifc.NET.IfcClassificationReference ifcClassificationReference;

            if (ifcPropertySetTemplate == null)
                return null;

            if (m_IfcClassificationReferenceFromIfcPropertySetTemplateId.TryGetValue(ifcPropertySetTemplate.Id, out ifcClassificationReference))
                return ifcClassificationReference;

            IEnumerable<Ifc.NET.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = Get<Ifc.NET.IfcRelAssociatesClassification>();
            var relatingClassification = (from ifcRelAssociatesClassification in ifcRelAssociatesClassificationCollection
                                          from relatedObjects in ifcRelAssociatesClassification.RelatedObjects.Items.OfType<Ifc.NET.IfcPropertySetTemplate>()
                                          where relatedObjects.Ref == ifcPropertySetTemplate.Id
                                          select ifcRelAssociatesClassification.RelatingClassification.Item).FirstOrDefault();

            ifcClassificationReference = relatingClassification as Ifc.NET.IfcClassificationReference;
            if (ifcClassificationReference != null)
            {
                ifcClassificationReference = Get<Ifc.NET.IfcClassificationReference>().FirstOrDefault(item => item.Id == ifcClassificationReference.Ref);
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

            Ifc.NET.IfcPropertySetTemplate ifcPropertySetTemplate;
            if (m_IfcPropertySetTemplateDictionary.TryGetValue(ifcGlobalId, out ifcPropertySetTemplate))
                return ifcPropertySetTemplate;

            return null;
        }

    }

    public abstract partial class Entity : Ifc.NET.BaseObject 
    {

        private bool? m_Nil;

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "nil", Namespace = System.Xml.Schema.XmlSchema.InstanceNamespace)]
        public bool Nil
        {
            get
            {
                if(m_Nil.HasValue)
                    return m_Nil.Value;

                return !String.IsNullOrWhiteSpace(Ref);
            }
            set
            {
                if (this.m_Nil.HasValue && this.m_Nil.Value != value)
                {
                    this.RaisePropertyChanging("Nil");
                    this.m_Nil = value;
                    this.RaisePropertyChanged("Nil");
                }
            }
        }


        [System.ComponentModel.Browsable(false)]
        public bool NilSpecified
        {
            get { return this.Nil; }
        }

        //public bool ShouldSerializeNil
        //{
        //    get { return Nil; }
        //}


        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.Browsable(false)]
        public bool IsRef
        {
            get { return !String.IsNullOrEmpty(this.Ref); }
        }

        //public Entity Clone()
        //{
        //    return this.MemberwiseClone() as Entity;
        //}



    }

}
