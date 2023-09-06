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
using static UnityEngine.GraphicsBuffer;
using System.Threading;
using System.Threading.Tasks;

public class StationGenerating : MonoBehaviour
{
    public static StationGenerating singletone;
    Camera mainCamera;
    public GameObject info;
    public Transform river;

    // defining the place for the stations
    //private (float x, float y) centre = (0f, 0f); // later -> centre of the list
    private float surroundings = 1.05f;
    private float scaleYGenarating = 1f;
    public float scaling = 0.02f;
    private float distanceFromOthers = 1f;
    public int defaultWeeksForStations = 3;
    public float distanceFromRiver = 1f;

    // scaling of the station
    public float scaleX = 0.025f;
    public float scaleY = 0.045f;


    // scaling of camera size
    private float cameraScaleX = 0.057f;
    private float cameraScaleY = 0.025f;

    // sizes of boarder
    private float cameraMaxX = 0.48f;
    private float cameraMaxY = 0.36f;

    // scales for stations
    private float cameraScaleCheckX = 0.3f;
    private float cameraScaleCheckY = 0.2f;

    // values for border
    //private float borderXOld = 37f;
    //private float borderYOld = 19f;

    // values for border
    private float borderX = 0.45f;
    private float borderY = 0.45f;

    private float alpha = 2f; // = 1.3f; later for scaling
    public string[] stationsNames = new string[7] { "circle", "square", "triangle", "hexagon", "rectangular", "pentagon", "star" };
    public List<string> alreadySpawnedStationsTypes = new List<string>();

    // init positions for three stations
    public List<Vector3> initPositions = new List<Vector3>();
    private int initStation = -1; // not used, up to 2
    public List<Vector3> shuffledPositions = new List<Vector3>();

    private float easyWeeks = 4f;

    // Awake is called when the script is initialized and when a Scene loads
    void Awake()
    {
        singletone = this;
    }
    // Start is called before the first frame update
    private void Start()
    { 
        mainCamera = Camera.main;
        surroundings = mainCamera.orthographicSize * scaling;
        Vector3 positionA = new Vector3(0, 0.4f * surroundings, 0);
        initPositions.Add(positionA);
        Vector3 positionB = new Vector3(1.2f * surroundings, -0.1f * surroundings, 0);
        initPositions.Add(positionB);
        Vector3 positionC = new Vector3(-1f * surroundings, -0.4f * surroundings, 0);
        initPositions.Add(positionC);
        shuffledPositions = GetShuffledNumbers(initPositions);
    }

