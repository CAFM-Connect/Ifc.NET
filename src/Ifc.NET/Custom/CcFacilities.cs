using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    [Ifc4.Attributes.CustomDisplayNameAttribute("CLASS_CCFACILITIES_DISPLAYNAME", "Anlagen")]
    public class CcFacilities<T> : BaseObjects<T> where T : Ifc4.CcFacility
    {
        public CcFacilities(BaseObject parent)
            : base(parent)
        {
        }

        public CcFacility GetCcFacilityFromTempId(Guid guid)
        {
            if (guid.Equals(Guid.Empty))
                return null;

            return Descendants(this).FirstOrDefault(item => item.TempId.Equals(guid));
        }

        private IEnumerable<CcFacility> Descendants(IEnumerable<CcFacility> facilities)
        {
            foreach (var facility in facilities)
            {
                yield return facility;
                foreach (var subFacility in Descendants(facility.Facilities))
                    yield return subFacility;
            }
        }

        public IEnumerable<CcFacility> Descendants()
        {
            return Descendants(this);
        }

        public override IEnumerable<Interfaces.IBaseObject> GetElementsEnumerator()
        {
            return Document.IfcXmlDocument.Items.OfType<T>().Where(item => item.Parent == this);
        }

        // the 1st IfcSystem level is always related to the project

        //<IfcRelAggregates GlobalId="3rWB5ezmH1TBP5fYFzYyD4">
        //  <RelatingObject xsi:type="IfcProject" ref="i100" xsi:nil="true" />
        //  <RelatedObjects>
        //    <IfcSite ref="i130" xsi:nil="true" />
        //    <IfcSystem ref="i138" xsi:nil="true" />
        //  </RelatedObjects>
        //</IfcRelAggregates>

        public override bool Read(BaseObject parent, bool recursive = true)
        {
            ((BaseObject)this).Parent = parent;

            string Ref = String.Empty;

            if (this.Parent.GetType() == typeof(Ifc4.IfcProject))
            {
                //<IfcRelAggregates GlobalId="3rWB5ezmH1TBP5fYFzYyD4">
                //  <RelatingObject xsi:type="IfcProject" ref="i100" xsi:nil="true" />
                //  <RelatedObjects>
                //    <IfcSite ref="i130" xsi:nil="true" />
                //    <IfcProxy ref="i138" xsi:nil="true" />
                //  </RelatedObjects>
                //</IfcRelAggregates>

                Ref = ((Ifc4.IfcProject)this.Parent).Id;

                IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == Ref);
                var ifcSystemCollection = Document.IfcXmlDocument.Items.OfType<Ifc4.IfcSystem>().ToList();
                List<Entity> loadedProjectRelatedObjects = new List<Entity>();
                foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                {
                    foreach (var relatedObject in ifcRelAggregates.RelatedObjects.Items.OfType<IfcSystem>())
                    {
                        var ifcSystem = ifcSystemCollection.FirstOrDefault(item => item.Id == relatedObject.Ref);
                        if (ifcSystem != null && !loadedProjectRelatedObjects.Contains(ifcSystem))
                        {
                            loadedProjectRelatedObjects.Add(ifcSystem);

                            var facility = (T)AddNew();
                            ifcSystem.Parent = facility;
                            ifcSystem.ParentIBaseObject = facility;
                            facility.IfcSystem = ifcSystem;
                            if (recursive)
                            {
                                facility.Facilities.Read(facility, recursive);
                            }
                        }
                    }
                }
            }
            if (this.Parent.GetType() == typeof(Ifc4.CcFacility))
            {
                if (((Ifc4.CcFacility)this.Parent).IfcObjectDefinition != null)
                {
                    Ref = ((Ifc4.CcFacility)this.Parent).IfcObjectDefinition.Id;

                    IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == Ref);
                    IEnumerable<IfcObjectDefinition> ifcObjectDefinitionCollection = Document.IfcXmlDocument.Items.OfType<IfcObjectDefinition>();
                    foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                    {
                        foreach (var relatedObject in ifcRelAggregates.RelatedObjects.Items)
                        {
                            var ifcObjectDefinition = ifcObjectDefinitionCollection.FirstOrDefault(item => item.Id == relatedObject.Ref);
                            if (ifcObjectDefinition != null)
                            {
                                ifcObjectDefinition.Parent = this;
                                var facility = (T)AddNew();
                                facility.IfcObjectDefinition = ifcObjectDefinition;
                                if (recursive)
                                    facility.Facilities.Read(facility, recursive);
                            }
                        }
                    }
                }
                else if (((Ifc4.CcFacility)this.Parent).IfcSystem != null)
                {
                    Ref = ((Ifc4.CcFacility)this.Parent).IfcSystem.Id;

                    IEnumerable<IfcRelAssignsToGroup> ifcRelAssignsToGroupCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAssignsToGroup>().Where(item => item.RelatingGroup != null && item.RelatingGroup.Ref == Ref);
                    IEnumerable<IfcObjectDefinition> ifcObjectDefinitionCollection = Document.IfcXmlDocument.Items.OfType<IfcObjectDefinition>();
                    foreach (IfcRelAssignsToGroup ifcRelAssignsToGroup in ifcRelAssignsToGroupCollection)
                    {
                        foreach (var relatedObject in ifcRelAssignsToGroup.RelatedObjects.Items)
                        {
                            var ifcObjectDefinition = ifcObjectDefinitionCollection.FirstOrDefault(item => item.Id == relatedObject.Ref);
                            if (ifcObjectDefinition != null)
                            {
                                ifcObjectDefinition.Parent = this;
                                var facility = (T)AddNew();
                                facility.IfcObjectDefinition = ifcObjectDefinition;
                                if (recursive)
                                    facility.Facilities.Read(facility, recursive);
                            }
                        }
                    }
                }

            }
            return true;
        }

        public T AddNewSystem(Ifc4.IfcObjectDefinition relatingObject)
        {
            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T facility = (T)AddNew();

                var ifcSystem = new IfcSystem();
                // ifcSystem.Parent = this;
                ifcSystem.Parent = facility;
                ifcSystem.ParentIBaseObject = facility;
                facility.IfcSystem = ifcSystem;

                var document = this.GetParent<Ifc4.Document>();
                if (document != null)
                {
                    Entity entity = ifcSystem as Entity;
                    if (entity != null)
                    {
                        entity.Id = document.GetNextSid();
                        document.IfcXmlDocument.Items.Add(entity);
                    }
                }
                // ------------------------------------------------------------------------------
                string Ref = String.Empty;
                if (relatingObject == null)
                    Ref = document.Project.Id;
                else
                    Ref = relatingObject.Id;


                //<IfcRelAggregates id="i84" GlobalId="3SVa7UPpXAux$wEdBKgcDI">
                //    <RelatingObject xsi:type="IfcProject" xsi:nil="true" ref="i100"/>
                //    <RelatedObjects>
                //        <IfcSite ref="i83" xsi:nil="true"/> 
                //        <IfcSystem ref="i2501" xsi:nil="true"/>  <!-- 431 Lüftungsanlagen -->
                //        <IfcSystem ref="i2508" xsi:nil="true"/>  <!-- 461 Aufzugsanlagen -->
                //    </RelatedObjects>
                //</IfcRelAggregates>

                if (relatingObject != null)
                {
                    IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>().Where(item => item.RelatingObject != null && item.RelatingObject.Ref == Ref).ToList();
                    if (ifcRelAggregatesCollection.Any())
                    {
                        foreach (var ifcRelAggregates in ifcRelAggregatesCollection)
                        {
                            var relatedObject = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == ifcSystem.Id);
                            if (relatedObject == null)
                            {
                                ifcRelAggregates.RelatedObjects.Items.Add(ifcSystem.RefInstance());
                                // add only to one IfcRelAggregates - RelatedObjects List - RelatingObject xsi:type="IfcProject"
                                break;
                            }
                        }
                    }
                    else
                    {
                        var relAggregatesProject = new IfcRelAggregates()
                        {
                            GlobalId = Document.GetNewGlobalId(),
                            RelatingObject = new Ifc4.IfcProject() { Ref = Ref }
                        };
                        relAggregatesProject.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                        relAggregatesProject.RelatedObjects.Items.Add(ifcSystem.RefInstance());
                        this.Document.IfcXmlDocument.Items.Add(relAggregatesProject);
                    }
                }
                return facility;
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

        public override object AddNew()
        {
            T instance = base.AddNew() as T;
            if (instance != null)
                instance.InitializeAdditionalProperties();

            return instance;
        }

        public T AddNewFacility(CcFacility cloneFacility = null, bool oneToOneCopy = false)
        {
            EventType enabledEventTypes = BaseObject.EventsEnabled;
            BaseObject.EventsEnabled = EventType.None;
            try
            {
                T facility = (T)AddNew();
                // ------------------------------------------------------------------------------
                // Annahme Anlage = IfcBuildingElementProxy
                IfcBuildingElementProxy ifcBuildingElementProxy = new IfcBuildingElementProxy();

                facility.IfcObjectDefinition = ifcBuildingElementProxy;
                ifcBuildingElementProxy.Parent = this;

                Ifc4.Document document = this.GetParent<Ifc4.Document>();
                if (document != null)
                {
                    Entity entity = ifcBuildingElementProxy as Entity;
                    if (entity != null)
                    {
                        entity.Id = document.GetNextSid();
                        document.IfcXmlDocument.Items.Add(entity);
                    }
                    IfcRoot ifcRoot = ifcBuildingElementProxy as IfcRoot;
                    if (ifcRoot != null)
                    {
                        ifcRoot.GlobalId = document.GetNewGlobalId();
                    }
                }
                // ------------------------------------------------------------------------------
                if (this.Parent.GetType() == typeof(Ifc4.IfcProject))
                {

                    IEnumerable<IfcRelAggregates> ifcRelAggregatesCollection = this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>()
                                                                                .Where(item => item.RelatingObject != null && item.RelatingObject.Ref == ((Ifc4.IfcProject)this.Parent).Id).ToList();
                    if (ifcRelAggregatesCollection.Any())
                    {
                        foreach (IfcRelAggregates ifcRelAggregates in ifcRelAggregatesCollection)
                        {
                            var relatedObject = ifcRelAggregates.RelatedObjects.Items.FirstOrDefault(item => item.Ref == ifcBuildingElementProxy.Id);
                            if (relatedObject == null)
                            {
                                ifcRelAggregates.RelatedObjects.Items.Add(ifcBuildingElementProxy.RefInstance());
                            }
                        }
                    }
                    else
                    {
                        IfcRelAggregates relAggregatesProject = new IfcRelAggregates()
                        {
                            GlobalId = Document.GetNewGlobalId(),
                            // kann aktuell nur IfcProject sein
                            RelatingObject = new Ifc4.IfcProject() { Ref = ((Ifc4.IfcProject)this.Parent).Id }
                        };
                        relAggregatesProject.RelatedObjects = new IfcRelAggregatesRelatedObjects();
                        relAggregatesProject.RelatedObjects.Items.Add(ifcBuildingElementProxy.RefInstance());
                        this.Document.IfcXmlDocument.Items.Add(relAggregatesProject);
                    }
                }
                // ------------------------------------------------------------------------------
                if (this.Parent.GetType() == typeof(Ifc4.CcFacility))
                {
                    if (((Ifc4.CcFacility)this.Parent).IfcObjectDefinition != null)
                    {
                        Ifc4.IfcRelAggregates ifcRelAggregates = ((Ifc4.CcFacility)this.Parent).GetIfcRelAggregates();
                        ifcRelAggregates.RelatedObjects.Items.Add(ifcBuildingElementProxy.RefInstance());
                    }
                    else if (((Ifc4.CcFacility)this.Parent).IfcSystem != null)
                    {
                        Ifc4.IfcRelAssignsToGroup ifcRelAssignsToGroup = ((Ifc4.CcFacility)this.Parent).GetIfcRelAssignsToGroup();
                        ifcRelAssignsToGroup.RelatedObjects.Items.Add(ifcBuildingElementProxy.RefInstance());
                    }
                }
                // ------------------------------------------------------------------------------
                // assign facility properties from clipboard facility
                facility.AssignPropertiesFromFacility(cloneFacility, oneToOneCopy);
                // ------------------------------------------------------------------------------

                return facility;
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
