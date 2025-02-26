using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class BaseBotsGarage : Allocator<Bot>
{
    private BotBuilder _botBuilder;
    private BaseBuilder _baseBuilder;
    private int _radius = 5;
    private int _yPositionForSetBase = 5;
    private WaitForSeconds _wait = new WaitForSeconds(0.5f);

    public List<Bot> FreeObjects => _freeObjects;
    public List<Bot> OcupiedObjects => _occupiedObjects;

    public event Action<Bot> SettedParentForBot;
    public event Action FlagReturned;

    private void OnEnable()
    {
        _baseBuilder = GetComponentInParent<BaseBuilder>();
        _botBuilder = GetComponentInParent<BotBuilder>();
        FillList();
        StartCoroutine(FreeBot());
    }

    public void GiveBotBaseMission(Bot bot, Flag flag)
    {
        bot.TakeBaseMission(flag);
        _occupiedObjects.Add(bot);
        _freeObjects.Remove(bot);
    }

    public void GiveBotResourceMission(Bot bot, Resource resource)
    {
        bot.TakeResourceMission(resource);
        _occupiedObjects.Add(bot);
        _freeObjects.Remove(bot);
    }

    public Bot CreateNewBot(Base @base)
    {
        Bot bot = _botBuilder.CreateBot(@base);
        _freeObjects.Add(bot);
        bot.BaseChanged += SetNewBase;

        return bot;
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