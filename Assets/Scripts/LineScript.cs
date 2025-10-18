using UnityEngine;

public class LineScript : MonoBehaviour
{
    [SerializeField]
    private Color lineColor;
    [SerializeField]
    private double InkAmount;

    [SerializeField, Range(0.1f, 0.2f)] 
    private float Width = 0.2f;


    private LineRenderer lineRenderer;


    public void CreateLine(Vector3 position)
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = lineColor;
        lineRenderer.startColor = lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = Width;
        lineRenderer.endWidth = Width;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, position);
    }

    public void AddPoint(Vector3 point)
    {

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

    }

    public void SetInkAmount(double ink)
    {
        InkAmount = ink;
    }

    public double GetInk()
    {
        return InkAmount;
    }

    public void DeleteLine()
    {

        Destroy(this.gameObject);
    }

    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}