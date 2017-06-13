using System;

namespace Test.Chapter09Listings.EfClasses
{
    public interface IWhenWho //#A
    {
        string CreatedBy { get; }   //#B
        DateTime CreatedOn { get; } //#B

        string UpdatedBy { get; }   //#C
        DateTime UpdatedOn { get; } //#C

        void SetWhenWhere //D
            (Func<string> getUserName, bool add);
    }
    /*****************************************************
    #A This interface is added to any entity class that I want to log changes on
    #B These two properties hold the UserId of the user who created the original entity, and when that happened
    #C These two properties hold the UserId of the user who updated the entity, and when that happened
    #D This method must set the four properties. It takes a function that will get the UserId, and the second parameter is true if an add, false for an update
     * *****************************************************/
}