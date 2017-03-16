// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;

namespace test.Mocks
{
    public class MockHttpCookieAccess
    {
        private readonly Dictionary<string, string> _requestCookies = new Dictionary<string, string>();

        private readonly FrameResponseHeaders _responseCookies = new FrameResponseHeaders();

        public IRequestCookieCollection CookiesIn { get; private set; }

        public Microsoft.Extensions.Primitives.StringValues ResponseCookies => _responseCookies.HeaderSetCookie;

        public IResponseCookies CookiesOut { get; private set; }

        public MockHttpCookieAccess(string cookieName = null, string cookieContent = null)
        {
            if (cookieName != null)
                _requestCookies[cookieName] = cookieContent;

            CookiesIn = new RequestCookieCollection(_requestCookies);
            CookiesOut = new ResponseCookies(_responseCookies, null);
            var x = _responseCookies.HeaderSetCookie;
        }

    }
}