using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using MLAgents;
using TMPro;
using UnityEngine;

public class PenguinArea : Area
{
    public PenguinAgent agent;
    public GameObject penguinBaby;
    public Fish fishPrefab;
    public TextMeshPro cumulativeRewardText;

    [HideInInspector] public float fishSpeed;
    [HideInInspector] public float feedRadius = 1f;

    private readonly List<GameObject> fishBeings = new List<GameObject>();

    private void Update()
    {
        cumulativeRewardText.text = agent.GetCumulativeReward().ToString("0.00", CultureInfo.InvariantCulture);
    }

    public override void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawnFish(4, fishSpeed);
    }

    public void RemoveFish(GameObject fish)
    {
        fishBeings.Remove(fish);
        Destroy(fish);
    }

    public static Vector3 RandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        var angle = Random.Range(minAngle, maxAngle);
        var radius = Random.Range(minRadius, maxRadius);

        return center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
    }

    private void RemoveAllFish()
    {
        for (var fishIndex = 0; fishIndex < fishBeings.Count; fishIndex++)
            RemoveFish(fishBeings[fishIndex]);
    }

    private void PlacePenguin()
    {
        agent.transform.position = RandomPosition(transform.position, 0f, 360f, 0f, 9f) + Vector3.up * 0.5f;
        agent.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0f);
    }

    private void PlaceBaby()
    {
        penguinBaby.transform.position = RandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * 0.5f;
        penguinBaby.transform.rotation = Quaternion.Euler(0, 180f, 0f);
    }

    private void SpawnFish(int count, float speed)
    {
        for (int fishIndex = 0; fishIndex < count; fishIndex++)
        {
            var fish = Instantiate(fishPrefab.gameObject);
            
            fish.transform.position = RandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up * 0.5f;
            fish.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0f);
            fish.transform.parent = transform;
            
            fishBeings.Add(fish);
            fish.GetComponent<Fish>().speed = speed;
        }
    }
}
