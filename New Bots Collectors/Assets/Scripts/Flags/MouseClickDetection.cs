using System;
using UnityEngine;

public class MouseClickDetection : MonoBehaviour
{
    private Camera _camera;
    private bool _basePressed;
    private Base _baseForFlag;

    public event Action BaseClicked;

    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.tag == "Base" && !_basePressed)
                {
                    _baseForFlag = hit.collider.gameObject.GetComponent<Base>();
                    BaseClicked?.Invoke();
                }
                else if (hit.collider.gameObject.tag == "Ground" && _basePressed)
                {
                    _baseForFlag.SetFlag(hit.point);
                    ChangeBool();
                }
            }
        }
    }

    public void ChangeBool()
    {
        _basePressed = !_basePressed;
    }
}