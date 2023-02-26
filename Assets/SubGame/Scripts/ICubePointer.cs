using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICubePointer
{
    Vector3Int current_pointerCube { get; set; }

    void UpdatePointerCube();
}


