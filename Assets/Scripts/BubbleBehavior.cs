using UnityEngine;

public class BubbleBehavior : MonoBehaviour
{

    enum BubbleStates 
    { 
        Bubble, 
        Shark, 
        SharkDead 
    }


    BubbleStates state = BubbleStates.Bubble;


    [SerializeField] float timeToShark = 12f;   
    [SerializeField] float sharkLifetime = 8f;  
    [SerializeField] float sharkDieDuration = 0.1f; 
    float timer;

    [SerializeField] string formParam = "Form";
    [SerializeField] string bubbleStateName = "Bubble_idle";
    [SerializeField] string sharkStateName  = "Shark_swim";

    [SerializeField] string foodTag = "food";         
    [SerializeField] string predatorTag = "predator"; 
    [SerializeField] string seagullTag = "Seagull";   

    [SerializeField] float sharkCircleRadius = 1.6f;
    [SerializeField] float sharkAngularSpeedDeg = 70f;

    Vector3 sharkCenter;
    float sharkAngleDeg;

    SpriteRenderer sr;
    Animator anim;
    Collider2D col;

    void Awake()
    {
        sr   = GetComponent<SpriteRenderer>();
        col  = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        col.isTrigger = true;
    }

    void OnEnable()
    {
        EnterBubble();
    }

    void Update()
    {
        switch (state)
        {
            case BubbleStates.Bubble:
                RunBubble();
                break;

            case BubbleStates.Shark:
                RunShark();
                break;

            case BubbleStates.SharkDead:
                RunSharkDead();
                break;
        }
    }


    void EnterBubble()
    {
        state = BubbleStates.Bubble;
        timer = 0f;
        SetForm(0, bubbleStateName);
        gameObject.tag = foodTag; 
    }

    void EnterShark()
    {
        state = BubbleStates.Shark;
        timer = 0f;
        SetForm(2, sharkStateName);
        gameObject.tag = predatorTag;

        sharkCenter   = transform.position;
        sharkAngleDeg = Random.Range(0f, 360f);
    }

    void EnterSharkDead()
    {
        state = BubbleStates.SharkDead;
        timer = 0f;

        gameObject.tag = "Untagged";
        if (col) 
        {
            col.enabled = false;
        }

        Destroy(gameObject, sharkDieDuration);
    }

    void RunBubble()
    {
        timer += Time.deltaTime;
        if (timer >= timeToShark)
        {
            EnterShark();
        }
    }

    void RunShark()
    {
        //turning
        sharkAngleDeg += sharkAngularSpeedDeg * Time.deltaTime;
        float r = sharkAngleDeg * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(r), Mathf.Sin(r), 0f) * sharkCircleRadius;
        transform.position = sharkCenter + offset;

        timer += Time.deltaTime;
        if (timer >= sharkLifetime)
        {
             EnterSharkDead();
        }

    }

    void RunSharkDead()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == BubbleStates.Shark && other != null && other.CompareTag(seagullTag))
        {
            var gull = other.GetComponent<SeagullBehavior>();
            if (gull != null) 
            {
                gull.Die();
            }
        }
    }

    void SetForm(int value, string stateNameToPlay)
    {
        if (!anim) 
        {
            return;
        }
        
        anim.SetInteger(formParam, value);
        anim.Play(stateNameToPlay, 0, 0f); 
    }

}