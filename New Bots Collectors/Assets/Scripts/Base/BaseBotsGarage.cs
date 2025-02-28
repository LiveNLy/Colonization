using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class BaseBotsGarage : Allocator<Bot>
{
    private BotBuilder _botBuilder;
    private int _radius = 5;
    private WaitForSeconds _wait = new WaitForSeconds(0.5f);

    public List<Bot> FreeObjects => _freeObjects;
    public List<Bot> OcupiedObjects => _occupiedObjects;

    private void OnEnable()
    {
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

        return bot;
    }

    public void ForgetAboutBot(Bot bot)
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

    public void AddBot(Bot bot)
    {
        _freeObjects.Add(bot);
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

    private void FillList()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Bot bot))
            {
                _freeObjects.Add(bot);
            }
        }
    }
}