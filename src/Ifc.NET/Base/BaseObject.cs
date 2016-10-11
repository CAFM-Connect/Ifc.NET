using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ifc4.Extensions;
using Ifc4.Interfaces;

namespace Ifc4
{
    // System.Xml.Serialization.IXmlSerializable
    [Serializable]
    [System.Runtime.Serialization.DataContractAttribute()]
    public abstract class BaseObject : object, IBaseObject
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseObject()
            : this(null)
        {
        }

        public BaseObject(IBaseObject parent)
        {
            TempId = Guid.NewGuid();
            Parent = parent;
            ParentIBaseObject = parent;
            ImageKey = String.Empty;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.Browsable(false)]
        public bool IsPropertyChangingSubscribed
        {
            get { return this.PropertyChanging != null; }
        }

        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.Browsable(false)]
        public bool IsPropertyChangedSubscribed
        {
            get { return this.PropertyChanged != null; }
        }

        // -----------------------------------------------------------------------
        private static Dictionary<string, List<ValidationProperty>> m_ValidationProperties = new Dictionary<string, List<ValidationProperty>>();
        public void ResetValidationProperties()
        {
            m_ValidationProperties = null;
        }

        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.Browsable(false)]
        public IEnumerable<ValidationProperty> ValidationProperties
        {
            get
            {
                if (m_ValidationProperties != null)
                {
                    List<ValidationProperty> validationProperties = null;
                    string key = this.GetType().Name;
                    if (m_ValidationProperties.TryGetValue(key, out validationProperties))
                        return validationProperties;
                }
                return Enumerable.Empty<ValidationProperty>();
            }
        }
        public ValidationProperty AddValidationProperty(string propertyName, Type propertyType, ValidationEnumType validationType, object compareValue = null)
        {
            if (String.IsNullOrEmpty(propertyName))
                return null;

            if (m_ValidationProperties == null)
                m_ValidationProperties = new Dictionary<string, List<Ifc4.ValidationProperty>>();


            string key = this.GetType().Name;
            ValidationProperty validationProperty = null;
            validationProperty = new ValidationProperty(propertyName, propertyType, validationType, compareValue);

            List<ValidationProperty> validationProperties = null;
            if (!m_ValidationProperties.TryGetValue(key, out validationProperties))
            {
                validationProperties = new List<ValidationProperty>();
                validationProperties.Add(validationProperty);
                m_ValidationProperties.Add(key, validationProperties);
            }
            else
            {
                validationProperties.Add(validationProperty);
            }
            return validationProperty;
        }

        public bool Validate(StringBuilder messages, out int errorCounter)
        {
            errorCounter = 0;

            var objectCustomDisplayNameAttribute = this.GetType().GetCustomAttributes(typeof(Ifc4.Attributes.CustomDisplayNameAttribute), false).SingleOrDefault() as Ifc4.Attributes.CustomDisplayNameAttribute;
            string objectDisplayName;
            if (objectCustomDisplayNameAttribute != null)
                objectDisplayName = objectCustomDisplayNameAttribute.DisplayName;
            else
                objectDisplayName = this.GetType().Name;

            bool validation = true;
            bool propertyError = false;

            foreach (var validationProperty in this.ValidationProperties)
            {
                propertyError = false;
                System.Reflection.PropertyInfo propertyInfo = this.GetType().GetProperty(validationProperty.PropertyName);
                if (propertyInfo != null)
                {
                    object value = propertyInfo.GetValue(this, null);
                    if (value == null)
                    {
                        propertyError = true;
                    }
                    else
                    {
                        if (validationProperty.PropertyType == typeof(object))
                        {
                            if (validationProperty.ValidationType == ValidationEnumType.IsNotNull)
                            {
                                if (value == null)
                                {
                                    propertyError = true;
                                }
                            }
                            else
                            {
                                // TODOJV
                                // no other comparer defined
                            }
                        }
                        else if (validationProperty.PropertyType == typeof(String))
                        {
                            if (validationProperty.ValidationType == ValidationEnumType.IsNotNullOrNotEmpty)
                            {
                                if (value == null || value.ToString().Length == 0)
                                {
                                    propertyError = true;
                                }
                            }
                            else
                            {
                                // TODOJV
                                // no other comparer defined
                            }
                        }
                        else if (validationProperty.PropertyType == typeof(Double))
                        {
                            if (validationProperty.ValidationType == ValidationEnumType.GreaterThan)
                            {
                                double dblValue;
                                double compareValue;
                                if (
                                        !TryParseValue<double>(value, out dblValue) ||
                                        !TryParseValue(validationProperty.CompareValue, out compareValue) ||
                                        !(dblValue > compareValue)
                                    )
                                {
                                    propertyError = true;
                                }
                            }
                            else if (validationProperty.ValidationType == ValidationEnumType.GreaterThanOrEqual)
                            {
                                double dblValue;
                                double compareValue;
                                if (
                                        !TryParseValue<double>(value, out dblValue) ||
                                        !TryParseValue(validationProperty.CompareValue, out compareValue) ||
                                        !(dblValue >= compareValue)
                                    )
                                {
                                    propertyError = true;
                                }
                            }
                            else
                            {
                                // TODOJV
                                // no other comparer defined
                            }

                        }
                    }

                    if (propertyError)
                    {
                        validation = false;

                        string propertyDisplayName = GetDisplayName(propertyInfo);
                        if (String.IsNullOrEmpty(propertyDisplayName))
                            propertyDisplayName = validationProperty.PropertyName;

                        Guid tempId;
                        if (this is IfcSystem)
                            tempId = this.ParentIBaseObject != null ? this.ParentIBaseObject.TempId : this.TempId;
                        else
                            tempId = this.TempId;

                        messages.Append(Ifc4.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("Feld {0} - [{1}] nicht gefüllt! ${2}$ ", objectDisplayName, propertyDisplayName, tempId)));

                        errorCounter++;
                    }
                }
            }

            return validation;
        }

        public static bool TryParseValue<T>(object input, out T value)
        {
            value = default(T);

            try
            {
                value = (T)Convert.ChangeType(input, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch // (Exception exc)
            {
            }
            return false;
        }

        private string GetDisplayName(System.Reflection.PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                var propertyCustomDisplayNameAttribute = propertyInfo.GetCustomAttributes(typeof(Ifc4.Attributes.CustomDisplayNameAttribute), false).SingleOrDefault() as Ifc4.Attributes.CustomDisplayNameAttribute;
                if (propertyCustomDisplayNameAttribute != null)
                    return propertyCustomDisplayNameAttribute.DisplayName;
            }
            return null;
        }
        // -----------------------------------------------------------------------


        [System.ComponentModel.Browsable(false)]
        public Document Document
        {
            get
            {
                if (this is Document)
                    return (Document)this;

                Document document = GetParent<Document>();
                if (document == null)
                    return Workspace.CurrentWorkspace.ActiveDocument;

                return document;
            }
        }

        public virtual bool Read(BaseObject baseObject)
        {
            //throw new NotImplementedException("You must override 'public virtual bool Read()'");
            return true;
        }

        public virtual bool Read(BaseObject baseObject, bool recursice = true)
        {
            //throw new NotImplementedException("You must override 'public virtual bool Read()'");
            return true;
        }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public Guid TempId { get; set; }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public object Parent { get; set; }


        public T GetParent<T>() where T : Interfaces.IObject
        {
            if (Parent == null)
                return default(T);

            if (Parent is T)
                return (T)Parent;
            else
                return ((BaseObject)Parent).GetParent<T>();
        }

        public virtual Type GetAddObjectType()
        {
            Type componentType = this.GetType();
            if (this is System.Collections.IEnumerable)
            {
                Type[] types;
                if (TryGetGenericTypes(this, out types))
                    componentType = types[0];
            }
            return componentType;
        }

        private bool TryGetGenericTypes(object instance, out Type[] types)
        {
            types = new Type[] { };
            Type type = instance.GetType();
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    types = type.GetGenericArguments();
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        [Flags]
        public enum EventType
        {
            None,
            BaseObjectPropertyChanging,
            BaseObjectPropertyChanged,
            BaseObjectMessageLogged,
            All =
                    BaseObjectPropertyChanging |
                    BaseObjectPropertyChanged |
                    BaseObjectMessageLogged
        }

        private static EventType m_EventType = EventType.All;
        public static void EnableEvent(EventType eventType, bool enabled)
        {
            if (enabled)
                m_EventType |= eventType;
            else
                m_EventType &= ~eventType;
        }

        public static bool IsEventEnabled(EventType eventType)
        {
            return ((m_EventType & eventType) == eventType);
        }

        [System.ComponentModel.Browsable(false)]
        public static EventType EventsEnabled
        {
            get { return m_EventType; }
            set { m_EventType = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public static EventType LockEvents()
        {
            EventType eventsEnabled = EventsEnabled;
            EnableEvent(EventType.All, false);
            return eventsEnabled;
        }

        public static void UnlockEvents(EventType eventType)
        {
            EnableEvent(eventType, true);
        }

        public void RaisePropertyChanging(string propertyName)
        {
            if (!IsEventEnabled(EventType.BaseObjectPropertyChanging))
                return;

            PropertyChangingEventHandler eventHandler = PropertyChanging;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangingEventArgs(propertyName));
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (!IsEventEnabled(EventType.BaseObjectPropertyChanged))
                return;

            PropertyChangedEventHandler eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));

                if (Document != null)
                    Document.SetDirty();
            }

        }

        public string PropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        //public string PropertyName<TProperty>(Expression<Func<TProperty>> property)
        //{
        //    var lambda = (LambdaExpression)property;
        //    MemberExpression memberExpression;
        //    if (lambda.Body is UnaryExpression)
        //    {
        //        var unaryExpression = (UnaryExpression)lambda.Body;
        //        memberExpression = (MemberExpression)unaryExpression.Operand;
        //    }
        //    else
        //    {
        //        memberExpression = (MemberExpression)lambda.Body;
        //    }
        //    return memberExpression.Member.Name;
        //}



        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public IBaseObject ParentIBaseObject { get; set; }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual string ImageKey { get; set; }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool CanAdd
        {
            get { return false; }
        }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool CanEdit
        {
            // TODOJV IFC4 Changes
            //get { return false; }
            get { return true; }
        }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool CanRemove
        {
            get { return false; }
        }

        public virtual bool Remove()
        {
            return InternalRemove();
        }

        private bool InternalRemove()
        {
            // throw new NotImplementedException("You must override Remove method.");

            if (this is Ifc4.Entity)
            {
                string sid = ((Ifc4.Entity)this).Id;

                List<Entity> removeEntities = new List<Entity>();

                // ------------------------------------------------------------------------------
                foreach (var ifcRelAggregates in this.Document.IfcXmlDocument.Items.OfType<IfcRelAggregates>())
                {
                    if (ifcRelAggregates.RelatedObjects != null && ifcRelAggregates.RelatedObjects.Items != null)
                    {
                        ifcRelAggregates.RelatedObjects.Items.RemoveAll(relatedObject => relatedObject.Ref == sid);
                        if (!ifcRelAggregates.RelatedObjects.Items.Any()) // has no items
                            removeEntities.Add(ifcRelAggregates);
                    }
                }
                // ------------------------------------------------------------------------------
                //< IfcRelAssociatesDocument GlobalId="0eDueiBMrB9AOMWod2KzTn">
                //  <RelatedObjects>
                //    <IfcProject ref="i1" xsi:nil="true" />
                //  </RelatedObjects>
                //  <RelatingDocument>
                //    <IfcDocumentInformation ref="i3302" xsi:nil="true" />
                //  </RelatingDocument>
                //</IfcRelAssociatesDocument>
                // ------------------------------------------------------------------------------
                foreach (var ifcRelAssociatesDocument in this.Document.IfcXmlDocument.Items.OfType<IfcRelAssociatesDocument>())
                {
                    if (ifcRelAssociatesDocument.RelatingDocument?.Item?.Ref == sid)
                    {
                        removeEntities.Add(ifcRelAssociatesDocument);
                    }
                }
                foreach (var ifcExternalReferenceRelationship in this.Document.IfcXmlDocument.Items.OfType<IfcExternalReferenceRelationship>())
                {
                    if (
                            ifcExternalReferenceRelationship.RelatedResourceObjects?.Items != null &&
                            ifcExternalReferenceRelationship.RelatedResourceObjects.Items.Any(item => item.Ref == sid)
                        )
                    {
                        ifcExternalReferenceRelationship.RelatedResourceObjects.Items.RemoveAll(relatedResourceObject => relatedResourceObject.Ref == sid);
                        if (!ifcExternalReferenceRelationship.RelatedResourceObjects.Items.Any()) // has no items
                            removeEntities.Add(ifcExternalReferenceRelationship);
                    }
                }
                // ------------------------------------------------------------------------------
                if (this.Parent is System.Collections.IList)
                {
                    var parent = (System.Collections.IList)this.Parent;
                    if (parent != null)
                        parent.Remove(this);
                }
                // ------------------------------------------------------------------------------
                foreach (var ifcRelDefinesByProperties in this.Document.IfcXmlDocument.Items.OfType<IfcRelDefinesByProperties>())
                {

                    if (ifcRelDefinesByProperties.RelatedObjects == null || ifcRelDefinesByProperties.RelatedObjects.Ref != sid)
                        continue;

                    if (ifcRelDefinesByProperties.RelatingPropertyDefinition == null || ifcRelDefinesByProperties.RelatingPropertyDefinition.Item == null)
                        continue;

                    //1. [System.Xml.Serialization.XmlElementAttribute("IfcElementQuantity", typeof(IfcElementQuantity), IsNullable = true)]
                    //2. [System.Xml.Serialization.XmlElementAttribute("IfcPropertySet", typeof(IfcPropertySet), IsNullable=true)]
                    //[System.Xml.Serialization.XmlElementAttribute("IfcPropertySetDefinition", typeof(IfcPropertySetDefinition), IsNullable=true)]
                    //[System.Xml.Serialization.XmlElementAttribute("IfcPropertySetDefinitionSet-wrapper", typeof(IfcPropertySetDefinitionSetwrapper), IsNullable=true)]

                    // --------------------------------------------------------------------------------
                    // 1.
                    if (ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcElementQuantity)
                    {
                        IfcElementQuantity ifcElementQuantity = (IfcElementQuantity)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item;
                        if (ifcElementQuantity.IsRef)
                            ifcElementQuantity = this.Document.IfcXmlDocument.Items.OfType<IfcElementQuantity>().FirstOrDefault(item => item.Id == ifcElementQuantity.Ref);

                        if (ifcElementQuantity.Quantities != null && ifcElementQuantity.Quantities.Items != null)
                        {
                            foreach (var quantityItem in ifcElementQuantity.Quantities.Items)
                            {
                                IfcPhysicalQuantity ifcPhysicalQuantity = quantityItem;
                                if (ifcPhysicalQuantity.IsRef)
                                    ifcPhysicalQuantity = this.Document.IfcXmlDocument.Items.OfType<IfcPhysicalQuantity>().FirstOrDefault(item => item.Id == ifcPhysicalQuantity.Ref);

                                removeEntities.Add(ifcPhysicalQuantity);
                            }
                            ifcElementQuantity.Quantities.Items.Clear();
                            removeEntities.Add(ifcElementQuantity);
                        }
                    }
                    // --------------------------------------------------------------------------------
                    // 2.
                    else if (ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcPropertySet)
                    {
                        IfcPropertySet ifcPropertySet = (IfcPropertySet)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item;
                        if (ifcPropertySet.IsRef)
                            ifcPropertySet = this.Document.IfcXmlDocument.Items.OfType<IfcPropertySet>().FirstOrDefault(item => item.Id == ifcPropertySet.Ref);

                        if (ifcPropertySet.HasProperties != null && ifcPropertySet.HasProperties.Items != null)
                        {
                            foreach (var ifcPropertyItem in ifcPropertySet.HasProperties.Items)
                            {
                                IfcProperty ifcProperty = ifcPropertyItem;
                                if (ifcProperty.IsRef)
                                    ifcProperty = this.Document.IfcXmlDocument.Items.OfType<IfcProperty>().FirstOrDefault(item => item.Id == ifcProperty.Ref);

                                removeEntities.Add(ifcProperty);
                            }
                            ifcPropertySet.HasProperties.Items.Clear();
                        }

                        // ------------------------------------------------------------------------------
                        foreach (var ifcRelDefinesByTemplate in this.Document.IfcXmlDocument.Items.OfType<IfcRelDefinesByTemplate>())
                        {
                            if (ifcRelDefinesByTemplate.RelatedPropertySets != null && ifcRelDefinesByTemplate.RelatedPropertySets.Items != null)
                            {
                                ifcRelDefinesByTemplate.RelatedPropertySets.Items.RemoveAll(relatedPropertySet => relatedPropertySet.Ref == ifcPropertySet.Id);
                                if (!ifcRelDefinesByTemplate.RelatedPropertySets.Items.Any()) // has no items
                                    removeEntities.Add(ifcRelDefinesByTemplate);
                            }
                        }

                        removeEntities.Add(ifcPropertySet);

                    }
                    else
                    {

                    }

                    removeEntities.Add(ifcRelDefinesByProperties);

                }
                // ------------------------------------------------------------------------------
                foreach (var ifcRelAssignsToGroup in this.Document.IfcXmlDocument.Items.OfType<IfcRelAssignsToGroup>())
                {
                    if (ifcRelAssignsToGroup.RelatedObjects != null && ifcRelAssignsToGroup.RelatedObjects.Items != null)
                    {
                        ifcRelAssignsToGroup.RelatedObjects.Items.RemoveAll(relatedObject => relatedObject.Ref == sid);
                        if (!ifcRelAssignsToGroup.RelatedObjects.Items.Any()) // has no items
                            removeEntities.Add(ifcRelAssignsToGroup);
                    }
                }
                // ------------------------------------------------------------------------------
                foreach (var ifcRelAssociatesClassification in this.Document.IfcXmlDocument.Items.OfType<IfcRelAssociatesClassification>())
                {
                    if (ifcRelAssociatesClassification.RelatedObjects != null && ifcRelAssociatesClassification.RelatedObjects.Items != null)
                    {
                        ifcRelAssociatesClassification.RelatedObjects.Items.RemoveAll(relatedObject => relatedObject.Ref == sid);
                        if (!ifcRelAssociatesClassification.RelatedObjects.Items.Any()) // has no items
                            removeEntities.Add(ifcRelAssociatesClassification);
                    }
                }
                // ------------------------------------------------------------------------------
                removeEntities.Add(((Ifc4.Entity)this));
                this.Document.IfcXmlDocument.Items.RemoveAll(item => removeEntities.Contains(item));
                // ------------------------------------------------------------------------------
            }
            return true;
        }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        IObject IObject.Parent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public enum ObjectPosition
        {
            First,
            Next,
            //NextLevel,
            Previous,
            //PreviousLevel
            Last
        }

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool AllowMoving
        {
            get { return false; }
        }

        public virtual bool HasSibling(ObjectPosition objectPosition)
        {
            // throw new NotImplementedException("You must override in inherited class!");

            var parentCollection = this.Parent as IEnumerable<BaseObject>;
            if (parentCollection == null)
                return false;

            int index = parentCollection.IndexOf(this);

            if (objectPosition == ObjectPosition.First || objectPosition == ObjectPosition.Previous)
            {
                return (index > 0);
            }
            else if (objectPosition == ObjectPosition.Last || objectPosition == ObjectPosition.Next)
            {
                return (index < parentCollection.Count() - 1);
            }

            return false;
        }

        public virtual BaseObject GetSibling(ObjectPosition objectPosition)
        {
            // throw new NotImplementedException("You must override in inherited class!");

            var parentCollection = this.Parent as IEnumerable<BaseObject>;
            if (parentCollection == null)
                return null;

            int index = -1;

            switch (objectPosition)
            {
                case ObjectPosition.First:
                    return parentCollection.FirstOrDefault();
                case ObjectPosition.Next:
                    index = parentCollection.IndexOf(this);
                    return parentCollection.ElementAt(index + 1);
                case ObjectPosition.Previous:
                    index = parentCollection.IndexOf(this);
                    return parentCollection.ElementAt(index - 1);
                case ObjectPosition.Last:
                    return parentCollection.LastOrDefault();
                default:
                    break;
            }
            return null;
        }

        public virtual bool Reposition(ObjectPosition elementPosition)
        {
            throw new NotImplementedException("You must override in inherited class!");
        }

    }
}
