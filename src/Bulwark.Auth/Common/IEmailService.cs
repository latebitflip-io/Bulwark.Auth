using System.Threading.Tasks;

namespace Bulwark.Auth.Common;
public interface IEmailService
{
    Task Send(string to, string template);
}

