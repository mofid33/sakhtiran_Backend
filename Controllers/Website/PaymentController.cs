using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.UserOrder;
using Microsoft.Extensions.Configuration;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using FSS.Pipe;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Dtos.ShopPlan;

namespace Controllers.Controllers
{
    public class PaymentController : Controller
    {
        public IUserOrderRepository _userOrderRepository { get; }
        public IShopService _shopService { get; }
        public IUserOrderService _userOrderService { get; }
        public IConfiguration Configuration { get; }

        public PaymentController(
        IUserOrderRepository userOrderRepository,
        IShopService shopService,
        IUserOrderService userOrderService,
        IConfiguration Configuration)
        {
            this._userOrderRepository = userOrderRepository;
            this._shopService = shopService;
            this._userOrderService = userOrderService;
            this.Configuration = Configuration;
        }
        [HttpPost]
        [Route("Gateway/index")]
        public async Task<IActionResult> Index()
        {


            var httpcontext = HttpContext.Request;


            // mellat

            var refId = httpcontext.Form["RefId"].ToString();
            var resCode = httpcontext.Form["ResCode"].ToString();
            var saleOrderId = httpcontext.Form["saleOrderId"].ToString();
            var saleReferenceId = httpcontext.Form["SaleReferenceId"].ToString();

            if (!string.IsNullOrWhiteSpace(saleReferenceId) && resCode == "0")
            {
                await _userOrderService.PayOrderMellatCheck(refId, resCode, saleOrderId, saleReferenceId);
            }
                ViewBag.transactionId = refId;
                return View();
        
        }


        [HttpPost]
        [Route("Gateway/plan")]
        public async Task<IActionResult> Plan()
        {



            var httpcontext = HttpContext.Request;


            // mellat

            var refId = httpcontext.Form["RefId"].ToString();
            var resCode = httpcontext.Form["ResCode"].ToString();
            var saleOrderId = httpcontext.Form["saleOrderId"].ToString();
            var saleReferenceId = httpcontext.Form["SaleReferenceId"].ToString();

            if (!string.IsNullOrWhiteSpace(saleReferenceId) && resCode == "0")
            {
                await _shopService.PayPlanPayment(refId, resCode, saleOrderId, saleReferenceId);
                ViewBag.transactionId = refId;

            }
                return View();
        
 

        }




    }
}