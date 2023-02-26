using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObjInfo
{
    public GameObject pushableObj;

    public enum Action { Moving, Sliding, Dormant };
    public Action action = Action.Moving;
    public enum State { None, Frozen, Fired};
    public State state = State.None;

    public PushableObjInfo(GameObject pushableObj)
    {
        this.pushableObj = pushableObj;
    }
}
