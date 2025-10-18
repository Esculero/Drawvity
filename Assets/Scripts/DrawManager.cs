using System.Collections;
using System.Linq;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    #region Input hook
    private InputManager inputManager;
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

    #region Game State hooks

    bool IsActive = true;

    void OnGamePaused()
    {
        IsActive = false;
    }

    void OnGameStarted()
    {
        IsActive = true;
    }
    #endregion

    void OnEnable()
    {
        // hook into GameManager events


        // input events
        inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();

        inputManager.OnDrawPositionChanged += position => drawPos = position;
        inputManager.OnDrawStarted += () => CreateLine();
        inputManager.OnDrawEnded += () => FinishDrawingLine();
        inputManager.OnInkTypeSelected += index =>
        {
            SelectedInk = index;
            FinishDrawingLine();
        }; 
    }

    void Update()
    {
        if (!IsActive) return;
        
        if (currentLine != null)
        {
            Debug.Log(InkPercentage[SelectedInk]);
            Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
            CurrentPosition.z = 0f;

            float distance = Vector3.Distance(CurrentPosition, PreviousPosition);
            if (distance > MinDistance && InkPercentage[SelectedInk] > 0)
            {
                currentLine.positionCount++;
                currentLine.SetPosition(currentLine.positionCount - 1, CurrentPosition);
                PreviousPosition = CurrentPosition;
                InkPercentage[SelectedInk] -= PaintingCost * distance * Time.deltaTime;

            }
        }
    }

    void CreateLine()
    {
        if (!IsActive) return;
        
        Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
        CurrentPosition.z = 0f;
        PreviousPosition = CurrentPosition;

        // TODO - create prefab for Line that contains a controller script and the needed components (LineRenderer, Collider, etc)
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
        if(!IsActive) return;
        currentLine = null;
    }
}
