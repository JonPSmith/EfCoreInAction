// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace test.Mocks
{
    public class MockHttpCookieAccess
    {
        private readonly Dictionary<string, string> _requestCookies = new Dictionary<string, string>();

        public IRequestCookieCollection CookiesIn { get; private set; }

        public Microsoft.Extensions.Primitives.StringValues ResponseCookies { get; }
    

        public IResponseCookies CookiesOut { get; private set; }

        public MockHttpCookieAccess(string cookieName = null, string cookieContent = null)
        {
            throw new NotImplementedException("cannot find out how to mock cookies in NET Core 3");
        }

    }
}