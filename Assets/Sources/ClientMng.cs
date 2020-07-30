using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;
using LitJson;

public class StringQueue
{
    List<string> _DataStream = new List<string>();

    public void Add(string str)
    {
        _DataStream.Add(str);
    }

    public string getData()
    {
        string str = _DataStream[0];
        _DataStream.RemoveAt(0);
        return str;
    }

    public bool IsEmpty()
    {
        return _DataStream.Count >= 1 ? false : true;
    }
}

public class ReadMng
{
    ClientMng cmng;
    StringQueue StringQueue;
    StreamReader reader;
    public ReadMng(ClientMng c,StringQueue sq,StreamReader r)
    {
        cmng = c;
        StringQueue = sq;
        reader = r;
    }

    public void Reading()
    {
        while (true)
        {
            string str = cmng.readerStream.ReadLine();
            StringQueue.Add(str);
        }
    }
}

public class ClientMng : MonoBehaviour
{
    [SerializeField]
    GameMng _GameMng;
    

    StringQueue _StringQueue = new StringQueue();

    bool _On = true;
    bool _getEnemyId = false;
    string _EnemyId = "";

    [SerializeField]
    SceneMng _SceneMng;
    [SerializeField]
    LoginMng _LoginMng;

    NetworkStream networkStream;
    public StreamReader readerStream;
    StreamWriter writerStream;
    TcpClient client = new TcpClient();
    Thread readmngThread;
    
    void Start()
    {
        try
        {
            client.Connect("52.141.20.150", 7125);
            //client.Connect("192.168.200.172", 7125);

            //client.Connect("113.131.43.152", 7125);
            networkStream = client.GetStream();
            Encoding encode = Encoding.GetEncoding("UTF-8");
            readerStream = new StreamReader(networkStream);
            writerStream = new StreamWriter(networkStream);

            

            ReadMng readmng = new ReadMng(this,_StringQueue,readerStream);
            readmngThread = new Thread(new ThreadStart(readmng.Reading));
            readmngThread.Start();

            StartCoroutine(Reading(_StringQueue));

        }
        catch (Exception exp)
        {
            Debug.LogError(exp.ToString());
        }

    }

    private void Update()
    {
        if(_getEnemyId)
        {
            _getEnemyId = false;
            _LoginMng.GetEnemyName_Ready(_EnemyId);
            //_LoginMng.GetEnemyRecords(_EnemyId);
        }
        if(_GameMng._GameStarted && _EnemyId!="")
            _LoginMng.GetEnemyName_Ready(_EnemyId);
    }

    IEnumerator Reading(StringQueue sq)
    {
        while(_On)
        {
            yield return null;
            string str = "";
            if(!sq.IsEmpty())
            {
                str = sq.getData();

                if (str != "")
                {
                    Debug.Log(str);
                    if (str[0] == '{')
                    {
                        SendDataFormat data = JsonUtility.FromJson<SendDataFormat>(str);
                        if (data.type == "enemyid")
                        {
                            _EnemyId = data.data;
                            _getEnemyId = true;
                        }
                    }
                    else if (str[0] == 'o' && str[1] == 'm')
                    {
                        string data = str.Replace("om", "");
                        OmSendDataFormat dd = JsonUtility.FromJson<OmSendDataFormat>(data);
                        _GameMng._OmGameMng.StoneSet_GetData(dd);
                    }
                    else if (str[0] == 'o' && str[1] == 'c')
                    {
                        string data = str.Replace("oc", "");
                        _GameMng.GameChange_GetData("toom");
                        JsonData item = JsonMapper.ToObject(data);
                        _GameMng._OmGameMng.SetRePosition_OmGame(item);
                    }
                    else if (str[0] == 's' && str[1] == 'h')
                    {
                        _GameMng._AlGameMng.TurnOffEnemySlot(str[2] - '0');
                    }
                    else if (str[0] == 'a' && str[1] == 'l')
                    {
                        string data = str.Replace("al", "");
                        JsonData item = JsonMapper.ToObject(data);
                        _GameMng._AlGameMng.SetStones_GetData(item);
                    }
                    else if (str[0] == 'a' && str[1] == 's')
                    {
                        string data = str.Replace("as", "");
                        JsonData item = JsonMapper.ToObject(data);
                        _GameMng._AlGameMng.SetStones_GetData(item);
                    }
                    else if (str[0] == 'c' && str[1] == 'c')
                    {
                        _GameMng.EnemyChangeStoneCountSet(str[2] - '0');
                    }
                    else if (str == "stonestoped")
                    {
                        _GameMng._AlGameMng.TurnChange_GetData();
                    }
                    else if (str == "black" || str == "white")
                    {
                        Matched_GameStart(str);
                    }
                    else if (str == "youlose")
                    {
                        _GameMng._OmGameMng.LoseAfterEffectCheck();
                    }
                    else if (str == "draw")
                    {
                        _GameMng._OmGameMng.LoseAfterEffectCheck();
                    }
                    else if (str == "getdataoknormal")
                    {
                        _GameMng.EnemyReadyPopupSet_Normal(false);
                    }
                    else if (str == "getdataokchange")
                    {
                        _GameMng.EnemyReadyPopupSet_Change(false);
                    }
                    else if (str == "changetoal")
                    {
                        _GameMng.GameChange_GetData("toal");
                    }
                    else if (str == "youwintime")
                    {
                        _SceneMng.OpenWinnerPopup_Time();
                    }
                    else if (str == "youlosetime")
                        _SceneMng.OpenLoserPopup_Time();
                    else if (str == "youwindisconnect")
                    {
                        WinThisGame_Disconnect();
                    }
                }
                else
                {
                    SendDataToServer("enemyout");
                    Debug.Log("empty");
                }
            }
            
        }
    }

