using System;
using System.Collections.Generic;
using UnityEngine;

public class Ticker
{
    private int actualCalculationsPerTick = 1;
    private List<CelestialBody> bodies = new List<CelestialBody>();
    private int calculationsPerTick = 1;
    private double deltaTIncrement = 1;
    private MoveAlgorithm integration;
    private double secondsPerTick = 1;

    public void setDT()
    {
        if (calculationsPerTick == 0 || secondsPerTick == 0)
            return;

        if (secondsPerTick < calculationsPerTick)
        {
            actualCalculationsPerTick = (int) secondsPerTick;
        }
        else
        {
            actualCalculationsPerTick = calculationsPerTick;
        }
        deltaTIncrement = Math.Round(secondsPerTick/actualCalculationsPerTick);
        secondsPerTick = deltaTIncrement*actualCalculationsPerTick;
    }

    private void moveByGravity(double epochTime)
    {
        for (int t = 1; t <= actualCalculationsPerTick; t++)
        {
            integration.moveBodies(epochTime + (t*deltaTIncrement), deltaTIncrement);
        }
    }

    private void moveByElements(double epochTime)
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].setPositionFromDate(epochTime, false);
        }
    }


    public double tick(bool computePhysics, double epochTime)
    {
        if (computePhysics)
        {
            moveByGravity(epochTime - secondsPerTick);
        }
        else
        {
            moveByElements(epochTime);
        }

        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].afterTick(secondsPerTick, !computePhysics);
        } /**/

        return secondsPerTick;
    }

    public void setBodies(List<CelestialBody> b)
    {
        bodies = new List<CelestialBody>(b);

        integration = new Quadratic();
        integration.init(b);
    }

    public void setCalculationsPerTick(int n)
    {
        calculationsPerTick = n;
        setDT();
    }

    public void setSecondsPerTick(double s)
    {
        secondsPerTick = s;
        setDT();
    }

    public double getDeltaT()
    {
        return secondsPerTick;
    }
}