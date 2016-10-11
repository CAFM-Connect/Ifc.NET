using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ifc4.CustomModel
{
    public class CustomLongNullableTypeConverter : System.ComponentModel.TypeConverter
    {
        public CustomLongNullableTypeConverter()
        {
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.String))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(System.String))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            string newValue = value.ToString().Trim();
            if (newValue.Length == 0)
                return null;

            if (IsDigitsOnly(newValue))
                return newValue;

            throw new FormatException("Die Eingabezeichenfolge hat das falsche Format.");
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value.ToString().Trim().Length == 0)
                return null;

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class CustomBooleanNullableTypeConverter : System.ComponentModel.TypeConverter
    {

        string[] trueArray = new string[] { Boolean.TrueString, "Wahr", "Ja", "1" };
        string[] falseArray = new string[] { Boolean.FalseString, "Falsch", "Nein", "0" };

        public CustomBooleanNullableTypeConverter()
        {
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            string newValue = value.ToString().Trim();
            if (newValue.Length == 0)
                return null;

            if (trueArray.Union(falseArray).Contains(newValue, new CustomCompare()))
                return newValue;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Die Eingabezeichenfolge hat das falsche Format.");
            sb.AppendLine("Zulässige Werte.");
            sb.AppendLine(String.Format("'True' = {0}", String.Join("; ", trueArray)));
            sb.AppendLine(String.Format("'False' = {0}", String.Join("; ", falseArray)));

            throw new FormatException(sb.ToString());
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            string newValue = value.ToString().Trim();
            if (trueArray.Union(falseArray).Contains(newValue, new CustomCompare()))
                return newValue;

            return base.ConvertFrom(context, culture, value);
        }

        class CustomCompare : IEqualityComparer<string>
        {
            public bool Equals(string a, string b)
            {
                return a.Equals(b, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return 0;
            }
        }
    }

    public class CustomNullableTypeConverter<T> : System.ComponentModel.TypeConverter
    {
        public CustomNullableTypeConverter()
        {
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.String))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(System.String))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value.GetType() == typeof(System.String))
            {
                if (value.ToString().Trim().Length == 0)
                    return null;

                if (typeof(T) == typeof(double?))
                {
                    double result;
                    if (Double.TryParse(value.ToString().Trim(), System.Globalization.NumberStyles.Number, culture, out result))
                        return result;
                }

                return Ifc4.CustomModel.CustomPropertyDescriptor.CustomChangeType<T>(value, typeof(T));
                //return CustomChangeType<T>(value, typeof(T));
                //return null;
            }
            return base.ConvertFrom(context, culture, value);
        }

        //private T CustomChangeType<T>(object value, Type conversionType)
        //{
        //    if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
        //    {
        //        if (value == null)
        //            return default(T);

        //        NullableConverter nullableConverter = new NullableConverter(conversionType);
        //        conversionType = nullableConverter.UnderlyingType;
        //    }
        //    return (T)Convert.ChangeType(value, conversionType);
        //}

    }

    public class CustomEnumTypeConverter : System.ComponentModel.TypeConverter
	{
        public CustomEnumTypeConverter()
		{
		}

		public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true; // zeigt ComboBox Symbol an
		}

		public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true; // es können nur Einträge aus der ComboBox ausgewählt werden
		}

		public override StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			if (context == null)
				return base.GetStandardValues(context);

			CustomModel.CustomPropertyDescriptor customPropertyDescriptor = context.PropertyDescriptor as CustomModel.CustomPropertyDescriptor;
			if (customPropertyDescriptor != null)
			{
				IEnumerable<string> ie = customPropertyDescriptor.PropertyItem.ComboboxValues.Select(item => item.Key);
				StandardValuesCollection standardValuesCollection = new StandardValuesCollection(ie.ToList());
				return standardValuesCollection;
			}

			return base.GetStandardValues(context);
		}

		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(System.String))
				return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && destinationType == typeof(string) && (value is string))
			{
				CustomModel.CustomPropertyDescriptor customPropertyDescriptor = context.PropertyDescriptor as CustomModel.CustomPropertyDescriptor;
				if (customPropertyDescriptor != null && customPropertyDescriptor.PropertyItem != null)
				{
					CustomPropertyStandardValue enumItem;
					enumItem = customPropertyDescriptor.PropertyItem.ComboboxValues.Find(item => item.Key.Equals((string)value));
					if (enumItem != null)
					{
						// System.Diagnostics.Debug.WriteLine(String.Format("ConvertTo: {0} - {1}", value, enumItem.DisplayText));
						return enumItem.DisplayText;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (context != null && value.GetType() == typeof(string))
			{
				CustomModel.CustomPropertyDescriptor customPropertyDescriptor = context.PropertyDescriptor as CustomModel.CustomPropertyDescriptor;
				if (customPropertyDescriptor != null && customPropertyDescriptor.PropertyItem != null)
				{
					CustomPropertyStandardValue enumItem;
					enumItem = customPropertyDescriptor.PropertyItem.ComboboxValues.Find(item => item.DisplayText.Equals((string)value));
					if (enumItem != null)
					{
						// System.Diagnostics.Debug.WriteLine(String.Format("ConvertFrom: {0} - {1}", value, enumItem.Key));
						return enumItem.Key;
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
