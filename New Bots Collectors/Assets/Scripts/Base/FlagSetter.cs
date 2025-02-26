using UnityEngine;

public class FlagSetter : MonoBehaviour
{
    private Flag _flag;
    private bool _isSet;

    public bool IsSet => _isSet;
    public Flag Flag => _flag;

    private void OnEnable()
    {
        _flag = GetComponentInChildren<Flag>();
    }

    public void SetFlag(Vector3 flagPosition)
    {
        _flag.transform.position = flagPosition;
        _isSet = true;
    }

    public void ReturnFlag()
    {
        _flag.transform.position = transform.position;
        _isSet = false;
    }
}