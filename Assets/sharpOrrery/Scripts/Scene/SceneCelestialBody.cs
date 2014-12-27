using SharpOrrery;
using UnityEngine;
using System.Collections;

public class SceneCelestialBody : MonoBehaviour
{
    public CelestialBody celestialBody;
    public const double Divider = 1000000000;

    private OrbitRenderer orbitRenderer;


	// Use this for initialization
	void Start () {
        orbitRenderer = gameObject.AddComponent<OrbitRenderer>();
        orbitRenderer.Init(this);
        orbitRenderer.showOrbit();
	}

    public void Init(CelestialBody celestialBody)
    {
        name = celestialBody.name;
        this.celestialBody = celestialBody;
        if (name != "The Sun")
            transform.localScale = new Vector3((float)celestialBody.radius / 1000, (float)celestialBody.radius / 1000, (float)celestialBody.radius / 1000);
        else
            transform.localScale = new Vector3(70, 70, 70);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position = celestialBody.GetUnityPosition(Divider);
	}
}
