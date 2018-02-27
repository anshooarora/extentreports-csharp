using System;
using System.Collections.Generic;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public class GenericStructure<T>
    {
        private List<T> _list;

        public GenericStructure()
        {
            _list = new List<T>();
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public void Add(T t)
        {
            _list.Add(t);
        }

        public void Remove(T t)
        {
            _list.Remove(t);
        }

        public T Get(int index)
        {
            return _list[index];
        }

        public T GetLast()
        {
            if (_list.Count == 0)
                return default(T);

            return _list[_list.Count - 1];
        }

        public List<T> GetAllItems()
        {
            return _list;
        }

        public TIterator<T> GetEnumerator()
        {
            return new TIterator<T>(_list);
        }
    }
}
