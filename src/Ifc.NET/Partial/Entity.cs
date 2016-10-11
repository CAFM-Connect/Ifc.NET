using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public abstract partial class Entity
    {

        private bool? m_Nil;

        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "nil", Namespace = System.Xml.Schema.XmlSchema.InstanceNamespace)]
        public bool Nil
        {
            get
            {
                if (m_Nil.HasValue)
                    return m_Nil.Value;

                return !String.IsNullOrWhiteSpace(Ref);
            }
            set
            {
                if (this.m_Nil.HasValue && this.m_Nil.Value != value)
                {
                    this.RaisePropertyChanging("Nil");
                    this.m_Nil = value;
                    this.RaisePropertyChanged("Nil");
                }
            }
        }


        [System.ComponentModel.Browsable(false)]
        public bool NilSpecified
        {
            get { return this.Nil; }
        }

        //public bool ShouldSerializeNil
        //{
        //    get { return Nil; }
        //}

        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.Browsable(false)]
        public bool IsRef
        {
            get { return !String.IsNullOrEmpty(this.Ref); }
        }

        //public Entity Clone()
        //{
        //    return this.MemberwiseClone() as Entity;
        //}

        public T RefInstance<T>() where T : class
        {
            var instance = Activator.CreateInstance(this.GetType());
            var property = instance.GetType().GetProperty(nameof(Entity.Ref));
            property.SetValue(instance, this.Id, new object[] { });
            return instance as T;
        }

    }

}
