namespace TownOfUsReworked.Options;

public sealed class ListHolderOptionAttribute(PlayerLayerEnum entryType, bool isBan) : BaseHeaderOptionAttribute<ListHolderOption>(MultiMenu.Main)
{
    private PlayerLayerEnum EntryType { get; } = entryType;
    private bool IsBan { get; } = isBan;

    protected override ListHolderOption SetUpOption() => new(EntryType, IsBan);
}