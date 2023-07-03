using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokBookStore.DAL;
using PustokBookStore.Entities;
using PustokBookStore.ViewModels;
using System.Security.Claims;

namespace PustokBookStore.Services
{
    public class LayoutService
    {
        readonly PustokDbContext _context;
        readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutService(PustokDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x=>x.Key, x=>x.Value);
        }

        public BasketVM GetBasket()
        {
            var basketVM = new BasketVM();

            if(_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dbItems = _context.BasketItems.Include(x => x.Book).ThenInclude(x => x.BookImages.Where(x => x.POsterStatus == true)).Where(x => x.AppUserId == userId).ToList();

                foreach (var dbItem in dbItems)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = dbItem.Count,
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == dbItem.BookId)
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }
            }
            else
            {
                var basketStr = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

                List<BasketCookieItemVM> cookieItems = null;
                if (basketStr == null)
                {
                    cookieItems = new List<BasketCookieItemVM>();
                }
                else
                {
                    cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);
                }

                foreach (var cookieItem in cookieItems)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = cookieItem.Count,
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == cookieItem.BookId)
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }
            }

            
            return basketVM;
        }
    }
}
