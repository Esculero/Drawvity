using System.Collections;
using System.Linq;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    #region Input hook
    // TODO - hook to InputManager when Andrei fucking does it, lazy ass piece of work
    private InputSystem_Actions inputSystem;
    private Vector2 drawPos;
    #endregion

    #region Ink control
    
    [SerializeField]
    private double[] InkPercentage = {100.0f, 100.0f, 100.0f };
    private int SelectedInk = 0;
    
    [SerializeField]
    private double PaintingCost = 1000.0f;
    #endregion

    #region Line creation
    private LineRenderer[] lines;
    MeshCollider newMeshCollider;

    private LineRenderer currentLine;
    private Vector3 PreviousPosition;

    [SerializeField]
    private float MinDistance = 0.1f;

    [SerializeField, Range(0.1f, 0.2f)] 
    private float Width = 0.2f;
    

    #endregion

    void OnEnable()
    {
        inputSystem = new();
        inputSystem.Enable();

        inputSystem.Player.Draw.performed +=
            context =>
            {
                drawPos = context.ReadValue<Vector2>();
            }; // linia asta citeste constant pozitia si o stocheaza in drawPos

        inputSystem.Player.IsDrawing.started += Context =>
        {
            CreateLine();
        };

        inputSystem.Player.IsDrawing.canceled += Context =>
        {
            FinishDrawingLine();
        };

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentLine != null)
        {
            Debug.Log(InkPercentage[1]);
            Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
            CurrentPosition.z = 0f;

            float distance =  Vector3.Distance(CurrentPosition, PreviousPosition);
            if (distance > MinDistance && InkPercentage[1] > 0)
            {
                currentLine.positionCount++;
                currentLine.SetPosition(currentLine.positionCount - 1, CurrentPosition);
                PreviousPosition = CurrentPosition;
                InkPercentage[1] -= PaintingCost * distance * Time.deltaTime;
                
            }
        }
    }

    void CreateLine()
    {
        Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
        CurrentPosition.z = 0f;
        PreviousPosition = CurrentPosition;

        GameObject newLine = new GameObject();
        currentLine = newLine.AddComponent<LineRenderer>();
        // newMeshCollider = newLine.AddComponent<MeshCollider>();

        // Mesh mesh = new Mesh();
        // currentLine.BakeMesh(mesh, true);
        // newMeshCollider.sharedMesh = mesh;

        currentLine.positionCount = 1;
        currentLine.startWidth = Width;
        currentLine.endWidth = Width;


        currentLine.SetPosition(0, CurrentPosition);

        lines.Append(currentLine);

        currentLine.transform.SetParent(this.transform);
    }
    
    void FinishDrawingLine()
    {
        currentLine = null;
    }
}
