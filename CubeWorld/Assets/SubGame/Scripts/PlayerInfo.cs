using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public GameObject playerObj;
    public int playerHeight;
    public enum Action { Moving, Sliding, Climbing, Sliping };
    public Action action = Action.Moving;

    public PlayerInfo(GameObject playerObj, int playerHeight)
    {
        this.playerObj = playerObj;
        this.playerHeight = playerHeight;
    }
}
