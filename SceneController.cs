using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public float Speed=8f;
    [Range(-1f,1f)]public float bendX = 0.1f;
    [Range(-1f, 1f)]public float bendY = 0.1f;
    public Material[] materials;
    public Character charSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        charSpeed = FindObjectOfType<Character>();
        Debug.Log(charSpeed.Dead);
        foreach(var m in materials)
        {
            m.SetFloat(Shader.PropertyToID("X_Axis"), bendX);
            m.SetFloat(Shader.PropertyToID("Y_Axis"), bendY);

        }
        transform.Translate(Vector3.back * Speed * Time.deltaTime);
        if(charSpeed)
        {
            Speed = 0f;
        }    
        
    }
}
