using UnityEngine;

public class BotBuilder : MonoBehaviour
{
    [SerializeField] private Bot _botPrefab;

    public Bot CreateBot(Base @base)
    {
        Bot bot = Instantiate(_botPrefab, @base.transform);
        bot.transform.position = @base.transform.position;
        bot.gameObject.SetActive(true);
        return bot;
    }
}