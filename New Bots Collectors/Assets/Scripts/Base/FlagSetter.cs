using UnityEngine;

public class FlagSetter : MonoBehaviour
{
    private Flag _flag;

    public Flag Flag => _flag;

    private void OnEnable()
    {
        _flag = GetComponentInChildren<Flag>();
    }

    public void SetFlag(Vector3 flagPosition)
    {
        _flag.transform.position = flagPosition;
    }

    public void ReturnFlag()
    {
        _flag.transform.position = transform.position;
    }
}