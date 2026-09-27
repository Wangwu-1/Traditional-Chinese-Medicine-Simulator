using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

/// <summary>
/// Keeps an XR-grabbed drawer on its local Z rail and prevents it from rotating.
/// </summary>
public sealed class DrawerZLimitGrabTransformer : XRBaseGrabTransformer
{
    [SerializeField, Min(0f)]
    [Tooltip("Maximum travel from the drawer's authored local Z position, in Unity units.")]
    float m_MaxTravel = 3f;

    Vector3 m_InitialLocalPosition;
    Quaternion m_InitialLocalRotation;

    protected override RegistrationMode registrationMode => RegistrationMode.None;

    public override void OnLink(XRGrabInteractable grabInteractable)
    {
        base.OnLink(grabInteractable);
        m_InitialLocalPosition = grabInteractable.transform.localPosition;
        m_InitialLocalRotation = grabInteractable.transform.localRotation;
    }

    public override void Process(
        XRGrabInteractable grabInteractable,
        XRInteractionUpdateOrder.UpdatePhase updatePhase,
        ref Pose targetPose,
        ref Vector3 localScale)
    {
        var drawerTransform = grabInteractable.transform;
        var parent = drawerTransform.parent;
        if (parent == null)
            return;

        var localPosition = parent.InverseTransformPoint(targetPose.position);
        localPosition.x = m_InitialLocalPosition.x;
        localPosition.y = m_InitialLocalPosition.y;
        localPosition.z = Mathf.Clamp(
            localPosition.z,
            m_InitialLocalPosition.z - m_MaxTravel,
            m_InitialLocalPosition.z + m_MaxTravel);

        targetPose.position = parent.TransformPoint(localPosition);
        targetPose.rotation = parent.rotation * m_InitialLocalRotation;
        localScale = drawerTransform.localScale;
    }
}
