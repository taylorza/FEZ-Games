// Copyright (c) 2012 Chris Taylor

using System;

namespace TinyCLR.Game
{
    internal class GameObjectContainer
    {
        private const int DefaultSize = 4;

        private GameObject[] _items = new GameObject[DefaultSize];
        private int _count;

        public void Add(GameObject item)
        {
            if (_count + 1 == _items.Length)
            {
                EnsureCapacity(_count + 1);
            }
            _items[_count++] = item;
        }

        public void Remove(GameObject item)
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                if (_items[i] == item)
                {
                    RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAt(int i)
        {
            if (i == _count - 1)
            {
                _items[i] = null;
                _count--;
            }
            else if (_count > 0)
            {
                Array.Copy(_items, i + 1, _items, i, _count - i - 1);
                _items[_count - 1] = null;
                _count--;
            }
        }

        public int Count { get { return _count; } }

        public GameObject this[int index]
        {
            get { return _items[index]; }
        }

        private void EnsureCapacity(int required)
        {
            if (_items == null)
            {
                _items = new GameObject[DefaultSize];
            }
            else if (required >= _items.Length)
            {
                GameObject[] newItems = new GameObject[_items.Length * 2];
                Array.Copy(_items, newItems, _items.Length);
                _items = newItems;
            }
        }
    }
}
