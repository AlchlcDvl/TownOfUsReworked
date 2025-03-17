namespace TownOfUsReworked.Options;

public sealed class ListHolderOption(PlayerLayerEnum entryType, bool isBan) : HeaderOption(MultiMenu.Main, CustomOptionType.ListHolder)
{
    private PlayerLayerEnum EntryType { get; } = entryType;
    private bool IsBan { get; } = isBan;

    public override void PostLoadSetup()
    {
        if (IsBan)
        {
            switch (EntryType)
            {
                case PlayerLayerEnum.Role:
                {
                    Holders.RolesBanList = this;
                    break;
                }
                case PlayerLayerEnum.Modifier:
                {
                    Holders.ModifiersBanList = this;
                    break;
                }
                case PlayerLayerEnum.Ability:
                {
                    Holders.AbilitiesBanList = this;
                    break;
                }
                case PlayerLayerEnum.Disposition:
                {
                    Holders.DispositionsBanList = this;
                    break;
                }
            }
        }
        else
        {
            switch (EntryType)
            {
                case PlayerLayerEnum.Role:
                {
                    Holders.RolesEntryList = this;
                    break;
                }
                case PlayerLayerEnum.Modifier:
                {
                    Holders.ModifiersEntryList = this;
                    break;
                }
                case PlayerLayerEnum.Ability:
                {
                    Holders.AbilitiesEntryList = this;
                    break;
                }
                case PlayerLayerEnum.Disposition:
                {
                    Holders.DispositionsEntryList = this;
                    break;
                }
            }
        }
    }

    public void AddEntryForPlayer()
    {
        var entry = new ListEntryOption(EntryType, IsBan, GroupMembers.Count)
        {
            Value = ListSlot.None,
            Header = this
        };
        entry.PostLoadSetup();
        entry.Debug();
        GroupMembers.Add(entry);
    }
}