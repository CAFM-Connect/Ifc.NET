using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public class Unit
    {
        internal Unit(Units units, string key, string displayText, bool supportedByIfc = true)
        {
            Key = key;
            DisplayText = displayText;
            Units = units;
            SupportedByIfc = supportedByIfc;
        }

        private Ifc4.Entity m_IfcUnitEntity;
        public Ifc4.Entity Entity
        {
            get
            {
                if (m_IfcUnitEntity == null)
                    m_IfcUnitEntity = this.Units.AddUnitToIfcDocument(Ifc4.Workspace.CurrentWorkspace.ActiveDocument, this);

                return m_IfcUnitEntity;
            }
        }

        public string Key { get; private set; }
        public string DisplayText{get; private set;}
        public bool Used { get; set; }
        public Units Units { get; private set; }
        public bool SupportedByIfc { get; private set; }


    }
}
