using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSPACES_DisplayName", "Räume")]
    public partial class CcIfcSpaces<T> : BaseObjects<T> where T : Ifc4.IfcSpace
    {
        public CcIfcSpaces(BaseObject parent)
            : base(parent)
        {
        }

        public override IEnumerable<Interfaces.IBaseObject> GetElementsEnumerator()
        {
            return Document.IfcXmlDocument.Items.OfType<T>().Where(item => item.Parent == this);
        }

        //public override bool Read(BaseObject parent)
        //{
        //    ((BaseObject)this).Parent = parent;

        //    IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)parent).Id).ToList();
        //    var entities = Document.IfcXmlDocument.Items.OfType<T>();
        //    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection.ToList())
        //    {
        //        foreach (var relatedObject in ifcRelAggregates.RelatedObjects.Items)
        //        {
        //            var entity = entities.SingleOrDefault(item => item.Id == relatedObject.Ref);
        //            if (entity == null)
        //                continue;

        //            entity.Parent = this;
        //            entity.Read(entity);
        //            ((ICollection<T>)this).Add(entity);
        //        }
        //    }
        //    return true;
        //}

        public override bool Read(BaseObject parent)
        {
            ((BaseObject)this).Parent = parent;

            foreach (T entity in Document.GetSpatialStructureChilds<T>(((Ifc4.Entity)parent).Id))
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
            if (instance != null)
                instance.InitializeAdditionalProperties();

            return instance;
        }

        public IfcSpace AddNewSpace()
        {
            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T ifcSpace = (T)AddNew();

                // ------------------------------------------------------------------------------
                IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)this.Parent).Id).ToList();
                if (ifcRelAggregatesCollection.Any())
                {
                    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                    {
                        var relatedObject = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == ifcSpace.Id);
                        if (relatedObject == null)
                        {
                            ifcRelAggregates.RelatedObjects.Items.Add(ifcSpace.RefInstance());
                        }
                    }
                }
                else
                {
                    IfcRelAggregates relAggregatesBuildingStorey = new IfcRelAggregates()
                    {
                        GlobalId = Document.GetNewGlobalId(),
                        RelatingObject = new Ifc4.IfcBuildingStorey() { Ref = ((Ifc4.Entity)this.Parent).Id }
                    };
                    relAggregatesBuildingStorey.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                    relAggregatesBuildingStorey.RelatedObjects.Items.Add(ifcSpace.RefInstance());
                    this.Document.IfcXmlDocument.Items.Add(relAggregatesBuildingStorey);
                }
                // ------------------------------------------------------------------------------
                return ifcSpace;
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
    }

}
