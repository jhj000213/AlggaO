using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_SpriteAnimation : MonoBehaviour
{
    SpriteRenderer _MySprite;

    [SerializeField]
    Sprite[] _Animations;

    [SerializeField]
    float _FrameTime;

    [SerializeField]
    bool _Loop;
    [SerializeField]
    bool _Destroy;

    float _NowTime;
    public int _FrameNumber;
    bool _isPlaying;

    private void Start()
    {
        _MySprite = GetComponent<SpriteRenderer>();
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
