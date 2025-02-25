using System;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(ResourceAllocator), typeof(BaseBotsGarage))]
public class Base : MonoBehaviour
{
    private FlagSetter _flagSetter;
    private ResourceAllocator _resourceAllocator;
    private BaseBotsGarage _botsGarage;
    private bool _isBaseReadyToBuild = false;

    public event Action<Vector3> FlagSetted;

    private void OnEnable()
    {
        _resourceAllocator = GetComponent<ResourceAllocator>();
        _botsGarage = GetComponent<BaseBotsGarage>();
        _flagSetter = GetComponent<FlagSetter>();

        _botsGarage.BaseBuilded += ConstructionReadiness;
        _botsGarage.FlagReturned += ReturnFlag;
        _botsGarage.SettedParentForBot += SetChildBot;
    }

    private void LateUpdate()
    {
        GiveJobToBots();
    }

    private void OnDisable()
    {
        _botsGarage.BaseBuilded -= ConstructionReadiness;
        _botsGarage.FlagReturned -= ReturnFlag;
        _botsGarage.SettedParentForBot -= SetChildBot;
    }

    public void SetFlag(Vector3 flagPosition)
    {
        _flagSetter.SetFlag(flagPosition);
        FlagSetted?.Invoke(_flagSetter.Flag.transform.position);
    }

    private void SetChildBot(Bot bot)
    {
        bot.gameObject.transform.SetParent(transform);
    }

    private void ReturnFlag()
    {
        _flagSetter.ReturnFlag();
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

        bot.TakeResourceMission(resource);
    }

    private void DispatchBotForBase(Bot bot)
    {
        _botsGarage.UnfreedBot(bot);

        bot.TakeBaseMission(_flagSetter.Flag);
    }
}