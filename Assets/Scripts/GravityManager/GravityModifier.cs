using UnityEditor;
using UnityEngine;

public class GravityModifier
{
    private GUID _GUID;
    public GUID GUID { get => _GUID; }

    private GravityModifierDelegate modifier;
    private ModifierMode modifierMode;

    bool wasRun;

    public GravityModifier(GravityModifierDelegate modifier, ModifierMode modifierMode)
    {
        _GUID = GUID.Generate();
        this.modifier = modifier;
        this.modifierMode = modifierMode;

        if (modifierMode == ModifierMode.Instantaneous)
            wasRun = false;
    }

    public GravityParameters? Invoke(GravityParameters parameters)
    {
        if(modifierMode == ModifierMode.Instantaneous && wasRun)
            return null;
        wasRun = true;
        return modifier.Invoke(parameters);
    }
}
