using SharpOrrery;
using UnityEngine;
using System.Collections;

public class SceneCelestialBody : MonoBehaviour
{
    private CelestialBody celestialBody;
    private double divider = 100000000;

	// Use this for initialization
	void Start () {
	
	}

    public void Init(CelestialBody celestialBody)
    {
        name = celestialBody.name;
        this.celestialBody = celestialBody;
        if (name != "The Sun")
            transform.localScale = new Vector3((float)celestialBody.radius / 100, (float)celestialBody.radius / 100, (float)celestialBody.radius / 100);
        else
            transform.localScale = new Vector3(70, 70, 70);

    }
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position = celestialBody.GetUnityPosition(divider);
	}
}
