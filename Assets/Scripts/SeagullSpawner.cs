using UnityEngine;

public class SeagullSpawner : MonoBehaviour
{

    [SerializeField] 
    GameObject seagullPrefab;   

    [SerializeField] 
    Transform[] possibleTargets; 


    [SerializeField] 
    float spawnInterval = 5f;    

    [SerializeField] 
    float offscreenMargin = 1.5f; 

    [SerializeField] 
    bool matchTargetY = true;    


    float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnOffscreenSeagull();
            timer = spawnInterval;
        }
    }

    void SpawnOffscreenSeagull()
    {
        if (seagullPrefab == null) 
        {
            return;
        }

        Camera cam = Camera.main;

        if (cam == null || !cam.orthographic)
        {
            InstantiateAndInit(seagullPrefab, transform.position, true);
            return;
        }

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        int side = Random.Range(0, 2);

        float y;

        if (matchTargetY && possibleTargets != null && possibleTargets.Length > 0)
        {
            var t = possibleTargets[Random.Range(0, possibleTargets.Length)];
            y = t.position.y;
        }
        else
        {
            y = Random.Range(cam.transform.position.y - halfH, cam.transform.position.y + halfH);
        }
        float x = (side == 0)
            ? cam.transform.position.x - halfW - offscreenMargin  
            : cam.transform.position.x + halfW + offscreenMargin; 

        Vector3 spawnPos = new Vector3(x, y, 0f);
        InstantiateAndInit(seagullPrefab, spawnPos, side == 1); 
    }

    void InstantiateAndInit(GameObject prefab, Vector3 position, bool comingFromRight)
    {
        var go = Instantiate(prefab, position, Quaternion.identity);

        var gull = go.GetComponent<SeagullBehavior>();
        if (gull != null && possibleTargets != null && possibleTargets.Length > 0)
        {
            gull.SetPossibleTargets(possibleTargets);  
        }
    }
}