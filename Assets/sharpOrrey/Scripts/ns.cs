using UnityEngine;
using System.Collections;

namespace sharpOrrery
{
    public static class ns
    {

        public static string name = "orbit";
	    public static string version = "2013-09-16";

	    //gravitational constant to measure the force with masses in kg and radii in meters N(m/kg)^2
	    public static float G = 6.6742e-11;
	    //astronomical unit in km
	    public static int AU = 149597870;
	    public static float CIRCLE = 2 * Math.PI;
	    public static float KM = 1000f;
	    public static float DEG_TO_RAD = Math.PI/180f;
	    public static float RAD_TO_DEG = 180f/Math.PI;

	    public static float NM_TO_KM = 1.852f;
	    public static float LB_TO_KG = 0.453592f;
	    public static float LBF_TO_NEWTON = 4.44822162f;
	    public static float FT_TO_M = 0.3048f;

	    //use physics or orbital elements to animate
	    public static bool USE_PHYSICS_BY_DEFAULT = false;
		
	    //duration in seconds
	    public static float DAY = 60 * 60 * 24;
	    //duration in days
	    public static float YEAR = 365.25f;
	    //duration in days
	    public static float CENTURY = 100f * ns.YEAR;
	    public static float SIDERAL_DAY = 3600 * 23.9344696f;

        public static float J2000 = new DateTime("").GetTime(); // to-do  new Date('2000-01-01T12:00:00-00:00');

	    public static float defaultCalculationsPerTick = 10;

	    //in universe size (so depends on max orbit among all bodies)
	    public static float vertexDist = 1f/50f;

        public static Universe U;
    }
}