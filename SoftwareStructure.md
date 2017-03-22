# The structure of the software

This is an overview of the structure of the software in this GitHub repo.

The repo is split up into branches, with each branch containing a working application.
You need to select the branch in Visual Studio and `Pull` the branch you want to work with.

You have three parts you can look at: 

1. A console application that is fairly simple, and a good place to start if you are new to .NET
2. An ASP.NET Core application, which is much more complex and is meant to be similar to real-world applications.
3. A `Test` project, which contains numerous unit tests.

Read the notes below for more details on each of these three.

## 1. Simple console application

If you want a really simple application to play with then select the `Chapter01` branch.
This contains a project called `MyFirstEfCoreApp`, which is a console application and 
fairly easy to run and/or change. You can see it in action in Chapter 1 of the book. 

The files to look at are:
* [Program.cs](https://github.com/JonPSmith/EfCoreInAction/blob/Chapter01/MyFirstEfCoreApp/Program.cs)
which has the code to read the commands that you type in.
* [Commands.cs](https://github.com/JonPSmith/EfCoreInAction/blob/Chapter01/MyFirstEfCoreApp/Commands.cs)
which contains each of the commands that the access the database.
* [AppDbContext.cs](https://github.com/JonPSmith/EfCoreInAction/tree/Chapter01/MyFirstEfCoreApp)
which contains the application's AppDbContext.
* `Book.cs` and `Author.cs` contain the entity classes that I use with the database.
* `MyLoggingProvider.cs` is used to log the SQL commands (which I show in chapter 1).

## 1. The ASP.NET Core application - an example book selling site 

This is a multi-project application. The figure below shows you the whole application after chapter 4
(before chapter 4 then the `BizLogic` and `BizDbAccess` projects aren't there).

![Diagram of site](https://github.com/JonPSmith/EfCoreInAction/blob/master/ExampleBookSellingSiteStructure.png)

I think the diagram above shows the various parts, and in chapter 2, section 
**2.7.2	Introducing the architecture of the book selling site application**
I explain the `DataLayer`, `ServiceLayer` and `EfCoreInAction` (ASP.NET Core, presentation layer) projects.
In chapter 4, section 
**4.3	A design pattern to help you implement business logic**
I explain the two extra layers, `BizLogic` (pure business logic) and `BizDbAccess` (business database access) projects.

### Directory structure of this application

#### DataLayer

The directories are:
* `EfClasses` contains all the entity classes used in the database.
* `EfCore` which contains all important 
[EfCoreContext.cs](https://github.com/JonPSmith/EfCoreInAction/blob/Chapter02/DataLayer/EfCode/EfCoreContext.cs),
which is the application's DbContext.
* `Migrations` is a directory created by EF Core to handle database migrations (see chapter 2 for a quick overview).
* `QueryObjects` holds the genric paging *query object*, which I describe in chapter 2.

### ServiceLayer

This project goes as the book progresses. It has a series of top-level directories.
The ones to look at end with `...Service`, as they contain the various classes that 
are used in the ASP.NET Core presentation layer to query or add, update or delete data.

These `...Service` directories hold *DTOs* (Data Transfer Objects), which I explain in chapter 2.
They can also contain the following sub-directories:
* `Concrete`, which holds the classes that hold the code that is executed
* `QueryObjects`, which hold *query objects*, which I describe in chapter 2.

### EfCoreInAction (ASP.NET Core, presentation layer)

This follows the normal ASP.NET Core structure.
This book isn't about ASP.NET Core - if you want to learn more about ASP.NET Core then please look at 
[ASP.NET Core documentation](https://docs.microsoft.com/en-us/aspnet/core/).

## 3. Test - unit test project

This contains loads of unit tests. Every listing, and many of the facts, in the book are tested in this project. 
I use [xUnit](https://xunit.github.io/) as the testing frameword with the fluent asserts.

The tests are in a directory called `UnitTests`, with sub-directories for each project in the solution.
Each of the test classes starts with the branch they refer to, for instance a test class
whoes name starts with `Ch02_` comes from the branch `Chapter02`. 

This project is fairly complicated and I'm not going explain it, but if you know about iunit tests
then there is a lot of learning to be had looking at the tests.






