using System.Collections.Generic;
using UnityEngine;

public class Allocator<T> : MonoBehaviour where T : MonoBehaviour
{
    protected List<T> _freeObjects = new();
    protected List<T> _occupiedObjects = new();

    public T GetNextItem()
    {
        if (_freeObjects.Count == 0)
            return default;

        return _freeObjects[0];
    }
}