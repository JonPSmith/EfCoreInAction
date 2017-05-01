using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter06Listings
{
    public class MyEntityClass
    {
        public int MyEntityClassId { get; set; }

        [Required]
        public string NormalProp { get; set; } //#A

        public string InternalSet { get; internal set; } //#B

        public string PrivateSet { get; private set; } //#C

        public string InternalGet { internal get; set; } //#D

        public string PrivateGet { private get; set; } //#E


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
    #A INCLUDED: a normal public property, with public getter and setter
    #B INCLUDED: a public property, with the setter with an internal access modifier
    #C INCLUDED: a public property, with the setter with an private access modifier
    #D EXCLUDED: a public property, with the getter with an internal access modifier
    #E EXCLUDED: a public property, with the getter with an private access modifier
    #F EXCLUDED: a property is automatically excluded because it has no setter
    #G EXCLUDED: Placing a [NotMapped] attribute tells EF Core to not map this property to a column in the database
    #H EXCLUDED: This property will be excluded because the class definition has a [NotMapped] attribute on it
    #I EXCLUDED: Placing a [NotMapped] attribute on the class definition tells EF Core that this class should not be mapped to the database if used in an entity class
     * ***********************************************************/
}
