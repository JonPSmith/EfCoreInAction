// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_Hierarchical
    {
        [Fact]
        public void TestEmployeeCreateOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Employee
                {
                    Name = "Employee1"
                };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.Employees.Count().ShouldEqual(1);
                context.Employees.First().EmployeeId.ShouldEqual(1);
                context.Employees.First().ManagerEmployeeId.ShouldBeNull();
            }
        }

        [Fact]
        public void TestEmployeeAndManagerCreateOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Employee
                {
                    Name = "Employee1",
                    Manager = new Employee {  Name = "Employee2"}
                };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.Employees.Count().ShouldEqual(2);
                var employees = context.Employees.Include(x => x.Manager).OrderBy(x => x.Name).ToList();
                employees[0].Manager.ShouldEqual(employees[1]);
                employees[1].Manager.ShouldBeNull();
            }
        }

        [Fact]
        public void TestDeleteManagerDefaultOnDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            int managerId;
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Employee
                {
                    Name = "Employee1",
                    Manager = new Employee {Name = "Employee2"}
                };
                context.Add(entity);
                context.SaveChanges();
                managerId = (int)entity.Manager.EmployeeId;
            }
            using (var context = new Chapter07DbContext(options))
            {
                var manager = context.Employees.Find(managerId);
                context.Remove(manager);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'FOREIGN KEY constraint failed'.");
            }
        }

        [Fact]
        public void TestDeleteManagerClientSetNullOnDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            int managerId;
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new EmployeeShortFk
                {
                    Name = "Employee1",
                    Manager = new EmployeeShortFk { Name = "Employee2" }
                };
                context.Add(entity);
                context.SaveChanges();
                managerId = (int) entity.Manager.EmployeeShortFkId;
            }
            using (var context = new Chapter07DbContext(options))
            {
                var manager = context.EmployeeShortFks.Find(managerId);
                context.Remove(manager);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'FOREIGN KEY constraint failed'.");
            }
        }

        //Use this to see what happens in sql server, which does not have RESTRICT in the same way Sqlite does
        [RunnableInDebugOnly]
        public void TestDeleteManagerSqlBad()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter07DbContext>();

            optionsBuilder.UseSqlServer(connection);
            int managerId;
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                var logger = new LogDbContext(context);
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Employee
                {
                    Name = "Employee1",
                    Manager = new Employee { Name = "Employee2" }
                };
                context.Add(entity);
                context.SaveChanges();
                managerId = (int)entity.Manager.EmployeeId;
            }
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                var manager = context.Employees.Find(managerId);
                context.Remove(manager);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldContain(
                    "The DELETE statement conflicted with the SAME TABLE REFERENCE constraint \"FK_Employees_Employees_ManagerEmployeeId\".");
            }
        }
    }
}