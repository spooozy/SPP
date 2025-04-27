using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

public class DynamicList<T> : IEnumerable <T>{

    private T[] _items;
    private int _size;
    private static readonly T[] _emptyArray = new T[0];
    private const int DefaultCapacity = 4;
    public int Size => _size;
    public int Count => _size;
    public DynamicList(){
        _items = _emptyArray;
    }
    public DynamicList(int capacity){
        if (capacity < 0) throw new ArgumentOutOfRangeException();
        if (capacity == 0) _items = _emptyArray;
        else _items = new T[capacity];
    }

    public T this[int index]{
        get{
            if(index >= _size || index < 0) throw new ArgumentOutOfRangeException();
            return _items[index];
        }
        set{
            if(index < 0 || index >= _size) throw new ArgumentOutOfRangeException();
            _items[index] = value;
        }
    }

    public void Add(T item){
        T[] array = _items;
        int size = _size;
        if ((uint)size < (uint)array.Length)
        {
            _size = size + 1;
            array[size] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }
    
    private void AddWithResize(T item)
    {
        if (_size != _items.Length)
            throw new InvalidOperationException("List must be fuild");

        int size = _size;
        Grow(size + 1);
        _size = size + 1;
        _items[size] = item;
    }

    private void Grow(int capacity)
    {
        Capacity = GetNewCapacity(capacity);
    }

    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _size)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (value != _items.Length)
            {
                if (value > 0)
                {
                    T[] newItems = new T[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, newItems, _size);
                    }
                    _items = newItems;
                }
                else
                {
                    _items = _emptyArray;
                }
            }
        }
    }

     private int GetNewCapacity(int capacity)
    {
        int newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;
        if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;
        if (newCapacity < capacity) newCapacity = capacity;

        return newCapacity;
    }
    
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            throw new ArgumentOutOfRangeException();
        }
        _size--;
        if (index < _size)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _items[_size] = default!;
        }
    }

    public int IndexOf(T item) => Array.IndexOf(_items, item, 0, _size);

     public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            int size = _size;
            _size = 0;
            if (size > 0)
            {
                Array.Clear(_items, 0, size);
            }
        }
        else
        {
            _size = 0;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _size; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}