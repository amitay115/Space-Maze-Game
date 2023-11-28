using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    private Vector3 offset;
  //  private Vector3 dis = new Vector3(0,0,0.5f);

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y , target.position.z) + offset, Time.deltaTime * 6);
    }
}
