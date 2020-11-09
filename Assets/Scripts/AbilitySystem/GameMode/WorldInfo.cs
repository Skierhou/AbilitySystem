using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameMode
{
    GM_None,
};

public class WorldInfo : SingletonMono<WorldInfo>
{
    protected Dictionary<EGameMode, GameMode> AllGameModeDict;
    public EGameMode GameMode { get; protected set; }
    public GameMode Game { get; protected set; }

    private void Awake()
    {
        
    }

    public void GotoGameMode(EGameMode inGameMode)
    {
        if (inGameMode == GameMode)
        {
            Debug.LogWarning("Goto Same GameMode:" + GameMode);
            return;
        }
        if (AllGameModeDict.TryGetValue(inGameMode, out GameMode mode))
        {
            Debug.Log("Goto New GameMode:" + inGameMode + " Pre GameMode:" + GameMode);
            if(Game != null)
                Game.OnRemoveGame();
            Game = mode;
            mode.OnInitGame();
        }
        else
        {
            Debug.LogError("Goto Null GameMode:" + inGameMode);
        }
    }

    private void Update()
    {
        if (Game != null)
            Game.Update();
    }
}
