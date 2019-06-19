using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacementHandler : MonoBehaviour
{
    [SerializeField] LineManager m_LineManager;
    [SerializeField] GameObject m_ShadowObject;
    [SerializeField] Material m_ObjectMat;

    public Material objectMat
    {
        get => m_ObjectMat;
        set => m_ObjectMat = value;
    }

    const float k_MaxRange = 0.75f;
    Vector3 m_ObjectOffset;
    float m_Dist;
    float m_HeightDiff;


    public void SetObject(Vector3 posePosition)
    {
        Camera m_Device = Camera.main;
        
        Debug.Log(Vector3.Distance(m_Device.transform.position, posePosition) + " current device y : "+Vector3.Distance(posePosition, m_Device.transform.position) + " "+m_Device.transform.localPosition.y);

        m_Dist = Vector3.Distance(m_Device.transform.position, posePosition);
        m_HeightDiff = m_Device.transform.position.y - posePosition.y;

        Debug.Log(m_HeightDiff +" HIEGHT DIFF");
        
        if (m_HeightDiff > k_MaxRange)
        {
            m_LineManager.gameObject.SetActive(true);

            //m_Dist = Vector3.Distance(posePosition, m_Device.transform.position);
           
            
            this.transform.position = posePosition + new Vector3(0, m_Dist/2, 0);
            
            m_ShadowObject.SetActive(false);
            m_LineManager.SetPositions(posePosition);
        }
        else
        {
            m_LineManager.gameObject.SetActive(false);
            m_ShadowObject.SetActive(true);
        }
    }
}
