using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.CoreUtils;
using UnityEngine.UIElements;

public class StationGenerating : MonoBehaviour
{
    Camera mainCamera;
    public GameObject info;
    // TimePlanning planner; // pøiøadit v unity
    // defining the place for the stations
    //private (float x, float y) centre = (0f, 0f); // later -> centre of the list
    private float sorroundings = 1.05f;
    public float scaling = 0.02f;
    public float distanceFromOthers = 200f;
    public int defaultWeeksForStations = 3;

    // scaling of the station
    private float scaleX = 0.025f;
    private float scaleY = 0.045f;

    private float cameraScaleX = 0.057f;
    private float cameraScaleY = 0.025f;

    // values for border
    //private float borderXOld = 37f;
    //private float borderYOld = 19f;

    // values for border
    private float borderX = 0.45f;
    private float borderY = 0.45f;

    private float alpha; // = 1.3f;
    public string[] stationsNames = new string[7] { "circle", "square", "triangle", "hexagon", "rectangular", "pentagon", "star" };
    public List<string> alreadySpawnedStationsTypes = new List<string>();

    public List<Vector3> initPositions = new List<Vector3>();
    public List<int> randomNumbers = new List<int>();
    private int initStation = 0;

    // Start is called before the first frame update
    private void Start()                        
    {                                           
        mainCamera = Camera.main;
        sorroundings = mainCamera.orthographicSize * scaling;
        Vector3 positionA = new Vector3 (1.5f, 1, 0);
        initPositions.Add(positionA);
        Vector3 positionB = new Vector3(-2.5f, 0.5f, 0);
        initPositions.Add(positionB);
        Vector3 positionC = new Vector3(2f, -1, 0);
        initPositions.Add(positionC);
        randomNumbers = GetShuffledNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        sorroundings = mainCamera.orthographicSize * scaling;
        alpha = (float)mainCamera.orthographicSize * scaling;
        scaling += 0.001f;
    }
    /// <summary>
    /// Generates a new station object based on the current week and adds it to the stations list.
    /// </summary>
    /// <param name="stations">A dictionary containing all available station types.</param>
    /// <param name="stationsList">The parent transform of the stations list.</param>
    /// <param name="currentWeek">The current week of the game.</param>
    /// <param name="stationsQueues">A list of stations with their passengers queues.</param>
    public void GenerateStation(Dictionary<string, Transform> stations, Transform stationsList, float currentWeek, List<Station> stationsQueues)
    {
        string name = GetStationName(stationsNames, currentWeek);
        if (!alreadySpawnedStationsTypes.Contains(name))
        {
            alreadySpawnedStationsTypes.Add(name);
        }
        GameObject newStation = Instantiate(stations[name].gameObject, Vector3.zero, Quaternion.identity, stationsList);
        newStation.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        Vector3 position = GetPositionInCentre(stationsList);
        newStation.transform.localPosition = Vector3.zero + position;
        newStation.GetComponent<Station>().name = name;
        stationsQueues.Add(newStation.GetComponent<Station>());
    }
    public void GenerateDefaultStation(string name, Dictionary<string, Transform> stations, Transform stationsList, float currentWeek, List<Station> stationsQueues)
    {
        if (!alreadySpawnedStationsTypes.Contains(name))
        {
            alreadySpawnedStationsTypes.Add(name);
        }
        GameObject newStation = Instantiate(stations[name].gameObject, Vector3.zero, Quaternion.identity, stationsList);
        newStation.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        Vector3 position = GetInitPosition();
        newStation.transform.localPosition = Vector3.zero + position;
        newStation.GetComponent<Station>().name = name;
        stationsQueues.Add(newStation.GetComponent<Station>());
    }


    public Vector3 GetInitPosition()
    {
        int index = randomNumbers[initStation];
        initStation++;
        return initPositions[index];
    }
    public List<int> GetShuffledNumbers()
    {
        List<int> numbers = new List<int> { 0, 1, 2 };
        List<int> shuffledNumbers = new List<int>();

        while (numbers.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, numbers.Count);
            shuffledNumbers.Add(numbers[index]);
            numbers.RemoveAt(index);
        }

        return numbers;
    }
    /// <summary>
    /// Generates a random position within a specified area centered around the camera's view, considering scaling and surroundings.
    /// </summary>
    /// <param name="stationsList">The list of stations for checking position validity.</param>
    /// <returns>A random position within the specified area.</returns>
    Vector3 GetPositionInCentre(Transform stationsList)
    {
        float sizeX = cameraScaleX + scaling;
        float sizeY = cameraScaleY + scaling;
        sizeX = sorroundings * sizeX;
        sizeY = sorroundings * sizeY;
        // sizeX = GetFloatBitFarFromCentre(sizeX);
        // sizeY = GetFloatBitFarFromCentre(sizeY);
        float randomX = UnityEngine.Random.Range(- sizeX, sizeX);
        float randomY = UnityEngine.Random.Range(- sizeY, sizeY);

        
        Vector3 position = new Vector3(randomX, randomY, 0);
        position = CheckValidPosition(position, stationsList);
        return position;
    }
    /// <summary>
    /// Adjusts the input number based on the current week's scaling factor.
    /// </summary>
    /// <param name="number">The number to be adjusted.</param>
    /// <returns>The adjusted number according to current week's scaling.</returns>
    private float GetFloatBitFarFromCentre(float number)
    {
        if (number > 0)
        {
            number += scaling * info.GetComponent<TimePlanning>().currentWeek;
        }
        else if(number < 0)
        {
            number -= scaling *  info.GetComponent<TimePlanning>().currentWeek;
        }
        return number;
    }
    /// <summary>
    /// Generates a random position within the specified surroundings around the centre point. Use rather GetPositionInCentre
    /// </summary>
    /// <returns>A random position.</returns>
    Vector3 GetRandomPosition()
    {
        float randomX = UnityEngine.Random.Range(0 - borderX * scaling, 0 + borderX * scaling);
        float randomY = UnityEngine.Random.Range(0 - borderY * scaling, 0 + borderY * scaling);
        Vector3 position = new Vector3(randomX, randomY, 0f);
        return position;
    }
    /// <summary>
    /// Retrieves a random station name based on the current week.
    /// </summary>
    /// <param name="stations">An array of station names.</param>
    /// <param name="currentWeek">The current week of the game.</param>
    /// <returns>A randomly selected station name.</returns>
    private string GetStationName(string[] stations, float currentWeek)
    {
        float index;
        if (currentWeek < defaultWeeksForStations)
        {
            index = UnityEngine.Random.Range(0, defaultWeeksForStations);
        }
        else
        {
            index = UnityEngine.Random.Range(0, stations.Length);
        }
        return stations[(int)index];
    }
    /// <summary>
    /// Checks if the given position is valid by performing a sphere overlap check with other colliders.
    /// If the position is invalid (colliders overlap), a new random position is generated and checked again.
    /// </summary>
    /// <param name="position">The position to check for validity.</param>
    /// <returns>A valid position that does not overlap with other colliders.</returns>
    public Vector3 CheckValidPosition(Vector3 position, Transform stationList)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distanceFromOthers);
        if (colliders.Length > 0)
        {
            position = GetPositionInCentre(stationList);
            position = CheckValidPosition(position, stationList);
        }

        return position;
    }
}