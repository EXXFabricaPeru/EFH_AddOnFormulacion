using System;
using System.Windows.Forms;
using AddonListaMaterialesYrutas.commons;
using AddonListaMaterialesYrutas.conexion;

namespace AddonListaMaterialesYrutas
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Conexion oConexion = new Conexion();

            if ((oConexion != null) && (Conexion.company.Connected))
            {
                EstructuraDatos oEstructuraDatos = new EstructuraDatos();

                GC.KeepAlive(oConexion);
                FormCommon.StatusMessageSuccess("Inicio satisfactorio."); 
                Application.Run();
            }
            else
                Application.Exit();

            Application.Run();
        }
    }
}
