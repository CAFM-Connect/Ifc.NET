using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc.NET
{
    public abstract partial class IfcObjectDefinition
    {
        public Ifc.NET.IfcPropertySet GetIfcPropertySetFromRelatingPropertyDefinition()
        {
            Ifc.NET.Document document = this.Document;

            //<IfcRelDefinesByProperties id="i98" GlobalId="0DZQ9OC1z3EwnCxq2V5i7q" Name="PropertyContainer" Description="BuildingContainerForPropertySet">
            //    <RelatedObjects xsi:type="IfcBuilding" ref="i85"/>
            //    <RelatingPropertyDefinition>
            //        <IfcPropertySet xsi:nil="true" ref="i97"/>
            //    </RelatingPropertyDefinition>
            //</IfcRelDefinesByProperties>

            IfcPropertySet ifcPropertySet = (from ifcRelDefinesByProperties in document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcRelDefinesByProperties>()
                                             where ifcRelDefinesByProperties.RelatedObjects != null &&
                                                     ifcRelDefinesByProperties.RelatedObjects.Ref == this.Id &&
                                                     ifcRelDefinesByProperties.RelatingPropertyDefinition != null &&
                                                     ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcPropertySet
                                             select ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).FirstOrDefault() as IfcPropertySet;

            if (ifcPropertySet != null && ifcPropertySet.IsRef)
            {
                ifcPropertySet = document.IfcXmlDocument.Items.OfType<IfcPropertySet>().FirstOrDefault(item => item.Id == ifcPropertySet.Ref);
            }

            return ifcPropertySet;
        }

        private static Dictionary<string, Ifc.NET.IfcElementQuantity> m_IfcElementQuantities;

        public Ifc.NET.IfcElementQuantity GetIfcElementQuantityFromRelatingPropertyDefinition()
        {
            Ifc.NET.Document document = this.Document;
            m_IfcElementQuantities = null;

            IfcElementQuantity ifcElementQuantity = (from ifcRelDefinesByProperties in document.IfcXmlDocument.Items.OfType<Ifc.NET.IfcRelDefinesByProperties>()
                                                     where ifcRelDefinesByProperties.RelatedObjects != null &&
                                                             ifcRelDefinesByProperties.RelatedObjects.Ref == this.Id &&
                                                             ifcRelDefinesByProperties.RelatingPropertyDefinition != null &&
                                                             ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcElementQuantity
                                                     select ifcRelDefinesByProperties.RelatingPropertyDefinition.Item).FirstOrDefault() as IfcElementQuantity;

            if (ifcElementQuantity != null && ifcElementQuantity.IsRef)
            {
                ifcElementQuantity = document.IfcXmlDocument.Items.OfType<IfcElementQuantity>().FirstOrDefault(item => item.Id == ifcElementQuantity.Ref);
            }

            return ifcElementQuantity;
        }


        public IfcObjectDefinition RefInstance()
        {
            var instance = Activator.CreateInstance(this.GetType());
            var property = instance.GetType().GetProperty("Ref");
            property.SetValue(instance, this.Id, new object[] { });
            return instance as IfcObjectDefinition;
        }

    }

}
