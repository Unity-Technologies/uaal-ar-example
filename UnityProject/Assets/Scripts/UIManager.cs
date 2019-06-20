using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    private ARCameraManager m_CameraManager;

    [SerializeField]
    private PlaceSingleObjectOnPlane m_ObjectPlacer;

    [SerializeField]
    private ARPlaneManager m_PlaneManager;

    [SerializeField]
    Animator m_MoveDeviceAnimation;

    [SerializeField]
    Animator m_TapToPlaceAnimation;

    const string k_FadeOffAnim = "FadeOff";
    const string k_FadeOnAnim = "FadeOn";

    bool m_ShowingTapToPlace = false;
    bool m_ShowingMoveDevice = true;

    protected virtual void OnEnable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived += OnFrameChanged;
        }

        if (m_ObjectPlacer != null)
        {
            m_ObjectPlacer.ObjectPlaced += OnObjectPlaced;
        }
    }

    protected virtual void OnDisable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived -= OnFrameChanged;
        }

        if (m_ObjectPlacer != null)
        {
            m_ObjectPlacer.ObjectPlaced -= OnObjectPlaced;
        }
    }

    private void OnFrameChanged(ARCameraFrameEventArgs frameEventArgs)
    {
        if (PlanesFound() && m_ShowingMoveDevice)
        {
            if (m_MoveDeviceAnimation)
            {
                m_MoveDeviceAnimation.SetTrigger(k_FadeOffAnim);
            }

            if (m_TapToPlaceAnimation)
            {
                m_TapToPlaceAnimation.SetTrigger(k_FadeOnAnim);
            }

            m_ShowingTapToPlace = true;
            m_ShowingMoveDevice = false;
        }
    }

    private bool PlanesFound()
    {
        if (m_PlaneManager == null)
        {
            return false;
        }

        return m_PlaneManager.trackables.count > 0;
    }

    private void OnObjectPlaced(ObjectPlacementHandler placedObject)
    {
        if (placedObject != null)
        {
            if (m_ShowingTapToPlace)
            {
                if (m_TapToPlaceAnimation)
                {
                    m_TapToPlaceAnimation.SetTrigger(k_FadeOffAnim);
                }

                m_ShowingTapToPlace = false;
            }
        }
        else
        {
            // Show tap to place again
            if (!m_ShowingTapToPlace)
            {
                if (m_TapToPlaceAnimation)
                {
                    m_TapToPlaceAnimation.SetTrigger(k_FadeOnAnim);
                }

                m_ShowingTapToPlace = true;
            }

            if (m_ShowingMoveDevice)
            {
                if (m_MoveDeviceAnimation)
                {
                    m_MoveDeviceAnimation.SetTrigger(k_FadeOffAnim);
                }

                m_ShowingMoveDevice = false;
            }
        }
    }
}
