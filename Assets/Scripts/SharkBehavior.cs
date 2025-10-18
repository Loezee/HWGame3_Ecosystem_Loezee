using UnityEngine;

public class SharkBehavior : MonoBehaviour
{
    [SerializeField] 
    float radius = 1.6f;

    [SerializeField] 
    float angularSpeedDeg = 70f;

    [SerializeField] 
    string seagullTag = "Seagull";

    Vector3 center;
    float angle;

    void OnEnable()
    {
        center = transform.position;
        angle = Random.Range(0f, 360f);

        var col = GetComponent<Collider2D>();
        
        if (col) 
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        angle += angularSpeedDeg * Time.deltaTime;
        float r = angle * Mathf.Deg2Rad;
        transform.position = center + new Vector3(Mathf.Cos(r), Mathf.Sin(r), 0f) * radius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag(seagullTag))
        {
            var gull = other.GetComponent<SeagullBehavior>();
            if (gull != null) gull.Die();
        }
    }
}
