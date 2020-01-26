using System;
using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class PenguinAgent : Agent
{
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");

    public GameObject heartPrefab;
    public GameObject regurgitatedFishPrefab;

    private PenguinArea area;
    private Animator animator;
    private RayPerception3D rayPerception;

    private GameObject baby;
    private bool isFull;

    private void Start()
    {
        area = GetComponentInParent<PenguinArea>();
        baby = area.penguinBaby;

        animator = GetComponent<Animator>();
        rayPerception = GetComponent<RayPerception3D>();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, baby.transform.position) < area.feedRadius)
        {
            RegurgitateFish();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Fish"))
            EatFish(other.gameObject);
        else if (other.transform.CompareTag("Baby"))
            RegurgitateFish();
    }

    private void EatFish(GameObject fish)
    {
        if (isFull)
            return;

        isFull = true;
        area.RemoveFish(fish);
        AddReward(1f);
    }

    private void RegurgitateFish()
    {
        if (!isFull)
            return;

        isFull = false;
        
        var babyPosition = baby.transform.position;
        var transformParent = transform.parent;

        var regurgitatedFish = Instantiate(regurgitatedFishPrefab, transformParent, true);
        regurgitatedFish.transform.position = babyPosition;
        Destroy(regurgitatedFish, 4f);

        var heart = Instantiate(heartPrefab, transformParent, true);
        heart.transform.position = babyPosition + Vector3.up;
        Destroy(heart, 4f);
        
        AddReward(1f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var forward = vectorAction[0];
        var leftOrRight = 0f;

        switch (vectorAction[1])
        {
            case 1f:
                leftOrRight = -1f;
                break;
            case 2f:
                leftOrRight = 1f;
                break;
        }

        animator.SetFloat(Vertical, forward);
        animator.SetFloat(Horizontal, leftOrRight);

        AddReward(-1f / agentParameters.maxStep);
    }

    public override void AgentReset()
    {
        isFull = false;
        area.ResetArea();
    }

    public override void CollectObservations()
    {
        var (distance, direction) = SpaceInformation();

        AddVectorObs(distance);
        AddVectorObs(direction);
        AddVectorObs(transform.forward);
        AddVectorObs(Perceptions());
        AddVectorObs(isFull);
    }

    private (float, Vector3) SpaceInformation()
    {
        var babyPosition = baby.transform.position;
        var position = transform.position;
        var distance = Vector3.Distance(babyPosition, position);
        var direction = (babyPosition - position).normalized;

        return (distance, direction);
    }

    private List<float> Perceptions()
    {
        var rayDistance = 20f;
        var rayAngles = new[] {30f, 60f, 90f, 120f, 150f};
        var detectables = new[] {"Baby", "Fish", "Wall"};
        return rayPerception.Perceive(rayDistance, rayAngles, detectables, 0f, 0f);
    }
}