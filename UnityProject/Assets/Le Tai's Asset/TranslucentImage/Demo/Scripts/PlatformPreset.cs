using System;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage.Demo
{
    public class PlatformPreset : MonoBehaviour
    {
        public Preset[] presets;

        // Use this for initialization
        void Start()
        {
            Slider sizeSlider         = GameObject.Find("Size Slider").GetComponent<Slider>();
            Slider iterationSlider    = GameObject.Find("Iteration Slider").GetComponent<Slider>();
            Slider downsampleSlider   = GameObject.Find("Downsample Slider").GetComponent<Slider>();
            Slider maxUpdateRateField = GameObject.Find("Max update rate Slider").GetComponent<Slider>();

            foreach (Preset preset in presets)
            {
                if (preset.platform == Application.platform)
                {
                    sizeSlider.value         = preset.size;
                    iterationSlider.value    = preset.iteration;
                    downsampleSlider.value   = preset.downsample;
                    maxUpdateRateField.value = preset.maxUpdateRate;
                }
            }
        }
    }

    [Serializable]
    public struct Preset
    {
        public RuntimePlatform platform;
        public float           size;
        public int             iteration;
        public int             downsample;
        public float           maxUpdateRate;
    }
}