using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.CoreUtils;

public class StationGenerating : MonoBehaviour
{
    Camera mainCamera;

    // defining the place for the stations
    private (float x, float y) centre = (0f, 0f); // later -> centre of the list
    private float sorroundings;
    public float scaling = 0.6f;
    public float distanceFromOthers = 2f;
    public int defaultWeeksForStations = 3;

    // scaling of the station
    private float scaleX = 0.025f;
    private float scaleY = 0.045f;

    // values for border
    private float borderXOld = 37f;
    private float borderYOld = 19f;

    // values for border
    private float borderX = 0.45f;
    private float borderY = 0.45f;

    public string[] stationsNames = new string[7]{ "circle", "square", "triangle", "hexagon", "rectangular", "pentagon", "star"};
    public List<string> alreadySpawnedStationsTypes = new List<string>();

    // Start is called before the first frame update
    private void Start()                        
    {                                           
        mainCamera = Camera.main;               
        sorroundings = mainCamera.orthographicSize * scaling;
    }
    // Update is called once per frame
    void Update()
    {
        sorroundings = mainCamera.orthographicSize * scaling;
    }
    /// <summary>
    /// Generates a new station object based on the current week and adds it to the stations list.
    /// </summary>
    /// <param name="stations">A dictionary containing all available station types.</param>
    /// <param name="stationsList">The parent transform of the stations list.</param>
    /// <param name="currentWeek">The current week of the game.</param>
    /// <param name="stationsQueues">A list of stations with their passengers queues.</param>
    public void GenerateStation(Dictionary<string, Transform> stations, Transform stationsList, int currentWeek, List<Station> stationsQueues)
    {
        string name = GetStationName(stationsNames, currentWeek);
        if (!alreadySpawnedStationsTypes.Contains(name))
        {
            alreadySpawnedStationsTypes.Add(name);
        }
        // position = position + stationsList.GetComponent<Canvas>().transform.position;
        GameObject newStation = Instantiate(stations[name].gameObject, Vector3.zero, Quaternion.identity, stationsList);
        newStation.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        Vector3 position = GetRandomPosition();
        position = CheckValidPosition(position, stationsList);
        newStation.transform.localPosition = position;
        newStation.GetComponent<Station>().name = name;
        stationsQueues.Add(newStation.GetComponent<Station>());
    }
    /// <summary>
    /// Generates a random position within the specified surroundings around the centre point.
    /// </summary>
    /// <returns>A random position.</returns>
    Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(0 - borderX * scaling, 0 + borderX * scaling);
        float randomY = Random.Range(0 - borderY * scaling, 0 + borderY * scaling);

        return new Vector3(randomX, randomY, 0f);
    }
    /// <summary>
    /// Retrieves a random station name based on the current week.
    /// </summary>
    /// <param name="stations">An array of station names.</param>
    /// <param name="currentWeek">The current week of the game.</param>
    /// <returns>A randomly selected station name.</returns>
    private string GetStationName(string[] stations, int currentWeek)
    {
        int index;
        if (currentWeek < defaultWeeksForStations)
        {
            index = UnityEngine.Random.Range(0, defaultWeeksForStations);
        }
        else
        {
            index = UnityEngine.Random.Range(0, stations.Length);
        }
        return stations[index];
    }
    /// <summary>
    /// Checks if the given position is valid by performing a sphere overlap check with other colliders.
    /// If the position is invalid (colliders overlap), a new random position is generated and checked again.
    /// </summary>
    /// <param name="position">The position to check for validity.</param>
    /// <returns>A valid position that does not overlap with other colliders.</returns>
    private Vector3 CheckValidPosition(Vector3 position, Transform stationList)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distanceFromOthers);
        if (colliders.Length > 0)
        {
            position = GetRandomPosition();
            position = CheckValidPosition(position, stationList);
        }
        return position;
    }
}