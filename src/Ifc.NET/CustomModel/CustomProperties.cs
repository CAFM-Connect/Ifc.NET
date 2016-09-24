using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifc4.CustomModel
{
    public class CustomProperties : IList<CustomProperty>
    {
		private List<CustomProperty> m_InternalList;
        public CustomProperties()
		{
			m_InternalList = new List<CustomProperty>();
		}

		public CustomProperty Find(Predicate<CustomProperty> match)
		{
			return m_InternalList.Find(match);
		}

		public void AddRange(IEnumerable<CustomProperty> items)
		{
			m_InternalList.AddRange(items);
		}
		public bool ContainsKey(string key)
		{
			if (key == null)
			{
				System.Diagnostics.Debug.Assert(false);
				return false;
			}
			foreach (CustomProperty elementProperty in m_InternalList)
			{
				if (elementProperty.Key == null)
				{
					System.Diagnostics.Debug.Assert(false);
					continue;
				}
				if (elementProperty.Key == key)
					return true;
			}
			return false;
		}
		public void ForEach(Action<CustomProperty> action)
		{
			m_InternalList.ForEach(action);
		}


		public int IndexOf(CustomProperty item)
		{
			return m_InternalList.IndexOf(item);
		}

		public void Insert(int index, CustomProperty item)
		{
			m_InternalList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			m_InternalList.RemoveAt(index);
		}

		public CustomProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
					return null;
				else
					return m_InternalList[index];
			}
			set
			{
				if (index < 0 || index >= this.Count)
					return;
				else
					m_InternalList[index] = value;
			}
		}

		public void Add(CustomProperty item)
		{
			m_InternalList.Add(item);
		}

		public void Clear()
		{
			m_InternalList.Clear();
		}

		public bool Contains(CustomProperty item)
		{
			return m_InternalList.Contains(item);
		}

		public void CopyTo(CustomProperty[] array, int arrayIndex)
		{
			m_InternalList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return m_InternalList.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<CustomProperty>)m_InternalList).IsReadOnly; }
		}

		public bool Remove(CustomProperty item)
		{
			return m_InternalList.Remove(item);
		}

		public IEnumerator<CustomProperty> GetEnumerator()
		{
			return m_InternalList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_InternalList.GetEnumerator();
		}

	}

}

