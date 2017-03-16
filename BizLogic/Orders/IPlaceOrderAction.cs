using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;

namespace BizLogic.Orders
{
    public interface IPlaceOrderAction : IBizAction<PlaceOrderInDto, Order> { }
}