using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using static UnityEngine.Rendering.CoreUtils;

public class TimePlanning : MonoBehaviour
{
    public static TimePlanning singletone;
    public Slider slider;
    public Transform stationsGeneratedList;
    public StationGenerating stationGenerator;
    public Transform stationsTransform;
    public Dictionary<string, Transform> stations = new Dictionary<string, Transform>(7);
    public List<string> alredySpawnedStationsTypes = new List<string>();
    public List<Station> stationsQueues = new List<Station>();

    public PeopleGenerating peopleGenerator;
    public Transform peopleTransform;
    private Dictionary<string, Transform> people = new Dictionary<string, Transform>(7);

    // for camera scrolling
    private bool scrollerIsUpdated = false;
    private float weekDuration = 84f; //84f
    private float currentTimeInWeek;
    public float currentWeek = 0;
    private int firstFastPeriod = 5;

    // slider & generating
    private float defaultSliderPercentage = 0.01f;
    private float sliderPercentageForSpawningStation;
    private float sliderValueForGeneratingNextStation;
    private float defaultBreakForSpawningStation = 3f;
    private float sliderPercentageForSpawningPerson;
    private float sliderValueForGeneratingNextPerson;

    // speed of people generating
    // floats because of dividing
    private float daysInWeek = 7;
    private float peoplePerDay = 3;

    // camera sizes
    public float defaultCameraSize = 2f;
    private float currentScrollAmount;
    public float scrollAmountFast = 0.3f;
    public float scrollAmountSlow;
    public float scrollingSmoothness = 0.25f;
    private float differenceOfScrolling = 3;

    // camera borders
    public Camera mainCamera;
    private float cameraSize;
    private float newCameraSize;
    private float cameraSizeMin;
    private float cameraSizeMax;

    // people transported
    public TextMeshProUGUI totalPassengersCount;

    // checking peopleLimit
    public float timeLeft;
    private float enoughTime = 20f;
    private int maximalCapacity = 20;
    private bool criticalMode = false;
    public GameObject countdownCount;

    // Awake is called when the script is initialized and when a Scene loads
    void Awake()
    {
        singletone = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        stationGenerator = this.GetComponent<StationGenerating>();
        peopleGenerator = this.GetComponent<PeopleGenerating>();
        SetSpawningProperty();
        SetDefaultTimer();
        SetCameraDefaultProperty();
        GetStationsIntoDictionary();
        GetPeopleIntoDictionary();
        GenerateThreeInitStations();
        SetCameraSize();
    }
    /// <summary>
    /// Generates three initial stations of different types to create a sense of transporting 
    /// people right from the beginning.
    /// </summary>
    public void GenerateThreeInitStations()
    {
        stationGenerator.GenerateDefaultStation("circle", stations, stationsGeneratedList, currentWeek, stationsQueues);
        stationGenerator.GenerateDefaultStation("square",stations, stationsGeneratedList, currentWeek, stationsQueues);
        stationGenerator.GenerateDefaultStation("triangle", stations, stationsGeneratedList, currentWeek, stationsQueues);
        sliderValueForGeneratingNextStation = defaultBreakForSpawningStation * sliderPercentageForSpawningStation;
    }
    // Update is called once per frame, main loop of these
    void Update()
    {
        UpdateTimer();
        CheckQueues();
        if (criticalMode)
        {
            GameOverCountdown();
        }
        
        // totalPassengersCount.text = AddTransportedPassenger(totalPassengersCount.text, stationsQueues);
        if (slider.value == slider.maxValue)
        { // new week
            currentWeek++;
            SetDefaultTimer();

            
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
            SetCameraSize();

            stationGenerator.GenerateStationAsync(stations, stationsGeneratedList, currentWeek, stationsQueues);

            sliderValueForGeneratingNextStation += sliderPercentageForSpawningStation;

        }
        if (scrollerIsUpdated)
        {
            if (newCameraSize - cameraSize > 2 * currentScrollAmount) // scrolling is slow
            {
                ChangeCameraSizeContinuosly(0.25f * scrollingSmoothness); // 0.25f
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
        cameraSizeMin = 0.5f; //3f, too fast
        cameraSizeMax = 16f;
        scrollAmountSlow = scrollAmountFast / differenceOfScrolling;
        totalPassengersCount.text = "0";
        timeLeft = enoughTime;
        countdownCount.GetComponent<TextMeshProUGUI>().text = timeLeft.ToString();
        
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
        newCameraSize = Mathf.Clamp(newCameraSize, cameraSizeMin, cameraSizeMax);
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
    /// <summary>
    /// Increases the count of transported passengers from a station's queue to the given count and returns the updated count.
    /// </summary>
    /// <param name="count">The current count of transported passengers.</param>
    /// <param name="stationsList">A list of stations to check for passengers.</param>
    /// <returns>The updated count of transported passengers after adding passengers from the station queues.</returns>
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
    /// <summary>
    /// Checks the passenger queues of generated stations to determine if any queue has exceeded the maximal capacity.
    /// If a queue exceeds the capacity, it triggers a critical mode that may activate the countdown and other actions.
    /// </summary>
    public void CheckQueues()
    {
        for (int i = 0; i < stationsGeneratedList.childCount; i++)
        {
            if (stationsGeneratedList.GetChild(i).GetComponent<Station>().passengerQueue.Count > maximalCapacity)
            {
                if (criticalMode)
                {
                    continue;
                }
                criticalMode = true;
                countdownCount.SetActive(true); // visible
                break;
            }
            else
            {
                continue;
            }
        }
        if (!criticalMode)
        {
            timeLeft = enoughTime; // restore time for critical part
            countdownCount.SetActive(false); // not visible
        }
    }
    /// <summary>
    /// Manages the countdown for the game over condition.
    /// Decreases the time left by the time passed since the last frame when is in critical secction, 
    /// updates the countdown display, and triggers the game over condition when time runs out.
    /// </summary>
    public void GameOverCountdown()
    {
        timeLeft -= Time.deltaTime;
        countdownCount.GetComponent<TextMeshProUGUI>().text = Mathf.Round(timeLeft).ToString();
        if (timeLeft < 0)
        {
            // GameOver(); show endGame window
            return;
        }
    }
}