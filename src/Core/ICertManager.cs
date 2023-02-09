using System.Collections.Generic;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;
public interface ICertManager
{
    TokenStrategyContext TokenContext { get; }
    void GenerateCertificate(int days);
	List<Certificate> GetCerts();
}


