using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public delegate Object Func();

    public class Lazy<T>: IDisposable where T: class, new()
    {
        public Lazy() { }
        public Lazy(Func initializor)
        {
            _Initializor = initializor;
        }

        Func _Initializor = null;
        T _Value = null;
        static Object SyncRoot = new object();

        public T Value
        {
            get 
            {
                if (!IsValueCreated)
                {
                    lock(SyncRoot)
                    {
                        if (!IsValueCreated)
                        {
                            _Value = _Initializor == null ? new T() :
                                (T)_Initializor();
                        }
                    }
                }
                return _Value;
            }
        }

        public Boolean IsValueCreated
        {
            get { return _Value != null; }
        }

        public void Dispose()
        {
            if (IsValueCreated)
            {
                lock (SyncRoot)
                {
                    if (IsValueCreated)
                    {
                        if (_Value is IDisposable)
                        {
                            ((IDisposable)_Value).Dispose();
                        }
                    }
                }
            }
        }
    }
}
