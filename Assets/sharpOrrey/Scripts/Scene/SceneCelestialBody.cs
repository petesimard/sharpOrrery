using UnityEngine;
using System.Collections;

public class SceneCelestialBody : MonoBehaviour
{
    private CelestialBody celestialBody;
    private double divider = 1000000000;

	// Use this for initialization
	void Start () {
	
	}

    public void Init(CelestialBody celestialBody)
    {
        name = celestialBody.name;
        this.celestialBody = celestialBody;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position = celestialBody.getPosition().GetVector3(divider);
	}
}
