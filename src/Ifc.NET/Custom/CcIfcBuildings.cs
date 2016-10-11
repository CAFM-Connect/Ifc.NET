using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_IFCBUILDINGS_DisplayName", "Gebäude")]
    public partial class CcIfcBuildings<T> : BaseObjects<T> where T : Ifc4.IfcBuilding
    {
        public CcIfcBuildings(BaseObject parent)
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

            foreach (T entity in Document.GetSpatialStructureChilds<T>(((Ifc4.Entity)parent).Id))
            {
                if (entity == null)
                    continue;

                entity.Parent = this;
                entity.Read(entity);

                ((ICollection<T>)this).Add(entity);
                entity.BuildingStoreys.Read(entity);
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

        public IfcBuilding AddNewBuilding()
        {
            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T ifcBuilding = (T)AddNew();
                // ------------------------------------------------------------------------------
                IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.Entity)this.Parent).Id).ToList();
                if (ifcRelAggregatesCollection.Any())
                {
                    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                    {
                        var relatedObject = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == ifcBuilding.Id);
                        if (relatedObject == null)
                        {
                            ifcRelAggregates.RelatedObjects.Items.Add(ifcBuilding.RefInstance());
                        }
                    }
                }
                else
                {
                    IfcRelAggregates relAggregatesSite = new IfcRelAggregates()
                    {
                        GlobalId = Document.GetNewGlobalId(),
                        RelatingObject = new Ifc4.IfcSite() { Ref = ((Ifc4.Entity)this.Parent).Id }
                    };
                    relAggregatesSite.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                    relAggregatesSite.RelatedObjects.Items.Add(ifcBuilding.RefInstance());
                    this.Document.IfcXmlDocument.Items.Add(relAggregatesSite);
                }
                // ------------------------------------------------------------------------------

                return ifcBuilding;
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
    }

}
