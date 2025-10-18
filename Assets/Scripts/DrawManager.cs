using System.Collections;
using System.Linq;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    #region Input hook
    // TODO - hook to InputManager when Andrei fucking does it, lazy ass piece of work
    private InputSystem_Actions inputSystem = new();
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

        // pe inputSystem.Player.IsDrawing.started trebuie gestionata desenarea unui nou obiect
        // pe inputSystem.Player.IsDrawing.canceled trebuie gestionata finalizarea desenarii obiectului
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
        currentLine = null;
    }
}
