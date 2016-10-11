using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4.CustomModel
{
    public class CustomPropertyDescriptor : System.ComponentModel.PropertyDescriptor
    {
        private CustomProperty m_CustomProperty;
        private CustomTypeDescriptorBag m_CustomTypeDescriptor;

        public CustomPropertyDescriptor(CustomProperty customProperty, CustomTypeDescriptorBag customTypeDescriptor)
            : base(customProperty.Name, customProperty.Attributes)
        {
            m_CustomProperty = customProperty;
            m_CustomTypeDescriptor = customTypeDescriptor;

            this.IfcPropertyName = customProperty.IfcPropertyName;
            this.IfcPropertyGlobalId = customProperty.IfcPropertyGlobalId;
        }

        public static T CustomChangeType<T>(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                    return default(T);

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return (T)Convert.ChangeType(value, conversionType);
        }

        public CustomModel.CustomProperty PropertyItem
        {
            get { return m_CustomProperty; }
        }

        public override string Name
        {
            get { return m_CustomProperty.Name; }
            //get { return m_CustomProperty.DisplayName; }
        }

        public override bool CanResetValue(object component)
        {
			if (m_CustomProperty.DefaultValue == null)
				return false;

			return !this.GetValue(component).Equals(m_CustomProperty.DefaultValue);
        }

        public override Type ComponentType
        {
            get { return m_CustomProperty.GetType(); }
        }

        private Exception m_LastValueError;
        public Exception LastValueError
        {
            get { return m_LastValueError; }
        }

        public void ResetLastValueError()
        {
            m_LastValueError = null;
        }

        public override object GetValue(object component)
        {
            m_LastValueError = null;

            System.Reflection.MethodInfo methodInfo = typeof(Ifc4.CcFacility).GetMethod("GetValue");
            System.Reflection.MethodInfo generic = methodInfo.MakeGenericMethod(this.PropertyType);

            try
            {
                object value = generic.Invoke(((Ifc4.CcFacility)component), new object[] { this });
                return value;
            }
            catch (Exception exc)
            {
                m_LastValueError = exc;
                return null;
            }
        }

        public override void SetValue(object component, object value)
        {
            m_LastValueError = null;

            System.Reflection.MethodInfo methodInfo = typeof(Ifc4.CcFacility).GetMethod("SetValue");
            System.Reflection.MethodInfo generic = methodInfo.MakeGenericMethod(this.PropertyType);

            try
            {
                generic.Invoke(((Ifc4.CcFacility)component), new object[] { this, value });
            }
            catch (Exception exc)
            {
                m_LastValueError = exc;
            }
        }

        public string ToString(string arg)
        {
            if (arg == "I")
            {
                string displayName;
                if (PropertyItem != null)
                    displayName = PropertyItem.DisplayName;
                else
                    displayName = this.DisplayName;

                var ifcPropertyName = String.IsNullOrEmpty(this.IfcPropertyName) == null ? "<Empty>": this.IfcPropertyName;
                return String.Format("Property: {0} / IfcPropertyName: {1}", displayName, ifcPropertyName);
            }

            return base.ToString();
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
                throw new NotImplementedException();
            }
        }

        public override Type PropertyType
        {
            get
            {
                return m_CustomProperty.PropertyType;
                throw new NotImplementedException();
            }
        }

        public override void ResetValue(object component)
        {
            SetValue(component, m_CustomProperty.DefaultValue);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
            throw new NotImplementedException();
        }

        //private Dictionary<string, object> m_DynamicProperties = new Dictionary<string, object>();
        //public Dictionary<string, object> DynamicProperties { get; private set; }

        public string IfcPropertyName
        {
            get;
            set;
        }

        public string IfcPropertyGlobalId
        {
            get;
            set;
        }

    }

    public class CustomPropertyDescriptorCollection : System.ComponentModel.PropertyDescriptorCollection
    {
        public CustomPropertyDescriptorCollection(System.ComponentModel.PropertyDescriptor[] properties)
            : base(properties)
        {
        }

        public CustomPropertyDescriptorCollection(System.ComponentModel.PropertyDescriptor[] properties, bool readOnly)
            : base(properties, readOnly)
        {
        }
    }
        
    public class CustomTypeDescriptionProvider : System.ComponentModel.TypeDescriptionProvider
    {
        private System.ComponentModel.TypeDescriptionProvider m_DefaultTypeProvider = null;
        private Dictionary<int, System.ComponentModel.ICustomTypeDescriptor> m_ICustomTypeDescriptorCollectionByHashCode;

        public CustomTypeDescriptionProvider(Type type)
            : base(System.ComponentModel.TypeDescriptor.GetProvider(type))
        {
            m_DefaultTypeProvider = System.ComponentModel.TypeDescriptor.GetProvider(type);
            m_ICustomTypeDescriptorCollectionByHashCode = new Dictionary<int, System.ComponentModel.ICustomTypeDescriptor>();
        }

        public void Reset(object instance)
        {
            if (instance != null)
            {
                m_ICustomTypeDescriptorCollectionByHashCode.Remove(instance.GetHashCode());
            }
        }

        public override System.ComponentModel.ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            System.ComponentModel.ICustomTypeDescriptor defaultTypeDescriptor = base.GetTypeDescriptor(objectType, instance);
            if (instance == null)
                return defaultTypeDescriptor;

            System.ComponentModel.ICustomTypeDescriptor customTypeDescriptor;
            if (m_ICustomTypeDescriptorCollectionByHashCode.TryGetValue(instance.GetHashCode(), out customTypeDescriptor))
                return customTypeDescriptor;

            customTypeDescriptor = new CustomTypeDescriptorBag(defaultTypeDescriptor, instance);
            m_ICustomTypeDescriptorCollectionByHashCode.Add(instance.GetHashCode(), customTypeDescriptor);

            //if (instance != null)
            //    System.Diagnostics.Debug.WriteLine("HashCode: " + instance.GetHashCode());

            return customTypeDescriptor;
        }
    }

    public delegate void PropertyEventHandler(object sender, PropertyEventArgs e);
    public delegate void PropertyValidatingEventHandler(object sender, PropertyValidatingEventArgs e);

    public class PropertyEventArgs : System.EventArgs
    {
        public PropertyEventArgs(CustomProperty customProperty, object value)
        {
            CustomProperty = customProperty;
            Value = value;
        }
        public CustomProperty CustomProperty { get; private set; }
        public object Value { get; set; }
    }

    public class PropertyValidatingEventArgs : PropertyEventArgs
    {
        public PropertyValidatingEventArgs(CustomProperty customProperty, object value)
            : base(customProperty, value)
        {
        }
        public bool Cancel { get; set; }
        public string Message { get; set; }
    }

    public class CustomTypeDescriptorBag : System.ComponentModel.CustomTypeDescriptor
    {
        public event PropertyEventHandler GetValue;
        public event PropertyEventHandler SetValue;

        //public event XtElementPropertyValidatingEventHandler ValidatingValue;

        //public delegate void BeforePropertyValueChangedEventHandler(object sender, PropertyEventHandler e);
        //public event BeforePropertyValueChangedEventHandler BeforePropertyValueChanged;

        //public delegate void AfterPropertyValueChangedEventHandler(object sender, PropertyEventHandler e);
        //public event AfterPropertyValueChangedEventHandler AfterPropertyValueChanged;

        private Ifc4.CcFacility m_Facility = null;
        internal CustomTypeDescriptorBag(System.ComponentModel.ICustomTypeDescriptor parent, object instance)
            : base(parent)
        {
            m_Facility = (Ifc4.CcFacility)instance;
        }

        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        private List<CustomModel.CustomPropertyDescriptor> GetRuntimeProperties(Ifc4.BaseObject baseObject)
        {
            CustomModel.CustomPropertyDescriptor cpd;
            CustomModel.CustomProperty customProperty;
            List<CustomModel.CustomPropertyDescriptor> propertyDescriptorCollection = new List<CustomModel.CustomPropertyDescriptor>();

            Ifc4.CcFacility facility = baseObject as Ifc4.CcFacility;
            if (facility == null || String.IsNullOrEmpty(facility.ObjectTypeId))
                return propertyDescriptorCollection;

            Ifc4.Document document = baseObject.GetParent<Ifc4.Document>();

            IEnumerable<Ifc4.IfcPropertySetTemplate> ifcPropertySetTemplateCollection = document.GetIfcPropertySetTemplateCollection(facility);

            ////string objectTypeId = null;
            ////if (facility != null && !String.IsNullOrEmpty(facility.ObjectTypeId))
            ////{
            ////    objectTypeId = facility.ObjectTypeId;
            ////    IEnumerable<Ifc4.IfcRelAssociatesClassification> ifcRelAssociatesClassificationCollection = document.IfcXmlDocument.Items.OfType<Ifc4.IfcRelAssociatesClassification>();
            ////    IEnumerable<string> relatedObjectsRefs = from ifcRelAssociatesClassification in ifcRelAssociatesClassificationCollection
            ////                                             from relatedObject in ifcRelAssociatesClassification.RelatedObjects.Items
            ////                                             where ifcRelAssociatesClassification.RelatingClassification.Item != null && ifcRelAssociatesClassification.RelatingClassification.Item.Ref == objectTypeId
            ////                                             select relatedObject.Ref;

            ////    ifcPropertySetTemplateCollection = document.IfcXmlDocument.Items.OfType<Ifc4.IfcPropertySetTemplate>().Where(item => relatedObjectsRefs.Contains(item.Id));
            ////}

            if(ifcPropertySetTemplateCollection == null)
                return propertyDescriptorCollection;


            foreach (var ifcPropertySetTemplate in ifcPropertySetTemplateCollection)
            {
                foreach (var propertyTemplate in ifcPropertySetTemplate.HasPropertyTemplates.Items)
                {
                    Ifc4.IfcSimplePropertyTemplate simplePropertyTemplate = propertyTemplate as Ifc4.IfcSimplePropertyTemplate;

                    if (simplePropertyTemplate == null)
                        continue;

                    //string displayName = String.IsNullOrEmpty(simplePropertyTemplate.PrimaryMeasureType) ? simplePropertyTemplate.Name : String.Format("{0} [{1}]", simplePropertyTemplate.Name, simplePropertyTemplate.PrimaryMeasureType);
                    string displayName = simplePropertyTemplate.Name;

                    string unit = null;
                    if (simplePropertyTemplate.PrimaryUnit != null)
                    {
                        if (simplePropertyTemplate.PrimaryUnit.Item is Ifc4.IfcSIUnit)
                        {
                            Ifc4.IfcSIUnit ifcSIUnit = GetUnit((Ifc4.IfcSIUnit)simplePropertyTemplate.PrimaryUnit.Item);    
                            if (ifcSIUnit.NameSpecified)
                            {
                                if (ifcSIUnit.PrefixSpecified)
                                {
                                    if (ifcSIUnit.Prefix == IfcSIPrefix.Centi && ifcSIUnit.Name == IfcSIUnitName.Metre)
                                        unit = "[cm]";
                                    else if (ifcSIUnit.Prefix == IfcSIPrefix.Milli && ifcSIUnit.Name == IfcSIUnitName.Metre)
                                        unit = "[mm]";
                                    else if (ifcSIUnit.Prefix == IfcSIPrefix.Kilo && ifcSIUnit.Name == IfcSIUnitName.Gram)
                                        unit = "[kg]";
                                    else if (ifcSIUnit.Prefix == IfcSIPrefix.Kilo && ifcSIUnit.Name == IfcSIUnitName.Watt)
                                        unit = "[kW]";
                                }
                                else
                                {
                                    if (ifcSIUnit.Name == IfcSIUnitName.Metre)
                                        unit = "[m]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.SquareMetre)
                                        unit = "[m²]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.CubicMetre)
                                        unit = "[m³]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.Volt)
                                        unit = "[V]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.DegreeCelsius)
                                        unit = "[°C]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.Ampere)
                                        unit = "[A]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.Hertz)
                                        unit = "[Hz]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.Watt)
                                        unit = "[W]";
                                    else if (ifcSIUnit.Name == IfcSIUnitName.Pascal)
                                        unit = "[Pa]";
                                }
                            }
                        }
                        else if (simplePropertyTemplate.PrimaryUnit.Item is Ifc4.IfcDerivedUnit)
                        {
                            Ifc4.IfcDerivedUnit ifcDerivedUnit = GetUnit<Ifc4.IfcDerivedUnit>((Ifc4.IfcDerivedUnit)simplePropertyTemplate.PrimaryUnit.Item);    
                            if (ifcDerivedUnit.UnitTypeSpecified)
                            {
                                if (ifcDerivedUnit.UnitType == IfcDerivedUnitEnum.Heatfluxdensityunit)
                                    unit = "[Ah]";
                                else if (ifcDerivedUnit.UnitType == IfcDerivedUnitEnum.Volumetricflowrateunit)
                                    unit = "[m³/h]";
                                else if (ifcDerivedUnit.UnitType == IfcDerivedUnitEnum.Massdensityunit)
                                    unit = "[kg/m³]";
                                else if (ifcDerivedUnit.UnitType == IfcDerivedUnitEnum.Soundpowerlevelunit)
                                    unit = "[db]";
                            }
                        }
                        if (!String.IsNullOrEmpty(unit))
                            displayName = String.Concat(displayName, " ", unit);

                    }

                    List<Attribute> attributes = new List<Attribute>()
                    {
                        new System.ComponentModel.BrowsableAttribute(true),
                        new System.ComponentModel.CategoryAttribute("Runtime Object"),
                        new System.ComponentModel.DisplayNameAttribute(displayName)
                    };


                    CustomPropertyStandardValues customPropertyStandardValues = null;
                    Type customPropertyType;

                    string primaryMeasureType = simplePropertyTemplate.PrimaryMeasureType;

                    if (simplePropertyTemplate.TemplateType == Ifc4.IfcSimplePropertyTemplateTypeEnum.PSinglevalue)
                    {
                        // default
                        customPropertyType = typeof(System.String);

                        // overwrite 
                        if (!String.IsNullOrEmpty(primaryMeasureType))
                        {
                            if (primaryMeasureType.Equals("IfcText", StringComparison.OrdinalIgnoreCase))
                            {
                                customPropertyType = typeof(System.String);
                            }
                            else if (primaryMeasureType.Equals("IfcDate", StringComparison.OrdinalIgnoreCase))
                            {
                                customPropertyType = typeof(System.Nullable<DateTime>);
                                attributes.Add(new System.ComponentModel.TypeConverterAttribute(typeof(CustomNullableTypeConverter<DateTime?>)));
                            }
                            else if (primaryMeasureType.Equals("IfcReal", StringComparison.OrdinalIgnoreCase))
                            {
                                customPropertyType = typeof(System.Nullable<double>);
                                attributes.Add(new System.ComponentModel.TypeConverterAttribute(typeof(CustomNullableTypeConverter<double?>)));
                            }
                            else if (primaryMeasureType.Equals("IfcBoolean", StringComparison.OrdinalIgnoreCase))
                            {
                                customPropertyType = typeof(System.String);
                                attributes.Add(new System.ComponentModel.TypeConverterAttribute(typeof(CustomBooleanNullableTypeConverter)));
                            }
                        }
                    }
                    else if (simplePropertyTemplate.TemplateType == Ifc4.IfcSimplePropertyTemplateTypeEnum.PEnumeratedvalue)
                    {
                        customPropertyStandardValues = new CustomPropertyStandardValues();
                        attributes.Add(new System.ComponentModel.TypeConverterAttribute(typeof(CustomEnumTypeConverter)));

                        int i = 0;
                        // add empty enum value
                        if (simplePropertyTemplate.Enumerators.EnumerationValues.Items.Any())
                        {
                            // empty string not possible - use a string with length > 0 
                            customPropertyStandardValues.Add(new CustomPropertyStandardValue("E" + (i++), " "));
                        }

                        foreach (var enumItem in simplePropertyTemplate.Enumerators.EnumerationValues.Items)
                        {
                            string enumValue;
                            if (enumItem is Ifc4.IfcLabelwrapper)
                            {
                                Ifc4.IfcLabelwrapper labelWrapper = enumItem as Ifc4.IfcLabelwrapper;
                                enumValue = labelWrapper.Value;
                            }
                            else
                            {
                                enumValue = enumItem.ToString();
                            }

                            customPropertyStandardValues.Add(new CustomPropertyStandardValue("E" + (i++), enumValue.Trim()));
                        }

                        customPropertyType = typeof(System.Enum);
                    }
                    else
                    {
                        customPropertyType = typeof(System.String);
                    }

                    // ------------------------------------------------------------------
                    StringBuilder sbAttributeDescription = new StringBuilder();
                    sbAttributeDescription.AppendLine("");
                    if (String.IsNullOrEmpty(primaryMeasureType))
                    {
                        sbAttributeDescription.AppendLine(String.Format("IFC4 Type: {0}", "unknown"));
                    }
                    else
                    {
                        sbAttributeDescription.AppendLine(String.Format("IFC4 Type: {0}", primaryMeasureType));
                    }

                    if (customPropertyType.IsGenericType && customPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        sbAttributeDescription.AppendLine(String.Format(".NET Type: Nullable {0}", (customPropertyType.GetGenericArguments()[0]).Name));
                    }
                    else
                    {
                        sbAttributeDescription.AppendLine(String.Format(".NET Type: {0}", customPropertyType.Name));
                    }
                    

                    if (!String.IsNullOrEmpty(simplePropertyTemplate.Description))
                    {
                        sbAttributeDescription.AppendLine("");
                        sbAttributeDescription.AppendLine(simplePropertyTemplate.Description);
                    }

                    if(sbAttributeDescription.Length > 0)
                    {
                        attributes.Add(new System.ComponentModel.DescriptionAttribute(sbAttributeDescription.ToString()));
                    }
                    // ------------------------------------------------------------------

                    customProperty = new CustomModel.CustomProperty
                    (
                        null,
                        simplePropertyTemplate.TempId.ToString("N"),
                        simplePropertyTemplate.TempId.ToString("N"),
                        customPropertyType,
                        simplePropertyTemplate.Name,
                        simplePropertyTemplate.Description,
                        attributes,
                        customPropertyStandardValues,
                        null
                    );

                    customProperty.IfcPropertyName = simplePropertyTemplate.Name;
                    customProperty.IfcPropertyGlobalId = simplePropertyTemplate.GlobalId;

                    CustomModel.CustomAssembly customAssembly = new CustomAssembly();
                    Type type = customAssembly.CreateDynamicEnum(customProperty);
                    if (type != null)
                        customProperty.PropertyType = type;

                    cpd = new CustomModel.CustomPropertyDescriptor(customProperty, this);
                    propertyDescriptorCollection.Add(cpd);
                }

            }

            return propertyDescriptorCollection;
        }

        private T GetUnit<T>(T unit) where T : Entity
        {
            if(unit == null)
                return null;

            if(unit.IsRef)
                unit = (T)unit.Document.Project.UnitsInContext.Units.Items.FirstOrDefault(item => item.Id == unit.Ref);

            return unit;
        }

        private System.ComponentModel.PropertyDescriptorCollection m_PropertyDescriptorCollection;
        public override System.ComponentModel.PropertyDescriptorCollection GetProperties()
        {
            if (m_PropertyDescriptorCollection != null)
                return m_PropertyDescriptorCollection;

            System.ComponentModel.PropertyDescriptorCollection originalProperties = base.GetProperties();
            List<System.ComponentModel.PropertyDescriptor> newProperties = new List<System.ComponentModel.PropertyDescriptor>();
            foreach (System.ComponentModel.PropertyDescriptor pd in originalProperties)
                newProperties.Add(pd);

            // System.Diagnostics.Debug.WriteLine(" *** ID: " + m_Facility.TempId.ToString());

            if (m_Facility != null && !String.IsNullOrEmpty(m_Facility.ObjectTypeId))
            {
                var c = GetRuntimeProperties(m_Facility);
                newProperties.AddRange(c);
                m_PropertyDescriptorCollection = new System.ComponentModel.PropertyDescriptorCollection(newProperties.ToArray(), true);
                return m_PropertyDescriptorCollection;
            }
            
            return new System.ComponentModel.PropertyDescriptorCollection(newProperties.ToArray(), true);

        }

        public /*virtual*/ void OnGetValue(PropertyEventArgs e)
        {
            if (GetValue != null)
            {
                // ------------------------------------
                // automatische Lösung
                object value;
                if (GetProperty(e.CustomProperty.Instance, e.CustomProperty.Name, out value))
                {
                    e.Value = value;
                    return;
                }
                // ------------------------------------
                GetValue(this, e);
            }
        }

        public /*virtual*/ void OnSetValue(PropertyEventArgs e)
        {

            // --------------------------------------------------------------
            //if (ValidatingValue != null)
            //{
            //    XtElementPropertyValidatingEventArgs validatingEventArgs = new PropertyValidatingEventArgs(e.PropertyGridItem, e.Value);
            //    ValidatingValue(this, validatingEventArgs);
            //    if (validatingEventArgs.Cancel)
            //        return;
            //}
            // --------------------------------------------------------------

            //if (BeforePropertyValueChanged != null) // steuert transactions
            //    BeforePropertyValueChanged(this, e);

            if (SetValue != null)
            {

                // ------------------------------------
                // automatische Lösung
                SetProperty(e.CustomProperty.Instance, e.CustomProperty.Name, e.Value);
                // ------------------------------------
                // Überschrieben Funktion auch noch überprüfen
                SetValue(this, e);

            }

            //if (AfterPropertyValueChanged != null) // steuert transactions
            //    AfterPropertyValueChanged(this, e);

        }

        public bool SetProperty(object instance, string propertyName, object newValue)
        {
            System.Reflection.PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                return false;

            newValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
            if (instance is Ifc4.BaseObject)
                ((Ifc4.BaseObject)instance).Document.SetDirty();

            propertyInfo.SetValue(instance, newValue, new object[] { });
            return true;
        }

        public bool GetProperty(object instance, string propertyName, out object value)
        {
            value = null;
            System.Reflection.PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                return false;

            value = propertyInfo.GetValue(instance, new object[] { });
            return true;
        }



    }



}
