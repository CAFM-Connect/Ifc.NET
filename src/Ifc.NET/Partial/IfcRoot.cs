using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc.NET
{
    public partial class IfcRoot
    {
        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.ReadOnly(true)]
        [Ifc.NET.Attributes.CustomDisplayNameAttribute("CLASS_IFCROOT_PROPERTY_UNIQUEIDS_DisplayName", "IFC GUID / GUID")]
        public string UniqueIds
        {
            get
            {
                if(!String.IsNullOrEmpty(this.GlobalId))
                    return String.Format("{{{0}}} | {{{1}}}", this.GlobalId, Ifc.NET.GlobalId.ConvertFromIfcGUID(this.GlobalId));

                return String.Empty;
            }
        }

        //[System.Xml.Serialization.XmlIgnore()]
        //[System.ComponentModel.ReadOnly(true)]
        //public Guid GlobalIdAsNetGuid
        //{
        //    get
        //    {
        //        if (!String.IsNullOrEmpty(this.GlobalId))
        //            return Ifc.NET.GlobalId.ConvertFromIfcGUID(this.GlobalId);

        //        return Guid.Empty;
        //    }
        //}

    }
}
