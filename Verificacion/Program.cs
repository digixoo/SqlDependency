using FechaControl.Controller;
using System;

namespace Verificacion
{
    class Program
    {
        static void Main(string[] args)
        {
            ControllerFecha controller = new ControllerFecha();

            try
            {
                controller.ConnectionString = "Data Source=172.27.4.207;Initial Catalog=BBVA_FACTOR; Persist Security Info=True;User ID=sa;Password=DimCert2019;connection reset=false;;Connection Timeout=1; connection lifetime=0; min pool size=1; max pool size=5000;";
                controller.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            while (true)
            {                
                Console.WriteLine(controller.FechaSistema);
                while(ConsoleKey.Enter != Console.ReadKey().Key)
                {

                }
            }
            



        }
    }
}
