using JWT.Algorithms;

namespace Bulwark.Auth.Core.SigningAlgs;

public interface ISigningAlgorithm
{
    string Name { get; }
    IJwtAlgorithm GetAlgorithm(string privateKey, string publicKey);
}