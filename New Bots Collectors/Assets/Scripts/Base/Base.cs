using System;
using UnityEngine;

[RequireComponent(typeof(ResourceAllocator), typeof(BaseBotsGarage))]
public class Base : MonoBehaviour
{
    private Flag _flag;
    private ResourceAllocator _resourceAllocator;
    private BaseBotsGarage _botsGarage;
    private bool _isBaseReadyToBuild = false;

    public event Action<Flag> FlagSetted;

    private void OnEnable()
    {
        _resourceAllocator = GetComponent<ResourceAllocator>();
        _botsGarage = GetComponent<BaseBotsGarage>();
        _flag = GetComponentInChildren<Flag>();

        _botsGarage.BuildingBase += ConstructionReadiness;
        _botsGarage.ReturnFlag += ReturnFlag;
    }

    private void LateUpdate()
    {
        GiveJobToBots();
    }

    private void OnDisable()
    {
        _botsGarage.BuildingBase -= ConstructionReadiness;
        _botsGarage.ReturnFlag -= ReturnFlag;
    }

    public void SetFlag(Vector3 flagPosition)
    {
        _flag.transform.position = flagPosition;
        FlagSetted?.Invoke(_flag);
    }

    private void ReturnFlag()
    {
        _flag.transform.position = transform.position;
    }

    private void ConstructionReadiness()
    {
        _isBaseReadyToBuild = true;
    }

    private void GiveJobToBots()
    {
        while (enabled)
        {
            if (!_isBaseReadyToBuild)
            {
                if (_resourceAllocator.FreeObjects.Count == 0)
                    return;

                Bot bot = _botsGarage.GetNextItem();
                Resource resource = _resourceAllocator.GetNextItem();

                if (bot == null || resource == null)
                    return;

                DispatchBotForResource(bot, resource);
            }
            else
            {
                Bot bot = _botsGarage.GetNextItem();

                if (bot == null)
                    return;

                DispatchBotForBase(bot);

                _isBaseReadyToBuild = false;
            }

        }
    }

    private void DispatchBotForResource(Bot bot, Resource resource)
    {
        _resourceAllocator.UnfreedResources(resource);
        _botsGarage.UnfreedBot(bot);

        bot.GetResourceMission(resource);
    }

    private void DispatchBotForBase(Bot bot)
    {
        _botsGarage.UnfreedBot(bot);

        bot.GetBaseMission(_flag);
    }
}