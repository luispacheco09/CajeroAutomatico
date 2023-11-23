using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtmController : ControllerBase
    {
        // GET: api/Atm/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DL.Atm>> GetById(int id)
        {
            ML.Result result = BL.Atm.GetById(id);
            if (result.Correct)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
