using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public enum Products { Mug, Shirt, Sticker};
    public Products SelectedProduct;
    
    [SerializeField] PlaceSingleObjectOnPlane m_PlaceSingleObjectOnPlane;

    [SerializeField] GameObject m_ARMug;
    [SerializeField] GameObject m_ARShirt;
    [SerializeField] GameObject m_ARSticker;

    const string k_MugName = "Unity Mug";
    const string k_ShirtName = "Unisex Logo Tee";
    const string k_StickerName = "Unity Logo Sticker";

    ObjectPlacementHandler m_CurrentObject;

    public ObjectPlacementHandler CurrentObject
    {
        get { return m_CurrentObject; }
        set { m_CurrentObject = value; }
    }
    
    ColorRef.BrandColors m_CurrentColor = ColorRef.BrandColors.white;

    [SerializeField] Transform m_ColorSelection;
    [SerializeField] Transform[] m_ColorButtons;
    [SerializeField] TMP_Text m_ProductText;

    // 0 = white, 1 = magenta, 2 = cyan, 3 = lime
    public void SetColor(int colorValue)
    {
        m_CurrentColor = (ColorRef.BrandColors)colorValue;

        switch (m_CurrentColor)
        {
            case ColorRef.BrandColors.white:
                m_CurrentObject.objectMat.color = Color.white;
                m_ColorSelection.position = m_ColorButtons[0].position;
                break;
            
            case ColorRef.BrandColors.magenta:
                m_CurrentObject.objectMat.color = ColorRef.s_ColorRef.magenta;
                m_ColorSelection.position = m_ColorButtons[1].position;
                break;
            
            case ColorRef.BrandColors.cyan:
                m_CurrentObject.objectMat.color = ColorRef.s_ColorRef.cyan;
                m_ColorSelection.position = m_ColorButtons[2].position;
                break;
            
            case ColorRef.BrandColors.lime:
                m_CurrentObject.objectMat.color = ColorRef.s_ColorRef.lime;
                m_ColorSelection.position = m_ColorButtons[3].position;
                break;
        }
    }
    
    // call first when loading from native
    // 0 = mug, 1 = shirt, 2 = sticker
    public void SetProduct(int productEnum)
    {
        SelectedProduct = (Products)productEnum;

        switch (SelectedProduct)
        {
            case Products.Mug:
                m_PlaceSingleObjectOnPlane.placedPrefab = m_ARMug;
                m_ProductText.text = k_MugName;
                break;
            
            case Products.Shirt:
                m_PlaceSingleObjectOnPlane.placedPrefab = m_ARShirt;
                m_ProductText.text = k_ShirtName;
                break;
            
            case Products.Sticker:
                m_PlaceSingleObjectOnPlane.placedPrefab = m_ARSticker;
                m_ProductText.text = k_StickerName;
                break;
        }
    }

    public void PurchaseMade()
    {
        // go back to native app, passing back m_CurrentColor
    }

    public void ExitAR()
    {
        // exiting out of AR
    }
}
