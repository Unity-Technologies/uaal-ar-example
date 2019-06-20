using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceSingleObjectOnPlane : MonoBehaviour
{
    [SerializeField] 
    GameObject m_UIPanelRoot;
    
    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab { get; set; }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    /// <summary>
    /// Invoked whenever an object is placed in on a plane.
    /// </summary>
    public event Action<ObjectPlacementHandler> ObjectPlaced;

    private ARRaycastManager m_RaycastManager;

    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }
        
        Touch touch = Input.GetTouch(0);
        if (IsTouchOverUIObject(touch))
        {
            return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            if (m_RaycastManager.Raycast(touch.position, m_Hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = m_Hits[0].pose;

                if (spawnedObject != null)
                {
                    // Destroy existing object
                    Destroy(spawnedObject);
                }
                
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                
                // content face the user
                Vector3 lookVector = Camera.main.transform.position - spawnedObject.transform.position;
                spawnedObject.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                spawnedObject.transform.rotation = new Quaternion(0, spawnedObject.transform.rotation.y, 0, spawnedObject.transform.rotation.w) * Quaternion.Euler(0,180,0);

                ObjectPlacementHandler objectPlacementHandler = spawnedObject.GetComponent<ObjectPlacementHandler>();
                
                objectPlacementHandler.SetObject(hitPose.position);
                m_UIPanelRoot.SetActive(true);

                ObjectPlaced?.Invoke(objectPlacementHandler);
            }
        }
    }
    
    bool IsTouchOverUIObject(Touch touch)
    {   
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
