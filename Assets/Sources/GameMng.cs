using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;


public class GameMng : MonoBehaviour
{
    [SerializeField]
    SceneMng _SceneMng;
    [SerializeField]
    LoginMng _LoginMng;
    [SerializeField]
    ClientMng _ClientMng;
    [SerializeField]
    SoundMng _SoundMng;

    [SerializeField]
    GameObject _OmGameUI;
    [SerializeField]
    GameObject _AlGameUI;

    public OmGameMng _OmGameMng;
    public AlGameMng _AlGameMng;

    public bool _MustGoLobby = false;

    [SerializeField]
    Text _MyNameText;
    [SerializeField]
    Text _EnemyNameText;

    int _ChangeStoneCount_My;
    int _ChangeStoneCount_Enemy;
    [SerializeField]
    Text _ChangeStoneCount_OmGame_My;
    [SerializeField]
    Text _ChangeStoneCount_AlGame_My;

    [SerializeField]
    Text _ChangeStoneCountText_Enemy;

    [SerializeField]
    GameObject _StoneField;
    [SerializeField]
    GameObject _StonePrefab;

    [SerializeField]
    Image _MyColorIcon;
    [SerializeField]
    Image _EnemyColorIcon;

    bool _IsBlackTurn = true;
    bool _ImBlack;
    public bool _NowGameOm;

    public bool _GameStarted;
    public bool _TimerOn;
    float _MyTurnTimer;
    float _EnemyTimer;
    const float _TurnTime = 30.0f;
    [SerializeField]
    Image _TimerGaze;
    [SerializeField]
    Image _TimerGaze_Shadow;

    public int _UseStoneNumber = 0;
    List<Stone> _Stones = new List<Stone>();

    public GameObject[] _UIShadows = new GameObject[5];

    [SerializeField]
    AutoFadeObject_GameChange _ChangeGameEffect_ToAl;
    [SerializeField]
    AutoFadeObject_GameChange _ChangeGameEffect_ToOm;

    public Vector3 _BoardZeroPos;
    public Vector3 _BoardCenterPos;
    

    void Start()
    {
        _BoardZeroPos = new Vector3(50, 800, 0);
        _BoardCenterPos = _BoardZeroPos + new Vector3(490, 490, 0);
        for (int i = 0; i < 250; i++)
        {
            GameObject obj = Instantiate(_StonePrefab);
            obj.transform.parent = _StoneField.transform;
            obj.GetComponent<Stone>().Init(_SoundMng.isSoundOn());
            _Stones.Add(obj.GetComponent<Stone>());
        }
        Screen.SetResolution(Screen.width, Screen.width * 16/9, false);
        _GameStarted = false;
        _TimerOn = false;
    }

    private void Update()
    {
        _ChangeStoneCount_OmGame_My.text = _ChangeStoneCount_My.ToString();
        _ChangeStoneCount_AlGame_My.text = _ChangeStoneCount_My.ToString();
        _ChangeStoneCountText_Enemy.text = _ChangeStoneCount_Enemy.ToString();

        if(_GameStarted)
        {
            _MyNameText.text = _LoginMng._UserRecordValue[3];
            _EnemyNameText.text = _LoginMng.GetEnemyName();
            //_MyRecordText.text = _LoginMng._UserRecordValue[4] + " / " + _LoginMng._UserRecordValue[2] + " / " + _LoginMng._UserRecordValue[0];
            //_EnemyRecordText.text = _LoginMng._EnemyRecordValue[4] + " / " + _LoginMng._EnemyRecordValue[2] + " / " + _LoginMng._EnemyRecordValue[0];
            if(_TimerOn)
            {
                if (IsMyTurn())
                {
                    _MyTurnTimer -= Time.smoothDeltaTime;
                    _TimerGaze.fillAmount = _MyTurnTimer / _TurnTime;
                    _TimerGaze_Shadow.fillAmount = _MyTurnTimer / _TurnTime;
                    if (_MyTurnTimer <= 0)
                    {
                        _TimerGaze.fillAmount = 0;
                        _TimerGaze_Shadow.fillAmount = 0;
                    }
                    if (_MyTurnTimer <= 0.0f)
                        _ClientMng.LoseThisGame();
                }
                else
                {
                    _EnemyTimer -= Time.smoothDeltaTime;
                    _TimerGaze.fillAmount = _EnemyTimer / _TurnTime;
                    _TimerGaze_Shadow.fillAmount = _EnemyTimer / _TurnTime;
                    if (_EnemyTimer <= 0)
                    {
                        _TimerGaze.fillAmount = 0;
                        _TimerGaze_Shadow.fillAmount = 0;
                    }
                }
            }
            for(int i=0;i<5;i++)
            {
                _UIShadows[i].SetActive(!IsMyTurn());
            }
        }
    }

