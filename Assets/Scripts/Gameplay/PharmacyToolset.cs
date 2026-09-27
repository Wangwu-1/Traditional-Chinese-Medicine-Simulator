using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace TcmSimulator.Gameplay
{
    /// <summary>Builds the playable washing, chopping, and grinding tools for the prototype scene.</summary>
    public sealed class PharmacyToolset : MonoBehaviour
    {
        private Material wood;
        private Material stone;
        private Material metal;
        private Material water;

        private void Awake()
        {
            if (transform.childCount > 0)
                return;

            wood = MakeMaterial(new Color(0.30f, 0.13f, 0.05f));
            stone = MakeMaterial(new Color(0.36f, 0.34f, 0.29f));
            metal = MakeMaterial(new Color(0.52f, 0.55f, 0.57f));
            water = MakeMaterial(new Color(0.15f, 0.50f, 0.72f, 0.72f));

            CreateChoppingStation(transform.position + new Vector3(-0.58f, 0f, 0f));
            CreateMortarStation(transform.position + new Vector3(0f, 0f, 0f));
            CreateWaterBasin(transform.position + new Vector3(0.62f, 0f, 0f));
            CreatePracticeHerbs(transform.position + new Vector3(-0.66f, 0f, -0.34f));
        }

        private void CreateChoppingStation(Vector3 position)
        {
            var board = CreatePrimitive("铡药砧板 / Chopping Board", PrimitiveType.Cube, position + new Vector3(0f, 0.03f, 0f), new Vector3(0.48f, 0.06f, 0.36f), wood);
            var station = board.AddComponent<ChoppingBoard>();
            station.Configure(0.28f);

            var knife = CreatePrimitive("铡药刀 / Herb Chopper", PrimitiveType.Cube, position + new Vector3(0f, 0.20f, 0f), new Vector3(0.06f, 0.07f, 0.40f), metal);
            AddGrabbablePhysics(knife, 0.45f);
            knife.AddComponent<HerbChopper>().Configure(station);
            CreateLabel("洗 → 铡 → 碾", position + new Vector3(0f, 0.38f, 0.20f), 0.055f);
        }

        private void CreateMortarStation(Vector3 position)
        {
            var mortar = CreatePrimitive("碾药臼 / Mortar", PrimitiveType.Cylinder, position + new Vector3(0f, 0.10f, 0f), new Vector3(0.34f, 0.20f, 0.34f), stone);
            AddGrabbablePhysics(mortar, 1.1f);
            var mortarTool = mortar.AddComponent<MortarTool>();
            mortarTool.Configure(0.18f);

            var zone = new GameObject("碾药区 / Grinding Zone");
            zone.transform.SetParent(mortar.transform, false);
            zone.transform.localPosition = new Vector3(0f, 0.13f, 0f);
            var zoneCollider = zone.AddComponent<SphereCollider>();
            zoneCollider.radius = 0.42f;
            zoneCollider.isTrigger = true;
            zone.AddComponent<MortarIngredientZone>().Configure(mortarTool);

            var pestle = CreatePrimitive("碾杵 / Pestle", PrimitiveType.Capsule, position + new Vector3(0.38f, 0.24f, 0f), new Vector3(0.10f, 0.30f, 0.10f), stone);
            AddGrabbablePhysics(pestle, 0.32f);
            pestle.AddComponent<PestleTool>().Configure(mortarTool);
        }

        private void CreateWaterBasin(Vector3 position)
        {
            var basin = CreatePrimitive("水盆 / Water Basin", PrimitiveType.Cylinder, position + new Vector3(0f, 0.08f, 0f), new Vector3(0.36f, 0.16f, 0.36f), stone);
            AddGrabbablePhysics(basin, 0.9f);

            var surface = CreatePrimitive("水面 / Water", PrimitiveType.Cylinder, position + new Vector3(0f, 0.18f, 0f), new Vector3(0.30f, 0.01f, 0.30f), water);
            surface.transform.SetParent(basin.transform, true);
            var surfaceCollider = surface.GetComponent<Collider>();
            surfaceCollider.isTrigger = true;
            surface.AddComponent<WaterBasin>().Configure(1.1f);
        }

        private void CreatePracticeHerbs(Vector3 position)
        {
            CreateLabel("待处理药材", position + new Vector3(0.06f, 0.22f, 0f), 0.045f);
            CreateHerb(position + new Vector3(-0.10f, 0.07f, 0f), HerbId.DangGui, "当归", new Color(0.55f, 0.30f, 0.12f));
            CreateHerb(position + new Vector3(0.06f, 0.07f, 0f), HerbId.JuHua, "菊花", new Color(0.92f, 0.73f, 0.12f));
            CreateHerb(position + new Vector3(0.22f, 0.07f, 0f), HerbId.GanCao, "甘草", new Color(0.68f, 0.55f, 0.26f));
        }

        private void CreateHerb(Vector3 position, HerbId id, string displayName, Color color)
        {
            var herb = CreatePrimitive($"药材 / {displayName}", PrimitiveType.Sphere, position, Vector3.one * 0.10f, MakeMaterial(color));
            AddGrabbablePhysics(herb, 0.05f);
            var item = herb.AddComponent<HerbItem>();
            item.Configure(id, displayName);
        }

        private static void AddGrabbablePhysics(GameObject target, float mass)
        {
            var body = target.AddComponent<Rigidbody>();
            body.mass = mass;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            target.AddComponent<XRGrabInteractable>();
        }

        private GameObject CreatePrimitive(string objectName, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            var target = GameObject.CreatePrimitive(type);
            target.name = objectName;
            target.transform.SetParent(transform, true);
            target.transform.SetPositionAndRotation(position, Quaternion.identity);
            target.transform.localScale = scale;
            target.GetComponent<Renderer>().material = material;
            return target;
        }

        private void CreateLabel(string text, Vector3 position, float size)
        {
            var label = new GameObject(text);
            label.transform.SetParent(transform, true);
            label.transform.SetPositionAndRotation(position, Quaternion.identity);
            var mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.characterSize = size;
            mesh.fontSize = 64;
            mesh.color = Color.white;
        }

        private static Material MakeMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var result = new Material(shader) { color = color };
            if (color.a < 1f)
                result.SetFloat("_Surface", 1f);
            return result;
        }
    }

    public sealed class ChoppingBoard : MonoBehaviour
    {
        private float radius;

        public void Configure(float boardRadius) => radius = boardRadius;

        public void TryChop(Vector3 bladePosition, float speed)
        {
            if (speed < 0.45f)
                return;

            foreach (var collider in Physics.OverlapSphere(transform.position + Vector3.up * 0.12f, radius))
            {
                var herb = collider.GetComponent<HerbItem>();
                if (herb != null && herb.PreparationStage == HerbPreparationStage.Washed && Vector3.Distance(bladePosition, herb.transform.position) < radius)
                    herb.TryPrepare(HerbPreparationStage.Chopped);
            }
        }
    }

    public sealed class HerbChopper : MonoBehaviour
    {
        private ChoppingBoard board;
        private Rigidbody body;

        public void Configure(ChoppingBoard choppingBoard) => board = choppingBoard;

        private void Awake() => body = GetComponent<Rigidbody>();
        private void FixedUpdate()
        {
            if (board != null)
                board.TryChop(transform.position - transform.forward * 0.18f, body.velocity.magnitude);
        }
    }

    public sealed class MortarIngredientZone : MonoBehaviour
    {
        private MortarTool mortar;
        public void Configure(MortarTool mortarTool) => mortar = mortarTool;
        private void OnTriggerEnter(Collider other) => mortar?.AddIngredient(other.GetComponent<HerbItem>());
        private void OnTriggerExit(Collider other) => mortar?.RemoveIngredient(other.GetComponent<HerbItem>());
    }

    public sealed class MortarTool : MonoBehaviour
    {
        private readonly HashSet<HerbItem> ingredients = new HashSet<HerbItem>();
        private float grindProgress;
        private float radius;

        public void Configure(float mortarRadius) => radius = mortarRadius;
        public void AddIngredient(HerbItem herb) { if (herb != null) ingredients.Add(herb); }
        public void RemoveIngredient(HerbItem herb) { if (herb != null) ingredients.Remove(herb); }

        public void Grind(Vector3 pestlePosition, float speed)
        {
            if (speed < 0.25f || Vector3.Distance(pestlePosition, transform.position) > radius + 0.12f)
                return;

            foreach (var herb in ingredients)
            {
                if (herb != null && herb.PreparationStage == HerbPreparationStage.Chopped)
                {
                    grindProgress += Time.fixedDeltaTime * speed;
                    if (grindProgress >= 1.4f)
                    {
                        herb.TryPrepare(HerbPreparationStage.Ground);
                        grindProgress = 0f;
                    }
                    break;
                }
            }
        }
    }

    public sealed class PestleTool : MonoBehaviour
    {
        private MortarTool mortar;
        private Rigidbody body;
        public void Configure(MortarTool mortarTool) => mortar = mortarTool;
        private void Awake() => body = GetComponent<Rigidbody>();
        private void FixedUpdate() => mortar?.Grind(transform.position, body.velocity.magnitude);
    }

    public sealed class WaterBasin : MonoBehaviour
    {
        private float washSeconds;
        private readonly Dictionary<HerbItem, float> immersionTimes = new Dictionary<HerbItem, float>();
        public void Configure(float seconds) => washSeconds = seconds;
        private void OnTriggerStay(Collider other)
        {
            var herb = other.GetComponent<HerbItem>();
            if (herb == null || herb.PreparationStage != HerbPreparationStage.Raw)
                return;

            immersionTimes.TryGetValue(herb, out var time);
            time += Time.fixedDeltaTime;
            immersionTimes[herb] = time;
            if (time >= washSeconds)
            {
                herb.TryPrepare(HerbPreparationStage.Washed);
                immersionTimes.Remove(herb);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var herb = other.GetComponent<HerbItem>();
            if (herb != null)
                immersionTimes.Remove(herb);
        }
    }
}
