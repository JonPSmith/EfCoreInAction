# EfCoreInAction

Welcome to the Git repo that is associated with the book 
**[Entity Framework Core in Action](https://www.manning.com/books/entity-framework-core-in-action?a_aid=su4utaraxuTre8tuthup&a_bid=4cef27ce)**
published by [Manning Publications](https://www.manning.com/).
This book details how to use 
[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index) (EF Core)
to develop database access code in [.NET Core](https://www.microsoft.com/net) applications. 

This Git repo contains all the code in the book, plus an 
[example book selling site](http://efcoreinaction.com/) 
that I develop, and improve, in the book.
All the code uses Microsoft's, open-source 
[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index) library for database access.

## NOTE: Now updated to .NET Core 2.0 release

I have extensively updated the  Git repo https://github.com/JonPSmith/EfCoreInAction and each branch is now at the new .NET Core 2.0 release level. There are lots of changes to the code to match both changes in EF Core 2.0.0 and ASP.NET Core 2.0.0.
To run the changed Git repo code you need to:
1.	Install .NET Core 2.0 SDK – go to https://www.microsoft.com/net/download/core and select the correct SDK for your system
2.	I recommend updating to Visual Studio 2017 15.3 as that has the new templates for ASP.NET Core 2.0.0. (I think my Git repo will work with Visual Studio 2017 15.2, but I haven’t tried it).

### WARNING. 
If you have already run any of the unit tests prior to the 2.0.0 update you do need to delete all the tests databases, as they are out of date and will throw errors. There is a unit test built specifically to do this called `DeleteAllTestDatabasesOk` in the class `Test.UnitCommands.DeleteAllUnitTestDatabases`, which you must run in debug mode.

# Where's the code!

This repo has a branch-per-chapter, and sometime and branch-per-section, approach
so if you are looking at the master branch then you won't see any code!
Just click the [Branches](https://github.com/JonPSmith/EfCoreInAction/branches) on the site and you will
find all the branches I have created so far.

## Map of the branches

The branches mainly inhert from each other (shown as `-->` in the diagram)
but a few are on there own, like `master` and `Chapter01` (shown with spaces between them).
The ones that are build for deployment to a web site are branches off the main stem (shown as higher offshoots).
I did that because I built the normal code so you can run it locally without migrations.  
**Note: to fit the diagram in I shorten the real branch names and use `Ch4` instead of `Chapter04`**

### Part 1 - Getting started
```
                                        Ch5Migrate
                                          /
master    Ch1    Ch2 --> Ch3 --> Ch4 --> Ch5 -->
```
### Part 2 - Entity Framework in depth
```
(ch5) --> Ch6 --> Ch7 --> Ch8 --> Ch9
```
### Part 3 - Using Entity Framework in real-world applications
```
                                       Ch13-Part4
                                      /
                                     |     Ch13-Part3
                                     |        /
                                     |  Ch13-Part2    Ch14MySql
                                     |      /         /
(ch9) --> Ch10 --> Ch11 --> Ch12 --> Ch13-Part1 --> Ch14  --> (Ch15) Note1
```
*Note1: Chapter 15, on unit testing has its own Git repo https://github.com/JonPSmith/EfCore.TestSupport 
and a NuGet package called [EfCore.TestSupport](https://www.nuget.org/packages/EfCore.TestSupport/).*


## Live example book selling site

You can find a live verison of the [example books selling site](http://efcoreinaction.com/).
This provides an online example of the site to look at. You can't edit the data on the live site for obvious reasons,
but you can if you clone the git this Git repo and run the example application locally.
You can 'buy a book' though on the live site, and even see your old 'purchases'.

The **Logs** navigation button will show you the SQL code that EF Core used to implement the last command you used.

![Diagram of site](https://github.com/JonPSmith/EfCoreInAction/blob/master/ExampleBookSellingSite.png)

*Note: The admin button is missing on the live site.*

## Database naming conventions

Because the database structure can change in each branch I use a system of building the database name 
using the branch name from a file which is written by a small gulpfile that is run by Visual Studio #
on every build.

Also I use [xUnit unit tests](https://xunit.github.io/) which run in parallel, so I cannot use one
database for all the tests, as they would all be changing the database at one time.
I therefore use another system to append the test class name, and sometimes the method that ran.
**This is all explained in chapter 11**, but I wanted to warn you you will get lots of databases 
if you run the unit tests.

If you get fed up with all databases there is a unit test in 
[test/UnitCommands/DeleteAllUnitTestDatabases](https://github.com/JonPSmith/EfCoreInAction/blob/Chapter02/Test/UnitCommands/DeleteAllUnitTestDatabases.cs)
that will delete all the unit test databases in the branch you are in.

## Licence 

[MIT](https://github.com/JonPSmith/EfCoreInAction/blob/master/LICENSE)

## Other links
My twitter handle is @thereformedprog.  
My blog site is at http://www.thereformedprogrammer.net/ where you will find articles on EF 6.x
and some on EF Core.

Happy coding!