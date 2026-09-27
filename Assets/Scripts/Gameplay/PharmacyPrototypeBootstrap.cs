using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace TcmSimulator.Gameplay
{
    /// <summary>
    /// Week-one playable blockout. It deliberately creates only primitive geometry at runtime,
    /// so replacing it with the interior-pack art later does not alter interaction logic.
    /// </summary>
    public sealed class PharmacyPrototypeBootstrap : MonoBehaviour
    {
        private Material wood;
        private Material paper;

        private void Awake()
        {
            CreateMaterials();
            BuildRoom();
            BuildCounter();
            BuildHerbShelf();
            BuildPreparationTablePlaceholder();
            BuildSigns();
        }

        private void CreateMaterials()
        {
            wood = CreateMaterial(new Color(0.30f, 0.12f, 0.045f));
            paper = CreateMaterial(new Color(0.82f, 0.72f, 0.50f));
        }

        private void BuildRoom()
        {
            CreateBlock("Pharmacy Floor", new Vector3(0f, -0.05f, 2.5f), new Vector3(8f, 0.1f, 8f), paper);
            CreateBlock("Back Wall", new Vector3(0f, 1.75f, 6.4f), new Vector3(8f, 3.5f, 0.15f), wood);
            CreateBlock("Left Wall", new Vector3(-3.95f, 1.75f, 2.5f), new Vector3(0.15f, 3.5f, 8f), wood);
            CreateBlock("Right Wall", new Vector3(3.95f, 1.75f, 2.5f), new Vector3(0.15f, 3.5f, 8f), wood);
        }

        private void BuildCounter()
        {
            CreateBlock("Counter", new Vector3(0f, 0.6f, 2.8f), new Vector3(2.8f, 1.2f, 0.65f), wood);
            CreateLabel("柜台  COUNTER", new Vector3(0f, 1.35f, 2.43f), 0.16f);
        }

        private void BuildHerbShelf()
        {
            var shelfPosition = new Vector3(-2.5f, 1.25f, 4.8f);
            CreateBlock("Herb Cabinet", shelfPosition, new Vector3(2.2f, 2.5f, 0.45f), wood);
            CreateLabel("药柜  HERB SHELF", new Vector3(-2.5f, 2.65f, 4.53f), 0.14f);

            CreateShelfSlot(new Vector3(-3.15f, 1.55f, 4.48f), HerbId.DangGui, "Dang Gui", new Color(0.55f, 0.30f, 0.12f));
            CreateShelfSlot(new Vector3(-2.50f, 1.55f, 4.48f), HerbId.JuHua, "Ju Hua", new Color(0.92f, 0.73f, 0.12f));
            CreateShelfSlot(new Vector3(-1.85f, 1.55f, 4.48f), HerbId.GanCao, "Gan Cao", new Color(0.68f, 0.55f, 0.26f));
        }

        private void CreateShelfSlot(Vector3 position, HerbId herbId, string herbName, Color herbColor)
        {
            var slot = CreateBlock($"Slot - {herbName}", position, new Vector3(0.52f, 0.08f, 0.28f), wood);
            var shelfSlot = slot.AddComponent<HerbShelfSlot>();
            shelfSlot.SetStatusRenderer(slot.GetComponent<Renderer>());

            var herb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            herb.transform.SetPositionAndRotation(position + new Vector3(0f, 0.20f, -0.02f), Quaternion.identity);
            herb.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
            herb.GetComponent<Renderer>().material = CreateMaterial(herbColor);
            herb.AddComponent<Rigidbody>().mass = 0.15f;
            herb.AddComponent<XRGrabInteractable>();
            var item = herb.AddComponent<HerbItem>();
            item.Configure(herbId, herbName);
            shelfSlot.Register(item);
            CreateLabel(herbName, position + new Vector3(0f, -0.18f, -0.17f), 0.07f);
        }

        private void BuildPreparationTablePlaceholder()
        {
            CreateBlock("Preparation Table", new Vector3(2.4f, 0.55f, 4.55f), new Vector3(1.8f, 1.1f, 1.1f), wood);
            CreateLabel("制药台  COMING NEXT", new Vector3(2.4f, 1.2f, 3.96f), 0.10f);
        }

        private void BuildSigns()
        {
            CreateLabel("TCM PHARMACY - WEEK 1 PROTOTYPE", new Vector3(0f, 2.8f, 6.28f), 0.16f);
            CreateLabel("Grab an herb from the shelf", new Vector3(0f, 2.48f, 6.28f), 0.10f);
        }

        private GameObject CreateBlock(string objectName, Vector3 position, Vector3 scale, Material material)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = objectName;
            block.transform.position = position;
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().material = material;
            return block;
        }

        private void CreateLabel(string text, Vector3 position, float characterSize)
        {
            var label = new GameObject($"Label - {text}");
            label.transform.SetPositionAndRotation(position, Quaternion.identity);
            var mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.characterSize = characterSize;
            mesh.fontSize = 64;
            mesh.color = Color.white;
        }

        private static Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { color = color };
            return material;
        }
    }
}