    // Update is called once per frame
    void Update()
    {
        surroundings = mainCamera.orthographicSize * scaling;
        // alpha = (float)mainCamera.orthographicSize * scaling;
        // scaling += 0.0002f; // 0.001f;
    }
    /// <summary>
    /// Generates asynchronously a new station object based on the current week and adds it to the stations list.
    /// </summary>
    /// <param name="stations">A dictionary containing all available station types.</param>
    /// <param name="stationsGeneratedList">The parent transform of the stations list.</param>
    /// <param name="currentWeek">The current week of the game.</param>
    /// <param name="stationsQueues">A list of stations with their passengers queues.</param>
    public async void GenerateStationAsync(Dictionary<string, Transform> stations, Transform stationsGeneratedList, float currentWeek, List<Station> stationsQueues)
    {
        string name = GetStationName(stationsNames, currentWeek);
        if (!alreadySpawnedStationsTypes.Contains(name))
        {
            alreadySpawnedStationsTypes.Add(name);
        }
        GameObject newStation = Instantiate(stations[name].gameObject, Vector3.zero, Quaternion.identity, stationsGeneratedList);
        newStation.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        Vector3 randomVector = GetRandomPosition();
        Vector3 position = await GetPositionInGameAsync(stationsGeneratedList);
        if (info.GetComponent<TimePlanning>().currentWeek > 3*easyWeeks)
        {
            position = GetPositionFartherInGame(position);
        }
        position = MixDownPositionsTogether(position, randomVector);
        position = RandomizePositivityOfXYCoordinates(position);
        newStation.transform.localPosition = Vector3.zero + position;
        // TODO for a tester: better distibution of surroundings positions
        // I need more position to spawn to have them in a circle from a centre
        newStation.GetComponent<Station>().name = name;
        stationsQueues.Add(newStation.GetComponent<Station>());
    }
    /// <summary>
    /// Generates a default station of the specified type and adds it to the list of stations.
    /// </summary>
    /// <param name="name">The name of the station type to be generated.</param>
    /// <param name="stations">A dictionary containing available station types.</param>
    /// <param name="stationsGeneratedList">The parent transform for holding generated station objects.</param>
    /// <param name="currentWeek">The current week of the simulation.</param>
    /// <param name="stationsQueues">A list of station queues to which the new station will be added.</param>
    public void GenerateDefaultStation(string name, Dictionary<string, Transform> stations, Transform stationsGeneratedList, float currentWeek, List<Station> stationsQueues)
    {
        if (!alreadySpawnedStationsTypes.Contains(name))
        {
            alreadySpawnedStationsTypes.Add(name);
        }
        GameObject newStation = Instantiate(stations[name].gameObject, Vector3.zero, Quaternion.identity, stationsGeneratedList);
        newStation.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        Vector3 position = GetInitPosition();
        newStation.transform.localPosition = Vector3.zero + position;
        newStation.GetComponent<Station>().name = name;
        stationsQueues.Add(newStation.GetComponent<Station>());
    }
    /// <summary>
    /// Adjusts the given position by moving it farther away from the center of the game, along with the border checking.
    /// </summary>
    /// <param name="position">The original position to be adjusted.</param>
    /// <returns>The adjusted position, moved farther from the center and checked against the border.</returns>
    private Vector3 GetPositionFartherInGame(Vector3 position)
    {
        float x = GetFloatBitFarFromCentre(position.x);
        float y = GetFloatBitFarFromCentre(position.y);
        position.x = x;
        position.y = scaleYGenarating * y;
        position = CheckBorder(position);
        position = RandomizePositivityOfXYCoordinates(position);
        return position;
    }
    /// <summary>
    /// Retrieves the initial position for a station, incrementing the index to the next shuffled position.
    /// </summary>
    /// <returns>The initial position for the station.</returns>
    public Vector3 GetInitPosition()
    {
        initStation++;
        return shuffledPositions[initStation];
    }
    /// <summary>
    /// Mixes the components of two vectors together, taking the minimum value for each component.
    /// </summary>
    /// <param name="goodVector">The vector with good components.</param>
    /// <param name="badVector">The vector with bad components.</param>
    /// <returns>A new vector with the minimum components from the input vectors.</returns>
    private Vector3 MixDownPositionsTogether(Vector3 goodVector, Vector3 badVector)
    {
        float x = Math.Min(goodVector.x, badVector.x);
        float y = Math.Min(goodVector.y, badVector.y);
        Vector3 mixedVector = new Vector3(x, scaleYGenarating * y, goodVector.z);
        mixedVector = CheckBorder(mixedVector); 
        return mixedVector;
    }
    /// <summary>
    /// Shuffles a list of Vector3 positions using Fisher-Yates algorithm.
    /// </summary>
    /// <param name="positions">The list of Vector3 positions to be shuffled.</param>
    /// <returns>A new list of Vector3 positions shuffled in random order.</returns>
    public List<Vector3> GetShuffledNumbers(List<Vector3> positions)
    {
        List<int> numbers = new List<int> { 0, 1, 2 };
        List<int> shuffledNumbers = new List<int>();

        while (numbers.Count > 1)
        {
            int index = UnityEngine.Random.Range(0, numbers.Count);
            shuffledNumbers.Add(numbers[index]);
            numbers.RemoveAt(index);
        }
        if (numbers.Count == 1)
        {
            shuffledNumbers.Add(numbers[0]);
            numbers.RemoveAt(0);
        }
        List<Vector3> shuffledPositions = new List<Vector3>();
        for (int i = 0; i < shuffledNumbers.Count; i++)
        {
            shuffledPositions.Add(positions[shuffledNumbers[i]]);
        }
        return shuffledPositions;
    }
    /// <summary>
    /// Generates asynchronously a random position within a specified area centered around the camera's view, considering scaling and surroundings.
    /// </summary>
    /// <param name="stationsGeneratedList">The list of stations for checking position validity.</param>
    /// <returns>A random position within the specified area.</returns>
    public async Task<Vector3> GetPositionInGameAsync(Transform stationsGeneratedList)
    {
        float sizeX = cameraScaleX + scaling / 2f;
        float sizeY = cameraScaleY + scaling / 2f;
        //sizeX = GetHigherAbsSize((1 / alpha) * sizeX, (1 / alpha) * mainCamera.orthographicSize * surroundings * sizeX);
        //sizeY = GetHigherAbsSize(alpha * sizeY, alpha * mainCamera.orthographicSize * scaleYGenarating * surroundings * sizeY);

        // // TODO for a tester: to get precise sizes
        sizeX = Math.Max((1 / alpha) * sizeX, (1 / alpha) * mainCamera.orthographicSize * surroundings * sizeX);
        sizeY = Math.Max(alpha * sizeY, alpha * mainCamera.orthographicSize * scaleYGenarating * surroundings * sizeY);

        float weeks = info.GetComponent<TimePlanning>().currentWeek;
        float randomX = UnityEngine.Random.Range(-sizeX - weeks, sizeX + weeks);
        float randomY = UnityEngine.Random.Range(-sizeY - weeks, sizeY + weeks);

        Vector3 position = new Vector3(mainCamera.orthographicSize * randomX, mainCamera.orthographicSize * randomY, 0);
        position = await CheckValidPosition(position, stationsGeneratedList);
        position = CheckStationDistanceFromRiver(position, river, distanceFromRiver);
        position = CheckBorder(position);
        position = RandomizePositivityOfXYCoordinates(position);
        position.x = position.x / 2;
        position.y = position.y / 2;
        return position;
    }
    /// <summary>
    /// Randomizes the positivity of the coordinates of a given Vector3 position.
    /// </summary>
    /// <param name="position">The input position whose coordinates will be randomized.</param>
    /// <returns>A Vector3 with randomized coordinate positivities.</returns>
    public Vector3 RandomizePositivityOfXYCoordinates(Vector3 position)
    {
        List<(int, int)> tuplesOfCoordinates = new List<(int, int)> { (1, 1), (1,-1), (-1,1), (-1,-1) };


        // randomize positivity
        int index = UnityEngine.Random.Range(0, tuplesOfCoordinates.Count);
        (int x, int y) randomPositivity = tuplesOfCoordinates[index];

        position.x = position.x * randomPositivity.x;
        position.y = position.y * randomPositivity.y;

        return position;
    }
    /// <summary>
    /// Checks and adjusts the given Vector3 position to ensure it stays within the defined camera borders.
    /// </summary>
    /// <param name="position">The Vector3 position to be checked and adjusted.</param>
    /// <returns>The adjusted Vector3 position within the specified camera borders.</returns>
    private Vector3 CheckBorder(Vector3 position)
    {
        // Math.Max
        float maxX = Math.Min(cameraMaxX, cameraScaleCheckX * mainCamera.orthographicSize);
        float maxY = Math.Min(cameraMaxY, cameraScaleCheckY * mainCamera.orthographicSize);

        position.x = Mathf.Clamp(position.x, - maxX, maxX);
        position.y = Mathf.Clamp(position.y, - maxY, maxY);

        position.x = Mathf.Clamp(position.x, -cameraMaxX, cameraMaxX);
        position.y = Mathf.Clamp(position.y, -cameraMaxY, cameraMaxY);
        return position;
    }
    /// <summary>
    /// Compares two input float sizes and returns the higher value.
    /// </summary>
    /// <param name="sizeOld">The first size to compare.</param>
    /// <param name="sizeNew">The second size to compare.</param>
    /// <returns>The higher value between the two input sizes.</returns>
    private float GetHigherAbsSize(float sizeOld, float sizeNew)
    {
        if (sizeOld == 0 || sizeNew == 0)
        {
            return 0f;
        }
        if (sizeOld > 0)
        {
            if (sizeOld > sizeNew)
            {
                return sizeOld;
            }
            else
            {
                return sizeNew;
            }
        }
        else
        {
            if (sizeOld < sizeNew)
            {
                return sizeOld;
            }
            else
            {
                return sizeNew;
            }

        }
        
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
            number += surroundings; //* info.GetComponent<TimePlanning>().currentWeek;
        }
        else if (number < 0)
        {
            number -= surroundings; // * info.GetComponent<TimePlanning>().currentWeek;
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
        float randomY = UnityEngine.Random.Range(0 - scaleYGenarating * borderY * scaling, 0 + scaleYGenarating * borderY * scaling);
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
    public async Task<Vector3> CheckValidPosition(Vector3 position, Transform stationsGeneratedList)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distanceFromOthers);
        if (colliders.Length > 0)
        {
            position = await GetPositionInGameAsync(stationsGeneratedList);
            position = CheckStationDistanceFromRiver(position, river, distanceFromRiver);
            position = await FinalCheckValidPositionAsync(position, stationsGeneratedList);
        }

