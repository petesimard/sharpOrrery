using System;
using System.Reflection;
using SharpOrrery;
using UnityEngine;

namespace SharpOrrery
{
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
                epochCorrection = SO.U.getEpochTime(orbitalElements.epoch.Value);
            }
            this.calculator = calculator;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public Vector3d calculateVelocity(double timeEpoch, CelestialBody relativeTo, bool isFromDelta)
        {
            if (orbitalElements == null) return new Vector3d(0, 0, 0);

            var eclipticVelocity = new Vector3d();

            if (isFromDelta)
            {
                Vector3d pos1 = calculatePosition(timeEpoch);
                Vector3d pos2 = calculatePosition(timeEpoch + 60);
                eclipticVelocity = (pos2 - pos1)*(1.0/60.0);
            }
            else
            {
                //vis viva to calculate speed (not velocity, i.e not a vector)
                OrbitalElementsPieces el = calculateElements(timeEpoch);
                double speed = Math.Sqrt(SO.G*SO.U.getBody(relativeTo).mass*((2.0/(el.r.Value)) - (1.0/(el.a.Value))));

                //now calculate velocity orientation, that is, a vector tangent to the orbital ellipse
                double? k = el.r/el.a;
                double? o = ((2 - (2*el.e*el.e))/(k*(2 - k))) - 1;
                //floating point imprecision
                o = o > 1 ? 1 : o;
                double alpha = Math.PI - Math.Acos(o.Value);
                alpha = el.v < 0 ? (2*Math.PI) - alpha : alpha;
                double velocityAngle = el.v.Value + (alpha/2);
                //velocity vector in the plane of the orbit
                Vector3d orbitalVelocity = new Vector3d(Math.Cos(velocityAngle), Math.Sin(velocityAngle), 0).normalized*speed;
                OrbitalElementsPieces velocityEls = el.Copy();
                velocityEls.pos = orbitalVelocity;
                velocityEls.v = 0;
                velocityEls.r = 0;
                eclipticVelocity = getPositionFromElements(velocityEls);
            }

            //var diff = eclipticVelocityFromDelta.sub(eclipticVelocity);console.log(diff.length());
            return eclipticVelocity;
        }

        public Vector3d calculatePosition(double timeEpoch)
        {
            if (orbitalElements == null)
                return new Vector3d(0, 0, 0);

            OrbitalElementsPieces computed = calculateElements(timeEpoch);
            Vector3d pos = getPositionFromElements(computed);
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

/*        Debug.Log(this.name);

        Debug.Log(this.orbitalElements);
        Debug.Log(this.orbitalElements.cy);*/

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
            double tDays = timeEpoch/SO.DAY;
            double T = tDays/SO.CENTURY;
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

                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("a"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("e"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("i"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("l"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("lp"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("o"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("E"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("M"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("r"), computed, T);
                    //CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("t"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("v"), computed, T);
                    CalculateVariation(orbitalElements, orbitalElements.GetType().GetField("w"), computed, T);
/*
							variation = orbitalElements.cy ? orbitalElements.cy[el] : (orbitalElements.day[el] * ns.CENTURY);
							variation = variation || 0;
							computed[el] = orbitalElements.base[el] + (variation * T);
 */

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

                computed.a = computed.a*SO.KM; //was in km, set it in m
            }


            computed.i = SO.DEG_TO_RAD*computed.i;
            computed.o = SO.DEG_TO_RAD*computed.o;
            computed.w = SO.DEG_TO_RAD*computed.w;
            computed.M = SO.DEG_TO_RAD*computed.M;

            computed.E = solveEccentricAnomaly(computed.e.Value, computed.M.Value);

            computed.E = computed.E%SO.CIRCLE;
            computed.i = computed.i%SO.CIRCLE;
            computed.o = computed.o%SO.CIRCLE;
            computed.w = computed.w%SO.CIRCLE;
            computed.M = computed.M%SO.CIRCLE;

            //in the plane of the orbit
            computed.pos = new Vector3d((computed.a*(Math.Cos(computed.E.Value) - computed.e.Value)).Value,
                (computed.a.Value*(Math.Sqrt(1 - (computed.e.Value*computed.e.Value)))*Math.Sin(computed.E.Value)));

            computed.r = computed.pos.magnitude;
            computed.v = Math.Atan2(computed.pos.y, computed.pos.x);
            if (orbitalElements.relativeTo != null)
            {
                var relativeTo = SO.U.getBody(orbitalElements.relativeTo);
                if (relativeTo.tilt.HasValue)
                {
                    computed.tilt = -relativeTo.tilt.Value*SO.DEG_TO_RAD;
                }
            }

            return computed;
        }

        private void CalculateVariation(OrbitalElementsPieces orbitalElementsPieces, FieldInfo fieldInfo, OrbitalElementsPieces computed, double T)
        {
            if (!((double?) fieldInfo.GetValue(orbitalElements.baseElements)).HasValue)
                return;

            double variation = 0;

            if (orbitalElements.cy != null)
            {
                var baseVal = (double?) fieldInfo.GetValue(orbitalElements.cy);
                if (baseVal.HasValue)
                    variation = baseVal.Value;
            }
            else
            {
                var baseVal = (double?) fieldInfo.GetValue(orbitalElements.day);
                if (baseVal.HasValue)
                    variation = baseVal.Value*SO.CENTURY;
            }


            double val = (double) fieldInfo.GetValue(orbitalElements.baseElements) + (variation*T);
            fieldInfo.SetValue(computed, val);
        }

        public Vector3d getPositionFromElements(OrbitalElementsPieces computed)
        {
            /*	if(!computed) return new THREE.Vector3(0,0,0);

				var a1 = new THREE.Euler(computed.tilt || 0, 0, computed.o, 'XYZ');
				var q1 = new THREE.Quaternion().setFromEuler(a1);
				var a2 = new THREE.Euler(computed.i, 0, computed.w, 'XYZ');
				var q2 = new THREE.Quaternion().setFromEuler(a2);

				var planeQuat = new THREE.Quaternion().multiplyQuaternions(q1, q2);
				computed.pos.applyQuaternion(planeQuat);
				return computed.pos;
*/
            if (computed == null) return new Vector3d(0, 0, 0);

            var a1 = new Vector3d(computed.tilt, 0, computed.o.Value);
            QuaternionD q1 = QuaternionD.EulerRad(a1);
            var a2 = new Vector3d(computed.i.Value, 0, computed.w.Value);
            QuaternionD q2 = QuaternionD.EulerRad(a2);

            QuaternionD planeQuat = q1*q2;
            Vector3d pos = planeQuat*computed.pos; // bomb (should it apply in place?)
            return pos;
        }

        public double calculatePeriod(OrbitalElementsPieces elements, CelestialBody relativeTo)
        {
            double period = 0.0;
            if (orbitalElements != null && orbitalElements.day != null && orbitalElements.day.M.HasValue)
            {
                period = 360/orbitalElements.day.M.Value;
            }
            else if (SO.U.getBody(relativeTo) != null && SO.U.getBody(relativeTo).k.HasValue && elements != null)
            {
                period = 2.0*Math.PI*Math.Sqrt(Math.Pow(elements.a.Value/(SO.AU*1000.0), 3))/SO.U.getBody(relativeTo).k.Value;
            }
            period *= SO.DAY; //in seconds
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
            public Vector3d pos;
            public double? r; //	distance du centre
            public CelestialBody relativeTo;
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
}