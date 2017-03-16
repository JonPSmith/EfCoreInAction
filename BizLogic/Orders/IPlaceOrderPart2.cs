using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;

namespace BizLogic.Orders
{
    public interface IPlaceOrderPart2 : IBizAction<Part1ToPart2Dto, Order> { }
}