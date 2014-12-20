using System;
using System.Reflection;
using sharpOrrery;
using UnityEngine;

public class OrbitalElements
{
    public OrbitalElementsPieces orbitalElements;


    private Func<double, OrbitalElementsPieces> calculator;
    private double? epochCorrection;
    private double maxDE = 1e-15;
    private int maxIterationsForEccentricAnomaly = 10;
    private string name;

    private double solveEccentricAnomaly(Func<double, double> f, double x0, int maxIter)
    {
        double x = 0.0;
        double x2 = x0;

        for (int i = 0; i < maxIter; i++)
        {
            x = x2;
            x2 = f(x);
        }

        return x2;
    }

    private Func<double, double> solveKepler(double e, double M)
    {
        return x => { return x + (M + e*Math.Sin(x) - x)/(1 - e*Math.Cos(x)); };
    }

    private Func<double, double> solveKeplerLaguerreConway(double e, double M)
    {
        return x =>
        {
            double s = e*Math.Sin(x);
            double c = e*Math.Cos(x);
            double f = x - s - M;
            double f1 = 1 - c;
            double f2 = s;
            x += -5*f/(f1 + Math.Sign(f1)*Math.Sqrt(Math.Abs(16*f1*f1 - 20*f*f2)));
            return x;
        };
    }

    private Func<double, double> solveKeplerLaguerreConwayHyp(double e, double M)
    {
        return x =>
        {
            double s = e*Math.Sinh(x);
            double c = e*Math.Cosh(x);
            double f = x - s - M;
            double f1 = c - 1;
            double f2 = s;

            x += -5*f/(f1 + Math.Sign(f1)*Math.Sqrt(Math.Abs(16*f1*f1 - 20*f*f2)));
            return x;
        };
    }

