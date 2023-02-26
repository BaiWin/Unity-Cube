using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapContainer : MonoBehaviour
{
    private static MapContainer instance = null;
    public static MapContainer Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MapContainer>();
                if(instance == null)
                {
                    GameObject go = new GameObject("MapContainer");
                    instance = go.AddComponent<MapContainer>();
                }
            }
            return instance;
        }
    }

    public Transform map;

    [HideInInspector]
    public int min_X,max_X,min_Y,max_Y,min_Z,max_Z; //X-L, Y-W, Z-H

    [HideInInspector]
    private int[,,] _X_Y_Z = new int[1,1,1];
    
    void Awake()
    {
        List<Transform> cubes_transform = new List<Transform>();
        float count = map.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform cube = map.GetChild(i);
            cubes_transform.Add(cube);
        }
        min_X = 0;
        max_X = 0;
        min_Y = 0;
        max_Y = 0;
        min_Z = 0;
        max_Z = 0;
        foreach (var cube in cubes_transform)
        {
            int cube_x = Mathf.RoundToInt(cube.transform.position.x);
            int cube_y = Mathf.RoundToInt(cube.transform.position.y);
            int cube_z = Mathf.RoundToInt(cube.transform.position.z);
            if(cube_x < min_X) min_X = cube_x;
            if(cube_x > max_X) max_X = cube_x;
            if(cube_y < min_Y) min_Y = cube_y;
            if(cube_y > max_Y) max_Y = cube_y;
            if(cube_z < min_Z) min_Z = cube_z;
            if(cube_z > max_Z) max_Z = cube_z;
        }
        int length = max_X - min_X + 1;      
        int width = max_Z - min_Z + 1;      
        int height = max_Y - min_Y + 1 + 1;  //+1 additional for pushable object 

        Debug.Log($"Map is read: L is {length}, W is {width}, H is {height}");

        _X_Y_Z = new int[length, height, width];

        foreach (var cube in cubes_transform)
        {
            int cube_x = Mathf.RoundToInt(cube.transform.position.x);
            int cube_y = Mathf.RoundToInt(cube.transform.position.y);
            int cube_z = Mathf.RoundToInt(cube.transform.position.z);

            switch (cube.tag)
            {
                case "FrozenCube":
                    _X_Y_Z[cube_x, cube_y, cube_z] = 3;
                    break;
                case "SandCube":
                    _X_Y_Z[cube_x, cube_y, cube_z] = 4;
                    break;
                case "Stairs":
                    _X_Y_Z[cube_x, cube_y, cube_z] = 5;
                    break;
                case "IceCube":
                    _X_Y_Z[cube_x, cube_y, cube_z] = 6;
                    break;
                default:
                    _X_Y_Z[cube_x, cube_y, cube_z] = 1;
                    break;
            }
        }

        //0--Empty   1--Cube   2--PushableObject(Separate handle)   3--FrozenCube   4--SandCube   5--Stair   6--IceCube
    }

    void Update()
    {
        
    }

    public int GetMapValue(Vector3Int v3Int)
    {
        int x = v3Int.x;
        int y = v3Int.y;
        int z = v3Int.z;
        if (CheckLimitationIndex(x, y ,z) == false) return 0;
        return _X_Y_Z[x, y, z];
    }

    public int GetMapValue(int x, int y, int z)
    {
        if (CheckLimitationIndex(x, y, z) == false) return 0;
        return _X_Y_Z[x, y, z];
    }

    public void SetMapValue(int x, int y, int z, int num)
    {
        if (CheckLimitationIndex(x, y, z) == false) return;
        _X_Y_Z[x, y, z] = num;
    }

    //private bool CheckLimitationIndex(Vector3Int v3Int)
    //{

    //}

    private bool CheckLimitationIndex(int x, int y, int z)
    {
        if(x < min_X || x > max_X || y < min_Y || y > max_Y + 1 || z < min_Z || z > max_Z)
        {
            return false;
        }
        return true;
    }
}