    void Matched_GameStart(string str)
    {
        _SceneMng.CloseMatchingPopup();
        _SceneMng.OpenGameScene();
        //_SceneMng.CloseLobbyScene();
        _GameMng.GameStart(str);
        Debug.Log(_LoginMng._user.UserId);
        SendDataFormat data = new SendDataFormat("enemyid", _LoginMng._user.UserId);
        string item = JsonUtility.ToJson(data);
        SendDataToServer(item);//id
    }
    

    public void WinThisGame()
    {
        if(!_GameMng._MustGoLobby)
        {
            _GameMng._MustGoLobby = true;
            _LoginMng.UpRecord("win", _LoginMng._user.UserId);
            _LoginMng.UpRecord("lose", _EnemyId);
            _LoginMng.SaveBattleRecordWithEnemy(_LoginMng._user.UserId, _LoginMng.GetEnemyName(), "win");
            _LoginMng.SaveBattleRecordWithEnemy(_EnemyId, _LoginMng._UserRecordValue[3], "lose");
            SendDataToServer("win");
            _SceneMng.OpenWinnerPopup_Cor();
        }
    }

    void WinThisGame_Disconnect()
    {
        if (!_GameMng._MustGoLobby)
        {
            _GameMng._MustGoLobby = true;
            _LoginMng.UpRecord("win", _LoginMng._user.UserId);
            _LoginMng.UpRecord("lose", _EnemyId);
            _LoginMng.SaveBattleRecordWithEnemy(_LoginMng._user.UserId, _LoginMng.GetEnemyName(), "win");
            _LoginMng.SaveBattleRecordWithEnemy(_EnemyId, _LoginMng._UserRecordValue[3], "lose");
            SendDataToServer("win");
            _SceneMng.OpenWinnerPopup_Disconnect();
        }
    }

    IEnumerator EnemyLose()
    {
        yield return new WaitForSeconds(1.5f);
        
    }

    public void DrawThisGame()
    {
        if(!_GameMng._MustGoLobby)
        {
            _GameMng._MustGoLobby = true;
            _LoginMng.UpRecord("draw", _LoginMng._user.UserId);
            _LoginMng.UpRecord("draw", _EnemyId);
            _LoginMng.SaveBattleRecordWithEnemy(_LoginMng._user.UserId, _LoginMng.GetEnemyName(), "draw");
            _LoginMng.SaveBattleRecordWithEnemy(_EnemyId, _LoginMng._UserRecordValue[3], "draw");
            SendDataToServer("draw");
            _SceneMng.OpenDrawPopup_Cor();
        }
    }

    public void LoseThisGame()
    {
        if (!_GameMng._MustGoLobby)
        {
            _GameMng._MustGoLobby = true;
            _LoginMng.UpRecord("lose", _LoginMng._user.UserId);
            _LoginMng.UpRecord("win", _EnemyId);
            _LoginMng.SaveBattleRecordWithEnemy(_LoginMng._user.UserId, _LoginMng.GetEnemyName(), "lose");
            _LoginMng.SaveBattleRecordWithEnemy(_EnemyId, _LoginMng._UserRecordValue[3], "win");
            SendDataToServer("lose");
            _SceneMng.OpenLoserPopup_Time();
        }
    }


    public void SendDataToServer(string str)
    {
        if(_On)
        {
            if (client.Connected)
            {

                writerStream.WriteLine(str);
                writerStream.Flush();
                Debug.Log(str);
                if (str == "quit")
                {
                    Debug.Log("Quit");
                    _On = false;
                    readmngThread.Interrupt();
                    readmngThread.Abort();
                }
            }
            else
                _SceneMng.OpenQuitPopup();
        }
    }



    public void MatchingStart()
    {
        if (_LoginMng._CanMatching)
        {
            SendDataToServer("match");
            _SceneMng.OpenMatchingPopup();
        }
    }
    public void MatchingCancel()
    {
        SendDataToServer("matchcancel");
        _SceneMng.CloseMatchingPopup();
    }
    void DisconnectClient()
    {
        SendDataToServer("quit");
    }

    public void QuitGame()
    {
        DisconnectClient();
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        DisconnectClient();
    }
}

class SendDataFormat
{
    public string type;
    public string data;

    public SendDataFormat(string t,string d)
    {
        type = t;
        data = d;
    }
}