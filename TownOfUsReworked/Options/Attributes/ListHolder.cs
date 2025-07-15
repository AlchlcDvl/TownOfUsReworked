namespace TownOfUsReworked.Options.Attributes;

public sealed class ListHolderOptionAttribute(PlayerLayerEnum entryType, bool isBan) : BaseHeaderOptionAttribute<ListHolderOption>(MultiMenu.Main)
{
    private readonly PlayerLayerEnum EntryType = entryType;
    private readonly bool IsBan = isBan;

    protected override ListHolderOption SetUpOption() => new(EntryType, IsBan);
}