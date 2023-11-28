using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Character : MonoBehaviour
{
    public enum State { Idle, Run, Jump, InAir, Roll }
    public enum SIDE { Left = -2, Mid = 0, Right = 2 }
    public enum HitX { Left, Mid, Right, None };
    public enum HitY { Up, Mid, Down, Low, None };
    public enum HitZ { Forward, Mid, Backward, None };
    SIDE m_Side = SIDE.Mid;
    public float JetY_value = 10f;
    State m_state = State.Idle;
    float x, y;
    public float SpeedDodge;
    public float gravity = 10f;
    public float JumpPower = 7f, SuperJumpPower = 14f;
    bool InJump, InRoll, _isgrounded;
    public bool InAir = false;
    public bool IsGrounded
    {
        get { return _isgrounded; }
        set
        {
            if (_isgrounded == value) return; _isgrounded = value;
            if (OnGroundChange != null) OnGroundChange(_isgrounded);
        }
    }
    public event OnVariableChangeDelegate OnGroundChange;
    public delegate void OnVariableChangeDelegate(bool newVal);

    private bool _swipeUp, _swipeLeft, _swipeRight, _swipeDown;
    public bool swipeDown
    {
        get { return _swipeDown; }
        set
        {
            if (_swipeDown == value) return; _swipeDown = value;
            if (OnSwipeDown != null) OnSwipeDown(_swipeDown);
        }
    }
    public event OnVariableChangeDelegate OnSwipeDown;
    public bool swipeUp
    {
        get { return _swipeUp; }
        set
        {
            if (_swipeUp == value) return; _swipeUp = value;
            if (OnSwipeUp != null) OnSwipeUp(_swipeUp);
        }
    }
    public event OnVariableChangeDelegate OnSwipeUp;
    public bool swipeLeft
    {
        get { return _swipeLeft; }
        set
        {
            if (_swipeLeft == value) return; _swipeLeft = value;
            if (OnSwipeLeft != null) OnSwipeLeft(_swipeLeft);
        }
    }
    public event OnVariableChangeDelegate OnSwipeLeft;
    public bool swipeRight
    {
        get { return _swipeRight; }
        set
        {
            if (_swipeRight == value) return; _swipeRight = value;
            if (OnSwipeRight != null) OnSwipeRight(_swipeRight);
        }
    }
    public event OnVariableChangeDelegate OnSwipeRight;
    public bool HaveGuard;
    public float FwdSpeed = 9f, JetSpeedMultiple = 3f;
    HitX hitX = HitX.None;
    HitY hitY = HitY.None;
    HitZ hitZ = HitZ.None;
    private float ColHeight, ColCenterY, BColHeight, BColCenterY, _timer;
    private SIDE LastSide;
    private bool StopAllState = false;
    public bool CanInput = true;
    public FollowShip robot;
    public float curDistance = 0.6f;
    private float timer, lastJetmultiple;
    bool hitmovingtrain;
    public bool Dead;
    Animator m_Animator;
    public CapsuleCollider bcol;
    public CapsuleCollider _col;
    Rigidbody rigid;
    public AudioClip[] clips;
    AudioSource _source;
    public LayerMask GroundLayer;
    public static Character main;
    CameraFollow cam;
    SwipeControl _control;
    bool _haveMagnet, _haveJet = false, blockCollision = false, _haveBooster, _haveShield=false, IsStart = false;


    //public GameObject gameOverText;
    public Transform DNextPos;
    private float jumpTimer;
    private float jumpGracePeriod = 0.3f;



    void Awake()
    {
        main = this;
    }
    private void OnEnable()
    {
        main = this;
    }


    // Start is called before the first frame update
    private void Start()
    {
        
        lastJetmultiple = JetSpeedMultiple;
        _control = GetComponent<SwipeControl>();
        cam = FindObjectOfType<CameraFollow>();
        rigid = GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
        ColHeight = _col.height;
        BColHeight = bcol.height;
        BColCenterY = bcol.center.y;
        ColCenterY = _col.center.y;
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("GAS");
        //_spraycan.SetActive(true);
        OnGroundChange += GroundChangeDelegte;
        OnSwipeUp += JumpUp;
        OnSwipeDown += RollDown;
        OnSwipeLeft += SwipeLeftDelegate;
        OnSwipeRight += SwipeRightDelegate;
        //SoundManager.main.PlaySimpleClip(SoundManager.main.shakeCan);
    }
    public void StartGame()
    {
        if (IsStart)
            ResetMe();
        else
        {
            cam.TakingPos = true;
            m_Animator.SetBool("Start", true);
            robot.transform.position = Vector3.zero;
            transform.position = Vector3.zero;
            if (HaveGuard)
                curDistance = 0.6f;
            robot.StartRun();
            Invoke("ResetMe", 1.15f);
           //SoundManager.main.PlaySimpleClip(SoundManager.main._music);
        }
    }
    public void ResetMe()
    {
        m_Side = SIDE.Mid;
        transform.position = Vector3.zero;
        if (lastcollision)
            if (lastcollision.gameObject.tag == "Obstacles")
                lastcollision.isTrigger = true;
        ResetValues();
        StartCoroutine(Courtin());
        IEnumerator Courtin()
        {
            yield return new WaitForSeconds(0.2f);
            cam.PosTaken = true;
        }
        robot.ResetRobot();
        if (curDistance < 0.5)
            curDistance = 5f;
    }

    // Update is called once per frame
    public void ResetValues()
    {
        IsStart = true;
        m_Animator.SetLayerWeight(1, 0);
        ResetRollColl();
        m_Animator.Play("GAS");
        _col.isTrigger = false;
        hitmovingtrain = false;
        rigid.useGravity = true;
        rigid.isKinematic = false;
        m_state = State.Run;
        timer = 0.0f;
        y = 0f;
        cam.IsDead = false;
        CanInput = true;
        Dead = false;
        StopAllState = false;
        InRoll = false;
        InJump = false;
        hitX = HitX.None;
        hitY = HitY.None;
        hitZ = HitZ.None;
        blockCollision = false;
        // _spraycan.SetActive(false);
    }
    void Update()
    {
        print(IsGrounded);
        InputHandle();
        FwdSpeed = GameController.GameSpeed;
        robot.curDis = curDistance;
        if (!InAir)
        {
            if (transform.position.y < -2f)
            {
                Vector3 v = transform.position;
                v.y = 0.0f;
                transform.position = v;
                
            }
        }
        
        if (Dead)
        {
            if (curDistance < 1.25f && HaveGuard)
            {
                curDistance = Mathf.MoveTowards(curDistance, 0.0f, Time.deltaTime * 5f);

                if (hitmovingtrain)
                {
                    robot.HitMovingObject();
                    hitmovingtrain = false;
                    return;
                }
                robot.CoughtPlayer();
                return;
            }
            else
            {
                curDistance = 5f;
            }
            

        }
        if (!CanInput) return;
        CheckGround();

        if (timer > -0.1f)
            timer = Mathf.MoveTowards(timer, 0.0f, Time.deltaTime);
        if (timer <= 0.0f)
        {
            StopAllState = false;
            m_Animator.SetLayerWeight(1, 0);
        }

    }
    private void FixedUpdate()
    {
        if (CanInput && !Dead)
            curDistance = Mathf.MoveTowards(curDistance, 50f, Time.deltaTime * (curDistance < 1.25f ? 0.1f : 0.2f));
        if (curDistance > 0f)
            robot.Follow(transform.position, FwdSpeed);
        else
        {
            robot.DirectPos(transform.position);
        }
        if (!CanInput)
            return;
        x = Mathf.Lerp(x, (int)m_Side, Time.deltaTime * SpeedDodge);
        Vector3 moveVector = new Vector3(x - transform.position.x, 0, FwdSpeed * (_haveJet ? JetSpeedMultiple : 1) * Time.fixedDeltaTime);
        if (_haveJet)
        {
            y = Mathf.Lerp(y, JetY_value, Time.deltaTime);
            moveVector.y = y - transform.position.y;
        }
        rigid.MovePosition(transform.position + moveVector);
        if (!IsGrounded && !_haveJet)
        {

            rigid.AddForce(Vector3.down * gravity);
            PlayAnimation("FALL");
        }
            
    }
    public void DeathPlayer(string anim)
    {
        if (_haveShield)
        {
            robot.ResetRobot();
            curDistance = 5f;
            //expol.Play();
            BlastNeighbourItems();
            hitmovingtrain = false;
            ResetValues();
            blockCollision = true;
            Invoke("ActiveCollision", 1f);
            SoundManager.main.PlaySimpleClip(SoundManager.main.crash);
            return;
        }
        CanInput = false;
        Dead = true;
        StopAllState = true;
        cam.IsDead = true;
        //curDistance = 0f;
        m_Animator.SetLayerWeight(1, 0);
        m_Animator.Play(anim);
        _source.PlayOneShot(clips[5]);
        cam.ShakeCam();
       // gameOverText.SetActive(true);
        print(hitX.ToString() + hitY.ToString() + hitZ.ToString());
        GameController.manager.EndGame();

    }
    public void SaveMe()
    {
        robot.ResetRobot();
        curDistance = 5f;
        //expol.Play();
        BlastNeighbourItems();
        hitmovingtrain = false;
        SoundManager.main.PlaySimpleClip(SoundManager.main.SaveDeath);
        ResetValues();
        blockCollision = true;
        Invoke("ActiveCollision", 1f);
        GameController.manager.StartGame();
    }
    public void ActiveCollision()
    {
        hitmovingtrain = false;
        blockCollision = false;
    }
    public void PlayAnimation(string anim)
    {
        if (StopAllState || Dead) return;
        m_Animator.Play(anim);
    }
    public void Stumble(string anim, int layer)
    {
        StopAllState = true;
        _timer = 0.0f;
        m_Animator.Play(anim);
        StopAllCoroutines();
        StartCoroutine(ResetCol(0.2f));

        timer = m_Animator.GetCurrentAnimatorStateInfo(layer).length;
        if (curDistance < 0.8f)
        {

            DeathPlayer("DeathFront");
            Dead = true;
            return;
        }
        robot.Stumble();
        cam.ShakeCam();
        if (HaveGuard)
            curDistance = 0.6f;
        ResetCollision();
        _source.PlayOneShot(clips[4]);
    }
    public void InputHandle()
    {
        if (!CanInput)
        {
            swipeLeft = swipeRight = swipeDown = swipeUp = false;
            return;
        }
        swipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || _control.swipeLeft;
        swipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || _control.swipeRight;
        swipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || _control.swipeUp;
        swipeDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || _control.swipeDown;
    }
    private void JumpUp(bool v)
    {
        jumpTimer = Time.time;
        //if (!_haveJet && (m_state == State.Run || m_state == State.Roll))
        //{
        if (IsGrounded)
        {
            if (v && (!InJump || (jumpTimer > 0 && Time.time < jumpTimer + jumpGracePeriod)))
            {
                m_state = State.Jump;
                InJump = true;
                PlayAnimation("JUMP");
                print("jumps");
                StopAllState = true;
                timer = m_Animator.GetCurrentAnimatorStateInfo(0).length;
                robot.Jump();
                _source.PlayOneShot(clips[_haveBooster ? 6 : 0]);
                Vector3 vel = rigid.velocity;
                vel.y = JumpPower;//_haveBooster ? SuperJumpPower : JumpPower;
                rigid.velocity = vel;
                //  m_state = State.InAir;
            }
        }
        //}
    }

    public void ResetRollColl()
    {
        _col.center = new Vector3(0, ColCenterY, 0);
        _col.height = ColHeight;
        bcol.center = new Vector3(bcol.center.x, BColCenterY, bcol.center.z);
        bcol.height = BColHeight;
    }
    private void RollDown(bool v)
    {
        if (!_haveJet && m_state != State.Roll && v && !InRoll)
        {
            InJump = false;
            InRoll = true;
            _col.center = new Vector3(0, ColCenterY / 2f, 0);
            bcol.center = new Vector3(bcol.center.x, BColCenterY / 2f, bcol.center.z);
            _col.height = ColHeight / 2f;
            bcol.height = BColHeight / 2f;
            m_Animator.CrossFadeInFixedTime("Roll", 0.01f);
            m_state = State.Roll;
            _source.PlayOneShot(clips[1]);
            StartCoroutine(RollForce());
        }
    }
    private IEnumerator RollForce()
    {
        float _time = 0.75f;
        while (InRoll)
        {
            _time -= Time.deltaTime;
            rigid.AddForce(Vector3.down * gravity * 5f);
            if (_time <= 0.0f || InJump)
                InRoll = false;
            yield return null;
        }
        m_state = State.Run;
        ResetRollColl();
        StopAllState = false;
    }

    private void GroundChangeDelegte(bool v)
    {
        if (v)
        {
            InJump = false;
            m_state = State.Run;
            if (!InRoll)
            {
                _source.PlayOneShot(clips[3]);
                PlayAnimation("LAND");
                print("land");
            }
            // m_Animator.SetBool("jump", false);
            StopAllState = false;
        }
        else if (!InJump && !StopAllState)
        {
            m_Animator.Play("FALL");
            print("falling");
        }
        m_Animator.SetBool("IsGrounded", IsGrounded);
    }
    public void SwipeLeftDelegate(bool swipe)
    {
        if (swipe && m_state != State.Idle)
        {
            if (m_Side == SIDE.Mid)
            {
                m_Side = SIDE.Left;
                if (!IsGrounded || InRoll) return;
                PlayAnimation("LEFT");
                robot.LeftDodge();
                _source.PlayOneShot(clips[2]);
            }
            else if (m_Side == SIDE.Right)
            {
                LastSide = m_Side;
                m_Side = SIDE.Mid;
                if (!IsGrounded || InRoll) return;
                robot.LeftDodge();
                PlayAnimation("LEFT");
                _source.PlayOneShot(clips[2]);
            }
            else if (m_Side != LastSide && !_haveJet)
            {
                LastSide = m_Side;
                Stumble("stumbleLEFT", 0);
            }
        }
    }
    public void SwipeRightDelegate(bool swipe)
    {
        if (swipe && m_state != State.Idle)
        {
            if (m_Side == SIDE.Mid)
            {
                LastSide = m_Side;
                m_Side = SIDE.Right;
                if (!IsGrounded || InRoll) return;
                robot.RightDodge();
                PlayAnimation("RIGHT");
                _source.PlayOneShot(clips[2]);
            }
            else if (m_Side == SIDE.Left)
            {
                LastSide = m_Side;
                m_Side = SIDE.Mid;
                if (!IsGrounded || InRoll) return;
                robot.RightDodge();
                PlayAnimation("RIGHT");
                _source.PlayOneShot(clips[2]);
            }
            else if (m_Side != LastSide && !_haveJet)
            {
                LastSide = m_Side;
                Stumble("stumbleRIGHT", 0);
            }
        }
    }


    private void CheckGround()
    {
        RaycastHit hit;

        IsGrounded = (Physics.SphereCast(transform.position + transform.up * 0.75f,
            0.1f, -transform.up, out hit, (InJump ? 0.75f : 1.1f), GroundLayer, QueryTriggerInteraction.Ignore));

    }
    #region Collision
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == ("Diamond"))
        {
            //print("yea");
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.GetComponent<CoinScript>().ScoreMode = true;
            SoundManager.main.PlaySimpleClip(SoundManager.main.CoinEat);
            //other.gameObject.SetActive(false);

            GameController.manager.IncrementScore();
            if (!other.GetComponent<RectTransform>())
            {
                other.gameObject.AddComponent<RectTransform>();
                other.GetComponent<RectTransform>().anchorMin = Vector2.one;
                other.GetComponent<RectTransform>().anchorMax = Vector2.one;
            }
            other.transform.parent = DNextPos.parent;
            //GameController.manager.IncrementScore();
            return;
        }
         if (other.tag == "Player" || other.tag == "Floor") //|| other.tag == "Diamond") || !CanInput || other.isTrigger || blockCollision)
           return;
           
        
         
        OnCollision(other);
    }

    Collider lastcollision;
    public void OnCollision(Collider col)
    {
        
        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);
            lastcollision = col;

        if (hitZ == HitZ.Forward && hitX == HitX.Mid) ///Death
        {
            
            if (hitY == HitY.Low && col.tag == "Obstacles")
            {
                col.isTrigger = true;
                Stumble("CrushDown", 0);
            }
            else if (hitY == HitY.Low && col.tag != "Ramp")
            {
                Stumble("CrushDown", 0);
            }
            else if (hitY == HitY.Down && col.tag == "Bush")
            {
                Stumble("CrushDown", 0);
            }
            else if (hitY == HitY.Down && col.tag != "Ramp")
            {
                DeathPlayer("DeathFront");
                //curDistance = 0.6f;
            }
            else if (hitY == HitY.Mid && col.tag != "Ramp")
            {
                if (col.tag == "MovingTrain")
                {
                    _col.isTrigger = true;
                    rigid.isKinematic = true;
                    hitmovingtrain = true;
                    DeathPlayer("DeathFront");
                }
                else if (col.tag != "Ramp")
                {
                    DeathPlayer("DeathFront");
                }
            }
            else if (hitY == HitY.Up)
            {
                DeathPlayer("DeathFront");
            }
            else
            if (hitY == HitY.Low && !IsGrounded)
            {
                DeathPlayer("DeathFront");
            }
        }
        else if (hitZ == HitZ.Mid)
        {
            if (hitY == HitY.Low && !InRoll && col.tag != "Ramp")
            {
                Stumble("CrushDown", 0);
            }
            else if (hitX == HitX.Right)
            {
                m_Side = LastSide;
                Stumble("stumbleRIGHT", 0);

            }
            else if (hitX == HitX.Left)
            {
                m_Side = LastSide;
                Stumble("stumbleLEFT", 0);

            }
        }
        else
        {
            if (hitX == HitX.Right)
            {
                m_Animator.SetLayerWeight(1, 1);
                Stumble("stumbleRIGHT", 1);

            }
            else if (hitX == HitX.Left)
            {
                m_Animator.SetLayerWeight(1, 1);
                Stumble("stumbleLEFT", 1);

            }
        }

    }


    private void ResetCollision()
    {
        print(hitX.ToString() + hitY.ToString() + hitZ.ToString());
        hitX = HitX.None;
        hitY = HitY.None;
        hitZ = HitZ.None;
    }

    public HitX GetHitX(Collider col)
    {
        Bounds char_bounds = bcol.bounds;
        Bounds col_bounds = col.bounds;
        float min_x = Mathf.Max(col_bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Min(col_bounds.max.x, char_bounds.max.x);
        float average = (min_x + max_x) / 2f - col_bounds.min.x;
        HitX hit;

        if (average > col_bounds.size.x - 0.33f)
            hit = HitX.Right;
        else if (average < 0.33f)
            hit = HitX.Left;
        else
            hit = HitX.Mid;
        return hit;
    }
    public HitY GetHitY(Collider col)
    {
        Bounds char_bounds = bcol.bounds;
        Bounds col_bounds = col.bounds;
        float min_y = Mathf.Max(col_bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Min(col_bounds.max.y, char_bounds.max.y);
        float average = ((min_y + max_y) / 2f - char_bounds.min.y) / char_bounds.size.y;
        HitY hit;
        if (average < 0.2f)
            hit = HitY.Low;
        else if (average < 0.33f)
            hit = HitY.Down;
        else if (average < 0.66f)
            hit = HitY.Mid;
        else
            hit = HitY.Up;
        return hit;
    }
    public HitZ GetHitZ(Collider col)
    {
        Bounds char_bounds = bcol.bounds;
        Bounds col_bounds = col.bounds;
        float min_z = Mathf.Max(col_bounds.min.z, char_bounds.min.z);
        float max_z = Mathf.Min(col_bounds.max.z, char_bounds.max.z);
        float average = ((min_z + max_z) / 2f - char_bounds.min.z) / char_bounds.size.z;
        HitZ hit;
        if (average < 0.25f)
            hit = HitZ.Backward;
        else if (average < 0.66f)
            hit = HitZ.Mid;
        else
            hit = HitZ.Forward;
        return hit;
    }
    public IEnumerator ResetCol(float v)
    {
        bcol.isTrigger = false;
        yield return new WaitForSeconds(v);
        bcol.isTrigger = true;
    }
    #endregion
    public float blast = 12f;
    public void BlastNeighbourItems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blast);
        for (var i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Cube" || hitColliders[i].tag == "Ramp" || hitColliders[i].tag == "Bush"
                || hitColliders[i].tag == "MovingObject" || hitColliders[i].tag == "Obstacles")
            {
                Destroy(hitColliders[i].gameObject);
                if (hitColliders[i].tag == "Ramp")
                    Destroy(hitColliders[i].transform.parent.gameObject);

            }
        }
    }
}
