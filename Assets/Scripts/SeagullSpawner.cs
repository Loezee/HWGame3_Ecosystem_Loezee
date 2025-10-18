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
    [SerializeField] 
    bool spawnOnStart = true;
    [SerializeField] 
    int maxAlive = 0;


    [SerializeField] 
    bool autoFindTargetsByTag = false;
    [SerializeField] 
    string targetTag = "TestTarget";

    float timer;

    void Start()
    {
        if (autoFindTargetsByTag && (possibleTargets == null || possibleTargets.Length == 0))
        {
            var gos = GameObject.FindGameObjectsWithTag(targetTag);
            possibleTargets = new Transform[gos.Length];
            for (int i = 0; i < gos.Length; i++) 
            {
                possibleTargets[i] = gos[i].transform;
            }
        }

        if (spawnOnStart)
        {
            timer = 0f;   
        }
        else
        {
            timer = spawnInterval; 
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        SpawnOffscreenSeagull();
        timer = spawnInterval;
    }

    void SpawnOffscreenSeagull()
    {
        if (seagullPrefab == null) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            InstantiateAndInit(transform.position);
            return;
        }

        if (!cam.orthographic)
        {
            InstantiateAndInit(transform.position);
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

        float x;

        if (side == 0)
        {
            x = cam.transform.position.x - halfW - offscreenMargin;
        }
        else
        {
            x = cam.transform.position.x + halfW + offscreenMargin;
        }

        Vector3 spawnPos = new Vector3(x, y, 0f);
        InstantiateAndInit(spawnPos);
    }

    void InstantiateAndInit(Vector3 position)
    {
        var go = Instantiate(seagullPrefab, position, Quaternion.identity);
        go.SetActive(true);
        var sr = go.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        var gull = go.GetComponent<SeagullBehavior>();
        if (gull != null)
        {
            if (possibleTargets != null && possibleTargets.Length > 0)
            {
                gull.SetPossibleTargets(possibleTargets);
            }
        }
    }
}