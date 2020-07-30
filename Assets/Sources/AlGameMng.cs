using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class AlGameMng : MonoBehaviour
{
    [SerializeField]
    ClientMng _ClientMng;

    [SerializeField]
    GameMng _GameMng;

    [SerializeField]
    GameObject _ShootVectorArrow;

    [SerializeField]
    GameObject _TouchBlocking;

    [SerializeField]
    Sprite _StoneImage_Black;
    [SerializeField]
    Sprite _StoneImage_White;

    int _NowSelectStoneNumber;

    bool _NowTouchDown = false;
    bool _Shooted;

    int _SpareStoneCount_My = 0;
    int _SpareStoneCount_Enemy = 0;
    [SerializeField]
    Text _SpareCountText_Temp;

    [SerializeField]
    GameObject _ShootSlotsGroup_My;
    [SerializeField]
    GameObject _ShootLostsGroup_MyArrows;
    [SerializeField]
    GameObject _ShootSlotsGroup_Enemy;


    bool _WantUseChangeStone_AlGame;
    [SerializeField]
    GameObject _NowUsingStone_Normal;
    [SerializeField]
    GameObject _NowUsingStone_Change;
    [SerializeField]
    GameObject _UsingStoneChangeGray;

    public bool _Spinning = false;
    [SerializeField]
    GameObject _BoardSpinArrow;


    bool[] _CanShootSlot_My = { false, false, false, false, false };
    bool[] _CanShootSlot_Enemy = { false, false, false, false, false };

    [SerializeField]
    Image[] _CanShootSlotImage_My = new Image[5];
    [SerializeField]
    Image[] _CanShootSlotImage_Enemy = new Image[5];

    Vector2[] _ShootReadyStonePos = { new Vector2(180, 700), new Vector2(360, 700),
        new Vector2(540, 700), new Vector2(720, 700), new Vector2(900, 700) };

    public void GameStart()
    {
        _WantUseChangeStone_AlGame = false;
        
        for (int i=0;i<5;i++)
        {
            _CanShootSlot_My[i] = true;
            _CanShootSlot_Enemy[i] = true;
        }
        _NowSelectStoneNumber = -1;
        _ShootVectorArrow.SetActive(false);
        _TouchBlocking.SetActive(false);
        _Shooted = false;
        _Spinning = false;
        _SpareStoneCount_My = 5;
        _SpareStoneCount_Enemy = 5;

        _ShootSlotsGroup_My.SetActive(_GameMng.IsMyTurn());
        _ShootLostsGroup_MyArrows.SetActive(!_GameMng._NowGameOm);
        _ShootSlotsGroup_Enemy.SetActive(!_GameMng.IsMyTurn());
        for(int i=0;i<5;i++)
        {
            if (_GameMng.ImBlack())
            {
                _CanShootSlotImage_My[i].sprite = _StoneImage_Black;
                _CanShootSlotImage_Enemy[i].sprite = _StoneImage_White;
            }
            else
            {
                _CanShootSlotImage_My[i].sprite = _StoneImage_White;
                _CanShootSlotImage_Enemy[i].sprite = _StoneImage_Black;
            }
        }
    }
    

    public void ShootStone_Touch(int i)
    {
        if(_CanShootSlot_My[i] && _GameMng.IsMyTurn())
        {
            _TouchBlocking.SetActive(true);
            Debug.Log(i);
            _ShootVectorArrow.SetActive(true);
            _ShootVectorArrow.transform.localPosition = _ShootReadyStonePos[i];
            _NowSelectStoneNumber = i;
            _NowTouchDown = true;
        }
    }

    public void ShootStone_Shoot(Vector2 power)
    {
        
        _ShootVectorArrow.SetActive(false);
        _NowTouchDown = false;
        _GameMng._TimerOn = false;
        Stone stone = _GameMng.UseStone();
        stone.InitAl(_GameMng.ImBlack(), _ShootReadyStonePos[_NowSelectStoneNumber]);
        stone.ShootAl(power);

        _CanShootSlot_My[_NowSelectStoneNumber] = false;
        _ClientMng.SendDataToServer("sh" + _NowSelectStoneNumber.ToString());
        _NowSelectStoneNumber = -1;
        _SpareStoneCount_My--;
        _Shooted = true;
    }

    public void TurnOffEnemySlot(int i)
    {
        _CanShootSlot_Enemy[i] = false;
    }

    void StoneStoped()
    {
        _TouchBlocking.SetActive(false);
        _Shooted = false;
        _GameMng.CheckStoneOutOfBoard();
        _GameMng.TurnChange();
        _ShootSlotsGroup_My.SetActive(false);
        _ShootLostsGroup_MyArrows.SetActive(false);
        _ShootSlotsGroup_Enemy.SetActive(true);
        _GameMng.EnemyReadyPopupSet_Normal(true);
        _ClientMng.SendDataToServer("stonestoped");
        
        if (_SpareStoneCount_My == 0 && _SpareStoneCount_Enemy == 0)
        {
            _GameMng.EnemyReadyPopupSet_Change(true);
            ChangeToOmGame();
            _GameMng._NowGameOm = true;
        }
        else if(_WantUseChangeStone_AlGame)
        {
            _GameMng.UseChangeStone();
            _GameMng.EnemyReadyPopupSet_Change(true);
            ChangeToOmGame();
            _GameMng._NowGameOm = true;
        }
        _WantUseChangeStone_AlGame = false;
    }

    public void TurnChange_GetData()
    {
        _GameMng.CheckStoneOutOfBoard();
        _GameMng._UseStoneNumber++;
        _SpareStoneCount_Enemy--;
        _GameMng.TurnChange();
        _ShootSlotsGroup_My.SetActive(true);
        _ShootLostsGroup_MyArrows.SetActive(true);
        _ShootSlotsGroup_Enemy.SetActive(false);
        Debug.Log("changed");
        _ClientMng.SendDataToServer("getdataoknormal");
    }

    public void SetStones_GetData(JsonData data)
    {
        for(int i=0;i<data.Count;i++)
        {
            _GameMng.SetStone_ForAlGet(data[i]);
        }
    }

    public void TurnBoardToClockVector()//left 시계
    {
        _GameMng.SpinBoardAlGame(true);
    }

    public void TurnBoardToClockReverseVector()//right 반시계
    {
        _GameMng.SpinBoardAlGame(false);
    }

    public void WantUseNormalStone()
    {
        if (_GameMng.IsMyTurn())
            _WantUseChangeStone_AlGame = false;
    }

    public void WantUseChangeStone()
    {
        if (_GameMng.CanUseChangeStoneCount() > 0 && _SpareStoneCount_Enemy>0 && _GameMng.IsMyTurn())
            _WantUseChangeStone_AlGame = true;
    }

    private void Update()
    {
        if (_NowTouchDown)
        {
            Vector2 nowPos = new Vector2(Input.mousePosition.x * (1080.0f / Screen.width), Input.mousePosition.y * (1920.0f / Screen.height));
            

            float arrowangle = Mathf.Atan2(_ShootReadyStonePos[_NowSelectStoneNumber].y - nowPos.y, _ShootReadyStonePos[_NowSelectStoneNumber].x - nowPos.x) * Mathf.Rad2Deg;
            float dis = Vector2.Distance(_ShootReadyStonePos[_NowSelectStoneNumber], nowPos);
            _ShootVectorArrow.transform.localScale = new Vector3(dis / 66.0f, 1, 1);
            _ShootVectorArrow.transform.localEulerAngles = new Vector3(0,0, arrowangle);

            if (Input.GetMouseButtonUp(0))
            {
                if(nowPos.x>= _ShootReadyStonePos[_NowSelectStoneNumber].x-55.0f && nowPos.x <= _ShootReadyStonePos[_NowSelectStoneNumber].x + 55.0f &&
                    nowPos.y >= _ShootReadyStonePos[_NowSelectStoneNumber].y - 55.0f && nowPos.y <= _ShootReadyStonePos[_NowSelectStoneNumber].y + 55.0f)
                {
                    _TouchBlocking.SetActive(false);
                    _ShootVectorArrow.SetActive(false);
                    _NowTouchDown = false;
                    _NowSelectStoneNumber = -1;
                }
                else
                {
                    Vector2 power = _ShootReadyStonePos[_NowSelectStoneNumber] - nowPos;
                    Debug.Log(power);
                    ShootStone_Shoot(power * 4.5f);//ShootPower
                }
            }
        }
        
        for (int i = 0; i < 5; i++)
        {
            _CanShootSlotImage_My[i].gameObject.SetActive(_CanShootSlot_My[i]);
            _CanShootSlotImage_Enemy[i].gameObject.SetActive(_CanShootSlot_Enemy[i]);
        }

        _BoardSpinArrow.SetActive(_GameMng.IsMyTurn() && !_Shooted && !_Spinning);


        _UsingStoneChangeGray.SetActive(_GameMng.CanUseChangeStoneCount() == 0 || _SpareStoneCount_Enemy == 0);
        _SpareCountText_Temp.text = "my : " + _SpareStoneCount_My + "\nenemy : " + _SpareStoneCount_Enemy;
        _NowUsingStone_Normal.SetActive(!_WantUseChangeStone_AlGame);
        _NowUsingStone_Change.SetActive(_WantUseChangeStone_AlGame);
    }

    private void FixedUpdate()
    {
        if (_Shooted)
        {
            if (_GameMng.CheckAllStoneStop())
            {
                Debug.Log("stop");
                StoneStoped();
            }
            else
            {
                List<AlStoneDataFormat> list = new List<AlStoneDataFormat>();
                for (int i = 0; i < _GameMng.StonesCount(); i++)
                    list.Add(_GameMng.GetStoneData(i));
                JsonData data = JsonMapper.ToJson(list);
                //Debug.Log(data.ToString());
                _ClientMng.SendDataToServer("al" + data.ToString());
            }
        }
    }


    public void ChangeToOmGame()
    {
        _GameMng.RePositionStoneToOmGame();
        _GameMng.GameChange_PopupChange_ToOm();
        _WantUseChangeStone_AlGame = false;
        for (int i=0;i<5;i++)
        {
            _CanShootSlot_My[i] = false;
            _CanShootSlot_Enemy[i] = false;
        }
        _SpareStoneCount_My = 5;
        _SpareStoneCount_Enemy = 5;
    }
    public void ChangeToOmGame_GetData()
    {
        _GameMng.GameChange_PopupChange_ToOm();
        _WantUseChangeStone_AlGame = false;
        for (int i = 0; i < 5; i++)
        {
            _CanShootSlot_My[i] = false;
            _CanShootSlot_Enemy[i] = false;
        }
        _SpareStoneCount_My = 5;
        _SpareStoneCount_Enemy = 5;
    }
}