using Microsoft.AspNetCore.Mvc;
using my_eshop_api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace my_eshop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ItemContext _context;

        public OrderController(ItemContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetOrder(int id)
        {
            var order = await this._context.Orders.FindAsync(id);
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Item>> Post([FromBody] OrderDTO dto)
        {
            try
            {
                var NewOrder = await CreateOrderFromDTO(dto);

                if (!TryValidateModel(NewOrder, nameof(Order)))
                {
                    return BadRequest(ModelState);
                }

                await this._context.Orders.AddAsync(NewOrder);
                await this._context.SaveChangesAsync();

                var returnDTO = CreateDTOFromOrder(NewOrder);
                return CreatedAtAction(nameof(GetOrder), new { id = returnDTO.Id }, returnDTO);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        private async Task<Order> CreateOrderFromDTO(OrderDTO dto)
        {
            var NewOrder = new Order();
            NewOrder.OrderDetails = new List<OrderDetail>();

            NewOrder.UserId = dto.UserId;
            NewOrder.OrderDate = DateTime.Now;

            var tempAddr = await this._context.Addresses.FindAsync(dto.DeliveryAddressId);
            NewOrder.FirstName = tempAddr.FirstName;
            NewOrder.LastName = tempAddr.LastName;
            NewOrder.Street = tempAddr.Street;
            NewOrder.Zip = tempAddr.Zip;
            NewOrder.City = tempAddr.City;
            NewOrder.Country = tempAddr.Country;

            decimal tempTotalPrice = 0;
            foreach (var detail in dto.OrderDetails)
            {
                var NewOrderDetail = new OrderDetail();
                var tempItem = await this._context.Items.FindAsync(detail.ItemId);
                NewOrderDetail.ItemId = detail.ItemId;
                NewOrderDetail.ItemName = tempItem.Name;
                NewOrderDetail.ItemUnitPrice = tempItem.Price;
                NewOrderDetail.Quantity = detail.Quantity;
                NewOrderDetail.TotalPrice = tempItem.Price * detail.Quantity;

                NewOrder.OrderDetails.Add(NewOrderDetail);
                tempTotalPrice += NewOrderDetail.TotalPrice;
            }
            NewOrder.TotalPrice = tempTotalPrice;
            return NewOrder;
        }

        private OrderDTO CreateDTOFromOrder(Order order)
        {
            var dto = new OrderDTO();
            dto.OrderDetails = new List<OrderDetailDTO>();

            dto.Id = order.Id;
            dto.UserId = order.UserId;
            dto.OrderDate = order.OrderDate;

            dto.FirstName = order.FirstName;
            dto.LastName = order.LastName;
            dto.Street = order.Street;
            dto.Zip = order.Zip;
            dto.City = order.City;
            dto.Country = order.Country;

            foreach (var detail in order.OrderDetails)
            {
                var dtoDetail = new OrderDetailDTO();
                dtoDetail.Id = detail.Id;
                dtoDetail.OrderId = detail.OrderId;
                dtoDetail.ItemId = detail.ItemId;
                dtoDetail.ItemName = detail.ItemName;
                dtoDetail.ItemUnitPrice = detail.ItemUnitPrice;
                dtoDetail.Quantity = detail.Quantity;
                dtoDetail.TotalPrice = detail.TotalPrice;

                dto.OrderDetails.Add(dtoDetail);
            }
            dto.TotalPrice = order.TotalPrice;
            return dto;
        }
    }
}