    public void GameStart(string color)
    {
        for (int i = 0; i < _Stones.Count; i++)
        {
            _Stones[i].Clean(_SoundMng.isSoundOn());
            _UseStoneNumber = 0;
        }
        _SceneMng.OpenOmGamePopup();
        _SceneMng.CloseAlGamePopup();
        _SceneMng.CloseGameQuitPopup();
        _SceneMng.CloseOptionPopup();
        _SceneMng.CloseLogoutPopup();
        _SceneMng.CloseCreditPopup();
        _IsBlackTurn = true;
        _NowGameOm = true;
        _GameStarted = true;
        _MustGoLobby = false;
        _TimerOn = true;
        _ImBlack = color == "black" ? true : false;
        _OmGameMng.GameStart();
        
        if (_ImBlack)
        {
            _ChangeStoneCount_My = 2;
            _ChangeStoneCount_Enemy = 3;
            _MyColorIcon.sprite = _OmGameMng._SpareStoneImage_Black;
            _EnemyColorIcon.sprite = _OmGameMng._SpareStoneImage_White;
        }
        else
        {
            _ChangeStoneCount_My = 3;
            _ChangeStoneCount_Enemy = 2;
            _MyColorIcon.sprite = _OmGameMng._SpareStoneImage_White;
            _EnemyColorIcon.sprite = _OmGameMng._SpareStoneImage_Black;
        }

        

        _OmGameUI.SetActive(true);
        _AlGameUI.SetActive(false);


        _MyTurnTimer = _TurnTime;
        _EnemyTimer = _TurnTime;
    }

    public void GameEndAndGoLobby()
    {
        _GameStarted = false;
        _MustGoLobby = false;
        _LoginMng.ResetEnemyRecord();

        _LoginMng.UpdateRecordBlocks();
        _SceneMng.CloseWinnerPopup();
        _SceneMng.CloseLoserPopup();
        _SceneMng.CloseDrawPopup();
        _SceneMng.CloseGameScene();
        _SceneMng.CloseOmGamePopup();
        _SceneMng.CloseAlGamePopup();
        //_SceneMng.OpenLobbyScene();

        _SceneMng.CloseEnemyReady_ChangePopup();
        _SceneMng.CloseEnemyReady_NormalPopup();
    }

    public void UseChangeStone()
    {
        _ChangeStoneCount_My--;
        _ClientMng.SendDataToServer("cc" + _ChangeStoneCount_My.ToString());
    }
    public void EnemyChangeStoneCountSet(int n)
    {
        _ChangeStoneCount_Enemy = n;
    }

    public Stone UseStone()
    {
        if (_UseStoneNumber >= 250)
            _UseStoneNumber = 0;
        Stone stone = _Stones[_UseStoneNumber];
        _UseStoneNumber += 1;
        return stone;
    }

    public void GameChange_GetData(string str)
    {
        if(str == "toal")
        {
            _OmGameMng.ChangeToAlGame();
            
        }
        else if(str == "toom")
        {
            _AlGameMng.ChangeToOmGame_GetData();
            
        }
        _ClientMng.SendDataToServer("getdataokchange");
    }

    public void GameChange_PopupChange_ToOm()
    {
        _NowGameOm = true;
        _SoundMng.EffectSoundPlay("change");
        _ChangeGameEffect_ToOm.Init(3.0f);
        _SceneMng.CloseAlGamePopup();
        _SceneMng.OpenOmGamePopup();
        _OmGameUI.SetActive(true);
        _AlGameUI.SetActive(false);
    }
    public void GameChange_PopupChange_ToAl()
    {
        _NowGameOm = false;
        _SoundMng.EffectSoundPlay("change");
        _ChangeGameEffect_ToAl.Init(3.0f);
        for (int i = 0; i < _Stones.Count; i++)
            _Stones[i].SetRigidbodyOn();
        _SceneMng.OpenAlGamePopup();
        _SceneMng.CloseOmGamePopup();
        _OmGameUI.SetActive(false);
        _AlGameUI.SetActive(true);

        _AlGameMng.GameStart();
    }

