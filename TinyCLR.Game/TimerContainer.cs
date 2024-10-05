// Copyright (c) 2012 Chris Taylor

using System;

namespace TinyCLR.Game
{  
  internal class TimerContainer 
  {
    private const int DefaultSize = 4;

    private CountDownTimer[] _items = new CountDownTimer[DefaultSize];
    private int _count;

    public void Update(float elapsedTime)
    {
      for (int i = 0; i < _count; i++)
      {
        CountDownTimer timer = _items[i];
        if (timer.IsRunning)
        {
          timer.Update(elapsedTime);
        }
      }
    }

    public void Add(CountDownTimer item)
    {
      if (_count + 1 == _items.Length)
      {
        EnsureCapacity(_count + 1);
      }
      _items[_count++] = item;
    }

    public void Remove(CountDownTimer item)
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

    public CountDownTimer this[int index]
    {
      get { return _items[index]; }
    }

    private void EnsureCapacity(int required)
    {
      if (_items == null)
      {
        _items = new CountDownTimer[DefaultSize];
      }
      else if (required >= _items.Length)
      {
        CountDownTimer[] newItems = new CountDownTimer[_items.Length * 2];
        Array.Copy(_items, newItems, _items.Length);
        _items = newItems;
      }      
    }
  }
}
