using System;

public class GravityModifier
{
    private Guid _GUID;
    public Guid GUID { get => _GUID; }

    private GravityModifierDelegate modifier;
    private ModifierMode modifierMode;

    bool wasRun;

    public GravityModifier(GravityModifierDelegate modifier, ModifierMode modifierMode)
    {
        _GUID = Guid.NewGuid();
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
