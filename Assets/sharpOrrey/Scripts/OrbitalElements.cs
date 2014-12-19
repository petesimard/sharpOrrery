using System;
using sharpOrrery;
using UnityEngine;
using System.Collections;

public class OrbitalElements {

    public class OrbitalElementsPieces
    {
        public OrbitalElementsPieces baseElements;
        public OrbitalElementsPieces cy;
        public OrbitalElementsPieces day;

        public double tilt;
        public Vector3 pos;
        public double? epoch;

        public double? a; // 	Semi-major axis
        public double? e; // 	Eccentricity
        public double? i; // 	Inclination
        public double? o; // 	Longitude of Ascending Node (Î©)
        public double? w; // 	Argument of periapsis (Ï‰)
        public double? E; // 	Eccentric Anomaly
        public double? T; // 	Time at perihelion
        public double? M; //	Mean anomaly
        public double? l; // 	Mean Longitude
        public double? lp; //	longitude of periapsis
        public double? r; //	distance du centre
        public double? v; //	true anomaly

        public double? t; // Time maybe??

        public double? P; //	Sidereal period (mean value)
        public double? Pw; //	Argument of periapsis precession period (mean value)
        public double? Pn; //	Longitude of the ascending node precession period (mean value)
        public string relativeTo;

        public OrbitalElementsPieces Copy()
        {
            var element = new OrbitalElementsPieces()
                          {
                              relativeTo = relativeTo,
                              pos = pos,
                              baseElements = baseElements,
                              day = day,
                              cy = cy,
                              tilt = tilt,
                              epoch = tilt,

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

    int maxIterationsForEccentricAnomaly = 10;
	double maxDE = 1e-15;
    private  Func<double, OrbitalElementsPieces> calculator;
    private  OrbitalElementsPieces orbitalElements;
    private double? epochCorrection;
    private  string name;

		private double solveEccentricAnomaly(Func<double, double> f, double x0, int maxIter) {
				
			var x = 0.0;
			var x2 = x0;
			
			for (var i = 0; i < maxIter; i++) {
				x = x2;
				x2 = f(x);
			}
			
			return x2;
		}

		private Func<double, double> solveKepler(double e, double M)
		{
		    return (x) => {
				return x + (M + e * Math.Sin(x) - x) / (1 - e * Math.Cos(x));

		    };
		}

		private Func<double, double> solveKeplerLaguerreConway(double e, double M)
        {
		    return (x) => {

				var s = e * Math.Sin(x);
				var c = e * Math.Cos(x);
				var f = x - s - M;
				var f1 = 1 - c;
				var f2 = s;
				x += -5 * f / (f1 + Math.Sign(f1) * Math.Sqrt(Math.Abs(16 * f1 * f1 - 20 * f * f2)));
				return x;
			};
		}

		private Func<double, double> solveKeplerLaguerreConwayHyp(double e, double M)
        {
		    return (x) => {
				var s = e * Math.Sinh(x);
				var c = e * Math.Cosh(x);
				var f = x - s - M;
				var f1 = c - 1;
				var f2 = s;

				x += -5 * f / (f1 + Math.Sign(f1) * Math.Sqrt(Math.Abs(16 * f1 * f1 - 20 * f * f2)));
				return x;
			};
		}

		private	void setDefaultOrbit(OrbitalElementsPieces orbitalElements, Func<double, OrbitalElementsPieces> calculator) {
				this.orbitalElements = orbitalElements;
				if(orbitalElements != null && orbitalElements.epoch.HasValue) {
					this.epochCorrection = ns.U.getEpochTime(orbitalElements.epoch);
				}
				this.calculator = calculator;
			}

            public void setName(string name){
				this.name = name;
			}

			public Vector3 calculateVelocity(double timeEpoch, CelestialBody relativeTo, bool isFromDelta) {
				if(this.orbitalElements == null) return new Vector3(0,0,0);

				var eclipticVelocity = new Vector3();
				
    			if ( isFromDelta ) {
	    			var pos1 = this.calculatePosition(timeEpoch);
    				var pos2 = this.calculatePosition(timeEpoch + 60);
    				eclipticVelocity = (pos2 - pos1) * (1.0f/60.0f);
    			} else {
    				//vis viva to calculate speed (not velocity, i.e not a vector)
					var el = this.calculateElements(timeEpoch);
					var speed = Math.Sqrt(ns.G * ns.U.getBody(relativeTo).mass * ((2 / (el.r)) - (1 / (el.a))));

					//now calculate velocity orientation, that is, a vector tangent to the orbital ellipse
					var k = el.r / el.a;
					var o = ((2 - (2 * el.e * el.e)) / (k * (2-k)))-1;
					//floating point imprecision
					o = o > 1 ? 1 : o;
					var alpha = Math.PI - Math.Acos(o.Value);
					alpha = el.v < 0 ? (2 * Math.PI) - alpha  : alpha;
					var velocityAngle = el.v.Value + (alpha / 2);
					//velocity vector in the plane of the orbit
					var orbitalVelocity = new Vector3((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle), 0).normalized * (float)speed;
					var velocityEls = el.Copy();
                    velocityEls.pos = orbitalVelocity;
                    velocityEls.v = 0;
                    velocityEls.r = 0;
	    			eclipticVelocity = this.getPositionFromElements(velocityEls);
    			}

    			//var diff = eclipticVelocityFromDelta.sub(eclipticVelocity);console.log(diff.length());
    			return eclipticVelocity;
			}

			public Vector3 calculatePosition(double timeEpoch) {
				if(this.orbitalElements == null)
                    return new Vector3(0,0,0);
				
                var computed = this.calculateElements(timeEpoch);
				var pos =  this.getPositionFromElements(computed);
				return pos;
			}

			private double solveEccentricAnomaly(double e, double M){
				if (e == 0.0) {
					return M;
				}  else if (e < 0.9) {
					var sol = solveEccentricAnomaly(solveKepler(e, M), M, 6);
					return sol;
				} else if (e < 1.0) {
					var E = M + 0.85 * e * ((Math.Sin(M) >= 0.0) ? 1 : -1);
					var sol = solveEccentricAnomaly(solveKeplerLaguerreConway(e, M), E, 8);
					return sol;
				} else if (e == 1.0) {
					return M;
				} else {
					var E = Math.Log(2 * M / e + 1.85);
					var sol = solveEccentricAnomaly(solveKeplerLaguerreConwayHyp(e, M), E, 30);
					return sol;
				}
			}

			private OrbitalElementsPieces calculateElements(double timeEpoch, OrbitalElementsPieces forcedOrbitalElements = null) 
            {
				if(forcedOrbitalElements == null && this.orbitalElements == null) 
                    return null;

				var orbitalElements = forcedOrbitalElements ?? this.orbitalElements;

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
			    if (this.epochCorrection.HasValue) {
			    	timeEpoch -= this.epochCorrection.Value;
			    }
				var tDays = timeEpoch / ns.DAY;
				var T = tDays / ns.CENTURY ;
				//console.log(T);
			    var computed = new OrbitalElementsPieces();
			    computed.t = timeEpoch;

				if(this.calculator != null && forcedOrbitalElements == null) {
					var realorbit = this.calculator(T);
                    computed = realorbit.Copy();
				} else {

					if (orbitalElements.baseElements != null) {
						var variation = 0.0;

					    CalculateVariation(orbitalElements.baseElements, orbitalElements.baseElements.GetType().GetField("a"), computed, T);
						// Bomb
					} else
					{
					    computed = orbitalElements.Copy();
					}

					if (!computed.w.HasValue) {
						computed.w = computed.lp - computed.o;
					}

					if (!computed.M.HasValue) {
						computed.M = computed.l - computed.lp;
					}

					computed.a = computed.a * ns.KM;//was in km, set it in m
				}


				computed.i = ns.DEG_TO_RAD * computed.i;
				computed.o = ns.DEG_TO_RAD * computed.o;
				computed.w = ns.DEG_TO_RAD * computed.w;
				computed.M = ns.DEG_TO_RAD * computed.M;

				computed.E = this.solveEccentricAnomaly(computed.e.Value, computed.M.Value);

				computed.E = computed.E % ns.CIRCLE;
				computed.i = computed.i % ns.CIRCLE;
				computed.o = computed.o % ns.CIRCLE;
				computed.w = computed.w % ns.CIRCLE;
				computed.M = computed.M % ns.CIRCLE;

				//in the plane of the orbit
				computed.pos = new Vector3((float)(computed.a * (Math.Cos(computed.E.Value) - computed.e.Value)), 
                    (float)(computed.a.Value * (Math.Sqrt(1 - (computed.e.Value*computed.e.Value))) * Math.Sin(computed.E.Value)));

				computed.r = computed.pos.magnitude;
    			computed.v = Math.Atan2(computed.pos.y, computed.pos.x);
    			if(!string.IsNullOrEmpty(orbitalElements.relativeTo)) {
    				var relativeTo = ns.U.getBody(orbitalElements.relativeTo);
    				if(relativeTo.tilt) {
    					computed.tilt = -relativeTo.tilt * ns.DEG_TO_RAD;
    				}
    			};
				return computed;
			}

            private void CalculateVariation(OrbitalElementsPieces orbitalElementsPieces, System.Reflection.FieldInfo fieldInfo, OrbitalElementsPieces computed, double T)
            {
			    var variation = orbitalElementsPieces.cy != null ? (double)fieldInfo.GetValue(orbitalElements.cy) : ((double)fieldInfo.GetValue(orbitalElements.day) * ns.CENTURY);

                var val = (double)fieldInfo.GetValue(orbitalElements.baseElements) + (variation*T);
                fieldInfo.SetValue(computed, val);
            }

			getPositionFromElements : function(computed) {

				if(!computed) return new Vector3(0,0,0);

				var a1 = new THREE.Euler(computed.tilt || 0, 0, computed.o, 'XYZ');
				var q1 = new THREE.Quaternion().setFromEuler(a1);
				var a2 = new THREE.Euler(computed.i, 0, computed.w, 'XYZ');
				var q2 = new THREE.Quaternion().setFromEuler(a2);

				var planeQuat = new THREE.Quaternion().multiplyQuaternions(q1, q2);
				computed.pos.applyQuaternion(planeQuat);
				return computed.pos;
			},

			calculatePeriod : function(elements, relativeTo) {
				var period;
				if(this.orbitalElements && this.orbitalElements.day && this.orbitalElements.day.M) {
					period = 360 / this.orbitalElements.day.M ;
				}else if(ns.U.getBody(relativeTo) && ns.U.getBody(relativeTo).k && elements) {
					period = 2 * Math.PI * Math.sqrt(Math.pow(elements.a/(ns.AU*1000), 3)) / ns.U.getBody(relativeTo).k;
				}
				period *= ns.DAY;//in seconds
				return period;
			}
		};
	}

}
