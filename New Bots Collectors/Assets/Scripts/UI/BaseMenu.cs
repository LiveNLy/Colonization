using UnityEngine;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] private MouseClickDetection _mouseClickDetection;
    [SerializeField] private Button _placeFlagButton;
    [SerializeField] private Transform _buttonPointToMove;

    private Vector2 _buttonStartPosition;

    private void OnEnable()
    {
        _mouseClickDetection.BaseClicked += ShowButton;
        _placeFlagButton.onClick.AddListener(HideButton);
    }

    private void OnDisable()
    {
        _mouseClickDetection.BaseClicked -= ShowButton;
        _placeFlagButton.onClick.RemoveListener(HideButton);
    }

    private void ShowButton()
    {
        _buttonStartPosition = _placeFlagButton.transform.position;
        _placeFlagButton.transform.position = _buttonPointToMove.position;
    }

    private void HideButton()
    {
        _mouseClickDetection.ChangeBool();
        _placeFlagButton.transform.position = _buttonStartPosition;
    }
}