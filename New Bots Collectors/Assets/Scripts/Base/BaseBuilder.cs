using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [SerializeField] private Base _basePrefab;

    public void MakeNewBase(Vector3 flagPosition)
    {
        Instantiate(_basePrefab, flagPosition, _basePrefab.transform.rotation, GetComponent<Scanner>().transform);
    }
}