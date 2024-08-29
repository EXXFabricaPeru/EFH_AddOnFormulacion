using System.Collections.Generic;

namespace AddonListaMaterialesYrutas.data_schema
{
    class SCLinProd
    {
        #region _CABECERA_TABLA
        public const string TABLE_CABE = "EXP_LINPRDC";
        public const string TABLE_CABE_DES = "EXP - Línea de Prod";

        #endregion

        #region _CAMPOS
        public static List<CampoBean> getCamposTablas()
        {
            var myList = new List<CampoBean>();

            #region Cabecera
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT1",
                descrp_campo = "Ruta 1",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT2",
                descrp_campo = "Ruta 2",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT3",
                descrp_campo = "Ruta 3",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT4",
                descrp_campo = "Ruta 4",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT5",
                descrp_campo = "Ruta 5",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT6",
                descrp_campo = "Ruta 6",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_RT7",
                descrp_campo = "Ruta 7",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            #endregion

            return myList;
        }

        #endregion
        #region _OBJETO

        #endregion


    }
}
