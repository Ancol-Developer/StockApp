using StockApp.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class SellOrderRequest
    {
        [Required]
        public string StockSymbol { get; set; }
        [Required]
        public string StockName { get; set; }
        [OrderAndSellDateTimeValidation("Jan 01,2000")]
        public DateTime DateAndTimeOfOrder { get; set; }
        [Range(1,100000)]
        public uint Quantity { get; set; }
        [Range(1,10000)]
        public double Price { get; set; }
        public SellOrder ToSellOrder()
        {
            return new SellOrder
            {
                StockSymbol = StockSymbol,
                StockName = StockName,
                DateAndTimeOfOrder = DateAndTimeOfOrder,
                Quantity = Quantity,
                Price = Price
            };
        }
    }
}
