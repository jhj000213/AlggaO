using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFadeObject : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    Text text;

    

    float _Time;
    bool _On = false;
    
    public void Init(float time,string str)
    {
        text.text = str;
        _Time = time;
        _On = true;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(_On)
        {
            _Time -= Time.smoothDeltaTime;
            image.color = new Color(1, 1, 1, _Time >= 1.0f ? 1 : _Time);
            text.color = new Color(1, 1, 1, _Time >= 1.0f ? 1 : _Time);
            if (_Time <= 0.0f)
            {
                _Time = 0.0f;
                _On = false;
                gameObject.SetActive(false);
            }
        }
    }
}
