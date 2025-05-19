using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewButtonManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> closedButtons = new(); // Top-down ordered buttons
    [SerializeField] private List<GameObject> alternateViews = new();
    [SerializeField] private GameObject closeButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setAlternateView(int viewtype)
    {
        // 0 = original view
        if (viewtype == 0)
        {
            Debug.Log("setting viewtype to 0");
            foreach (var button in closedButtons)
            {
                button.SetActive(true);
            }
            foreach (var altView in alternateViews)
            {
                altView.SetActive(false);
            }
            closeButton.SetActive(false);
            return;
        }
        // 1 = house, 2 = fairy, 3 = decor
        alternateViews[viewtype-1].SetActive(true);
        Debug.Log("setting active " + viewtype);
        closeButton.SetActive(true);
        // Set the initial states to inactive
        foreach (var button in closedButtons)
        {
            button.SetActive(false);
        }
    }
}
