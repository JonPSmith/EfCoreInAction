// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;

namespace Test.EfHelpers
{
    public static class AutoMapperHelpers
    {
        public static MapperConfiguration MapperConfig<T>() where T : Profile, new()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new T());
            });
            return config;
        }

        public static MapperConfiguration MapperConfig<T1, T2>() where T1 : Profile, new() where T2 : Profile, new()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new T1());
                cfg.AddProfile(new T2());
            });
            return config;
        }
    }
}