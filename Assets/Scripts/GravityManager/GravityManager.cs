using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static LineScript;

public class GravityManager : MonoBehaviour
{
    [SerializeField]
    private GravityParameters gravityParameters;

    [SerializeField]
    private GravityParameters defaultGravityParameters;

    [SerializeField]
    private int gravityScaleReturnSpeed = 2;

    [SerializeField]
    private int gravityDirectionReturnSpeed = 2;

    [SerializeField]
    private Rigidbody2D playerRigidbody;   

    private List<GravityModifier> currentModifiers;
    private GravityModifier defaultModifier;

    void Start()
    {
        if(playerRigidbody == null)
        {
            playerRigidbody = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Rigidbody2D>() ?? null;
            if(playerRigidbody == null)
            {
                Debug.LogError("GravityManager: Player Rigidbody2D not assigned and Player tag not found.");
                // disable this component - gravity manager shouldn't work in main menu, for example
                this.enabled = false;
            }
        }

        currentModifiers = new();
        defaultModifier = new GravityModifier(
            (param) =>
            {
                // Smoothly return gravity parameters to default values
                param.gravityScale = Mathf.MoveTowards(param.gravityScale, defaultGravityParameters.gravityScale, gravityScaleReturnSpeed * Time.fixedDeltaTime);
                param.gravityDirection = Vector2.MoveTowards(param.gravityDirection, defaultGravityParameters.gravityDirection, gravityDirectionReturnSpeed * Time.fixedDeltaTime);
                return param;
            },
            ModifierMode.Continuous);
    }

    void FixedUpdate()
    {
        foreach (var modifier in currentModifiers)
            gravityParameters = modifier.Invoke(gravityParameters) ?? gravityParameters;

        // Gravity
        playerRigidbody.AddForce(gravityParameters.gravityDirection * gravityParameters.gravityScale * 9.81f,
            ForceMode2D.Force);


        // get player's "down"
        Vector2 playerDown = -playerRigidbody.transform.up;

        // rotate player to align with gravity direction
        float angle = Vector2.SignedAngle(playerDown, gravityParameters.gravityDirection);

        // smoothly rotate rb to align with gravity direction
        playerRigidbody.MoveRotation(playerRigidbody.rotation + angle * 0.1f);
    }

    public void AddModifier(GravityModifier gravityModifier)
    {   
        if(currentModifiers.Where(currentModifiers => currentModifiers.GUID == gravityModifier.GUID).Any())
            return;
        if(currentModifiers.Contains(defaultModifier))
            currentModifiers.Remove(defaultModifier);

        currentModifiers.Add(gravityModifier);        
    }

    public void RemoveModifier(GravityModifier gravityModifier)
    {
        int idx = currentModifiers.IndexOf(gravityModifier);
        if (idx != -1)
            currentModifiers.RemoveAt(idx);

        if(currentModifiers.Count == 0)
        {
            // Reset gravity to default values over time
            AddModifier(defaultModifier);
        }
    }

    public Vector2 GetGravityDirection()
    {
        return gravityParameters.gravityDirection;
    }

}
