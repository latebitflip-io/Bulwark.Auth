using System.Collections.Generic;


namespace Bulwark.Auth.Core.Social;

public class ValidatorStrategies : IValidatorStrategies
{
    private Dictionary<string, ISocialValidator> _validators;

    public ValidatorStrategies()
	{
        _validators = new Dictionary<string, ISocialValidator>();
	}

    public void Add(ISocialValidator validator)
    {
        _validators.Add(validator.Name, validator);
    }

    public ISocialValidator Get(string name)
    {
        return _validators[name];
    }

    public Dictionary<string, ISocialValidator> GetAll()
    {
        return _validators;
    }
}


