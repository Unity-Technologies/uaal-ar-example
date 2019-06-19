using UnityEngine;

namespace LeTai.Asset.TranslucentImage.Demo
{
    public class ChangeParams : MonoBehaviour
    {
        TranslucentImageSource    source;
        public TranslucentImage[] translucentImages;

        // Use this for initialization
        void Awake()
        {
            source = GetComponent<TranslucentImageSource>();
        }

        public void ChangeBlurSize(float value)
        {
            source.Size = value;
        }

        public void ChangeIteration(float value)
        {
            source.Iteration = Mathf.RoundToInt(value);
        }

        public void ChangeDownsample(float value)
        {
            source.Downsample = Mathf.RoundToInt(value);
        }

        public void ChangeVibrancy(float value)
        {
            for (int i = 0; i < translucentImages.Length; i++)
            {
                translucentImages[i].vibrancy = value;
            }
        }
    }
}