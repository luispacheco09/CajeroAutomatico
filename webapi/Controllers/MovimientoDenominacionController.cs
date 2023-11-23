using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoDenominacionController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<DL.MovimientoDenominacion>> GetMovimientoDenominacion(int id)
        {
            ML.Result result = BL.MovimientoDenominacion.MovimientoDenominaciones(id);
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
