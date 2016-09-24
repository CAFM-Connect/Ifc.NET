using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ifc4
{
    public partial class IfcPropertySet
    {

        public IEnumerable<T> GetIfcPropertyCollection<T>() where T : Ifc4.IfcProperty
        {
            if (this.HasProperties == null)
                yield break;

            IEnumerable<T> ifcPropertyCollection = null;

            foreach (Ifc4.IfcProperty ifcPropertyTmp in this.HasProperties.Items)
            {
                Ifc4.IfcProperty ifcProperty = null;
                if (ifcPropertyTmp.IsRef)
                {
                    if (ifcPropertyCollection == null)
                        ifcPropertyCollection = this.Document.IfcXmlDocument.Items.OfType<T>();

                    ifcProperty = ifcPropertyCollection.FirstOrDefault(item => item.Id == ifcPropertyTmp.Ref);
                }
                else
                {
                    ifcProperty = ifcPropertyTmp;
                }

                yield return (T)ifcProperty;

            }
        }
        //public IEnumerable<T> GetIfcPropertyCollection<T>() where T : Ifc4.IfcProperty
        //{
        //    if(this.HasProperties != null)
        //        yield break;

        //    IEnumerable<T> ifcPropertyCollection = null;

        //    foreach (Ifc4.IfcProperty ifcPropertyTmp in this.HasProperties.Items)
        //    {
        //        Ifc4.IfcProperty ifcProperty = null;
        //        if (ifcPropertyTmp.IsRef)
        //        {
        //            if(ifcPropertyCollection == null)
        //                ifcPropertyCollection = this.Document.IfcXmlDocument.Items.OfType<T>();

        //            ifcProperty = ifcPropertyCollection.SingleOrDefault(item => item.Id == ifcPropertyTmp.Ref);
        //        }
        //        else
        //        {
        //            ifcProperty = ifcPropertyTmp;
        //        }

        //        if (ifcProperty != null && ifcProperty is Ifc4.IfcPropertySingleValue)
        //        {
        //            Ifc4.IfcPropertySingleValue ifcPropertySingleValue = (Ifc4.IfcPropertySingleValue)ifcProperty;
        //            var nominalValue = ( Ifc4.IfcPropertySingleValueNominalValue)ifcPropertySingleValue.NominalValue;
        //            if (nominalValue != null && nominalValue.Item is Ifc4.IfcLabelwrapper)
        //                yield return (T)ifcProperty;
        //        }
        //        else if (ifcProperty != null && ifcProperty is Ifc4.IfcPropertyEnumeratedValue)
        //        {
        //            Ifc4.IfcPropertyEnumeratedValue ifcPropertyEnumeratedValue = (Ifc4.IfcPropertyEnumeratedValue)ifcProperty;
        //            var ifcPropertyEnumeratedValueEnumerationValues = (Ifc4.IfcPropertyEnumeratedValueEnumerationValues)ifcPropertyEnumeratedValue.EnumerationValues;
        //            if (ifcPropertyEnumeratedValueEnumerationValues != null)
        //            {
        //                foreach(var enumeratedValue in ifcPropertyEnumeratedValueEnumerationValues.Items)
        //                {
        //                    if (enumeratedValue is Ifc4.IfcLabelwrapper)
        //                        yield return (T)ifcProperty;

        //                    break; // no flag
        //                }
        //            }
        //        }
        //    }
        //}

    }
}
