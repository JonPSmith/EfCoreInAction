// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch11_AddAttributeToPartial
    {
        public partial class Users
        {
            public int UserId { get; set; }
            [StringLength(100)]
            public string Email { get; set; }
        }

        [ModelMetadataType(typeof(Users_Validation))] //#A
        public partial class Users {}//#B

        public class Users_Validation //#C
        {
            [EmailAddress] //#D
            public string Email { get; set; } //#E
        }
        /*************************************************************
        #A This attribute, which can be found in the Microsoft.AspNetCore.Mvc namespace, allows me to attach another class, called Users_Validation,  which contains matching properties with data attributes on them
        #B I have to create another partial class that I can apply the ModelMetadataType attribute to. I do this because I don't want to edit the partial class created via the scaffold command, which I may need to delete and recreate if the database changes
        #C This is the class that contains properties that match the class I am trying to add data attributes to
        #D I have added the EmailAddress attribute, which will make ASP.NET Core check that the input to this property matches the format of an email address
        #E This property matches one in the class, Users. ASP.NET Core can combine the attributes in the properties in the original class, and the properties in the class provided via the ModelMetadataType attribute
         * ***********************************************************/

        [Fact]
        public void TestModelMetadataTypePartialGetAttributes()
        {
            //SETUP
            var propInfo = typeof(Users).GetProperty(nameof(Users.Email));

            //ATTEMPT
            var modelAttrs = ModelAttributes.GetAttributesForProperty(typeof(Users), propInfo);

            //VERIFY
            modelAttrs.PropertyAttributes[0].ShouldBeType<StringLengthAttribute>();
            modelAttrs.PropertyAttributes[1].ShouldBeType<EmailAddressAttribute>();
        }
    }
}