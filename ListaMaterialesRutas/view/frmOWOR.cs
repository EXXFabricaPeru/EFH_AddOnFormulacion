
using AddonListaMaterialesYrutas.commons;
using AddonListaMaterialesYrutas.conexion;
using System;
using System.Globalization;

namespace AddonListaMaterialesYrutas.view
{
    public class frmOWOR : FormCommon, IForm
    {
        #region variables
        private SAPbouiCOM.Form mForm, udfForm;
        private SAPbouiCOM.Matrix oMatrix;


        private const string GRD_ITM = "38", GRD_SERV = "39";//ID Grid
        private const string BTN_OK = "1"; //ID BOTONES
        private const string BTN_FORMUL = "btnForm", BASE_POS = "255000118";//FormButtom
        private const string MTX_LISTA = "37", EDT_ITEMCODE = "6", EDT_WHS = "78", EDT_QTY = "12";//Form
        private const string UDF_TIPO = "U_EXX_UNDOV", UDF_CANT = "U_EXX_CNTOV";//Form
        private const string COL_TIPO = "1880000002", COL_CODE = "4", COL_DESC = "3", COL_WHS = "10", COL_CANT = "14", COL_PRICE = "14", COL_CANTADIC = "1880000005", COL_POSICION = "U_EXP_POSICION", COL_PREDECE = "U_EXP_PREDECE";//LISTA MAT
        private const int COL_ITM = 5, COL_STOCK = 2;

        #endregion

        public frmOWOR() { }


        #region _EVENTOS_ITEMEVENT

