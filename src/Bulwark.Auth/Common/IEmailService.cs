using System.Threading.Tasks;

namespace Bulwark.Auth.Common.Payloads;
public interface IEmailService
{
    Task Send(string to, string template);
}

