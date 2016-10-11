using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ifc4.Interfaces;

namespace Ifc4
{
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
            foreach (T t in collection)
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
            ((Ifc4.Interfaces.IBaseObjects<T>)this).Add(instance);
            instance.Parent = this;
            Ifc4.Document document = this.GetParent<Ifc4.Document>();
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

}


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
//        ((Ifc4.Interfaces.IBaseObjects<T>)this).Add(instance);
//        instance.Parent = this;
//        Ifc4.Document document = this.GetParent<Ifc4.Document>();
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
