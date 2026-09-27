using UnityEngine;

namespace TcmSimulator.Gameplay
{
    /// <summary>Represents a visible shelf slot. Its state is consumed by later stock and prescription systems.</summary>
    public sealed class HerbShelfSlot : MonoBehaviour
    {
        [SerializeField] private bool isOccupied = true;
        [SerializeField] private Renderer statusRenderer;

        public bool IsOccupied => isOccupied;

        public void Register(HerbItem herb)
        {
            herb.PickedUp += OnHerbPickedUp;
            RefreshVisual();
        }

        private void Awake()
        {
            RefreshVisual();
        }

        private void OnHerbPickedUp(HerbItem _)
        {
            isOccupied = false;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (statusRenderer == null)
                return;

            statusRenderer.material.color = isOccupied
                ? new Color(0.74f, 0.47f, 0.18f)
                : new Color(0.19f, 0.12f, 0.07f);
        }

        public void SetStatusRenderer(Renderer renderer)
        {
            statusRenderer = renderer;
            RefreshVisual();
        }
    }
}
