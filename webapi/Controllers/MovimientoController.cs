using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        // POST: api/Movimiento
        [HttpPost]
        public async Task<ActionResult<DL.Movimiento>> PostMovimiento(DL.Movimiento movimiento)
        {
            ML.Result result = BL.Movimiento.Retiro(movimiento);
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
