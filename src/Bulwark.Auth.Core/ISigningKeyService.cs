using System.Collections.Generic;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;
public interface ISigningKeyService
{
    TokenStrategyContext TokenContext { get; }
    void GenerateKey();
	List<Key> GetKeys();
}


