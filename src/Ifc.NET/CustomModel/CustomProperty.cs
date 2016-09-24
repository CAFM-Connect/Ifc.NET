using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4.CustomModel
{

    public class CustomPropertyStandardValue
    {
        public CustomPropertyStandardValue(string key, string displayText)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Argument 'key' is null or empty.", "key");

            if (String.IsNullOrEmpty(displayText))
                displayText = key;

            Key = key;
            OrginalText = displayText;
            DisplayText = displayText;
        }

        public string Key { get; set; }
        public string OrginalText { get; set; }
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return DisplayText;
        }

    }
    public class CustomPropertyStandardValues : List<CustomPropertyStandardValue>
    {
        public CustomPropertyStandardValues()
        {
        }
        public CustomPropertyStandardValue AddValue(string key, string displayText)
        {
            if (key == null)
                key = displayText;

            CustomPropertyStandardValue standardValue = new CustomPropertyStandardValue(key, displayText);
            this.Add(standardValue);
            return standardValue;
        }
    }

	public class CustomProperty
	{
		public CustomProperty(object instance, string key, string propertyName, Type type, string displayName, string description)

            : this(instance, key, propertyName, type, displayName, description, null, null, null)
		{
		}

		public CustomProperty(object instance, string key, string propertyName, Type type, string displayName, string description, List<Attribute> attributes, CustomPropertyStandardValues standardValues)
            : this(instance, key, propertyName, type, displayName, description, attributes, standardValues, null)
		{
		}

		public CustomProperty(object instance, string key, string propertyName, Type type, string displayName, string description, List<Attribute> attributeList, CustomPropertyStandardValues standardValues,  object[] args)
		{
			DisplayBold = false;
			Instance = instance;
			Key = key;
			Name = propertyName;
            DisplayName = displayName;
            TypeName = type.AssemblyQualifiedName;
            PropertyType = type;
			Category = "UNKNOWN";
			Description = description;
			DefaultValue = null;

            IfcPropertyName = String.Empty;
            IfcPropertyGlobalId = String.Empty;
			
			ComboboxValues = null;
            if (standardValues != null)
            {
                ComboboxValues = new CustomPropertyStandardValues();
                ComboboxValues.AddRange(standardValues);
            }

			Args = args;

			if (attributeList == null)
				attributeList = new List<Attribute>();

			IEnumerable<Attribute> query = attributeList.Where(e => e is System.ComponentModel.EditorAttribute);
			
			// Falls kein TypeEditor angemeldet ist
			// den korrekten Typeeditor anmelden

			if (query.Count() == 0)
			{
				// Falls String
				if (type == typeof(System.String))
				{
					IEnumerable<Attribute> query1 = attributeList.Where(e => e is System.ComponentModel.TypeConverterAttribute || e is System.ComponentModel.EditorAttribute);
					if (query1.Count() == 0)
					{
						// gilt für alle String properties
						// Standard TextEditor benutzen
						string editorTypeName = "";
						attributeList.Add(new System.ComponentModel.EditorAttribute(editorTypeName, typeof(System.Drawing.Design.UITypeEditor)));
					}
				}

				// falls nicht string
				else
				{
				}

			}
			else
			{
				foreach(System.ComponentModel.EditorAttribute editorAttribute in query)
				{
					if (editorAttribute.EditorTypeName == "")
						CanOnlyEditByExternalDialog = true;
				}
			}
			// ----------------------------------------------------------------------------
			if (type == typeof(Boolean))
			{
				// wird nur für die Übersetzung benötigt
				if (!(attributeList.Exists(item => item is System.ComponentModel.TypeConverterAttribute)))
				{
					string typeConverter = "";
					attributeList.Add(new System.ComponentModel.TypeConverterAttribute(typeConverter));
				}
			}


            // ----------------------------------------------------------------------------
            // TODOJV zur Zeit kein Multiselect
            // MergableAttribute auf false setzen
            // kein MultiSelect erlaubt
            System.ComponentModel.MergablePropertyAttribute mergablePropertyAttribute;
            mergablePropertyAttribute = attributeList.Find(item => item.GetType() == typeof(System.ComponentModel.MergablePropertyAttribute)) as System.ComponentModel.MergablePropertyAttribute;
            bool updateMergableAttribute = false;
            bool allowMerge = false;
            if (mergablePropertyAttribute == null)
            {
                updateMergableAttribute = true;
                allowMerge = false;
            }
            if (updateMergableAttribute)
                attributeList.Add(new System.ComponentModel.MergablePropertyAttribute(allowMerge));
            // ----------------------------------------------------------------------------
             
			Attributes = attributeList.ToArray();
			
		}

		public bool DisplayBold { get; set; }
		public object Instance { get; private set; }
        public string Key { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string TypeName { get; set; }
        public string Category { get; set; }
        public Type PropertyType { get; set; }
		public string Description { get; set; }
		public object DefaultValue { get; set; }
		public CustomPropertyStandardValues ComboboxValues { get; private set; }
		public Attribute[] Attributes { get; set; }
		public object[] Args { get; private set; }
		public bool CanOnlyEditByExternalDialog { get; protected set; }

        public string IfcPropertyName { get; set; }
        public string IfcPropertyGlobalId { get; set; }

	}

}

