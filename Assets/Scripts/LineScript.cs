using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LineScript : MonoBehaviour
{
    public enum InkType
    { 
        GravityReduction = 0,
        InvertGravity = 1,
        RotateGravity90 = 2
    }

    [SerializeField]
    private InkType inkType;

    [SerializeField]
    private Color lineColor;
    [SerializeField]
    private float InkAmount;    

    [SerializeField, Range(0.1f, 0.2f)] 
    private float Width = 0.2f;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    public GravityModifierDelegate gravityModifierFun
    {
        get
        {
            switch (inkType)
            {
                case InkType.GravityReduction:
                    return (GravityParameters parameters) =>
                    {
                        parameters.gravityScale = Mathf.Max(0.0f, parameters.gravityScale - 0.1f);
                        return parameters;
                    };
                case InkType.InvertGravity:
                    return (GravityParameters parameters) =>
                    {
                        parameters.gravityDirection *= -1;
                        return parameters;
                    };
                case InkType.RotateGravity90:
                default:
                    return (GravityParameters parameters) =>
                    {
                        parameters.gravityDirection = new Vector2(-parameters.gravityDirection.y, parameters.gravityDirection.x);                        
                        return parameters;
                    };
            };
        }
    }


    private ModifierMode gravityModifierMode
    {
        get
        {
            switch (inkType)
            {
                case InkType.GravityReduction:
                    return ModifierMode.Continuous;
                case InkType.InvertGravity:
                case InkType.RotateGravity90:
                default:
                    return ModifierMode.Instantaneous;
            };
        }
    }

    private GravityModifier gravityModifier;

    public void CreateLine(Vector3 position)
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineRenderer.endWidth = Width;        
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, position);
    }

    public void AddPoint(Vector3 point)
    {

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);        
    }

    public void FinishLine(float currentInkAmount)
    {
        InkAmount -= currentInkAmount; // store used ink

        // generate collider points based on line renderer positions
        Vector3[] positions;
        lineRenderer.GetPositions(positions = new Vector3[lineRenderer.positionCount]);

        edgeCollider.points = positions.Select(pos => new Vector2(pos.x, pos.y)).ToArray();
    }

    public void SetInkAmount(float ink)
    {
        InkAmount = ink;
    }

    public float GetInk()
    {
        return InkAmount;
    }

    public InkType GetInkType()
    {
        return inkType;
    }

    public void DeleteLine()
    {
        Destroy(this.gameObject);
    }

    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    void Start()
    {
        if (lineRenderer.positionCount > 2)
            FinishLine(0);
    }

    public GravityModifier GetModifier()
    {
        if(gravityModifier == null)
            gravityModifier = new GravityModifier(gravityModifierFun, gravityModifierMode);

        return gravityModifier;
    }

    public void RemoveGravityModifier()
    {
        gravityModifier = null;
    }
}