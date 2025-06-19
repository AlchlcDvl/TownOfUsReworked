namespace TownOfUsReworked.Monos;

public sealed class GameTimerHandler : MonoBehaviour
{
    public static GameTimerHandler Prefab;
    public static GameTimerHandler Instance;

    private TextMeshPro TimeText;
    private Transform TimerBar;
    private MeshRenderer TimerBarRenderer;
    private Transform ChunkBar;
    private float TargetBarSize;
    private bool FreezeChunk;
    private Coroutine ChunkCoroutine;
    private float Timer;
    private bool IsFinalStage;
    private UColor Original;
    private int Increment;

    public void Start()
    {
        var hnsTimer = GetComponent<HideAndSeekTimerBar>();
        TimeText = hnsTimer.timeText;
        TimerBar = hnsTimer.timerBar;
        TimerBarRenderer = hnsTimer.timerBarRenderer;
        ChunkBar = hnsTimer.chunkBar;
        TargetBarSize = hnsTimer.targetBarSize;
        FreezeChunk = hnsTimer.freezeChunk;
        TimeText = hnsTimer.timeText;
        hnsTimer.enabled = false;
        Original = TimerBarRenderer.material.GetColor(Color1);
        TimerBarRenderer.material = new(TimerBarRenderer.material);

        transform.localPosition -= new Vector3(0f, 0.4f, 0f);

        Timer = GameModifiers.GameTimer;
    }

    public void Update()
    {
        var num = Time.deltaTime * 10f;
        AdjustScale(TimerBar, num);

        if (!FreezeChunk)
            AdjustScale(ChunkBar, num);
    }

    public void FixedUpdate()
    {
        if (Meeting() && ((GameModifiers.DuringMeetings == DuringMeeting.TimeRemaining && Timer < GameModifiers.TimeLeft) || (GameModifiers.DuringMeetings == DuringMeeting.PeopleRemaining &&
            GameData.Instance.AllPlayers.Count(x => x) < GameModifiers.PlayersLeft)))
        {
            return;
        }

        Timer = Mathf.Clamp(Timer - Time.fixedDeltaTime, 0f, GameModifiers.GameTimer);
        var timeSpan = TimeSpan.FromSeconds(Timer);
        TimeText.text = timeSpan.ToString("m\\:ss");
        TargetBarSize = Mathf.Clamp01(Timer / GameModifiers.GameTimer);

        if (Timer <= 0f && AmongUsClient.Instance.AmHost)
            CheckEndGame.PerformStalemate();

        if (!IsFinalStage && Timer <= GameModifiers.GameTimer * 0.2f)
            StartFinalMoments();
    }

    public void StartFinalMoments()
    {
        TimerBarRenderer.material.SetColor(Color1, Palette.ImpostorRed);
        StopChunkCoroutine();
        ChunkBar.gameObject.SetActive(false);
        IsFinalStage = true;
    }

    public void ResetFinalMoments()
    {
        TimerBarRenderer.material.SetColor(Color1, Original);
        StartChunkCoroutine();
        ChunkBar.gameObject.SetActive(true);
        IsFinalStage = false;
    }

    private void AdjustScale(Transform item, float num)
    {
        var vector = item.localScale;
        vector.x = Mathf.Lerp(vector.x, TargetBarSize, num);
        item.localScale = vector;
    }

    public void TaskComplete() => StartChunkCoroutine();

    public void ExtendTimer() => Increment++;

    public void UpdateTimer()
    {
        Timer = Mathf.Clamp(Timer + (Increment * GameModifiers.TimerExtension), 0f, GameModifiers.GameTimer);

        if (IsFinalStage && Timer > GameModifiers.GameTimer * 0.2f)
            StartFinalMoments();

        Increment = 0;
    }

    private void StartChunkCoroutine()
    {
        StopChunkCoroutine();
        ChunkCoroutine = this.StartCoroutine(CoChunk());
    }

    private void StopChunkCoroutine()
    {
        if (ChunkCoroutine is null)
            return;

        StopCoroutine(ChunkCoroutine);
        ChunkCoroutine = null;
    }

    [HideFromIl2Cpp]
    private IEnumerator CoChunk()
    {
        FreezeChunk = true;
        yield return Wait(1f);
        FreezeChunk = false;
        ChunkCoroutine = null;
    }
}