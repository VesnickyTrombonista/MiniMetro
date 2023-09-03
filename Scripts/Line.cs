using UnityEngine;
using System.Collections.Generic;

public class Line : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    //public Rigidbody2D rigidBody;

    [HideInInspector] public List<Vector2> points = new List<Vector2>();
    [HideInInspector] public int pointsCount = 0;

    // minimum distance between line's points.
    float pointsMinimalDistance = 0.1f;

    // circle collider added to each line's point
    float circleColliderRadius;

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