        //Principal
        public bool HandleItemEvents(SAPbouiCOM.ItemEvent itemEvent)
        {
            var result = true;
            try
            {
                switch (itemEvent.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        result = WhenFormLoad(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        result = WhenItemPressed(itemEvent);
                        break;

                }
            }
            catch (Exception ex)
            {
                result = false;
                StatusMessageError("HandleItemEvents() > " + ex.Message);
            }
            return result;
        }

        //Método maneja evento
        private bool WhenLostFocus(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;

            switch (oEvent.ItemUID)
            {
                default:
                    break;
            }
            return res;
        }


        private bool WhenFormLoad(SAPbouiCOM.ItemEvent oEvent)
        {
            if (!oEvent.BeforeAction) return true;
            AddUIAux(Conexion.application.Forms.Item(oEvent.FormUID));
            return true;
        }

        private bool WhenItemPressed(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {
                case BTN_FORMUL:
                    if (!oEvent.BeforeAction)
                        res = OperateFormulacion(oEvent);
                    break;
                default:
                    break;
            }
            return res;
        }

        private bool WhenDataAdd(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;

            switch (oEvent.ItemUID)
            {
                default:
                    break;
            }
            return res;
        }

        #endregion

        public bool HandleFormDataEvents(SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo)
        {
            switch (oBusinessObjectInfo.EventType)
            {
                default:
                    break;
            }
            return true;
        }

        public bool HandleMenuDataEvents(SAPbouiCOM.MenuEvent menuEvent)
        {
            return true;
        }

        #region _EVENTS_RIGHTCLICK
        public bool HandleRightClickEvent(SAPbouiCOM.ContextMenuInfo menuInfo)
        {
            var result = true;
            return result;
        }
        #endregion

        #region _METODOS_PROPIOS

        private void AddUIAux(SAPbouiCOM.Form oForm)
        {
            SAPbouiCOM.Items oItems = oForm.Items;
            SAPbouiCOM.Item oGeneric;
            SAPbouiCOM.Button oBtn;
            try
            {
                oGeneric = oItems.Add(BTN_FORMUL, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oGeneric.Top = oItems.Item(BASE_POS).Top + oItems.Item(BASE_POS).Height + 10;
                oGeneric.Left = oItems.Item(BASE_POS).Left;
                oGeneric.Width = oItems.Item(BASE_POS).Width;
                oGeneric.Enabled = true;
                oGeneric.Visible = true;
                oBtn = oGeneric.Specific;
                oBtn.Caption = "Formulación";
            }
            catch (Exception ex)
            {
                StatusMessageError("AddUIExcel() > " + ex.Message);
            }
            finally
            {
                LiberarObjetoGenerico(oItems);
            }
        }

        private bool OperateFormulacion(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            double CantOri = 0;
            string TipoOri = "";
            mForm = Conexion.application.Forms.Item(oEvent.FormUID);
            udfForm = Conexion.application.Forms.Item(mForm.UDFFormUID);
            string itemCode = mForm.Items.Item(EDT_ITEMCODE).Specific.value;
            oMatrix = mForm.Items.Item(MTX_LISTA).Specific;

            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                //if (mForm.Mode != SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                //{
                //    StatusMessageWarning("Solo se formula en modo creación");
                //    return false;
                //}
                if (string.IsNullOrEmpty(itemCode))
                {
                    StatusMessageWarning("No se tiene ItemCode");
                    return false;
                }

                oRS.DoQuery(Queries.GetFormulacion(itemCode));
                if (oRS.RecordCount == 0)
                {
                    StatusMessageWarning("No hay formulación asignada");
                    return false;
                }

                CantOri = oRS.Fields.Item("Qty").Value;
                TipoOri = oRS.Fields.Item("Type").Value;

                if (!string.IsNullOrEmpty(udfForm.Items.Item(UDF_TIPO).Specific.value))
                {
                    string tipoFormulacion = GetTipoFormulacion(udfForm.Items.Item(UDF_TIPO).Specific.value);
                    ActualizarCantTipo(itemCode, double.Parse(udfForm.Items.Item(UDF_CANT).Specific.value), tipoFormulacion); //Actualizar formula base
                }
                    
                
                StatusMessageSuccess("Calculando formulas");
                mForm.Freeze(true);

                ProcesoFormulacion(itemCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (CantOri != 0) ActualizarCantTipo(itemCode, CantOri, TipoOri);//Retoma a valores iniciales;
                mForm.Freeze(false);
            }
            return res;
        }

        private string GetTipoFormulacion(string undMedidaSAP)
        {
            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            
            try
            {
                string query = $"SELECT \"U_EXX_UMEDFOR\" FROM \"@EXX_OUOM\" WHERE \"Code\" = '{undMedidaSAP}'";
                
                oRS.DoQuery(query);

                if (oRS.RecordCount == 0)
                    throw new Exception("No se encontró un valor de unidad de medida de formulación homologado para la unidad de medida: " + undMedidaSAP);

                return oRS.Fields.Item("U_EXX_UMEDFOR").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }
        #endregion
        private void ActualizarCantTipo(string itemCode, double cantidad, string tipo)
        {
            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                oRS.DoQuery(Queries.UpdateFormulacion(itemCode, tipo, cantidad));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }
        private void ProcesoFormulacion(string itemCode)
        {
            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                //mForm.DataSources.DataTables.Add("DT_QRY");
                //SAPbouiCOM.DataTable dt = mForm.DataSources.DataTables.Item("DT_QRY");
                //dt.ExecuteQuery(Queries.GetFormulacionDetalle(itemCode, Conexion.application.Company.UserName));
                oRS.DoQuery(Queries.GetFormulacionDetalle(itemCode, Conexion.application.Company.UserName));

                //double cantidad = oRS.Fields.Item("KILO").Value;
                //double cantidadDT = dt.GetValue("KILO", 0);

                if (oRS.RecordCount == 0) return;
                int position = 1;
                mForm.Items.Item(EDT_QTY).Specific.value = oRS.Fields.Item("KILO").Value.ToString();
                mForm.Items.Item(EDT_WHS).Specific.value = oRS.Fields.Item("WHS").Value;
                
                //double cant = oRS.Fields.Item("CANT2").Value;

                while (!oRS.EoF)
                {
                    SAPbouiCOM.ComboBox combobox = oMatrix.Columns.Item(COL_TIPO).Cells.Item(position).Specific;

                    try
                    {
                        combobox.Select(oRS.Fields.Item("TIPO").Value, SAPbouiCOM.BoSearchKey.psk_Index);
                    }
                    catch (Exception) //ES UNA LINEA NO EDITABLE (NO FUNCIONA EL CONSULTAR LA PROPIEDAD ENABLE)
                    {
                        position++;
                        oRS.MoveNext();
                        continue;
                    }
                    
                    oMatrix.Columns.Item(COL_CODE).Cells.Item(position).Specific.Value = oRS.Fields.Item("ITEMCODE").Value;

                    if (oRS.Fields.Item("TIPO").Value == 3)
                    {
                        oMatrix.Columns.Item(COL_DESC).Cells.Item(position).Specific.Value = oRS.Fields.Item("DESC").Value;
                    }
                    else
                    {
                        //oMatrix.Columns.Item(COL_CANT).Cells.Item(position).Specific.Value = oRS.Fields.Item("CANT").Value == 0 ? -1 : oRS.Fields.Item("CANT").Value;
                        //oMatrix.Columns.Item(COL_CANTADIC).Cells.Item(position).Specific.Value = oRS.Fields.Item("CADI").Value;

                        string xml = oRS.GetAsXML();
                        var valor = oRS.Fields.Item("CANT").Value;
                        oMatrix.GetCellSpecific(COL_CANT, position).Value = oRS.Fields.Item("CANT").Value == 0 ? -1 : oRS.Fields.Item("CANT").Value;
                        oMatrix.GetCellSpecific(COL_CANTADIC, position).Value = oRS.Fields.Item("CADI").Value;

                        //if (oRS.Fields.Item("PRICE").Value != 0) oMatrix.Columns.Item(COL_PRICE).Cells.Item(position).Specific.Value = oRS.Fields.Item("PRICE").Value;
                        if (oRS.Fields.Item("PRICE").Value != 0)
                            oMatrix.GetCellSpecific(COL_PRICE, position).Value = oRS.Fields.Item("PRICE").Value;

                        //oMatrix.Columns.Item(COL_WHS).Cells.Item(position).Specific.Value = oRS.Fields.Item("WHS").Value;
                        oMatrix.GetCellSpecific(COL_WHS, position).Value = oRS.Fields.Item("WHS").Value;
                        oMatrix.Columns.Item(COL_POSICION).Cells.Item(position).Specific.Value = oRS.Fields.Item("RUTA").Value;
                        if (oRS.Fields.Item("PRED").Value != "0") oMatrix.Columns.Item(COL_PREDECE).Cells.Item(position).Specific.Value = oRS.Fields.Item("PRED").Value;
                    }


                    position++;
                    oRS.MoveNext();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }

        public string getFormUID()
        {
            if (mForm != null)
                return mForm.UniqueID;
            else
                return null;
        }
    }
}