using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc.NET
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

        private Ifc.NET.Entity m_IfcUnitEntity;
        public Ifc.NET.Entity Entity
        {
            get
            {
                if (m_IfcUnitEntity == null)
                    m_IfcUnitEntity = this.Units.AddUnitToIfcDocument(Ifc.NET.Workspace.CurrentWorkspace.ActiveDocument, this);

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
