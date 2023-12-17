using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(LayoutElement))]
public class TabBarItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title2;

    VerticalLayoutGroup layoutGroup;
    LayoutElement layoutElement;

    LayoutElement textLayoutElement;
    private void Awake()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        textLayoutElement = title2.GetComponent<LayoutElement>();
        layoutElement = GetComponent<LayoutElement>();
        if (textLayoutElement == null)
            textLayoutElement = title2.gameObject.AddComponent<LayoutElement>();
        if (icon.GetComponent<LayoutElement>())
            icon.gameObject.AddComponent<LayoutElement>();
    }

    public void ApplyEffect(FATabBarEffect effect)
    {
        layoutElement.flexibleWidth = effect.FlexibleWidth;
        layoutGroup.padding = new RectOffset(0, 0, effect.PaddingTop, 0);
        textLayoutElement.preferredHeight = effect.TitlePreferedHeight;
        textLayoutElement.flexibleHeight = effect.TitleFlexibleHeight;
        Color c = title2.color;
        c.a = effect.TitleAlpha;
        title2.color = c;

    }
}

public class FATabBarEffect
{
    public float FlexibleWidth { get; set; }
    public int PaddingTop { get; set; }
    public float TitlePreferedHeight { get; set; }
    public float TitleFlexibleHeight { get; set; }
    public float TitleAlpha { get; set; }
}
