using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4.Extensions
{
    public static class ExtensionMethods
    {

        private static Dictionary<Enum, string> m_EnumValues = new Dictionary<Enum, string>();
        public static string GetXmlEnumAttributeValue(this Enum enumValue)
        {
            string enumStringValue;
            if (m_EnumValues.TryGetValue(enumValue, out enumStringValue))
                return enumStringValue;

            System.Reflection.FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            System.Xml.Serialization.XmlEnumAttribute xmlEnumAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(System.Xml.Serialization.XmlEnumAttribute))  as System.Xml.Serialization.XmlEnumAttribute;
            if (xmlEnumAttribute != null)
            {
                enumStringValue = xmlEnumAttribute.Name;
                m_EnumValues.Add(enumValue, enumStringValue);
            }
            else
            {
                enumStringValue = enumValue.ToString();
            }
            return enumStringValue;
        }

        // ONLY .NET Framework 4.0!!!
        public static object GetCustomAttribute(this System.Reflection.PropertyInfo propertyInfo, Type type, bool inherit)
        {
            var customAttributes = propertyInfo.GetCustomAttributes(type, inherit);
            return (customAttributes != null && customAttributes.Length > 0) ? customAttributes[0] : null;
        }

        // ONLY .NET Framework 4.0!!!
        public static object GetCustomAttribute(this Type thisType, Type type, bool inherit)
        {
            var customAttributes = thisType.GetCustomAttributes(type, inherit);
            return (customAttributes != null && customAttributes.Length > 0) ? customAttributes[0] : null;
        }


        //public static T GetCustomAttribute<T>(this System.Reflection.PropertyInfo propertyInfo, Type type, bool inherit) where T : class
        //{
        //    var customAttributes = propertyInfo.GetCustomAttributes(type, inherit);
        //    if (customAttributes == null || customAttributes.Length == 0)
        //        return default(T);

        //    return customAttributes.Length > 0 ? (T)customAttributes[0] : default(T);
        //}


        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            return source.IndexOf(item, EqualityComparer<T>.Default);
        }
        public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            IList<T> list = source as IList<T>;
            if (list != null)
                return list.IndexOf(item);

            int i = -1;
            foreach (T x in source)
            {
                i++;
                if (comparer.Equals(x, item))
                    return i;
            }
            return -1;
        }

    }
}
