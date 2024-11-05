namespace TownOfUsReworked.Custom;

public class CustomMeeting
{
    public PlayerControl Owner { get; }
    public OnClick Click { get; }
    public Action Parallel { get; }
    public Exemption IsExempt { get; }
    public string ActiveSprite { get; }
    public string DisabledSprite { get; }
    public MeetingTypes Type { get; }
    public Vector3? Position { get; set; }
    public Dictionary<byte, bool> Actives { get; set; }
    public Dictionary<byte, GameObject> Buttons { get; set; }
    public bool AfterVote { get; }
    public delegate void OnClick(PlayerVoteArea voteArea, MeetingHud __instance);
    public delegate bool Exemption(PlayerVoteArea voteArea);
    private static Vector3 BasePosition => new(-0.95f, 0.03f, -1.3f);
    public static readonly List<CustomMeeting> AllCustomMeetings = [];

    public CustomMeeting(PlayerControl owner, string active, string disabled, bool vote, OnClick click, Exemption isExempt = null, Vector3? position = null) : this(owner, active, disabled,
        MeetingTypes.Toggle, vote, click, isExempt, null, position) {}

    public CustomMeeting(PlayerControl owner, string sprite, bool vote, OnClick click, Exemption isExempt = null, Action parallel = null, Vector3? position = null) : this(owner, sprite, "",
        MeetingTypes.Click, vote, click, isExempt, parallel, position) {}

    public CustomMeeting(PlayerControl owner, string active, string disabled, bool vote, OnClick click, Exemption isExempt = null, Action parallel = null, Vector3? position = null) :
        this(owner, active, disabled, MeetingTypes.Toggle, vote, click, isExempt, parallel, position) {}

    private CustomMeeting(PlayerControl owner, string active, string disabled, MeetingTypes type, bool vote, OnClick click, Exemption isExempt = null, Action parallel = null, Vector3?
        position = null)
    {
        Owner = owner;
        Click = click;
        AfterVote = vote;
        ActiveSprite = active;
        DisabledSprite = disabled;
        Type = type;
        Parallel = parallel ?? BlankVoid;
        IsExempt = isExempt ?? BlankFalse;
        Position = position ?? BasePosition;
        Actives = [];
        Buttons = [];
        AllCustomMeetings.Add(this);
    }

    public void HideButtons()
    {
        Buttons.Keys.ForEach(HideSingle);
        Buttons.Clear();
        Actives.Clear();
    }

    public void HideSingle(byte targetId)
    {
        Actives[targetId] = false;

        if (!Buttons.TryGetValue(targetId, out var button) || !button)
            return;

        button.GetComponent<PassiveButton>().WipeListeners();
        button.SetActive(false);
        button.Destroy();
        Buttons[targetId] = null;
    }

    private void GenButton(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        Actives.Add(voteArea.TargetPlayerId, false);

        if (IsExempt(voteArea))
        {
            Buttons.Add(voteArea.TargetPlayerId, null);
            return;
        }

        var targetBox = UObject.Instantiate(voteArea.Buttons.transform.Find("CancelButton").gameObject, voteArea.transform);
        targetBox.name = Owner.name + ActiveSprite + voteArea.name;
        targetBox.transform.localPosition = Position.Value;
        var renderer = targetBox.GetComponent<SpriteRenderer>();
        renderer.sprite = GetSprite(Type == MeetingTypes.Toggle ? DisabledSprite : ActiveSprite);
        var button = targetBox.GetComponent<PassiveButton>();
        button.OverrideOnClickListeners(() => Click(voteArea, __instance));
        button.OverrideOnMouseOverListeners(() => renderer.color = UColor.red);
        button.OverrideOnMouseOutListeners(() => renderer.color = Type == MeetingTypes.Toggle && Actives[voteArea.TargetPlayerId] ? UColor.green : UColor.white);
        button.ClickSound = GetAudio("Click");
        button.HoverSound = GetAudio("Hover");
        var collider = targetBox.GetComponent<BoxCollider2D>();
        collider.size = renderer.sprite.bounds.size;
        collider.offset = Vector2.zero;
        targetBox.transform.GetChild(0).gameObject.Destroy();
        Buttons.Add(voteArea.TargetPlayerId, targetBox);
    }

    public void GenButtons(MeetingHud __instance, bool usable = true)
    {
        HideButtons();

        if (!usable || Owner != CustomPlayer.Local)
            return;

        Parallel();
        Actives.Clear();
        Buttons.Clear();
        AllVoteAreas().ForEach(x => GenButton(x, __instance));
    }

    public void Update(MeetingHud __instance)
    {
        if (Type == MeetingTypes.Toggle)
        {
            foreach (var pair in Buttons)
            {
                if (!pair.Value)
                    continue;

                var sprite = pair.Value.GetComponent<SpriteRenderer>();
                sprite.sprite = GetSprite(Actives[pair.Key] ? ActiveSprite : DisabledSprite);
                sprite.color = Actives[pair.Key] ? UColor.green : UColor.white;
            }
        }
    }

    public void Voted()
    {
        if (!AfterVote)
            HideButtons();
    }

    public void Destroy() => HideButtons();

    public static void DestroyAll()
    {
        AllCustomMeetings.ForEach(x => x.Destroy());
        AllCustomMeetings.Clear();
    }
}