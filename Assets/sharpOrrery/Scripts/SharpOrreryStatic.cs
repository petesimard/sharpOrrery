using System;
using System.Globalization;
using UnityEngine;
using System.Collections;

namespace SharpOrrery
{
    public static class SO
    {

        public static string name = "orbit";
	    public static string version = "2013-09-16";

	    //gravitational constant to measure the force with masses in kg and radii in meters N(m/kg)^2
	    public static double G = 6.6742e-11;
	    //astronomical unit in km
	    public static double AU = 149597870;
        public static double CIRCLE = 2 * Math.PI;
        public static double KM = 1000.0;
        public static double DEG_TO_RAD = Math.PI / 180.0;
        public static double RAD_TO_DEG = 180.0 / Math.PI;

        public static double NM_TO_KM = 1.852;
        public static double LB_TO_KG = 0.453592;
        public static double LBF_TO_NEWTON = 4.44822162;
        public static double FT_TO_M = 0.3048;

	    //use physics or orbital elements to animate
	    public static bool USE_PHYSICS_BY_DEFAULT = false;
		
	    //duration in seconds
        public static double DAY = 60 * 60 * 24.0;
	    //duration in days
        public static double YEAR = 365.25;
	    //duration in days
        public static double CENTURY = 100.0 * SO.YEAR;
        public static double SIDERAL_DAY = 3600 * 23.9344696;

        public static double J2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc).GetTime();

	    public static int defaultCalculationsPerTick = 10;

	    //in universe size (so depends on max orbit among all bodies)
        public static double vertexDist = 1.0 / 50.0;

        public static Universe U;
    }


}