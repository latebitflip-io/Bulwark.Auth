using System.Threading.Tasks;

namespace Bulwark.Auth.Core.Social;

public interface ISocialValidator
{
    string Name { get; }
    Task<Social> ValidateToken(string token);
}

