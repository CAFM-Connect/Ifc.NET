using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcProject
    {
        public IfcProject()
        {
        }

        private CcIfcSites<Ifc4.IfcSite> m_Sites;
        [System.Xml.Serialization.XmlIgnore]
        public CcIfcSites<Ifc4.IfcSite> Sites
        {
            get
            {
                if (m_Sites == null)
                {
                    m_Sites = new CcIfcSites<Ifc4.IfcSite>(this);
                }
                return m_Sites;
            }
        }

        private CcFacilities<Ifc4.CcFacility> m_Facilities;
        [System.Xml.Serialization.XmlIgnore]
        public CcFacilities<Ifc4.CcFacility> Facilities
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
