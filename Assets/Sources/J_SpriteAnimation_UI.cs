using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class J_SpriteAnimation_UI : MonoBehaviour
{
    Image _MySprite;

    [SerializeField]
    Sprite[] _Animations;

    [SerializeField]
    float _FrameTime;

    [SerializeField]
    bool _Loop;
    [SerializeField]
    bool _Destroy;

    float _NowTime;
    int _FrameNumber;
    bool _isPlaying;

    private void Start()
    {
        _MySprite = GetComponent<Image>();
        _FrameNumber = 0;
        _isPlaying = true;
    }

    private void Update()
    {
        if(_isPlaying)
        {
            _NowTime += Time.smoothDeltaTime;
            if (_NowTime >= _FrameTime)
            {
                _NowTime -= _FrameTime;
                if (_FrameNumber < _Animations.Length - 1)
                    _FrameNumber++;
                else
                {
                    if(!_Loop)
                    {
                        if (_Destroy)
                            Destroy(gameObject);
                        else
                            _isPlaying = false;
                    }
                    else
                    {
                        _FrameNumber = 0;
                    }
                }
            }
            _MySprite.sprite = _Animations[_FrameNumber];
        }
    }
}
