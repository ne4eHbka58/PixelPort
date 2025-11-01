using System.Linq.Expressions;

namespace PixelPort.Server.Services.IServices
{
    public interface IHashing
    {
        string ComputeHashSha128(string str);
    }
}
