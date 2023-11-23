using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoDenominacionController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<DL.MovimientoDenominacion>> PostMovimientoDenominacion(DL.MovimientoDenominacion movimiento)
        {
            ML.Result result = BL.MovimientoDenominacion.MovimientoDenominaciones(movimiento, 1500);
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
