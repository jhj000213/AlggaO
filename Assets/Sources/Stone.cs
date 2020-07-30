using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    int _Pos_X;
    int _Pos_Y;
    bool _isOmGame;
    bool _isUsing = false;
    bool _isStop = true;
    bool _isBlack = true;

    bool _Lerping;
    public Vector2 _LerpTargetPos;
    public float _LerpSpeed;

    [SerializeField]
    Sprite _BlackStoneImage;
    [SerializeField]
    Sprite _WhiteStoneImage;

    SpriteRenderer _StoneImage;
    Rigidbody2D _MyRigidbody;

    [SerializeField]
    GameObject _HitParticle;
    [SerializeField]
    GameObject _DestroyEffect_Black;
    [SerializeField]
    GameObject _DestroyEffect_White;

    [SerializeField]
    GameObject _OmocBackLightEffect_Black;
    [SerializeField]
    GameObject _OmocBackLightEffect_White;

    float _SpinningDis;
    float _NowAngle;
    float _TargetAngle;
    bool _Spinning;
    float _SpinningPosX;
    float _SpinningPosY;
    public Vector3 _SpinningTargetPos;

    Vector2 _BoardZeroPos = new Vector2(50, 800);
    Vector3 _BoardCenterPos = new Vector3(540,1290,0);

    bool _SoundOn;

    [SerializeField]
    GameObject _StoneDestroyClip;
    [SerializeField]
    GameObject _StoneHitClip;

    public void Init(bool soundon)
    {
        _SoundOn = soundon;
        _LerpSpeed = 4;
        _Pos_X = -5;
        _Pos_Y = -5;
        _isUsing = false;
        _isStop = true;
        _Lerping = false;
        _StoneImage = GetComponent<SpriteRenderer>();
        _MyRigidbody = GetComponent<Rigidbody2D>();
        transform.localPosition = new Vector3();
        gameObject.SetActive(false);
        _OmocBackLightEffect_Black.SetActive(false);
        _OmocBackLightEffect_White.SetActive(false);
    }

    public void Clean(bool soundon)
    {
        _SoundOn = soundon;
        _Pos_X = -5;
        _Pos_Y = -5;
        _isUsing = false;
        _isStop = true;
        _Lerping = false;
        transform.localPosition = new Vector3();
        //transform.localEulerAngles = new Vector3(0, 0, 0);
        _OmocBackLightEffect_Black.SetActive(false);
        _OmocBackLightEffect_White.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SettingForAl(bool isblack,float x,float y)
    {
        _isBlack = isblack;
        gameObject.SetActive(true);
        _isUsing = true;
        transform.localPosition = new Vector2(x, y);
        if (_isBlack)
            _StoneImage.sprite = _BlackStoneImage;
        else
            _StoneImage.sprite = _WhiteStoneImage;
    }
    public void SettingForOm(bool isblack, int x, int y)
    {
        _isBlack = isblack;
        gameObject.SetActive(true);
        _isUsing = true;
        _Pos_X = x;
        _Pos_Y = y;
        SetPositioningAnimation(new Vector2(_Pos_X * 70.0f, _Pos_Y * 70.0f) + _BoardZeroPos,4);
        //transform.localPosition = new Vector2(_Pos_X * 70.0f, _Pos_Y * 70.0f) + _BoardZeroPos;
        //transform.localEulerAngles = new Vector3(0, 0, 0);
        if (_isBlack)
            _StoneImage.sprite = _BlackStoneImage;
        else
            _StoneImage.sprite = _WhiteStoneImage;
    }

    public void SetPositioningAnimation(Vector2 pos,float speed)
    {
        _LerpTargetPos = pos;
        _Lerping = true;
        _LerpSpeed = speed;
    }
    public void SetSpinningAnimationAlGame(bool clockvector,float dis,float startangle,Vector3 targetpos)
    {
        float targetangle = 90;
        if (clockvector)
            targetangle *= -1;

        _NowAngle = startangle;
        _SpinningDis = dis;
        _TargetAngle = startangle + targetangle;
        _SpinningTargetPos = targetpos;
        _Spinning = true;
    }

    private void Update()
    {
        if(_Lerping)
        {
            
            transform.localPosition = Vector2.Lerp(transform.localPosition,_LerpTargetPos,Time.smoothDeltaTime*_LerpSpeed);
            if(Vector2.Distance(transform.localPosition,_LerpTargetPos)<=3.0f)
            {
                _Lerping = false;
                transform.localPosition = _LerpTargetPos;
            }
        }
        if(_Spinning)
        {
            _NowAngle = Mathf.Lerp(_NowAngle, _TargetAngle, Time.smoothDeltaTime * 6.5f);
            _SpinningPosX = _SpinningDis * Mathf.Cos(_NowAngle * Mathf.Deg2Rad);
            _SpinningPosY = _SpinningDis * Mathf.Sin(_NowAngle * Mathf.Deg2Rad);
            transform.localPosition = new Vector3(_SpinningPosX, _SpinningPosY, 0) + _BoardCenterPos;
            if (Vector3.Distance(transform.localPosition, _SpinningTargetPos)<3.0f)
            {
                _Spinning = false;
                transform.localPosition = _SpinningTargetPos;
            }

        }
    }

    public void InitOm(bool isblack,int x,int y)
    {
        _isOmGame = true;
        _isUsing = true;
        _isBlack = isblack;
        _Pos_X = x;
        _Pos_Y = y;
        if (_isBlack)
            _StoneImage.sprite = _BlackStoneImage;
        else
            _StoneImage.sprite = _WhiteStoneImage;
        gameObject.SetActive(true);
        transform.localPosition = new Vector2(_Pos_X * 70.0f, _Pos_Y * 70.0f) + _BoardZeroPos;
    }
    public void InitAl(bool isblack, Vector2 pos)
    {
        _isOmGame = false;
        _isBlack = isblack;
        if (_isBlack)
            _StoneImage.sprite = _BlackStoneImage;
        else
            _StoneImage.sprite = _WhiteStoneImage;
        gameObject.transform.localPosition = pos;
    }

    public void ShootAl(Vector2 shootvector)
    {
        gameObject.SetActive(true);
        _isUsing = true;
        _isStop = false;
        _MyRigidbody.velocity = shootvector;
    }

    public bool OutOfBoard()
    {
        bool check = false;

        if (transform.localPosition.x < _BoardZeroPos.x - 35.0f || transform.localPosition.x > 1065.0f ||
            transform.localPosition.y < _BoardZeroPos.y - 35.0f || transform.localPosition.y > _BoardZeroPos.y + 980.0f + 35.0f)
            check = true;

        return check;
    }

    public void DestroyStone()
    {
        _OmocBackLightEffect_Black.SetActive(false);
        _OmocBackLightEffect_White.SetActive(false);
        _Pos_X = -5;
        _Pos_Y = -5;
        _isOmGame = false;
        _isUsing = false;
        transform.localPosition = new Vector3();
        gameObject.SetActive(false);
    }

    public void DestroyAndEffect()
    {
        GameObject obj = new GameObject();
        GameObject sound = Instantiate(_StoneDestroyClip);
        sound.transform.parent = transform.parent;
        if (IsBlack())
        {
            obj = Instantiate(_DestroyEffect_Black);
            obj.transform.parent = transform.parent;
            obj.transform.localPosition = transform.localPosition += new Vector3(7.4f, 53, 0);
        }
        else
        {
            obj = Instantiate(_DestroyEffect_White);
            obj.transform.parent = transform.parent;
            obj.transform.localPosition = transform.localPosition += new Vector3(7, 53.2f, 0);
        }
        
        DestroyStone();
    }

    public void SetRePosition(int x,int y)
    {
        _Pos_X = x;
        _Pos_Y = y;
        SetPositioningAnimation(new Vector2(_Pos_X * 70.0f, _Pos_Y * 70.0f) + _BoardZeroPos,4);
        //transform.localEulerAngles = new Vector3(0,0,0);
    }

    private void FixedUpdate()
    {
        _MyRigidbody.velocity *= 0.97f;
        _MyRigidbody.angularVelocity *= 0.97f;
        if (_MyRigidbody.velocity.magnitude < 1.0f)
        {
            _MyRigidbody.velocity = new Vector2(0, 0);
            _isStop = true;
        }
        else
            _isStop = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_isStop)
        {
            float speed = Mathf.Sqrt((Mathf.Pow(_MyRigidbody.velocity.x, 2) + Mathf.Pow(_MyRigidbody.velocity.y, 2)))/300.0f;
            speed = speed >= 1.0f ? 1.0f : speed;

            GameObject obj = Instantiate(_HitParticle);
            obj.transform.parent = transform.parent;
            obj.transform.localPosition = (transform.localPosition + collision.transform.localPosition) / 2;
            obj.transform.localScale *= speed;
            
            if(_SoundOn)
            {
                GameObject sound = Instantiate(_StoneHitClip);
                sound.GetComponent<AudioSource>().volume = speed;
                sound.transform.parent = transform.parent;
            }
        }
    }

    public void OnOmocWinnerEffect()
    {
        if (_isBlack)
        {
            _OmocBackLightEffect_Black.SetActive(true);
            _OmocBackLightEffect_Black.GetComponent<J_SpriteAnimation>()._FrameNumber = 0;
        }
        else
        {
            _OmocBackLightEffect_White.SetActive(true);
            _OmocBackLightEffect_White.GetComponent<J_SpriteAnimation>()._FrameNumber = 0;
        }
    }

    public void SetRigidbodyOn()
    {
        _MyRigidbody.isKinematic = false;
    }
    public void SetRigidbodyOff()
    {
        _MyRigidbody.isKinematic = true;
    }

    public int GetPos_X() { return _Pos_X; }
    public int GetPos_Y() { return _Pos_Y; }
    public bool IsUsing() { return _isUsing; }
    public bool IsStop() { return _isStop; }
    public bool IsBlack() { return _isBlack; }
}
