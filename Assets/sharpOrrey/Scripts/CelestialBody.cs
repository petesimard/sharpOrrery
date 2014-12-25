using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using sharpOrrery;

public class CelestialBody
{
    public double mass;
    public bool isCentral;
    public double? tilt;
    public double radius;
    public bool isStill;
    public double? k;
    public double invMass;
    public string name;

    public Vector3d velocity;
    public Vector3d position;
    public Vector3d force;
    public CelestialBody relativeTo;
    public OrbitalElements.OrbitalElementsPieces orbit;
    public bool calculateFromElements;


    private Vector3d movement;
    private  double angle;
    private Vector3d? previousPosition;
    private  double epoch;
    private Vector3d relativePosition;
    private Vector3d previousRelativePosition;
    private OrbitalElements orbitalElements;

    private Action customInitialize;
    private Action<double, DateTime, double> customAfterTick;
    private Action onOrbitCompleted;
    private double speed;

    public delegate void RevolutionDelegate(CelestialBody celestialBody);

    private Func<double, OrbitalElements.OrbitalElementsPieces> orbitCalculator;

    public RevolutionDelegate revolution;

    public void AssignInitialValues(CelestialBodyDefinition info)
    {
        // BOMB

        info.AssignDataToCelestialBody(this);
    }

    public void init()
    {
        reset();
        movement = new Vector3d();
        this.invMass = 1/mass;

        this.orbitalElements = new OrbitalElements();
        this.orbitalElements.setName(name);
        this.orbitalElements.setDefaultOrbit(this.orbit, this.orbitCalculator);
    }

    public void reset()
    {
        this.angle = 0;
        this.force = new Vector3d();
        movement = new Vector3d();
        this.previousPosition = null;
    }

    //if epoch start is not j2000, get epoch time from j2000 epoch time
    private double getEpochTime(double epochTime)
    {
        //Debug.Log(epochTime);
        //Debug.Log(this.epoch);

        if (this.epoch > 0)
        {
            epochTime = epochTime - ((this.epoch - ns.J2000) / 1000); // bomb (original divided by 1000)
        }

        //Debug.Log(epochTime);

        return epochTime;
    }

    public void setPositionFromDate(double epochTime, bool calculateVelocity)
    {
        epochTime = getEpochTime(epochTime);
        this.position = this.isCentral ? new Vector3d() : this.orbitalElements.getPositionFromElements(this.orbitalElements.calculateElements(epochTime));
        this.relativePosition = new Vector3d();
        if (calculateVelocity)
        {
            this.velocity = this.isCentral ? new Vector3d() : this.orbitalElements.calculateVelocity(epochTime, this.relativeTo, this.calculateFromElements);
        }
    }

    public double getAngleTo(CelestialBody body)
    {
        CelestialBody refBody = ns.U.getBody(body);
        if (refBody != null)
        {
            var eclPos = (this.position - (refBody.getPosition())).normalized;
            eclPos.z = 0;
            var angleX = Vector3d.Angle(eclPos, new Vector3d(1, 0, 0)) * ns.DEG_TO_RAD;
            var angleY = Vector3d.Angle(eclPos, new Vector3d(0, 1, 0)) * ns.DEG_TO_RAD;
            //console.log(angleX, angleY);
            var angle = angleX;
            double q = Math.PI/2;
            if (angleY > q) angle = -angleX;
            return angle;
        }
        return 0;
    }

    public void afterInitialized()
    {
        this.previousRelativePosition = this.position;

        positionRelativeTo();

        if (this.customInitialize != null)
            this.customInitialize();

        if (this.customAfterTick != null)
            this.customAfterTick(ns.U.epochTime, ns.U.date, 0);
    }

    private void positionRelativeTo()
    {
        if (this.relativeTo != null)
        {
            CelestialBody central = ns.U.getBody(this.relativeTo);
            if (central != null && central != ns.U.getBody() /**/)
            {
                this.position += (central.getPosition());
                //console.log(this.name+' pos rel to ' + this.relativeTo);
                this.velocity += (central.velocity);
                // bomb
            }
        }
    }

    public void beforeMove(double deltaTIncrement)
    {
    }

    public void afterMove(double deltaTIncrement)
    {
    }

    /**
			Calculates orbit line from orbital elements. By default, the orbital elements might not be osculating, i.e. they might account for perturbations. But the given orbit would thus be different slightly from the planet's path, as the velocity is calculated by considering that the orbit is elliptic.
			*/

