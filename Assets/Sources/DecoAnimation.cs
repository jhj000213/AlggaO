using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoAnimation : MonoBehaviour
{
    public bool _isWater;
    

    Vector3 _tergetPos_Water = new Vector3(-4, 0, 0);
    Vector3 _tergetPos_Else= new Vector3(0, -4, 0);

    private void Start()
    {
        StartCoroutine(FirstGo(_isWater));
    }
    IEnumerator FirstGo(bool iswater)
    {
        if(iswater)
        {
            transform.localPosition += _tergetPos_Water;
            yield return new WaitForSeconds(0.5f);
            
        }
        else
        {

            transform.localPosition += _tergetPos_Else;
            yield return new WaitForSeconds(0.3f);
            
        }
        StartCoroutine(FirstStop(iswater));
    }
    IEnumerator FirstStop(bool iswater)
    {
        if (iswater)
        {
            transform.localPosition += _tergetPos_Water;
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            transform.localPosition += _tergetPos_Else;
            yield return new WaitForSeconds(1.0f);
        }
        StartCoroutine(FirstBack(iswater));

    }
    IEnumerator FirstBack(bool iswater)
    {
        if (iswater)
        {
            transform.localPosition -= _tergetPos_Water;
            yield return new WaitForSeconds(0.5f);
        }
        else
        {

            transform.localPosition -= _tergetPos_Else;
            yield return new WaitForSeconds(0.3f);
        }
        StartCoroutine(SecondStop(iswater));

    }
    IEnumerator SecondStop(bool iswater)
    {
        if (iswater)
        {
            transform.localPosition -= _tergetPos_Water;
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            transform.localPosition -= _tergetPos_Else;
            yield return new WaitForSeconds(1.0f);
        }
        StartCoroutine(RandonTimeReady(iswater));

    }
    IEnumerator RandonTimeReady(bool iswater)
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 3.0f));
        StartCoroutine(FirstGo(iswater));
    }
}
