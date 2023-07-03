using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokBookStore.DAL;
using PustokBookStore.Entities;
using PustokBookStore.ViewModels;
using System.Security.Claims;

namespace PustokBookStore.Controllers
{
    public class BookController : Controller
    {
        readonly PustokDbContext _context;
        public BookController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult GetDetail(int id)
        {
            Book book = _context.Books
                .Include(x => x.BookImages)
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .Include(x => x.BookTags).ThenInclude(x =>x.Tag)
                .FirstOrDefault(x => x.Id == id);
            return PartialView("_BookModalPartial", book);
        }

        public IActionResult AddToBasket(int id)
        {
            BasketVM basketVM = new BasketVM();

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = _context.BasketItems.Where(x=>x.AppUserId == userId).ToList();

                var basketItem = _context.BasketItems.FirstOrDefault(x=>x.BookId == id);

                if(basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        BookId = id,
                        Count = 1,
                        AppUserId = userId,
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }

                _context.SaveChanges();

                var items = _context.BasketItems.Include(x => x.Book).ThenInclude(x => x.BookImages.Where(x => x.POsterStatus == true)).Where(x => x.AppUserId == userId).ToList();

                foreach (var bi in items)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = bi.Count,
                        Book = bi.Book,
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }

            }
            else
            {
                var basketStr = HttpContext.Request.Cookies["basket"];
                List<BasketCookieItemVM> cookieItems = null;

                if (basketStr == null)
                {
                    cookieItems = new List<BasketCookieItemVM>();
                }
                else
                {
                    cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);
                }

                BasketCookieItemVM cookieItem = cookieItems.FirstOrDefault(x => x.BookId == id);
                if (cookieItem == null)
                {
                    cookieItem = new BasketCookieItemVM
                    {
                        BookId = id,
                        Count = 1
                    };
                    cookieItems.Add(cookieItem);
                }
                else
                {
                    cookieItem.Count++;
                }

                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

                foreach (var ci in cookieItems)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = ci.Count,
                        Book = _context.Books.Include(x => x.BookImages.Where(x => x.POsterStatus == true)).FirstOrDefault(x => x.Id == ci.BookId)
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }
            }

           

            return PartialView("_BasketPartial", basketVM);
        }

        public IActionResult ShowBasket()
        {
            var dataStr = HttpContext.Request.Cookies["basket"];
            var data = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(dataStr);
            return Json(data);
        }
    }
}
