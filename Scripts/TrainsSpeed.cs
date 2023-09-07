using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrainsSpeed : MonoBehaviour
{
    private TimePlanning planner;
    public GameObject myButton;
    private UnityEngine.Color myColor;

    public float speed;
    public float amountOfUpgradeSpeed;
    private float week;
    private float upgradesCount;
    private float currentUpgradesCount;
    void Awake()
    {
        speed = Train.speed; // 0.5f
    }
    // Start is called before the first frame update
    void Start()
    {
        planner = TimePlanning.singletone;
        ColorUtility.TryParseHtmlString("#00FFD6", out myColor);
        week = 0;
        amountOfUpgradeSpeed = 0.02f;
        upgradesCount = 0;
        currentUpgradesCount = 0;
        // add available clicks
        string availableClicks = (upgradesCount - currentUpgradesCount).ToString();
        string text = myButton.GetComponentInChildren<TextMeshProUGUI>().text + " " + availableClicks;
        myButton.GetComponentInChildren<TextMeshProUGUI>().text = text;

    }
    // Update is called once per frame
    void Update()
    {
        if (planner.currentWeek > week)
        {
            upgradesCount++;
            week++;
            // add available clicks
            string availableClicks = (upgradesCount - currentUpgradesCount).ToString();
            string text = myButton.GetComponentInChildren<TextMeshProUGUI>().text.Remove(5) + " " + availableClicks;
            // speed has 5 letters
            myButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
        if (upgradesCount > currentUpgradesCount) // available, then change color
        {
            myButton.GetComponent<Button>().interactable = true;
            myButton.GetComponentInChildren<TextMeshProUGUI>().color = myColor;
            
        }
        if (upgradesCount == currentUpgradesCount) // not available, change color
        {
            myButton.GetComponent<Button>().interactable = false;
            myButton.GetComponentInChildren<TextMeshProUGUI>().color = UnityEngine.Color.black;
        }
    }

    /// <summary>
    /// Increases the speed of the train and updates the global train speed variable.
    /// </summary>
    public void ButtonClicked()
    {
        speed += amountOfUpgradeSpeed;
        Train.speed = speed;
        currentUpgradesCount++;
    }
}
