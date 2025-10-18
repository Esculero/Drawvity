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
    private double[] InkPercentage = { 100f, 100f, 100f };

    private int SelectedInk = 0;
    #endregion

    #region Line creation
    private LineRenderer[] lines;

    private LineRenderer currentLine;
    private Vector3 PreviousPosition;

    [SerializeField]
    private float MinDistance = 0.1f;
    [SerializeField,Range(0.1f,0.2f)] private float Width = 0.1f;


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
    }

    void Update()
    {
        if(!IsActive) return;
        if (currentLine != null)
        {
            Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
            CurrentPosition.z = 0f;


            if (Vector3.Distance(CurrentPosition, PreviousPosition) > MinDistance)
            {
                currentLine.positionCount++;
                currentLine.SetPosition(currentLine.positionCount - 1, CurrentPosition);
                PreviousPosition = CurrentPosition;
            }

            
        }
    }

    void CreateLine()
    {
        if(!IsActive) return;
        // TODO - create prefab for Line that contains a controller script and the needed components (LineRenderer, Collider, etc)
        GameObject newLine = new GameObject();
        currentLine = newLine.AddComponent<LineRenderer>();

        Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
        CurrentPosition.z = 0f;

        currentLine.positionCount = 1;
        currentLine.startWidth = Width;
        currentLine.endWidth = Width;
            
        currentLine.SetPosition(0, CurrentPosition);

        lines.Append(currentLine);

        GameObject.Instantiate(newLine, this.transform);
    }
    
    void FinishDrawingLine()
    {
        if(!IsActive) return;
        currentLine = null;
    }
}
