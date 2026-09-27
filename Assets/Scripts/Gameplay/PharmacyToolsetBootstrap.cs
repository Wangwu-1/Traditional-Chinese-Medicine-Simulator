using UnityEngine;
using UnityEngine.SceneManagement;

namespace TcmSimulator.Gameplay
{
    /// <summary>Ensures the production toolset is present in the configured game scene.</summary>
    public static class PharmacyToolsetBootstrap
    {
        private const string PharmacyTableName = "P_PROP_table_interior_03";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateToolset()
        {
            if (SceneManager.GetActiveScene().path != "Assets/game.unity" || Object.FindObjectOfType<PharmacyToolset>() != null)
                return;

            var toolset = new GameObject("制药工具组 / Pharmacy Tools");
            var pharmacyTable = GameObject.Find(PharmacyTableName);
            // The table pivot is at floor level; its authored collider height is 0.89 units.
            toolset.transform.position = pharmacyTable != null
                ? pharmacyTable.transform.position + Vector3.up * 0.90f
                : new Vector3(2.4f, 1.16f, 4.55f);
            toolset.AddComponent<PharmacyToolset>();
        }
    }
}
