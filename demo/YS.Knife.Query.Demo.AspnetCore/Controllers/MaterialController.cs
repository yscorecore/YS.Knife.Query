using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YS.Knife.Query.Demo.AspnetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FlyTiger.AutoConstructor]
    public partial class MaterialController : ControllerBase, IMaterialService
    {
        private readonly IMaterialService materialService;

        [HttpGet]
        public Task<PagedList<IMaterialService.MaterialInfo>> QueryMaterials([FromQuery] LimitQueryInfo queryInfo)
        {
            return materialService.QueryMaterials(queryInfo);
        }
    }
}
