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
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    /// <summary>
    /// Invoked whenever an object is placed in on a plane.
    /// </summary>
    public static event Action onPlacedObject;

    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    [SerializeField] ProductManager m_ProductManager;
    
    ObjectPlacementHandler m_ObjectPlacementHandler;

    [SerializeField] GameObject m_UIPanelRoot;

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
            if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = s_Hits[0].pose;

                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    // content face the user
                    Vector3 lookVector = Camera.main.transform.position - spawnedObject.transform.position;
                    spawnedObject.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                    spawnedObject.transform.rotation = new Quaternion(0, spawnedObject.transform.rotation.y, 0, spawnedObject.transform.rotation.w) * Quaternion.Euler(0,180,0);
                    
                    
                    m_ObjectPlacementHandler = spawnedObject.GetComponent<ObjectPlacementHandler>();

                    m_ProductManager.CurrentObject = m_ObjectPlacementHandler;
                    m_ObjectPlacementHandler.SetObject(hitPose.position);
                    m_UIPanelRoot.SetActive(true);
                    
                }
                else
                {
                    spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                    // content face the user
                    Vector3 lookVector = Camera.main.transform.position - spawnedObject.transform.position;
                    spawnedObject.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                    spawnedObject.transform.rotation = new Quaternion(0, spawnedObject.transform.rotation.y, 0, spawnedObject.transform.rotation.w) * Quaternion.Euler(0,180,0);
                    spawnedObject.GetComponent<ObjectPlacementHandler>().SetObject(hitPose.position);
                }
                
                if (onPlacedObject != null)
                {
                    onPlacedObject();
                }
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
