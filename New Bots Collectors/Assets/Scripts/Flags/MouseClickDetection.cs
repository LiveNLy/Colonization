using System;
using UnityEngine;

public class MouseClickDetection : MonoBehaviour
{
    private Camera _camera;
    private bool _basePressed;

    public event Action<Vector3> FlagSetted;
    public event Action<Base> BaseClicked;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out Base @base) && !_basePressed)
                {
                    BaseClicked?.Invoke(@base);
                }
                else if (hit.collider.gameObject.TryGetComponent(out Ground ground) && _basePressed)
                {
                    FlagSetted?.Invoke(hit.point);
                    ChangePressingState();
                }
            }
        }
    }

    public void ChangePressingState()
    {
        _basePressed = !_basePressed;
    }
}