    public void setDefaultOrbit(OrbitalElementsPieces orbitalElements, Func<double, OrbitalElementsPieces> calculator)
    {
        this.orbitalElements = orbitalElements;
        if (orbitalElements != null && orbitalElements.epoch.HasValue)
        {
            epochCorrection = ns.U.getEpochTime(orbitalElements.epoch.Value);
        }
        this.calculator = calculator;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public Vector3 calculateVelocity(double timeEpoch, CelestialBody relativeTo, bool isFromDelta)
    {
        if (orbitalElements == null) return new Vector3(0, 0, 0);

        var eclipticVelocity = new Vector3();

        if (isFromDelta)
        {
            Vector3 pos1 = calculatePosition(timeEpoch);
            Vector3 pos2 = calculatePosition(timeEpoch + 60);
            eclipticVelocity = (pos2 - pos1)*(1.0f/60.0f);
        }
        else
        {
            //vis viva to calculate speed (not velocity, i.e not a vector)
            OrbitalElementsPieces el = calculateElements(timeEpoch);
            double speed = Math.Sqrt(ns.G*ns.U.getBody(relativeTo.name).mass*((2.0/(el.r.Value)) - (1.0/(el.a.Value))));

            //now calculate velocity orientation, that is, a vector tangent to the orbital ellipse
            double? k = el.r/el.a;
            double? o = ((2 - (2*el.e*el.e))/(k*(2 - k))) - 1;
            //floating point imprecision
            o = o > 1 ? 1 : o;
            double alpha = Math.PI - Math.Acos(o.Value);
            alpha = el.v < 0 ? (2*Math.PI) - alpha : alpha;
            double velocityAngle = el.v.Value + (alpha/2);
            //velocity vector in the plane of the orbit
            Vector3 orbitalVelocity = new Vector3((float) Math.Cos(velocityAngle), (float) Math.Sin(velocityAngle), 0).normalized*(float) speed;
            OrbitalElementsPieces velocityEls = el.Copy();
            velocityEls.pos = orbitalVelocity;
            velocityEls.v = 0;
            velocityEls.r = 0;
            eclipticVelocity = getPositionFromElements(velocityEls);
        }

        //var diff = eclipticVelocityFromDelta.sub(eclipticVelocity);console.log(diff.length());
        return eclipticVelocity;
    }

    public Vector3 calculatePosition(double timeEpoch)
    {
        if (orbitalElements == null)
            return new Vector3(0, 0, 0);

        OrbitalElementsPieces computed = calculateElements(timeEpoch);
        Vector3 pos = getPositionFromElements(computed);
        return pos;
    }

    private double solveEccentricAnomaly(double e, double M)
    {
        double sol;
        double E;
        if (e == 0.0)
        {
            return M;
        }
        if (e < 0.9)
        {
            sol = solveEccentricAnomaly(solveKepler(e, M), M, 6);
            return sol;
        }
        if (e < 1.0)
        {
            E = M + 0.85*e*((Math.Sin(M) >= 0.0) ? 1 : -1);
            sol = solveEccentricAnomaly(solveKeplerLaguerreConway(e, M), E, 8);
            return sol;
        }
        if (e == 1.0)
        {
            return M;
        }
        
        E = Math.Log(2*M/e + 1.85);
        sol = solveEccentricAnomaly(solveKeplerLaguerreConwayHyp(e, M), E, 30);
        return sol;
    }

    public OrbitalElementsPieces calculateElements(double timeEpoch, OrbitalElementsPieces forcedOrbitalElements = null)
    {
        if (forcedOrbitalElements == null && this.orbitalElements == null)
            return null;

        OrbitalElementsPieces orbitalElements = forcedOrbitalElements ?? this.orbitalElements;

        /*
	
				Epoch : J2000

				a 	Semi-major axis
			    e 	Eccentricity
			    i 	Inclination
			    o 	Longitude of Ascending Node (Î©)
			    w 	Argument of periapsis (Ï‰)
				E 	Eccentric Anomaly
			    T 	Time at perihelion
			    M	Mean anomaly
			    l 	Mean Longitude
			    lp	longitude of periapsis
			    r	distance du centre
			    v	true anomaly

			    P	Sidereal period (mean value)
				Pw	Argument of periapsis precession period (mean value)
				Pn	Longitude of the ascending node precession period (mean value)

			    */
        if (epochCorrection.HasValue)
        {
            timeEpoch -= epochCorrection.Value;
        }
        double tDays = timeEpoch/ns.DAY;
        double T = tDays/ns.CENTURY;
        //console.log(T);
        var computed = new OrbitalElementsPieces();
        computed.t = timeEpoch;

        if (calculator != null && forcedOrbitalElements == null)
        {
            OrbitalElementsPieces realorbit = calculator(T);
            computed = realorbit.Copy();
        }
        else
        {
            if (orbitalElements.baseElements != null)
            {
                double variation = 0.0;

                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("a"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("e"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("i"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("l"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("lp"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("o"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("E"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("M"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("r"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("t"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("v"), computed, T);
                CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("w"), computed, T);


                // Bomb
            }
            else
            {
                computed = orbitalElements.Copy();
            }

            if (!computed.w.HasValue)
            {
                computed.w = computed.lp - computed.o;
            }

            if (!computed.M.HasValue)
            {
                computed.M = computed.l - computed.lp;
            }

            computed.a = computed.a*ns.KM; //was in km, set it in m
        }


        computed.i = ns.DEG_TO_RAD*computed.i;
        computed.o = ns.DEG_TO_RAD*computed.o;
        computed.w = ns.DEG_TO_RAD*computed.w;
        computed.M = ns.DEG_TO_RAD*computed.M;

        computed.E = solveEccentricAnomaly(computed.e.Value, computed.M.Value);

        computed.E = computed.E%ns.CIRCLE;
        computed.i = computed.i%ns.CIRCLE;
        computed.o = computed.o%ns.CIRCLE;
        computed.w = computed.w%ns.CIRCLE;
        computed.M = computed.M%ns.CIRCLE;

        //in the plane of the orbit
        computed.pos = new Vector3((float) (computed.a*(Math.Cos(computed.E.Value) - computed.e.Value)),
            (float) (computed.a.Value*(Math.Sqrt(1 - (computed.e.Value*computed.e.Value)))*Math.Sin(computed.E.Value)));

        computed.r = computed.pos.magnitude;
        computed.v = Math.Atan2(computed.pos.y, computed.pos.x);
        if (!string.IsNullOrEmpty(orbitalElements.relativeTo))
        {
            var relativeTo = ns.U.getBody(orbitalElements.relativeTo);
            if (relativeTo.tilt.HasValue)
            {
                computed.tilt = -relativeTo.tilt.Value*ns.DEG_TO_RAD;
            }
        }
        ;
        return computed;
    }

    private void CalculateVariation(OrbitalElementsPieces orbitalElementsPieces, FieldInfo fieldInfo, OrbitalElementsPieces computed, double T)
    {
        double variation = orbitalElementsPieces.cy != null ? (double) fieldInfo.GetValue(orbitalElements.cy) : ((double) fieldInfo.GetValue(orbitalElements.day)*ns.CENTURY);

        double val = (double) fieldInfo.GetValue(orbitalElements.baseElements) + (variation*T);
        fieldInfo.SetValue(computed, val);
    }

    public Vector3 getPositionFromElements(OrbitalElementsPieces computed)
    {
        if (computed == null) return new Vector3(0, 0, 0);

        var a1 = new Vector3((float) computed.tilt, 0, (float) computed.o.Value);
        Quaternion q1 = Quaternion.Euler(a1);
        var a2 = new Vector3((float) computed.i.Value, 0, (float) computed.w.Value);
        Quaternion q2 = Quaternion.Euler(a2);

        Quaternion planeQuat = q1*q2;
        Vector3 pos = planeQuat*computed.pos; // bomb (should it apply in place?)
        return pos;
    }

    public double calculatePeriod(OrbitalElementsPieces elements, string relativeTo)
    {
        double period = 0.0;
        if (orbitalElements != null && orbitalElements.day != null && orbitalElements.day.M.HasValue)
        {
            period = 360/orbitalElements.day.M.Value;
        }
        else if (ns.U.getBody(relativeTo) && ns.U.getBody(relativeTo).k.HasValue && elements != null)
        {
            period = 2.0*Math.PI*Math.Sqrt(Math.Pow(elements.a.Value/(ns.AU*1000.0), 3))/ns.U.getBody(relativeTo).k.Value;
        }
        period *= ns.DAY; //in seconds
        return period;
    }

    public class OrbitalElementsPieces
    {
        public double? E; // 	Eccentric Anomaly
        public double? M; //	Mean anomaly
        public double? P; //	Sidereal period (mean value)
        public double? Pn; //	Longitude of the ascending node precession period (mean value)
        public double? Pw; //	Argument of periapsis precession period (mean value)
        public double? T; // 	Time at perihelion
        public double? a; // 	Semi-major axis
        public OrbitalElementsPieces baseElements;
        public OrbitalElementsPieces cy;
        public OrbitalElementsPieces day;
        public DateTime? epoch;

        public double? e; // 	Eccentricity
        public double? i; // 	Inclination
        public double? l; // 	Mean Longitude
        public double? lp; //	longitude of periapsis
        public double? o; // 	Longitude of Ascending Node (Î©)
        public Vector3 pos;
        public double? r; //	distance du centre
        public string relativeTo;
        public double? t; // Time maybe??
        public double tilt;
        public double? v; //	true anomaly
        public double? w; // 	Argument of periapsis (Ï‰)

        public OrbitalElementsPieces Copy()
        {
            var element = new OrbitalElementsPieces
                          {
                              relativeTo = relativeTo,
                              pos = pos,
                              baseElements = baseElements,
                              day = day,
                              cy = cy,
                              tilt = tilt,
                              epoch = epoch,
                              t = t,
                              a = a,
                              e = e,
                              i = i,
                              o = o,
                              w = w,
                              E = E,
                              T = T,
                              M = M,
                              l = l,
                              lp = lp,
                              r = r,
                              v = v,
                              P = P,
                              Pw = Pw,
                              Pn = Pn,
                          };

            return element;
        }
    }
}