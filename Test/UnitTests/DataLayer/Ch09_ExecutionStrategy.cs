// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using test.Helpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_ExecutionStrategy 
    {
        private readonly ITestOutputHelper _output;
        private DbContextOptions<Chapter09DbContext> _options;

        public Ch09_ExecutionStrategy(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter09DbContext>();

            optionsBuilder.UseSqlServer(connection,
                options => options.EnableRetryOnFailure());
            _options = optionsBuilder.Options;
        }

        [Fact]
        public void TestSetupSqlExecutionStrategyOk()
        {
            //SETUP
            using (var context = new Chapter09DbContext(_options))
            {
                context.Database.EnsureCreated();
                var unique = Guid.NewGuid().ToString();
                //ATTEMPT
                var entity = new MyEntity {MyString = unique };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.AsNoTracking().Single(x => x.Id == entity.Id).MyString.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestSqlExecutionStrategyTransactionBad()
        {
            //SETUP
            Exception ex;
            using (var context = new Chapter09DbContext(_options))
            {
                context.Database.EnsureCreated();
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Add(new MyEntity());

                    //ATTEMPT
                    ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
                }
            }

            //VERIFY
            ex.Message.StartsWith("The configured execution strategy 'SqlServerRetryingExecutionStrategy' does not support user initiated transactions. ").ShouldBeTrue();
        }

        [Fact]
        public void TestSqlExecutionStrategyTransactionOk()
        {
            //SETUP
            using (var context = new Chapter09DbContext(_options))
            {
                context.Database.EnsureCreated();
                var numEnts = context.MyEntities.Count(); //!!!!!!!!!!!!!!!!!! REMOVE in book listing

                var strategy = context.Database
                    .CreateExecutionStrategy(); //#B
                //ATTEMPT
                strategy.Execute(() => //#C
                {
                    try
                    {
                        using (var transaction = context
                            .Database.BeginTransaction()) //#D
                        {
                            context.Add(new MyEntity());
                            context.SaveChanges();       
                            context.Add(new MyEntity()); 
                            context.SaveChanges();       

                            transaction.Commit();        
                        }
                    }
                    catch (Exception e)
                    {
                        //Error handling to go here
                        throw;
                    }
                });

            //VERIFY
            context.MyEntities.Count().ShouldEqual(numEnts+2);//!!!!!!!!!!!!!!!!!! REMOVE in book listing
            }
            /********************************************************************
            #A I configure the database to use the SQL execution strategy. This means I have to handle transactions differently
            #B I create and IExecutionStrategy instance, which uses the execution strategy I configured the DbContext with
            #C The important thing is to make the whole transaction code into an Action method it can call
            #D The rest of the transaction setup and running your code is the same
             * *****************************************************************/
        }

        public class MyExecutionStrategy : IExecutionStrategy
        {
            public TResult Execute<TState, TResult>(Func<TState, TResult> operation, Func<TState, ExecutionResult<TResult>> verifySucceeded, TState state)
            {
                throw new NotImplementedException();
            }

            public Task<TResult> ExecuteAsync<TState, TResult>(Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded, TState state,
                CancellationToken cancellationToken = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public bool RetriesOnFailure { get; }
        }

        [Fact]
        public void TestOwnExecutionStrategyConfigureOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter09DbContext>();

            optionsBuilder.UseSqlServer(connection,
                options => options.ExecutionStrategy(
                    p => new MyExecutionStrategy()));

            using (var context = new Chapter09DbContext(optionsBuilder.Options))
            {
                //Cannot use context as MyExecutionStrategy is not implemented 
            }
        }
    }
}
