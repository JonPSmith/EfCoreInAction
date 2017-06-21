using System;

namespace Test.Chapter09Listings.EfClasses
{
    public interface IWhen //#A
    {
        DateTime CreatedOn { get; } //#B
        DateTime UpdatedOn { get; } //#C

        void SetWhen(bool add); //#D
    }
    /*****************************************************
    #A This interface is added to any entity class that I want to when the entity was added or updated
    #B This holds the datetime when the entity was first added to the database
    #C This holds the datetime when the entity was last updated
    #D This method is called when an add or update to the entity is found. Its job is to update the properties based on the add flag
     * *****************************************************/
}