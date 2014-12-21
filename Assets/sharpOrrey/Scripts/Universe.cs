using System;
using System.Collections.Generic;
using System.Linq;
using sharpOrrery;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public double epochTime;
    public DateTime date;
    public double currentTime;
    public bool playing;

    public ScenarioLoader scenarioLoader;


    private readonly Ticker ticker = new Ticker();
    private Dictionary<string, CelestialBody> bodies;
    private CelestialBody centralBody;
    public Scenario scenario;

    private double size;
    private double startEpochTime;
    private bool? usePhysics = true;

    void Start()
    {
        scenarioLoader.LoadCommonBodies();
        scenarioLoader.InitScenarios();
        init();
    }

    public void init()
    {
        //name = scenario.name;
        //var initialSettings = _.extend({}, scenario.defaultGuiSettings, qstrSettings, scenario.forcedGuiSettings);
        //console.log(initialSettings);

        //Universe is, well, global
        ns.U = this;

        usePhysics = scenario.usePhysics.HasValue ? scenario.usePhysics : ns.USE_PHYSICS_BY_DEFAULT;

        /*
            		this.dateDisplay = Gui.addDate(function(){
					this.playing = false;
					this.epochTime = 0;
					this.currentTime = this.startEpochTime = this.getEpochTime(Gui.getDate());
					this.repositionBodies();
					this.scene.onDateReset();
				}.bind(this));
                 */

        playing = false;
        epochTime = 0;

        date = new DateTime(); // bomb (needs to go on UI time)
        currentTime = startEpochTime = getEpochTime(date);

        createBodies(scenario);

        calculateDimensions();

        initBodies(scenario);
        ticker.setSecondsPerTick(scenario.secondsPerTick); // bomb - original used .initial
        ticker.setCalculationsPerTick(scenario.calculationsPerTick.HasValue ? scenario.calculationsPerTick.Value : ns.defaultCalculationsPerTick);
    }


    private void createBodies(Scenario scenario)
    {
        bodies = new Dictionary<string, CelestialBody>();

        foreach (string bodyName in scenario.bodies.Keys)
        {
            var body = new CelestialBody();
            body.AssignInitialValues(scenario.bodies[bodyName]);
            body.name = bodyName;
            centralBody = (centralBody != null && centralBody.mass > body.mass) ? centralBody : body;
            bodies[body.name] = body;
        }

        centralBody.isCentral = true;
    }

    private void initBodies(Scenario scenario)
    {
        foreach (var celestialBodyKV in bodies)
        {
            CelestialBody body = celestialBodyKV.Value;

            if (!scenario.calculateAll && !body.isCentral)
            {
                body.mass = 1;
            }

            body.init();
            body.setPositionFromDate(currentTime, true);
        }


        setBarycenter();

        //after all is inialized
        foreach (var celestialBodyKV in bodies)
        {
            CelestialBody body = celestialBodyKV.Value;
            //this.scene.addBody(body);

            body.afterInitialized();
            //console.log(body.name, body.isCentral);
        }


        //this.scene.setCentralBody(this.centralBody);

        ticker.setBodies(bodies.Values.ToList());
    }

    /* balance the system by calculating the masses of the non-central bodies and placing the central body to balance it.*/

    private void setBarycenter()
    {
        CelestialBody central = centralBody;
        if (!usePhysics.Value || central.isStill || scenario.useBarycenter == false)
            return;

        double massRatio = 0.0;

        double massCenter_mass = 0.0;
        var massCenter_pos = new Vector3();
        var massCenter_momentum = new Vector3();

        foreach (var celestialBodyKV in bodies)
        {
            CelestialBody b = celestialBodyKV.Value;

            if (b == central)
                continue;
            massCenter_mass += b.mass;
            massRatio = b.mass/massCenter_mass;
            massCenter_pos = (massCenter_pos + b.getPosition())*(float) massRatio;
            massCenter_momentum = (massCenter_momentum + b.getVelocity())*(float) b.mass;
        }

        massCenter_momentum *= (float) (1f/massCenter_mass);

        massRatio = massCenter_mass/central.mass;
        central.velocity = massCenter_momentum*(float) (massRatio*-1);
        central.position = massCenter_pos*(float) (massRatio*-1);

        foreach (var celestialBodyKV in bodies)
        {
            CelestialBody b = celestialBodyKV.Value;

            if (b == central || (b.relativeTo != null && b.relativeTo != central))
                continue;

            b.velocity += central.velocity;
            //if central body's mass is way bigger than the object, we assume that the central body is the center of rotation. Otherwise, it's the barycenter
            if (central.mass/b.mass > 10e10)
            {
                b.position += central.position;
            }
            else if (b.relativeTo == central)
            {
                b.relativeTo = null;
            }
        }
    }

    private void repositionBodies()
    {
        foreach (CelestialBody body in bodies.Values)
        {
            body.reset();
            body.setPositionFromDate(currentTime, true);
        }

        ticker.tick(false, currentTime);

        setBarycenter();

        //adjust position depending on other bodies' position (for example a satellite is relative to its main body)
        foreach (CelestialBody body in bodies.Values)
        {
            body.afterInitialized();
        }
    }

    // Original code used null name to represent central body
    public CelestialBody getBody(CelestialBody body = null)
    {
        if (body == null)
        {
            return centralBody;
        }

        if (!bodies.ContainsKey(body.name))
            return null;

        return bodies[name];
    }

    private void calculateDimensions()
    {
        string centralBodyName = getBody().name;

        //find the largest radius in km among all bodies
        double largestRadius = bodies.Values.Max(b => b.radius);
        // bomb - This needs to be verified

        //find the largest semi major axis in km among all bodies

        double? largestSMA = bodies.Values.Where(b => !b.isCentral && b.orbit != null).Max(b => b.orbit.baseElements.a);

        double? smallestSMA = bodies.Values.Where(b => !b.isCentral && b.orbit != null && (b.relativeTo == null || b.relativeTo == centralBody)).Max(b => b.orbit.baseElements.a);


        smallestSMA *= ns.KM;
        largestSMA *= ns.KM;
        largestRadius *= ns.KM;

        //console.log('universe size', largestSMA, ' m');

        size = largestSMA.Value;
        //this.scene.setDimension(largestSMA, smallestSMA, largestRadius);
    }
    
    void Update()
    {
        tick();
    }

    private void tick()
    {
        if (playing)
        {
            epochTime += ticker.getDeltaT();
            currentTime = startEpochTime + epochTime;
            ticker.tick(usePhysics.Value, currentTime);

            //this.scene.updateCamera();
            //this.scene.draw();
            //this.showDate();
        }

        //requestAnimFrame(this.ticker);
    }

/*

			getScene : function(){
				return this.scene;
			},
*/

    public double getEpochTime(DateTime userDate)
    {
        return (userDate.GetTime() - ns.J2000);
    }

    public bool isPlaying()
    {
        return playing;
    }

    public void stop()
    {
        playing = false;
        //this.scene.updateCamera();
        //this.scene.draw();
    }
}