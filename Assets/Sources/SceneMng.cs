using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMng : MonoBehaviour
{
    [SerializeField]
    GameMng _GameMng;
    [SerializeField]
    SoundMng _SoundMng;

    //Scenes
    public GameObject _LoginScene;
    public GameObject _LobbyScene;
    public GameObject _GameScene;
    public GameObject _GameScene_Camera;
    public GameObject _LobbySceneBlocking;
    public GameObject _GameSceneBlocking;
    Vector3 _ScenesPosForLerp = new Vector3(0,0, 100);
    const float _LerpSpeed = 5;

    //Popups
    public GameObject _QuitPopup;
    public GameObject _UserAccountPopup;
    public GameObject _OptionPopup;
    public GameObject _MatchingPopup;
    public GameObject _LogoutPopup;
    public GameObject _CreditPopup;

    public GameObject _GameQuitPopup;

    public GameObject _WinnerPopup;
    public GameObject _LoserPopup;
    public GameObject _WinnerPopup_Time;
    public GameObject _LoserPopup_Time;
    public GameObject _WinnerPopup_Disconnect;
    public GameObject _DrawPopup;

    public GameObject _EnemyReadyPopup_Normal;
    public GameObject _EnemyReadyPopup_GameChange;

    public GameObject _OmGamePopup;
    public GameObject _AlGamePopup;

    public AutoFadeObject _TextLogPopup;

    public void OpenTextLogPopup(string str)
    {
        _TextLogPopup.Init(4, str);
    }

    public void OpenLoginScene() { _LoginScene.SetActive(true); }
    public void CloseLoginScene() { _LoginScene.SetActive(false); }
           
    public void OpenLobbyScene() { _LobbyScene.SetActive(true); _LobbySceneBlocking.SetActive(false); }
    public void CloseLobbyScene() { _LobbyScene.SetActive(false); }

    public void OpenGameScene()
    {
        _ScenesPosForLerp = new Vector3(0,1920, 100);
        _LobbySceneBlocking.SetActive(true);
        _GameSceneBlocking.SetActive(false);
    }
    public void CloseGameScene()
    {
        _ScenesPosForLerp = new Vector3(0, 0, 100);
        _LobbySceneBlocking.SetActive(false);
        _GameSceneBlocking.SetActive(true);
    }
    private void Update()
    {
        _LobbyScene.transform.localPosition = Vector2.Lerp(_LobbyScene.transform.localPosition,_ScenesPosForLerp,Time.smoothDeltaTime* _LerpSpeed);
        _GameScene.transform.localPosition = Vector2.Lerp(_GameScene.transform.localPosition, _ScenesPosForLerp-new Vector3(0,1920), Time.smoothDeltaTime * _LerpSpeed);
        _GameScene_Camera.transform.localPosition = Vector3.Lerp(_GameScene_Camera.transform.localPosition, _ScenesPosForLerp - new Vector3(0, 1920,0), Time.smoothDeltaTime * _LerpSpeed);
     
        if(Input.GetKeyDown(KeyCode.Escape) && !_GameMng._GameStarted)
        {
            _GameQuitPopup.SetActive(!_GameQuitPopup.activeSelf);
        }
    }



    public void OpenQuitPopup() { _QuitPopup.SetActive(true); }
    public void CloseQuitPopup() { _QuitPopup.SetActive(false); }
           
    public void OpenUserAccountPopup() { _UserAccountPopup.SetActive(true); }
    public void CloseUserAccountPopup() { _UserAccountPopup.SetActive(false); }

    public void OpenOptionPopup() { _OptionPopup.SetActive(true); }
    public void CloseOptionPopup() { _OptionPopup.SetActive(false); }

    public void OpenMatchingPopup() { _MatchingPopup.SetActive(true); }
    public void CloseMatchingPopup() { _MatchingPopup.SetActive(false); }

    public void OpenLogoutPopup() { _LogoutPopup.SetActive(true); }
    public void CloseLogoutPopup() { _LogoutPopup.SetActive(false); }

    public void OpenCreditPopup() { _CreditPopup.SetActive(true); }
    public void CloseCreditPopup() { _CreditPopup.SetActive(false); }

    public void OpenGameQuitPopup() { _GameQuitPopup.SetActive(true); }
    public void CloseGameQuitPopup() { _GameQuitPopup.SetActive(false); }



    public void OpenWinnerPopup_Cor() { _GameSceneBlocking.SetActive(true); StartCoroutine(OnResultPopup(1)); }
    public void OpenLoserPopup_Cor() { _GameSceneBlocking.SetActive(true); StartCoroutine(OnResultPopup(2)); }
    public void OpenDrawPopup_Cor() { _GameSceneBlocking.SetActive(true); StartCoroutine(OnResultPopup(3)); }
    IEnumerator OnResultPopup(int num)
    {
        _GameMng._GameStarted = false;
        yield return new WaitForSeconds(4.0f);
        _GameMng._GameStarted = false;
        _GameSceneBlocking.SetActive(false);
        if (num == 1)
            _WinnerPopup.SetActive(true);
        else if (num == 2)
            _LoserPopup.SetActive(true);
        else if (num == 3)
            _DrawPopup.SetActive(true);
        _SoundMng.EffectSoundPlay("result");
    }
    public void OpenWinnerPopup_Time() { _SoundMng.EffectSoundPlay("result"); _GameMng._GameStarted = false; _WinnerPopup_Time.SetActive(true); }

    public void OpenLoserPopup_Time() { _SoundMng.EffectSoundPlay("result"); _GameMng._GameStarted = false; _LoserPopup_Time.SetActive(true); }
    

    public void OpenWinnerPopup_Disconnect() { _SoundMng.EffectSoundPlay("result"); _GameMng._GameStarted = false; _WinnerPopup_Disconnect.SetActive(true); }
    
    

    public void CloseWinnerPopup() { _WinnerPopup.SetActive(false); _WinnerPopup_Time.SetActive(false); _WinnerPopup_Disconnect.SetActive(false); }
    public void CloseLoserPopup() { _LoserPopup.SetActive(false); _LoserPopup_Time.SetActive(false); }
    public void CloseDrawPopup() { _DrawPopup.SetActive(false); }



    public void OpenEnemyReady_NormalPopup() { _EnemyReadyPopup_Normal.SetActive(true); }
    public void CloseEnemyReady_NormalPopup() { _EnemyReadyPopup_Normal.SetActive(false); }
    public void OpenEnemyReady_ChangePopup() { _EnemyReadyPopup_GameChange.SetActive(true); }
    public void CloseEnemyReady_ChangePopup() { _EnemyReadyPopup_GameChange.SetActive(false); }



    public void OpenOmGamePopup() { _OmGamePopup.SetActive(true); }
    public void CloseOmGamePopup() { _OmGamePopup.SetActive(false); }

    public void OpenAlGamePopup() { _AlGamePopup.SetActive(true); }
    public void CloseAlGamePopup() { _AlGamePopup.SetActive(false); }
}
