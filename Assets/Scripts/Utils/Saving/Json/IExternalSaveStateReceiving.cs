using Newtonsoft.Json.Linq;

namespace Utils.Saving
{
    public interface IExternalSaveStateReceiving
    {
        public void ReceiveExternalStringSaveState(string stateJson);
    }
}
