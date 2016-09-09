using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using Ifc.NET.Interfaces;
using Ifc.NET.Extensions;


namespace Ifc.NET.Interfaces
{
    public interface IObject
    {
        IObject Parent { get; set; }
        T GetParent<T>() where T : IObject;
    }

	public interface IObjects<T> : ICollection<T>, IObject where T : IObject
	{
	}

    public interface IBaseObject :
                                        IObject,
                                        System.ComponentModel.INotifyPropertyChanged,
                                        System.ComponentModel.INotifyPropertyChanging
    {
        Guid TempId { get; }
        IBaseObject ParentIBaseObject { get; set; }
        bool CanAdd { get; }
        bool CanEdit { get; }
        bool CanRemove { get; }
    }

    public interface IBaseObjects_old<T> : System.ComponentModel.IBindingList, ICollection<T>, IBaseObject where T : IBaseObject
    {
    }

    public interface IBaseObjects<T> : System.Collections.IList, ICollection<T>, IBaseObject where T : IBaseObject
    {
    }

}

namespace Ifc.NET
{

    //public class BaseObjects_old<T> : BaseObject, IBaseObjects_old<T> where T : BaseObject
    //{

    //    private System.ComponentModel.BindingList<T> m_InternalList;

    //    public BaseObjects_old(IBaseObject parent)
    //        : base(parent)
    //    {
    //        m_InternalList = new BindingList<T>();
    //        m_InternalList.AllowNew = false;
    //        m_InternalList.AllowEdit = true;
    //        m_InternalList.AllowRemove = true;
    //        m_InternalList.RaiseListChangedEvents = true;
    //        m_InternalList.ListChanged += new ListChangedEventHandler(m_InternalList_ListChanged);

    //        this.Parent = parent;
    //    }

    //    void m_InternalList_ListChanged(object sender, ListChangedEventArgs e)
    //    {
    //        if (m_ListChangedEvent != null)
    //            m_ListChangedEvent(this, e);
    //    }

    //    public Type GenericType
    //    {
    //        get { return typeof(T); }
    //    }

    //    void IBindingList.AddIndex(PropertyDescriptor property)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public virtual object AddNew()
    //    {
    //        // darf auch keine base Funktionalität geben!!!
    //        //throw new NotImplementedException();

    //        T instance = Activator.CreateInstance<T>() as T;
    //        ((Ifc.NET.Interfaces.IBaseObjects<T>)this).Add(instance);
    //        instance.Parent = this;
    //        Ifc.NET.Document document = this.GetParent<Ifc.NET.Document>();
    //        if (document != null)
    //        {
    //            Entity entity = instance as Entity;
    //            entity.Id = document.GetNextSid();
    //            if (entity != null)
    //            {
    //                document.IfcXmlDocument.Items.Add(entity);
    //            }
    //            return instance;
    //        }
    //        return null;
    //    }

    //    public virtual IEnumerable<IBaseObject> GetElementsEnumerator()
    //    {
    //        return m_InternalList.Cast<IBaseObject>();
    //    }

    //    bool IBindingList.AllowEdit
    //    {
    //        get { return m_InternalList.AllowEdit; }
    //    }

    //    bool IBindingList.AllowNew
    //    {
    //        get { return false; }
    //        //get { return m_InternalList.AllowNew; }
    //    }

    //    bool IBindingList.AllowRemove
    //    {
    //        get { return m_InternalList.AllowRemove; }
    //    }

    //    void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    int IBindingList.Find(PropertyDescriptor property, object key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    bool IBindingList.IsSorted
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    //event ListChangedEventHandler IBindingList.ListChanged
    //    //{
    //    //   add { throw new NotImplementedException(); }
    //    //   remove { throw new NotImplementedException(); }
    //    //}
    //    private Object dummyLock = new Object();
    //    private event ListChangedEventHandler m_ListChangedEvent;
    //    event ListChangedEventHandler IBindingList.ListChanged
    //    {
    //        add
    //        {
    //            lock (dummyLock)
    //            {
    //                m_ListChangedEvent += value;
    //            }
    //        }
    //        remove
    //        {
    //            lock (dummyLock)
    //            {
    //                m_ListChangedEvent -= value;
    //            }
    //        }
    //    }

