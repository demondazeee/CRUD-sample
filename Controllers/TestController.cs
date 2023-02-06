using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace test_crud.Controllers
{
    [Route("/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public TestController()
        {

        }

        [HttpGet]
        public ActionResult GetTest()
        {
            return Ok(new
            {
                message  = "test",
            });
        }
    }
}
