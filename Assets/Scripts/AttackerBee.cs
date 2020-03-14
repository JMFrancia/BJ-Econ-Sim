using UnityEngine;
using System.Collections;

//Attack Bee prefab should contain a 2D animated sprite of bee,
//as well as this script
public class AttackerBee : MonoBehaviour
{
    Vector2 target;  //Target position that the attacker is flying toward
    Vector2 direction; //Normalized direction of movement
    float speed;    //Speed of attacker bee movement
    float minDistance = .2f;  //Min distance to target before helper bee disappears

    //Tells attacker to fly toward target transform
    public void SetTarget(Vector2 target, float speed)
    {
        this.speed = speed;
        this.target = target;
        direction = (target - (Vector2)transform.position).normalized;
    }

    private void Update()
    {
        //If within minDistance of target, destroy self
        if (Vector2.Distance(transform.position, target) < minDistance)
        {
            Destroy(gameObject);
        }

        //Get min of distance and speed to prevent overshooting target
        float distance = Vector2.Distance(target, transform.position);
        Vector2 delta = Mathf.Min(distance, speed) * direction;

        transform.position += (Vector3)delta;
    }
}
