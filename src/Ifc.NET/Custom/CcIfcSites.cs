using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCSITES_DisplayName", "Liegenschaften")]
    public partial class CcIfcSites<T> : BaseObjects<T> where T : Ifc4.IfcSite
    {
        public CcIfcSites(BaseObject parent)
            : base(parent)
        {
        }

        public override IEnumerable<Interfaces.IBaseObject> GetElementsEnumerator()
        {
            return Document.IfcXmlDocument.Items.OfType<T>().Where(item => item.Parent == this);
        }

        //public override bool Read_old(BaseObject parent)
        //{
        //    ((BaseObject)this).Parent = parent;

        //    IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)parent).Id).ToList();
        //    var entities = Document.IfcXmlDocument.Items.OfType<T>();
        //    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
        //    {
        //        foreach (var relatedObject in ifcRelAggregates.RelatedObjects.Items)
        //        {
        //            var entity = entities.SingleOrDefault(item => item.Id == relatedObject.Ref);
        //            if(entity != null))
        //            {
        //                //entity.ParentIBaseObject = this;
        //                entity.Parent = this;

        //                ((ICollection<T>)this).Add(entity);
        //                entity.Buildings.Read(entity);
        //            }
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

                //entity.ParentIBaseObject = this;
                entity.Parent = this;

                ((ICollection<T>)this).Add(entity);
                entity.Buildings.Read(entity);
            }

            return true;
        }


        public override bool CanAdd
        {
            get
            {
                return true;
            }
        }

        public T AddNewSite()
        {
            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T ifcSite = (T)AddNew();
                // ------------------------------------------------------------------------------
                IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)this.Parent).Id).ToList();
                if (ifcRelAggregatesCollection.Any())
                {
                    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                    {
                        var relatedObject = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == ifcSite.Id);
                        if (relatedObject == null)
                        {
                            ifcRelAggregates.RelatedObjects.Items.Add(ifcSite.RefInstance());
                        }
                    }
                }
                else
                {
                    IfcRelAggregates relAggregatesProject = new IfcRelAggregates()
                    {
                        GlobalId = Document.GetNewGlobalId(),
                        RelatingObject = new Ifc4.IfcProject() { Ref = ((Ifc4.Entity)this.Parent).Id }
                    };
                    relAggregatesProject.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                    relAggregatesProject.RelatedObjects.Items.Add(ifcSite.RefInstance());
                    this.Document.IfcXmlDocument.Items.Add(relAggregatesProject);
                }
                // ------------------------------------------------------------------------------
                return ifcSite;
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
