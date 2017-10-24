// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace DataNoSql
{
    public class RavenStore : //#A
        INoSqlCreators //#B
    {
        public const string RavenEventIdStart  //#C
            = "EfCoreInAction.NoSql.RavenDb";  //#C
        private readonly DocumentStore _store;
        private readonly ILogger _logger;

        public RavenStore(string connectionString, //#D
            ILogger logger)                        //#D
        {
            if (string.IsNullOrEmpty(connectionString))//#E
                return;                                //#E
            _logger = logger;

            var store = new DocumentStore();              //#F
            store.ParseConnectionString(connectionString);//#F
            store.Initialize();                           //#F

            //Add indexes if not already present
            new BookById().Execute(store);         //#G
            new BookByActualPrice().Execute(store);//#G
            new BookByVotes().Execute(store);      //#G

            _store = store; //#H
        }

        public INoSqlUpdater CreateNoSqlUpdater()    //#I
        {                                            //#I
            return new RavenUpdater(_store, _logger);//#I
        }                                            //#I

        public INoSqlAccessor CreateNoSqlAccessor()       //#J
        {                                                 //#J
            return new RavenBookAccesser(_store, _logger);//#J
        }                                                 //#J
    }
    /********************************************************
    #A This is the primary class to access the RavenDB store. There should only be one instance of this class in an application
    #B This interface defines the two creator methods: CreateNoSqlUpdater and CreateNoSqlAccessor
    #C I use this EventId name when logging accesses. It allows my logging display to mark these as database accesses
    #D The RavenStore needs the RavenDB connection string, and a logger
    #E To stop the application from throwing an exception on startup if there is no connection string I just leave the store as null. I can throw a better exception later on
    #F These are RavenDB commands to initialize the database
    #G These ensure the indexes the application needs have been created
    #H I save the store ready for the calls to the Create... methods
    #I This method returns a class which matches the INoSqlUpdater interface. This contains methods to create, update and delete a BookListNoSql item
    #J This method returns a class which matches the INoSqlAccessor interface. This has a method to create a context (session in RavenDB terms) and then gain LINQ access to the BookListNoSql items
     * ******************************************************/
}