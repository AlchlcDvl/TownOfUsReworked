using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TownOfUsReworked.Modules;

public class CustomAddressable<T>(T item, string guid) : CustomAddressable(guid) where T : UObject
{
    private T Asset { get; } = item?.DontDestroy();

    public override AsyncOperationHandle LoadAsync() => Handle ??= Addressables.ResourceManager.CreateCompletedOperation(Asset, Asset ? string.Empty : "Unable to find asset");
}

public abstract class CustomAddressable
{
    public static readonly Dictionary<string, CustomAddressable> CustomAdressables = [];

    public AssetReference Ref { get; }
    protected AsyncOperationHandle Handle { get; set; }

    protected CustomAddressable(string guid)
    {
        guid = $"asset/reworked:{guid.ToLower().Trim()}";
        Ref = new(guid);
        CustomAdressables[guid] = this;
    }

    public abstract AsyncOperationHandle LoadAsync();
}