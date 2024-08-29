using System;
using System.Text;

namespace AddonListaMaterialesYrutas.commons
{
    public class Queries
    {
        #region _Attributes_
        private static StringBuilder m_sSQL = new StringBuilder();
        #endregion

        #region _Functions_
        public static string ConsultaSeries()
        {
            m_sSQL.Length = 0;
            //m_sSQL.AppendFormat("SELECT \"Series\", \"SeriesName\", \"DocSubType\" FROM NNM1 where \"ObjectCode\" IN ('{0}')  AND \"Locked\" = 'N' and \"DocSubType\" IN ('--','IB')", 13);
            m_sSQL.Append("SELECT \"Series\", \"SeriesName\", \"DocSubType\"");
            m_sSQL.AppendFormat(" FROM NNM1 where \"ObjectCode\" IN ('{0}')  AND \"Locked\" = 'N' AND \"Indicator\" = '{1}' and \"DocSubType\" IN ('--','IB')", 13, DateTime.Now.Year);
            return m_sSQL.ToString();
        }

        public static string GetCheckCFGAux(string entry)
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT TOP 1 IFNULL(\"{0}\",'N') \"Check\" FROM \"@{1}\"", entry, data_schema.SCConfig.TABLE_CABE);
            return m_sSQL.ToString();
        }

