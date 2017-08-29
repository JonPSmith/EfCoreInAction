// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_DeleteBehaviour
    {
        [Fact]
        public void TestCreateDeletePrincipalNoDependentsOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DeletePrincipal();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDeletePrincipalDefaultOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Database.EnsureCreated();

                var entity = new DeletePrincipal {DependentDefault = new DeleteDependentDefault()};
                //Guard test - check the deafault delete behaviour for nullable key is ClientSetNull
                context.Model.FindEntityType(entity.DependentDefault.GetType().FullName)
                    .GetForeignKeys().Single().DeleteBehavior.ShouldEqual(DeleteBehavior.ClientSetNull);
                context.Add(entity);
                context.SaveChanges();


                //ATTEMPT
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentDefault>().Count().ShouldEqual(1);              
            }
        }

        [Fact]
        public void TestDeletePrincipalSetNullOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();
                var entity = new DeletePrincipal { DependentSetNull = new DeleteDependentSetNull() };
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentSetNull>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDeletePrincipalRestrictOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();
                var entity = new DeletePrincipal { DependentRestrict = new DeleteDependentRestrict() };
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(entity);
                var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.ShouldEqual("The association between entity types 'DeletePrincipal' and 'DeleteDependentRestrict' has been severed but the foreign key for this relationship cannot be set to null. If the dependent entity should be deleted, then setup the relationship to use cascade deletes.");
            }
        }

        [Fact]
        public void TestDeletePrincipalCascadeOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();
                var entity = new DeletePrincipal { DependentCascade = new DeleteDependentCascade() };
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentCascade>().Count().ShouldEqual(0);
            }
        }

    }
}