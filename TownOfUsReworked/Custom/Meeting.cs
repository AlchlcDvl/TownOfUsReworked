namespace TownOfUsReworked.Custom;

public class CustomMeeting
{
    public readonly PlayerControl Owner;
    public readonly OnClick Click;
    public readonly Action Parallel;
    public readonly Exemption IsExempt;
    public readonly string ActiveSprite;
    public readonly string DisabledSprite;
    public readonly MeetingTypes Type;
    public Vector3? Position { get; set; }
    public Dictionary<byte, bool> Actives { get; set; }
    public Dictionary<byte, GameObject> Buttons { get; set; }
    public readonly bool AfterVote;
    public delegate void OnClick(PlayerVoteArea voteArea, MeetingHud __instance);
    public delegate bool Exemption(PlayerVoteArea voteArea);
    private static Vector3 BasePosition => new(-0.95f, 0.03f, -1.3f);
    public static readonly List<CustomMeeting> AllCustomMeetings = new();

    public CustomMeeting(PlayerControl owner, string active, string disabled, bool vote, OnClick click, Exemption isExempt = null, Vector3? position = default) : this(owner, active, disabled,
        MeetingTypes.Toggle, vote, click, isExempt, null, position) {}

    public CustomMeeting(PlayerControl owner, string sprite, bool vote, OnClick click, Exemption isExempt = null, Action parallel = null, Vector3? position = default) : this(owner,
        sprite, "", MeetingTypes.Click, vote, click, isExempt, parallel, position) {}

    public CustomMeeting(PlayerControl owner, string active, string disabled, bool vote, OnClick click, Exemption isExempt = null, Action parallel = null, Vector3? position = default)
        : this(owner, active, disabled, MeetingTypes.Toggle, vote, click, isExempt, parallel, position) {}

    private CustomMeeting(PlayerControl owner, string active, string disabled, MeetingTypes type, bool vote, OnClick click, Exemption isExempt = null, Action parallel = null, Vector3?
        position = default)
    {
        Owner = owner;
        Click = click;
        AfterVote = vote;
        ActiveSprite = active;
        DisabledSprite = disabled;
        Type = type;
        Parallel = parallel ?? Blank;
        IsExempt = isExempt ?? BlankBool;
        Position = position ?? BasePosition;
        Actives = new();
        Buttons = new();
        AllCustomMeetings.Add(this);
    }

    private static bool BlankBool(PlayerVoteArea voteArea) => false;

    private static void Blank() {}

    public void HideButtons()
    {
        Buttons.Keys.ForEach(HideSingle);
        Buttons.Clear();

        if (Type == MeetingTypes.Toggle)
        {
            Actives.Keys.ForEach(x => Actives[x] = false);
            Actives.Clear();
        }
    }

    public void HideSingle(byte targetId)
    {
        if (!Buttons.TryGetValue(targetId, out var button) || button == null)
            return;

        button.GetComponent<PassiveButton>().OnClick = new();
        button.GetComponent<PassiveButton>().OnMouseOut = new();
        button.GetComponent<PassiveButton>().OnMouseOver = new();
        button.SetActive(false);
        button.Destroy();
        Buttons[targetId] = null;

        if (Type == MeetingTypes.Toggle)
            Actives[targetId] = false;
    }

    private void GenButton(PlayerVoteArea voteArea, MeetingHud __instance, bool usable = true)
    {
        Actives.Clear();
        Buttons.Clear();

        if (!usable || Owner != CustomPlayer.Local)
            return;

        if (IsExempt(voteArea))
        {
            Buttons.Add(voteArea.TargetPlayerId, null);

            if (Type == MeetingTypes.Toggle)
                Actives.Add(voteArea.TargetPlayerId, false);

            return;
        }

        var targetBox = UObject.Instantiate(voteArea.Buttons.transform.Find("CancelButton").gameObject, voteArea.transform);
        targetBox.name = Owner.name + ActiveSprite + voteArea.name;
        targetBox.transform.localPosition = Position.Value;
        var renderer = targetBox.GetComponent<SpriteRenderer>();
        renderer.sprite = GetSprite(Type == MeetingTypes.Toggle ? DisabledSprite : ActiveSprite);
        var button = targetBox.GetComponent<PassiveButton>();
        button.OnClick = new();
        button.OnClick.AddListener((Action)(() => Click(voteArea, __instance)));
        button.OnMouseOver = new();
        button.OnMouseOver.AddListener((Action)(() => renderer.color = UColor.red));
        button.OnMouseOut = new();
        button.OnMouseOut.AddListener((Action)(() => renderer.color = Type == MeetingTypes.Toggle && Actives[voteArea.TargetPlayerId] ? UColor.green : UColor.white));
        var collider = targetBox.GetComponent<BoxCollider2D>();
        collider.size = renderer.sprite.bounds.size;
        collider.offset = Vector2.zero;
        targetBox.transform.GetChild(0).gameObject.Destroy();
        Buttons.Add(voteArea.TargetPlayerId, targetBox);

        if (Type == MeetingTypes.Toggle)
            Actives.Add(voteArea.TargetPlayerId, false);
    }

    public void GenButtons(MeetingHud __instance, bool usable = true)
    {
        Parallel();
        AllVoteAreas.ForEach(x => GenButton(x, __instance, usable));
    }

    public void Update()
    {
        if (Type == MeetingTypes.Toggle)
        {
            foreach (var pair in Buttons)
            {
                if (pair.Value == null)
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