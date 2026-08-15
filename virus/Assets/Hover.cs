using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject target;


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (target == null) return;

        target.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (target == null) return;

        target.SetActive(false);
    }

}
