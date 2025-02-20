using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorter : MonoBehaviour
{
    private List<Resource> _scannedResources = new();
    private List<Resource> _deliveringResources = new();
    private List<Base> _scannedBases = new();
    private WaitForSeconds _wait = new WaitForSeconds(0.5f);

    public List<Resource> ScannedResources => _scannedResources;
    public List<Resource> DeliveredResources => _deliveringResources;
    public List<Base> ScannedBases => _scannedBases;

    private void Start()
    {
        StartCoroutine(DistributResources());
    }

    public void GetObjects(List<Resource> resources, List<Base> bases)
    {
        _scannedResources = resources;
        _scannedBases = bases;
    }

    private void ResourceSorting(ResourceAllocator resourceAllocator, Resource resource)
    {
        if (!resourceAllocator.FreeObjects.Contains(resource) && !resourceAllocator.OccupiedObjects.Contains(resource))
        {
            resourceAllocator.GetResources(resource);
        }
    }

    private void RemoveCollectedResource(Resource resource, ResourceAllocator resourceAllocator)
    {
        _deliveringResources.Remove(resource);
        resourceAllocator.ResourceCollected -= RemoveCollectedResource;
    }

    private IEnumerator DistributResources()
    {
        float distance = float.MaxValue;
        Base warehouseBase = null;
        Resource resource = null;
        ResourceAllocator resourceAllocator = null;

        while (enabled)
        {
            if (_scannedBases.Count > 0 && _scannedResources.Count > 0)
            {
                foreach (Base scannedBase in _scannedBases)
                {
                    foreach (Resource scannedResource in _scannedResources)
                    {
                        if (distance > Vector3.Distance(scannedBase.transform.position, scannedResource.transform.position))
                        {
                            warehouseBase = scannedBase;
                            resource = scannedResource;
                            distance = Vector3.Distance(scannedBase.transform.position, scannedResource.transform.position);
                        }
                    }
                }

                resourceAllocator = warehouseBase.GetComponent<ResourceAllocator>();
                resourceAllocator.ResourceCollected += RemoveCollectedResource;

                distance = float.MaxValue;

                ResourceSorting(resourceAllocator, resource);

                _deliveringResources.Add(resource);
                _scannedResources.Remove(resource);
            }

            yield return _wait;
        }
    }
}