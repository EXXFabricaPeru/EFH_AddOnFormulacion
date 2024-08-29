using AddonListaMaterialesYrutas.commons;
using AddonListaMaterialesYrutas.conexion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonListaMaterialesYrutas.view
{
    public class formObs : FormCommon, IForm
    {
        private SAPbouiCOM.Form mForm;
        public string Observacion { get; set; }
        public SAPbouiCOM.Form FormBase { get; set; }
        public SAPbouiCOM.Matrix MatrixBase { get; set; }
        public int FilaMatrix { get; set; }
        public formObs(Dictionary<string, IForm> dictionary, string observacion, SAPbouiCOM.Form formBase, SAPbouiCOM.Matrix matrixBase ,int filaMatrix)
        {
            mForm = CreateForm(Conexion.company, Conexion.application, Properties.Resources.FormObs, "FRMOBS");
            if (mForm != null)
            {
                dictionary.Add(getFormUID(), this);
                
                FormBase = formBase;
                FilaMatrix = filaMatrix;
                MatrixBase = matrixBase;
                Observacion = observacion;

                CentrarFormulario();

                mForm.Items.Item("Item_3").Specific.Value = Observacion;
                mForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                mForm.Visible = true;
            }
            else
                StatusMessageError("Construct: No se pudo crear el formulario " + "FRMOBS");
        }

        private void CentrarFormulario()
        {
            if (Conexion.application.ClientType == SAPbouiCOM.BoClientType.ct_Desktop)
            {
                mForm.Left = (Conexion.application.Desktop.Width - mForm.Width) / 2;
                mForm.Top = (Conexion.application.Desktop.Height - mForm.Height) / 2;
            }
        }

        public string getFormUID()
        {
            if (mForm != null)
                return mForm.UniqueID;
            else
                return null;
        }

        public bool HandleFormDataEvents(SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo)
        {
            return true;
        }

        public bool HandleItemEvents(SAPbouiCOM.ItemEvent itemEvent)
        {
            bool res = true;

            try
            {
                switch (itemEvent.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:HandleItemPressed(itemEvent, out res);
                        break;   
                }
            }
            catch (Exception ex)
            {
                string msj = ex.Message;
                throw;
            }

            return res;
        }

        private void HandleItemPressed(SAPbouiCOM.ItemEvent itemEvent, out bool bubbleEvent)
        {
            bubbleEvent = true;

            if(itemEvent.BeforeAction)
            {
                switch (itemEvent.ItemUID)
                {
                    case "1": ActualizarValorEnMatrixBase(); break;
                    default:
                        break;
                }
            }
        }

        private void ActualizarValorEnMatrixBase()
        {
            if(mForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
            {
                //SAPbouiCOM.Matrix matrix = FormBase.Items.Item("mtxCort").Specific;
                Observacion = mForm.DataSources.UserDataSources.Item("UD_0").Value;
                string colId = string.Empty;

                switch (MatrixBase.Item.UniqueID)
                {
                    case "mtxExtru": colId = "Col_15"; break;
                    case "mtxImpr": colId = "Col_27"; break;

                    case "mtxLamina":
                    case "mtxSela":
                    case "mtxCort":
                    case "mtxHab":
                    case "mtxRebo":
                    case "mtxServ": colId = "Col_12"; break;

                    default: break;
                }

                MatrixBase.GetCellSpecific(colId, FilaMatrix).Value = Observacion;
                mForm.Close();
            }
        }

        public bool HandleMenuDataEvents(SAPbouiCOM.MenuEvent menuEvent)
        {
            return true;
        }

        public bool HandleRightClickEvent(SAPbouiCOM.ContextMenuInfo menuInfo)
        {
            return true;
        }
    }
}
