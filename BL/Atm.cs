using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Atm
    {
        public static ML.Result GetById(int IdAtm)
        {
            ML.Result result = new ML.Result();

            using (DL.CajeroAutomaticoContext context = new DL.CajeroAutomaticoContext())
            {
                try
                {
                    var query = (from atmDL in context.Atms
                                 where atmDL.IdAtm == IdAtm
                                 select atmDL).FirstOrDefault();
                    if (query != null)
                    {
                        ML.Atm atm = new ML.Atm();
                        atm.IdAtm = query.IdAtm;
                        atm.Saldo = query.Saldo;

                        result.Object = atm;

                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No hay datos con ese id de atm";
                    }
                }
                catch (Exception ex)
                {
                    result.Correct = false;
                    result.ErrorMessage = ex.Message;
                }

            }
            return result;
        }
    }
}
