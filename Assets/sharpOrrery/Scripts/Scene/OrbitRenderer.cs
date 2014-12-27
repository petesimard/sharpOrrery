using System.Collections.Generic;
using System.Linq;
using SharpOrrery;
using UnityEngine;

public class OrbitRenderer : MonoBehaviour
{
    private CelestialBody celestial;
    private OrbitLine eclipticLine;
    private OrbitLine ellipticOrbitLine;
    private OrbitLine orbitLine;
    private OrbitLine perturbedOrbitLine;
    public SceneCelestialBody sceneCelestial;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    internal void Init(SceneCelestialBody sceneCelestialBody)
    {
        this.sceneCelestial = sceneCelestialBody;
        celestial = sceneCelestialBody.celestialBody;

        List<Vector3d> orbitVertices = celestial.getOrbitVertices(false);
        if (orbitVertices == null)
            return;
            
        //get orbit line calculated from precise locations instead of assumed ellipse
        if (perturbedOrbitLine == null)
        {
            perturbedOrbitLine = new OrbitLine();
            perturbedOrbitLine.init(this, celestial.name, Color.cyan);
        }
        perturbedOrbitLine.setLine(orbitVertices);

        //get new orbit vertices, but elliptical (not perturbed)
        orbitVertices = celestial.getOrbitVertices(true);

        //does this body revolves around the system's main body? If so, draw its ecliptic
        if (celestial.relativeTo == null || celestial.relativeTo == SO.U.getBody())
        {
            var eclipticVertices = new List<Vector3d>(orbitVertices);

            for (int index = 0; index < eclipticVertices.Count; index++)
            {
                eclipticVertices[index] = eclipticVertices[index]*-1;
            }

            if (eclipticLine == null)
            {
                eclipticLine = new OrbitLine();
                eclipticLine.init(this, celestial.name, Color.magenta);
            }
            eclipticLine.setLine(eclipticVertices);
        }

        if (ellipticOrbitLine == null)
        {
            ellipticOrbitLine = new OrbitLine();
            ellipticOrbitLine.init(this, celestial.name, Color.magenta);
        }
        ellipticOrbitLine.setLine(orbitVertices);

        if (celestial.calculateFromElements)
        {
            celestial.revolution += body => recalculateOrbitLine(false);
        }

        orbitLine = celestial.calculateFromElements ? perturbedOrbitLine : ellipticOrbitLine;
    }

    private void recalculateOrbitLine(bool isForced)
    {
        if (!isForced && (perturbedOrbitLine == null || !celestial.calculateFromElements))
            return;

        List<Vector3d> orbitVertices = celestial.getOrbitVertices(!celestial.calculateFromElements);
        if (orbitVertices != null)
        {
            bool wasAdded = orbitLine.Shown;
            hideOrbit();
            orbitLine.setLine(orbitVertices);
            if (wasAdded)
            {
                showOrbit();
            }
        }
    }

    public void showEcliptic()
    {
        if (eclipticLine == null) return;
        eclipticLine.Shown = true;
    }

    public void hideEcliptic()
    {
        if (eclipticLine == null) return;
        eclipticLine.Shown = false;
    }

    public void showOrbit()
    {
        if (orbitLine == null) return;
        orbitLine.Shown = true;
    }

    public void hideOrbit()
    {
        if (orbitLine == null) return;
        orbitLine.Shown = false;
        //this.getOrbitContainer().remove(this.ellipticOrbitLine.getDisplayObject());
    }

    private class OrbitLine
    {
        private LineRenderer lineRenderer;
        private OrbitRenderer orbitRenderer;
        private bool shown;
        private List<Vector3> vertices;

        public bool Shown
        {
            get { return shown; }
            set
            {
                shown = value;
                lineRenderer.enabled = value;
            }
        }

        public void init(OrbitRenderer orbitRenderer, string name, Color color)
        {
            var renderGameObject = new GameObject(name + " Orbit Renderer");
            renderGameObject.transform.parent = orbitRenderer.sceneCelestial.transform;
            renderGameObject.transform.localPosition = Vector3.zero;

            this.orbitRenderer = orbitRenderer;
            lineRenderer = renderGameObject.AddComponent<LineRenderer>();
            lineRenderer.SetColors(color, color);
            Shown = false;
        }

        internal void setLine(List<Vector3d> orbitVertices)
        {
            vertices = orbitVertices.Select(v =>
            {
                var vec = v.GetVector3(SceneCelestialBody.Divider);
                return new Vector3(vec.x, vec.z, vec.y);
            }).ToList();

            lineRenderer.SetVertexCount(vertices.Count);

            for (int index = 0; index < orbitVertices.Count; index++)
            {
                Vector3 orbitVertex = vertices[index];
                lineRenderer.SetPosition(index, orbitVertex);
            }
        }
    }
}