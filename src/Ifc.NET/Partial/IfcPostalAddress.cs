using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public partial class IfcPostalAddress
    {
        private string m_Text;
        [System.Xml.Serialization.XmlIgnore]
        [System.ComponentModel.Browsable(false)]
        public string AddressLinesAsText
        {
            get
            {
                if (this.AddressLines == null)
                    return null;

                List<string> values = new List<string>();
                foreach (var item in this.AddressLines.IfcLabelwrapper)
                {
                    if (String.IsNullOrEmpty(item.Value))
                        continue;

                    values.Add(item.Value);
                }

                m_Text = String.Join("\r\n", values.ToArray());
                return m_Text;
            }
            set
            {
                if (m_Text == value)
                    return;

                if (this.AddressLines == null)
                    this.AddressLines = new IfcPostalAddressAddressLines();
                else
                    this.AddressLines.IfcLabelwrapper.Clear();

                this.RaisePropertyChanging("AddressLinesAsText");

                if (!String.IsNullOrEmpty(value))
                {
                    foreach (var item in value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (item.Length == 0)
                            continue;
                        this.AddressLines.IfcLabelwrapper.Add(new IfcLabelwrapper() { Value = item });
                    }
                }

                if (!this.AddressLines.IfcLabelwrapper.Any())
                    this.AddressLines = null;

                this.RaisePropertyChanged("AddressLinesAsText");


            }
        }
    }

}
