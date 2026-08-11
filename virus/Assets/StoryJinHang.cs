using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryJinHang : MonoBehaviour
{
    [SerializeField] public int sceneCounter = 0;

    void Start()
    {
        Reset();
    }


    public void Reset()
    {
        sceneCounter = 0;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ToNext()
    {
        transform.GetChild(sceneCounter).gameObject.SetActive(false);

        sceneCounter++;

        transform.GetChild(sceneCounter).gameObject.SetActive(true);
    }



}
