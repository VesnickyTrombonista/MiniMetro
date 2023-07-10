using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.CoreUtils;

public class TimePlanning : MonoBehaviour
{
    public Slider slider;

    public Transform stationsGeneratedList;
    private StationGenerating stationGenerator;
    public Transform stationsTransform;
    private Dictionary<string, Transform> stations = new Dictionary<string, Transform>(7);
    List<string> alredySpawnedStationsTypes = new List<string>();
    List<Station> stationsQueues = new List<Station>();

    private PeopleGenerating peopleGenerator;
    public Transform peopleTransform;
    private Dictionary<string, Transform> people = new Dictionary<string, Transform>(7);

    // for camera scrolling
    private bool scrollerIsUpdated = false;
    public float weekDuration = 60f;
    private float currentTimeInWeek;
    private int currentWeek = 0;
    private int firstFastPeriod = 5;

    // slider & generating
    private float defaultSliderPercentage = 0.01f;
    private float sliderPercentageForSpawningStation;
    private float sliderValueForGeneratingNextStation;
    private float sliderPercentageForSpawningPerson;
    private float sliderValueForGeneratingNextPerson;

    // speed of people generating
    // floats because of dividing
    private float daysInWeek = 7; 
    private float peoplePerDay = 4;

    // camera 
    public float defaultCameraSize = 2f;
    private float currentScrollAmount;
    public float scrollAmountFast = 0.5f;
    public float scrollAmountSlow = 0.1f;
    public float scrollingSmoothness = 0.25f;

    public Camera mainCamera;
    private float cameraSize;
    private float newCameraSize;

    public TextMeshProUGUI totalPassengersCount;

    // Start is called before the first frame update
    void Start()
    {
        stationGenerator = new StationGenerating();
        peopleGenerator = new PeopleGenerating();
        SetSpawningProperty();
        SetDefaultTimer();
        SetCameraDefaultProperty();
        GetStationsIntoDictionary();
        GetPeopleIntoDictionary();
        totalPassengersCount.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        // totalPassengersCount.text = AddTransportedPassenger(totalPassengersCount.text, stationsQueues);
        if (slider.value == slider.maxValue) { // new week
            currentWeek++;
            SetDefaultTimer();

            SetCameraSize();
        }
        if (slider.value >= sliderValueForGeneratingNextPerson)
        {
            if (stationsGeneratedList.childCount > 0)
            {
                // in future: async?
                alredySpawnedStationsTypes = stationGenerator.alreadySpawnedStationsTypes;
                for (int i = 0; i < stationsGeneratedList.childCount; i++)
                {
                    Transform station = stationsGeneratedList.GetChild(i);
                    peopleGenerator.GeneratePerson(people, station, station.GetChild(0), alredySpawnedStationsTypes); 
                    // station.GetChild(0) = peopleQueue
                }
            }

            sliderValueForGeneratingNextPerson += sliderPercentageForSpawningPerson;
            
        }
        if (slider.value >= sliderValueForGeneratingNextStation)
        {

            stationGenerator.GenerateStation(stations, stationsGeneratedList, currentWeek, stationsQueues);

            sliderValueForGeneratingNextStation += sliderPercentageForSpawningStation;

        }
        if (scrollerIsUpdated)
        {
            if (newCameraSize - cameraSize > 2 * currentScrollAmount) // scrolling is slow
            {
                ChangeCameraSizeContinuosly(0.25f * scrollingSmoothness);
            }
            else
            {
                ChangeCameraSizeContinuosly(scrollingSmoothness);
            }
            
        }
        if (cameraSize == newCameraSize)
        {
            scrollerIsUpdated = false;
        }
    }
    /// <summary>
    /// Updates the timer and slider over time.
    /// </summary>
    private void UpdateTimer()
    {
        currentTimeInWeek += Time.deltaTime;
        slider.value = currentTimeInWeek / weekDuration;
    }
    /// <summary>
    /// Sets the timer to its default values.
    /// </summary>
    private void SetDefaultTimer()
    {
        slider.value = defaultSliderPercentage;
        currentTimeInWeek = 0f;
        sliderValueForGeneratingNextPerson = 0;
        sliderValueForGeneratingNextStation = 0;
        if (currentWeek < firstFastPeriod)
        {
            currentScrollAmount = scrollAmountFast;
        }
        else
        {
            currentScrollAmount = scrollAmountSlow;
        }
    }
    /// <summary>
    /// Sets the spawning time-properties for the people and the stations.
    /// </summary>
    private void SetSpawningProperty()
    {
        sliderPercentageForSpawningPerson = 1 / (daysInWeek * peoplePerDay);
        sliderPercentageForSpawningStation = 1 / daysInWeek;
    }
    /// <summary>
    /// Sets the default properties for the camera.
    /// </summary>
    private void SetCameraDefaultProperty()
    {
        mainCamera = Camera.main;
        mainCamera.orthographicSize = defaultCameraSize;
        cameraSize = mainCamera.orthographicSize;
        newCameraSize = cameraSize;
    }
    /// <summary>
    /// Sets the camera size based on the current week.
    /// </summary>
    private void SetCameraSize()
    {
        scrollerIsUpdated = true;
        if (currentWeek < firstFastPeriod)
        {
            newCameraSize = mainCamera.orthographicSize + scrollAmountFast;
        }
        else
        {
            newCameraSize = mainCamera.orthographicSize + scrollAmountSlow;
        }
        newCameraSize = Mathf.Clamp(newCameraSize, 3f, 16f);
    }

