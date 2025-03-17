// namespace TownOfUsReworked.Loaders;

// public sealed class CoDownload : IEnumerator
// {
//     public object Current => Task.Current;
//     public bool Completed { get; set; }

//     private IEnumerator Task { get; }

//     public CoDownload(string url, string location) => Task = CoDownloadItem(this, url, location);

//     public bool MoveNext() => Task.MoveNext();

//     public void Reset() => Task.Reset();

//     private static IEnumerator CoDownloadItem(CoDownload iEnum, string url, string location)
//     {
//         iEnum.Completed = false;
//         var www = UnityWebRequest.Get(url);
//         yield return www.SendWebRequest();

//         if (www.result != UnityWebRequest.Result.Success)
//             Error(www.error);
//         else
//         {
//             var persistTask = File.WriteAllBytesAsync(location, www.downloadHandler.data);

//             while (!persistTask.IsCompleted)
//             {
//                 if (persistTask.Exception != null)
//                 {
//                     Error(persistTask.Exception);
//                     break;
//                 }

//                 yield return EndFrame();
//             }
//         }

//         www.downloadHandler.Dispose();
//         www.Dispose();
//         iEnum.Completed = true;
//         yield return null;
//     }
// }