        return position;
    }
    /// <summary>
    /// Performs the final validation of a position asynchronously by checking its distance from other stations' centers.
    /// If the distance is less than the required distance, the position is adjusted further away.
    /// </summary>
    /// <param name="position">The position to be validated.</param>
    /// <param name="stationsGeneratedList">The list of stations to compare distances against.</param>
    /// <returns>A validated position that meets the required distance criteria from other stations.</returns>
    public async Task<Vector3> FinalCheckValidPositionAsync(Vector3 position, Transform stationsGeneratedList)
    {
        for (int i = 0; i < stationsGeneratedList.childCount; i++)
        {
            float distance = Vector3.Distance(position, stationsGeneratedList.GetChild(i).GetComponent<Station>().centre);
            if (distance >= distanceFromOthers)
            {
                continue;
            }
            else
            {
                position = GetPositionFartherInGame(position);
                position = CheckStationDistanceFromRiver(position, river, distanceFromRiver);
                position = await FinalCheckValidPositionAsync(position, stationsGeneratedList);
                break;
            }
        }

        return position;
    }

    /// <summary>
    /// Adjusts the position of a station to maintain a minimum distance from a river.
    /// If the station is too close to the river, its position is adjusted outward.
    /// </summary>
    /// <param name="position">The position of the station to be checked and adjusted.</param>
    /// <param name="river">The Transform of the river object containing an EdgeCollider2D.</param>
    /// <param name="minDistanceFromRiver">The minimum distance required between the station and the river.</param>
    /// <returns>An adjusted position for the station that maintains the specified minimum distance from the river.</returns>
    public Vector3 CheckStationDistanceFromRiver(Vector3 position, Transform river, float minDistanceFromRiver)
    {
        Vector2[] riverPoints = river.GetComponent<EdgeCollider2D>().points;

        foreach (Vector3 riverPoint in riverPoints)
        {
            float distance = Vector2.Distance(position, riverPoint);
            if (distance < minDistanceFromRiver)
            {
                Vector3 riverDirection = (position - riverPoint).normalized;
                position += riverDirection * (minDistanceFromRiver - distance);
            }
        }

        return position;
    }
    

}