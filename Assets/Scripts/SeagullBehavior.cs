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


    Transform target = null; 
    Vector3 startPos = Vector3.zero; 
    
    float lerpTime;

    //animation
    Animator anim;
    Vector3 lastPosition;


    enum SeagullStates
    {
        eating,
        showering,
        dying,
        idling
    }

   
    SeagullStates state = SeagullStates.idling;

    //timer that'll count down for hunger
    float hungerTime;
    //hunger stat
    float hungerVal = 5;

    //list for food currently in the scene
    List<GameObject> allFood = new List<GameObject>();

    //holds which game object the seagull has touched
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
            case SeagullStates.showering:
                break;
            case SeagullStates.dying:
                break;
            default:
                break;
        }

        //animation
        UpdateAnimation();

    }

    //animation
    void UpdateAnimation()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        bool isMoving = speed > 0.05f;

        anim.SetBool("isWalking", isMoving);

        lastPosition = transform.position;
    }

    void RunIdle()
    {
        if (target == null)
        { 
            int newTarget = Random.Range(0, possibleTargets.Length); 
            target = possibleTargets[newTarget]; 
            startPos = transform.position; 
            lerpTime = 0; 
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

    void RunEat() {
        if(target == null){ 
            target = FindNearest(allFood); 
            startPos = transform.position; 
            lerpTime = 0; 
        } else {
            transform.position = Move(); 
            if(touchingObj != null){ 
                if(touchingObj.tag == "food"){  
                    allFood.Remove(touchingObj); 
                    hungerVal = 5; 
                    Destroy(touchingObj); 
                    touchingObj = null; 
                    target = null; 
                    state = SeagullStates.idling; 
                }
            }
        }
    }

    void StepNeeds(){
        hungerTime -= Time.deltaTime; 
        if(hungerTime <= 0){ 
            hungerVal--; 
            hungerTime = hungerStep; 
        }
    }

    void FindAllFood(){
        allFood.AddRange(GameObject.FindGameObjectsWithTag("food")); 
    }

    Transform FindNearest(List<GameObject> objsToFind){
        float minDist = Mathf.Infinity; 
        Transform nearest = null; 
        for(int i = 0; i < objsToFind.Count; i++){ 
            float dist = Vector3.Distance(transform.position, objsToFind[i].transform.position); 
            if(dist < minDist){ 
                minDist = dist; 
                nearest = objsToFind[i].transform; 
            }
        }
        return nearest; 
    }

    Vector3 Move(){
        lerpTime += Time.deltaTime; 
        float percent = idleWalkCurve.Evaluate(lerpTime/lerpTimeMax); 
        Vector3 newPos = Vector3.LerpUnclamped(startPos, target.position, percent); 
        return newPos; 
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col != null) touchingObj = col.gameObject; 
    }

    void OnTriggerExit2D(Collider2D col){
        if(col != null) { 
            if(col.gameObject == touchingObj) touchingObj = null; 
        }
    }
}
