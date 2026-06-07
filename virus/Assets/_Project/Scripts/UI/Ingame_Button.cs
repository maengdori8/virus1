using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ingame_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    Image ButtonImg;
    [SerializeField()] private string SceneName;
    void Start()
    {
        ButtonImg = this.gameObject.GetComponent<Image>();
    }

    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ButtonImg.color = new Color32(126, 126, 126, 255);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SceneManager.LoadScene(SceneName);
        ButtonImg.color = new Color32(255, 255, 255, 255);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ButtonImg.color = new Color32(255, 255, 255, 255);
    }
}
