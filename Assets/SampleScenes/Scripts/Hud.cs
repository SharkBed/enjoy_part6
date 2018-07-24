using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud instance { get; private set; }

    public Text gameTime;
    public GameObject timeUp;
    public Text leveltext;

    public LEVEL _level;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        instance = this;
        timeUp.SetActive(false);
    }

    public void SetTime(float remainingTime)
    {
        if (remainingTime <= 0f)
        {
            timeUp.SetActive(true);
            gameTime.text = "0";
        }
        else
        {
            gameTime.text = ((int)remainingTime).ToString();
        }
    }
    public void SetLevel()
    {
        _level = Prototype.NetworkLobby.LobbyMainMenu.ReturnLevel();

        switch (_level)
        {
            case LEVEL.LV_EASY:
                leveltext.color = Color.green;
                leveltext.text = "\nEASY";
                break;
            case LEVEL.LV_NORMAL:
                leveltext.color = Color.white;
                leveltext.text = "\nNORMAL";
                break;
            case LEVEL.LV_HARD:
                leveltext.color = Color.red;
                leveltext.text = "\nHARD";
                break;
            case LEVEL.LV_SUPER:
                leveltext.color = Color.magenta;
                leveltext.text = "\nVERY HARD";
                break;
            default:
                break;
        }
    }

}
