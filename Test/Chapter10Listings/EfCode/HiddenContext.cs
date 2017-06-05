// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter10Listings.EfCode
{
    public class HiddenContext
    {
        internal Chapter10DbContext Context { get; private set; }
        public HiddenContext(Chapter10DbContext context)
        {
            Context = context;
        }
    }
}