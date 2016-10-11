using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcRoot
    {
        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.ReadOnly(true)]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCROOT_PROPERTY_UNIQUEIDS_DisplayName", "IFC GUID / GUID")]
        public string UniqueIds
        {
            get
            {
                if(!String.IsNullOrEmpty(this.GlobalId))
                    return String.Format("{{{0}}} | {{{1}}}", this.GlobalId, Ifc4.GlobalId.ConvertFromIfcGUID(this.GlobalId));

                return String.Empty;
            }
        }

        private List<string> m_IfcDocumentInformationRefs;
        public bool HasDocumentInformations(out List<IfcDocumentInformation> ifcDocumentInformationCollection)
        {
            m_IfcDocumentInformationRefs = new List<string>();
            CheckDocumentInformations(this);
            ifcDocumentInformationCollection = this.Document.IfcXmlDocument.Items.OfType<Ifc4.IfcDocumentInformation>().Where(item => m_IfcDocumentInformationRefs.Contains(item.Id)).ToList();
            return ifcDocumentInformationCollection.Any();
        }

        private void CheckDocumentInformations(Ifc4.IfcRoot ifcRoot)
        {
            if (ifcRoot == null)
                return;

            foreach (var ifcRelAssociatesDocument in this.Document.GetIfcRelAssociatesDocumentCollection(ifcRoot))
            {
                if (ifcRelAssociatesDocument.RelatingDocument != null && ifcRelAssociatesDocument.RelatingDocument.Item != null)
                    m_IfcDocumentInformationRefs.Add(ifcRelAssociatesDocument.RelatingDocument.Item.Ref);

                if (ifcRelAssociatesDocument.RelatedObjects == null)
                {
                    foreach (var item in ifcRelAssociatesDocument.RelatedObjects.Items)
                        CheckDocumentInformations(item);
                }
            }
        }


        public void AddIfcDocumentInformation(IfcDocumentInformation ifcDocumentInformation, bool overwriteIfExists = false)
        {
            if (ifcDocumentInformation == null)
                return;

            //<IfcRelAssociatesDocument GlobalId="1wbA9QmR90bwuS__laYrEh">
            //  <RelatedObjects>
            //    <IfcProject ref="i1" xsi:nil="true" />
            //  </RelatedObjects>
            //  <RelatingDocument>
            //    <IfcDocumentInformation ref="i3302" xsi:nil="true" />
            //  </RelatingDocument>
            //</IfcRelAssociatesDocument>

            // ------------------------------------------------------------------------------
            var ifcRelAssociatesDocumentCollection = (from ifcRelAssociatesDocument in this.Document.IfcXmlDocument.Items.OfType<IfcRelAssociatesDocument>()
                                                     where ifcRelAssociatesDocument.RelatingDocument != null &&
                                                           ifcRelAssociatesDocument.RelatingDocument.Item != null &&
                                                           ifcRelAssociatesDocument.RelatingDocument.Item.Ref == ifcDocumentInformation.Id
                                                     select ifcRelAssociatesDocument).ToList();

            if (ifcRelAssociatesDocumentCollection.Any())
            {
                foreach (var ifcRelAssociatesDocument in ifcRelAssociatesDocumentCollection)
                {
                    if (ifcRelAssociatesDocument.RelatedObjects == null)
                        ifcRelAssociatesDocument.RelatedObjects = new IfcRelAssociatesRelatedObjects();

                    var releatedObjectsItem = ifcRelAssociatesDocument.RelatedObjects.Items.FirstOrDefault(item => item.Ref == this.Id);
                    if (releatedObjectsItem == null)
                        ifcRelAssociatesDocument.RelatedObjects.Items.Add(this.RefInstance<IfcRoot>());
                }
            }
            else
            {
                IfcRelAssociatesDocument ifcRelAssociatesDocument = new IfcRelAssociatesDocument()
                {
                    GlobalId = Document.GetNewGlobalId(),
                    RelatingDocument = new IfcRelAssociatesDocumentRelatingDocument()
                    {
                        Item = ifcDocumentInformation.RefInstance<IfcDocumentInformation>()
                    }
                };
                ifcRelAssociatesDocument.RelatedObjects = new IfcRelAssociatesRelatedObjects();
                ifcRelAssociatesDocument.RelatedObjects.Items.Add(this.RefInstance<IfcRoot>());
                this.Document.IfcXmlDocument.Items.Add(ifcRelAssociatesDocument);
            }
        }



    }



}
