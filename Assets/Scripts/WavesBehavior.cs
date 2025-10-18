using UnityEngine;

public class WavesBehavior : MonoBehaviour
{

    //animation
    Animator anim;

    float timer = 0f;

    [SerializeField]
    float interval = 5f;

    //spawning bubbles
    [SerializeField] 
    GameObject bubblePrefab;  

    [SerializeField] 
    Transform spawnPoint; 

    [SerializeField] 
    Vector3 spawnJitter = new Vector3(0.25f, 0.15f);

    [SerializeField] 
    Vector3 spawnXRange = new Vector3(-8f, 8f);
    
    [SerializeField] 
    Vector3 spawnYRange = new Vector3(-4f, 4f);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            anim.SetTrigger("run");
            timer = 0f; 
        }
    }

    public void OnWaveRunComplete()
    {
        if (bubblePrefab == null)
            return;

        float randX = Random.Range(spawnXRange.x, spawnXRange.y);
        float randY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 randomPos = new Vector3(randX, randY, 0f);

        GameObject bubble = Instantiate(bubblePrefab, randomPos, Quaternion.identity);

        bubble.tag = "food";
    }
}
