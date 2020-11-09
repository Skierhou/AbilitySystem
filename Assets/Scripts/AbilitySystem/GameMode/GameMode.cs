using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode
{
    protected bool IsActive;

    public virtual void OnInitGame() { IsActive = true; }
    public virtual void OnRemoveGame() { IsActive = false; }
    public virtual void StartGame(){ }
    public virtual void EndGame() { }
    public virtual void Update() { }
}
