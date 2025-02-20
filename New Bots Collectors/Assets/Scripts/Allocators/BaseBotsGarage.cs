using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(ResourceStorage), typeof(Base))]
public class BaseBotsGarage : Allocator<Bot>
{
    [SerializeField] private Base _basePrefab;

    private ResourceStorage _resourceStorage;
    private Base _base;
    private int _priceForBot = 3;
    private int _priceForBase = 5;
    private int _radius = 5;
    private WaitForSeconds _wait = new WaitForSeconds(0.5f);

    public List<Bot> FreeObjects => _freeObjects;
    public List<Bot> OcupiedObjects => _occupiedObjects;

    public event Action<int> BotCreated;
    public event Action<int> BaseCreated;
    public event Action BuildingBase;
    public event Action ReturnFlag;

    private void OnEnable()
    {
        _base = GetComponent<Base>();
        _resourceStorage = GetComponent<ResourceStorage>();
        FindPrefab();
        FillList();

        _resourceStorage.EnoughResourcesForBot += CreateBot;
        _resourceStorage.EnoughResourcesForBase += BuildBase;
        StartCoroutine(FreeBot());
    }

    private void OnDisable()
    {
        _resourceStorage.EnoughResourcesForBot -= CreateBot;
        _resourceStorage.EnoughResourcesForBase -= BuildBase;
    }

    public void UnfreedBot(Bot bot)
    {
        _occupiedObjects.Add(bot);
        _freeObjects.Remove(bot);
    }

    private void FindPrefab()
    {
        GameObject basePrefab = GameObject.Find("BasePrefab");
        _basePrefab = basePrefab.GetComponent<Base>();
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

    private void CreateBot()
    {
        if (_freeObjects.Count != 0)
        {
            Bot bot = Instantiate(_base.GetComponentInChildren<Bot>(), _base.transform);
            bot.transform.position = _base.transform.position;
            bot.gameObject.SetActive(true);
            _freeObjects.Add(bot);
            bot.ChangingBase += RemoveBot;
            BotCreated?.Invoke(_priceForBot);
        }
    }

    private void BuildBase(Flag flagPosition)
    {
        if (_freeObjects.Count + _occupiedObjects.Count > 1)
        {
            BuildingBase?.Invoke();
            BaseCreated?.Invoke(_priceForBase);
        }
    }

    private Vector3 PositionForNewBase(Flag flag)
    {
        Vector3 position = flag.transform.position;
        position.y += 5;

        return position;
    }

    private void FillList()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Bot bot))
            {
                bot.ChangingBase += RemoveBot;
                _freeObjects.Add(bot);
                bot.gameObject.transform.SetParent(_base.transform);
            }
        }
    }

    private void RemoveBot(Bot bot, Flag flagPosition)
    {
        bot.ChangingBase -= RemoveBot;
        ForgetAboutBot(bot);
        Base newBase = Instantiate(_basePrefab, PositionForNewBase(flagPosition), _basePrefab.transform.rotation);
        newBase.gameObject.SetActive(true);
        ReturnFlag?.Invoke();
        bot.GetNewBase();
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