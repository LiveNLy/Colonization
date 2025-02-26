using System;
using UnityEngine;

[RequireComponent(typeof(BotCollisionHandler))]
public class Bot : MonoBehaviour
{
    private Base _base;
    private Resource _gatheredResourse;
    private BaseDeliveryPoint _basePoint;
    private BotCollisionHandler _collisionHandler;
    private BotMover _mover;
    private GrabPoint _grabPoint;
    private bool _isGotMission = false;

    public bool GotMission => _isGotMission;

    public event Action<Bot, Vector3> BaseChanged;

    private void OnEnable()
    {
        _collisionHandler = GetComponent<BotCollisionHandler>();
        _mover = GetComponent<BotMover>();
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
        _mover.StopMove();
        _isGotMission = true;
        _collisionHandler.SetTargetResource(resourse);
        _mover.StartMove(resourse.transform.position);
    }

    public void TakeBaseMission(Flag flag)
    {
        _mover.StopMove();
        _isGotMission = true;
        _collisionHandler.SetTargetFlag(flag);
        _mover.StartMove(flag.transform.position);
    }

    private void BuildingBase(Vector3 basePosition)
    {
        _mover.StopMove();
        _isGotMission = false;
        BaseChanged?.Invoke(this, basePosition);
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

    public void TakeResource(Resource resourse)
    {
        _gatheredResourse = resourse;
        resourse.ResourceReleased += DropResource;
        _mover.StopMove();
        _mover.StartMove(_basePoint.transform.position);
    }

    private void GatherResource()
    {
        if (_gatheredResourse != null)
            _gatheredResourse.transform.position = _grabPoint.transform.position;
    }
}