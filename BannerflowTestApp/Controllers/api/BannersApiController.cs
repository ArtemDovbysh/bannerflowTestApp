using System.Collections.Generic;
using System.Threading.Tasks;
using BannerflowTestApp.Data.Models;
using BannerflowTestApp.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace BannerflowTestApp.Web.Controllers.api
{
    [Route("api/banners")]
    public class BannersApiController : Controller
    {
        private readonly IRepository<Banner> _bannerRepository;

        public BannersApiController(IRepository<Banner> bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        // GET api/banners
        [HttpGet]
        public async Task<IEnumerable<Banner>> Get()
        {
            var banners = await _bannerRepository.GetAllAsync();
            return banners;
        }

        // GET api/banners/5
        [HttpGet("{id}")]
        public async Task<Banner> Get(int id)
        {
            return await _bannerRepository.GetAsync(id);
        }

        // POST api/banners
        [HttpPost]
        public async Task Post(Banner banner)
        {
            await _bannerRepository.CreateAsync(banner);
        }

        // PUT api/banners/5
        [HttpPut("{id}")]
        public async Task Put(int id, Banner banner)
        {
            await _bannerRepository.UpdateAsync(banner);
        }

        // DELETE api/banners/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _bannerRepository.DeleteAsync(id);
        }
    }
}
