using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sorter))]
public class Scanner : MonoBehaviour
{
    private Sorter _sorter;
    private List<Resource> _scannedResources = new();
    private List<Base> _scannedBases = new();
    private float _radius = 120f;
    private WaitForSeconds _scannDelay = new WaitForSeconds(1);

    private void OnEnable()
    {
        _sorter = GetComponent<Sorter>();
    }

    private void Start()
    {
        StartCoroutine(Scanning());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private IEnumerator Scanning()
    {
        while (enabled)
        {
            GetScannedObjects();

            yield return _scannDelay;
        }
    }

    private void GetScannedObjects()
    {
        _scannedResources.Clear();
        _scannedBases.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        ScanBases(colliders);
        ScanResources(colliders);

        _sorter.GetObjects(_scannedResources, _scannedBases);
    }

    private void ScanResources(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Resource resourse) && !_sorter.ScannedResources.Contains(resourse) && !_sorter.DeliveredResources.Contains(resourse))
            {
                _scannedResources.Add(resourse);
            }
        }
    }

    private void ScanBases(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Base warehouseBase) && !_sorter.ScannedBases.Contains(warehouseBase))
            {
                _scannedBases.Add(warehouseBase);
            }
        }
    }
}