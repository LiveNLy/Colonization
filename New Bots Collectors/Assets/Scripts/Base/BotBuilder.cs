using UnityEngine;

public class BotBuilder : MonoBehaviour
{
    private Base _base;

    private void OnEnable()
    {
        _base = GetComponent<Base>();
    }

    public Bot CreateBot()
    {
        Bot bot = Instantiate(_base.GetComponentInChildren<Bot>(), _base.transform);
        bot.transform.position = _base.transform.position;
        bot.gameObject.SetActive(true);
        return bot;
    }
}