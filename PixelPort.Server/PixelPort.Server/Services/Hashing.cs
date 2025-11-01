using PixelPort.Server.Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace PixelPort.Server.Services
{
    public class Hashing : IHashing
    {
        public string ComputeHashSha128(string str) 
        {
            using(var sha128 = SHA1.Create()) 
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(str);
                byte[] hash = sha128.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
