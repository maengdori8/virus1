using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StoryLIne : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int cutCounter = 0;
    [SerializeField] UnityEvent onEnd;

    void Start()
    {
        Reset();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        ShowNext();
    }

    void ShowNext()
    {
        if (cutCounter < transform.childCount)
        {
            transform.GetChild(cutCounter).gameObject.SetActive(true);
            cutCounter++;
        }
        else
        {
            onEnd?.Invoke();
        }
    }

    void Reset()
    {
        cutCounter = 0;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void EventTest()
    {
        Debug.Log("dddd");
    }
}
