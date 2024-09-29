namespace TownOfUsReworked.Debugger;

public abstract class BaseTab
{
    public abstract string Name { get; }

    public abstract void OnGUI();
}