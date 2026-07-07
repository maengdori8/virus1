using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ingame_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    Image buttonImg;
    [SerializeField()] private string sceneName;
    [SerializeField()] Color32 baseColor;
    [SerializeField()] Color32 pointColor;
    [SerializeField()] Color32 clickColor;
    void Start()
    {
        buttonImg = this.gameObject.GetComponent<Image>();
        buttonImg.color = baseColor;
    }

    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonImg.color = clickColor;
        SceneManager.LoadScene(sceneName);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonImg.color = baseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImg.color = pointColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImg.color = baseColor;
    }
}
