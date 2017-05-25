using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Test.Chapter09Listings.EfCode
{
    public class Notification2Entity : 
        INotifyPropertyChanged, 
        INotifyPropertyChanging //#A
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void SetWithNotify<T>(T value, ref T field, 
            [CallerMemberName] string propertyName = "")
        {
            if (!Object.Equals(field, value))
            {
                PropertyChanging?.Invoke(this, //#B
                    new PropertyChangingEventArgs(propertyName));
                field = value; //#C
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /*****************************************************
    #A I have added the extra interface, INotifyPropertyChanging
    #B Now must to trigger an event before the property is changed
     * ******************************************************/
}