using System;

namespace Test.Chapter09Listings.EfClasses
{
    public class LoggedEntity : IWhenWho //#A
    {
        public int LoggedEntityId { get; set; }

        public string MyString { get; set; }

        public DateTime CreatedOn { get; private set; } //#B
        public string CreatedBy { get; private set; }   //#B
        public DateTime UpdatedOn { get; private set; } //#B
        public string UpdatedBy { get; private set; }   //#B

        public void SetWhenWhere //#C
            (Func<string> getUserName, bool add)
        {
            if (getUserName == null) //#D
                throw new ArgumentNullException
                    (nameof(getUserName));

            var user = getUserName(); //#E
            var time = DateTime.UtcNow; //#E
            if (add)
            {
                CreatedOn = time; //#F
                CreatedBy = user; //#F
            }
            UpdatedOn = time; //#G
            UpdatedBy = user; //#G
        }
    }
    /****************************************************************
    #A The entity class inherits the interface IWhenWho, which means any add/update of the entity is logged
    #B These four properties are required by the IWhenWho interface. They all have private setters to stop software changing them, but it still allows EF Core to fill them in when the entity is loaded
    #C This is the method required by the IWhenWho interface. Its job is to set the four IWhenWho properties appropriately
    #D I check that the getUserName method is provided
    #E I obtain the userId and the current time here so that an add will have the same values in both the Created and Updated properties
    #F If it is an add I set the Created properties
    #G I always set the Updated properties
     * ************************************************************/
}