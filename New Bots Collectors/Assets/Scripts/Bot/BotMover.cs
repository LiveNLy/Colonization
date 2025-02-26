using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Bot))]
public class BotMover : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private Coroutine _coroutine;
    private WaitForFixedUpdate _wait;

    public void StartMove(Vector3 targetPosition)
    {
        _coroutine = StartCoroutine(Move(targetPosition));
    }

    public void StopMove()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private IEnumerator Move(Vector3 targetPosition)
    {
        while (enabled)
        {
            transform.LookAt(targetPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

            yield return _wait;
        }
    }
}