using System.Collections.Generic;
using sharpOrrery;
using UnityEngine;
using System.Collections;

public class ScenarioLoader : MonoBehaviour {

    private Dictionary<string, CelestialBodyDefinition> celestialBodyDefinitions = new Dictionary<string, CelestialBodyDefinition>();

    public List<Scenario> scenarios = new List<Scenario>();

    public void InitScenarios()
    {
        foreach (var scenario in scenarios)
        {
            scenario.init(this);
        }
    }

    public CelestialBodyDefinition GetCelestialBodyDefinition(string name)
    {
        return celestialBodyDefinitions[name];
    }

    public void LoadCommonBodies()
    {
        celestialBodyDefinitions.Clear();


        var body = new CelestialBodyDefinition()
        {
            title = "The Sun",
            mass = 1.9891e30,
            radius = 6.96342e5,
            k = 0.01720209895, //gravitational constant (Î¼)                
        };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition()
        {
            title = "Mercury",
			mass = 3.3022e23,
			radius = 2439,
            orbit = new OrbitalElements.OrbitalElementsPieces()
            {
                baseElements = new OrbitalElements.OrbitalElementsPieces() { a = 0.38709927 * ns.AU, e = 0.20563593, i = 7.00497902, l = 252.25032350, lp = 77.45779628, o = 48.33076593 },
                cy = new OrbitalElements.OrbitalElementsPieces() { a = 0.00000037 * ns.AU, e = 0.00001906, i = -0.00594749, l = 149472.67411175, lp = 0.16047689, o = -0.12534081 }
			}             
        };
        celestialBodyDefinitions[body.title] = body;

/*
        var test = new CelestialBodyDefinition()
                   {
                       title = "Mercury 6",
                       mass = 1224.7,
                       radius = 2,
                       orbit = new OrbitalElements.OrbitalElementsPieces()
                               {
                                   baseElements = new OrbitalElements.OrbitalElementsPieces()
                                                  {
                                                      a = ((earthRadius*2) + 159 + 265)/2,
                                                      e = 0.00804,
                                                      w = 0,
                                                      M = 0,
                                                      i = 32.5,
                                                      o = 0
                                                  }
                                   ,
                                   day = new OrbitalElements.OrbitalElementsPieces()
                                         {
                                             a = 0,
                                             e = 0,
                                             i = 0,
                                             M = (360/(88.5*60))*ns.DAY,
                                             w = 0,
                                             o = 0
                                         }
                               }
                   };*/
    }
}
