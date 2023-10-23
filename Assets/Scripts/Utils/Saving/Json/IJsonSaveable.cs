using Newtonsoft.Json.Linq;

namespace Utils.Saving.Json
{
    public interface IJsonSaveable
    {
        JToken CaptureAsJToken();

        void RestoreFromJToken(JToken state);
    }
}
