using System.Collections.Generic;

namespace AddonListaMaterialesYrutas.data_schema
{
    class SCConfig
    {
        #region _CABECERA_TABLA
        public const string TABLE_CABE = "EXP_CFG_RLM";
        public const string TABLE_CABE_DES = "EXP - Cfg. Rutas y Mat.";
        public const string TABLE_MATERIALES = "EXP_TIPMAT";
        public const string TABLE_MATERIALES_DES = "EXP - Tipo Material";
        public const string TABLE_CONVERSION = "EXP_CONMED";
        public const string TABLE_CONVERSION_DES = "EXP - Conversion medidas";
        public const string TABLE_TINTA = "EXP_TINTA";
        public const string TABLE_TINTA_DES = "EXP - Tipo Tinta";
        public const string TABLE_ALMACENFORM= "EXP_ALMFOR";
        public const string TABLE_ALMACENFORM_DES = "EXP - Almacen Form.";
        #endregion

        #region _CAMPOS
        public static List<CampoBean> getCamposTablas()
        {
            var myList = new List<CampoBean>();

            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CABE,
                nombre_campo = "EXP_FCPM3",
                descrp_campo = "Fact. Conv. Peso M3",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo=SAPbobsCOM.BoFldSubTypes.st_Measurement
            });

            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_MATERIALES,
                nombre_campo = "EXP_GCM33",
                descrp_campo = "Peso G/CM3",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });

            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_CONVERSION,
                nombre_campo = "EXP_TOMET",
                descrp_campo = "Conver. a metros",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });

            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ALMACENFORM,
                nombre_campo = "EXP_WHS",
                descrp_campo = "Almacen",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano=8
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ALMACENFORM,
                nombre_campo = "EXP_WHSCAST",
                descrp_campo = "Almacen CAST",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 8
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ALMACENFORM,
                nombre_campo = "EXP_RUT",
                descrp_campo = "Ruta",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 8
            });

            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ALMACENFORM,
                nombre_campo = "EXP_ORD",
                descrp_campo = "Ord",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 2
            });

            return myList;
        }

        #endregion
        #region _OBJETO

        public static ObjetoBean getObjeto()
        {
            var myObj = new ObjetoBean
            {
                code = TABLE_CABE,
                name = "CONFIG_RUTA_LISTMAT",
                tableName = TABLE_CABE,
                canCancel = SAPbobsCOM.BoYesNoEnum.tNO,
                canClose = SAPbobsCOM.BoYesNoEnum.tNO,
                canDelete = SAPbobsCOM.BoYesNoEnum.tNO,
                childTables = new string[] {  },
                canCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO,
                canFind = SAPbobsCOM.BoYesNoEnum.tNO,
                canLog = SAPbobsCOM.BoYesNoEnum.tNO,
                objectType = SAPbobsCOM.BoUDOObjType.boud_MasterData,
                manageSeries = SAPbobsCOM.BoYesNoEnum.tNO,
                enableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO,
                rebuildEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO
            };
            return myObj;
        }

        #endregion


    }
}
