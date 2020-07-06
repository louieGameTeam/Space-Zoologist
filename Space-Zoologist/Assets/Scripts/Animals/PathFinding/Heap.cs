using System;

/// <summary>
/// Generic heap data structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    public int currentItemCount { get; private set; }

    // gridX * gridY
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1)/2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1)/2;
        }
    }

    private void SortDown(T item)
    {
        while(true)
        {
            int childLeftIndex = item.HeapIndex * 2 + 1;
            int childRightIndex = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            if (childLeftIndex < currentItemCount)
            {
                swapIndex = childLeftIndex;
                if (childRightIndex < currentItemCount)
                {
                    if (items[childLeftIndex].CompareTo(items[childRightIndex]) < 0)
                    {
                        swapIndex = childRightIndex;
                    }
                }
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(T iteamA, T itemB)
    {
        items[iteamA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = iteamA;
        int temp = iteamA.HeapIndex;
        iteamA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = temp;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}