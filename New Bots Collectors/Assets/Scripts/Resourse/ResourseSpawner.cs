using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ResourseSpawner : MonoBehaviour
{
    [SerializeField] private Resource _prefab;
    [SerializeField] private int _poolDefaultCapacity = 5;
    [SerializeField] private int _poolMaxSize = 12;
    [SerializeField] private float _secondsTillSpawn = 7;
    [SerializeField] private float _radius = 2;

    private WaitForSeconds _wait;

    private ObjectPool<Resource> _pool;

    private void Awake()
    {
        _wait = new WaitForSeconds(_secondsTillSpawn);

        _pool = new ObjectPool<Resource>(
            actionOnGet: (res) => PlaceResource(res),
            createFunc: () => InstantiateResource(),
            actionOnRelease: (res) => res.gameObject.SetActive(false),
            actionOnDestroy: (res) => Destroy(res),
            collectionCheck: true,
            defaultCapacity: _poolDefaultCapacity,
            maxSize: _poolMaxSize);
    }

    private void Start()
    {
        StartCoroutine(SpawnResource());
    }

    public void ReleaseResource(Resource resource)
    {
        _pool.Release(resource);
        resource.ReleaseResource -= ReleaseResource;
    }

    private void GetResource()
    {
        _pool.Get();
    }

    private void PlaceResource(Resource resource)
    {
        resource.transform.position = SetPosition();
        resource.gameObject.SetActive(true);
        resource.ReleaseResource += ReleaseResource;
    }

    private Vector3 SetPosition()
    {
        Vector3 position = transform.localPosition;
        float minRandomX = -72f;
        float maxRandomX = 71f;
        float minRandomZ = -75f;
        float maxRandomZ = 70f;

        position.x = transform.position.x - Random.Range(minRandomX, maxRandomX);
        position.z = transform.position.z - Random.Range(minRandomZ, maxRandomZ);
        position.y = transform.position.y - 4.5f;

        if (!CheckPosition(position))
        {
            SetPosition();
        }

        return position;
    }

    private bool CheckPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _radius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Base warehouseBase))
            {
                return false;
            }
        }

        return true;
    }

    private Resource InstantiateResource()
    {
        return Instantiate(_prefab);
    }

    private IEnumerator SpawnResource()
    {
        while (enabled)
        {
            GetResource();
            yield return _wait;
        }
    }
}