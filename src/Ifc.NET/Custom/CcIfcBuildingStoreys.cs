using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGSTOREYS_DisplayName", "Etagen")]
    public partial class CcIfcBuildingStoreys<T> : BaseObjects<T> where T : Ifc4.IfcBuildingStorey
    {
        public CcIfcBuildingStoreys(BaseObject parent)
            : base(parent)
        {
        }

        public override IEnumerable<Interfaces.IBaseObject> GetElementsEnumerator()
        {
            return Document.IfcXmlDocument.Items.OfType<T>().Where(item => item.Parent == this);
        }

        public override object AddNew()
        {
            T instance = base.AddNew() as T;
            if (instance != null)
                instance.InitializeAdditionalProperties();

            return instance;
        }

        //public override bool Read_old(BaseObject parent)
        //{
        //    ((BaseObject)this).Parent = parent;

        //    IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)parent).Id);
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
        //            entity.Spaces.Read(entity);
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
                entity.Spaces.Read(entity);
            }
            return true;
        }


        public IfcBuildingStorey AddNewBuildingStorey()
        {
            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T ifcBuildingStorey = (T)AddNew();

                // ------------------------------------------------------------------------------
                IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)this.Parent).Id).ToList();
                if (ifcRelAggregatesCollection.Any())
                {
                    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                    {
                        var relatedObject = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == ifcBuildingStorey.Id);
                        if (relatedObject == null)
                        {
                            ifcRelAggregates.RelatedObjects.Items.Add(ifcBuildingStorey.RefInstance());
                        }
                    }
                }
                else
                {
                    IfcRelAggregates relAggregatesBuilding = new IfcRelAggregates()
                    {
                        GlobalId = Document.GetNewGlobalId(),
                        RelatingObject = new Ifc4.IfcBuilding() { Ref = ((Ifc4.Entity)this.Parent).Id }
                    };
                    relAggregatesBuilding.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                    relAggregatesBuilding.RelatedObjects.Items.Add(ifcBuildingStorey.RefInstance());
                    this.Document.IfcXmlDocument.Items.Add(relAggregatesBuilding);
                }
                // ------------------------------------------------------------------------------
                return ifcBuildingStorey;
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

        public override bool CanAdd
        {
            get
            {
                return true;
            }
        }

        public override Type GetAddObjectType()
        {
            return typeof(Ifc4.IfcSpace);
        }

    }

}
