namespace TownOfUsReworked.Custom;

public class PositionalArrow(PlayerControl owner, Vector3 target, UColor color) : CustomArrow(owner, color, () => target);