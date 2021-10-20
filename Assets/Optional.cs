using System;

namespace Sokoban
{
    public interface Optional<T>
    {
        bool isPresent();
        T unwrap();
    }

    public class Some<T> : Optional<T>
    {
        private T value;

        public Some(T value)
        {
            this.value = value;
        }

        public bool isPresent()
        {
            return true;
        }

        public T unwrap()
        {
            return value;
        }
    }

    public class None<T> : Optional<T>
    {
        public bool isPresent()
        {
            return false;
        }

        public T unwrap()
        {
            throw new Exception("Cannot unwrap empty optional");
        }
    }
}