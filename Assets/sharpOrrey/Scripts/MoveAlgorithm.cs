using System;
using System.Collections.Generic;

public abstract class MoveAlgorithm
{
    protected List<CelestialBody> bodies;
    private double deltaT3rd;
    private double deltaT4th;
    private double deltaTSq;
    private double halfDeltaT;
    private double halfDeltaTSq;
    private double inverted_deltaTSq;
    protected double lastDeltaT;
    public string name = "Quadratic";
    protected double onehalf_deltaTSq;
    protected double onehalf_halfDeltaTSq;
    protected double onesixth_deltaT3rd;
    protected double onethird_deltaT3rd;
    protected double onetwelvth_deltaT4th;


    public void init(List<CelestialBody> bodies)
    {
        //console.log(this.name);
        this.bodies = bodies;
    }

    public abstract void moveBodies(double epochTime, double deltaT);

    //precompute all variations of deltaT, only if it changed since last pass
    protected void computeDeltaT(double deltaT)
    {
        if (deltaT != lastDeltaT)
        {
            halfDeltaT = deltaT/2;
            halfDeltaTSq = Math.Pow(halfDeltaT, 2);
            onehalf_halfDeltaTSq = halfDeltaTSq/2;
            deltaTSq = Math.Pow(deltaT, 2);
            onehalf_deltaTSq = deltaTSq/2;
            inverted_deltaTSq = 1/deltaTSq;
            deltaT3rd = Math.Pow(deltaT, 3);
            onethird_deltaT3rd = deltaT3rd/3;
            onesixth_deltaT3rd = deltaT3rd/6;
            deltaT4th = Math.Pow(deltaT, 4);
            onetwelvth_deltaT4th = deltaT4th/12;
        }

        lastDeltaT = deltaT;
    }
}