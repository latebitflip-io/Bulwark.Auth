using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Bulwark.Token
{
    public class TokenStrategyContext
    {
        private Dictionary<string, ITokenizer> _tokenizers;

        public TokenStrategyContext(List<ITokenizer> tokenizers)
        {
            _tokenizers = new Dictionary<string, ITokenizer>();
            foreach (var tokenizer in tokenizers)
            {
                Add(tokenizer);
            }
        }

        public TokenStrategyContext()
        {
            _tokenizers = new Dictionary<string, ITokenizer>();
        }

        public void Add(ITokenizer tokenizer)
        {
            _tokenizers.Add(tokenizer.Name, tokenizer);
        }

        public string CreateAccessToken(string userId, string name = "default")
        {
            var tokenizer = _tokenizers[name];
            return tokenizer.CreateAccessToken(userId);
        }

        public string CreateIdToken(Dictionary<string, object> idClaims,
            string salt,
            string name = "default")
        {
            var tokenizer = _tokenizers[name];
            return tokenizer.CreateIdToken(idClaims, salt);
        }

        public string CreateRefreshToken(string userId, string salt,
            string name = "default")
        {
            var tokenizer = _tokenizers[name];
            return tokenizer.CreateRefreshToken(userId, salt);
        }

        public JObject ValidateAccessToken(string token, string name = "default")
        {
            var json = _tokenizers[name].ValidateAccessToken(token);
            return JObject.Parse(json);
        }

        public JObject ValidateRefreshToken(string token, string salt,
            string name = "default")
        {
            var json = _tokenizers[name].ValidateRefreshToken(token, salt);
            return JObject.Parse(json);
        }

        public JObject ValidateIdToken(string token, string salt,
            string name = "default")
        {
            var json = _tokenizers[name].ValidateIdToken(token, salt);
            return JObject.Parse(json);
        }
    } 
}
