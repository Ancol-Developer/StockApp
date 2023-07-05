using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IStockRepository
    {
        Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder);
        Task<SellOrder> CreateSellOrder(SellOrder sellOrder);
        Task<List<BuyOrder>> GetBuyOrders();
        Task<List<SellOrder>> GetSellOrders();  
    }
}
