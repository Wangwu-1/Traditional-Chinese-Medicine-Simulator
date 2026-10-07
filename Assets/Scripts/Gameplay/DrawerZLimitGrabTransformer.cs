using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

/// <summary>
/// Keeps an XR-grabbed drawer on its parent's positive Z rail, from closed to open.
/// </summary>
public sealed class DrawerZLimitGrabTransformer : XRBaseGrabTransformer
{
    [SerializeField, Min(0f)]
    [Tooltip("Maximum outward travel from the closed position along parent-local positive Z.")]
    float m_MaxTravel = 3f;

    Vector3 m_InitialLocalPosition;
    Quaternion m_InitialLocalRotation;
    Vector3 m_InitialLocalScale;
    Transform m_CabinetTransform;
    bool m_HasInitialPose;
    System.Func<IXRInteractable, Vector3, DistanceInfo> m_PreviousDistanceOverride;

    protected override RegistrationMode registrationMode => RegistrationMode.None;

    public override void OnLink(XRGrabInteractable grabInteractable)
    {
        base.OnLink(grabInteractable);
        // Imported mesh pivots share the cabinet origin. Measure from collider geometry instead.
        // Bounds also avoid unsupported ClosestPoint calls on the hollow, non-convex drawer mesh.
        if (grabInteractable.getDistanceOverride != GetColliderBoundsDistance)
        {
            m_PreviousDistanceOverride = grabInteractable.getDistanceOverride;
            grabInteractable.getDistanceOverride = GetColliderBoundsDistance;
        }

        if (m_HasInitialPose)
            return;

        m_InitialLocalPosition = grabInteractable.transform.localPosition;
        m_InitialLocalRotation = grabInteractable.transform.localRotation;
        m_InitialLocalScale = grabInteractable.transform.localScale;
        m_CabinetTransform = grabInteractable.transform.parent;
        m_HasInitialPose = true;
    }

    public override void OnUnlink(XRGrabInteractable grabInteractable)
    {
        if (grabInteractable.getDistanceOverride == GetColliderBoundsDistance)
            grabInteractable.getDistanceOverride = m_PreviousDistanceOverride;

        base.OnUnlink(grabInteractable);
    }

    static DistanceInfo GetColliderBoundsDistance(IXRInteractable interactable, Vector3 position)
    {
        var nearest = new DistanceInfo
        {
            point = interactable.transform.position,
            distanceSqr = float.PositiveInfinity,
        };

        foreach (var collider in interactable.colliders)
        {
            if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy)
                continue;

            var point = collider.bounds.ClosestPoint(position);
            var distanceSqr = (point - position).sqrMagnitude;
            if (distanceSqr >= nearest.distanceSqr)
                continue;

            nearest = new DistanceInfo { point = point, distanceSqr = distanceSqr, collider = collider };
        }

        if (float.IsPositiveInfinity(nearest.distanceSqr))
            nearest.distanceSqr = (nearest.point - position).sqrMagnitude;

        return nearest;
    }

    public override void Process(
        XRGrabInteractable grabInteractable,
        XRInteractionUpdateOrder.UpdatePhase updatePhase,
        ref Pose targetPose,
        ref Vector3 localScale)
    {
        // XRI temporarily unparents selected objects, so keep the authored cabinet rail.
        var parent = m_CabinetTransform;
        if (parent == null)
            return;

        var localPosition = parent.InverseTransformPoint(targetPose.position);
        localPosition.x = m_InitialLocalPosition.x;
        localPosition.y = m_InitialLocalPosition.y;
        localPosition.z = Mathf.Clamp(
            localPosition.z,
            m_InitialLocalPosition.z,
            m_InitialLocalPosition.z + m_MaxTravel);

        targetPose.position = parent.TransformPoint(localPosition);
        targetPose.rotation = parent.rotation * m_InitialLocalRotation;
        localScale = m_InitialLocalScale;
    }
}
