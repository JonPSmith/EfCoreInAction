using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter06Listings
{
    public class MyEntityClass
    {
        public int MyEntityClassId { get; set; }

        [Required]
        public string InDatabaseProp { get; set; }

        public int InDatabasePropLength => 
            InDatabaseProp.Length; //#A

        [NotMapped] //#B
        public string LocalString { get; set; }

        public ExcludeClass LocalClass { get; set; } //#C
    }

    [NotMapped] //#D
    public class ExcludeClass
    {
        public int LocalInt { get; set; }
    }
    /***********************************************************
    #A This property is automatically excluded because it has no public setter
    #B Placing a [NotMapped] attribute tells EF Core to not map this property to a column in the database
    #C This class will not be included in the database because the class definition has a [NotMapped] attribute on it
    #D Placing a [NotMapped] attribute on the class definition tells EF Core that this class should not be mapped to the database if used in an entity class
     * ***********************************************************/
}
