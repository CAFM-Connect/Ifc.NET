using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ifc.NET
{
    public partial class IfcPropertySet
    {

        public IEnumerable<T> GetIfcPropertyCollection<T>() where T : Ifc.NET.IfcProperty
        {
            if (this.HasProperties == null)
                yield break;

            IEnumerable<T> ifcPropertyCollection = null;

            foreach (Ifc.NET.IfcProperty ifcPropertyTmp in this.HasProperties.Items)
            {
                Ifc.NET.IfcProperty ifcProperty = null;
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
        //public IEnumerable<T> GetIfcPropertyCollection<T>() where T : Ifc.NET.IfcProperty
        //{
        //    if(this.HasProperties != null)
        //        yield break;

        //    IEnumerable<T> ifcPropertyCollection = null;

        //    foreach (Ifc.NET.IfcProperty ifcPropertyTmp in this.HasProperties.Items)
        //    {
        //        Ifc.NET.IfcProperty ifcProperty = null;
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

        //        if (ifcProperty != null && ifcProperty is Ifc.NET.IfcPropertySingleValue)
        //        {
        //            Ifc.NET.IfcPropertySingleValue ifcPropertySingleValue = (Ifc.NET.IfcPropertySingleValue)ifcProperty;
        //            var nominalValue = ( Ifc.NET.IfcPropertySingleValueNominalValue)ifcPropertySingleValue.NominalValue;
        //            if (nominalValue != null && nominalValue.Item is Ifc.NET.IfcLabelwrapper)
        //                yield return (T)ifcProperty;
        //        }
        //        else if (ifcProperty != null && ifcProperty is Ifc.NET.IfcPropertyEnumeratedValue)
        //        {
        //            Ifc.NET.IfcPropertyEnumeratedValue ifcPropertyEnumeratedValue = (Ifc.NET.IfcPropertyEnumeratedValue)ifcProperty;
        //            var ifcPropertyEnumeratedValueEnumerationValues = (Ifc.NET.IfcPropertyEnumeratedValueEnumerationValues)ifcPropertyEnumeratedValue.EnumerationValues;
        //            if (ifcPropertyEnumeratedValueEnumerationValues != null)
        //            {
        //                foreach(var enumeratedValue in ifcPropertyEnumeratedValueEnumerationValues.Items)
        //                {
        //                    if (enumeratedValue is Ifc.NET.IfcLabelwrapper)
        //                        yield return (T)ifcProperty;

        //                    break; // no flag
        //                }
        //            }
        //        }
        //    }
        //}

    }
}