    private List<Vector3d> getOrbitVertices(bool isElliptic)
    {
        var points = new List<Vector3d>();

        double startTime = getEpochTime(ns.U.currentTime);
        var elements = this.orbitalElements.calculateElements(startTime);
        var period = this.orbitalElements.calculatePeriod(elements, this.relativeTo);

        double incr = period/360.0;
        var defaultOrbitalElements = new OrbitalElements();

        //if we want an elliptic orbit from the current planet's position (i.e. the ellipse from which the velocity was computed with vis-viva), setup fake orbital elements from the position
        if (isElliptic)
        {
            defaultOrbitalElements.orbitalElements.baseElements = this.orbitalElements.calculateElements(startTime, null);

            defaultOrbitalElements.orbitalElements.day = new OrbitalElements.OrbitalElementsPieces() { M = 1};
            defaultOrbitalElements.orbitalElements.baseElements.a /= 1000;
            defaultOrbitalElements.orbitalElements.baseElements.i /= ns.DEG_TO_RAD;
            defaultOrbitalElements.orbitalElements.baseElements.o /= ns.DEG_TO_RAD;
            defaultOrbitalElements.orbitalElements.baseElements.w /= ns.DEG_TO_RAD;
            defaultOrbitalElements.orbitalElements.baseElements.M /= ns.DEG_TO_RAD;
			incr = ns.DAY;
			startTime = 0;
        }

        var total = 0.0;
        Vector3d? lastPoint = null;
        for (int i = 0; total < 360; i++)
        {
            OrbitalElements.OrbitalElementsPieces computed = this.orbitalElements.calculateElements(startTime + (incr * i), defaultOrbitalElements.orbitalElements);
            //if(this.name=='moon')console.log(startTime+(incr*i));
            Vector3d point = this.orbitalElements.getPositionFromElements(computed);
            if (lastPoint.HasValue)
            {
                angle = Vector3d.Angle(point, lastPoint.Value);
                //make sure we do not go over 360.5 
                if (angle > 1.3 || ((angle + total) > 360.5))
                {
                    for (var j = 0; j < angle; j++)
                    {
                        var step = (incr*(i - 1)) + ((incr/angle)*j);
                        computed = this.orbitalElements.calculateElements(startTime + step, defaultOrbitalElements.orbitalElements);
                        point = this.orbitalElements.getPositionFromElements(computed);
                        //when finishing the circle try to avoid going too far over 360 (break after first point going over 360)
                        if (total > 358)
                        {
                            var angleToPrevious = Vector3d.Angle(point, points[points.Count - 1]);
                            if ((angleToPrevious + total) > 360)
                            {
                                points.Add(point);
                                break;
                            }
                        }

                        points.Add(point);
                    }
                    total += Vector3d.Angle(point, lastPoint.Value);
                    lastPoint = point;
                    continue;
                }
                total += angle;
            }
            points.Add(point);
            lastPoint = point;
        }
        return points;
    }

    public void afterTick(double deltaT, bool isPositionRelativeTo)
    {
        if (!this.isCentral)
        {
            if (isPositionRelativeTo)
            {
                positionRelativeTo();
            }

            var relativeToPos = ns.U.getBody(this.relativeTo).getPosition();
            this.relativePosition = this.position - relativeToPos;
            movement = (this.relativePosition) - (this.previousRelativePosition);
            this.speed = movement.magnitude / deltaT;
            this.angle += Vector3d.Angle(this.relativePosition, this.previousRelativePosition) * ns.DEG_TO_RAD;
            this.previousRelativePosition = this.relativePosition;

            if (this.angle > ns.CIRCLE)
            {
                this.angle = this.angle % ns.CIRCLE;

                if(revolution != null)
                    revolution.Invoke(this);
            
                if (this.onOrbitCompleted != null)
                    this.onOrbitCompleted();
            }
        }

        if (this.customAfterTick != null) 
            this.customAfterTick(ns.U.epochTime, ns.U.date, deltaT);
    }

    public Vector3d calculatePosition(double t)
    {
        return this.orbitalElements.calculatePosition(t);
    }

    public Vector3d getPosition()
    {
        return this.position;
    }

    public Vector3d getVelocity()
    {
        return this.velocity;
    }

    //return true/false if this body is orbiting the requested body
    public bool isOrbitAround(CelestialBody celestial)
    {
        return celestial == this.relativeTo;
    }
}