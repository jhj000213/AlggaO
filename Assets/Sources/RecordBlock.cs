using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordBlock : MonoBehaviour
{
    Text _EnemyName;

    public void Init(string name,string n)
    {
        _EnemyName = transform.GetChild(3).GetComponent<Text>();
        _EnemyName.text = name;
        int i = 0;
        if (n == "win")
            i = 0;
        else if (n == "lose")
            i = 1;
        else if (n == "draw")
            i = 2;
        transform.GetChild(i).gameObject.SetActive(true);
    }
    public void SetOff()
    {
        _EnemyName = transform.GetChild(3).GetComponent<Text>();
        _EnemyName.text = "";
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