    public AlStoneDataFormat GetStoneData(int i)
    {
        if (_Stones[i].IsUsing())
        {
            AlStoneDataFormat data = new AlStoneDataFormat(i.ToString(), _Stones[i].IsBlack().ToString(),
                _Stones[i].transform.localPosition.x.ToString(), _Stones[i].transform.localPosition.y.ToString(), "t");
            return data;
        }
        else
        {
            AlStoneDataFormat data = new AlStoneDataFormat(i.ToString(), "", "", "", "f");
            return data;
        }
    }
    public AlStoneDataFormat GetStoneData_ForSpinning(int i)
    {
        if (_Stones[i].IsUsing())
        {
            AlStoneDataFormat data = new AlStoneDataFormat(i.ToString(), _Stones[i].IsBlack().ToString(),
                _Stones[i]._SpinningTargetPos.x.ToString(), _Stones[i]._SpinningTargetPos.y.ToString(), "t");
            return data;
        }
        else
        {
            AlStoneDataFormat data = new AlStoneDataFormat(i.ToString(), "", "", "", "f");
            return data;
        }
    }

    public AlStoneDataFormat GetStoneData_Om(int i)
    {
        if (_Stones[i].IsUsing())
        {
            AlStoneDataFormat data = new AlStoneDataFormat(i.ToString(), _Stones[i].IsBlack().ToString(),
                _Stones[i].GetPos_X().ToString(), _Stones[i].GetPos_Y().ToString(), "t"); ;
            return data;
        }
        else
        {
            AlStoneDataFormat data = new AlStoneDataFormat(i.ToString(), "", "", "", "f");
            return data;
        }
    }

    public void SetStone_ForAlGet(JsonData data)
    {
        int num = int.Parse(data["n"].ToString());
        if (data["u"].ToString()=="t")
        {
            bool black;
            if (data["b"].ToString() == "True")
                black = true;
            else
                black = false;

            _Stones[num].SettingForAl(black,float.Parse(data["x"].ToString()),float.Parse(data["y"].ToString()));
        }
        else if(data["u"].ToString() == "f")
        {
            _Stones[num].DestroyStone();
        }
    }

    public void SetStone_ForOmGet(JsonData data)
    {
        int num = int.Parse(data["n"].ToString());
        if (data["u"].ToString() == "t")
        {
            bool black;
            if (data["b"].ToString() == "True")
                black = true;
            else
                black = false;

            int x = int.Parse(data["x"].ToString());
            int y = int.Parse(data["y"].ToString());
            _OmGameMng._Board[y, x] = black ? 1 : 2;
            _Stones[num].SettingForOm(black, x, y);
        }
        else if (data["u"].ToString() == "f")
        {
            _Stones[num].DestroyStone();
        }
    }

    public bool CheckAllStoneStop()
    {
        bool check = true;

        for(int i=0;i< _Stones.Count; i++)
        {
            if(_Stones[i].IsUsing())
            {
                if (!_Stones[i].IsStop())
                    check = false;
            }
        }

        return check;
    }

    public void CheckStoneOutOfBoard()
    {
        for(int i=0;i<_Stones.Count;i++)
        {
            if(_Stones[i].IsUsing())
            {
                if (_Stones[i].OutOfBoard())
                    _Stones[i].DestroyAndEffect();
            }
        }
    }

    public void SetWinStoneEffect(List<StonePos> list)
    {
        for(int i=0;i<_Stones.Count;i++)
        {
            for(int j=0;j<list.Count;j++)
            {
                if (_Stones[i].GetPos_X() == list[j].x && _Stones[i].GetPos_Y() == list[j].y)
                    _Stones[i].OnOmocWinnerEffect();
            }
        }
    }

    public void RePositionStoneToOmGame()
    {
        for (int i = 0; i < _Stones.Count; i++)
        {
            if (_Stones[i].IsUsing())
            {
                Vector2 nowpos = _Stones[i].transform.localPosition - _BoardZeroPos + new Vector3(35, 35, 0);
                int x = (int)(nowpos.x / 70);
                int y = (int)(nowpos.y / 70);
                int n = _Stones[i].IsBlack() ? 1 : 2;
                if (_OmGameMng._Board[y, x] != 0)
                {
                    for (int j = 0; j < _Stones.Count; j++)
                    {
                        if (_Stones[j].IsUsing())
                        {
                            if (_Stones[j].GetPos_X() == x && _Stones[j].GetPos_Y() == y)
                            {
                                _Stones[j].DestroyAndEffect();
                                break;
                            }
                        }
                    }

                }
                _OmGameMng._Board[y, x] = n;
                _Stones[i].SetRePosition(x, y);
            }
        }

        List<AlStoneDataFormat> list = new List<AlStoneDataFormat>();
        for (int i = 0; i < StonesCount(); i++)
            list.Add(GetStoneData_Om(i));
        JsonData data = JsonMapper.ToJson(list);
        _ClientMng.SendDataToServer("oc" + data.ToString());

        _OmGameMng.WinOrDrawCheck();
    }

