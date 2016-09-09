using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc.NET
{
    public partial class IfcProject
    {
        public IfcProject()
        {
        }

        private CcIfcSites<Ifc.NET.IfcSite> m_Sites;
        [System.Xml.Serialization.XmlIgnore]
        public CcIfcSites<Ifc.NET.IfcSite> Sites
        {
            get
            {
                if (m_Sites == null)
                {
                    m_Sites = new CcIfcSites<Ifc.NET.IfcSite>(this);
                }
                return m_Sites;
            }
        }

        private CcFacilities<Ifc.NET.CcFacility> m_Facilities;
        [System.Xml.Serialization.XmlIgnore]
        public CcFacilities<Ifc.NET.CcFacility> Facilities
        {
            get
            {
                if (m_Facilities == null)
                {
                    m_Facilities = new CcFacilities<CcFacility>(this);
                }
                return m_Facilities;
            }
        }

        public override bool Read(BaseObject baseObject)
        {
            this.Parent = baseObject;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            this.Sites.Read(this);
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Sites.Read: " + sw.ElapsedMilliseconds + "ms");

            sw.Reset();
            sw.Start();
            //this.Facilities.Read(this, false);
            this.Facilities.Read(this, true);
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Facilities.Read: " + sw.ElapsedMilliseconds + "ms");

            return true;
        }

    }

}
