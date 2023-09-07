using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Net;
using System.Threading;

public class Line : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    // public Rigidbody2D rigidBody2D;
    public Train myTrain;
    public Vector3[] trainPoints;
    public Transform myTrainsTransform;
    public List<GameObject> myTrainsList = new List<GameObject>();

    [HideInInspector] public List<Vector2> points = new List<Vector2>();
    [HideInInspector] public int pointsCount = 0;
    public LinesDrawer lineDrawer;
    public StationGenerating generator;
    public TimePlanning planner;

    GameObject newTrain;
    int vertices;
    private int countsOfAllPoints = 0;

    // scaling of position, needs more testingand better values
    public float moveXCoordinates = -48f;
    public float moveYCoordinates = -28f;

    public float scaleXCoordinates = 0.014f;
    public float scaleYCoordiantes = 0.018f;

    // minimum distance between line's points.
    float pointsMinimalDistance = 0.1f;

    // circle collider added to each line's point
    float circleColliderRadius;

    private void Awake()
    {
        lineDrawer = LinesDrawer.singletone;
        generator = StationGenerating.singletone;
        planner = TimePlanning.singletone;
    }
    // Start is called before the first frame update
    private void Start()
    {
        myTrainsTransform = lineDrawer.allTrainsTransform;
        GetPositionsForTrain();
        GenerateTrain(lineDrawer.trainModel.gameObject, myTrainsTransform);
        myTrain = newTrain.GetComponent<Train>();
        myTrain.GetComponent<Train>().linePoints = trainPoints;
    }
    // Update is called once per frame
    private void Update()
    {
        if (countsOfAllPoints < this.GetComponent<LineRenderer>().positionCount)
        {
            GetPositionsForTrain();
            countsOfAllPoints = this.GetComponent<LineRenderer>().positionCount;
            vertices = this.GetComponent<LineRenderer>().GetPositions(trainPoints);
            myTrain.GetComponent<Train>().linePoints = trainPoints;
            myTrain.GetComponent<Train>().start = true;
            myTrain.GetComponent<Train>().maxPoint = trainPoints.Length;
        }
        /*if (trainPoints.Length < 2)
            {
                Destroy(newTrain);
            }
        */
    }
    /// <summary>
    /// Retrieves positions from the LineRenderer and stores them in the trainPoints list.
    /// </summary>
    public void GetPositionsForTrain()
    {
        trainPoints = new Vector3[this.GetComponent<LineRenderer>().positionCount];
        vertices = this.GetComponent<LineRenderer>().GetPositions(trainPoints);
    }
    /// <summary>
    /// Generates a new train object and adds it to the list of trains.
    /// </summary>
    /// <param name="trainModel">The train prefab to instantiate.</param>
    /// <param name="trains">The parent transform of the train objects.</param>
    public void GenerateTrain(GameObject trainModel, Transform trains)
    {
        newTrain = Instantiate(trainModel, Vector3.zero, Quaternion.identity, trains);
        newTrain.transform.localScale = new Vector3(generator.scaleX, generator.scaleY, 0f);
        newTrain.AddComponent<Train>();
        Vector3 position = trainPoints[0];
        // The start position of a train
        position = Vector3.zero + position;
        position = ScalePosition(position);
        newTrain.transform.localPosition = position;
        // Set the name of the new train based on the line's name and the current count of trains
        newTrain.name = this.name.ToString().Remove(5) + "->" + myTrainsTransform.childCount.ToString(); 
        // remove(5) because of pattern LineX, where X is from 1 to 6
        myTrainsList.Add(newTrain);
    }
    /// <summary>
    /// Scales a given position using specified scaling factors and offsets.
    /// </summary>
    /// <param name="position">The position to be scaled.</param>
    /// <returns>The scaled position.</returns>
    public Vector3 ScalePosition(Vector3 position)
    {
        float x = (position.x + moveXCoordinates) * scaleXCoordinates;
        float y = (position.y + moveYCoordinates) * scaleYCoordiantes;
        return new Vector3(x,  y,  position.z);
    }
    /// <summary>
    /// Adds a new point to the drawn line if the distance between the last point and 
    /// the new point is greater than pointsMinimalDistance.
    /// </summary>
    /// <param name="newPoint">The position of the new point to add to the line.</param>
    public void AddPoint(Vector2 newPoint)
    {
        // if distance between last point and new point is less than pointsMinimalDistance do nothing (return)
        if (pointsCount >= 1 && Vector2.Distance(newPoint, GetLastPoint()) < pointsMinimalDistance)
            return;

        points.Add(newPoint);
        pointsCount++;

        CircleCollider2D circleCollider = this.gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = newPoint;
        circleCollider.radius = circleColliderRadius;

        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPosition(pointsCount - 1, newPoint);

        // edge colliders accept only 2 points or more (we can't create an edge with one point :D )
        if (pointsCount > 1)
            edgeCollider.points = points.ToArray();
    }

    /// <summary>
    /// Returns the position of the last point added to the drawn line.
    /// </summary>
    /// <returns>The position of the last point as a Vector2.</returns>
    public Vector2 GetLastPoint()
    {
        return (Vector2)lineRenderer.GetPosition(pointsCount - 1);
    }

    /// <summary>
    /// Sets the color gradient of the line renderer.
    /// </summary>
    /// <param name="colorGradient">The gradient to set as the line's color.</param>
    public void SetLineColor(Gradient colorGradient)
    {
        lineRenderer.colorGradient = colorGradient;
    }

    /// <summary>
    /// Sets the minimum distance between points on the drawn line.
    /// </summary>
    /// <param name="distance">The minimum distance between points.</param>
    public void SetPointsMinDistance(float distance)
    {
        pointsMinimalDistance = distance;
    }
    /*/// <summary>
    /// Enable or disable physics simulation for the GameObject.
    /// </summary>
    /// <param name="usePhysics">True to enable physics, false to disable physics.</param>
    public void UsePhysics(bool usePhysics)
    {
        // isKinematic = true  means that this rigidbody is not affected by Unity's physics engine
        rigidBody2D.isKinematic = !usePhysics; // doesn't work
    }*/
    /// <summary>
    /// Sets the width of the drawn line.
    /// </summary>
    /// <param name="width">The width of the line.</param>
    public void SetLineWidth(float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        circleColliderRadius = width / 2f;

        edgeCollider.edgeRadius = circleColliderRadius;
    }

}