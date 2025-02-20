using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextCounter : MonoBehaviour
{
    private ResourceStorage _resourceStorage;
    private TextMeshProUGUI _text;
    private int _count = 0;

    private void OnEnable()
    {
        _resourceStorage = GetComponentInParent<ResourceStorage>();
        _text = GetComponent<TextMeshProUGUI>();

        _resourceStorage.TextChanging += Count;
    }

    private void OnDisable()
    {
        _resourceStorage.TextChanging -= Count;
    }

    private void Count(int count)
    {
        _count = count + 1;
        _text.text = $"Ресурсы: {_count}";
    }
}