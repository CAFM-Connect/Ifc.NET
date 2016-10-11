using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    public class CcIfcRelAssociatesClassifications<T> : BaseObjects<T> where T : Ifc4.IfcRelAssociatesClassification
    {
        internal CcIfcRelAssociatesClassifications(Ifc4.Interfaces.IBaseObject parent)
            : base(parent)
        {
            this.Read();
        }

        private void Read()
        {
            ((System.Collections.IList)this).Clear();
            foreach (var item in Document.IfcXmlDocument.Items.OfType<T>())
            {
                ((Ifc4.Interfaces.IBaseObjects<T>)this).Add(item);
            }
        }
    }
}
