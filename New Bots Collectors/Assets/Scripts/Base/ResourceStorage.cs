using System;
using UnityEngine;

[RequireComponent(typeof(BaseCollisionHandler), typeof(BaseBotsGarage), typeof(Base))]
public class ResourceStorage : MonoBehaviour
{
    private int _collectedResources = 0;

    public int CollectedResources => _collectedResources;

    public event Action<int> TextChanged;
    public event Action<Resource> ResourceCollected;

    public void RemoveResources(int count)
    {
        _collectedResources -= count;
        TextChanged?.Invoke(_collectedResources);
    }

    public void CollectResource(Resource resource)
    {
        ResourceCollected?.Invoke(resource);
        _collectedResources++;
        TextChanged?.Invoke(_collectedResources);
    }
}