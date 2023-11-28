using System.Collections;

using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private float radius;

    public float rotateSpeed;
    private bool coroutineAllowed;
    public bool ScoreMode;
    // Start is called before the first frame update
    void Start()
    {
        coroutineAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        FindThePlayer();

    }
    private void FixedUpdate()
    {
        transform.Rotate(0, rotateSpeed, 0);
        if(coroutineAllowed)
        {
            StartCoroutine("StartPulsing");
        }

    }
    private IEnumerator StartPulsing()
    {
        coroutineAllowed = false;
        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            transform.localScale = new Vector3(
                (Mathf.Lerp(transform.localScale.x, transform.localScale.x + 0.025f, Mathf.SmoothStep(0f, 1f, i))),
                (Mathf.Lerp(transform.localScale.y, transform.localScale.y + 0.025f, Mathf.SmoothStep(0f, 1f, i))),
                (Mathf.Lerp(transform.localScale.z, transform.localScale.z + 0.025f, Mathf.SmoothStep(0f, 1f, i)))
               );
            yield return new WaitForSeconds(0.03f);
        }
        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            transform.localScale = new Vector3(
                (Mathf.Lerp(transform.localScale.x, transform.localScale.x - 0.025f, Mathf.SmoothStep(0f, 1f, i))),
                (Mathf.Lerp(transform.localScale.y, transform.localScale.y - 0.025f, Mathf.SmoothStep(0f, 1f, i))),
                (Mathf.Lerp(transform.localScale.z, transform.localScale.z - 0.025f, Mathf.SmoothStep(0f, 1f, i)))
               );
            yield return new WaitForSeconds(0.03f);
        }
        coroutineAllowed = true;
    }

    private void FindThePlayer()
    {
        if (!ScoreMode)
        {
            Collider[] CoinColl = Physics.OverlapSphere(transform.position, radius);
            foreach (var c in CoinColl)
            {
                if (c.CompareTag("Player"))
                {
                    transform.position = Vector3.MoveTowards(transform.position, c.transform.position + new Vector3(0f, 1f, 0f), Time.deltaTime * 50f);
                    //SoundManager.main.PlaySimpleClip(SoundManager.main.CoinEat);
                   // GameController.manager.IncrementScore();
                    //gameObject.SetActive(false);
                }
            }
        }
        else
        {
            
            transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(12f, 10f, 5f), Time.deltaTime * 75f);

            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 4f);
           // gameObject.SetActive(false);
        }

    }
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            //GameController.manager.IncrementScore();
            return;
        }
    }*/
        private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
