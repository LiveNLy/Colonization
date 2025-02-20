using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceAllocator : Allocator<Resource>
{
    private ResourceStorage _resourceStorage;

    public List<Resource> FreeObjects => _freeObjects;
    public List<Resource> OccupiedObjects => _occupiedObjects;

    public event Action<Resource, ResourceAllocator> ResourceCollected;

    private void OnEnable()
    {
        _resourceStorage = GetComponent<ResourceStorage>();

        _resourceStorage.ResourceCollected += RemoveCollectedResources;
    }

    private void OnDisable()
    {
        _resourceStorage.ResourceCollected -= RemoveCollectedResources;
    }

    public void UnfreedResources(Resource resourse)
    {
        _occupiedObjects.Add(resourse);
        _freeObjects.Remove(resourse);
    }

    public void GetResources(Resource resource)
    {
        _freeObjects.Add(resource);
    }

    private void RemoveCollectedResources(Resource resource)
    {
        ResourceCollected?.Invoke(resource, this);
        _occupiedObjects.Remove(resource);
    }
}