using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowShip : MonoBehaviour
{

    public Animator RobotAnimator;
    public Transform Robot;
    public float curDis;
    bool Dead;
    AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void Jump()
    {
        StartCoroutine(PlayAnim("Jump"));
    }
    public void Fall()
    {
        StartCoroutine(PlayAnim("Fall"));
    }
    public void LeftDodge()
    {
        StartCoroutine(PlayAnim("Left"));
    }
    public void RightDodge()
    {
        StartCoroutine(PlayAnim("Right"));
    }
    public void ResetRobot()
    {
        RobotAnimator.Play("Chase");
        StopAllCoroutines();
        Dead = false;
    }
    public void StartRun()
    {
        RobotAnimator.Play("Idle");
        StopAllCoroutines();
        Dead = false;
    }
    public void PlayStartShout()
    {
        source.clip = (SoundManager.main.GuardStartRun);
        source.Play();
        Invoke("StopShout", 5f);
    }
    public void CoughtPlayer()
    {
        if (Dead) return;
        RobotAnimator.Play("Catch");
        source.clip = (SoundManager.main.catchPlayer);
        source.Play();
        StopAllCoroutines();
        Dead = true;
    }
    public void Stumble()
    {
        StopAllCoroutines();
        source.clip = (SoundManager.main.stumbleGuard);
        source.Play();
        StartCoroutine(PlayAnim("Stumble"));
    }
    public void HitMovingObject()
    {
        StartCoroutine(PlayAnim("Death"));
    }
    private void StopShout()
    {
        source.Stop();
    }
    private IEnumerator PlayAnim(string anim)
    {
        yield return new WaitForSeconds(curDis / 10f);
        if (!Dead)
        {
            RobotAnimator.Play(anim);
        }
    }

    public void Follow(Vector3 pos, float speed)
    {
        Vector3 position = pos - Vector3.forward * curDis;
        if (curDis != 0.0f)
            Robot.position = Vector3.Lerp(Robot.position, position, Time.deltaTime * speed / curDis);
    }
    public void DirectPos(Vector3 pos)
    {
        Robot.position = pos;

    }
}
