using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace TcmSimulator.Gameplay
{
    /// <summary>Stable identifiers used by prescriptions, stock, and processing later on.</summary>
    public enum HerbId
    {
        DangGui,
        JuHua,
        GanCao,
        JinYinHua,
        ChuanBei,
    }

    /// <summary>Physical preparation stages used by the pharmacy tools.</summary>
    public enum HerbPreparationStage
    {
        Raw,
        Washed,
        Chopped,
        Ground,
    }

    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class HerbItem : MonoBehaviour
    {
        [SerializeField] private HerbId herbId;
        [SerializeField] private string displayName;
        [SerializeField] private HerbPreparationStage preparationStage;

        public HerbId HerbId => herbId;
        public string DisplayName => displayName;
        public HerbPreparationStage PreparationStage => preparationStage;
        public event Action<HerbItem> PickedUp;

        public void Configure(HerbId id, string name)
        {
            herbId = id;
            displayName = name;
            gameObject.name = $"Herb - {name}";
        }

        /// <summary>Advances the herb only when the requested preparation step is valid.</summary>
        public bool TryPrepare(HerbPreparationStage nextStage)
        {
            if ((int)nextStage != (int)preparationStage + 1)
                return false;

            preparationStage = nextStage;
            RefreshPreparationVisual();
            return true;
        }

        private void RefreshPreparationVisual()
        {
            var renderer = GetComponent<Renderer>();
            if (renderer == null)
                return;

            switch (preparationStage)
            {
                case HerbPreparationStage.Washed:
                    renderer.material.color = Color.Lerp(renderer.material.color, new Color(0.55f, 0.72f, 0.42f), 0.45f);
                    break;
                case HerbPreparationStage.Chopped:
                    transform.localScale *= 0.72f;
                    renderer.material.color = new Color(0.47f, 0.66f, 0.24f);
                    break;
                case HerbPreparationStage.Ground:
                    transform.localScale = new Vector3(0.13f, 0.07f, 0.13f);
                    renderer.material.color = new Color(0.68f, 0.52f, 0.24f);
                    break;
            }
        }

        private void OnEnable()
        {
            GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnSelectEntered);
        }

        private void OnDisable()
        {
            GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs _)
        {
            PickedUp?.Invoke(this);
        }
    }
}
