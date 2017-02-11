using System.Threading.Tasks;

namespace BraviaControl
{
    public interface IBraviaControlClient
    {
        BraviaControlClient.IAccessControl AccessControl { get; }
        BraviaControlClient.IAppControl AppControl { get; }
        BraviaControlClient.IAudio Audio { get; }
        BraviaControlClient.IAvContent AvContent { get; }
        BraviaControlClient.IBroadcastLink BroadcastLink { get; }
        BraviaControlClient.IBrowser Browser { get; }
        BraviaControlClient.ICec Cec { get; }
        BraviaControlClient.IEncryption Encryption { get; }
        BraviaControlClient.IGuide Guide { get; }
        BraviaControlClient.IRecording Recording { get; }
        BraviaControlClient.ISystem System { get; }
        BraviaControlClient.IVideoScreen VideoScreen { get; }

        Task<string> RegisterAsync(string pinCode);
        Task<string> RenewAuthKeyAsync();
        Task<bool> RequestPinAsync();
        Task SendIrccAsync(string code);
    }
}