using System;
using UnityEngine;

[RequireComponent(typeof(BotCollisionHandler))]
public class Bot : MonoBehaviour
{
    private Base _base;
    private BaseDeliveryPoint _basePoint;
    private BotCollisionHandler _collisionHandler;
    private GrabPoint _grabPoint;
    private Flag _targetFlag;
    private bool _isGotMission = false;
    private Resource _gatheredResourse;

    public bool GotMission => _isGotMission;

    public event Action<Vector3> Moving;
    public event Action<Bot, Flag> ChangingBase;
    public event Action Stoping;

    private void OnEnable()
    {
        _collisionHandler = GetComponent<BotCollisionHandler>();
        _grabPoint = GetComponentInChildren<GrabPoint>();
        GetNewBase();

        _collisionHandler.TouchedResourse += GetResource;
        _collisionHandler.TouchedBase += GetToBase;
        _collisionHandler.BuildingBase += BuildingBase;
    }

    private void Update()
    {
        GatherResource();
    }

    private void OnDisable()
    {
        _collisionHandler.TouchedResourse -= GetResource;
        _collisionHandler.TouchedBase -= GetToBase;
        _collisionHandler.BuildingBase -= BuildingBase;
    }

    public void GetNewBase()
    {
        _base = GetComponentInParent<Base>();
        _basePoint = _base.GetComponentInChildren<BaseDeliveryPoint>();

        _collisionHandler.GetTargetBase(_base);
    }

    public void GetResourceMission(Resource resourse)
    {
        Stoping?.Invoke();
        _isGotMission = true;
        _collisionHandler.GetTargetResource(resourse);
        Moving?.Invoke(resourse.transform.position);
    }

    public void GetBaseMission(Flag flag)
    {
        _targetFlag = flag;
        Stoping?.Invoke();
        _isGotMission = true;
        _collisionHandler.GetTargetFlag(flag);
        Moving?.Invoke(flag.transform.position);
    }

    private void BuildingBase()
    {
        Stoping?.Invoke();
        _isGotMission = false;
        ChangingBase?.Invoke(this, _targetFlag);
    }

    private void GetToBase()
    {
        _isGotMission = false;
    }

    private void DropResource()
    {
        _gatheredResourse.ResourceReleased -= DropResource;
        _gatheredResourse = null;
    }

    private void GetResource(Resource resourse)
    {
        _gatheredResourse = resourse;
        resourse.ResourceReleased += DropResource;
        Stoping?.Invoke();
        Moving?.Invoke(_basePoint.transform.position);
    }

    private void GatherResource()
    {
        if (_gatheredResourse != null)
            _gatheredResourse.transform.position = _grabPoint.transform.position;
    }
}