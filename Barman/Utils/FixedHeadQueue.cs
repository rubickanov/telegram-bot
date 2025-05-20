using System.Collections;

namespace Barman.Utils
{
    public class FixedHeadQueue<T> : IReadOnlyCollection<T>, ICollection
    {
        private readonly T _head;
        private readonly Queue<T> _innerQueue;
        private readonly int _maxSize;

        public FixedHeadQueue(T head, int maxSize)
        {
            _head = head;
            _maxSize = maxSize;
            _innerQueue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            if (_innerQueue.Count + 1 >= _maxSize)
            {
                _innerQueue.Dequeue();
            }
            _innerQueue.Enqueue(item);
        }

        public T Dequeue()
        {
            return _innerQueue.Dequeue();
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return _head;
            foreach (var item in _innerQueue)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            if (array is T[] typedArray)
            {
                typedArray[index++] = _head;
                _innerQueue.CopyTo(typedArray, index);
            }
            else
            {
                throw new ArgumentException("Array must be of type T[].");
            }
        }

        public int Count => _innerQueue.Count + 1;

        int IReadOnlyCollection<T>.Count => Count;
        int ICollection.Count => Count;

        public bool IsSynchronized => false;

        public object SyncRoot => this;
    }
}