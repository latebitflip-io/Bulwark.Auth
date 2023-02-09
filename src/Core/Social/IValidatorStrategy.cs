using System.Collections.Generic;

namespace Bulwark.Auth.Core.Social;
public interface IValidatorStrategies
{
	void Add(ISocialValidator validator);
	ISocialValidator Get(string key);
    Dictionary<string, ISocialValidator> GetAll();
}