    //    void IBindingList.RemoveIndex(PropertyDescriptor property)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    void IBindingList.RemoveSort()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    ListSortDirection IBindingList.SortDirection
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    PropertyDescriptor IBindingList.SortProperty
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    bool IBindingList.SupportsChangeNotification
    //    {
    //        get { return false; }
    //        //get { return ((IBindingList)m_InternalList).SupportsChangeNotification; }

    //    }

    //    bool IBindingList.SupportsSearching
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    bool IBindingList.SupportsSorting
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    int System.Collections.IList.Add(object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    void System.Collections.IList.Clear()
    //    {
    //        m_InternalList.Clear();
    //    }

    //    bool System.Collections.IList.Contains(object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    int System.Collections.IList.IndexOf(object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    void System.Collections.IList.Insert(int index, object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    bool System.Collections.IList.IsFixedSize
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    bool System.Collections.IList.IsReadOnly
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    void System.Collections.IList.Remove(object value)
    //    {
    //        if (value == null)
    //            return;

    //        if (value is T)
    //        {
    //            m_InternalList.Remove((T)value);
    //        }
    //    }

    //    void System.Collections.IList.RemoveAt(int index)
    //    {
    //        if (index >= 0 && index < m_InternalList.Count)
    //            m_InternalList.RemoveAt(index);
    //    }

    //    object System.Collections.IList.this[int index]
    //    {
    //        get { return ((BaseObjects_old<T>)this)[index]; }
    //        set { ((BaseObjects_old<T>)this)[index] = (T)value; }
    //    }

    //    public T this[int index]
    //    {
    //        get
    //        {
    //            if (index >= 0 && index < m_InternalList.Count)
    //                return m_InternalList[index];

    //            System.Diagnostics.Debug.Assert(false, "Class: BaseObjects(...), Indexer: get: public T this[int index]");
    //            return default(T);
    //        }
    //        set
    //        {
    //            if (index >= 0 && index < m_InternalList.Count)
    //            {
    //                m_InternalList[index] = value;
    //                return;
    //            }
    //            System.Diagnostics.Debug.Assert(false, "Class: BaseObjects(...), Indexer: set: public T this[int index]");

    //        }
    //    }

    //    void System.Collections.ICollection.CopyTo(Array array, int index)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    int System.Collections.ICollection.Count
    //    {
    //        get { return m_InternalList.Count; }
    //    }

    //    bool System.Collections.ICollection.IsSynchronized
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    object System.Collections.ICollection.SyncRoot
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return m_InternalList.GetEnumerator();
    //        //throw new NotImplementedException();
    //    }

    //    void ICollection<T>.Add(T item)
    //    {
    //        m_InternalList.Add(item);
    //    }

    //    void ICollection<T>.Clear()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    bool ICollection<T>.Contains(T item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    //    {
    //        m_InternalList.CopyTo(array, arrayIndex);
    //    }

    //    int ICollection<T>.Count
    //    {
    //        get { return m_InternalList.Count(); }
    //    }

    //    bool ICollection<T>.IsReadOnly
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    bool ICollection<T>.Remove(T item)
    //    {
    //        return m_InternalList.Remove(item);
    //    }

    //    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    //    {
    //        return m_InternalList.GetEnumerator();
    //    }

    //    Guid IBaseObject.TempId
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    [System.Xml.Serialization.XmlIgnore]
    //    IBaseObject IBaseObject.ParentIBaseObject
    //    {
    //        get { return Parent as IBaseObject; }
    //        set { ((IBaseObject)this).ParentIBaseObject = value; }
    //    }

    //    public Interfaces.IObject Parent { get; private set; }

    //    Interfaces.IObject Interfaces.IObject.Parent
    //    {
    //        get { return Parent; }
    //        set { Parent = value; }
    //    }
    //}

    [Serializable]
    public class BaseObjects<T> : BaseObject, IBaseObjects<T> where T : BaseObject
    {
        private System.ComponentModel.BindingList<T> m_InternalList;

        public BaseObjects(IBaseObject parent)
            : base(parent)
        {
            m_InternalList = new BindingList<T>();
            m_InternalList.AllowNew = false;
            m_InternalList.AllowEdit = true;
            m_InternalList.AllowRemove = true;
            m_InternalList.RaiseListChangedEvents = true;
            m_InternalList.ListChanged += new ListChangedEventHandler(m_InternalList_ListChanged);

            this.Parent = parent;
        }

        void m_InternalList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (m_ListChangedEvent != null)
                m_ListChangedEvent(this, e);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach(T t in collection)
                m_InternalList.Add(t);
        }

        public Type GenericType
        {
            get { return typeof(T); }
        }

        //void IBindingList.AddIndex(PropertyDescriptor property)
        //{
        //    throw new NotImplementedException();
        //}

        public virtual object AddNew()
        {
            // darf auch keine base Funktionalität geben!!!
            //throw new NotImplementedException();

            T instance = Activator.CreateInstance<T>() as T;
            ((Ifc.NET.Interfaces.IBaseObjects<T>)this).Add(instance);
            instance.Parent = this;
            Ifc.NET.Document document = this.GetParent<Ifc.NET.Document>();
            if (document != null)
            {
                Entity entity = instance as Entity;
                if (entity != null)
                {
                    entity.Id = document.GetNextSid();
                    document.IfcXmlDocument.Items.Add(entity);
                }
                IfcRoot ifcRoot = instance as IfcRoot;
                if (ifcRoot != null)
                {
                    ifcRoot.GlobalId = document.GetNewGlobalId();
                }

                return instance;
            }
            return null;
        }

        public virtual IEnumerable<IBaseObject> GetElementsEnumerator()
        {
            return m_InternalList.Cast<IBaseObject>();
        }

        public bool Swap(T a, T b)
        {
            int indexA = m_InternalList.IndexOf(a);
            int indexB = m_InternalList.IndexOf(b);

            if (indexA == -1 || indexB == -1)
                return false;

            if (indexA == indexB)
                return false;

            if (indexA >= m_InternalList.Count())
                return false;

            if (indexB >= m_InternalList.Count())
                return false;

            T tmp = m_InternalList[indexA];
            m_InternalList[indexA] = m_InternalList[indexB];
            m_InternalList[indexB] = tmp;
            return true;
        }

        //bool IBindingList.AllowEdit
        //{
        //    get { return m_InternalList.AllowEdit; }
        //}

        //bool IBindingList.AllowNew
        //{
        //    get { return false; }
        //    //get { return m_InternalList.AllowNew; }
        //}

        //bool IBindingList.AllowRemove
        //{
        //    get { return m_InternalList.AllowRemove; }
        //}

        //void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        //{
        //    throw new NotImplementedException();
        //}

        //int IBindingList.Find(PropertyDescriptor property, object key)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IBindingList.IsSorted
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //event ListChangedEventHandler IBindingList.ListChanged
        //{
        //   add { throw new NotImplementedException(); }
        //   remove { throw new NotImplementedException(); }
        //}
        private Object dummyLock = new Object();
        private event ListChangedEventHandler m_ListChangedEvent;
        //event ListChangedEventHandler IBindingList.ListChanged
        //{
        //    add
        //    {
        //        lock (dummyLock)
        //        {
        //            m_ListChangedEvent += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (dummyLock)
        //        {
        //            m_ListChangedEvent -= value;
        //        }
        //    }
        //}

        //void IBindingList.RemoveIndex(PropertyDescriptor property)
        //{
        //    throw new NotImplementedException();
        //}

        //void IBindingList.RemoveSort()
        //{
        //    throw new NotImplementedException();
        //}

        //ListSortDirection IBindingList.SortDirection
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //PropertyDescriptor IBindingList.SortProperty
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool IBindingList.SupportsChangeNotification
        //{
        //    get { return false; }
        //    //get { return ((IBindingList)m_InternalList).SupportsChangeNotification; }

        //}

        //bool IBindingList.SupportsSearching
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool IBindingList.SupportsSorting
        //{
        //    get { throw new NotImplementedException(); }
        //}

        int System.Collections.IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void System.Collections.IList.Clear()
        {
            m_InternalList.Clear();
        }

        bool System.Collections.IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int System.Collections.IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            return this.m_InternalList.IndexOf(item);
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        bool System.Collections.IList.IsReadOnly
        {
            //get { throw new NotImplementedException(); }
            get { return ((System.Collections.IList)m_InternalList).IsReadOnly; }
        }

        void System.Collections.IList.Remove(object value)
        {
            if (value == null)
                return;

            if (value is T)
            {
                m_InternalList.Remove((T)value);
            }
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            if (index >= 0 && index < m_InternalList.Count)
                m_InternalList.RemoveAt(index);
        }

        object System.Collections.IList.this[int index]
        {
            get { return ((BaseObjects<T>)this)[index]; }
            set { ((BaseObjects<T>)this)[index] = (T)value; }
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < m_InternalList.Count)
                    return m_InternalList[index];

                System.Diagnostics.Debug.Assert(false, "Class: BaseObjects(...), Indexer: get: public T this[int index]");
                return default(T);
            }
            set
            {
                if (index >= 0 && index < m_InternalList.Count)
                {
                    m_InternalList[index] = value;
                    return;
                }
                System.Diagnostics.Debug.Assert(false, "Class: BaseObjects(...), Indexer: set: public T this[int index]");

            }
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int System.Collections.ICollection.Count
        {
            get { return m_InternalList.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_InternalList.GetEnumerator();
            //throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            m_InternalList.Add(item);
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            m_InternalList.CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return m_InternalList.Count(); }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<T>.Remove(T item)
        {
            return m_InternalList.Remove(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_InternalList.GetEnumerator();
        }

        Guid IBaseObject.TempId
        {
            get { throw new NotImplementedException(); }
        }

        [System.Xml.Serialization.XmlIgnore]
        IBaseObject IBaseObject.ParentIBaseObject
        {
            get { return Parent as IBaseObject; }
            set { ((IBaseObject)this).ParentIBaseObject = value; }
        }

        public Interfaces.IObject Parent { get; private set; }

        Interfaces.IObject Interfaces.IObject.Parent
        {
            get { return Parent; }
            set { Parent = value; }
        }


    }

    //public partial  class IfcClassificationReference
    //{
    //    public override string ToString()
    //    {
    //        return this.Name;
    //    }
    //}

    public enum ValidationEnumType
    {
        NotDefined,
        IsNotNull,
        IsNotNullOrNotEmpty,
        GreaterThan,
        GreaterThanOrEqual
    }

    public class ValidationProperty
    {
        public ValidationProperty(string propertyName, Type propertyType,  ValidationEnumType validationType, object compareValue)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            ValidationType = validationType;
            CompareValue = compareValue;
        }

        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public ValidationEnumType ValidationType { get; private set; }

        public object CompareValue { get; private set; }

    }

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
            get{return this.PropertyChanging != null;}
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
                if(m_ValidationProperties != null)
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
            if(String.IsNullOrEmpty(propertyName))
                return null;

