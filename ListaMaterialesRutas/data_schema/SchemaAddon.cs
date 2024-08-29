using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonListaMaterialesYrutas.data_schema
{
    public class SchemaAddon
    {
        #region TABLAS_GENERICAS
        public static Dictionary<string, string> tablesGeneric()
        {
            var tables = new Dictionary<string, string>();
            tables.Add(SCLinProd.TABLE_CABE, SCLinProd.TABLE_CABE_DES);
            tables.Add(SCCtrHora.TABLE_CTRLHORA, SCCtrHora.TABLE_CTRLHORA_DES);
            tables.Add(SCCtrHora.TABLE_PARADA, SCCtrHora.TABLE_PARADA_DES);
            tables.Add(SCCtrHora.TABLE_TIPPAR, SCCtrHora.TABLE_TIPPAR_DES);
            tables.Add(SCBalanza.TABLE_BALANZA, SCBalanza.TABLE_BALANZA_DES);
            tables.Add(SCBalanza.TABLE_USRRUT, SCBalanza.TABLE_USRRUT_DES);
            tables.Add(SCBalanza.TABLE_PARTENT, SCBalanza.TABLE_PARTENT_DES);
            tables.Add(SCBalanza.TABLE_ROLBAL, SCBalanza.TABLE_ROLBAL_DES);
            tables.Add(SCConfig.TABLE_MATERIALES, SCConfig.TABLE_MATERIALES_DES);
            tables.Add(SCConfig.TABLE_ALMACENFORM, SCConfig.TABLE_ALMACENFORM_DES);

            return tables;
        }
        public static Dictionary<string, string> tablesGenericNoAuto()
        {
            var tables = new Dictionary<string, string>();
            tables.Add(SCBalanza.TABLE_UNITALT, SCBalanza.TABLE_UNITALT_DES);
            tables.Add(SCConfig.TABLE_CONVERSION, SCConfig.TABLE_CONVERSION_DES);
            tables.Add(SCConfig.TABLE_TINTA, SCConfig.TABLE_TINTA_DES);

            return tables;
        }
        #endregion
        #region TABLAS_DATOS_MAESTROS
        //Cabeceras
        public static Dictionary<string, string> tablesMasterH()
        {
            var tables = new Dictionary<string, string>();
            tables.Add(SCConfig.TABLE_CABE, SCConfig.TABLE_CABE_DES);
            tables.Add(SCFormulacion.TABLE_CABE, SCFormulacion.TABLE_CABE_DES);
            return tables;
        }

        //Detalles
        public static Dictionary<string, string> tablesMasterD()
        {
            var tables = new Dictionary<string, string>();
            tables.Add(SCFormulacion.TABLE_FRRUTA, SCFormulacion.TABLE_FRRUTA_DES);
            tables.Add(SCFormulacion.TABLE_FORMUL, SCFormulacion.TABLE_FORMUL_DES);
            tables.Add(SCFormulacion.TABLE_SUBPRD, SCFormulacion.TABLE_SUBPRD_DES);
            tables.Add(SCFormulacion.TABLE_INDUCT, SCFormulacion.TABLE_INDUCT_DES);
            tables.Add(SCFormulacion.TABLE_FREXTR, SCFormulacion.TABLE_FREXTR_DES);
            tables.Add(SCFormulacion.TABLE_FRIMPR, SCFormulacion.TABLE_FRIMPR_DES);
            tables.Add(SCFormulacion.TABLE_LAMINA, SCFormulacion.TABLE_LAMINA_DES);
            tables.Add(SCFormulacion.TABLE_FRSELA, SCFormulacion.TABLE_FRSELA_DES);
            tables.Add(SCFormulacion.TABLE_FRCORT, SCFormulacion.TABLE_FRCORT_DES);
            tables.Add(SCFormulacion.TABLE_FRHABI, SCFormulacion.TABLE_FRHABI_DES);
            tables.Add(SCFormulacion.TABLE_FRREBO, SCFormulacion.TABLE_FRREBO_DES);
            tables.Add(SCFormulacion.TABLE_FRSERV, SCFormulacion.TABLE_FRSERV_DES);

            return tables;
        }
        #endregion
        #region TABLAS_DOCUMENTOS
        //Cabeceras
        public static Dictionary<string, string> tablesDocsH()
        {
            var tables = new Dictionary<string, string>();


            return tables;
        }

        //Detalles
        public static Dictionary<string, string> tablesDocsD()
        {
            var tables = new Dictionary<string, string>();

            return tables;
        }
        #endregion
        public static List<CampoBean> camposTB()
        {
            var campos = new List<CampoBean>();
            campos.AddRange(SCUserFields.getCamposUsuario());
            campos.AddRange(SCLinProd.getCamposTablas());
            campos.AddRange(SCFormulacion.getCamposTablas());
            campos.AddRange(SCBalanza.getCamposTablas());
            campos.AddRange(SCCtrHora.getCamposTablas());
            campos.AddRange(SCConfig.getCamposTablas());
            return campos;
        }

        public static List<ObjetoBean> objetosADDON()
        {
            var objects = new List<ObjetoBean>();
            //objects.Add(SCConfig.getObjeto());
            objects.Add(SCFormulacion.getObjeto());

            return objects;
        }
    }
}
