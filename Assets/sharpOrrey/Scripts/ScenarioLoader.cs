using System.Collections.Generic;
using SharpOrrery;
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
                baseElements = new OrbitalElements.OrbitalElementsPieces() { a = 0.38709927 * SO.AU, e = 0.20563593, i = 7.00497902, l = 252.25032350, lp = 77.45779628, o = 48.33076593 },
                cy = new OrbitalElements.OrbitalElementsPieces() { a = 0.00000037 * SO.AU, e = 0.00001906, i = -0.00594749, l = 149472.67411175, lp = 0.16047689, o = -0.12534081 }
			}             
        };
        celestialBodyDefinitions[body.title] = body;


        body = new CelestialBodyDefinition()
        {
            title = "Venus",
            mass = 4.868e24,
            radius = 6051,
            orbit = new OrbitalElements.OrbitalElementsPieces()
                    {
                        baseElements = new OrbitalElements.OrbitalElementsPieces() {a = 0.72333566*SO.AU, e = 0.00677672, i = 3.39467605, l = 181.97909950, lp = 131.60246718, o = 76.67984255},
                        cy = new OrbitalElements.OrbitalElementsPieces() {a = 0.00000390*SO.AU, e = -0.00004107, i = -0.00078890, l = 58517.81538729, lp = 0.00268329, o = -0.27769418}
                    }
        };
        celestialBodyDefinitions[body.title] = body;



        body = new CelestialBodyDefinition()
        {
            title = "The Earth",
            mass = 5.9736e24,
            radius = 3443.9307*SO.NM_TO_KM,
            sideralDay = SO.SIDERAL_DAY,
            tilt = 23 + (26/60) + (21/3600),
            orbit = new OrbitalElements.OrbitalElementsPieces()
                    {
                        baseElements = new OrbitalElements.OrbitalElementsPieces() {a = 1.00000261*SO.AU, e = 0.01671123, i = -0.00001531, l = 100.46457166, lp = 102.93768193, o = 0.0},
                        cy = new OrbitalElements.OrbitalElementsPieces() {a = 0.00000562*SO.AU, e = -0.00004392, i = -0.01294668, l = 35999.37244981, lp = 0.32327364, o = 0.0}
                    }
        };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition()
        {
            title = "Mars",
            mass = 6.4185e23,
            radius = 3376,
            sideralDay = 1.025957*SO.DAY,
            orbit = new OrbitalElements.OrbitalElementsPieces()
            {
                baseElements = new OrbitalElements.OrbitalElementsPieces(){a = 1.52371034*SO.AU, e = 0.09339410, i = 1.84969142, l = -4.55343205, lp = -23.94362959, o = 49.55953891},
                cy = new OrbitalElements.OrbitalElementsPieces(){a = 0.00001847*SO.AU, e = 0.00007882, i = -0.00813131, l = 19140.30268499, lp = 0.44441088, o = -0.29257343}
            }
        };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition()
        {
            title = "Jupiter",
            mass = 1.8986e27,
            radius = 71492,
            orbit = new OrbitalElements.OrbitalElementsPieces()
            {
                baseElements = new OrbitalElements.OrbitalElementsPieces(){a = 5.20288700*SO.AU, e = 0.04838624, i = 1.30439695, l = 34.39644051, lp = 14.72847983, o = 100.47390909},
                cy = new OrbitalElements.OrbitalElementsPieces(){a = -0.00011607*SO.AU, e = -0.00013253, i = -0.00183714, l = 3034.74612775, lp = 0.21252668, o = 0.20469106}
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
