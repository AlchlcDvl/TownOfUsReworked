namespace TownOfUsReworked.Monos;

public sealed class RpcHandler : MonoBehaviour
{
    private readonly Queue<ReworkedMessage> LateMessages = [];

    private float Timer;

    private const float MinInterval = 0.1f;

    [HideFromIl2Cpp]
    public void QueueLateMessage(ReworkedMessage message) => LateMessages.Enqueue(message);

    public void FixedUpdate()
    {
        if (!AmongUsClient.Instance || TownOfUsReworked.MciActive || LateMessages.Count == 0)
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
        message.Dispose();
    }
}