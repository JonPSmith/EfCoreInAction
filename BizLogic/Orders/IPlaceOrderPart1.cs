using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;

namespace BizLogic.Orders
{
    public interface IPlaceOrderPart1 : IBizAction<PlaceOrderInDto, Part1ToPart2Dto> { }
}