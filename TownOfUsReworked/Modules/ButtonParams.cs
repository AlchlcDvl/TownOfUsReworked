namespace TownOfUsReworked.Modules;

public delegate bool VentExclusion(Vent vent);
public delegate bool ConsoleExclusion(Console console);
public delegate bool PlayerBodyExclusion(PlayerControl player);

public delegate bool EndFunc();
public delegate bool UsableFunc();
public delegate bool ConditionFunc();

public delegate void OnClick();
public delegate void DelayVoid();
public delegate void EffectVoid();
public delegate void DelayEndVoid();
public delegate void EffectEndVoid();
public delegate void DelayStartVoid();
public delegate void EffectStartVoid();

public delegate float DifferenceFunc();
public delegate float MultiplierFunc();

public delegate string LabelFunc();

public record PostDeath(bool Value);

public record CanClickAgain(bool Value);

public record SpriteName(string Value);

public record Cooldown(float Value);

public record Duration(float Value);

public record Delay(float Value);