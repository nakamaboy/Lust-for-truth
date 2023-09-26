using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI theText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = new Color32(122, 221, 245, 255); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = new Color32(81, 88, 236, 255); 
    }
}
