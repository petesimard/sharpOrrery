using System;
using System.Collections.Generic;
using sharpOrrery;
using UnityEngine;

public class CelestialBody
{
    public delegate void RevolutionDelegate(CelestialBody celestialBody);

    private double angle;
    public bool calculateFromElements;
    private Action<double, DateTime, double> customAfterTick;
    private Action customInitialize;
    private double epoch;
    public Vector3d force;
    public double invMass;

    public bool isCentral;
    public bool isStill;
    public double? k;
    public double mass;
    private Vector3d movement;
    public string name;
    private Action onOrbitCompleted;

    public OrbitalElements.OrbitalElementsPieces orbit;
    private Func<double, OrbitalElements.OrbitalElementsPieces> orbitCalculator;
    private OrbitalElements orbitalElements;
    public Vector3d position;
    private Vector3d? previousPosition;
    private Vector3d previousRelativePosition;
    public double radius;
    private Vector3d relativePosition;
    public CelestialBody relativeTo;

    public RevolutionDelegate revolution;
    private double speed;
    public double? tilt;
    public Vector3d velocity;

    public void AssignInitialValues(CelestialBodyDefinition info)
    {
        // BOMB

        info.AssignDataToCelestialBody(this);
    }

    public void init()
    {
        reset();
        movement = new Vector3d();
        invMass = 1/mass;

        orbitalElements = new OrbitalElements();
        orbitalElements.setName(name);
        orbitalElements.setDefaultOrbit(orbit, orbitCalculator);
    }

    public void reset()
    {
        angle = 0;
        force = new Vector3d();
        movement = new Vector3d();
        previousPosition = null;
    }

    //if epoch start is not j2000, get epoch time from j2000 epoch time
    private double getEpochTime(double epochTime)
    {
        //Debug.Log(epochTime);
        //Debug.Log(this.epoch);

        if (epoch > 0)
        {
            epochTime = epochTime - ((epoch - ns.J2000)/1000); // bomb (original divided by 1000)
        }

        //Debug.Log(epochTime);

        return epochTime;
    }

    public void setPositionFromDate(double epochTime, bool calculateVelocity)
    {
        epochTime = getEpochTime(epochTime);
        position = isCentral ? new Vector3d() : orbitalElements.getPositionFromElements(orbitalElements.calculateElements(epochTime));
        relativePosition = new Vector3d();
        if (calculateVelocity)
        {
            velocity = isCentral ? new Vector3d() : orbitalElements.calculateVelocity(epochTime, relativeTo, calculateFromElements);
        }
    }

    public double getAngleTo(CelestialBody body)
    {
        CelestialBody refBody = ns.U.getBody(body);
        if (refBody != null)
        {
            Vector3d eclPos = (position - (refBody.getPosition())).normalized;
            eclPos.z = 0;
            double angleX = Vector3d.Angle(eclPos, new Vector3d(1, 0, 0))*ns.DEG_TO_RAD;
            double angleY = Vector3d.Angle(eclPos, new Vector3d(0, 1, 0))*ns.DEG_TO_RAD;
            //console.log(angleX, angleY);
            double angle = angleX;
            double q = Math.PI/2;
            if (angleY > q) angle = -angleX;
            return angle;
        }
        return 0;
    }

    public void afterInitialized()
    {
        previousRelativePosition = position;

        positionRelativeTo();

        if (customInitialize != null)
            customInitialize();

        if (customAfterTick != null)
            customAfterTick(ns.U.epochTime, ns.U.date, 0);
    }

    private void positionRelativeTo()
    {
        if (relativeTo != null)
        {
            CelestialBody central = ns.U.getBody(relativeTo);
            if (central != null && central != ns.U.getBody() /**/)
            {
                position += (central.getPosition());
                //console.log(this.name+' pos rel to ' + this.relativeTo);
                velocity += (central.velocity);
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
        OrbitalElements.OrbitalElementsPieces elements = orbitalElements.calculateElements(startTime);
        double period = orbitalElements.calculatePeriod(elements, relativeTo);

        double incr = period/360.0;
        var defaultOrbitalElements = new OrbitalElements();

        //if we want an elliptic orbit from the current planet's position (i.e. the ellipse from which the velocity was computed with vis-viva), setup fake orbital elements from the position
        if (isElliptic)
        {
            defaultOrbitalElements.orbitalElements.baseElements = orbitalElements.calculateElements(startTime, null);

            defaultOrbitalElements.orbitalElements.day = new OrbitalElements.OrbitalElementsPieces {M = 1};
            defaultOrbitalElements.orbitalElements.baseElements.a /= 1000;
            defaultOrbitalElements.orbitalElements.baseElements.i /= ns.DEG_TO_RAD;
            defaultOrbitalElements.orbitalElements.baseElements.o /= ns.DEG_TO_RAD;
            defaultOrbitalElements.orbitalElements.baseElements.w /= ns.DEG_TO_RAD;
            defaultOrbitalElements.orbitalElements.baseElements.M /= ns.DEG_TO_RAD;
            incr = ns.DAY;
            startTime = 0;
        }

        double total = 0.0;
        Vector3d? lastPoint = null;
        for (int i = 0; total < 360; i++)
        {
            OrbitalElements.OrbitalElementsPieces computed = orbitalElements.calculateElements(startTime + (incr*i), defaultOrbitalElements.orbitalElements);
            //if(this.name=='moon')console.log(startTime+(incr*i));
            Vector3d point = orbitalElements.getPositionFromElements(computed);
            if (lastPoint.HasValue)
            {
                angle = Vector3d.Angle(point, lastPoint.Value);
                //make sure we do not go over 360.5 
                if (angle > 1.3 || ((angle + total) > 360.5))
                {
                    for (int j = 0; j < angle; j++)
                    {
                        double step = (incr*(i - 1)) + ((incr/angle)*j);
                        computed = orbitalElements.calculateElements(startTime + step, defaultOrbitalElements.orbitalElements);
                        point = orbitalElements.getPositionFromElements(computed);
                        //when finishing the circle try to avoid going too far over 360 (break after first point going over 360)
                        if (total > 358)
                        {
                            double angleToPrevious = Vector3d.Angle(point, points[points.Count - 1]);
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
        if (!isCentral)
        {
            if (isPositionRelativeTo)
            {
                positionRelativeTo();
            }

            Vector3d relativeToPos = ns.U.getBody(relativeTo).getPosition();
            relativePosition = position - relativeToPos;
            movement = (relativePosition) - (previousRelativePosition);
            speed = movement.magnitude/deltaT;
            angle += Vector3d.Angle(relativePosition, previousRelativePosition)*ns.DEG_TO_RAD;
            previousRelativePosition = relativePosition;

            if (angle > ns.CIRCLE)
            {
                angle = angle%ns.CIRCLE;

                if (revolution != null)
                    revolution.Invoke(this);

                if (onOrbitCompleted != null)
                    onOrbitCompleted();
            }
        }

        if (customAfterTick != null)
            customAfterTick(ns.U.epochTime, ns.U.date, deltaT);
    }

    public Vector3d calculatePosition(double t)
    {
        return orbitalElements.calculatePosition(t);
    }

    public Vector3d getPosition()
    {
        return position;
    }

    public Vector3 GetUnityPosition(double divider)
    {
        // Convert to Unity's coordiante system
        return new Vector3d(position.x, position.z, position.y).GetVector3(divider);
    }

    public Vector3d getVelocity()
    {
        return velocity;
    }

    //return true/false if this body is orbiting the requested body
    public bool isOrbitAround(CelestialBody celestial)
    {
        return celestial == relativeTo;
    }
}