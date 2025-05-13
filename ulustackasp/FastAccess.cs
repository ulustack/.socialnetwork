namespace ulustackasp
{
    public class FastAccess
    {
        public class HashSet<T>
        {
            private List<T>[] buckets;
            private int count;
            private int capacity;
            private const double LoadFactorThreshold = 0.75; //hashsetin büyümesi için eşik

            public HashSet(int initialCapacity = 101) //asal sayı veriyoruz
            {
                capacity = NextPrime(initialCapacity);
                buckets = new List<T>[capacity];
                for (int i = 0; i < capacity; i++)
                    buckets[i] = new List<T>();
                count = 0;
            }

            private int GetIndex(T value)
            {
                int hash = value?.GetHashCode() ?? 0;
                return Math.Abs(hash % capacity);
            }

            public void Add(T value)
            {
                if (Contains(value)) return;

                if ((double)(count + 1) / capacity > LoadFactorThreshold)
                    Resize();

                int index = GetIndex(value);
                buckets[index].Add(value);
                count++;
            }

            public bool Contains(T value)
            {
                int index = GetIndex(value);
                return buckets[index].Contains(value);
            }

            public void Remove(T value)
            {
                int index = GetIndex(value);
                if (buckets[index].Remove(value))
                    count--;
            }

            public int Count => count;

            private void Resize()
            {
                int newCapacity = NextPrime(capacity * 2);
                var newBuckets = new List<T>[newCapacity];

                for (int i = 0; i < newCapacity; i++)
                    newBuckets[i] = new List<T>();

                foreach (var bucket in buckets)
                {
                    foreach (var item in bucket)
                    {
                        int newIndex = Math.Abs(item?.GetHashCode() ?? 0) % newCapacity;
                        newBuckets[newIndex].Add(item);
                    }
                }

                buckets = newBuckets;
                capacity = newCapacity;
            }

            private int NextPrime(int start)
            {
                while (!IsPrime(start)) start++;
                return start;
            }

            private bool IsPrime(int number)
            {
                if (number <= 1) return false;
                if (number == 2) return true;
                if (number % 2 == 0) return false;

                int boundary = (int)Math.Floor(Math.Sqrt(number));
                for (int i = 3; i <= boundary; i += 2)
                    if (number % i == 0) return false;

                return true;
            }
        }

    }

}
