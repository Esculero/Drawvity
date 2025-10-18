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
    private GameObject[] Lines;
    
    [SerializeField]
    private double PaintingCost = 1000.0f;
    #endregion

    #region Line creation
    private LineScript currentLine;
    private Vector3 PreviousPosition;

    [SerializeField]
    private float MinDistance = 0.1f;

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


    void Start()
    {
        // input events
        inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();

        inputManager.OnDrawPositionChanged += position => drawPos = position;
        inputManager.OnDrawStarted += () => CreateLine();
        inputManager.OnDrawEnded += () => FinishDrawingLine();
        inputManager.OnInkTypeSelected += index =>
        {
            FinishDrawingLine();
            SelectedInk = index;
        };

        inputManager.OnErasePressed += () =>
        {
            if (currentLine != null) return; // cannot erase while drawing
            EraseLine();           
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
                currentLine.AddPoint(CurrentPosition);
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

        GameObject newLine = GameObject.Instantiate(Lines[SelectedInk], this.transform);
        currentLine = newLine.GetComponent<LineScript>();
        currentLine.CreateLine(CurrentPosition);
        currentLine.SetInkAmount(InkPercentage[SelectedInk]);
    }
    
    void FinishDrawingLine()
    {
        if (!IsActive) return;

        currentLine?.FinishLine(InkPercentage[SelectedInk]);
        currentLine = null;
    }

    void EraseLine()
    {
        // get all LineScript under the mouse position - LineScript is attached to an object with LineRenderer
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(drawPos);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(worldPos2D, 0.2f, Vector2.zero);

        foreach (var hit in hits)
        {
            var line = hit.collider.gameObject.GetComponent<LineScript>();
            if (line != null)
            {
                InkPercentage[(int)line.GetInkType()] += line.GetInk();
                Destroy(line.gameObject);
            }
        }

    }
}
