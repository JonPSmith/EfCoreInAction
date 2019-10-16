using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace test.Mocks
{
    public class FakeResponseCookies : IResponseCookies
    {
        public List<KeyValuePair<string, string>> Responses { get; private set; } =
            new List<KeyValuePair<string, string>>();

        public void Append(string key, string value)
        {
            Responses.Add(new KeyValuePair<string, string>(key, value));
        }

        public void Append(string key, string value, CookieOptions options)
        {
            Responses.Add(new KeyValuePair<string, string>(key, $"{value}; {options.ToString()}"));
        }

        public void Delete(string key)
        {
            var found = Responses.SingleOrDefault(x => x.Key == key);
            if (found.Key == key)
                Responses.Remove(found);
        }

        public void Delete(string key, CookieOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}