using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaerCubePointer : ICubePointer
{
    bool LocatePointerMove(Vector3 dir);
    bool LocatePointerClimb(Vector3 dir);
    bool LocatePointerSlid();
}
