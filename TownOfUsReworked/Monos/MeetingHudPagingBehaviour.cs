namespace TownOfUsReworked.Monos;

public class MeetingHudPagingBehaviour : BasePagingBehaviour
{
    public MeetingHudPagingBehaviour(IntPtr ptr) : base(ptr) {}

    [HideFromIl2Cpp]
    public IEnumerable<PlayerVoteArea> Targets => Menu.playerStates.OrderBy(p => p.AmDead);
    public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;
    public MeetingHud Menu;

    public override void Update()
    {
        base.Update();

        if (Menu.state is MeetingHud.VoteStates.Animating or MeetingHud.VoteStates.Proceeding)
            return;

        Menu.TimerText.text += $" ({PageIndex + 1}/{MaxPageIndex + 1})";
    }

    public override void OnPageChanged()
    {
        var i = 0;

        foreach (var button in Targets)
        {
            if (i >= PageIndex * MaxPerPage && i < (PageIndex + 1) * MaxPerPage && !Ability.GetAssassins().Any(x => x.Phone) && !Role.GetRoles<Guesser>(LayerEnum.Guesser).Any(x => x.Phone)
                 && !Role.GetRoles<Thief>(LayerEnum.Thief).Any(x => x.Phone) && !Ability.GetAssassins().Any(x => x.Phone))
            {
                button.gameObject.SetActive(true);
                var relativeIndex = i % MaxPerPage;
                var row = relativeIndex / 3;
                var col = relativeIndex % 3;
                button.transform.localPosition = Menu.VoteOrigin + new Vector3(Menu.VoteButtonOffsets.x * col, Menu.VoteButtonOffsets.y * row, button.transform.localPosition.z);
            }
            else
                button.gameObject.SetActive(false);

            i++;
        }
    }
}