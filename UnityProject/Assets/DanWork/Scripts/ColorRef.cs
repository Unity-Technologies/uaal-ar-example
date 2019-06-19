using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ColorRef : MonoBehaviour
{

    public static ColorRef s_ColorRef;

    public enum BrandColors
    {
        white,
        magenta,
        cyan,
        lime
    };
    
    [HideInInspector]
    public Color magenta = new Color(0.929f,0.094f,0.278f);
    [HideInInspector]
    public Color cyan = new Color(0.019f, 0.733f, 0.827f);
    [HideInInspector]
    public Color lime = new Color(0.796f, 0.858f, 0.164f);

    void OnEnable()
    {
        if (s_ColorRef == null)
        {
            s_ColorRef = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
