namespace TownOfUsReworked.Custom;

public sealed class PositionalArrow(PlayerControl owner, Vector3 target, UColor color) : CustomArrow(owner, color, () => target);