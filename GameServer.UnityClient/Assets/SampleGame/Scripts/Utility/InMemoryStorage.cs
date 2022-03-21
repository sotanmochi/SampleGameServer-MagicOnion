namespace SampleGame.Utility
{
    public class InMemoryStorage<T> where T : class
    {
        protected readonly T[] _storage;

        public InMemoryStorage(int capacity)
        {
            _storage = new T[capacity];
        }

        public T this[int index] => _storage[index];

        public bool TryAdd(int id, T value)
        {
            if (id < 0 || id >= _storage.Length)
            {
                return false;
            }

            _storage[id] = value;
            return true;
        }

        public bool TryGetValue(int id, out T value)
        {
            if (id < 0 || id >= _storage.Length)
            {
                value = null;
                return false;
            }

            value = _storage[id];
            return true;
        }

        public bool TryRemove(int id, out T value)
        {
            if (id < 0 || id >= _storage.Length)
            {
                value = null;
                return false;
            }

            value = _storage[id];
            _storage[id] = null;
            return true;
        }
    }
}