        public static string GetNextFormCode(string itemcode)
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("select top 1 * from(SELECT TOP 1  LPAD(SUBSTRING(\"Code\", length('{0}')+1)+1,2,'0') \"Value\" FROM \"@EXP_OFRM\" where \"Code\" like '{0}%' ", itemcode);
            m_sSQL.AppendFormat("union all ");
            m_sSQL.AppendFormat("SELECT '00' \"Value\" FROM dummy) order by \"Value\" desc; ");
            return m_sSQL.ToString();
        }
        public static string GetCFGValue(string entry)
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT TOP 1 IFNULL(\"{0}\",'') \"Value\" FROM \"@{1}\"", entry, data_schema.SCConfig.TABLE_CABE);
            return m_sSQL.ToString();

        }
        public static string GetUDFValue(string table, string campo, string cod)
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT T1.\"Descr\" \"Value\" ");
            m_sSQL.AppendFormat("FROM CUFD \"T0\" ");
            m_sSQL.AppendFormat("JOIN UFD1 \"T1\" ON T0.\"TableID\"=T1.\"TableID\" AND T0.\"FieldID\"=T1.\"FieldID\" ");
            m_sSQL.AppendFormat("WHERE T0.\"TableID\" = '@{0}' AND T0.\"AliasID\" = '{1}' AND T1.\"FldValue\" = '{2}'", table.Trim(), campo.Trim(), cod.Trim());
            return m_sSQL.ToString();
        }
        public static string GetFormulacion(string itemcode)
        {
            m_sSQL.Length = 0;
            m_sSQL.Append("SELECT ");
            m_sSQL.Append("FO0.\"U_EXP_QTT\" \"Qty\", ");
            m_sSQL.Append("FO0.\"U_EXP_TIPO\" \"Type\" ");
            m_sSQL.Append("FROM OITM \"ITM\" ");
            m_sSQL.AppendFormat("JOIN \"@{0}\" \"FO0\" ON ITM.\"U_EXP_FORM\"=FO0.\"Code\" ", data_schema.SCFormulacion.TABLE_CABE);
            m_sSQL.AppendFormat("WHERE ITM.\"ItemCode\"='{0}'", itemcode);
            return m_sSQL.ToString();
        }
        public static string UpdateFormulacion(string itemcode, string tipo, double cant)
        {
            m_sSQL.Length = 0;
            m_sSQL.Append("UPDATE FO0 SET ");
            m_sSQL.AppendFormat("FO0.\"U_EXP_QTT\" ={0}, ", cant);
            m_sSQL.AppendFormat("FO0.\"U_EXP_TIPO\" ='{0}' ", tipo);
            m_sSQL.Append("FROM OITM \"ITM\" ");
            m_sSQL.AppendFormat("JOIN \"@{0}\" \"FO0\" ON ITM.\"U_EXP_FORM\"=FO0.\"Code\" ", data_schema.SCFormulacion.TABLE_CABE);
            m_sSQL.AppendFormat("WHERE ITM.\"ItemCode\"='{0}'", itemcode);
            return m_sSQL.ToString();
        }
        public static string GetFormulacionDetalle(string itemcode, string user)
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("CALL \"EXP_FormulacionListaMateriales\" ('{0}','{1}')", itemcode, user);
            return m_sSQL.ToString();
        }
        public static string GetComboTax()
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT  \"Code\" \"Value\",\"Name\" \"Name\" FROM \"OSTC\" WHERE \"Lock\"='N'");
            return m_sSQL.ToString();
        }

        public static string GetComboRutas()
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT  \"Code\" \"Value\",\"Desc\" \"Name\" FROM \"ORST\"");
            return m_sSQL.ToString();
        }

        public static string GetComboMateriales()
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT  \"Name\" \"Value\",\"Name\" \"Name\" FROM \"@EXP_TIPMAT\"");
            return m_sSQL.ToString();
        }
        public static string GetComboTinta()
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT  \"Code\" \"Value\",\"Name\" \"Name\" FROM \"@EXP_TINTA\"");
            return m_sSQL.ToString();
        }

        public static string GetComboUnidadMedidas()
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT  \"Code\" \"Value\",\"Name\" \"Name\" FROM \"@EXP_CONMED\"");
            return m_sSQL.ToString();
        }
        public static string GetRefile(string ItemCode)
        {
            m_sSQL.Length = 0;
            m_sSQL.Append("SELECT TOP 1 IFNULL(IFNULL(NULLIF(\"U_EXP_RFANCHF\",0),NULLIF(\"U_EXP_RFLARGF\",0)),0) \"Value\" ");
            m_sSQL.Append("FROM OITM ");
            m_sSQL.AppendFormat("WHERE \"ItemCode\" = '{0}'", ItemCode);
            return m_sSQL.ToString();
        }

        public static string GetFactor(string ItemCode)
        {
            m_sSQL.Length = 0;
            m_sSQL.Append("SELECT TOP 1 IFNULL(IFNULL(NULLIF(\"U_EXP_FCANCHF\",0),NULLIF(\"U_EXP_FCLARGF\",0)),0) \"Value\" ");
            m_sSQL.Append("FROM OITM ");
            m_sSQL.AppendFormat("WHERE \"ItemCode\" = '{0}'", ItemCode);
            return m_sSQL.ToString();
        }

        public static string GetRutaValues(string cod)
        {
            m_sSQL.Length = 0;
            m_sSQL.Append("SELECT RU.\"Desc\" \"Value\", IFNULL(RU.\"U_EXP_SUBPRD\",'') \"SUBPRD\", IFNULL(RU.\"U_EXP_SCRAP\",'') \"SCRAP\", IFNULL(RU.\"U_EXP_REFILE\",'') \"REFILE\" ");
            m_sSQL.Append(", IFNULL(IT.\"ItemCode\",'') \"INDUCTOR\" ");
            m_sSQL.Append("FROM \"ORST\" \"RU\" ");
            m_sSQL.Append("LEFT JOIN \"OITM\" \"IT\" ON RU.\"Code\"=IT.\"U_EXP_RIND\" ");
            m_sSQL.AppendFormat("WHERE RU.\"Code\"='{0}'", cod);
            return m_sSQL.ToString();
        }
        public static string GetComboPeriodos()
        {
            m_sSQL.Length = 0;
            m_sSQL.AppendFormat("SELECT TOP 6 distinct \"LineId\",\"U_Fecha\" \"Value\",\"U_Fecha\" \"Name\" FROM \"@EXD_FACT_PROR\" ORDER BY  \"LineId\" DESC");
            return m_sSQL.ToString();
        }
        public static string GetLiquidacionList(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string periodo, string tipo)
        {
            m_sSQL.Length = 0;
            string[] per = periodo.Split('-');
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    switch (tipo)
                    {
                        case Constantes.SERV_CODE:
                            m_sSQL.AppendFormat(Constantes.PREFIX_SP_HANA + "\"EXD_PROIGV_SERV\" ({0},{1})", per[1], per[0]);
                            break;
                        case Constantes.ITEMS_CODE:
                            m_sSQL.AppendFormat(Constantes.PREFIX_SP_HANA + "\"EXD_PROIGV_IT\" ({0},{1})", per[1], per[0]);
                            break;
                        case Constantes.ACTF_CODE:
                            m_sSQL.AppendFormat(Constantes.PREFIX_SP_HANA + "\"EXD_PROIGV_AF\" ({0},{1})", per[1], per[0]);
                            break;
                    }
                    break;
            }
            return m_sSQL.ToString();
        }
        public static string GetLiquidacionOp(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string periodo, string tipo)
        {
            m_sSQL.Length = 0;
            string[] per = periodo.Split('-');
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    switch (tipo)
                    {
                        case Constantes.SERV_CODE:
                            m_sSQL.AppendFormat(Constantes.PREFIX_SP_HANA + "\"EXD_PROIGV_SERV\" ({0},{1})", per[1], per[0]);
                            break;
                        case Constantes.ITEMS_CODE:
                            m_sSQL.AppendFormat(Constantes.PREFIX_SP_HANA + "\"EXD_PROIGV_IT_SUM\" ({0},{1})", per[1], per[0]);
                            break;
                        case Constantes.ACTF_CODE:
                            m_sSQL.AppendFormat(Constantes.PREFIX_SP_HANA + "\"EXD_PROIGV_AF\" ({0},{1})", per[1], per[0]);
                            break;
                    }
                    break;
            }
            return m_sSQL.ToString();
        }
        public static string ConsultaTablaConfiguracion(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string NAddon, string Version, bool Ordenamiento)
        {
            m_sSQL.Length = 0;

            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT * FROM \"@{0}\"", NAddon.ToUpper());
                    if (NAddon != "" || Version != "")
                    {
                        m_sSQL.Append(" WHERE ");
                        if (NAddon != "")
                        {
                            m_sSQL.AppendFormat("\"Name\" Like '{0}%'", NAddon);
                            if (Version != "") m_sSQL.AppendFormat(" AND \"Code\" = '{0}'", Version);
                        }
                        else if (Version != "") m_sSQL.AppendFormat("\"Code\" = '{0}'", Version);
                    }
                    if (Ordenamiento) m_sSQL.Append(" ORDER BY LENGTH(\"Code\") DESC, \"Code\" DESC");
                    break;
                default:
                    m_sSQL.AppendFormat("SELECT * FROM [@{0}]", NAddon.ToUpper());
                    if (NAddon != "" || Version != "")
                    {
                        m_sSQL.Append(" WHERE ");
                        if (NAddon != "")
                        {
                            m_sSQL.AppendFormat("Name Like '{0}%'", NAddon);
                            if (Version != "") m_sSQL.AppendFormat(" AND Code = '{0}'", Version);
                        }
                        else if (Version != "") m_sSQL.AppendFormat("Code = '{0}'", Version);
                    }
                    if (Ordenamiento) m_sSQL.Append(" ORDER BY LEN(Code) DESC, Code DESC");
                    break;
            }
            return m_sSQL.ToString();
        }
        #endregion
    }
}
