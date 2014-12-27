using System.Collections.Generic;
using SharpOrrery;
using UnityEngine;

namespace SharpOrrery
{
    public class Quadratic : MoveAlgorithm
    {
        private double halfDeltaT;
        private double inverted_deltaTSq;


        public override void moveBodies(double epochTime, double deltaT)
        {
            this.computeDeltaT(deltaT);

            //var i, b, n = {};
            var n = new BodyCalcContainer[bodies.Count];
            for (int i = 0; i < n.Length; i++)
            {
                n[i] = new BodyCalcContainer();
            }

            //forces at t0;
            Gravity.calculateGForces(bodies);

            //find accel at t0 and pos at t0.5
            for (int i = 0; i < bodies.Count; i++)
            {
                CelestialBody b = bodies[i];
                if (b.isStill) continue;
                b.beforeMove(deltaT);

                if (b.calculateFromElements)
                {
                    b.setPositionFromDate(epochTime + (halfDeltaT), false); //bomb - original didn't supply 2nd param. Defaults to false?
                }
                else
                {
                    n[i].accel.Add(b.force*b.invMass);

                    n[i].pos.Add(b.position);
                    n[i].pos.Add(b.position + (b.getVelocity()*halfDeltaT) + (n[i].accel[0]*onehalf_halfDeltaTSq));

                    b.position = n[i].pos[1];
                }
            }

            //forces at t0.5 (all this.bodies are positionned at t0.5)
            Gravity.calculateGForces(bodies);

            //find accel at t0.5 and positions at t1
            for (int i = 0; i < bodies.Count; i++)
            {
                CelestialBody b = bodies[i];
                if (b.isStill)
                    continue;

                if (b.calculateFromElements)
                {
                    b.setPositionFromDate(epochTime + deltaT, false);
                }
                else
                {
                    n[i].accel.Add(b.force*b.invMass);

                    //pos1 = pos0 + (vel0 * deltat) + (accel05 * 0.5 * Math.pow(deltaT, 2))
                    n[i].pos.Add(
                        n[i].pos[0] + (b.getVelocity()*deltaT)
                        + (n[i].accel[1]*onehalf_deltaTSq));

                    b.position = (n[i].pos[2]);
                }
            }

            //forces at t1
            Gravity.calculateGForces(bodies);

            //find accel at t1
            for (int i = 0; i < bodies.Count; i++)
            {
                CelestialBody b = bodies[i];
                if (b.isStill)
                    continue;
                if (!b.calculateFromElements)
                {
                    n[i].accel.Add(b.force*b.invMass);
                }
            }

            //perform the actual integration
            var c1 = new Vector3d();
            var c2 = new Vector3d();
            var deltaV = new Vector3d();
            var deltaP = new Vector3d();

            for (int i = 0; i < bodies.Count; i++)
            {
                CelestialBody b = bodies[i];

                if (!b.calculateFromElements && !b.isStill)
                {
                    c1 = (n[i].accel[0]*-3)
                         - n[i].accel[2]
                         + (n[i].accel[1]*4)
                         *(1/deltaT);

                    c2 = n[i].accel[0]
                         + (n[i].accel[2])
                         - (n[i].accel[1]*2)
                         *2
                         *inverted_deltaTSq;

                    deltaV = n[i].accel[0]
                             *deltaT
                             + (c1*(onehalf_deltaTSq))
                             + (c2*(onethird_deltaT3rd));

                    deltaP = b.getVelocity()
                             *deltaT
                             + (n[i].accel[0]*onehalf_deltaTSq)
                             + (c1*(onesixth_deltaT3rd))
                             + (c2*(onetwelvth_deltaT4th));

                    bodies[i].position = (n[i].pos[0]) + (deltaP);
                    bodies[i].velocity += (deltaV);
                }

                b.afterMove(deltaT);
            }
        }

        private class BodyCalcContainer
        {
            public readonly List<Vector3d> accel = new List<Vector3d>();
            public readonly List<Vector3d> pos = new List<Vector3d>();
        }
    }
}