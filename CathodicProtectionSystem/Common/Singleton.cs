using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Singleton<T> where T: class, new()
    {
        private Singleton() { }

        static volatile T _Instance;
        static Object SyncRoot = new Object();

        public T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new T();
                    }
                }
                return _Instance;
            }
        }

        public T GetInstance()
        {
            return Instance;
        }
    }
}
