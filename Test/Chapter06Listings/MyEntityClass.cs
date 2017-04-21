using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter06Listings
{
    public class MyEntityClass
    {
        public int MyEntityClassId { get; set; }

        [Required]
        public string InDatabaseProp { get; set; } //#A

        public string InternalSet { get; internal set; } //#B

        public string PrivateSet { get; private set; } //#C

        public string InternalGet { internal get; set; } //#D

        public string PrivateGet { private get; set; } //#E

        public int InDatabasePropLength => 
            InDatabaseProp.Length; //#F

        [NotMapped] //#G
        public string LocalString { get; set; }

        public ExcludeClass LocalClass { get; set; } //#H


    }

    [NotMapped] //#I
    public class ExcludeClass
    {
        public int LocalInt { get; set; }
    }
    /***********************************************************
    #A Automatically included: a normal public property, with public getter and setter
    #B Automatically included: a public property, with the setter with an internal access modifier
    #C Automatically included: a public property, with the setter with an private access modifier
    #D Automatically excluded: a public property, with the getter with an internal access modifier
    #E Automatically excluded: a public property, with the getter with an private access modifier
    #F Automatically excluded: a property is automatically excluded because it has no public setter
    #G Excluded: Placing a [NotMapped] attribute tells EF Core to not map this property to a column in the database
    #H Excluded: This class will not be included in the database because the class definition has a [NotMapped] attribute on it
    #I Excluded: Placing a [NotMapped] attribute on the class definition tells EF Core that this class should not be mapped to the database if used in an entity class
     * ***********************************************************/
}
