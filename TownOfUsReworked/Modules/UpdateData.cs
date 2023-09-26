using Newtonsoft.Json.Linq;
using Version = SemanticVersioning.Version;

namespace TownOfUsReworked.Modules;

public class UpdateData
{
    public string Content;
    public string Tag;
    public string TimeString;
    public JObject Request;
    public Version Version => Version.Parse(Tag);

    public UpdateData(JObject data)
    {
        Tag = data["tag_name"]?.ToString().TrimStart('v');
        Content = data["body"]?.ToString();
        TimeString = DateTime.FromBinary(((Il2CppSystem.DateTime)data["published_at"]).ToBinaryRaw()).ToString();
        Request = data;
    }

    public bool IsNewer(Version version)
    {
        if (!Version.TryParse(Tag, out var myVersion))
            return false;

        return myVersion?.BaseVersion() > version?.BaseVersion();
    }
}