    /// <summary>
    /// Changes the camera size continuously to smoothly transition to the new camera size.
    /// </summary>
    /// <param name="smoothness">The smoothness factor controlling the speed of the transition.</param>
    private void ChangeCameraSizeContinuosly(float smoothness)
    {
        cameraSize = mainCamera.orthographicSize;
        mainCamera.orthographicSize = Mathf.Lerp(cameraSize, newCameraSize, Time.deltaTime * smoothness);
    }
    /// <summary>
    /// Populates the 'stations' dictionary with station objects from the 'stationsTransform' parent object.
    /// </summary>
    private void GetStationsIntoDictionary()
    {
        stations.Add("square", stationsTransform.GetChild(0));
        stations.Add("circle", stationsTransform.GetChild(1));
        stations.Add("hexagon", stationsTransform.GetChild(2));
        stations.Add("rectangular", stationsTransform.GetChild(3));
        stations.Add("pentagon", stationsTransform.GetChild(4));
        stations.Add("star", stationsTransform.GetChild(5));
        stations.Add("triangle", stationsTransform.GetChild(6));
    }
    /// <summary>
    /// Populates the 'people' dictionary with people objects from the 'peopleTransform' parent object.
    /// </summary>
    private void GetPeopleIntoDictionary()
    {
        people.Add("star", peopleTransform.GetChild(0));
        people.Add("hexagon", peopleTransform.GetChild(1));
        people.Add("pentagon", peopleTransform.GetChild(2));
        people.Add("triangle", peopleTransform.GetChild(3));
        people.Add("rectangular", peopleTransform.GetChild(4));
        people.Add("square", peopleTransform.GetChild(5));
        people.Add("circle", peopleTransform.GetChild(6));
    }
    public string AddTransportedPassenger(string count, List<Station> stationsList)
    {
        int newCount;
        string name;
        int peopleCount = 0;
        foreach (Station station in stationsList)
        {
            name = station.name;
            List<GameObject>.Enumerator copy = station.passengerQueue.GetEnumerator();
            GameObject person;
            while ((person = copy.Current) is not null)
            {
                copy.MoveNext();
                if (person.name == name)
                {
                    peopleCount += 1;
                    station.passengerQueue.Remove(person); // that concrete passenger
                    Destroy(person);
                }
            }
        }
        newCount = Int32.Parse(count) + peopleCount;
        return newCount.ToString();
    }
}
