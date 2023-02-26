using UnityEngine;

public class ConvinientFunc
{
    public static Vector3Int Vector3FloatToInt(Vector3 v)
    {
        int x = Mathf.RoundToInt(v.x);
        int y = Mathf.RoundToInt(v.y);
        int z = Mathf.RoundToInt(v.z);
        return new Vector3Int(x, y, z);
    }
}
