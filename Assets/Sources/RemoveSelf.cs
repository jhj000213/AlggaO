using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSelf : MonoBehaviour
{
    public float _Time;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(_Time);
        Destroy(gameObject);
    }
}
