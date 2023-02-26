using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    Coroutine corou;
    void Start()
    {
        corou = StartCoroutine(DestroyMe());
    }

    private void OnDestroy()
    {
        StopCoroutine(corou);
    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
}
