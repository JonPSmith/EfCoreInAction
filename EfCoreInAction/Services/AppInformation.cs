// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreInAction.Services
{
    public class AppInformation
    {
        public AppInformation(string gitBranchName)
        {
            GitBranchName = gitBranchName;
        }

        public string GitBranchName { get; private set; }

        public IEnumerable<Tuple<string, string>> GetAssembliesInfo()
        {
            var efCore = typeof(DbContext).GetTypeInfo().Assembly.GetName();
            yield return new Tuple<string, string>(efCore.Name, efCore.Version.ToString());
            var aspNetCore = typeof(WebHostBuilder).GetTypeInfo().Assembly.GetName();
            yield return new Tuple<string, string>(aspNetCore.Name, aspNetCore.Version.ToString());
            var netCore = typeof(Program).GetTypeInfo().Assembly;
            yield return new Tuple<string, string>("Targeted .NET Core", netCore.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName);
        }
    }
}