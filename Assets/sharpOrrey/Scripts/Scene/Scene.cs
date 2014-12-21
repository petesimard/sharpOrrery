using System;
using UnityEngine;
using System.Collections;

public class Scene : MonoBehaviour
{
    public Universe universe;
    public GameObject prefab;

	// Use this for initialization
	void Awake () {
	    universe.OnCelestialBodyAdded += OnCelestialBodyAdded;
	}

    private void OnCelestialBodyAdded(CelestialBody body)
    {
        var bodyRenderer = GameObject.Instantiate(prefab) as GameObject;
        var sceneBody = bodyRenderer.GetComponent<SceneCelestialBody>();
        sceneBody.Init(body);
    }

    // Update is called once per frame
	void Update () {
	
	}
}
