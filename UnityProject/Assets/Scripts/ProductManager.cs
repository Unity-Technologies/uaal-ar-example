﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS
using System.Runtime.InteropServices;

public class NativeAPI {
    [DllImport("__Internal")]
    public static extern void updateUnityShopItem();
    [DllImport("__Internal")]
    public static extern void itemPlacedInAR();
}
#endif

/// <summary>
/// An object that can be previewed in AR.
/// </summary>
[Serializable]
public class ARObject
{
    /// <summary>
    /// The prefab to spawn that represents the object.
    /// </summary>
    public GameObject m_Prefab;
    /// <summary>
    /// The name of this object.
    /// </summary>
    public string m_Name;
}

public class ProductManager : MonoBehaviour
{
    public enum BrandColors
    {
        White,
        Magenta,
        Cyan,
        Lime
    };
    
    private static readonly Color k_White = Color.white;
    private static readonly Color k_Magenta = new Color(0.929f, 0.094f, 0.278f);
    private static readonly Color k_Cyan = new Color(0.019f, 0.733f, 0.827f);
    private static readonly Color k_Lime = new Color(0.796f, 0.858f, 0.164f);

    [SerializeField]
    private ARObject[] m_ArObjects;

    [SerializeField] 
    PlaceSingleObjectOnPlane m_PlaceSingleObjectOnPlane;

    [SerializeField] 
    TMP_Text m_ProductText;

    private ARSession m_ArSession;

    private int m_SelectedIndex;
    ObjectPlacementHandler m_CurrentObject;

    BrandColors m_CurrentColor = BrandColors.White;

    protected void OnEnable()
    {
        m_ArSession = GameObject.Find("AR Session").GetComponent<ARSession>();

        if (m_PlaceSingleObjectOnPlane != null)
        {
            m_PlaceSingleObjectOnPlane.ObjectPlaced += OnObjectedPlaced;

            SetProduct("0");
            SetColor(BrandColors.Magenta.ToString());
            UpdateShopItem();
        }
    }

    protected void OnDisable()
    {
        if (m_PlaceSingleObjectOnPlane != null)
        {
            m_PlaceSingleObjectOnPlane.ObjectPlaced -= OnObjectedPlaced;
        }

        m_ArSession = null;
    }

    public void PauseARSession(string pause)
    {
        //m_ArSession.attemptUpdate = string.Equals(pause, "false");
        m_ArSession.enabled = string.Equals(pause, "false");;
    }

    public void ClearPlacedItem()
    {
        m_PlaceSingleObjectOnPlane.Clear();
    }

    // 0 = white, 1 = magenta, 2 = cyan, 3 = lime
    public void SetColor(string colourName)
    {
        m_CurrentColor = (BrandColors)Enum.Parse(typeof(BrandColors), colourName);

        if (m_CurrentObject != null)
        {
            m_CurrentObject.objectMat.color = GetColor(m_CurrentColor);
        }
    }

    // call first when loading from native
    // 0 = mug, 1 = shirt
    public void SetProduct(string productNumberString)
    {
        if (!int.TryParse(productNumberString, out int productNumber))
        {
            throw new FormatException("ProductNumber was not a valid integer.");
        }
        if (productNumber < 0 || productNumber >= m_ArObjects.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(productNumber));
        }

        m_SelectedIndex = productNumber;
        var selectedObject = m_ArObjects[m_SelectedIndex];

        m_PlaceSingleObjectOnPlane.SetObjectPrefab(selectedObject.m_Prefab);
        m_ProductText.text = selectedObject.m_Name;
    }

    private Color GetColor(BrandColors mCurrentColor)
    {
        switch (mCurrentColor)
        {
            case BrandColors.White:
                return k_White;
            case BrandColors.Magenta:
                return k_Magenta;
            case BrandColors.Cyan:
                return k_Cyan;
            case BrandColors.Lime:
                return k_Lime;
            default:
                throw new ArgumentOutOfRangeException(nameof(mCurrentColor), mCurrentColor, null);
        }
    }
   
    private void OnObjectedPlaced(ObjectPlacementHandler placedObject)
    {
        m_CurrentObject = placedObject;
        
        if (m_CurrentObject != null)
        {
            m_CurrentObject.objectMat.color = GetColor(m_CurrentColor);

#if UNITY_ANDROID
            try
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.company.product.OverrideUnityActivity");
                AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
                overrideActivity.Call("itemPlacedInAR");
            } 
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
#elif UNITY_IOS
            NativeAPI.itemPlacedInAR();
#endif
        }
    }

    private void UpdateShopItem()
    {
#if UNITY_ANDROID
            try
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.company.product.OverrideUnityActivity");
                AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
                overrideActivity.Call("updateUnityShopItem");
            } 
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
#elif UNITY_IOS
            NativeAPI.updateUnityShopItem();
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            UpdateShopItem();
        }
    }
}
