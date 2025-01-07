namespace TownOfUsReworked.Modules;

public delegate bool VentExclusion(Vent vent);
public delegate bool ConsoleExclusion(Console console);
public delegate bool PlayerBodyExclusion(PlayerControl player);

public delegate bool EndFunc();
public delegate bool UsableFunc();
public delegate bool ConditionFunc();

public delegate void DelayVoid();
public delegate void EffectVoid();
public delegate void DelayEndVoid();
public delegate void EffectEndVoid();
public delegate void OtherDelayVoid();
public delegate void DelayStartVoid();
public delegate void EffectStartVoid();
public delegate void OtherDelayEndVoid();
public delegate void OtherDelayStartVoid();

public delegate void OnClickTargetless();
public delegate void OnClickVent(Vent target);
public delegate void OnClickBody(DeadBody target);
public delegate void OnClickConsole(Console target);
public delegate void OnClickPlayer(PlayerControl target);

public delegate float DifferenceFunc();
public delegate float MultiplierFunc();

public delegate string LabelFunc();

public delegate string SpriteFunc();

public record PostDeath(bool Value);

public record CanClickAgain(bool Value);

public record Manual(bool Value);

public record SpriteName(string Value);

public record Cooldown(float Value);

public record Duration(float Value);

public record Delay(float Value);

public record OtherDelay(float Value);

public record UsesDecrement(int Value);