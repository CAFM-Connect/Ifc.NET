using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCDOCUMENTCONTAINER_DisplayName", "DocumentContainer")]
    public partial class CcIfcDocumentContainer<T> : BaseObjects<T> where T : Ifc4.IfcDocumentInformation
    {
        public CcIfcDocumentContainer(BaseObject parent)
            : base(parent)
        {
        }

        public override IEnumerable<Interfaces.IBaseObject> GetElementsEnumerator()
        {
            return Document.IfcXmlDocument.Items.OfType<T>().Where(item => item.Parent == this);
        }

        public override bool Read(BaseObject parent)
        {
            ((BaseObject)this).Parent = parent;
            foreach (T entity in Document.IfcXmlDocument.Items.OfType<T>())
            {
                if (entity == null)
                    continue;

                entity.Parent = this;
                entity.Read(entity);
                ((ICollection<T>)this).Add(entity);
            }
            return true;
        }

        public override object AddNew()
        {
            T instance = base.AddNew() as T;

            //if (instance != null)
            //    instance.InitializeAdditionalProperties();

            return instance;
        }

        public T AddNewDocumentInformation(
                                                string fullName,
                                                Ifc4.IfcClassificationReference ifcClassificationReference,
                                                Ifc4.IfcRoot ifcRoot,
                                                out bool alreadyExists)
        {
            alreadyExists = false;

            if (String.IsNullOrWhiteSpace(fullName))
                throw new ArgumentNullException(nameof(fullName));

            if (!System.IO.File.Exists(fullName))
                throw new System.IO.FileNotFoundException(nameof(fullName));

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullName);

            string zipFileLocation = Document.GetZipFileLocation(fileInfo.FullName);
            var existingDocumentInformation = Document.Project.DocumentContainer.SingleOrDefault(item => item.Location != null && item.Location.Equals(zipFileLocation, StringComparison.OrdinalIgnoreCase));
            if (existingDocumentInformation != null)
            {
                alreadyExists = true;
                return (T)existingDocumentInformation;
            }

            //<IfcExternalReferenceRelationship>
            //<RelatingReference xsi:type="IfcClassificationReference" ref="i91175"/>
            //<RelatedResourceObjects>
            //<IfcDocumentInformation ref="i91651"/>
            //</RelatedResourceObjects>
            //</IfcExternalReferenceRelationship> 

            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T ifcDocumentInformation = (T)AddNew();
                ifcDocumentInformation.InternalFullName = fileInfo.FullName;
                ifcDocumentInformation.Identification = Guid.NewGuid().ToString("N");
                ifcDocumentInformation.Name = fileInfo.Name;
                ifcDocumentInformation.CreationTime = fileInfo.CreationTime.ToString();
                ifcDocumentInformation.LastRevisionTime = fileInfo.LastWriteTime.ToString();
                ifcDocumentInformation.Location = this.Document.GetZipFileLocation(fileInfo.FullName);
                ifcDocumentInformation.ElectronicFormat = Document.GetMimeFromFile(fileInfo.FullName);
                ifcDocumentInformation.AddRelatedObject(ifcRoot);
                ifcDocumentInformation.IfcClassificationReference = ifcClassificationReference;

                return ifcDocumentInformation;

            }
            catch (Exception exc)
            {
                return default(T);
            }
            finally
            {
                BaseObject.EventsEnabled = enabledEventTypes;
            }
        }

        //<IfcExternalReferenceRelationship>
        //<RelatingReference xsi:type="IfcClassificationReference" ref="i91175"/>
        //<RelatedResourceObjects>
        //<IfcDocumentInformation ref="i91651"/>
        //</RelatedResourceObjects>
        //</IfcExternalReferenceRelationship>             

        public void AssignIfcClassification()
        {

        }

        //public override bool CanAdd
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}
    }

}
