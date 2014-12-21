using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Scenario : ScriptableObject
{
    public bool? usePhysics;
    public int? calculationsPerTick;
    public double secondsPerTick;

    [HideInInspector]
    public Dictionary<string, CelestialBodyDefinition> bodies = new Dictionary<string, CelestialBodyDefinition>(); // Name, Body Config
    public bool calculateAll;
    public bool useBarycenter;

    public List<string> common;

    internal void init(ScenarioLoader loader)
    {
        foreach (var bodyName in common)
        {
            bodies[bodyName] = loader.GetCelestialBodyDefinition(bodyName);
        }
    }
}
