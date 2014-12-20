using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Scenario
{
    public string name;
    
    public bool? usePhysics;
    public int? calculationsPerTick;
    public double secondsPerTick;
    public Dictionary<string, OrbitalElements.OrbitalElementsPieces> bodies; // Name, Body Config
    public bool calculateAll;
    public bool useBarycenter;
}
