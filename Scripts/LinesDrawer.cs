using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Linq;
using System.Collections.Generic;

public class LinesDrawer : MonoBehaviour
{

    public GameObject linePrefab;
    public List<GameObject> linePrefabs;
    public int cantDrawOverLayerIndex;
    public int selectedLine;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance;
    public float lineWidth;
    Line currentLine;

    Camera myCamera;

    public List<GameObject> linesTypes;
    public static LinesDrawer singletone;

    public GameObject trainModel;
    public Transform allTrainsTransform;

    void Awake()
    {
        singletone = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
        cantDrawOverLayerIndex = 5; // UI layer for gameinfo
        selectedLine = 0;
        linePrefab = linePrefabs[selectedLine];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            BeginDraw();

        if (currentLine != null)
            Draw();

        if (Input.GetMouseButtonUp(0))
            EndDraw();
        CheckPressedButtons();
    }
    /// <summary>
    /// Initiates the drawing process by creating a new line based on the provided prefab.
    /// Sets the line's color, minimum distance between points, and width.
    /// </summary>
    void BeginDraw()
    {
        currentLine = Instantiate(linePrefabs[selectedLine], this.transform).GetComponent<Line>();

        // currentLine.UsePhysics(false);
        currentLine.SetLineColor(lineColor);
        currentLine.SetPointsMinDistance(linePointsMinDistance);
        currentLine.SetLineWidth(lineWidth);

    }
    /// <summary>
    /// Continuously draws a line based on the mouse position.
    /// Checks if the mouse position intersects with a collider on the "CantDrawOver" layer.
    /// If there is an intersection, it ends the current drawing; otherwise, it adds a new point to the line.
    /// </summary>
    void Draw()
    {
        Vector2 mousePosition = myCamera.ScreenToWorldPoint(Input.mousePosition);

        // check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cantDrawOverLayerIndex);

        if (hit)
            EndDraw();
        else
            currentLine.AddPoint(mousePosition);
    }
    /// <summary>
    /// Ends the drawing process, checking the current line's point count.
    /// If the line has only one point, it gets destroyed; otherwise, drawing is concluded.
    /// </summary>
    void EndDraw()
    {
        if (currentLine != null)
        {
            if (currentLine.pointsCount < 2)
            {
                // if line has one point
                Destroy(currentLine.gameObject);
            }
            else
            {
                currentLine = null;
            }
        }

    }
    /// <summary>
    /// Checks and can change the colour of the drawed line. Input is from buttons.
    /// </summary>
    public void CheckPressedButtons()
    {
        linePrefab = linePrefabs[selectedLine];
    }
}
