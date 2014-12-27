using System.Collections.Generic;
using SharpOrrery;
using UnityEngine;

public class ScenarioLoader : MonoBehaviour
{
    private readonly Dictionary<string, CelestialBodyDefinition> celestialBodyDefinitions = new Dictionary<string, CelestialBodyDefinition>();

    public List<Scenario> scenarios = new List<Scenario>();

    public void InitScenarios()
    {
        foreach (Scenario scenario in scenarios)
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


        var body = new CelestialBodyDefinition
                   {
                       title = "The Sun",
                       mass = 1.9891e30,
                       radius = 6.96342e5,
                       k = 0.01720209895, //gravitational constant (Î¼)                
                   };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Mercury",
                   mass = 3.3022e23,
                   radius = 2439,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 0.38709927*SO.AU, e = 0.20563593, i = 7.00497902, l = 252.25032350, lp = 77.45779628, o = 48.33076593},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = 0.00000037*SO.AU, e = 0.00001906, i = -0.00594749, l = 149472.67411175, lp = 0.16047689, o = -0.12534081}
                           }
               };
        celestialBodyDefinitions[body.title] = body;


        body = new CelestialBodyDefinition
               {
                   title = "Venus",
                   mass = 4.868e24,
                   radius = 6051,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 0.72333566*SO.AU, e = 0.00677672, i = 3.39467605, l = 181.97909950, lp = 131.60246718, o = 76.67984255},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = 0.00000390*SO.AU, e = -0.00004107, i = -0.00078890, l = 58517.81538729, lp = 0.00268329, o = -0.27769418}
                           }
               };
        celestialBodyDefinitions[body.title] = body;


        body = new CelestialBodyDefinition
               {
                   title = "The Earth",
                   mass = 5.9736e24,
                   radius = 3443.9307*SO.NM_TO_KM,
                   sideralDay = SO.SIDERAL_DAY,
                   tilt = 23 + (26/60) + (21/3600),
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 1.00000261*SO.AU, e = 0.01671123, i = -0.00001531, l = 100.46457166, lp = 102.93768193, o = 0.0},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = 0.00000562*SO.AU, e = -0.00004392, i = -0.01294668, l = 35999.37244981, lp = 0.32327364, o = 0.0}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Mars",
                   mass = 6.4185e23,
                   radius = 3376,
                   sideralDay = 1.025957*SO.DAY,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 1.52371034*SO.AU, e = 0.09339410, i = 1.84969142, l = -4.55343205, lp = -23.94362959, o = 49.55953891},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = 0.00001847*SO.AU, e = 0.00007882, i = -0.00813131, l = 19140.30268499, lp = 0.44441088, o = -0.29257343}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Jupiter",
                   mass = 1.8986e27,
                   radius = 71492,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 5.20288700*SO.AU, e = 0.04838624, i = 1.30439695, l = 34.39644051, lp = 14.72847983, o = 100.47390909},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = -0.00011607*SO.AU, e = -0.00013253, i = -0.00183714, l = 3034.74612775, lp = 0.21252668, o = 0.20469106}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Saturn",
                   mass = 5.6846e26,
                   radius = 58232,
                   tilt = 26.7,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 9.53667594*SO.AU, e = 0.05386179, i = 2.48599187, l = 49.95424423, lp = 92.59887831, o = 113.66242448},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = -0.00125060*SO.AU, e = -0.00050991, i = 0.00193609, l = 1222.49362201, lp = -0.41897216, o = -0.28867794}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Uranus",
                   mass = 8.6810e25,
                   radius = 25559,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 19.18916464*SO.AU, e = 0.04725744, i = 0.77263783, l = 313.23810451, lp = 170.95427630, o = 74.01692503},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = -0.00196176*SO.AU, e = -0.00004397, i = -0.00242939, l = 428.48202785, lp = 0.40805281, o = 0.04240589}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Neptune",
                   mass = 1.0243e26,
                   radius = 24764,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 30.06992276*SO.AU, e = 0.00859048, i = 1.77004347, l = -55.12002969, lp = 44.96476227, o = 131.78422574},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = 0.00026291*SO.AU, e = 0.00005105, i = 0.00035372, l = 218.45945325, lp = -0.32241464, o = -0.00508664}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Pluto",
                   mass = 1.305e22 + 1.52e21,
                   radius = 1153,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 39.48211675*SO.AU, e = 0.24882730, i = 17.14001206, l = 238.92903833, lp = 224.06891629, o = 110.30393684},
                               cy = new OrbitalElements.OrbitalElementsPieces {a = -0.00031596*SO.AU, e = 0.00005170, i = 0.00004818, l = 145.20780515, lp = -0.04062942, o = -0.01183482}
                           }
               };
        celestialBodyDefinitions[body.title] = body;

        body = new CelestialBodyDefinition
               {
                   title = "Halley's Comet",
                   mass = 2.2e14,
                   radius = 50,
                   orbit = new OrbitalElements.OrbitalElementsPieces
                           {
                               baseElements = new OrbitalElements.OrbitalElementsPieces {a = 17.83414429*SO.AU, e = 0.967142908, i = 162.262691, M = 360*(438393600/(75.1*SO.YEAR*SO.DAY)), w = 111.332485, o = 58.420081},
                               day = new OrbitalElements.OrbitalElementsPieces {a = 0, e = 0, i = 0, M = (360/(75.1*365.25)), w = 0, o = 0}
                           }
               };
        celestialBodyDefinitions[body.title] = body;
        /*
        body = new CelestialBodyDefinition()
        {
				title = "The Moon",
				mass = 7.3477e22,
				radius = 1738.1,
				sideralDay = (27.3215782 * SO.DAY) ,
				tilt = 1.5424,
				relativeTo = celestialBodyDefinitions["The Earth"],
				orbitCalculator = MoonRealOrbit,
				calculateFromElements = true,
				orbit= new OrbitalElements.OrbitalElementsPieces(){
					baseElements = new OrbitalElements.OrbitalElementsPieces(){
						a = 384400,
						e = 0.0554,
						w = 318.15,
						M = 135.27,
						i = 5.16,
						o = 125.08
					},
					day = new OrbitalElements.OrbitalElementsPieces(){
						a = 0,
						e = 0,
						i = 0,
						M = 13.176358,//360 / 27.321582,
						w = (360 / 5.997) / 365.25,
						o = (360 / 18.600) / 365.25
					}	
				},
				getMapRotation : function(angle){
					if(angle > 0) {
						return angle - Math.PI;
					}
					return angle + Math.PI;
				},
				customInitialize : function() {
					if(this.relativeTo !== 'earth') return;
					this.baseMapRotation = this.getMapRotation(this.getAngleTo('earth'));
					this.nextCheck = this.sideralDay;
				},
				customAfterTick : function(time){
					if(this.relativeTo !== 'earth') return;
					//when a sideral day has passed, make sure that the near side is still facing the earth. Since the moon's orbit is heavily disturbed, some imprecision occurs in its orbit, and its duration is not always the same, especially in an incomplete scenario (where there are no sun/planets). Therefore, a correction is brought to the base map rotation, tweened so that is is not jerky.
					if(time >= this.nextCheck){
						this.nextCheck += this.sideralDay;
						TweenMax.to(this, 2, {baseMapRotation : this.getMapRotation(this.getAngleTo('earth')), ease:Sine.easeInOut});
					}
				}
			}*/
    }
}