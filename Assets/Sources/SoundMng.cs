using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMng : MonoBehaviour
{
    bool _SoundOn;

    [SerializeField]
    GameMng _GameMng;

    [SerializeField]
    GameObject _SoundBall_On;
    [SerializeField]
    GameObject _SoundBall_Off;

    [SerializeField]
    AudioSource _LobbyBgm;
    [SerializeField]
    AudioSource _IngameBgm;
    

    [SerializeField]
    GameObject _ButtonClickClip;
    [SerializeField]
    GameObject _GameResultClip;
    [SerializeField]
    GameObject _SetStoneClip;
    [SerializeField]
    GameObject _MatchingClip;
    [SerializeField]
    GameObject _GameChangeClip;

    private void Start()
    {
        _SoundOn = PlayerPrefs.GetInt("soundon",1)==1?true:false;
    }
    private void Update()
    {
        if(_SoundOn)
        {
            _SoundBall_On.SetActive(true);
            _SoundBall_Off.SetActive(false);
            
        }
        else
        {
            _SoundBall_On.SetActive(false);
            _SoundBall_Off.SetActive(true);
        }
        _LobbyBgm.gameObject.SetActive(!_GameMng._GameStarted);
        _IngameBgm.gameObject.SetActive(_GameMng._GameStarted);
        _LobbyBgm.mute = !_SoundOn;
        _IngameBgm.mute = !_SoundOn;
    }

    public void ResetBgmTime()
    {
        _LobbyBgm.time = 0;
        _IngameBgm.time = 0;
    }

    public void SoundTurnOn()
    {
        _SoundOn = true;
        PlayerPrefs.SetInt("soundon", 1);
    }
    public void SoundTurnOff()
    {
        _SoundOn = false;
        PlayerPrefs.SetInt("soundon", 0);
    }

    

    public void EffectSoundPlay(string str)
    {
        if(_SoundOn)
        {
            GameObject obj = new GameObject();
            switch (str)
            {
                case "click":
                    obj = Instantiate(_ButtonClickClip);
                    break;
                case "result":
                    obj = Instantiate(_GameResultClip);
                    break;
                case "setstone":
                    obj = Instantiate(_SetStoneClip);
                    break;
                case "match":
                    obj = Instantiate(_MatchingClip);
                    break;
                case "change":
                    obj = Instantiate(_GameChangeClip);
                    break;
            }
            obj.transform.parent = transform;
            obj.transform.localPosition = new Vector3();
        }
        
    }

    public bool isSoundOn() { return _SoundOn; }
}
