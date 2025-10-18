using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    [SerializeField]
    float timeToShark;

    [SerializeField]
    GameObject Shark;

    float timer;

    public void Init(float timeToSharkSeconds, GameObject shark)
    {
        timeToShark = timeToSharkSeconds;
        Shark = shark;
    }

    void Update()
    {

        timer += Time.deltaTime;
        if (timeToShark > 0f && timer >= timeToShark)
        {
            BecomeShark();
        }
    }

    void BecomeShark()
    {
         if (Shark == null)
        {
            return; 
        }

        Instantiate(Shark, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
