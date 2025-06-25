using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Demo.Models;
using static YS.Knife.Query.Demo.IMaterialService;

namespace YS.Knife.Query.Demo.Impl
{
    [AutoConstructor]
    [Mapper(typeof(Material), typeof(MaterialInfo), MapperType = MapperType.Query)]
    public partial class MaterialService : IMaterialService
    {
        private readonly EFContext context;
        public Task<PagedList<MaterialInfo>> QueryMaterials(LimitQueryInfo queryInfo)
        {
            return Task.FromResult(context.Materials.To<MaterialInfo>()
                 .QueryPage(queryInfo));
        }
    }
}
