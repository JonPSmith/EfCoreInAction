// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Autofac;
using System.Reflection;

namespace ServiceLayer.Utils
{
    public class MyAutoFacModule : Autofac.Module //#A
    {
        protected override void Load( //#B
            ContainerBuilder builder) //#B
        {
            builder.RegisterAssemblyTypes( //#C
                GetType().GetTypeInfo().Assembly) //#D
                .Where(c => c.Name.EndsWith("Service")) //#E
                .AsImplementedInterfaces(); //#F
        }
    }
    /********************************************************
    #A I create a class that inherits from AutoFac's Moduler class
    #B I then have to override the method load
    #C I use the AuthoFac RegisterAssemblyTypes
    #D ... and give it the assembly I am in, which will be the Service Layer
    #E All my database access classes have a name ending in "Service" so I only pick those
    #F This registers all those classes with their interface
     * ******************************************************/
}