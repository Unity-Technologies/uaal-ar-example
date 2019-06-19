using UnityEngine;
using UnityEngine.EventSystems;

namespace LeTai.Asset.TranslucentImage.Demo
{
    public class ControlCenter : MonoBehaviour
    {
        public RectTransform handle;
        RectTransform        rt;

        void Start()
        {
            rt = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (Mathf.Approximately(handle.rect.height, 0))
                return;
            rt.anchoredPosition = new Vector2(
                rt.anchoredPosition.x,
                Mathf.Clamp(rt.anchoredPosition.y, -rt.rect.height / 2 + handle.rect.height, rt.rect.height / 2 - 1)
            );
        }

        public void Drag(BaseEventData baseEventData)
        {
            PointerEventData data = (PointerEventData) baseEventData;
            rt.position = new Vector2(rt.position.x, rt.position.y + data.delta.y);
        }
    }
}