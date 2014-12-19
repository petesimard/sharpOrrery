using System;
using UnityEngine;
using Object = UnityEngine.Object;
using sharpOrrery;

public class CelestialBody : MonoBehaviour
{
    private float mass;
    private Vector3 movement;
    private  float invMass;
    private  float angle;
    private  Vector3 force;
    private Vector3? previousPosition;
    private  float epoch;
    private bool isCentral;
    private Vector3 position;
    private Vector3 relativePosition;
    private Vector3 velocity;
    private CelestialBody relativeTo;
    private bool calculateFromElements;
    private Vector3 previousRelativePosition;

    private Action customInitialize;
    private Action<float, DateTime> customAfterTick;


    private void Awake()
    {
        reset();
        movement = new Vector3();
        this.invMass = 1/mass;

        this.orbitalElements = new OrbitalElements();
        this.orbitalElements.setName(name);
        this.orbitalElements.setDefaultOrbit(this.orbit, this.orbitCalculator);
    }

    private void reset()
    {
        this.angle = 0;
        this.force = new Vector3();
        movement = new Vector3();
        this.previousPosition = null;
    }

    //if epoch start is not j2000, get epoch time from j2000 epoch time
    private float getEpochTime(float epochTime)
    {
        if (this.epoch > 0)
        {
            epochTime = epochTime - ((this.epoch.Value.GetTime() - ns.J2000.GetTime())/1000);
        }
        return epochTime;
    }

    private void setPositionFromDate(float epochTime, bool calculateVelocity)
    {
        epochTime = getEpochTime(epochTime);
        this.position = this.isCentral ? new Vector3() : this.orbitalElements.getPositionFromElements(this.orbitalElements.calculateElements(epochTime));
        this.relativePosition = new Vector3();
        if (calculateVelocity)
        {
            this.velocity = this.isCentral ? new Vector3() : this.orbitalElements.calculateVelocity(epochTime, this.relativeTo, this.calculateFromElements);
        }
    }

    public float getAngleTo(string bodyName)
    {
        CelestialBody refBody = ns.U.getBody(bodyName);
        if (refBody)
        {
            var eclPos = (this.position - (refBody.getPosition())).normalized;
            eclPos.z = 0;
            var angleX = Vector3.Angle(eclPos, new Vector3(1, 0, 0)) * Mathf.Deg2Rad;
            var angleY = Vector3.Angle(eclPos, new Vector3(0, 1, 0)) * Mathf.Deg2Rad;
            //console.log(angleX, angleY);
            var angle = angleX;
            double q = Math.PI/2;
            if (angleY > q) angle = -angleX;
            return angle;
        }
        return 0;
    }

    private void afterInitialized()
    {
        this.previousRelativePosition = this.position;

        positionRelativeTo();

        if (this.customInitialize != null)
            this.customInitialize();

        if (this.customAfterTick != null)
            this.customAfterTick(ns.U.epochTime, ns.U.date);
    }

    private void positionRelativeTo()
    {
        if (this.relativeTo)
        {
            var central = ns.U.getBody(this.relativeTo);
            if (central && central != = ns.U.getBody() /**/)
            {
                this.position.add(central.position);
                //console.log(this.name+' pos rel to ' + this.relativeTo);
                this.velocity && central.velocity && this.velocity.add(central.velocity);
            }
        }
    }

    private void beforeMove(float deltaTIncrement)
    {
    }

    private void afterMove(float deltaTIncrement)
    {
    }

    /**
			Calculates orbit line from orbital elements. By default, the orbital elements might not be osculating, i.e. they might account for perturbations. But the given orbit would thus be different slightly from the planet's path, as the velocity is calculated by considering that the orbit is elliptic.
			*/

