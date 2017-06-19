using System;

namespace Test.Chapter09Listings.EfClasses
{
    public class AutoWhenEntity : IWhen //#A
    {
        public int AutoWhenEntityId { get; set; }

        public string MyString { get; set; }

        public DateTime CreatedOn { get; private set; } //#B
        public DateTime UpdatedOn { get; private set; } //#B

        public void SetWhen (bool add) //#C
        {
            var time = DateTime.UtcNow; //#D
            if (add)
            {
                CreatedOn = time; //#E
            }
            UpdatedOn = time; //#F
        }
    }
    /****************************************************************
    #A The entity class inherits the interface IWhen, which means any add/update of the entity is logged
    #B These two properties are required by the IWhen interface. They all have private setters to stop software changing them, but it still allows EF Core to fill them in when the entity is loaded
    #C This is the method required by the IWhen interface. Its job is to set the two IWhen properties appropriately
    #D I obtain the current time here so that an add will have the same values in both the Created and Updated properties
    #E If it is an add I set the Created properties
    #F I always set the Updated properties
     * ************************************************************/
}