namespace TownOfUsReworked.Custom;

public sealed class CustomButtonBuilder(PlayerLayer owner, ReworkedAbilityTypes type, KeybindType keybind)
{
    // Required Parameters
    public readonly PlayerLayer Owner = owner;
    public readonly ReworkedAbilityTypes Type = type;
    public readonly string Keybind = keybind.ToString();

    // Visuals
    public string ButtonSprite = "Placeholder";
    public Func<string> SpriteFunc = BlankButtonSprite;
    public string ButtonLabel = "ABILITY";
    public Func<string> ButtonLabelFunc = BlankButtonLabel;
    public UColor? TextColor = null;
    public Func<UColor?> TextColorFunc = BlankNullColor;

    // State & Limits
    public float Cooldown = 1f;
    public int MaxUses = -1;
    public int UseDecrement = 1;
    public bool CanClickAgain = true;
    public bool PostDeath = false;
    public bool IsManual = false;
    public Action ManualUpdate = BlankVoid;

    // Timings
    public float Duration = 0f;
    public float Delay = 0f;
    public float OtherDelay = 0f;

    // Filters (Predicates)
    public Func<bool> IsUsable = BlankTrue;
    public Func<bool> Condition = BlankTrue;
    public Func<PlayerControl, bool> PlayerFilter = BlankFalse;
    public Func<Vent, bool> VentFilter = BlankFalse;
    public Func<Console, bool> ConsoleFilter = BlankFalse;
    public Func<DeadBody, bool> BodyFilter = BlankFalse;

    // Math modifiers
    public Func<float> Difference = BlankZero;
    public Func<float> Multiplier = BlankOne;

    // Actions (Click Handlers)
    public Action DoClickTargetless = BlankVoid;
    public Action<PlayerControl> DoClickPlayer = BlankVoid;
    public Action<Vent> DoClickVent = BlankVoid;
    public Action<DeadBody> DoClickBody = BlankVoid;
    public Action<Console> DoClickConsole = BlankVoid;
    public Action OnClickedAgain = BlankVoid;

    // Actions (Lifecycle)
    public Action Effect = BlankVoid;
    public Action OnEffectStart = BlankVoid;
    public Action OnEffectEnd = BlankVoid;

    public Action ActionDelay = BlankVoid;
    public Action OnDelayStart = BlankVoid;
    public Action OnDelayEnd = BlankVoid;

    public Action ActionOtherDelay = BlankVoid;
    public Action OnOtherDelayStart = BlankVoid;
    public Action OnOtherDelayEnd = BlankVoid;

    public Func<bool> End = BlankFalse;

    public CustomButtonBuilder WithSprite(string sprite)
    {
        ButtonSprite = sprite;
        return this;
    }

    public CustomButtonBuilder WithSprite(Func<string> spriteFunc)
    {
        SpriteFunc = spriteFunc;
        return this;
    }

    public CustomButtonBuilder WithLabel(string label)
    {
        ButtonLabel = label;
        return this;
    }

    public CustomButtonBuilder WithLabel(Func<string> labelFunc)
    {
        ButtonLabelFunc = labelFunc;
        return this;
    }

    public CustomButtonBuilder WithColor(UColor? color)
    {
        TextColor = color;
        return this;
    }

    public CustomButtonBuilder WithLabel(Func<UColor?> labelFunc)
    {
        TextColorFunc = labelFunc;
        return this;
    }

    public CustomButtonBuilder WithCooldown(float cooldown)
    {
        Cooldown = cooldown;
        return this;
    }

    public CustomButtonBuilder WithUses(int maxUses, int decrement = 1)
    {
        MaxUses = maxUses;
        UseDecrement = decrement;
        return this;
    }

    public CustomButtonBuilder WithDuration(float duration)
    {
        Duration = duration;
        return this;
    }

    public CustomButtonBuilder WithDelay(float delay)
    {
        Delay = delay;
        return this;
    }

    public CustomButtonBuilder WithOtherDelay(float otherDelay)
    {
        OtherDelay = otherDelay;
        return this;
    }

    public CustomButtonBuilder WithPostDeath(bool postDeath)
    {
        PostDeath = postDeath;
        return this;
    }

    public CustomButtonBuilder WithClickAgain(bool canClickAgain)
    {
        CanClickAgain = canClickAgain;
        return this;
    }

    public CustomButtonBuilder AsManual(bool isManual, Action manualUpdate = null)
    {
        IsManual = isManual;

        if (manualUpdate != null)
            ManualUpdate = manualUpdate;

        return this;
    }

    public CustomButtonBuilder OnClick(Action action)
    {
        DoClickTargetless = action;
        return this;
    }

    public CustomButtonBuilder OnClick(Action<PlayerControl> action)
    {
        DoClickPlayer = action;
        return this;
    }

    public CustomButtonBuilder OnClick(Action<Vent> action)
    {
        DoClickVent = action;
        return this;
    }

    public CustomButtonBuilder OnClick(Action<DeadBody> action)
    {
        DoClickBody = action;
        return this;
    }

    public CustomButtonBuilder OnClick(Action<Console> action)
    {
        DoClickConsole = action;
        return this;
    }

    public CustomButtonBuilder OnClickAgain(Action action)
    {
        OnClickedAgain = action;
        return this;
    }

    public CustomButtonBuilder WithEffect(Action effect, Action start = null, Action end = null)
    {
        Effect = effect;

        if (start != null)
            OnEffectStart = start;

        if (end != null)
            OnEffectEnd = end;

        return this;
    }

    public CustomButtonBuilder WithDelayActions(Action delayAction, Action start = null, Action end = null)
    {
        ActionDelay = delayAction;

        if (start != null)
            OnDelayStart = start;

        if (end != null)
            OnDelayEnd = end;

        return this;
    }

    public CustomButtonBuilder WithOtherDelayActions(Action otherAction, Action start = null, Action end = null)
    {
        ActionOtherDelay = otherAction;

        if (start != null)
            OnOtherDelayStart = start;

        if (end != null)
            OnOtherDelayEnd = end;

        return this;
    }

    public CustomButtonBuilder WithEndCondition(Func<bool> endCondition)
    {
        End = endCondition;
        return this;
    }

    public CustomButtonBuilder WithCondition(Func<bool> condition)
    {
        Condition = condition;
        return this;
    }

    public CustomButtonBuilder WithUsability(Func<bool> usable)
    {
        IsUsable = usable;
        return this;
    }

    public CustomButtonBuilder WithPlayerExclusion(Func<PlayerControl, bool> exclusion)
    {
        PlayerFilter = x => !exclusion(x);
        BodyFilter = x => !PlayerFilter(PlayerByBody(x));
        return this;
    }

    public CustomButtonBuilder WithVentExclusion(Func<Vent, bool> exclusion)
    {
        VentFilter = x => !exclusion(x);
        return this;
    }

    public CustomButtonBuilder WithConsoleExclusion(Func<Console, bool> exclusion)
    {
        ConsoleFilter = x => !exclusion(x);
        return this;
    }

    public CustomButtonBuilder WithCooldownModifiers(Func<float> difference = null, Func<float> multiplier = null)
    {
        if (difference != null)
            Difference = difference;

        if (multiplier != null)
            Multiplier = multiplier;

        return this;
    }

    public CustomButton Build() => new(this);
}