    public void SpinBoardAlGame(bool isClockVector)
    {
        _AlGameMng._Spinning = true;
        for (int i=0;i<_Stones.Count;i++)
        {
            if(_Stones[i].IsUsing())
            {
                Vector2 pos = _Stones[i].transform.localPosition - _BoardCenterPos;//보드의중점
                if (isClockVector)
                    pos = new Vector2(pos.y, pos.x * -1);
                else
                    pos = new Vector2(pos.y * -1, pos.x);
                pos += new Vector2(_BoardCenterPos.x,_BoardCenterPos.y);
                //pos = 목표위치
                _Stones[i].SetRigidbodyOff();

                float dis = Vector2.Distance(_BoardCenterPos, _Stones[i].transform.localPosition);
                float angle = Mathf.Atan2(_Stones[i].transform.localPosition.y - _BoardCenterPos.y,
                    _Stones[i].transform.localPosition.x - _BoardCenterPos.x) * Mathf.Rad2Deg;

                _Stones[i].SetSpinningAnimationAlGame(isClockVector, dis, angle, pos);



                //_Stones[i].SetPositioningAnimation(pos,8);
            }
        }

        List<AlStoneDataFormat> list = new List<AlStoneDataFormat>();
        for (int i = 0; i < StonesCount(); i++)
            list.Add(GetStoneData_ForSpinning(i));
        JsonData data = JsonMapper.ToJson(list);
        _ClientMng.SendDataToServer("as" + data.ToString());
        
        StartCoroutine(SpinnerArrowOn());
    }

    IEnumerator SpinnerArrowOn()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < _Stones.Count; i++)
        {
            if (_Stones[i].IsUsing())
            {
                _Stones[i].SetRigidbodyOn();
            }
        }
        _AlGameMng._Spinning = false;

    }

    public void EnemyReadyPopupSet_Normal(bool b)
    {
        if (b)
            _SceneMng.OpenEnemyReady_NormalPopup();
        else
        {
            _TimerOn = true;
            _SceneMng.CloseEnemyReady_NormalPopup();
        }
    }

    public void EnemyReadyPopupSet_Change(bool b)
    {
        if (b)
            _SceneMng.OpenEnemyReady_ChangePopup();
        else
            _SceneMng.CloseEnemyReady_ChangePopup();
    }

    public bool IsMyTurn()
    {
        return !(_IsBlackTurn ^ _ImBlack);
    }

    public void TurnChange()
    {
        _IsBlackTurn = !_IsBlackTurn;
        if(_NowGameOm)
        {
            for (int i = 0; i < _Stones.Count; i++)
            {
                _Stones[i].SetRigidbodyOff();
            }
        }
        else
        {
            if (IsMyTurn())
            {
                for (int i = 0; i < _Stones.Count; i++)
                {
                    _Stones[i].SetRigidbodyOn();
                }
            }
            else
            {
                for (int i = 0; i < _Stones.Count; i++)
                {
                    _Stones[i].SetRigidbodyOff();
                }
            }
        }
        _MyTurnTimer = _TurnTime;
        _EnemyTimer = _TurnTime;
    }

    public bool ImBlack() { return _ImBlack; }

    public int GetMyBlockNumber() { return _ImBlack ? 1 : 2; }
    public int GetEnemyBlockNumber() { return _ImBlack ? 2 : 1; }
    public bool NowGameOmoc() { return _NowGameOm; }
    public int StonesCount() { return _Stones.Count; }
    public int CanUseChangeStoneCount() { return _ChangeStoneCount_My; }
}

public class AlStoneDataFormat
{
    public string n;
    public string b;
    public string x;
    public string y;
    public string u;
    public AlStoneDataFormat(string num,string isblack,string xpos,string ypos,string isuse)
    {
        n = num;
        b = isblack;
        x = xpos;
        y = ypos;
        u = isuse;
    }
}


class IngameDataFormat
{
    public string gametype;
    public string data;

    public IngameDataFormat(string g,string d)
    {
        gametype = g;
        data = d;
    }
}