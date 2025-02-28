using System;
using UnityEngine;

[RequireComponent(typeof(BotCollisionHandler))]
public class Bot : MonoBehaviour
{
    private Base _base;
    private Resource _gatheredResourse;
    private BaseDeliveryPoint _basePoint;
    private BaseBuilder _baseBuilder;
    private BotCollisionHandler _collisionHandler;
    private BotMover _mover;
    private GrabPoint _grabPoint;

    public bool GotMission { get; private set; } = false;

    private void OnEnable()
    {
        _collisionHandler = GetComponent<BotCollisionHandler>();
        _mover = GetComponent<BotMover>();
        _grabPoint = GetComponentInChildren<GrabPoint>();
        _base = GetComponentInParent<Base>();
        _baseBuilder = _base.GetComponentInParent<BaseBuilder>();
        TakeNewBase();

        _collisionHandler.TouchedResourse += TakeResource;
        _collisionHandler.TouchedBase += TouchBase;
        _collisionHandler.BaseBuilded += BuildBase;
    }

    private void Update()
    {
        GatherResource();
    }

    private void OnDisable()
    {
        _collisionHandler.TouchedResourse -= TakeResource;
        _collisionHandler.TouchedBase -= TouchBase;
        _collisionHandler.BaseBuilded -= BuildBase;
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
        GotMission = true;
        _collisionHandler.SetTargetResource(resourse);
        _mover.StartMove(resourse.transform.position);
    }

    public void TakeBaseMission(Flag flag)
    {
        _mover.StopMove();
        GotMission = true;
        _collisionHandler.SetTargetFlag(flag);
        _mover.StartMove(flag.transform.position);
    }

    private void BuildBase(Vector3 basePosition)
    {
        _mover.StopMove();
        GotMission = false;
        Base newBase = _baseBuilder.BuildNewBase(basePosition);
        _base.RemoveBot(this);
        _base.ReturnFlag();
        newBase.AddBot(this);
        _base = newBase;
        TakeNewBase();
    }

    private void TouchBase()
    {
        GotMission = false;
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