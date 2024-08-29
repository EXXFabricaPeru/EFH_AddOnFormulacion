using System.Collections.Generic;

namespace AddonListaMaterialesYrutas.data_schema
{
    public class SCUserFields
    {
        #region _CABECERA_TABLA
        public const string TABLE_ITEMS = "OITM";
        public const string TABLE_USER = "OHEM";
        public const string TABLE_USERDB = "OUSR";
        public const string TABLE_RESOURCE = "ORSC";
        public const string TABLE_RUTA = "ORST";
        public const string TABLE_LOTE= "OBTN";
        public const string TABLE_OT_DET= "WOR1";
        public const string TABLE_OT= "OWOR";
        #endregion

        #region _COLUMNAS
        public static List<CampoBean> getCamposUsuario()
        {
            var myList = new List<CampoBean>();
            #region Recursos
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RESOURCE,
                nombre_campo = "EXP_VARORD",
                descrp_campo = "Varias Ordenes",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef="N"
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RESOURCE,
                nombre_campo = "EXP_VELMAQ",
                descrp_campo = "Velocidad maquina",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric,
                subtipo_campo=SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            #endregion
            #region Items
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_FCANCHF",
                descrp_campo = "F.Factor Ancho",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo=SAPbobsCOM.BoFldSubTypes.st_Quantity
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_RFANCHF",
                descrp_campo = "F.Refile Ancho",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Quantity
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_FCLARGF",
                descrp_campo = "F.Factor Largo",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Quantity
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_RFLARGF",
                descrp_campo = "F.Refile Largo",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Quantity
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_ANCHO",
                descrp_campo = "Ancho",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric,
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_UNDANCH",
                descrp_campo = "Und. Ancho",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 5,
                validValues = new string[] { "CM", "*", "MM" },
                validDescription = new string[] { "Centimetros", "Pulgadas", "Milimetros" }
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_LARGO",
                descrp_campo = "Largo",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric,
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_UNDLAR",
                descrp_campo = "Und. Largo",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 5,
                validValues = new string[] { "CM", "*", "MM" },
                validDescription = new string[] { "Centimetros", "Pulgadas", "Milimetros" }
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_ESP",
                descrp_campo = "Espesor",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric,
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_UNDESP",
                descrp_campo = "Und. Espesor",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 5,
                validValues = new string[] { "CM", "*", "MM" },
                validDescription = new string[] { "Centimetros", "Pulgadas", "Milimetros" }
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_REF",
                descrp_campo = "Refile",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric,
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_UNDREF",
                descrp_campo = "Und. Refile",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 10,
                validValues = new string[] { "CM", "*", "MM" },
                validDescription = new string[] { "Centimetros", "Pulgadas", "Milimetros" }
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_WGTESP",
                descrp_campo = "Peso Especifico",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric,
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_LINPRO",
                descrp_campo = "Línea de Producción",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
                //TODO @@EXP_LINPROD
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_DES",
                descrp_campo = "Descripción",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_DATADI",
                descrp_campo = "Datos adicionales",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_IMPRE",
                descrp_campo = "Impresión",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 100
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_PST",
                descrp_campo = "Presentación",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_CARDCD",
                descrp_campo = "Código de cliente",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 15
                //TODO OCRD

            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_FORM",
                descrp_campo = "Formulacion",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
                //TODO @EXP_OFRM
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_RIND",
                descrp_campo = "Ruta de Inductor",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_ITEMS,
                nombre_campo = "EXP_TPRO",
                descrp_campo = "Tipo En Produccion",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 5,
                validValues = new string[] { "-", "S", "R", "I", "V" },
                validDescription = new string[] { "Ninguno", "Scrap", "Refile", "Inductor", "Servicio" }
            });
            #endregion
            #region User
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_USER,
                nombre_campo = "EXP_USER",
                descrp_campo = "Usuario web",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_USER,
                nombre_campo = "EXP_ROLBAL",
                descrp_campo = "Rol Balanza",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tablaVinculada=SCBalanza.TABLE_ROLBAL,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_USERDB,
                nombre_campo = "EXX_CAST",
                descrp_campo = "Es CAST?",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });
            #endregion
            #region Ruta
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RUTA,
                nombre_campo = "EXP_SUBPRD",
                descrp_campo = "Subproducto",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RUTA,
                nombre_campo = "EXP_SCRAP",
                descrp_campo = "Merma",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RUTA,
                nombre_campo = "EXP_REFILE",
                descrp_campo = "Refile",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RUTA,
                nombre_campo = "EXP_RUTA",
                descrp_campo = "Ruta Reporte",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Memo,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_RUTA,
                nombre_campo = "EXP_REPORTE",
                descrp_campo = "Nombre Reporte",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            #endregion
            #region lote
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_LOTE,
                nombre_campo = "EXP_UndAlt",
                descrp_campo = "Unidad Alternativa",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_LOTE,
                nombre_campo = "EXP_CtdAlt",
                descrp_campo = "Cantidad Alt.",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Quantity
            });
            #endregion
            #region OrdenTrabajoDetalle
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_MEDIDA",
                descrp_campo = "Medida Final",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 50
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_ANCHO",
                descrp_campo = "Ancho",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo=SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_ESPESOR",
                descrp_campo = "Espesor",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_VELOCIDAD",
                descrp_campo = "Velocidad",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_METROS",
                descrp_campo = "Metros",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_MATERIAL",
                descrp_campo = "Material",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 20
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_DETALLE",
                descrp_campo = "Detalle",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 100
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_CANTPED",
                descrp_campo = "Cantidad Pedida",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_CANTAVA",
                descrp_campo = "Cantidad Avanzada",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_RODILLO",
                descrp_campo = "Rodillo",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_COLORES",
                descrp_campo = "Nro. de Colores",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Numeric
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_TIMPRESION",
                descrp_campo = "Tipo Impresion",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXC_KILOS",
                descrp_campo = "Kilos",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Float,
                subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_Measurement
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXX_DSTANDBY",
                descrp_campo = "StandBy",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXX_DTERMIN",
                descrp_campo = "Terminado",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXX_DPARCIAL",
                descrp_campo = "Parcial",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT_DET,
                nombre_campo = "EXX_DANULA",
                descrp_campo = "Anulado",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });

            #endregion
            #region OT
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT,
                nombre_campo = "EXX_CPROGRAM",
                descrp_campo = "Programar",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT,
                nombre_campo = "EXX_CTERMIN",
                descrp_campo = "Terminado",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_OT,
                nombre_campo = "EXX_CANULA",
                descrp_campo = "Anulado",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
                valorPorDef = "N"
            });

            #endregion
            return myList;
        }
        #endregion
    }
}
