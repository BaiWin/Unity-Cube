using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableCube : MonoBehaviour, IPushableObject
{
    public int x,y,z;
    private int last_x,last_y,last_z;

    public Vector3Int _dir;

    CubeUnitMove cubeUnitMove;

    [HideInInspector]
    public PushableObjInfo pushableObj;
    void Start()
    {
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        z = Mathf.RoundToInt(transform.position.z);

        last_x = x;
        last_y = y;
        last_z = z;

        MapContainer.Instance.SetMapValue(x, y, z, 2);

        cubeUnitMove = GetComponent<CubeUnitMove>();
        pushableObj = new PushableObjInfo(this.gameObject);
    }

    void Update()
    {
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        z = Mathf.RoundToInt(transform.position.z);

        if(x != last_x || y != last_y || z != last_z)
        {
            //ModifyPos(new Vector3(x,y,z));
            UpdateMapContainer(x, y, z, last_x, last_y, last_z);
        }

        last_x = x;
        last_y = y;
        last_z = z;

        if(pushableObj.state == PushableObjInfo.State.Frozen)
        {
            if(pushableObj.action == PushableObjInfo.Action.Dormant)
            {
                pushableObj.action = PushableObjInfo.Action.Moving;
                MapContainer.Instance.SetMapValue(x, y, z, 2);
            }
        }

        if (cubeUnitMove.isMoving)
        {
            switch (PointerDetection())
            {
                case 3:
                    Push(_dir);
                    break;
            }
        }

        if (!cubeUnitMove.isMoving)
        {
            switch (PointerDetection())
            {
                case 4:
                    if(pushableObj.action != PushableObjInfo.Action.Dormant && pushableObj.state != PushableObjInfo.State.Frozen)
                    {
                        pushableObj.action = PushableObjInfo.Action.Dormant;
                        if(MapContainer.Instance.GetMapValue(x, y, z) == 2)
                        {
                            MapContainer.Instance.SetMapValue(x, y, z, 1);
                        }
                    }
                    break;
            }
        }
    }

    //public void ModifyPos(Vector3 pos)
    //{
    //    transform.position = pos;
    //}

    private void UpdateMapContainer(int x, int y, int z, int last_x, int last_y, int last_z)
    {
        if(MapContainer.Instance.GetMapValue(last_x, last_y, last_z) == 2)
        {
            MapContainer.Instance.SetMapValue(last_x, last_y, last_z, 0);
            MapContainer.Instance.SetMapValue(x, y, z, 2);
        }
    }

    public void Push(Vector3Int dir)
    {
        if (CheckMovingState())
        {
            _dir = dir;
            cubeUnitMove.MoveTowards(dir);
        }
    }

    int PointerDetection()
    {
        int index = MapContainer.Instance.GetMapValue(x, y - 1, z);
        return index;
    }

    bool CheckMovingState()
    {
        if(pushableObj.action == PushableObjInfo.Action.Moving)
        {
            return true;
        }
        return false;
    }
}
