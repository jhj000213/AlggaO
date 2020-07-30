using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OmGameMng : MonoBehaviour
{
    [SerializeField]
    ClientMng _ClientMng;

    [SerializeField]
    GameMng _GameMng;

    [SerializeField]
    SceneMng _SceneMng;

    [SerializeField]
    SoundMng _SoundMng;

    [SerializeField]
    GameObject _TargetPointer;

    [SerializeField]
    Image[] _MySpareStoneImage = new Image[5];
    [SerializeField]
    Image[] _EnemySpareStoneImage = new Image[5];
    public Sprite _SpareStoneImage_Black;
    public Sprite _SpareStoneImage_White;

    int[] _xp = { 0, 1, 1, 1, 0, -1, -1, -1 };
    int[] _yp = { 1, 1, 0, -1, -1, -1, 0, 1 };
    public int[,] _Board = new int[15,15];

    int _SpareStoneCount = 0;
    int _SpareStoneCount_Enemy = 0;

    int _NowTargetPos_X = 7;
    int _NowTargetPos_Y = 7;

    const float DisplayWidth = 1080.0f;
    const float DisplayHeight = 1920.0f;
    const float BoardSize = 980;

    bool _WantUseChangeStone_OmGame;
    bool _FirstOmGame;
    [SerializeField]
    GameObject _NowUsingStone_Normal;
    [SerializeField]
    GameObject _NowUsingStone_Change;
    [SerializeField]
    GameObject _UsingStoneChangeGray;

    List<StonePos> _WinStonesPos = new List<StonePos>();

    void Update()
    {
        if (_GameMng.IsMyTurn())
            _TargetPointer.SetActive(true);
        else
            _TargetPointer.SetActive(false);

        if (Input.GetMouseButton(0))
        {
            Vector2 nowPos = new Vector2(Input.mousePosition.x * (DisplayWidth / Screen.width), Input.mousePosition.y * (DisplayHeight / Screen.height));
            if (nowPos.x >= 50.0f && nowPos.x <= 1030.0f && nowPos.y >= 800.0f && nowPos.y <= 1780.0f)
            {
                Vector2 newPos = (nowPos - new Vector2(_GameMng._BoardZeroPos.x - 35.0f, _GameMng._BoardZeroPos.y - 35.0f));
                _NowTargetPos_X = (int)(newPos.x / (BoardSize / 14));
                _NowTargetPos_Y = (int)(newPos.y / (BoardSize / 14));
                Debug.Log(_NowTargetPos_X+" : "+ _NowTargetPos_Y);

            }
        }
        _TargetPointer.transform.localPosition = new Vector2(_NowTargetPos_X * (BoardSize / 14), _NowTargetPos_Y * (BoardSize / 14));

        for (int i = 0; i < 5; i++)
        {
            _MySpareStoneImage[i].gameObject.SetActive(false);
            _EnemySpareStoneImage[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _SpareStoneCount; i++)
            _MySpareStoneImage[i].gameObject.SetActive(true);
        for (int i = 0; i < _SpareStoneCount_Enemy; i++)
            _EnemySpareStoneImage[i].gameObject.SetActive(true);

        _UsingStoneChangeGray.SetActive(_GameMng.CanUseChangeStoneCount() == 0 || _SpareStoneCount_Enemy == 0 || _FirstOmGame);
        _NowUsingStone_Normal.SetActive(!_WantUseChangeStone_OmGame);
        _NowUsingStone_Change.SetActive(_WantUseChangeStone_OmGame);

    }

    public void StoneSet_MySelf()
    {
        if(_GameMng.IsMyTurn() && _SpareStoneCount!=0)
        {
            if(_Board[_NowTargetPos_Y,_NowTargetPos_X]==0)
            {
                _SpareStoneCount--;
                _GameMng._TimerOn = false;
                _GameMng.UseStone().InitOm(_GameMng.ImBlack(), _NowTargetPos_X, _NowTargetPos_Y);
                _Board[_NowTargetPos_Y, _NowTargetPos_X] = _GameMng.GetMyBlockNumber();
                OmSendDataFormat data = new OmSendDataFormat(_NowTargetPos_X.ToString(), _NowTargetPos_Y.ToString());
                string item = JsonUtility.ToJson(data);
                _GameMng.TurnChange();
                _SoundMng.EffectSoundPlay("setstone");
                _GameMng.EnemyReadyPopupSet_Normal(true);
                _ClientMng.SendDataToServer("om" + item);
                WinOrDrawCheck();
                _WantUseChangeStone_OmGame = false;
            }
        }
    }

    public void WinOrDrawCheck()
    {
        if (CheckOmocWin())
        {
            _GameMng.EnemyReadyPopupSet_Normal(false);
            _GameMng.EnemyReadyPopupSet_Change(false);
            if (CheckEnemyOmoc())
                _ClientMng.DrawThisGame();
            else
                _ClientMng.WinThisGame();
        }
        else if (_SpareStoneCount_Enemy == 0 && _SpareStoneCount == 0)
        {
            _GameMng.EnemyReadyPopupSet_Change(true);
            ChangeToAlGame();
            _GameMng._NowGameOm = false;
            _FirstOmGame = false;
            _ClientMng.SendDataToServer("changetoal");
            
        }
        else if(_WantUseChangeStone_OmGame)
        {
            _GameMng.UseChangeStone();
            _GameMng.EnemyReadyPopupSet_Change(true);
            ChangeToAlGame();
            _GameMng._NowGameOm = false;
            _FirstOmGame = false;
            _ClientMng.SendDataToServer("changetoal");
        }
    }

    public void StoneSet_GetData(OmSendDataFormat data)
    {
        _NowTargetPos_X = int.Parse(data.setblockx);
        _NowTargetPos_Y = int.Parse(data.setblocky);
        _GameMng.UseStone().InitOm(!_GameMng.ImBlack(), _NowTargetPos_X, _NowTargetPos_Y);
        _Board[_NowTargetPos_Y, _NowTargetPos_X] = _GameMng.GetEnemyBlockNumber();
        _GameMng.TurnChange();
        _SoundMng.EffectSoundPlay("setstone");
        _ClientMng.SendDataToServer("getdataoknormal");
        _SpareStoneCount_Enemy--;
    }

    public void WantUseNormalStone()
    {
        if (_GameMng.IsMyTurn())
            _WantUseChangeStone_OmGame = false;
    }

    public void WantUseChangeStone()
    {
        if (_GameMng.CanUseChangeStoneCount() > 0 && _SpareStoneCount_Enemy>0 && _GameMng.IsMyTurn())
            _WantUseChangeStone_OmGame = true;
    }

    public void ChangeToAlGame()
    {
        _GameMng.GameChange_PopupChange_ToAl();
        for(int i=0;i<15;i++)
        {
            for(int j=0;j<15;j++)
            {
                _Board[j, i] = 0;
            }
        }
        _SpareStoneCount = 5;
        _SpareStoneCount_Enemy = 5;
        _WantUseChangeStone_OmGame = false;
        _FirstOmGame = false;
        SetPointerCenter();
    }

    public void SetRePosition_OmGame(LitJson.JsonData data)
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
                _Board[i, j] = 0;
        }

        for (int i=0;i<data.Count;i++)
        {
            _GameMng.SetStone_ForOmGet(data[i]);
        }
        WinOrDrawCheck();
        _FirstOmGame = false;
        SetPointerCenter();
    }

    public void GameStart()
    {
        for(int i=0;i<15;i++)
        {
            for(int j=0;j<15;j++)
                _Board[i, j] = 0;
        }
        SetPointerCenter();
        _SpareStoneCount = 5;
        _SpareStoneCount_Enemy = 5;
        _WantUseChangeStone_OmGame = false;
        for (int i=0;i<5;i++)
        {
            if(_GameMng.ImBlack())
            {
                _MySpareStoneImage[i].sprite = _SpareStoneImage_Black;
                _EnemySpareStoneImage[i].sprite = _SpareStoneImage_White;
            }
            else
            {
                _MySpareStoneImage[i].sprite = _SpareStoneImage_White;
                _EnemySpareStoneImage[i].sprite = _SpareStoneImage_Black;
            }
            _MySpareStoneImage[i].gameObject.SetActive(true);
            _EnemySpareStoneImage[i].gameObject.SetActive(true);
        }
        _FirstOmGame = true;
    }
    public void GameEnd()
    {
        for (int i = 0; i < 5; i++)
        {
            _MySpareStoneImage[i].gameObject.SetActive(false);
            _EnemySpareStoneImage[i].gameObject.SetActive(false);
        }
    }

    public void SetPointerCenter()
    {
        _NowTargetPos_X = 7;
        _NowTargetPos_Y = 7;
    }

    public void LoseAfterEffectCheck()
    {
        CheckEnemyOmoc();

        _SceneMng.OpenLoserPopup_Cor();
    }
    public void DrawAfterEffectCheck()
    {
        CheckOmocWin();
        CheckEnemyOmoc();
        _SceneMng.OpenDrawPopup_Cor();
    }

   

    bool CheckOmocWin()
    {
        bool result = false;
        _WinStonesPos.Clear();
        for (int y = 0; y < 15; y++)
        {
            for (int x = 0; x < 15; x++)
            {
                if(_Board[y,x]!=0)
                {
                    for(int i=0;i<8;i++)
                    {
                        if (CheckLine(_Board[y, x], x + _xp[i], y + _yp[i], i, 0))
                        {
                            _WinStonesPos.Add(new StonePos(x, y));
                            if (_Board[y, x] == _GameMng.GetMyBlockNumber())
                            {
                                result = true;
                                _GameMng.SetWinStoneEffect(_WinStonesPos);
                                break;
                            }
                            else
                                _WinStonesPos.Clear();
                        }
                        else
                            _WinStonesPos.Clear();
                    }
                }
            }
        }
        return result;
    }
    bool CheckEnemyOmoc()
    {
        bool result = false;
        _WinStonesPos.Clear();

        for (int y = 0; y < 15; y++)
        {
            for (int x = 0; x < 15; x++)
            {
                if (_Board[y, x] != 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (CheckLine(_Board[y, x], x + _xp[i], y + _yp[i], i, 0))
                        {
                            _WinStonesPos.Add(new StonePos(x,y));
                            if (_Board[y, x] == _GameMng.GetEnemyBlockNumber())
                            {
                                _GameMng.SetWinStoneEffect(_WinStonesPos);
                                result = true;
                                break;
                            }
                            else
                                _WinStonesPos.Clear();
                        }
                        else
                            _WinStonesPos.Clear();
                    }
                }
            }
        }
        return result;
    }

    bool CheckLine(int color,int xp,int yp,int vector,int addnum)
    {
        addnum++;
        if (addnum > 4)
            return true;

        if (xp >= 15 || yp >= 15 || xp < 0 || yp < 0)
        {
            return false;
        }
        else if (_Board[yp, xp] != color)
        {
            return false;
        }
        else
        {
            _WinStonesPos.Add(new StonePos(xp, yp));
            return CheckLine(color, xp + _xp[vector], yp + _yp[vector], vector, addnum);
        }
    }
}

public class StonePos
{
    public int x;
    public int y;
    public StonePos(int a,int b)
    {
        x = a;
        y = b;
    }
}

public class OmSendDataFormat
{
    public string setblockx;
    public string setblocky;

    public OmSendDataFormat(string x,string y)
    {
        setblockx = x;
        setblocky = y;
    }
}