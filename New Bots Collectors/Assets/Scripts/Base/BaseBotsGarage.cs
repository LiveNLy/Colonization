using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class BaseBotsGarage : Allocator<Bot>
{
    private ResourceStorage _resourceStorage;
    private BotBuilder _botBuilder;
    private BaseBuilder _baseBuilder;
    private int _priceForBot = 3;
    private int _priceForBase = 5;
    private int _radius = 5;
    private int _yPositionForSetBase = 5;
    private WaitForSeconds _wait = new WaitForSeconds(0.5f);

    public List<Bot> FreeObjects => _freeObjects;
    public List<Bot> OcupiedObjects => _occupiedObjects;

    public event Action<int> BotCreated;
    public event Action<int> BaseCreated;
    public event Action<Bot> SettedParentForBot;
    public event Action BaseBuilded;
    public event Action FlagReturned;

    private void OnEnable()
    {
        _baseBuilder = GetComponentInParent<BaseBuilder>();
        _resourceStorage = GetComponent<ResourceStorage>();
        _botBuilder = GetComponent<BotBuilder>();
        FillList();

        _resourceStorage.EnoughResourcesForBot += SetNewBot;
        _resourceStorage.EnoughResourcesForBase += StartBuildBase;
        StartCoroutine(FreeBot());
    }

    private void OnDisable()
    {
        _resourceStorage.EnoughResourcesForBot -= SetNewBot;
        _resourceStorage.EnoughResourcesForBase -= StartBuildBase;
    }

    public void UnfreedBot(Bot bot)
    {
        _occupiedObjects.Add(bot);
        _freeObjects.Remove(bot);
    }

    private void FreedBot()
    {
        foreach (Bot bot in _occupiedObjects)
        {
            if (bot.GotMission == false)
            {
                _freeObjects.Add(bot);
            }
        }

        foreach (Bot bot in _freeObjects)
        {
            _occupiedObjects.Remove(bot);
        }
    }

    private IEnumerator FreeBot()
    {
        while (enabled)
        {
            FreedBot();

            yield return _wait;
        }
    }

    private void SetNewBot()
    {
        if (_freeObjects.Count != 0)
        {
            Bot bot = _botBuilder.CreateBot();
            _freeObjects.Add(bot);
            bot.BaseChanged += SetNewBase;
            BotCreated?.Invoke(_priceForBot);
        }
    }

    private void StartBuildBase()
    {
        if (_freeObjects.Count + _occupiedObjects.Count > 1)
        {
            BaseBuilded?.Invoke();
            BaseCreated?.Invoke(_priceForBase);
        }
    }

    private Vector3 PositionForNewBase(Vector3 flagPosition)
    {
        Vector3 position = flagPosition;
        position.y += _yPositionForSetBase;

        return position;
    }

    private void FillList()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Bot bot))
            {
                bot.BaseChanged += SetNewBase;
                _freeObjects.Add(bot);
                SettedParentForBot?.Invoke(bot);
            }
        }
    }

    private void SetNewBase(Bot bot, Vector3 flagPosition)
    {
        bot.BaseChanged -= SetNewBase;
        ForgetAboutBot(bot);
        _baseBuilder.MakeNewBase(PositionForNewBase(flagPosition));
        FlagReturned?.Invoke();
        bot.TakeNewBase();
    }

    private void ForgetAboutBot(Bot bot)
    {
        if (_freeObjects.Contains(bot))
        {
            _freeObjects.Remove(bot);
        }
        else
        {
            _occupiedObjects.Remove(bot);
        }
    }
}