    private void getOrbitVertices(bool isElliptic)
    {
        double startTime = getEpochTime(ns.U.currentTime);
        var elements = this.orbitalElements.calculateElements(startTime);
        var period = this.orbitalElements.calculatePeriod(elements, this.relativeTo);
        if (!period) return;

        int incr = period/360;
        var points = [];
        var lastPoint;
        var point;
        var j;
        var angle;
        var step;
        int total = 0;
        var defaultOrbitalElements;
        var computed;
        var angleToPrevious;


        //if we want an elliptic orbit from the current planet's position (i.e. the ellipse from which the velocity was computed with vis-viva), setup fake orbital elements from the position
        if (isElliptic)
        {
            defaultOrbitalElements =
            {
                base :
                this.orbitalElements.calculateElements(startTime, null, true)
            }
            ;
            defaultOrbitalElements.day =
            {
                1
            }
            ;
            defaultOrbitalElements.
            base.a /= 1000;
            defaultOrbitalElements.
            base.i /= ns.DEG_TO_RAD;
            defaultOrbitalElements.
            base.o /= ns.DEG_TO_RAD;
            defaultOrbitalElements.
            base.w /= ns.DEG_TO_RAD;
            defaultOrbitalElements.
            base.M /= ns.DEG_TO_RAD;
            incr = ns.DAY;
            startTime = 0;
        }

        for (int i = 0; total < 360; i++)
        {
            computed = this.orbitalElements.calculateElements(startTime + (incr*i), defaultOrbitalElements);
            //if(this.name=='moon')console.log(startTime+(incr*i));
            point = this.orbitalElements.getPositionFromElements(computed);
            if (lastPoint)
            {
                angle = point.angleTo(lastPoint)*ns.RAD_TO_DEG;
                //make sure we do not go over 360.5 
                if (angle > 1.3 || ((angle + total) > 360.5))
                {
                    for (j = 0; j < angle; j++)
                    {
                        step = (incr*(i - 1)) + ((incr/angle)*j);
                        computed = this.orbitalElements.calculateElements(startTime + step, defaultOrbitalElements);
                        point = this.orbitalElements.getPositionFromElements(computed);
                        //when finishing the circle try to avoid going too far over 360 (break after first point going over 360)
                        if (total > 358)
                        {
                            angleToPrevious = point.angleTo(points[points.length - 1])*ns.RAD_TO_DEG;
                            if ((angleToPrevious + total) > 360)
                            {
                                points.push(point);
                                break;
                            }
                        }

                        points.push(point);
                    }
                    total += point.angleTo(lastPoint)*ns.RAD_TO_DEG;
                    lastPoint = point;
                    continue;
                }
                total += angle;
            }
            points.push(point);
            lastPoint = point;
        }
        return points;
    }

    private void afterTick(float deltaT, bool isPositionRelativeTo)
    {
        if (!this.isCentral)
        {
            if (isPositionRelativeTo)
            {
                positionRelativeTo();
            }

            var relativeToPos = ns.U.getBody(this.relativeTo).getPosition();
            this.relativePosition.copy(this.position).sub(relativeToPos);
            movement.copy(this.relativePosition).sub(this.previousRelativePosition);
            this.speed = movement.length()/deltaT;
            this.angle += this.relativePosition.angleTo(this.previousRelativePosition);
            this.previousRelativePosition.copy(this.relativePosition);

            if (this.angle > ns.CIRCLE)
            {
                this.angle = this.angle%ns.CIRCLE;
                this.dispatchEvent(
                {
                    "revolution"
                }
            )
                ;
                if (this.onOrbitCompleted) this.onOrbitCompleted();
            }
        }
        if (this.customAfterTick) this.customAfterTick(ns.U.epochTime, ns.U.date, deltaT);
    }

    private Vector3 calculatePosition(float t)
    {
        return this.orbitalElements.calculatePosition(t);
    }

    private Vector3 getPosition()
    {
        return this.position;
    }

    private Vector3 getVelocity()
    {
        return this.velocity;
    }

    //return true/false if this body is orbiting the requested body
    private bool isOrbitAround(CelestialBody celestial)
    {
        return celestial.name == = this.relativeTo;
    }
}