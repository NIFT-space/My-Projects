using Microservices_v1.Services;
using Microsoft.AspNetCore.Mvc;
using static Microservices_v1.Services.GetCycles;

namespace Microservices_v1.Controllers
{
    [ApiController]
    [Route("/J110/[controller]/[action]")]
    public class GetCycleListController : Controller
    {
        [HttpPost]
        public List<Cycles> GetCycles()
        {
            try
            {
                GetCycles gc = new GetCycles();
                List<Cycles> c = gc.FetchCyc();

                return c;
            }
            catch (Exception ex)
            {
                List<Cycles> c = new List<Cycles>();
                return c;
            }
        }
        
    }
}
