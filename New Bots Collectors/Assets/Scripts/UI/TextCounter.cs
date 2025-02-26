using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextCounter : MonoBehaviour
{
    private ResourceStorage _resourceStorage;
    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        _resourceStorage = GetComponentInParent<ResourceStorage>();
        _text = GetComponent<TextMeshProUGUI>();

        _resourceStorage.TextChanged += Count;
    }

    private void OnDisable()
    {
        _resourceStorage.TextChanged -= Count;
    }

    private void Count(int count)
    {
        _text.text = $"Ресурсы: {count}";
    }
}