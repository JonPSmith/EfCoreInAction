// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public static class PaymentConfig
    {
        public static void Configure
            (this EntityTypeBuilder<Payment> entity)
        {
            entity.HasDiscriminator(b => b.PType) //#A
                .HasValue<PaymentCash>(PTypes.Cash) //#B
                .HasValue<PaymentCard>(PTypes.Card); //#C
        }
    }
    /*******************************************
    #A The HasDiscriminator method idetifies the entity as a TPH and then selects the property PType as the discriminator for the different types. In this case it is an enum, which I have set to be byte in size
    #B This sets the discriminator value for the PaymentCash type
    #C This sets the discriminator value for the PaymentCard type
        * *******************************************/
}