using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Test.Chapter09Listings.EfCode
{
    public class NotificationEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetWithNotify<T>(T value, ref T field, 
            [CallerMemberName] string propertyName = "") //#A
        {
            if (!Object.Equals(field, value)) //#B
            {
                field = value; //#C
                PropertyChanged?.Invoke(this, //#D
                    new PropertyChangedEventArgs(propertyName)); //#E
            }
        }
    }
    /*****************************************************
    #A This automatically gets the propertyName using the System.Runtime.CompilerServices
    #B Only if the field and the value are different do we set the field and raise the event
    #C I set the field to the new value
    #D Then I invoke the PropertyChanged event
    #E ... with the name of the property
     * ******************************************************/
}