using UnityEngine;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    public float speed;

    private float randomizedSpeed;
    private float nextActionTime = -1f;
    private Vector3 target;

    private void FixedUpdate()
    {
        if (speed > 0f)
            Swim();
    }

    private void Swim()
    {
        if (Time.fixedTime >= nextActionTime)
            CalculateTime();
        else
            Move();
    }

    private void CalculateTime()
    {
        randomizedSpeed = speed + Random.Range(0.5f, 1.5f);

        target = PenguinArea.RandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);
        transform.rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);

        var timeToGet = Vector3.Distance(transform.position, target) / randomizedSpeed;
        nextActionTime = Time.fixedTime + timeToGet;
    }

    private void Move()
    {
        var move = transform.forward * (randomizedSpeed * Time.fixedDeltaTime);
        if (move.magnitude <= Vector3.Distance(transform.position, target))
        {
            transform.position += move;
        }
        else
        {
            transform.position = target;
            nextActionTime = Time.fixedTime;
        }
    }
}
