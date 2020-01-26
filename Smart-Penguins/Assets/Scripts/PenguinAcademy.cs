using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class PenguinAcademy : Academy
{
    private PenguinArea[] areas;

    public override void AcademyReset()
    {
        if (areas == null)
            areas = FindObjectsOfType<PenguinArea>();

        foreach (var area in areas)
        {
            area.fishSpeed = resetParameters["fish_speed"];
            area.feedRadius = resetParameters["feed_radius"];
            area.ResetArea();
        }
    }
}
