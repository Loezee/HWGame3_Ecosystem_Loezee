using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SeagullBehavior : MonoBehaviour
{


    [SerializeField]
    Transform[] possibleTargets; 

    [SerializeField]
    float lerpTimeMax; 

    [SerializeField]
    AnimationCurve idleWalkCurve; 

    [SerializeField]
    float hungerStep; 

    //animation
    Animator anim;
    Vector3 lastPosition;
    
    Transform target = null; 
    Vector3 startPos = Vector3.zero; 
    
    float lerpTime;


    enum SeagullStates
    {
        eating,
        dying,
        idling
    }

    SeagullStates state = SeagullStates.idling;

    //hunger
    float hungerTime;
    float hungerVal = 5;

    List<GameObject> allFood = new List<GameObject>();

    GameObject touchingObj;


    void Start()
    {
        FindAllFood(); //find all food objs in the scene
        hungerTime = hungerStep; //reset our hunger timer

        //animation
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        
    }

    void Update()
    {
        switch (state)
        {
            case SeagullStates.idling: 
                RunIdle(); 
                break;
            case SeagullStates.eating: 
                RunEat(); 
                break;
            case SeagullStates.dying:
                RunDying();
                break;
            default:
                break;
        }

        //animation
        if (state != SeagullStates.dying) 
        {
            UpdateAnimation();
        }
        else if (anim) 
        {
            anim.SetBool("isWalking", false);
        }

    }


    void RunIdle()
    {
        if (target == null)
        { 
            int newTarget = Random.Range(0, possibleTargets.Length); 
            target = possibleTargets[newTarget]; 
            startPos = transform.position; 
            lerpTime = 0f; 
        }
        else
        {
            transform.position = Move(); 
        }
        StepNeeds(); 
        if (hungerVal <= 0)
        { 
            target = null; 
            state = SeagullStates.eating; 
        }

    }

    void RunEat() 
    {
        if(target == null)
        { 
            RefreshFoodList(); // for spawned bubbles

            if (allFood.Count == 0)
            {
                state = SeagullStates.idling;
                return;
            }

            target = FindNearest(allFood); 
            startPos = transform.position; 
            lerpTime = 0; 
        } 
        else 
        {
            transform.position = Move(); 

            if (touchingObj != null && touchingObj.CompareTag("food"))
            {
                allFood.Remove(touchingObj);
                hungerVal = 5f;
                Destroy(touchingObj);
                touchingObj = null;
                target = null;
                state = SeagullStates.idling;
            }
        }
    }

    void RunDying()
    {

    }

    public void Die()
    {
        if (state == SeagullStates.dying)
        {
            return;
        }


        state = SeagullStates.dying;
        target = null;


        var col = GetComponent<Collider2D>();
        if (col) 
        {
            col.enabled = false;
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb) 
        {
            rb.linearVelocity = Vector2.zero;
        }

        Destroy(gameObject, 1f);
    }

    void StepNeeds()
    {
        hungerTime -= Time.deltaTime; 

        if(hungerTime <= 0f)
        { 
            hungerVal--; 
            hungerTime = hungerStep; 
        }
    }

    void FindAllFood()
    {
        allFood.AddRange(GameObject.FindGameObjectsWithTag("food")); 
    }

    void RefreshFoodList()
    {
        allFood.Clear();
        allFood.AddRange(GameObject.FindGameObjectsWithTag("food"));
    }

    Transform FindNearest(List<GameObject> objsToFind)
    {
        float minDist = Mathf.Infinity; 
        Transform nearest = null; 

        for(int i = 0; i < objsToFind.Count; i++)
        { 
            float dist = Vector3.Distance(transform.position, objsToFind[i].transform.position); 
            if(dist < minDist)
            { 
                minDist = dist; 
                nearest = objsToFind[i].transform; 
            }
        }
        return nearest; 
    }

    Vector3 Move()
    {
        if (target == null)
        {
            return transform.position;
        } 

        lerpTime += Time.deltaTime;
        float t = Mathf.Clamp01(lerpTime / Mathf.Max(0.0001f, lerpTimeMax));
        float percent = idleWalkCurve.Evaluate(t);

        Vector3 newPos = Vector3.Lerp(startPos, target.position, percent);

        // Snap when very close so we reliably enter triggers
        if (Vector3.Distance(newPos, target.position) <= 0.02f)
        {
            newPos = target.position;
        }

        return newPos;
    }

    //animation
    void UpdateAnimation()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        bool isMoving = speed > 0.05f;

        anim.SetBool("isWalking", isMoving);

        lastPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col != null) 
        {
            touchingObj = col.gameObject; 
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col != null) 
        { 
            if(col.gameObject == touchingObj) touchingObj = null; 
        }
    }

    //spawner
    public void SetPossibleTargets(Transform[] targets)
    {
        this.possibleTargets = targets;
    }
}



