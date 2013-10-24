using System;

namespace UniverseEngine
{
    public class DataArrayPoolTemplate<T> 
    {
        private T[][] pool;
        private int poolSize;
        
        public DataArrayPoolTemplate(int maxPoolSize)
        {
            pool = new T[maxPoolSize][];
        }
        
        public T[] GetArray(int size)
        {
            for (int i = 0; i < poolSize; i++)
            {
                if (pool[i].Length == size)
                {
                    T[] toReturn = pool[i];
                    pool[i] = pool[poolSize - 1];
                    poolSize--;
                    return toReturn;
                }
            }
            
            return new T[size];
        }
        
        public void ReturnArray(T[] array)
        {
            if (array != null)
                pool[poolSize++] = array;
        }
    }
}

