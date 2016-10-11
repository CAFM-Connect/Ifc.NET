using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4
{
    [Serializable]
    public class CcIfcOrganizations<T> : BaseObjects<T> where T : Ifc4.IfcOrganization
    {
        internal CcIfcOrganizations(Ifc4.Interfaces.IBaseObject parent)
            : base(parent)
        {
            this.Read();
        }

        //public override object AddNew()
        //{
        //    // generic
        //    T instance = Activator.CreateInstance<T>() as T;
        //    ((Ifc4.Interfaces.IBaseObjects<T>)this).Add(instance);
        //    Document.IfcXmlDocument.Items.Add(instance);
        //    return instance;

        //    //IfcOrganization ifcOrganization = new IfcOrganization();
        //    //((Ifc4.Interfaces.IBaseObjects<T>)this).Add(ifcOrganization);
        //    //Document.IfcXmlDocument.Items.Add(ifcOrganization);
        //    //return instance;

        //}

        //public T AddNewOrganization()
        //{
        //    return (T)AddNew();
        //}

        private void Read()
        {
            // yield
            ((System.Collections.IList)this).Clear();
            foreach (var item in Document.IfcXmlDocument.Items.OfType<T>())
            {
                ((Ifc4.Interfaces.IBaseObjects<T>)this).Add(item);
            }
        }

    }

}
