using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;
using prjFunShare_backend.Models.ManagerProduct;
using prjFunShare_backend.ViewModels;
using System.Linq;
using System.Text.Json;

namespace prjFunShare_backend.Controllers
{
    public class ManagerReoprtController : Controller
    {
        private readonly FUNShareContext _context;
        public ManagerReoprtController(FUNShareContext c)
        {
            _context = c;
        }
        public IActionResult Chart()
        {          
            IEnumerable<Supplier> datas = _context.Supplier.ToList();
            return View(datas);
        }
        public IActionResult ChartTest()
        {
            IEnumerable<Supplier> datas = _context.Supplier.ToList();
            return View(datas);
        }
        public IActionResult GetOrderDataChart(int year1, int year2)
        {
            // FunshareContext db = new FunshareContext();
            //廠商登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);

                var orderData = _context.Order
                    .Where(o => o.OrderTime.Year >= year1 && o.OrderTime.Year <= year2 && o.OrderDetail.FirstOrDefault().ProductDetail.Product.SupplierId == supplier.SupplierId)
                    .GroupBy(o => new { o.OrderTime.Year, o.OrderTime.Month })
                    .Select(g => new
                    {
                         年分 = g.Key.Year,
                         月份 = g.Key.Month,
                         總數 = g.Count()
                    })
                    .OrderBy(g => g.年分)
                    .ThenBy(g => g.月份)
                    .ToList();

                    return Json(orderData);
            }
            else 
            { 
                var orderData = _context.Order
                    .Where(o => o.OrderTime.Year >= year1 && o.OrderTime.Year <= year2)
                    .GroupBy(o => new { o.OrderTime.Year, o.OrderTime.Month })
                    .Select(g => new
                    {
                        年分 = g.Key.Year,
                        月份 = g.Key.Month,
                        總數 = g.Count()
                    })
                    .OrderBy(g => g.年分)
                    .ThenBy(g => g.月份)
                    .ToList();

                     return Json(orderData);
            }
        }
        public IActionResult GetGrowthRateMMDataChart()
        {
            //廠商登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);
               
                //月營收成長率 =（當月營收 – 上月營收）÷ 上月營收 x 100%
                var monthlySalesData = _context.OrderDetail
                        .Where(o => o.Order.OrderDetail.FirstOrDefault().ProductDetail.Product.SupplierId == supplier.SupplierId)
                        .GroupBy(x => new { x.Order.OrderTime.Year, x.Order.OrderTime.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Sales = g.Sum(x => x.ProductDetail.UnitPrice)
                        })
                        .OrderBy(g => g.Year)
                        .ThenBy(g => g.Month)
                        .ToList();

                var monthlyGrowthRates = new List<decimal>();

                for (int i = 1; i < monthlySalesData.Count; i++)
                {
                    decimal previousMonthSales = monthlySalesData[i - 1].Sales.GetValueOrDefault();//上個月
                    decimal currentMonthSales = monthlySalesData[i].Sales.GetValueOrDefault();//當月
                    decimal growthRate;

                    if (previousMonthSales != 0)
                        //年月營收成長率 =（要是有年後一直執行當月營收 – 上月營收）÷ 上月營收 x 100%
                        growthRate = (currentMonthSales - previousMonthSales) / previousMonthSales;
                    else
                        //空值回傳0
                        growthRate = 0;

                    monthlyGrowthRates.Add(growthRate);
                }

                return Json(new
                {
                    dates = monthlySalesData.Skip(1).Select(m => $"{m.Year}-{m.Month}").ToList(),//跳過第一年,沒得比較
                    growthRates = monthlyGrowthRates
                });
            }
            else
            {
                var monthlySalesData = _context.OrderDetail
                        .GroupBy(x => new { x.Order.OrderTime.Year, x.Order.OrderTime.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Sales = g.Sum(x => x.ProductDetail.UnitPrice)
                        })
                        .OrderBy(g => g.Year)
                        .ThenBy(g => g.Month)
                        .ToList();

                var monthlyGrowthRates = new List<decimal>();

                for (int i = 1; i < monthlySalesData.Count; i++)
                {
                    decimal previousMonthSales = monthlySalesData[i - 1].Sales.GetValueOrDefault();
                    decimal currentMonthSales = monthlySalesData[i].Sales.GetValueOrDefault();
                    decimal growthRate;

                    if (previousMonthSales != 0)
                        growthRate = (currentMonthSales - previousMonthSales) / previousMonthSales;
                    else
                        growthRate = 0;

                    monthlyGrowthRates.Add(growthRate);
                }

                return Json(new
                {
                    dates = monthlySalesData.Skip(1).Select(m => $"{m.Year}-{m.Month}").ToList(),
                    growthRates = monthlyGrowthRates
                });
            }
        }

        public IActionResult GetstockDataChart()
        {
            //廠商登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);

                var stockData = _context.Product
                .Include(pd => pd.ProductDetail)
                .Where(p => p.SupplierId == supplier.SupplierId)
                .SelectMany(pd => pd.ProductDetail)
                .Select(detail => new
                {
                    商品名稱 = detail.Product.ProductName,
                    已售數量 = detail.OrderDetail.Count(),
                    庫存量 = detail.Stock- detail.OrderDetail.Count()
                })
                .ToList();
                return Json(stockData);
            }
            else 
            {
                var stockData = _context.Product
                .SelectMany(pd => pd.ProductDetail)
                .Select(detail => new
                {
                    商品名稱 = detail.Product.ProductName,
                    已售數量 = detail.OrderDetail.Count(),
                    庫存量 = detail.Stock - detail.OrderDetail.Count()
                })
                .ToList();
                return Json(stockData);
            }
        }
        public IActionResult GetmomeyDataChart()
        {
            //這個版本目前可以
            //廠商登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);

                var salesData = _context.OrderDetail
                   .Where(x=>x.ProductDetail.Product.SupplierId==supplier.SupplierId)
                   .Select(detail => new
                   {
                       Year = detail.Order.OrderTime.Year,
                       Amount = detail.ProductDetail.UnitPrice
                   })
                   .GroupBy(item => item.Year)
                   .Select(group => new
                   {
                        Year = group.Key,
                        TotalAmount = group.Sum(item => item.Amount)
                   })
                   .OrderBy(item => item.Year)
                   .ToList();

                return Json(salesData);
            }
            else
            {
               var salesData = _context.OrderDetail
                  .Select(detail => new
                  {
                     Year = detail.Order.OrderTime.Year,
                     Amount = detail.ProductDetail.UnitPrice
                  })
                  .GroupBy(item => item.Year)
                  .Select(group => new
                  {
                     Year = group.Key,
                     TotalAmount = group.Sum(item => item.Amount)
                  })
                  .OrderBy(item => item.Year)
                  .ToList();

                   return Json(salesData);
            }
 
        }
        public IActionResult GetOrderGrowthChart()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);

                var orders = _context.Order.Where(o => o.OrderDetail.FirstOrDefault().ProductDetail.Product.SupplierId == supplier.SupplierId).ToList();
                var years = orders.Select(o => o.OrderTime.Year).Distinct().OrderBy(y => y).ToList();
                var growthRates = new List<double>();

                for (int i = 1; i < years.Count; i++)
                {
                    var previousYearData = orders.Where(o => o.OrderTime.Year == years[i - 1]).ToList();
                    var currentYearData = orders.Where(o => o.OrderTime.Year == years[i]).ToList();//從2021~2023顯示改變寫法

                    double growthRate = CalculateYearlyGrowthRate(previousYearData, currentYearData);
                    growthRates.Add(growthRate);
                }

                return Json(new
                {
                    years = years.Skip(1).ToList(), // 因為第一年沒有成長率，所以我們跳過它
                    growthRates = growthRates
                });
            }
            else {
                var orders = _context.Order.ToList();//先算出全部的訂單數
                var years = orders.Select(o => o.OrderTime.Year).Distinct().OrderBy(y => y).ToList();//只取出同年度的
                var growthRates = new List<double>();//丟到成長率的集合,因為會有很多年分,但不知道總用有多少年

                for (int i = 1; i < years.Count; i++)//只要有年度就計算
                {
                    var previousYearData = orders.Where(o => o.OrderTime.Year == years[i - 1]).ToList();//上一年度資訊
                    var currentYearData = orders.Where(o => o.OrderTime.Year == years[i]).ToList();//從2021~2023顯示改變寫法 當年度

                    double growthRate = CalculateYearlyGrowthRate(previousYearData, currentYearData);//計算成長率
                    growthRates.Add(growthRate);//新增資訊到成長率的值並且一直新增
                }
                //使用json回傳並且給前端處理
                return Json(new
                {
                    years = years.Skip(1).ToList(), // 因為第一年沒有成長率，所以我們跳過它
                    growthRates = growthRates
                });
            }
        }

        public double CalculateYearlyGrowthRate(List<Order> previousYearData, List<Order> currentYearData)
        {
            // 計算前一年的總數值
            double previousYearTotal = previousYearData.Sum(o => o.OrderTime.Year);

            // 計算當前年份的總數值
            double currentYearTotal = currentYearData.Sum(o => o.OrderTime.Year);

            // 計算年度成長率
            double growthRate = (currentYearTotal - previousYearTotal) / previousYearTotal * 100;

            return growthRate;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
