using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class QuickButton : MonoBehaviour, IPointerClickHandler
{
    [Header("0-SceneMove; 1-ObjectOpen; 2-ObjectClose;")]
    [SerializeField] int typeNo;
    [SerializeField] string targetSceneName;
    [SerializeField] GameObject targetObject;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        switch (typeNo) {
            case 0:
                SceneManager.LoadScene(targetSceneName);
                break;
            case 1:
                targetObject.SetActive(true);
                break;
            case 2:
                targetObject.SetActive(false);
                break;

        }

    }


}
