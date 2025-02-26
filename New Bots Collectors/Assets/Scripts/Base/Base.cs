using System;
using UnityEngine;

[RequireComponent(typeof(ResourceAllocator), typeof(BaseBotsGarage))]
public class Base : MonoBehaviour
{
    private FlagSetter _flagSetter;
    private ResourceAllocator _resourceAllocator;
    private ResourceStorage _resourceStorage;
    private BaseBotsGarage _botsGarage;
    private BaseCollisionHandler _collisionHandler;
    private bool _isBaseReadyToBuild = false;
    private int _resourcesForBase = 5;
    private int _resourcesForBot = 3;

    public event Action FlagSetted;

    private void OnEnable()
    {
        _resourceStorage = GetComponent<ResourceStorage>();
        _resourceAllocator = GetComponent<ResourceAllocator>();
        _botsGarage = GetComponent<BaseBotsGarage>();
        _flagSetter = GetComponent<FlagSetter>();
        _collisionHandler = GetComponent<BaseCollisionHandler>();

        _botsGarage.FlagReturned += ReturnFlag;
        _botsGarage.SettedParentForBot += SetChildBot;
        _collisionHandler.BaseOnResourceCollected += OnResourceCollected;
        _collisionHandler.ResourseCollected += ResourceCollected;
    }

    private void LateUpdate()
    {
        GiveJobToBots();
    }

    private void OnDisable()
    {
        _botsGarage.FlagReturned -= ReturnFlag;
        _botsGarage.SettedParentForBot -= SetChildBot;
        _collisionHandler.BaseOnResourceCollected -= OnResourceCollected;
        _collisionHandler.ResourseCollected -= ResourceCollected;
    }
    public void SetFlag(Vector3 flagPosition)
    {
        _flagSetter.SetFlag(flagPosition);
        FlagSetted?.Invoke();
    }

    private void ResourceCollected(Resource resource)
    {
        _resourceStorage.CollectResource(resource);
    }

    private void OnResourceCollected()
    {
        if (_flagSetter.IsSet)
        {
            if (GetValidBotsNumberForNewBase())
            {
                if (_resourceStorage.CollectedResources >= _resourcesForBase)
                {
                    _isBaseReadyToBuild = true;
                    _resourceStorage.RemoveResources(_resourcesForBase);

                    return;
                }

                return;
            }
            else
            {
                CreateBot();
            }
        }

        CreateBot();
    }

    private void CreateBot()
    {
        if (_resourceStorage.CollectedResources >= _resourcesForBot)
        {
            Bot bot = _botsGarage.CreateNewBot(this);

            SetChildBot(bot);

            _resourceStorage.RemoveResources(_resourcesForBot);
        }
    }

    private bool GetValidBotsNumberForNewBase()
    {
        if (_botsGarage.FreeObjects.Count + _botsGarage.OcupiedObjects.Count > 1)
            return true;

        return false;
    }

    private void SetChildBot(Bot bot)
    {
        bot.gameObject.transform.SetParent(transform);
    }

    private void ReturnFlag()
    {
        _flagSetter.ReturnFlag();
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
        _botsGarage.GiveBotResourceMission(bot, resource);
    }

    private void DispatchBotForBase(Bot bot)
    {
        _botsGarage.GiveBotBaseMission(bot, _flagSetter.Flag);
    }
}