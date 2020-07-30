using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFadeObject_GameChange : MonoBehaviour
{
    [SerializeField]
    Image[] image;

    

    float _Time;
    bool _On = false;
    
    public void Init(float time)
    {
        _Time = time;
        _On = true;
        gameObject.SetActive(true);
    }
    
    void Update()
    {
        if(_On)
        {
            _Time -= Time.smoothDeltaTime;
            for(int i=0;i<image.Length;i++)
            {

                image[i].color = new Color(1, 1, 1, _Time >= 1.0f ? 1 : _Time);
            }
            if (_Time <= 0.0f)
            {
                _Time = 0.0f;
                _On = false;
                gameObject.SetActive(false);
            }
        }
    }
}
