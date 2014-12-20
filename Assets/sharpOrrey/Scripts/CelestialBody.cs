using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using sharpOrrery;

public class CelestialBody : MonoBehaviour
{
    public double mass;
    public bool isCentral;
    public double? tilt;
    public double radius;
    public bool isStill;
    public double? k;

    public Vector3 velocity;
    public Vector3 position;
    public CelestialBody relativeTo;
    public OrbitalElements.OrbitalElementsPieces orbit;
    public bool calculateFromElements;


    private Vector3 movement;
    private  double invMass;
    private  double angle;
    private  Vector3 force;
    private Vector3? previousPosition;
    private  double epoch;
    private Vector3 relativePosition;
    private Vector3 previousRelativePosition;
    private OrbitalElements orbitalElements;

    private Action customInitialize;
    private Action<double, DateTime, double> customAfterTick;
    private Action onOrbitCompleted;
    private double speed;

    public delegate void RevolutionDelegate(CelestialBody celestialBody);

    private Func<double, OrbitalElements.OrbitalElementsPieces> orbitCalculator;

    public RevolutionDelegate revolution;

    public void AssignInitialValues(OrbitalElements.OrbitalElementsPieces info)
    {
        // BOMB
    }

    public void init()
    {
        reset();
        movement = new Vector3();
        this.invMass = 1/mass;

        this.orbitalElements = new OrbitalElements();
        this.orbitalElements.setName(name);
        this.orbitalElements.setDefaultOrbit(this.orbit, this.orbitCalculator);
    }

    public void reset()
    {
        this.angle = 0;
        this.force = new Vector3();
        movement = new Vector3();
        this.previousPosition = null;
    }

    //if epoch start is not j2000, get epoch time from j2000 epoch time
    private double getEpochTime(double epochTime)
    {
        if (this.epoch > 0)
        {
            epochTime = epochTime - (this.epoch - ns.J2000); // bomb (original divided by 1000)
        }
        return epochTime;
    }

    public void setPositionFromDate(double epochTime, bool calculateVelocity)
    {
        epochTime = getEpochTime(epochTime);
        this.position = this.isCentral ? new Vector3() : this.orbitalElements.getPositionFromElements(this.orbitalElements.calculateElements(epochTime));
        this.relativePosition = new Vector3();
        if (calculateVelocity)
        {
            this.velocity = this.isCentral ? new Vector3() : this.orbitalElements.calculateVelocity(epochTime, this.relativeTo, this.calculateFromElements);
        }
    }

    public double getAngleTo(string bodyName)
    {
        CelestialBody refBody = ns.U.getBody(bodyName);
        if (refBody)
        {
            var eclPos = (this.position - (refBody.getPosition())).normalized;
            eclPos.z = 0;
            var angleX = Vector3.Angle(eclPos, new Vector3(1, 0, 0)) * ns.DEG_TO_RAD;
            var angleY = Vector3.Angle(eclPos, new Vector3(0, 1, 0)) * ns.DEG_TO_RAD;
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
        if (this.relativeTo)
        {
            CelestialBody central = this.relativeTo;
            if (central && central != ns.U.getBody() /**/)
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

    private List<Vector3> getOrbitVertices(bool isElliptic)
    {
        var points = new List<Vector3>();

        double startTime = getEpochTime(ns.U.currentTime);
        var elements = this.orbitalElements.calculateElements(startTime);
        var period = this.orbitalElements.calculatePeriod(elements, this.relativeTo.name);

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
        Vector3? lastPoint = null;
        for (int i = 0; total < 360; i++)
        {
            OrbitalElements.OrbitalElementsPieces computed = this.orbitalElements.calculateElements(startTime + (incr * i), defaultOrbitalElements.orbitalElements);
            //if(this.name=='moon')console.log(startTime+(incr*i));
            Vector3 point = this.orbitalElements.getPositionFromElements(computed);
            if (lastPoint.HasValue)
            {
                angle = Vector3.Angle(point, lastPoint.Value);
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
                            var angleToPrevious = Vector3.Angle(point, points[points.Count - 1]);
                            if ((angleToPrevious + total) > 360)
                            {
                                points.Add(point);
                                break;
                            }
                        }

                        points.Add(point);
                    }
                    total += Vector3.Angle(point, lastPoint.Value);
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

            var relativeToPos = this.relativeTo.getPosition();
            this.relativePosition = this.position - relativeToPos;
            movement = (this.relativePosition) - (this.previousRelativePosition);
            this.speed = movement.magnitude / deltaT;
            this.angle += Vector3.Angle(this.relativePosition, this.previousRelativePosition) * ns.DEG_TO_RAD;
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

    public Vector3 calculatePosition(double t)
    {
        return this.orbitalElements.calculatePosition(t);
    }

    public Vector3 getPosition()
    {
        return this.position;
    }

    public Vector3 getVelocity()
    {
        return this.velocity;
    }

    //return true/false if this body is orbiting the requested body
    public bool isOrbitAround(CelestialBody celestial)
    {
        return celestial == this.relativeTo;
    }
}