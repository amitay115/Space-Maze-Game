using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CameraStatus
{
    Idle, Normal, OnJet, Dead
}
public class CameraFollow : MonoBehaviour
{
    public CameraStatus _status = CameraStatus.Idle;
    public Transform Player;
    public Transform TopPoint;
    public Transform StartPoint;
    public Transform Head;
    public bool IsDead, TakingPos, PosTaken;
    public float speed;
    public Vector3 Rotation;
    public float NormalZ;
    public float JetZ;
    public float NormalY;
    public float TrainY;
    public float Speedfollow = 5f;
    private float y, z;
    public float ymin, ymax;
    private void Awake()
    {
    }
    public void Start()
    {
        transform.position = TopPoint.position;
        transform.rotation = TopPoint.rotation;
        y = NormalY;
    }
    void LateUpdate()
    {
        switch (_status)
        {
            case CameraStatus.Idle:
                if (!TakingPos)
                {
                    transform.position = Vector3.Lerp(transform.position, StartPoint.position, Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, StartPoint.rotation, Time.deltaTime);
                }
                if (TakingPos)
                {
                    transform.LookAt(Head.position);
                    transform.position = Vector3.Lerp(transform.position, new Vector3(0, NormalZ, transform.position.z), Time.deltaTime);
                    if (PosTaken)
                    {
                        TakingPos = false;
                        _status++;
                        y = transform.position.y;
                        z = Player.position.z - transform.position.z;
                    }
                }
                break;
            case CameraStatus.Normal:
                z = Mathf.MoveTowards(z, NormalZ, Time.deltaTime * Speedfollow);
                y = Mathf.Lerp(y, YPos(), Time.deltaTime * Speedfollow);
                transform.position = new Vector3(Player.position.x, y, Player.position.z - z);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Rotation), Time.deltaTime * 3f);
                if (IsDead)
                    _status = CameraStatus.Dead;
                break;
            case CameraStatus.Dead:
                if (!IsDead)
                    _status = CameraStatus.Normal;
                break;
        }
    }
    public float YPos()
    {
        if (RampCheck() || (Player.position.y > ymin && Player.position.y < ymax))
            return TrainY;
        else if (Player.position.y < ymin)
            return NormalY;
        else
            return Player.position.y + NormalY;
    }
    public bool RampCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(Player.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f))
            if (hit.collider.tag == "Ramp")
                return true;
        return false;
    }
    public void ShakeCam()
    {
    }
    public void DeactivateJet()
    {
        Invoke("DelayLanding", 0.8f);
    }
    void DelayLanding()
    {
        if (_status == CameraStatus.OnJet)
            _status = CameraStatus.Normal;
    }
}
