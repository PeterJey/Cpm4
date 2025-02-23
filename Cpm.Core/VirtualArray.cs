using System;
using System.Collections.Generic;
using System.Linq;

namespace Cpm.Core
{
    /// <summary>
    /// Allows to refer to nonexistent indices: negative and greater or equal to its size
    /// by using SetAt()
    /// </summary>
    /// <typeparam name="T">Array element</typeparam>
    public class VirtualArray<T>
    {
        private readonly List<T> _values;

        public VirtualArray()
        {
            _values = new List<T>();
        }

        public VirtualArray(IEnumerable<T> values)
        {
            _values = new List<T>(values);
        }

        public int Offset { get; private set; }

        public T[] ToArray()
        {
            return _values.ToArray();
        }

        public void Add(T value)
        {
            _values.Add(value);
        }

        public void SetAt(int index, T value)
        {
            var actualIndex = index - Offset;

            if (_values.Count <= actualIndex)
            {
                _values.AddRange(Enumerable.Repeat(default(T), actualIndex - _values.Count + 1));
            }

            if (actualIndex < 0)
            {
                _values.InsertRange(0, Enumerable.Repeat(default(T), -actualIndex));
                Offset = index;
                actualIndex = 0;
            }

            _values[actualIndex] = value;
        }

        public void LeftTrim(Func<T, bool> predicate)
        {
            var count = _values
                .TakeWhile(predicate)
                .Count();

            _values.RemoveRange(0, count);
            Offset += count;
        }

        public void RightTrim(Func<T, bool> predicate)
        {
            var count = _values
                .AsEnumerable()
                .Reverse()
                .TakeWhile(predicate)
                .Count();

            if (count == 0)
            {
                return;
            }

            _values.RemoveRange(_values.Count - count, count);
        }

        public void Process(Func<T, T> mapping)
        {
            for (var i = 0; i < _values.Count; i++)
            {
                _values[i] = mapping(_values[i]);
            }
        }
    }
}