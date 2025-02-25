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

    public event Action<Vector3> Moved;
    public event Action<Bot, Vector3> BaseChanged;
    public event Action Stoped;

    private void OnEnable()
    {
        _collisionHandler = GetComponent<BotCollisionHandler>();
        _grabPoint = GetComponentInChildren<GrabPoint>();
        TakeNewBase();

        _collisionHandler.TouchedResourse += TakeResource;
        _collisionHandler.TouchedBase += TouchBase;
        _collisionHandler.BaseBuilded += BuildingBase;
    }

    private void Update()
    {
        GatherResource();
    }

    private void OnDisable()
    {
        _collisionHandler.TouchedResourse -= TakeResource;
        _collisionHandler.TouchedBase -= TouchBase;
        _collisionHandler.BaseBuilded -= BuildingBase;
    }

    public void TakeNewBase()
    {
        _base = GetComponentInParent<Base>();
        _basePoint = _base.GetComponentInChildren<BaseDeliveryPoint>();

        _collisionHandler.SetTargetBase(_base);
    }

    public void TakeResourceMission(Resource resourse)
    {
        Stoped?.Invoke();
        _isGotMission = true;
        _collisionHandler.SetTargetResource(resourse);
        Moved?.Invoke(resourse.transform.position);
    }

    public void TakeBaseMission(Flag flag)
    {
        _targetFlag = flag;
        Stoped?.Invoke();
        _isGotMission = true;
        _collisionHandler.SetTargetFlag(flag);
        Moved?.Invoke(flag.transform.position);
    }

    private void BuildingBase()
    {
        Stoped?.Invoke();
        _isGotMission = false;
        BaseChanged?.Invoke(this, _targetFlag.transform.position);
    }

    private void TouchBase()
    {
        _isGotMission = false;
    }

    private void DropResource()
    {
        _gatheredResourse.ResourceReleased -= DropResource;
        _gatheredResourse = null;
    }

    private void TakeResource(Resource resourse)
    {
        _gatheredResourse = resourse;
        resourse.ResourceReleased += DropResource;
        Stoped?.Invoke();
        Moved?.Invoke(_basePoint.transform.position);
    }

    private void GatherResource()
    {
        if (_gatheredResourse != null)
            _gatheredResourse.transform.position = _grabPoint.transform.position;
    }
}