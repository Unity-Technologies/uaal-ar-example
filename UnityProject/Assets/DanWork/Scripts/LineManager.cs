using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField] LineRenderer m_LineRenderer;

    [SerializeField] Transform m_FloorObject;
    [SerializeField] Transform m_ObjectRoot;

    Vector3[] m_Positions;

    void OnEnable()
    {
        m_Positions = new[] { m_FloorObject.localPosition, m_ObjectRoot.localPosition };
    }

    public void SetPositions(Vector3 floorPos)
    {
        m_FloorObject.transform.position = floorPos;
        
        m_Positions[0] = m_FloorObject.position;
        m_Positions[1] = m_ObjectRoot.position;
        
        m_LineRenderer.SetPositions(m_Positions);
    }
}
