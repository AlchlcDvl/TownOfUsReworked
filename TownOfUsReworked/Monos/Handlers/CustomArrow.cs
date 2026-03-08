namespace TownOfUsReworked.Monos;

public sealed class CustomArrowHandler : MonoBehaviour
{
    public CustomArrow? Arrow;

    public void Update() => Arrow?.Update();
}