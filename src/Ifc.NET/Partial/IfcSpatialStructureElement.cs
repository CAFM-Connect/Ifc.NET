using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcSpatialStructureElement
    {
        private List<CcFacility> m_Facilities;
        public bool HasFacilities(out List<CcFacility> facilities)
        {
            m_Facilities = new List<CcFacility>();
            CheckFacilities(this.Document.Project.Facilities);
            facilities = new List<CcFacility>(m_Facilities);
            return facilities.Any();
        }

        private void CheckFacilities(CcFacilities<CcFacility> facilities)
        {
            foreach (var facility in facilities)
            {
                if (this.Equals(facility.Location))
                    m_Facilities.Add(facility);

                CheckFacilities(facility.Facilities);
            }
        }

    }
}
