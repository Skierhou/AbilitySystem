using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    public GameObject PlayerTemplate;

    private List<Controller> PlayerControllers;
    private List<PlayerStart> PlayerStartPoints;

    private void Awake()
    {
        PlayerControllers = new List<Controller>();
        PlayerStartPoints = FindObjectsOfType<PlayerStart>().ToList();

        InitGame();
    }

    public void InitGame()
    {
        PlayerControllers.Add(new Controller());
        PlayerRestart(PlayerControllers[0]);
    }
    public void PlayerRestart(Controller controller)
    {
        foreach (PlayerStart ps in PlayerStartPoints)
        {
            if (ps.bEnable)
            {
                //重生
                GameObject go = Instantiate(PlayerTemplate, ps.transform.position, ps.transform.rotation);
                controller.Possess(go.GetComponent<Character>());
                break;
            }
        }
    }
    public List<Controller> GetControllers()
    {
        return PlayerControllers;
    }
}
