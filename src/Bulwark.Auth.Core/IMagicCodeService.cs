using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;
public interface IMagicCodeService
{
	Task<string> CreateCode(string email, int expireInMin);
	Task<Authenticated> AuthenticateCode(string email, string code,
		string tokenizerName = "jwt");
}


