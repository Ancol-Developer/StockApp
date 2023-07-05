using Entities;
using Entities.DTO;
using Microsoft.EntityFrameworkCore;
using Repository;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;

        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }
        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            if (buyOrderRequest == null)
            {
                throw new ArgumentNullException(nameof(buyOrderRequest));
            }
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            BuyOrder buyOrder= buyOrderRequest.ToBuyOrder();
            buyOrder.BuyOrderID = Guid.NewGuid();
            await _stockRepository.CreateBuyOrder(buyOrder);
            return buyOrder.ToBuyOrderResponse();
        }

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            if(sellOrderRequest == null)
            {
                throw new ArgumentNullException(nameof(sellOrderRequest));
            }
            sellOrderRequest.DateAndTimeOfOrder= DateTime.Now;
            SellOrder sellOrder = sellOrderRequest.ToSellOrder();
            sellOrder.SellOrderID = Guid.NewGuid();
            await _stockRepository.CreateSellOrder(sellOrder);
            return sellOrder.ToSellOrderResponse();
        }

        public async Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            List<BuyOrder> list_buyOrder = await _stockRepository.GetBuyOrders();
            return list_buyOrder.Select(temp => temp.ToBuyOrderResponse()).ToList();
        }

        public async Task<List<SellOrderResponse>> GetSellOrders()
        {
            List<SellOrder> list_sellOrder = await _stockRepository.GetSellOrders();
            return list_sellOrder.Select(temp => temp.ToSellOrderResponse()).ToList();
        }
    }
}
