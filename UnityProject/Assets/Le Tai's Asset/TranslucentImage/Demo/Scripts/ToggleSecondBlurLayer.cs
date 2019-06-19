using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage.Demo
{
    public class ToggleSecondBlurLayer : MonoBehaviour
    {
        public ChangeMaxUpdateRate changer;

        public Slider updateRateInput;

        public void Toggle()
        {
            if (Mathf.Approximately(changer.GetUpdateRate(), 0))
                changer.SetUpdateRate(updateRateInput.value);
            else
                changer.SetUpdateRate(0);
        }
    }
}