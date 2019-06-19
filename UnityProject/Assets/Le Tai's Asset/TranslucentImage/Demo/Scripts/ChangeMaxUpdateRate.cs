using UnityEngine;

namespace LeTai.Asset.TranslucentImage.Demo
{
    public class ChangeMaxUpdateRate : MonoBehaviour
    {
        TranslucentImageSource source;

        void Awake()
        {
            source = GetComponent<TranslucentImageSource>();
        }

        public void SetUpdateRate(float value)
        {
            source.maxUpdateRate = value;
        }

        public float GetUpdateRate()
        {
            return source.maxUpdateRate;
        }

        // Update is called once per frame
        void Update()
        { }
    }
}