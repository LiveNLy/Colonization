using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [SerializeField] private Base _basePrefab;

    private int _yCorrectionNumber = 5;

    public Base BuildNewBase(Vector3 flagPosition)
    {
        Base @base = Instantiate(_basePrefab, PositionAdjustment(flagPosition), _basePrefab.transform.rotation, GetComponent<Scanner>().transform);
        return @base;
    }

    public Vector3 PositionAdjustment(Vector3 position)
    {
        position.y += _yCorrectionNumber;

        return position;
    }
}