using System;
using UnityEngine;

[RequireComponent(typeof(Bot))]
public class BotCollisionHandler : MonoBehaviour
{
    private Bot _bot;
    private Resource _targetResourse;
    private Flag _targetFlag;
    private Base _targetBase;

    public event Action<Resource> TouchedResourse;
    public event Action TouchedBase;
    public event Action BuildingBase;

    private void OnEnable()
    {
        _bot = GetComponent<Bot>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resourse))
        {
            if (_targetResourse == resourse)
                TouchedResourse?.Invoke(resourse);
        }
        else if (other.gameObject.TryGetComponent(out Base mainBase))
        {
            if (_targetBase == mainBase && _bot.GotMission)
                TouchedBase?.Invoke();

        }
        else if (other.gameObject.TryGetComponent(out Flag flag))
        {
            if (_targetFlag == flag)
                BuildingBase?.Invoke();
        }
    }

    public void GetTargetBase(Base targetBase)
    {
        _targetBase = targetBase;
    }

    public void GetTargetFlag(Flag flag)
    {
        _targetFlag = flag;
    }

    public void GetTargetResource(Resource resource)
    {
        _targetResourse = resource;
    }
}