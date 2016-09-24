using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;

namespace Ifc4.Attributes
{

    [AttributeUsageAttribute(AttributeTargets.All)]
    public class CustomCategoryAttribute : System.ComponentModel.CategoryAttribute
    {
        public CustomCategoryAttribute(string resourceSid, string defaultCategory)
            : base(GetTextFromResource(resourceSid, defaultCategory))
        {
            ResourceSid = resourceSid;
            DefaultCategory = defaultCategory;
        }

        public string ResourceSid { get; private set; }
        public string DefaultCategory { get; private set; }

        private static string GetTextFromResource(string resourceSid, string defaultText)
        {
            string displayName = Ifc4.Properties.Resources.ResourceManager.GetString(resourceSid, Ifc4.Properties.Resources.Culture);
            if (!String.IsNullOrEmpty(displayName))
                return displayName;

            return defaultText;
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class CustomDisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public CustomDisplayNameAttribute(string resourceSid, string defaultDisplayName)
            : base(GetTextFromResource(resourceSid, defaultDisplayName))
        {
            ResourceSid = resourceSid;
            DefaultDisplayName = defaultDisplayName;
        }

        public string ResourceSid { get; private set; }
        public string DefaultDisplayName { get; private set; }

        private static string GetTextFromResource(string resourceSid, string defaultText)
        {
            string displayName = Ifc4.Properties.Resources.ResourceManager.GetString(resourceSid, Ifc4.Properties.Resources.Culture);
            if (!String.IsNullOrEmpty(displayName))
                return displayName;

            return defaultText;
        }
    }

    [System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CustomEnumItemDisplayNameAttribute : Attribute
    {
        private bool HasMessage;
        public CustomEnumItemDisplayNameAttribute(string resourceSid, string defaultText)
        {
            ResourceSid = resourceSid;
            DisplayName = GetTextFromResource(resourceSid, defaultText);
            DefaultDisplayName = defaultText;
        }
        public string ResourceSid { get; private set; }
        public string DisplayName { get; private set; }
        public string DefaultDisplayName { get; private set; }

        private static string GetTextFromResource(string resourceSid, string defaultText)
        {
            string displayName = Ifc4.Properties.Resources.ResourceManager.GetString(resourceSid, Ifc4.Properties.Resources.Culture);
            if (!String.IsNullOrEmpty(displayName))
                return displayName;

            return defaultText;
        }

    }

    
    //[System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    //public class CcLayoutAttribute : Attribute
    //{
    //    // Position
    //    // Witdh
    //    // ...
    //    public CcLayoutAttribute(int positionInGrid)
    //    {
    //        PositionInGrid = positionInGrid;
    //    }
    //    public int PositionInGrid { get; set; }
    //}

    //[System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    //public class CcFormLayoutAttribute : Attribute
    //{
    //    public CcFormLayoutAttribute(bool visible, int position)
    //    {
    //        Visible = visible;
    //        Position = position;
    //    }
    //    public bool Visible{ get; set; }
    //    public int Position { get; set; }
    //}

    //[System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    //public class CcFormTextControlAttribute : Attribute
    //{
    //    public CcFormTextControlAttribute(bool multiline)
    //    {
    //        Multiline = multiline;
    //    }
    //    public bool Multiline { get; set; }
    //}



}