            if(m_ValidationProperties == null)
                m_ValidationProperties = new Dictionary<string,List<Ifc.NET.ValidationProperty>>();


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

            var objectCustomDisplayNameAttribute = this.GetType().GetCustomAttributes(typeof(Ifc.NET.Attributes.CustomDisplayNameAttribute), false).SingleOrDefault() as Ifc.NET.Attributes.CustomDisplayNameAttribute;
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

                        messages.Append(Ifc.NET.EventArgs.MessageLoggedEventArgs.FormatMessage(String.Format("Feld {0} - [{1}] nicht gefüllt! ${2}$ ", objectDisplayName, propertyDisplayName, tempId)));

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
                var propertyCustomDisplayNameAttribute = propertyInfo.GetCustomAttributes(typeof(Ifc.NET.Attributes.CustomDisplayNameAttribute), false).SingleOrDefault() as Ifc.NET.Attributes.CustomDisplayNameAttribute;
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
                return (T) Parent;
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

                //Siehe niteoad


            if (this is Ifc.NET.Entity)
            {
                string sid = ((Ifc.NET.Entity)this).Id;

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
                    if(ifcRelDefinesByProperties.RelatingPropertyDefinition.Item is IfcElementQuantity)
                    {
                        IfcElementQuantity ifcElementQuantity = (IfcElementQuantity)ifcRelDefinesByProperties.RelatingPropertyDefinition.Item;
                        if(ifcElementQuantity.IsRef)
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
                removeEntities.Add(((Ifc.NET.Entity)this));
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
