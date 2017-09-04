// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure
            (EntityTypeBuilder<Payment> entity)
        {
            entity.HasDiscriminator(b => b.PType) //#A
                .HasValue<PaymentCash>(PTypes.Cash) //#B
                .HasValue<PaymentCard>(PTypes.Card); //#C

            //This is needed for TestChangePaymentTypeOk to work - see EF Core issue #7510
            entity.Property(p => p.PType)
                .Metadata.AfterSaveBehavior = PropertySaveBehavior.Save;
        }
    }
    /*******************************************
    #A The HasDiscriminator method idetifies the entity as a TPH and then selects the property PType as the discriminator for the different types. In this case it is an enum, which I have set to be byte in size
    #B This sets the discriminator value for the PaymentCash type
    #C This sets the discriminator value for the PaymentCard type
        * *******************************************/
}