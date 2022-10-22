using System.Collections.Generic;

namespace SunAndMoon
{
    public class RollbackStack<T> where T : class
    {
        private List<T> _datas = new List<T>();
        private int _position;

        public int Count => _position;

        public void Push(T data)
        {
            if (_datas.Count - 1 > _position)
            {
                _datas[_position] = data;
            }
            else
            {
                _datas.Add(data);
            }
            _position++;
        }

        public T Pop()
        {
            var data = _datas[--_position];
            return data;
        }

        public T Peek(int n)
        {
            var pos = _position - n - 1;
            if (pos < 0)
            {
                return null;
            }
            return _datas[pos];
        }

        public T Rollback(int n)
        {
            var pos = _position - n - 1;
            if (pos < 0)
            {
                return null;
            }
            _position = pos + 1;
            return _datas[pos];
        }
    }
}