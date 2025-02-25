using UnityEngine;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] private MouseClickDetection _mouseClickDetection;
    [SerializeField] private Button _placeFlagButton;
    [SerializeField] private Transform _buttonPointToMove;

    private Base _base;

    private Vector2 _buttonStartPosition;

    private void OnEnable()
    {
        _mouseClickDetection.BaseClicked += SetClickedBase;
        _mouseClickDetection.FlagSetted += SetFlagPosition;
        _placeFlagButton.onClick.AddListener(HideButton);
    }

    private void OnDisable()
    {
        _mouseClickDetection.BaseClicked -= SetClickedBase;
        _mouseClickDetection.FlagSetted -= SetFlagPosition;
        _placeFlagButton.onClick.RemoveListener(HideButton);
    }

    private void SetFlagPosition(Vector3 point)
    {
        _base.SetFlag(point);
    }

    private void SetClickedBase(Base @base)
    {
        _base = @base;
        ShowButton();
    }

    private void ShowButton()
    {
        _buttonStartPosition = _placeFlagButton.transform.position;
        _placeFlagButton.transform.position = _buttonPointToMove.position;
    }

    private void HideButton()
    {
        _mouseClickDetection.ChangePressingState();
        _placeFlagButton.transform.position = _buttonStartPosition;
    }
}