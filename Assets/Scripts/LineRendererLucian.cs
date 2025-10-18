using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Rendering.LookDev;

public class LineRendererLucian : MonoBehaviour
{

    private LineRenderer line;
    private LineRenderer line_uncommited;
    private Vector3 PreviousPosition;

    private InputSystem_Actions inputSystem;
    private Vector2 drawPos;

    [SerializeField]
    private float MinDistance = 0.1f;

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

        };
        inputSystem.Player.IsDrawing.canceled += Context =>
        {

        };

        // pe inputSystem.Player.IsDrawing.started trebuie gestionata desenarea unui nou obiect
        // pe inputSystem.Player.IsDrawing.canceled trebuie gestionata finalizarea desenarii obiectului

    }

    void Start()
    {
        line = GetComponent<LineRenderer>();
        PreviousPosition = transform.position;
        line_uncommited.endColor = Color.aliceBlue;
        line_uncommited.startColor = Color.aliceBlue;

    }

    void Update()
    {
        if(inputSystem.Player.IsDrawing.IsPressed())
        {
            Vector3 CurrentPosition = Camera.main.ScreenToWorldPoint(drawPos);
            CurrentPosition.z = 0f;

            

            if (Vector3.Distance(CurrentPosition, PreviousPosition) > MinDistance)
            {
                line_uncommited.positionCount++;
                line_uncommited.SetPosition(line_uncommited.positionCount - 1, CurrentPosition);
                PreviousPosition = CurrentPosition;
            }               
        }
    }

}

















