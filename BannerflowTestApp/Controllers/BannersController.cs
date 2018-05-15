using System.Threading.Tasks;
using BannerflowTestApp.Data.Models;
using BannerflowTestApp.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace BannerflowTestApp.Web.Controllers
{
    [Route("banners")]
    public class BannersController : Controller
    {
        private readonly IRepository<Banner> _bannersRepository;

        public BannersController(IRepository<Banner> bannersRepository)
        {
            _bannersRepository = bannersRepository;
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _bannersRepository.GetAllAsync();
            return View(banners);
        }
    }
}