using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiFechaControl.Controller;


namespace ConsoleApp1
{
    class Program
    {        
        static void Main(string[] args)
        {
            string conn = "Data Source=172.27.4.207;Initial Catalog=BBVA_FACTOR;Persist Security Info=True;User ID=sa;Password=DimCert2019;connection reset=false;Connection Timeout=60; connection lifetime=5000; min pool size=1; max pool size=5000;";
            IControllerFecha controller = new ControllerFecha(conn);

            controller.Query = "select fecha from tabla where campo = 1";
            controller.OnError += Error;
            controller.OnCambioFecha += FechaModificada;
            controller.CantidadReconecciones = -1;
            
            try
            {
                controller.Start();                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }          
            

            Console.WriteLine("----------");
            while (true)
            {
                
            }
        }

        private static void Error(object sender, ControlErroresEventArgs e)
        {
            Console.WriteLine(e.MensajeError);            
        }

        private static void FechaModificada(object sender, FechaControlEventArgs e)
        {            
            Console.WriteLine(e.FechaControl.ToShortDateString());
        }
    }
}
