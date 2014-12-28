using System;
using SharpOrrery;
using UnityEngine;
using System.Collections;

namespace SharpOrrery
{
    public class CelestialBodyDefinition
    {
        public string title;
        public double mass = 3.3022e23;
        public double radius = 2439;
        public double k;
        public double sideralDay;
        public double tilt;

        public OrbitalElements.OrbitalElementsPieces orbit;
        public CelestialBodyDefinition relativeTo;
        public Func<Double, OrbitalElements.OrbitalElementsPieces> orbitCalculator;
        public bool calculateFromElements;

        public Action<CelestialBody> customInitialize;

        internal void AssignDataToCelestialBody(CelestialBody celestialBody)
        {
            celestialBody.name = title;
            celestialBody.mass = mass;
            celestialBody.radius = radius;
            celestialBody.k = k;
            celestialBody.orbit = orbit;
            celestialBody.tilt = tilt;
            
            if (relativeTo != null)
                celestialBody.relativeTo = SO.U.getBody(relativeTo.title);

            celestialBody.orbitCalculator = orbitCalculator;
            celestialBody.calculateFromElements = calculateFromElements;
        }
    }
}