using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;

public class LoginMng : MonoBehaviour
{
    FirebaseApp firebaseApp;
    DatabaseReference databaseReference;

    [SerializeField]
    SceneMng _SceneMng;


    [SerializeField]
    GameObject _RecordLoadingCircle;
    public bool _CanMatching;
    

    //LoginScene
    [SerializeField]
    InputField _EmailField_Login;
    [SerializeField]
    InputField _PasswordField_Login;

    [SerializeField]
    InputField _EmailField_Account;
    [SerializeField]
    InputField _PasswordField_Account;
    [SerializeField]
    InputField _NicknameField;

    //LobbyScene
    public Text _UserName;
    public Text _UserWinCount;
    public Text _UserLoseCount;
    public Text _UserDrawCount;
    public Text _UserEmail;
    [SerializeField]
    RectTransform _RecordBlockObjectsContent;
    [SerializeField]
    RecordBlock[] _RecordBlockObjects;

    
    

    //
    FirebaseAuth auth;
    public FirebaseUser _user;

    bool _CheckNicknameChecker = false;
    string _CheckNicknameString = "";
    string _EnemyName = "";


    int _RecordBlockCount=-1;
    List<RecordString> _RecordBlockStrings = new List<RecordString>();

    /// <summary>
    ///  [0 - draw]
    ///  [1 - email]
    ///  [2 - lose]
    ///  [3 - name]
    ///  [4 = win]
    /// </summary>
    //public string[] _EnemyRecordValue = new string[5];
    /// <summary>
    ///  [0 - draw]
    ///  [1 - email]
    ///  [2 - lose]
    ///  [3 - name]
    ///  [4 = win]
    /// </summary>
    public string[] _UserRecordValue = new string[5];

    private void Start()
    {
        firebaseApp = FirebaseDatabase.DefaultInstance.App;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(ServerData.Data._FirebaseUrl);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseApp.DefaultInstance.SetEditorP12FileName(ServerData.Data._FirebaseFileName);
        FirebaseApp.DefaultInstance.SetEditorP12Password(ServerData.Data._FirebasePassword);

        _CanMatching = false;

        auth = FirebaseAuth.DefaultInstance;
        UserLoginCheck();
        if (auth.CurrentUser != null)
        {
            _SceneMng.OpenLobbyScene();
        }
        else
            _SceneMng.OpenLoginScene();

        //databaseReference.Child("records").SetValueAsync(null);



        //if (auth.CurrentUser != null)
        //    UpdateRecordBlocks();
    }


