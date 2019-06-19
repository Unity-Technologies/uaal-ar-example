using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage.Demo
{
    [RequireComponent(typeof(Text))]
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        int         m_FpsAccumulator;
        float       m_FpsNextPeriod;
        int         m_CurrentFps;
        string      display = "{0} FPS";
        Text        m_Text;


        void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            m_Text          = GetComponent<Text>();
            display         = m_Text.text;
        }


        void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps     =  (int) (m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator =  0;
                m_FpsNextPeriod  += fpsMeasurePeriod;
                m_Text.text      =  string.Format(display, m_CurrentFps);
            }
        }
    }
}