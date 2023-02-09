using Bulwark.Auth.Core.Domain;
using System.Threading.Tasks;

namespace Bulwark.Auth.Core.Social
{
    public interface ISocialManager
    {
        Task<Authenticated> Authenticate(string provider, string token,
            string tokenizerName = "default");
        void AddValidator(ISocialValidator validator);
    }
}
