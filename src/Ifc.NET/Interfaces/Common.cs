using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4.Interfaces
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
