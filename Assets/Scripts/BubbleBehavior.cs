using UnityEngine;

public class BubbleBehavior : MonoBehaviour
{
    enum BubbleStates
    {
        bubble,
        fish,
        shark
    }

    [SerializeField]
    float timeToFish;

    [SerializeField]
    GameObject Fish;

    [SerializeField]
    bool fishIsFood = false;

    float timer;
    BubbleStates state = BubbleStates.bubble;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        state = BubbleStates.bubble;
        timer = 0f;

        gameObject.tag = "food";
    }

    // Update is called once per frame
    void Update()
    {
        if (state != BubbleStates.bubble)
        {
            return;
        } 

        timer += Time.deltaTime;
        if (timer >= timeToFish)
        {
            TransformIntoFish();
        }
    }

    void TransformIntoFish()
    {
        state = BubbleStates.fish;

        if (Fish != null)
        {
            var fish = Instantiate(Fish, transform.position, Quaternion.identity);
            if (fishIsFood)
            {
                fish.tag = "food";
            }

        }

        Destroy(gameObject); // destroy bubble
    }
}
