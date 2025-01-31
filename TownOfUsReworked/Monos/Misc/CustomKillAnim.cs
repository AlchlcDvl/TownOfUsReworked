using PowerTools;

namespace TownOfUsReworked.Monos;

// Taken from Submerged going open source recently
public class CustomKillAnimationPlayer : MonoBehaviour
{
    private const string DEATH_ANIM =
        "0,0,4,0,0;" +
        "0,1.08,2,0,0;" +
        "0,1.15,2,0,0;" +
        "0,1.28,2,0,0;" +
        "0,1.33,4,0,0;" +
        "0,1.37,2,0,0;" +
        "0,1.39,4,0,0;" +
        "0,1.42,4,0,0;" +
        "0,1.46,4,0,0;" +
        "0,1.5,4,0,0;" +
        "0,1.56,4,0,0;" +
        "3,0.43,3,-0.3,0;" +
        "3,0.66,4,-0.5,0;" +
        "3,0.89,4,-0.5,0;" +
        "3,0.97,4,-0.5,0;" +
        "3,1.01,4,-0.5,0;" +
        "3,1.05,4,-0.5,0;" +
        "3,1.11,4,-0.5,0;" +
        "3,1.14,4,-0.5,0;" +
        "3,1.17,4,-0.5,0;" +
        "3,1.23,4,-0.5,0;" +
        "3,1.27,4,-0.5,0;" +
        "3,1.01,3,-0.5,0;" +
        "3,0.97,4,-0.5,0;" +
        "3,0.93,16,-0.5,0;";

    public float PlayHead;
    public bool Finished;

    private readonly Dictionary<int, KillAnimFrame> Frames = [];
    private OverlayKillAnimation Overlay;
    private SpriteAnim BodyAnim;
    private SpriteAnim SkinAnim;
    private GameObject Victim;

    public void Awake()
    {
        Overlay = GetComponent<OverlayKillAnimation>();
        Victim = Overlay.victimParts.gameObject;
        BodyAnim = Overlay.victimParts.cosmetics.bodySprites[0].BodySprite.GetComponent<SpriteAnim>();
        SkinAnim = Overlay.victimParts.GetSkinSpriteAnim();

        LoadFrom(DEATH_ANIM);
    }

    public void Start()
    {
        BodyAnim.Speed = 0;
        BodyAnim.Stop();

        SkinAnim.Speed = 0;
        SkinAnim.Stop();
    }

    public void Update()
    {
        if (!Frames.Any())
            return;

        PlayHead += Time.deltaTime;

        if (PlayHead > 2.5f)
        {
            Finished = true;
            return;
        }

        var currentFrame = Frames[Mathf.FloorToInt(PlayHead * Frames.Count / 2.5f)];
        UpdateVisuals(currentFrame.Time, currentFrame.Offset, currentFrame.Animation);
    }

    public void UpdateVisuals(float sampleTime, Vector2 characterOffset, int animation)
    {
        var skinData = Overlay.victimParts.cosmetics.skin.skin;
        UpdateVisuals(sampleTime, characterOffset, HUD().KillOverlay.KillAnims[animation].victimParts.cosmetics.bodySprites[0].BodySprite.GetComponent<SpriteAnim>().m_defaultAnim,
            animation switch
            {
                0 => skinData.KillStabVictim,
                1 => skinData.KillNeckVictim,
                2 => skinData.KillTongueVictim,
                3 => skinData.KillShootVictim,
                _ => null
            }
        );
    }

    public void UpdateVisuals(float sampleTime, Vector2 characterOffset, AnimationClip bodyAnimClip, AnimationClip skinAnimClip)
    {
        AnimationClip.SampleAnimation(BodyAnim.gameObject, bodyAnimClip, sampleTime, WrapMode.Default);

        if (skinAnimClip)
            AnimationClip.SampleAnimation(SkinAnim.gameObject, skinAnimClip, sampleTime, WrapMode.Default);

        Victim.transform.localPosition = characterOffset + new Vector2(-1.5f, 0);
    }

    public void LoadFrom(string toLoad)
    {
        var frameCount = 0;
        Frames.Clear();

        foreach (var dataString in toLoad.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var frame = KillAnimFrame.Deserialize(dataString);

            for (var idx = frameCount; idx < frameCount + frame.Length; idx++)
                Frames[idx] = frame;

            frameCount += frame.Length;
        }

        if (frameCount == 0)
            return;

        Frames[frameCount] = Frames[frameCount - 1];
    }

    [HideFromIl2Cpp]
    public IEnumerator WaitForFinish()
    {
        while (!Finished)
            yield return EndFrame();
    }
}