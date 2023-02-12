using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;
public interface IMagicCodeManager
{
	Task<string> CreateCode(string email, int expireInMins);
	Task<Authenticated> AuthenticateCode(string email, string code,
		string tokenizerName = "default");
}


