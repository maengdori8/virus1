using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject target;


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        target.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        target.SetActive(false);
    }

}
