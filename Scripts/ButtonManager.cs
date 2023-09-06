using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public int buttonName;
    private TimePlanning planner;
    public GameObject myButton;
    private UnityEngine.Color myColor;
    private LinesDrawer drawer;

    void Awake()
    {
        drawer = LinesDrawer.singletone;
        buttonName = Int32.Parse(gameObject.name.ToString());
    }
    // Start is called before the first frame update
    void Start()
    {
        drawer = LinesDrawer.singletone;
        planner = TimePlanning.singletone;
        ColorUtility.TryParseHtmlString("#00FFD6", out myColor);

    }
    // Update is called once per frame
    void Update()
    {
        if (planner.currentWeek < buttonName)
        {
            myButton.GetComponent<Button>().interactable = false;
            myButton.GetComponentInChildren<TextMeshProUGUI>().color = UnityEngine.Color.black;
        }
        else
        {
            myButton.GetComponent<Button>().interactable = true;
            myButton.GetComponentInChildren<TextMeshProUGUI>().color = myColor;
        }
    }
    
    public void ButtonClicked()
    {
        // Debug.Log("Button "+ buttonName +" clicked!"); for debugging
        drawer.selectedLine = buttonName;
        
    }
}
