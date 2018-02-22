// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using Test.Chapter10Listings.MappingClasses;
using Test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch10_BlogPostTag 
    {
        private readonly ITestOutputHelper _output;

        public Ch10_BlogPostTag(ITestOutputHelper output)
        {
            _output = output;
        }

        private const string AdaName = "Ada Lovelace";
        private const string SherlockName = "Sherlock Holmes";
        private static readonly DateTime DateForPosts = new DateTime(2018, 2, 18);

        private List<Post> SetupPosts()
        {
            var aboutMe = new Tag {Name = "About Me"};
            var programming = new Tag {Name = "Programming"};
            var myOpinion = new Tag {Name = "My Opinion"};
            var adaLovelace = new Blogger {Name = AdaName, EmailAddress = "ada@nospam.com"};

            var post1 = new Post {Title = "About Ada Loverlace", Blogger = adaLovelace, LastUpdated = DateForPosts };
            var post2 = new Post {Title = "Thoughts on programming with a spanner ", Blogger = adaLovelace, LastUpdated = DateForPosts };
            var post3 = new Post
            {
                Title = "About Sherlock Holmes",
                LastUpdated = DateForPosts,
                Blogger = new Blogger {Name = SherlockName, EmailAddress = "sherlock@nospam.com"}
            };

            post1.TagLinks = new List<PostTag>{ new PostTag{ Post = post1, Tag = aboutMe}};
            post2.TagLinks = new List<PostTag> { new PostTag { Post = post2, Tag = programming }, new PostTag { Post = post2, Tag = myOpinion } };
            post3.TagLinks = new List<PostTag> { new PostTag { Post = post3, Tag = aboutMe } };

            return new List<Post> {post1, post2, post3};
        }

        [Fact]
        public void FillDatabaseWithTestsData()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.AddRange(SetupPosts());
                context.SaveChanges();

                //VERIFY
                context.Posts.Count().ShouldEqual(3);
                context.Bloggers.Count().ShouldEqual(2);
                context.Tags.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestListBloggersEagerLoading()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var list = context.Bloggers
                    .Include(x => x.Posts)
                    .ToList();

                //VERIFY
                list.Count.ShouldEqual(2);
                list.Select(x => x.Name).ShouldEqual(new[] { AdaName, SherlockName });
                list.Select(x => x.Posts.Count).ShouldEqual(new[] { 2, 1 });

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestListBloggersDtoByHand()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var list = context.Bloggers.Select(x => 
                    new ListBloggersDto
                {
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    PostsCount = x.Posts.Count
                }).ToList();

                //VERIFY
                list.Count.ShouldEqual(2);
                list.Select(x => x.Name).ShouldEqual(new[] { AdaName, SherlockName });
                list.Select(x => x.PostsCount).ShouldEqual(new[] { 2, 1 });

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestListBloggersDto()
        {
            //SETUP
            var mapperConfig = AutoMapperHelpers.MapperConfig<ListBloggersDto>();
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var dtos = context.Bloggers
                    .ProjectTo<ListBloggersDto>(mapperConfig)
                    .ToList();

                //VERIFY
                dtos.Count.ShouldEqual(2);
                dtos.Select(x => x.Name).ShouldEqual(new[] { AdaName, SherlockName });
                dtos.Select(x => x.PostsCount).ShouldEqual(new[] { 2,1 });

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestListPostEagerloading()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var posts = context.Posts
                    .Include(r => r.Blogger)
                    .Include(r => r.TagLinks).ThenInclude(r => r.Tag)
                    .ToList();

                //VERIFY
                posts.Count.ShouldEqual(3);
                posts.Select(x => x.Blogger.Name).ShouldEqual(new[] { AdaName, AdaName, SherlockName });
                posts.All(x => x.LastUpdated == DateForPosts).ShouldBeTrue();
                posts.Select(x => x.TagLinks.Count).ShouldEqual(new[] { 1, 2, 1 });

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestListPostDtoByHand()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var dtos = context.Posts.Select(x => new ListPostsDto
                {
                    BloggerName = x.Blogger.Name,
                    Title = x.Title,
                    LastUpdated = x.LastUpdated,
                    TagNames = x.TagLinks.Select(y => y.Tag.Name).ToList()
                }).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(3);
                dtos.Select(x => x.BloggerName).ShouldEqual(new[] { AdaName, AdaName, SherlockName });
                dtos.All(x => x.LastUpdated == DateForPosts).ShouldBeTrue();
                dtos.Select(x => x.TagNames.Count).ShouldEqual(new[] { 1, 2, 1 });

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestListPostsDto()
        {
            //SETUP
            var mapperConfig = AutoMapperHelpers.MapperConfig<ListPostsDto>();
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var dtos = context.Posts
                    .ProjectTo<ListPostsDto>(mapperConfig)
                    .ToList();

                //VERIFY
                dtos.Count.ShouldEqual(3);
                dtos.Select(x => x.BloggerName).ShouldEqual( new []{AdaName, AdaName, SherlockName});
                dtos.All(x => x.LastUpdated == DateForPosts).ShouldBeTrue();
                dtos.Select(x => x.TagNames.Count).ShouldEqual(new []{1,2,1});

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestListPostsWithSubDto()
        {
            //SETUP
            var config = AutoMapperHelpers.MapperConfig<ListPostsWithSubDto>();
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(SetupPosts());
                context.SaveChanges();
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                //ATTEMPT
                var dtos = context.Posts.ProjectTo<ListPostsWithSubDto>(config).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(3);
                dtos.Select(x => x.BloggerName).ShouldEqual(new[] { AdaName, AdaName, SherlockName });
                dtos.All(x => x.LastUpdated == DateForPosts).ShouldBeTrue();
                dtos.Select(x => x.TagLinks.Count).ShouldEqual(new[] { 1, 2, 1 });

                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

    }
}