    public void CreateUser()
    {
        if (_PasswordField_Account.text.Length < 6)
        {
            _SceneMng.OpenTextLogPopup("비밀번호는 6자이상이어야 합니다");
            return;
        }
        else if(_NicknameField.text=="")
        {
            _SceneMng.OpenTextLogPopup("닉네임을 입력하세요");
            return;
        }
        else if(_NicknameField.text.Length>=9)
        {
            _SceneMng.OpenTextLogPopup("닉네임은 8자이하이어야 합니다");
            return;
        }
        else if(_EmailField_Account.text=="")
        {
            _SceneMng.OpenTextLogPopup("이메일을 입력하세요");
            return;
        }
        else if(!_EmailField_Account.text.Contains("@"))
        {
            _SceneMng.OpenTextLogPopup("이메일 형식으로 작성해주세요");
            return;
        }
        auth.CreateUserWithEmailAndPasswordAsync(_EmailField_Account.text, _PasswordField_Account.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                _SceneMng.OpenQuitPopup();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                _SceneMng.OpenQuitPopup();
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);


            User user = new User(_NicknameField.text, newUser.Email);
            string json = JsonUtility.ToJson(user);
            databaseReference.Child("users").Child(newUser.UserId).SetRawJsonValueAsync(json);

            Debug.Log("CreateUser Complete");
            LoginUser_NextAccount();
            UserLoginCheck();

        });

    }
    void LoginUser_NextAccount()
    {
        _SceneMng.CloseUserAccountPopup();
        LoginUser_NextAccount_Sub();
    }
    void LoginUser_NextAccount_Sub()
    {
        auth.SignInWithEmailAndPasswordAsync(_EmailField_Account.text, _PasswordField_Account.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                _SceneMng.OpenQuitPopup();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                _SceneMng.OpenTextLogPopup("아이디또는 비밀번호가 틀렸습니다");
                return;
            }
            FirebaseUser user = task.Result;
        });
    }

    public void LoginUser()
    {

        if (_EmailField_Login.text == "" || _PasswordField_Login.text == "")
        {
            _SceneMng.OpenTextLogPopup("이메일 또는 비밀번호를를 입력하세요");
            return;
        }
        LoginUser_Sub(_EmailField_Login.text, _PasswordField_Login.text);
    }

    void LoginUser_Sub(string em,string pw)
    {
        auth.SignInWithEmailAndPasswordAsync(em,pw ).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                _SceneMng.OpenQuitPopup();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                _SceneMng.OpenTextLogPopup("아이디또는 비밀번호가 틀렸습니다");
                return;
            }
            FirebaseUser user = task.Result;
        });
    }

    public void UpdateRecord()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").
                Child(_user.UserId).GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Error!!");
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snap = task.Result;
                        int i = 0;
                        foreach (var item in snap.Children)
                        {
                            _UserRecordValue[i] = item.Value.ToString();
                            i++;
                        }
                        
                    }
                });
    }

    public void UpdateRecordBlocks()
    {
        UpdateRecordBlocks_CountGet();
        UpdateRecordBlocks_BlockGet();
    }
    void UpdateRecordBlocks_CountGet()
    {
        FirebaseDatabase.DefaultInstance.GetReference("records").
                Child(_user.UserId).Child("count").GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Error!!");
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snap = task.Result;
                        if (task.Result.Value != null)
                            _RecordBlockCount = int.Parse(snap.Value.ToString());
                        else
                            _RecordBlockCount = 0;
                    }
                });
    }

    void UpdateRecordBlocks_BlockGet()
    {
        FirebaseDatabase.DefaultInstance.GetReference("records").
                Child(_user.UserId).Child("records").GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Error!!");
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snap = task.Result;
                        int i = 0;
                        foreach (var item in snap.Children)
                        {
                            RecordString bl = new RecordString();
                            bl.enemyname = item.Child("enemyid").Value.ToString();
                            bl.result = item.Child("result").Value.ToString();
                            _RecordBlockStrings.Add(bl);
                            i++;
                        }

                    }
                });
    }
    
    private void Update()
    {
        
        UserLoginCheck();
        if (auth.CurrentUser != null)
        {
            UpdateRecord();
            _UserName.text = _UserRecordValue[3];
            _UserWinCount.text = _UserRecordValue[4];
            _UserLoseCount.text = _UserRecordValue[2];
            _UserDrawCount.text = _UserRecordValue[0];
            _UserEmail.text = "ID : " + _UserRecordValue[1];
        }
        else
        {
            for (int i = 0; i < 5; i++)
                _UserRecordValue[i] = "";
            _UserName.text = "";
            _UserEmail.text = "";
            _UserWinCount.text = "";
            _UserLoseCount.text = "";
            _UserDrawCount.text = "";
        }
        if (_RecordBlockStrings.Count == _RecordBlockCount && _RecordBlockCount!=-1)
        {
            _RecordBlockStrings.Reverse();
            if (_RecordBlockStrings.Count > 10)
            {
                int count = _RecordBlockStrings.Count - 10;
                _RecordBlockStrings.RemoveRange(10, count);
            }
            _RecordBlockObjectsContent.transform.localPosition = new Vector3(0, 0, 0);
            _RecordBlockObjectsContent.sizeDelta = new Vector2(0, _RecordBlockStrings.Count * 144);
            for (int i = 0; i < 10; i++)
                _RecordBlockObjects[i].SetOff();
            Debug.Log(_RecordBlockStrings.Count);
            _CanMatching = true;
            for (int i = 0; i < _RecordBlockStrings.Count; i++)
            {
                _RecordBlockObjects[i].gameObject.SetActive(true);
                _RecordBlockObjects[i].Init(_RecordBlockStrings[i].enemyname, _RecordBlockStrings[i].result);
            }
            _RecordBlockStrings.Clear();
            _RecordBlockCount = -1;
        }

        _RecordLoadingCircle.SetActive(!_CanMatching);

    }
    
    void UserLoginCheck()
    {
        if (auth.CurrentUser != _user)
        {
            bool signedIn = _user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && _user != null)
            {
                Debug.Log("Signed out " + _user.UserId);
                _EmailField_Login.text = "";
                _PasswordField_Login.text = "";
                _EmailField_Account.text = "";
                _PasswordField_Account.text = "";
                _SceneMng.CloseOptionPopup();
                _SceneMng.OpenLoginScene();
                _SceneMng.CloseLobbyScene();
                
                for (int i = 0; i < 10; i++)
                    _RecordBlockObjects[i].gameObject.SetActive(false);
                for(int i=0;i<5;i++)
                    _UserRecordValue[i] = "";
            }
            _user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + _user.UserId);
                _SceneMng.CloseLoginScene();
                _SceneMng.CloseUserAccountPopup();
                _SceneMng.OpenLobbyScene();
                UpdateRecordBlocks();
            }
        }
    }
    
    public void OpenAccountPopup_LoginMng()
    {
        _SceneMng.OpenUserAccountPopup();
        _NicknameField.text = "";
        _EmailField_Account.text = "";
        _PasswordField_Account.text = "";
    }

    public void LogoutUser()
    {
        _CanMatching = false;
        auth.SignOut();
        UserLoginCheck();
        _RecordBlockCount = -1;
    }

    public void UpRecord(string rec,string id)
    {
        int count = -1;
        FirebaseDatabase.DefaultInstance.GetReference("users").
            Child(id).Child(rec).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    _SceneMng.OpenQuitPopup();
                else if (task.IsCompleted)
                {
                    DataSnapshot snap = task.Result;
                    count = int.Parse(snap.Value.ToString()) + 1;
                    databaseReference.Child("users").Child(id).Child(rec).SetValueAsync(count);
                }
            });
    }
    
    public void SaveBattleRecordWithEnemy(string myid,string enemyname,string result)
    {
        FirebaseDatabase.DefaultInstance.GetReference("records").
            Child(myid).Child("count").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    _SceneMng.OpenQuitPopup();
                else if (task.IsCompleted)
                {
                    DataSnapshot snap = task.Result;
                    string count;
                    if (snap.Value!=null)
                        count = (int.Parse(snap.Value.ToString())+1).ToString();
                    else
                        count = "1";
                    databaseReference.Child("records").Child(myid)
                    .Child("count").SetValueAsync(count);

                    databaseReference.Child("records").Child(myid)
                    .Child("records").Child(count).Child("result").SetValueAsync(result);

                    databaseReference.Child("records").Child(myid)
                    .Child("records").Child(count).Child("enemyid").SetValueAsync(enemyname);
                }
            });
    }


    public void GetEnemyName_Ready(string id)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").
            Child(id).Child("name").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    _SceneMng.OpenQuitPopup();
                else if (task.IsCompleted)
                {
                    DataSnapshot snap = task.Result;
                    _EnemyName = snap.Value.ToString();
                }
            });
    }
    //public void GetEnemyRecords(string id)
    //{
    //    FirebaseDatabase.DefaultInstance.GetReference("users").
    //            Child(id).Child("name").GetValueAsync().ContinueWith(task =>
    //            {
    //                if (task.IsFaulted)
    //                {
    //                    Debug.LogError("Error!!");
    //                }
    //                else if (task.IsCompleted)
    //                {
    //                    DataSnapshot snap = task.Result;
    //                    int i = 0;
    //                    _EnemyName
    //
    //                }
    //            });
    //}
    public void ResetEnemyRecord()
    {
        _EnemyName = "";
    }
    public string GetEnemyName()
    {
        return _EnemyName;   
    }

}

class RecordString
{
    public string result;
    public string enemyname;
}

class User
{
    public string name;
    public string email;
    public int win;
    public int lose;
    public int draw;

    public User(string name, string email)
    {
        this.name = name;
        this.email = email;
        win = 0;
        lose = 0;
        draw = 0;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["name"] = name;
        result["email"] = email;
        result["win"] = win;
        result["lose"] = lose;
        result["draw"] = draw;


        return result;
    }
}
