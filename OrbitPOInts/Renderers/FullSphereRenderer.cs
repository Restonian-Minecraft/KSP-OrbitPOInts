using OrbitPOInts.Extensions.Unity;

#if TEST
using KSPMock;
using UnityEngineMock;
using System.Linq;
using KSP_MapView = KSPMock.MapView;
using KSP_HighLogic = KSPMock.HighLogic;
using KSP_GameScenes = KSPMock.GameScenes;
#else
using UniLinq;
using UnityEngine;
using KSP_MapView = MapView;
using KSP_HighLogic = HighLogic;
using KSP_GameScenes = GameScenes;
#endif
using OrbitPOInts.Extensions.Unity;

namespace OrbitPOInts
{
    using HighLogic = KSP_HighLogic;
    using GameScenes = KSP_GameScenes;

    [RequireComponent(typeof(MeshRenderer))]
    public class FullSphereRenderer : MonoBehaviour, IRenderer
    {
        public float radius { get; set; } = 1.0f;
        public Color color { get; set; } = Color.green;
        public float lineWidth { get; set; } = 0.1f; // unused - here to match the other renderers
        private GameObject sphereObject;
        public bool IsDying { get; private set; }

        private void Awake()
        {
            enabled = false;

            if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                gameObject.layer = 10;
            }
            else
            {
                gameObject.layer = 24;
            }
        }

        private Material GetMaterial()
        {
            return new Material(Shader.Find("Legacy Shaders/Transparent/Specular"));
        }

        void Start()
        {
            sphereObject = new GameObject(NameKey);
            var sphere = sphereObject.AddComponent<MeshRenderer>();
            sphere.material = GetMaterial();
            sphere.material.color = color;
            sphere.receiveShadows = false;

            sphereObject.transform.SetParent(transform);
            sphereObject.transform.localPosition = Vector3.zero;
            sphereObject.transform.localScale *= 2.0f * radius; // scale up the unit-diameter sphere mesh
            
            var meshFilter = sphereObject.AddComponent<MeshFilter>();
            // Create a temporary sphere primitive to steal its mesh
            var spherePrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            meshFilter.sharedMesh = spherePrimitive.GetComponent<MeshFilter>().sharedMesh;
            Destroy(spherePrimitive);

            if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                gameObject.layer = 10;
                sphereObject.gameObject.layer = 10;
            }
            else
            {
                gameObject.layer = 24;
                sphereObject.gameObject.layer = 24;
            }
        }

        private void OnDestroy()
        {
            IsDying = true;
            DestroyImmediate(sphereObject);
        }

        public void SetEnabled(bool state)
        {
            enabled = state;
            if (!sphereObject.IsAlive()) return;
            sphereObject.SetActive(state);
        }

        public void SetColor(Color color)
        {
            this.color = color;
            if (!sphereObject.IsAlive()) return;
            sphereObject.GetComponent<MeshRenderer>().material.color = color;
        }

        public void SetWidth(float width)
        {
            lineWidth = width;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public static string NameKey => "FullSphereMesh";

        public override bool Equals(object obj)
        {
            if (obj is FullSphereRenderer other)
            {
                return GetInstanceID() == other.GetInstanceID();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetInstanceID();
        }
    }
}
