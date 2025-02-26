using System;
using UnityEngine;

[RequireComponent(typeof(Bot))]
public class BotCollisionHandler : MonoBehaviour
{
    private Resource _targetResourse;
    private Flag _targetFlag;
    private Base _targetBase;

    public event Action<Resource> TouchedResourse;
    public event Action<Vector3> BaseBuilded;
    public event Action TouchedBase;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resourse))
        {
            if (_targetResourse == resourse)
                TouchedResourse?.Invoke(resourse);
        }
        else if (other.gameObject.TryGetComponent(out Base mainBase))
        {
            if (_targetBase == mainBase && GetComponent<Bot>().GotMission)
                TouchedBase?.Invoke();
        }
        else if (other.gameObject.TryGetComponent(out Flag flag))
        {
            if (_targetFlag == flag)
                BaseBuilded?.Invoke(flag.transform.position);
        }
    }

    public void SetTargetBase(Base targetBase)
    {
        _targetBase = targetBase;
    }

    public void SetTargetFlag(Flag flag)
    {
        _targetFlag = flag;
    }

    public void SetTargetResource(Resource resource)
    {
        _targetResourse = resource;
    }
}