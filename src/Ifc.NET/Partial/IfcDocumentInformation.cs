using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ifc4
{
    public partial class IfcDocumentInformation
    {
        public override bool CanRemove
        {
            get { return true; }
        }

        [System.Xml.Serialization.XmlIgnore]
        [System.ComponentModel.Browsable(false)]
        public string InternalFullName { get; internal set; }

        private IfcClassificationReference m_IfcClassificationReference;
        [System.Xml.Serialization.XmlIgnore()]
        [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCDOCUMENTINFORMATION_PROPERTY_IFCCLASSIFICATIONREFERENCE_DISPLAYNAME", "Dokumenttyp")]
        public IfcClassificationReference IfcClassificationReference
        {
            get
            {
                if (m_IfcClassificationReference == null)
                    m_IfcClassificationReference = this.GetIfcClassificationReferenceFromIfcExternalReferenceRelationship();
                return m_IfcClassificationReference;
            }
            set
            {
                if (m_IfcClassificationReference != value)
                {
                    string propertyName = this.PropertyName(() => this.IfcClassificationReference);

                    RaisePropertyChanging(propertyName);

                    DeleteIfcExternalReferenceRelationship();

                    m_IfcClassificationReference = value;

                    UpdateIfcExternalReferenceRelationship(propertyName, m_IfcClassificationReference);

                    RaisePropertyChanged(propertyName);
                }
            }
        }

        private void UpdateIfcExternalReferenceRelationship(string propertyName, IfcClassificationReference ifcClassificationReference)
        {
            if (ifcClassificationReference == null)
            {
                // delete
                DeleteIfcExternalReferenceRelationship();
                return;
            }

            var existingIfcExternalReferenceRelationshipCollection = GetIfcExternalReferenceRelationshipCollection(ifcClassificationReference).ToList();
            if (existingIfcExternalReferenceRelationshipCollection.Any())
            {
                // update
                foreach (var ifcExternalReferenceRelationship in existingIfcExternalReferenceRelationshipCollection)
                {
                    ifcExternalReferenceRelationship.RelatedResourceObjects.Items.Add(this.RefInstance<IfcDocumentInformation>());
                }
            }
            else
            {
                // add
                IfcExternalReferenceRelationship ifcExternalReferenceRelationship = new IfcExternalReferenceRelationship();
                ifcExternalReferenceRelationship.RelatedResourceObjects = new IfcExternalReferenceRelationshipRelatedResourceObjects();
                ifcExternalReferenceRelationship.RelatedResourceObjects.Items.Add(this.RefInstance<IfcDocumentInformation>());
                ifcExternalReferenceRelationship.RelatingReference = ifcClassificationReference.RefInstance<IfcClassificationReference>();
                this.Document.IfcXmlDocument.Items.Add(ifcExternalReferenceRelationship);
            }
        }

        private void DeleteIfcExternalReferenceRelationship()
        {
            Ifc4.Document document = this.Document;
            List<IfcExternalReferenceRelationship> removeIfcExternalReferenceRelationshipCollection = new List<IfcExternalReferenceRelationship>();
            foreach (var ifcExternalReferenceRelationship in document.IfcXmlDocument.Items.OfType<Ifc4.IfcExternalReferenceRelationship>())
            {
                if (ifcExternalReferenceRelationship.RelatedResourceObjects != null)
                {
                    ifcExternalReferenceRelationship.RelatedResourceObjects.Items.RemoveAll(item => item.Ref == this.Id);
                    if (!ifcExternalReferenceRelationship.RelatedResourceObjects.Items.Any())
                        removeIfcExternalReferenceRelationshipCollection.Add(ifcExternalReferenceRelationship);
                }
            }
            foreach (var ifcExternalReferenceRelationship in removeIfcExternalReferenceRelationshipCollection)
            {
                document.IfcXmlDocument.Items.Remove(ifcExternalReferenceRelationship);
            }
        }

        private IEnumerable<IfcExternalReferenceRelationship> GetIfcExternalReferenceRelationshipCollection(IfcClassificationReference ifcClassificationReference)
        {
            Ifc4.Document document = this.Document;
            return from ifcExternalReferenceRelationship in document.IfcXmlDocument.Items.OfType<Ifc4.IfcExternalReferenceRelationship>()
                   where ifcExternalReferenceRelationship.RelatingReference != null &&
                   ifcExternalReferenceRelationship.RelatingReference.Ref == ifcClassificationReference.Id
                   select ifcExternalReferenceRelationship;
        }

        private Ifc4.IfcClassificationReference GetIfcClassificationReferenceFromIfcExternalReferenceRelationship()
        {
            Ifc4.Document document = this.Document;
            IfcClassificationReference ifcClassificationReference = (from ifcExternalReferenceRelationship in document.IfcXmlDocument.Items.OfType<Ifc4.IfcExternalReferenceRelationship>()
                                                                     where ifcExternalReferenceRelationship.RelatedResourceObjects != null &&
                                                                     ifcExternalReferenceRelationship.RelatedResourceObjects.Items.Exists(item => item.Ref == this.Id) &&
                                                                     ifcExternalReferenceRelationship.RelatingReference is IfcClassificationReference
                                                                     select ifcExternalReferenceRelationship.RelatingReference).FirstOrDefault() as IfcClassificationReference;

            if (ifcClassificationReference != null && ifcClassificationReference.IsRef)
                ifcClassificationReference = this.Document.IfcXmlDocument.Items.OfType<IfcClassificationReference>().FirstOrDefault(item => item.Id == ifcClassificationReference.Ref);

            return ifcClassificationReference;
        }

        public void AddRelatedObject<T>(T t) where T : IfcRoot
        {
            if (t == null)
                return;

            Ifc4.Document document = this.Document;
            var ifcRelAssociatesDocumentCollection = (from ifcRelAssociatesDocument in document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesDocument>()
                                                      where ifcRelAssociatesDocument.RelatingDocument?.Item?.Ref == this.Id
                                                      select ifcRelAssociatesDocument).ToList();

            if (ifcRelAssociatesDocumentCollection.Any())
            {
                // IfcRelAssociatesDocument already exists
                foreach (var ifcRelAssociatesDocument in ifcRelAssociatesDocumentCollection)
                {
                    // add related object if not exists
                    if (
                            ifcRelAssociatesDocument.RelatedObjects != null &&
                            !ifcRelAssociatesDocument.RelatedObjects.Items.Exists(item => item.Ref == t.Id)
                        )
                    {
                        ifcRelAssociatesDocument.RelatedObjects.Items.Add(t.RefInstance<T>());
                    }
                }
            }
            else
            {
                // add new IfcRelAssociatesDocument
                IfcRelAssociatesDocument ifcRelAssociatesDocument = new IfcRelAssociatesDocument()
                {
                    GlobalId = Document.GetNewGlobalId(),
                    RelatingDocument = new IfcRelAssociatesDocumentRelatingDocument()
                    {
                        Item = this.RefInstance<IfcDocumentInformation>()
                    }
                };
                ifcRelAssociatesDocument.RelatedObjects = new IfcRelAssociatesRelatedObjects();
                ifcRelAssociatesDocument.RelatedObjects.Items.Add(t.RefInstance<T>());
                this.Document.IfcXmlDocument.Items.Add(ifcRelAssociatesDocument);
            }

        }

    }
}
