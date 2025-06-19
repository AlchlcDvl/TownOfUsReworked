namespace TownOfUsReworked.Monos;

public sealed class RpcHandler : MonoBehaviour
{
    private readonly Queue<ReworkedMessage> LateMessages = [];

    [HideFromIl2Cpp]
    public void QueueLateMessage(ReworkedMessage message) => LateMessages.Enqueue(message);

    private const float MinInterval = 0.1f;
    private float Timer;

    public void FixedUpdate()
    {
        if (!AmongUsClient.Instance || TownOfUsReworked.MciActive)
            return;

        Timer += Time.fixedDeltaTime;

        if (Timer <= MinInterval)
            return;

        Timer = 0f;

        while (LateMessages.TryDequeue(out var message))
            SendImmediateMessage(message);
    }

    public static void SendImmediateMessage(ReworkedMessage message)
    {
        var writer = CreateMessageWriter(message.TargetClientId);
        message.SerializeRpcValues(writer);
        writer.Send();
    }
}