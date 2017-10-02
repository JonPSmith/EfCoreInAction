// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Raven.Client;
using Raven.Client.Document;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_RavenDb
    {
        private readonly ITestOutputHelper _output;
        private readonly IDocumentStore _store;

        private const string DatabaseName = "EfCoreInAction-Ch14-RavenDb";

        public Ch14_RavenDb(ITestOutputHelper output)
        {
            _output = output;
            _store = new DocumentStore
            {
                Url = RavenDbHelpers.RavenDbTestServerUrl,
                DefaultDatabase = DatabaseName
            }.Initialize();
            //if (_store.NumEntriesInDb() <= 0)
            //{
            //    _store.SeedDummyBooks();
            //}
        }

        [Fact]
        public void TestEnsureCreated()
        {
            //SETUP

            //ATTEMPT
            DatabaseName.EnsureCreated();

            //VERIFY
        }

        [Fact]
        public void TestAccessDatabase()
        {
            //SETUP

            //ATTEMPT
            var count = _store.NumEntriesInDb();

            //VERIFY
            count.ShouldEqual(10);
        }
    }


}