﻿using AddonListaMaterialesYrutas.bean;
using AddonListaMaterialesYrutas.commons;
using AddonListaMaterialesYrutas.conexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonListaMaterialesYrutas.dao
{
    public class SeriesDAO : FormCommon
    {
        public static List<SerieBean> obtenerSeries()
        {
            var res = new List<SerieBean>();
            SAPbobsCOM.Recordset oRS = null;
            try
            {
                oRS = Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRS.DoQuery(Queries.ConsultaSeries());
                if (oRS.RecordCount > 0)
                {
                    while (!oRS.EoF)
                    {
                        res.Add(new SerieBean()
                        {
                            serie = oRS.Fields.Item("Series").Value.ToString(),
                            tipo = oRS.Fields.Item("DocSubType").Value,
                            descripcion = oRS.Fields.Item("SeriesName").Value
                        });
                        oRS.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessageError("SeriesDAO > obtenerSeries() > " + ex.Message);
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }

            return res;
        }
    }
}