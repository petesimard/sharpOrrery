using sharpOrrery;
using UnityEngine;
using System.Collections;

public class CelestialBodyDefinition
{
    public string title;
	public double mass = 3.3022e23;
	public double radius = 2439;
    public double k;
    public OrbitalElements.OrbitalElementsPieces orbit;

    internal void AssignDataToCelestialBody(CelestialBody celestialBody)
    {
        celestialBody.name = title;
        celestialBody.mass = mass;
        celestialBody.radius = radius;
        celestialBody.k = k;
        celestialBody.orbit = orbit;
    }
}
