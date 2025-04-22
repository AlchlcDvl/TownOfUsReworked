namespace TownOfUsReworked.Options;

public abstract class OptionAttribute<T> : OptionAttribute where T : Option
{
    protected abstract T SetUpOption();

    protected override Option BaseSetUpOption() => SetUpOption();
}