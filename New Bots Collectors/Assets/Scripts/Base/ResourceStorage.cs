using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseCollisionHandler), typeof(BaseBotsGarage), typeof(Base))]
public class ResourceStorage : MonoBehaviour
{
    private List<Resource> _collectedResources = new();
    private BaseCollisionHandler _baseCollisionHandler;
    private BaseBotsGarage _botsGarage;
    private Base _warehouseBase;
    private Vector3 _flagPosition;
    private int _resourcesForBotCheck = 3;
    private int _resourcesForBaseCheck = 5;
    private bool _isFlagStands;
    private WaitForSeconds _wait = new WaitForSeconds(1);

    public event Action<int> TextChanged;
    public event Action<Resource> ResourceCollected;
    public event Action EnoughResourcesForBase;
    public event Action EnoughResourcesForBot;

    private void OnEnable()
    {
        StartCoroutine(SumCheck());

        _baseCollisionHandler = GetComponent<BaseCollisionHandler>();
        _botsGarage = GetComponent<BaseBotsGarage>();
        _warehouseBase = GetComponent<Base>();

        _baseCollisionHandler.ResourseCollecting += CollectResource;
        _botsGarage.BotCreated += RemoveResources;
        _botsGarage.BaseCreated += RemoveResources;
        _warehouseBase.FlagSetted += SetMission;
    }

    private void OnDisable()
    {
        _baseCollisionHandler.ResourseCollecting -= CollectResource;
        _botsGarage.BotCreated -= RemoveResources;
        _botsGarage.BaseCreated -= RemoveResources;
        _warehouseBase.FlagSetted -= SetMission;
    }

    private void SetMission(Vector3 flagPosition)
    {
        _flagPosition = flagPosition;
        ChangeBool();
    }

    private void CollectResource(Resource resource)
    {
        ResourceCollected?.Invoke(resource);
        TextChanged?.Invoke(_collectedResources.Count);
        _collectedResources.Add(resource);
    }

    private IEnumerator SumCheck()
    {
        while (enabled)
        {
            if (_collectedResources.Count >= _resourcesForBotCheck && !_isFlagStands)
            {
                EnoughResourcesForBot?.Invoke();
            }
            else if (_collectedResources.Count >= _resourcesForBaseCheck && _isFlagStands)
            {
                if (_botsGarage.FreeObjects.Count + _botsGarage.OcupiedObjects.Count != 1)
                {
                    EnoughResourcesForBase?.Invoke();
                    ChangeBool();
                }
                else
                {
                    EnoughResourcesForBot?.Invoke();
                }
            }

            yield return _wait;
        }
    }

    private void ChangeBool()
    {
        _isFlagStands = !_isFlagStands;
    }

    private void RemoveResources(int count)
    {
        _collectedResources.RemoveRange(0, count);
        TextChanged?.Invoke(_collectedResources.Count - 1);
    }
}