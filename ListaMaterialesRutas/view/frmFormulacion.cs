using AddonListaMaterialesYrutas.commons;
using AddonListaMaterialesYrutas.conexion;
using AddonListaMaterialesYrutas.data_schema;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AddonListaMaterialesYrutas.view
{
    public class frmFormulacion : FormCommon, commons.IForm
    {
        #region variables
        private SAPbouiCOM.Form mForm;
        private SAPbouiCOM.DBDataSource dsOFRM;
        private SAPbouiCOM.DBDataSource dsFRRUTA, dsFORMUL, dsSUBPRD, dsINDUCT, dsFREXTR, dsFRIMPR, dsLAMINA, dsFRSELL, dsFRCORT, dsFRHABI, dsFRREBO, dsFRSERV;
        private SAPbouiCOM.UserDataSource udRUTA;
        private SAPbouiCOM.Matrix oMatrix;
        private SAPbouiCOM.ComboBox cboCodTax, cboCodExp, cboCodRuta, cboCodRutaExt, cboCodMateriales;

        //CONST PARA LAYOUT
        private const string TYPE = "EXP_OFRM";
        public const string MENU = "FormuRLM";
        public const string PATHRPT = "Resources/prueba.rpt";
        public const string TYPENAME = "Formulacion";
        public const string ADDONNAME = "ListaMateriales";

        private const string EDT_CUENTA = "edtAct", EDT_ITEM = "edtItem", EDT_ENTRY = "edtEnt";  //EditTexts Header
        private const string EDT_FORPOR1 = "edtPor1", EDT_FORPOR2 = "edtPor2", EDT_FORPOR3 = "edtPor3", EDT_FORPOR4 = "edtPor4", EDT_FORPOR5 = "edtPor5"; //EditTexts Porc
        private const string BTN_ADD = "btnAdd", BTN_ADDLINE = "btnAddLi", BTN_ADDRUTA = "btnAddR", BTN_OK = "1", BTN_VISTAPREV = "btnPrev";//Buttons
        private const string CBO_RUTA = "cboRuta";//Combo
        private const string MTX_FRRUTA = "mtxRuta", MTX_FORMUL = "mtxForm", MTX_SUBPRD = "mtxSubPr", MTX_INDUCT = "mtxInd", MTX_FREXTR = "mtxExtru", MTX_FRIMPR = "mtxImpr",
            MTX_LAMINA = "mtxLamina", MTX_FRSELL = "mtxSela", MTX_FRCORT = "mtxCort", MTX_FRHABI = "mtxHab", MTX_FRREBO = "mtxRebo", MTX_FRSERV = "mtxServ"; //Matrix
        private const string UD_RUTA = "UdRuta";
        private const string C_CODRUT = "Col_0", C_CODRUTO = "Col_2";
        private const string C_FOR_KGA = "Col_3", C_FOR_KGB = "Col_4", C_FOR_KGC = "Col_5", C_FOR_KGD = "Col_6", C_FOR_KGE = "Col_7";//FORMULA
        private const string C_RUT_RUTAS = "Col_1";//RUTA
        private const string C_EXT_RDE = "Col_RDe", C_EXT_MAT = "Col_9", C_EXT_UMANC = "Col_23", C_EXT_MEANC = "Col_4", C_EXT_UMESP = "Col_16", C_EXT_MANGA = "Col_6", C_EXT_TRAT = "Col_8";//EXTRUSION
        private const string C_IMP_MAT = "Col_14", C_IMP_UMESP = "Col_10", C_IMP_SENT = "Col_30", C_IMP_NOCIL = "Col_15", C_IMP_DESA = "Col_16", C_IMP_REPETI = "Col_13", C_IMP_NRBAND = "Col_12", C_IMP_MEBAND = "Col_41", C_IMP_MPRIMA= "Col_MP",
            C_IMP_T1 = "Col_28", C_IMP_T8 = "Col_29", C_IMP_T2 = "Col_4", C_IMP_T3 = "Col_6", C_IMP_T4 = "Col_31", C_IMP_T5 = "Col_32", C_IMP_T6 = "Col_33", C_IMP_T7 = "Col_34";//IMPRESION
        private const string C_LAM_MAT1 = "Col_8", C_LAM_UMANC = "Col_14", C_LAM_UMESP = "Col_6", C_LAM_MAT2 = "Col_15";//LAMINA
        private const string C_SEL_MAT = "Col_17", C_SEL_UMESP = "Col_14", C_SEL_UMFUE = "Col_6", C_SEL_TSEL1 = "Col_9", C_SEL_TSEL2 = "Col_19";//SELLADO
        private const string C_COR_SENT = "Col_8", C_COR_UMESP = "Col_11", C_COR_MAT = "Col_15";//CORTE
        private const string C_HAB_UMESP = "Col_8", C_HAB_MAT = "Col_9";//HABILITADO
        private const string C_REB_UMESP = "Col_4", C_REB_MAT = "Col_9";//REBOBINADO

        string codeLM = "1", origRuta = "";

        //Right Click
        private string ItemUIDRightClick;
        private int RowItemRightClick;
        private object eCommon;
        #endregion

        public frmFormulacion(Dictionary<string, commons.IForm> dictionary)
        {
            try
            {
                mForm = CreateForm(Conexion.company, Conexion.application, Properties.Resources.frmFormulacion, FormName.FORMUL_RLM);
                if (mForm != null)
                {
                    mForm.Freeze(true);
                    dictionary.Add(getFormUID(), (commons.IForm)this);
                    Initializer();
                    ConfigurarLayout();
                    mForm.Visible = true;
                }
                else
                    StatusMessageError("Construct: No se pudo crear el formulario " + FormName.FORMUL_RLM);
            }
            catch (Exception)
            {
                throw;
            }
            finally { if (mForm != null) mForm.Freeze(false); }
        }

        private void ConfigurarLayout()
        {
            LayoutService.CrearLayout(TYPENAME, ADDONNAME, TYPE, MENU, PATHRPT);
            mForm.ReportType = LayoutService.ObtenerTypeCode(TYPENAME, ADDONNAME, TYPE);
        }

        #region _EVENTOS_ITEMEVENT

        //Principal

        private void Initializer()
        {
            try
            {
                mForm.Freeze(true);
                Automanage();
                if (dsOFRM == null) dsOFRM = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_CABE}");
                dsFRRUTA = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRRUTA}");
                dsFORMUL = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FORMUL}");
                dsSUBPRD = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_SUBPRD}");
                dsINDUCT = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_INDUCT}");
                dsFREXTR = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FREXTR}");
                dsFRIMPR = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRIMPR}");
                dsLAMINA = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_LAMINA}");
                dsFRSELL = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRSELA}");
                dsFRCORT = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRCORT}");
                dsFRHABI = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRHABI}");
                dsFRREBO = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRREBO}");
                dsFRSERV = mForm.DataSources.DBDataSources.Item($"@{SCFormulacion.TABLE_FRSERV}");
                udRUTA = mForm.DataSources.UserDataSources.Item(UD_RUTA);
                cboCodRuta = mForm.Items.Item(CBO_RUTA).Specific;
                cboCodRutaExt = mForm.Items.Item(MTX_FREXTR).Specific.GetCellSpecific(C_EXT_RDE, 0);
                //cboCodTax = mForm.Items.Item("cboTax").Specific;
                LoadDefaults();
                AddTotalizadoresFormula();
            }
            catch (Exception ex)
            {
                StatusMessageError("cargarOpcionesPorDefecto > " + ex.Message);
            }
            finally { mForm.Freeze(false); }
        }

        private void Automanage()
        {
            mForm.Items.Item("btnPrev").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
            mForm.Items.Item("btnPrev").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 1, BoModeVisualBehavior.mvb_True);
        }

        private void AddTotalizadoresFormula()
        {
            SAPbouiCOM.Column oColumn;
            oMatrix = mForm.Items.Item(MTX_FORMUL).Specific;
            SumColumna((SAPbouiCOM.Column)oMatrix.Columns.Item(C_FOR_KGA));
            SumColumna((SAPbouiCOM.Column)oMatrix.Columns.Item(C_FOR_KGB));
            SumColumna((SAPbouiCOM.Column)oMatrix.Columns.Item(C_FOR_KGC));
            SumColumna((SAPbouiCOM.Column)oMatrix.Columns.Item(C_FOR_KGD));
            SumColumna((SAPbouiCOM.Column)oMatrix.Columns.Item(C_FOR_KGE));
        }
        private void LoadDefaults()
        {
            dsOFRM.SetValue("U_EXP_PRCAP1", 0, "100.00");
            LoadCombo();
            InstanciateComboRuta(cboCodRuta);
            UpdateCFLConditions();
        }

        private void UpdateCFLConditions()
        {
            SAPbouiCOM.ChooseFromListCollection oChooseFromListCollection = mForm.ChooseFromLists;
            try
            {
                SAPbouiCOM.Conditions oConditions = null;
                SAPbouiCOM.Condition oCondition = null;
                SAPbouiCOM.ChooseFromList oChooseFromList = null;

                UpdateCFL(oChooseFromListCollection.Item("CFL_ITFOR"), "U_EXX_TIPOEXIS", SAPbouiCOM.BoConditionOperation.co_EQUAL, "03");
                UpdateCFL(oChooseFromListCollection.Item("CFL_MER"), "U_EXP_TPRO", SAPbouiCOM.BoConditionOperation.co_EQUAL, "S");
                UpdateCFL(oChooseFromListCollection.Item("CFL_REF"), "U_EXP_TPRO", SAPbouiCOM.BoConditionOperation.co_EQUAL, "R");
                UpdateCFL(oChooseFromListCollection.Item("CFL_IND"), "U_EXP_TPRO", SAPbouiCOM.BoConditionOperation.co_EQUAL, "I");
                UpdateCFL(oChooseFromListCollection.Item("CFL_SUBPR"), "ItmsGrpCod", SAPbouiCOM.BoConditionOperation.co_EQUAL, "198");
                UpdateCFL(oChooseFromListCollection.Item("CFL_ITMSER"), "U_EXP_TPRO", SAPbouiCOM.BoConditionOperation.co_EQUAL, "V");
                //UpdateCFL(oChooseFromListCollection.Item("CFL_MQEX"), "U_EXP_TPRO", SAPbouiCOM.BoConditionOperation.co_EQUAL, "V");
            }
            catch (Exception e)
            {
                StatusMessageError("UpdateCFLConditions() > " + e.Message);
            }
            finally
            {
                LiberarObjetoGenerico(oChooseFromListCollection);
            }
        }

        private void UpdateCFL(ChooseFromList cfl, string alias1, BoConditionOperation boCondition1, string value1)
        {
            SAPbouiCOM.Conditions oConditions = null;
            SAPbouiCOM.Condition oCondition = null;
            try
            {
                oConditions = cfl.GetConditions();
                oCondition = oConditions.Add();
                oCondition.Alias = alias1;
                oCondition.Operation = boCondition1;
                oCondition.CondVal = value1;
                cfl.SetConditions(oConditions);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LiberarObjetoGenerico(cfl);
                LiberarObjetoGenerico(oConditions);
                LiberarObjetoGenerico(oCondition);
            }
        }

        private void LoadCombo()
        {
            try
            {
                oMatrix = mForm.Items.Item(MTX_FRRUTA).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_RUT_RUTAS, 0), Queries.GetComboRutas());

                oMatrix = mForm.Items.Item(MTX_FREXTR).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_EXT_MAT, 0), Queries.GetComboMateriales());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_EXT_UMANC, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_EXT_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_EXT_MANGA, 0));
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_EXT_TRAT, 0));

                oMatrix = mForm.Items.Item(MTX_FRIMPR).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_MAT, 0), Queries.GetComboMateriales());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T1, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T2, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T3, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T4, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T5, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T6, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T7, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_T8, 0), Queries.GetComboTinta());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_IMP_SENT, 0));

                oMatrix = mForm.Items.Item(MTX_LAMINA).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_LAM_MAT1, 0), Queries.GetComboMateriales());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_LAM_UMANC, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_LAM_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_LAM_MAT2, 0), Queries.GetComboMateriales());

                oMatrix = mForm.Items.Item(MTX_FRSELL).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_SEL_MAT, 0), Queries.GetComboMateriales());
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_SEL_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_SEL_UMFUE, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_SEL_TSEL1, 0));
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_SEL_TSEL2, 0));

                oMatrix = mForm.Items.Item(MTX_FRCORT).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_COR_SENT, 0));
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_COR_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_COR_MAT, 0), Queries.GetComboMateriales());

                oMatrix = mForm.Items.Item(MTX_FRHABI).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_HAB_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_HAB_MAT, 0), Queries.GetComboMateriales());

                oMatrix = mForm.Items.Item(MTX_FRREBO).Specific;
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_REB_UMESP, 0), Queries.GetComboUnidadMedidas(), false);
                InstanciateCombo((ComboBox)oMatrix.GetCellSpecific(C_REB_MAT, 0), Queries.GetComboMateriales());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool HandleItemEvents(SAPbouiCOM.ItemEvent itemEvent)
        {
            var result = true;
            try
            {
                switch (itemEvent.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                                if (!itemEvent.BeforeAction)
                            whenChooseFromList(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        result = WhenFormLoad(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        result = WhenItemPressed(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                        if (!itemEvent.BeforeAction)
                            result = WhenComboSelectAfter(itemEvent);
                        else
                            result = WhenComboSelectBefore(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD:
                        result = WhenDataAdd(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                        if (!itemEvent.BeforeAction && itemEvent.InnerEvent)
                            result = WhenLostFocus(itemEvent);
                        break;
                    case BoEventTypes.et_DOUBLE_CLICK:
                        if (!itemEvent.BeforeAction)
                            result = WhenDoubleClick(itemEvent);
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

        private bool WhenDoubleClick(ItemEvent itemEvent)
        {
            bool res = true;
            try
            {
                string colId = string.Empty;
                string matrixID = string.Empty; //PARA VALIDAR QUE HAYA CAIDO EN ALGUNA DE LAS MATRIZES

                switch (itemEvent.ItemUID)
                {
                    case MTX_FREXTR: matrixID = MTX_FREXTR;  colId = "Col_15"; break;
                    case MTX_FRIMPR: matrixID = MTX_FRIMPR; colId = "Col_27"; break;
                    case MTX_LAMINA: matrixID = MTX_LAMINA; colId = "Col_12"; break;
                    case MTX_FRSELL: matrixID = MTX_FRSELL; colId = "Col_12"; break;
                    case MTX_FRCORT: matrixID = MTX_FRCORT; colId = "Col_12";break;
                    case MTX_FRHABI: matrixID = MTX_FRHABI; colId = "Col_12"; break;
                    case MTX_FRREBO: matrixID = MTX_FRREBO; colId = "Col_12"; break;
                    case MTX_FRSERV: matrixID = MTX_FRSERV; colId = "Col_12"; break;

                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(matrixID) && itemEvent.Row > 0 && itemEvent.ColUID == colId)
                {
                    oMatrix = mForm.Items.Item(matrixID).Specific;
                    string observacion = oMatrix.GetCellSpecific(itemEvent.ColUID, itemEvent.Row).Value;

                    formObs fmrObservacion = new formObs(Conexion.formOpen, observacion, mForm, oMatrix, itemEvent.Row);
                }

                mForm.Freeze(true);
            }
            catch (Exception)
            {

                throw;
            }
            finally { mForm.Freeze(false); }
            return res;
        }

        //Método maneja evento
        private bool WhenLostFocus(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            try
            {
                mForm.Freeze(true);
                switch (oEvent.ItemUID)
                {
                    case EDT_FORPOR1:
                    case EDT_FORPOR2:
                    case EDT_FORPOR3:
                    case EDT_FORPOR4:
                    case EDT_FORPOR5:
                        res = double.Parse(dsOFRM.GetValue("U_EXP_PRCAP1", 0)) + double.Parse(dsOFRM.GetValue("U_EXP_PRCAP2", 0)) + double.Parse(dsOFRM.GetValue("U_EXP_PRCAP3", 0)) + double.Parse(dsOFRM.GetValue("U_EXP_PRCAP4", 0)) + double.Parse(dsOFRM.GetValue("U_EXP_PRCAP5", 0)) < 100;
                        StatusMessageWarning("El total de la formulación es mayor a 100%");
                        break;
                    case MTX_FREXTR:
                        MatrixLostFocus(oEvent, dsFREXTR, Constantes.RT_EXTRUS);
                        if (oEvent.ColUID.Equals(C_EXT_UMANC) || oEvent.ColUID.Equals(C_EXT_MEANC))
                        {
                            CalcularAnchoExtrusion(oEvent);
                        }
                        break;
                    case MTX_FRIMPR:
                        MatrixLostFocus(oEvent, dsFRIMPR, Constantes.RT_IMPRES);
                        if (oEvent.ColUID.Equals(C_IMP_DESA) || oEvent.ColUID.Equals(C_IMP_REPETI))
                        {
                            oMatrix.FlushToDataSource();
                            dsFRIMPR.SetValue("U_EXP_FREC", oEvent.Row - 1, (double.Parse(dsFRIMPR.GetValue("U_EXP_DESA", oEvent.Row - 1), CultureInfo.InvariantCulture) / double.Parse(dsFRIMPR.GetValue("U_EXP_REPT", oEvent.Row - 1), CultureInfo.InvariantCulture)).ToString());
                            oMatrix.LoadFromDataSource();
                            oMatrix.AutoResizeColumns();
                        }
                        if (oEvent.ColUID.Equals(C_IMP_NRBAND) || oEvent.ColUID.Equals(C_IMP_MEBAND))
                        {
                            CalcularAnchoExtrusion(oEvent, true);
                        }
                        if (oEvent.ColUID.Equals(C_IMP_MPRIMA))
                        {
                            
                        }
                        break;
                    case MTX_LAMINA:
                        MatrixLostFocus(oEvent, dsLAMINA, Constantes.RT_LAMINA);
                        break;
                    case MTX_FRSELL:
                        MatrixLostFocus(oEvent, dsFRSELL, Constantes.RT_SELLAD);
                        break;
                    case MTX_FRCORT:
                        MatrixLostFocus(oEvent, dsFRCORT, Constantes.RT_CORTE);
                        break;
                    case MTX_FRHABI:
                        MatrixLostFocus(oEvent, dsFRHABI, Constantes.RT_HABILI);
                        break;
                    case MTX_FRREBO:
                        MatrixLostFocus(oEvent, dsFRREBO, Constantes.RT_REBOBI);
                        break;
                    case MTX_FRSERV:
                        MatrixLostFocus(oEvent, dsFRSERV, Constantes.RT_SERV);
                        break;
                    case MTX_FORMUL:
                        MatrixLostFocus(oEvent, dsFORMUL, "FORMUL");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

                mForm.Freeze(false);
            }

            return res;
        }
        private void CalcularAnchoExtrusion(SAPbouiCOM.ItemEvent oEvent, bool impUpdate = false)
        {
            double ancho = 0;
            double refile = 0;
            double factor = 0;
            oMatrix.FlushToDataSource();
            double a = double.Parse(dsFRIMPR.GetValue("U_EXP_MEBAND", oEvent.Row - 1), CultureInfo.InvariantCulture);
            double b = double.Parse(dsFRIMPR.GetValue("U_EXP_BAND", oEvent.Row - 1), CultureInfo.InvariantCulture);
            double c = double.TryParse(GenericQuery(Queries.GetFactor(dsOFRM.GetValue("U_EXP_ARTI", 0))), out factor) ? factor : 0;
            double d = double.TryParse(GenericQuery(Queries.GetRefile(dsOFRM.GetValue("U_EXP_ARTI", 0))), out refile) ? refile : 0;

            if (impUpdate)
            {
                ancho =
                    (double.Parse(dsFRIMPR.GetValue("U_EXP_MEBAND", oEvent.Row - 1), CultureInfo.InvariantCulture) * //MEDIDA BANDA
                    double.Parse(dsFRIMPR.GetValue("U_EXP_BAND", oEvent.Row - 1), CultureInfo.InvariantCulture)) * //Nro BANDAS
                    (double.TryParse(GenericQuery(Queries.GetFactor(dsOFRM.GetValue("U_EXP_ARTI", 0))), out factor) ? factor : 0)
                    + (double.TryParse(GenericQuery(Queries.GetRefile(dsOFRM.GetValue("U_EXP_ARTI", 0))), out refile) ? refile : 0);//REFILE

                oMatrix.LoadFromDataSource();
                oMatrix.AutoResizeColumns();
            }
            else
            {
                ancho = double.Parse(dsFREXTR.GetValue("U_EXP_ANCHO", 0), CultureInfo.InvariantCulture);
            }
            dsFREXTR.SetValue("U_EXP_ANCHO", 0, ancho.ToString());
            oMatrix = mForm.Items.Item(MTX_FREXTR).Specific;
            oMatrix.LoadFromDataSource();
            dsFRCORT.SetValue("U_EXP_ANCHO", 0, ancho.ToString());
            oMatrix = mForm.Items.Item(MTX_FRCORT).Specific;
            oMatrix.LoadFromDataSource();
            dsFRREBO.SetValue("U_EXP_ANCHO", 0, ancho.ToString());
            oMatrix = mForm.Items.Item(MTX_FRREBO).Specific;
            oMatrix.LoadFromDataSource();
            dsFRIMPR.SetValue("U_EXP_MEANC", 0, ancho.ToString());
            oMatrix = mForm.Items.Item(MTX_FRIMPR).Specific;
            oMatrix.LoadFromDataSource();
        }
        private bool MatrixLostFocus(SAPbouiCOM.ItemEvent oEvent, SAPbouiCOM.DBDataSource ds, string tipo)
        {
            oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;
            switch (tipo)
            {
                case "FORMUL":
                    if (oEvent.ColUID.Equals(C_FOR_KGA))
                    {
                        CalcularPorcentajesFormula();
                        mForm.Mode = BoFormMode.fm_UPDATE_MODE;
                        
                    }
                    break;
                case Constantes.RT_EXTRUS:
                    if (oEvent.ColUID.Equals(C_CODRUT))
                    {
                        oMatrix.FlushToDataSource();
                        ds.SetValue("U_EXP_NOMRUTDE", oEvent.Row - 1, GetRutaDesc(ds.GetValue("U_EXP_CODRUTDE", oEvent.Row - 1).Trim(), Constantes.RutaBase, tipo));
                        ds.SetValue("U_EXP_NMRTDE", oEvent.Row - 1, GetRutaDesc(ds.GetValue("U_EXP_CDRTDE", oEvent.Row - 1).Trim(), Constantes.RutaDestino, tipo, ds.GetValue("U_EXP_CODRUTDE", oEvent.Row - 1).Trim()));
                        oMatrix.LoadFromDataSource();
                        oMatrix.AutoResizeColumns();
                    }
                    break;
                default:
                    if (oEvent.ColUID.Equals(C_CODRUT) || oEvent.ColUID.Equals(C_CODRUTO))
                    {
                        oMatrix.FlushToDataSource();
                        ds.SetValue("U_EXP_NOMRUT", oEvent.Row - 1, GetRutaDesc(ds.GetValue("U_EXP_CODRUT", oEvent.Row - 1).Trim(), Constantes.RutaBase, tipo));
                        ds.SetValue("U_EXP_NOMRUTOR", oEvent.Row - 1, GetRutaDesc(ds.GetValue("U_EXP_CODRUTOR", oEvent.Row - 1).Trim(), Constantes.RutaOrigen, tipo, ds.GetValue("U_EXP_CODRUT", oEvent.Row - 1).Trim()));
                        oMatrix.LoadFromDataSource();
                        oMatrix.AutoResizeColumns();
                    }
                    break;
            }
            return true;
        }

        private bool WhenFormLoad(SAPbouiCOM.ItemEvent oEvent)
        {
            if (!oEvent.BeforeAction) return true;
            try
            {
                mForm.Freeze(true);
            }
            catch (Exception)
            {
                throw;
            }
            finally { mForm.Freeze(false); }

            return true;
        }

        private bool WhenItemPressed(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            try
            {
                switch (oEvent.ItemUID)
                {
                    case BTN_VISTAPREV:
                        if (!oEvent.BeforeAction)
                            Conexion.application.ActivateMenuItem(Constantes.Menu_ViewLayout); //ACTIVAMOS EL MENU DEL KEYPRINTLAYOUT
                        break;
                    case BTN_OK:
                        if (oEvent.BeforeAction)
                        {
                            oMatrix = mForm.Items.Item(MTX_FRRUTA).Specific;
                            oMatrix.FlushToDataSource();
                            if (!string.IsNullOrEmpty(dsFRRUTA.GetValue("U_EXP_SUBPRD", dsFRRUTA.Size - 1)))
                            {
                                StatusMessageWarning("Última etapa no debe tener subproducto.");
                                res = false;
                            }
                        }
                        else
                        {
                            if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE && oEvent.ActionSuccess)
                            {
                                mForm.Freeze(true);
                                mForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
                                //dsFORMUL.SetValue("Code", 0, codeLM);
                                mForm.Items.Item("edtCode").Specific.Value = codeLM;
                                mForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                                mForm.Freeze(false);
                            }
                        }
                        break;
                    case BTN_ADDLINE:
                        if (!oEvent.BeforeAction)
                            AddSpecificMatrix(MTX_FORMUL, dsFORMUL);
                        break;
                    case BTN_ADDRUTA:
                        if (!oEvent.BeforeAction)
                            AddRow(MTX_FRRUTA);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { mForm.Freeze(false); }
            return res;
        }
        private bool WhenComboSelectBefore(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            try
            {
                switch (oEvent.ItemUID)
                {
                    case MTX_FRRUTA:
                        origRuta = dsFRRUTA.GetValue("U_EXP_CODRUT", oEvent.Row - 1);
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return res;
        }
        private bool WhenComboSelectAfter(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            try
            {
                mForm.Freeze(true);
                switch (oEvent.ItemUID)
                {
                    case MTX_FRRUTA:

                        LimpiarInductoresAnteriores(oEvent); //LIMPIAMOS LA MATRIX DE INDUCTORES PARA NO ACUMULAR

                        oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;
                        oMatrix.FlushToDataSource();
                        if (!origRuta.Equals(dsFRRUTA.GetValue("U_EXP_CODRUT", oEvent.Row - 1)))
                        {
                            res = SetRuta(oEvent);
                            oMatrix.LoadFromDataSource();
                        }

                        OrdenarInductores();
                        InstanciateComboRuta(cboCodRuta);
                        InstanciateComboRuta(cboCodRutaExt, true);


                        break;

                    case MTX_FREXTR:
                        if (oEvent.ColUID.Equals(C_EXT_RDE))
                        {
                            oMatrix.FlushToDataSource();

                            dsFREXTR.SetValue("U_EXP_NMRTDE", oEvent.Row - 1, GetRutaDesc(dsFREXTR.GetValue("U_EXP_CDRTDE", oEvent.Row - 1).Trim(), Constantes.RutaDestino, Constantes.RT_EXTRUS, dsFREXTR.GetValue("U_EXP_CODRUTDE", oEvent.Row - 1).Trim()));

                            oMatrix.LoadFromDataSource();
                            //oMatrix.AutoResizeColumns();
                        }
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                mForm.Freeze(false);
            }
            return res;
        }

        private void OrdenarInductores( )
        {
            SAPbouiCOM.Matrix matrixInductor = mForm.Items.Item(MTX_INDUCT).Specific;
            List<Tuple<string, string, bool>> Listinduc = new List<Tuple<string, string, bool>>();
            for (int i = 1; i <= matrixInductor.RowCount; i++)
            {
                var induc = Tuple.Create(
                    matrixInductor.GetCellSpecific("Col_0", i).Value,
                    matrixInductor.GetCellSpecific("Col_1", i).Value,
                    ((CheckBox)(matrixInductor.Columns.Item("Col_2").Cells.Item(i).Specific)).Checked
                    );
                Listinduc.Add(induc);

            }

            matrixInductor.Clear();
            int cont = 1;
            foreach (var item in Listinduc.OrderBy(x => x.Item1).Distinct())
            {
                if (!string.IsNullOrEmpty(item.Item1))
                {
                    matrixInductor.AddRow();

                    var codRut = (EditText)matrixInductor.Columns.Item("Col_0").Cells.Item(cont).Specific;
                    codRut.Value = item.Item1;

                    var ind = (EditText)matrixInductor.Columns.Item("Col_1").Cells.Item(cont).Specific;
                    ind.Value = item.Item2;

                    var act = (CheckBox)matrixInductor.Columns.Item("Col_2").Cells.Item(cont).Specific;
                    act.Checked = item.Item3;

                    cont++;
                }

            }
        }

        private void LimpiarInductoresAnteriores(ItemEvent oEvent)
        {
            if (oEvent.Row > 0 && oEvent.ItemChanged && !oEvent.InnerEvent)
            {
                try
                {
                    oMatrix = mForm.Items.Item(MTX_FRRUTA).Specific;
                    ComboBox cmb = oMatrix.GetCellSpecific("Col_1", oEvent.Row);
                    string ruta = cmb.Selected.Value;
                    if (!string.IsNullOrEmpty(ruta))
                        ActualizarInductor(ruta, oEvent.Row);
                }
                catch (Exception ex)
                {
                    ; //PORQUE ESTE EVENTO SE DISPARA ANTES DE CARGAR EL COMBO
                }
            }
        }

        private void ActualizarInductor(string ruta, int row)
        {
            try
            {
                mForm.Freeze(true);
                SAPbobsCOM.Recordset rutaRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rutaRS.DoQuery(Queries.GetRutaValues(ruta));
                string inductor = rutaRS.Fields.Item("INDUCTOR").Value.ToString();

                Matrix matrixInductor = mForm.Items.Item(MTX_INDUCT).Specific;
                string codigoRuta = string.Empty;

                List<int> indiceInductores = new List<int>();
                bool hayInductor = true;

                while(hayInductor && matrixInductor.RowCount > 0)
                {
                    for (int i = 1; i <= matrixInductor.RowCount; i++)
                    {
                        codigoRuta = matrixInductor.GetCellSpecific("Col_0", i).Value;
                        if (codigoRuta == row.ToString())
                        {
                            hayInductor = true;
                            indiceInductores.Add(i);
                            matrixInductor.DeleteRow(i);    //SI NO HAY INDUCTOR Y PREVIAMENTE SE TENIA, SE ELIMINA LA FILA
                            break;
                        }
                        else
                            hayInductor = false;
                    }
                }

                List<Tuple<string, string, bool>> Listinduc = new List<Tuple<string, string, bool>>();
                for (int i = 1; i <= matrixInductor.RowCount; i++)
                {
                    var induc = Tuple.Create(
                        matrixInductor.GetCellSpecific("Col_0", i).Value,
                        matrixInductor.GetCellSpecific("Col_1", i).Value,
                        ((CheckBox)(matrixInductor.Columns.Item("Col_2").Cells.Item(i).Specific)).Checked
                        );
                    Listinduc.Add(induc);                
                 
                }

                matrixInductor.Clear();
                int cont = 1;
                foreach (var item in Listinduc.OrderBy(x => x.Item1).Distinct())
                {
                    if (!string.IsNullOrEmpty(item.Item1))
                    {
                        matrixInductor.AddRow();

                        var codRut = (EditText)matrixInductor.Columns.Item("Col_0").Cells.Item(cont).Specific;
                        codRut.Value = item.Item1;

                        var ind = (EditText)matrixInductor.Columns.Item("Col_1").Cells.Item(cont).Specific;
                        ind.Value = item.Item2;

                        var act = (CheckBox)matrixInductor.Columns.Item("Col_2").Cells.Item(cont).Specific;
                        act.Checked = item.Item3;

                        cont++;
                    }
                  
                }


                //int filasEliminadas = 0;
                //int numInductor = 0;

                //while (filasEliminadas != indiceInductores.Count)
                //{
                //    string codigoRutaInductor = 

                //    for (int i = 1; i <= matrixInductor.RowCount; i++)
                //    {
                //        codigoRuta = matrixInductor.GetCellSpecific("Col_0", i).Value;
                //        if (codigoRuta == indiceInductores.ToString())
                //        {
                //            indiceInductores.Add(i);
                //            //if (!string.IsNullOrEmpty(inductor))
                //            //    matrixInductor.GetCellSpecific("Col_1", i).Value = inductor;
                //            //else
                //            //{
                //            //    matrixInductor.DeleteRow(i);    //SI NO HAY INDUCTOR Y PREVIAMENTE SE TENIA, SE ELIMINA LA FILA
                //            //    break;
                //            //}
                //        }
                //    }
                //}
            }
            catch (Exception)
            {
                ;
            }
            finally { mForm.Freeze(false); }
        }

        private bool CheckDataSourceRuta(SAPbouiCOM.DBDataSource ds, string codeRut, string matrix)
        {
            try
            {
                if (matrix.Equals(MTX_INDUCT)) return false;
                for (int i = 0; i < ds.Size; i++)
                {
                    if ((matrix.Equals(MTX_FREXTR) && ds.GetValue("U_EXP_CODRUTDE", i).Equals(codeRut)) || (!matrix.Equals(MTX_FREXTR) && ds.GetValue("U_EXP_CODRUT", i).Equals(codeRut)))
                        return true;
                }
            }
            catch (Exception) { }
            return false;
        }

        private bool SetRuta(SAPbouiCOM.ItemEvent oEvent)
        {
            SAPbouiCOM.ComboBox oCombo = null;
            SAPbobsCOM.Recordset rutaRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string lineRut = "";
            try
            {
                oCombo = oMatrix.GetCellSpecific(C_RUT_RUTAS, oEvent.Row - 1);
                lineRut = dsFRRUTA.GetValue("LineId", oEvent.Row - 1);
                rutaRS.DoQuery(Queries.GetRutaValues(dsFRRUTA.GetValue("U_EXP_CODRUT", oEvent.Row - 1)));
                
                if (rutaRS.RecordCount > 0)
                {
                    dsFRRUTA.SetValue("U_EXP_NOMRUT", oEvent.Row - 1, rutaRS.Fields.Item("Value").Value.ToString());
                    dsFRRUTA.SetValue("U_EXP_SUBPRD", oEvent.Row - 1, rutaRS.Fields.Item("SUBPRD").Value.ToString());
                    dsFRRUTA.SetValue("U_EXP_SCRAP", oEvent.Row - 1, rutaRS.Fields.Item("SCRAP").Value.ToString());
                    dsFRRUTA.SetValue("U_EXP_REFILE", oEvent.Row - 1, rutaRS.Fields.Item("REFILE").Value.ToString());
                    oMatrix.LoadFromDataSource();

                    ValidateRutSpecificMatrix(dsFRRUTA.GetValue("U_EXP_CODRUT", oEvent.Row - 1), rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                    switch (dsFRRUTA.GetValue("U_EXP_CODRUT", oEvent.Row - 1))
                    {
                        case Constantes.RT_EXTRUS:
                            AddSpecificMatrix(MTX_FREXTR, dsFREXTR, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_IMPRES:
                            AddSpecificMatrix(MTX_FRIMPR, dsFRIMPR, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_LAMINA:
                            AddSpecificMatrix(MTX_LAMINA, dsLAMINA, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_SELLAD:
                            AddSpecificMatrix(MTX_FRSELL, dsFRSELL, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_CORTE:
                            AddSpecificMatrix(MTX_FRCORT, dsFRCORT, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_HABILI:
                            AddSpecificMatrix(MTX_FRHABI, dsFRHABI, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_REBOBI:
                            AddSpecificMatrix(MTX_FRREBO, dsFRREBO, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                        case Constantes.RT_SERV:
                            AddSpecificMatrix(MTX_FRSERV, dsFRSERV, rutaRS.Fields.Item("Value").Value.ToString(), lineRut);
                            break;
                    }

                    while (!rutaRS.EoF)
                    {
                        if (!String.IsNullOrEmpty(rutaRS.Fields.Item("INDUCTOR").Value.ToString()))
                            AddSpecificMatrix(MTX_INDUCT, dsINDUCT, rutaRS.Fields.Item("INDUCTOR").Value.ToString(), lineRut);
                        rutaRS.MoveNext();
                    }
                }
                //oMatrix.FlushToDataSource();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                LiberarObjetoGenerico(oCombo);
                LiberarObjetoGenerico(rutaRS);
            }
            return true;
        }

        private bool ValidateRutSpecificMatrix(string Ruta, string aux = "", string codeRut = "")
        {
            mForm.Freeze(true);
            var row = 0;
            try
            {
                oMatrix = mForm.Items.Item(MTX_FREXTR).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_FRIMPR).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_LAMINA).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_FRSELL).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_FRCORT).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_FRHABI).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_FRREBO).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }
                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

                oMatrix = mForm.Items.Item(MTX_FRSERV).Specific;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    var codRut = (EditText)oMatrix.Columns.Item(C_CODRUT).Cells.Item(i).Specific;

                    if (codRut.Value == codeRut)
                    {
                        row = i;
                    }

                }

                if (row != 0)
                {
                    oMatrix.DeleteRow(row);
                    return true;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                mForm.Freeze(false);
            }
           
         

           
            return true;
        }

        private bool WhenDataAdd(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {
            }
            return res;
        }
        #endregion
        private void whenChooseFromList(SAPbouiCOM.ItemEvent oEvent)
        {
            try
            {
                var cflEvnt = (ChooseFromListEvent)oEvent;
                //SAPbouiCOM.IChooseFromListEvent oChooseFromListEvent = (SAPbouiCOM.IChooseFromListEvent)oEvent;
                //SAPbouiCOM.DataTable oDataTable = oChooseFromListEvent.SelectedObjects;

               //cflEvnt.SelectedObjects is DataTable dtbl && e.InnerEvent == true

                if (cflEvnt.SelectedObjects is DataTable dtbl && oEvent.InnerEvent == true)
                {
                    //oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;

                    switch (oEvent.ItemUID)
                    {
                        case MTX_FORMUL:
                            oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;
                            oMatrix.FlushToDataSource();
                            dsFORMUL.SetValue("U_EXP_CODART", oEvent.Row - 1, dtbl.GetValue("ItemCode", 0).Trim());
                            dsFORMUL.SetValue("U_EXP_DESCRP", oEvent.Row - 1, dtbl.GetValue("ItemName", 0).Trim());
                            //oMatrix.FlushToDataSource();
                            oMatrix.LoadFromDataSource();
                            mForm.Mode = BoFormMode.fm_UPDATE_MODE;
                            break;
                        case MTX_FREXTR:
                            oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;
                            switch (oEvent.ColUID)
                            {
                                
                                case "Col_Maq":
                                    oMatrix.FlushToDataSource();
                                    dsFREXTR.SetValue("U_EXP_RECMAQ", oEvent.Row - 1, dtbl.GetValue("VisResCode", 0).Trim());
                                    oMatrix.LoadFromDataSource();
                                    mForm.Mode = BoFormMode.fm_UPDATE_MODE;

                                    break;
                            }
                            break;
                        case MTX_FRIMPR:

                            oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;
                            switch (oEvent.ColUID)
                            {
                                case "Col_MP":
                                    oMatrix.FlushToDataSource();
                                    dsFRIMPR.SetValue("U_EXP_MPRIMA", oEvent.Row - 1, dtbl.GetValue("ItemCode", 0).Trim());
                                    dsFRIMPR.SetValue("U_EXP_DESA", oEvent.Row - 1, (double.Parse(dsFRIMPR.GetValue("U_EXP_DESA", oEvent.Row - 1), CultureInfo.InvariantCulture)).ToString());
                                    //oMatrix.FlushToDataSource();
                                    oMatrix.LoadFromDataSource();
                                    mForm.Mode = BoFormMode.fm_UPDATE_MODE;
                                    break;
                                case "Col_Maq":
                                    oMatrix.FlushToDataSource();
                                    dsFRIMPR.SetValue("U_EXP_RECMAQ", oEvent.Row - 1, dtbl.GetValue("VisResCode", 0).Trim());
                                    oMatrix.LoadFromDataSource();
                                    mForm.Mode = BoFormMode.fm_UPDATE_MODE;
                                    break;
                            }

                            //var x = (EditText)(oMatrix.Columns.Item(C_IMP_DESA).Cells.Item(oEvent.Row).Specific);
                            //x.TabOrder = 150;
                            //oMatrix.AutoResizeColumns();
                            break;
                        case MTX_LAMINA:
                        case MTX_FRSELL:
                        case MTX_FRCORT:
                        case MTX_FRHABI:
                        case MTX_FRREBO:
                            UpdateChoose(GetDS(oEvent.ItemUID), oEvent, dtbl);
                            break;
                        case EDT_ITEM:
                            if (mForm.Mode == BoFormMode.fm_FIND_MODE) return;
                            dsOFRM.SetValue("U_EXP_ARTI", 0, dtbl.GetValue("ItemCode", 0).Trim());
                            dsOFRM.SetValue("U_EXP_ARTD", 0, dtbl.GetValue("ItemName", 0).Trim());
                            if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                            {
                                getNextFormCode(dtbl.GetValue("ItemCode", 0).Trim());
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                StatusMessageError("whenChooseFromList > " + e.Message);
            }
            finally
            {
                //ChangeUpdateMode();
            }
        }
        private void getNextFormCode(string itemcode)
        {
            dsOFRM.SetValue("Code", 0, itemcode + "-" + GenericQuery(Queries.GetNextFormCode(itemcode)));
        }

        private void UpdateChoose(SAPbouiCOM.DBDataSource ds, SAPbouiCOM.ItemEvent oEvent, SAPbouiCOM.DataTable oDataTable)
        {
            oMatrix = mForm.Items.Item(oEvent.ItemUID).Specific;
            oMatrix.FlushToDataSource();

            switch (oEvent.ColUID)
            {
                case "Col_Maq":

                    try{ oMatrix.GetCellSpecific("Col_Maq", oEvent.Row).Value = oDataTable.GetValue("ResCode", 0).ToString(); } catch (Exception){}

                    string columnaVelocidadMaquina = GetColVelocidadMaquina(ds);
                    string velocidad = oDataTable.GetValue("U_EXP_VELMAQ", 0).ToString();
                    try { oMatrix.GetCellSpecific(columnaVelocidadMaquina, oEvent.Row).Value = oDataTable.GetValue("U_EXP_VELMAQ", 0); } catch (Exception) { } ;

                    //ds.SetValue("U_EXP_RECMAQ", oEvent.Row - 1, oDataTable.GetValue("ResCode", 0).Trim());
                    //ds.SetValue("U_EXP_VELMAQ", oEvent.Row - 1, oDataTable.GetValue("U_EXP_VELMAQ", 0));
                    break;
                case "Col_MP":
                    try { oMatrix.GetCellSpecific("Col_MP", oEvent.Row).Value = oDataTable.GetValue("ItemCode", 0).Trim(); } catch (Exception) { }
                    try { oMatrix.GetCellSpecific("Col_16", oEvent.Row).Value = "0.00"; } catch (Exception) { }
                    //ds.SetValue("U_EXP_MPRIMA", oEvent.Row - 1, oDataTable.GetValue("ItemCode", 0).Trim());
                    break;
                default:
                    break;
            }

            if(ds == dsLAMINA && oEvent.ColUID == "Col_4")
            {
                try { oMatrix.GetCellSpecific("Col_4", oEvent.Row).Value = oDataTable.GetValue("ItemCode", 0).Trim(); } catch (Exception) { }
            }

            //oMatrix.FlushToDataSource();
            //oMatrix.LoadFromDataSource();
        }

        private string GetColVelocidadMaquina(DBDataSource ds)
        {
            switch (ds.TableName)
            {
                case "@EXP_LAMINA": return "Col_7";
                case "@EXP_FREXTR": return "Col_20";
                case "@EXP_FRIMPR": return "Col_8";
                case "@EXP_FRSELA": return "Col_10";
                case "@EXP_FRCORT": return "Col_7";
                case "@EXP_FRHABI": return "Col_7";
                case "@EXP_FRREBO": return "Col_7";


                default: return "";
                    break;
            }
        }

        public bool HandleFormDataEvents(SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo)
        {
            switch (oBusinessObjectInfo.EventType)
            {
                case SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD:
                    if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE && !oBusinessObjectInfo.BeforeAction && oBusinessObjectInfo.ActionSuccess)
                    {
                        codeLM = dsFORMUL.GetValue("Code", 0);
                    }
                    break;
                case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                    if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE && !oBusinessObjectInfo.BeforeAction && oBusinessObjectInfo.ActionSuccess)
                    {
                        WhenDataLoad(oBusinessObjectInfo);
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        private bool WhenDataLoad(BusinessObjectInfo oBusinessObjectInfo)
        {
            bool res = true;
            InstanciateComboRuta(cboCodRuta);
            InstanciateComboRuta(cboCodRutaExt, true);
            return res;
        }

        public bool HandleMenuDataEvents(SAPbouiCOM.MenuEvent menuEvent)
        {
            var result = true;
            try
            {
                if (menuEvent.BeforeAction)
                {
                    switch (menuEvent.MenuUID)
                    {
                        case Constantes.Menu_EliminarLinea:
                            DeleteRow(RowItemRightClick, ItemUIDRightClick);
                            break;
                        case Constantes.Menu_AgregarLinea:
                            AddRowRightClick(RowItemRightClick,ItemUIDRightClick);
                            break;
                        case Constantes.Menu_Duplicar:
                            result = DuplicateObject();
                            break;
                    }
                }
                else
                {
                    switch (menuEvent.MenuUID)
                    {
                        case Constantes.Menu_Crear:
                            dsOFRM.SetValue("U_EXP_PRCAP1", 0, "100.00");
                            break;
                        case Constantes.Menu_Buscar:
                            break;
                        case Constantes.Registro_Datos_Siguiente:
                        case Constantes.Registro_Datos_Anterior:
                        case Constantes.Primer_Registro_Datos:
                        case Constantes.Ultimo_Registro_Datos:


                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessageError("HandleMenuDataEvents > " + ex.Message);
            }
            return result;
        }

        #region _EVENTS_RIGHTCLICK
        public bool HandleRightClickEvent(SAPbouiCOM.ContextMenuInfo menuInfo)
        {
            var result = true;
            SAPbouiCOM.MenuItem oMenuItem;
            SAPbouiCOM.Menus oMenus;
            if (menuInfo.BeforeAction)
            {
                try
                {
                    if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE || mForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                        || mForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE)
                    {
                        SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                        oCreationPackage = Conexion.application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
                        oMenuItem = Conexion.application.Menus.Item(Constantes.Menu_Context);
                        oMenus = oMenuItem.SubMenus;

                        ItemUIDRightClick = menuInfo.ItemUID;
                        RowItemRightClick = menuInfo.Row;
                        if (ItemUIDRightClick.StartsWith("mtx"))
                        {
                            if (menuInfo.Row > 0 && !oMenus.Exists(Constantes.Menu_AgregarLinea))
                            {
                                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                                oCreationPackage.UniqueID = Constantes.Menu_AgregarLinea;
                                oCreationPackage.String = Constantes.Menu_AgregarLineaDescripcion;
                                oCreationPackage.Position = 100;
                                oCreationPackage.Enabled = true;
                                oMenus.AddEx(oCreationPackage);
                            }
                            if (menuInfo.Row > 0 && !oMenus.Exists(Constantes.Menu_EliminarLinea))
                            {
                                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                                oCreationPackage.UniqueID = Constantes.Menu_EliminarLinea;
                                oCreationPackage.String = Constantes.Menu_EliminarLineaDescripcion;
                                oCreationPackage.Position = 101;
                                oCreationPackage.Enabled = true;
                                oMenus.AddEx(oCreationPackage);
                            }
                        }
                        if (!oMenus.Exists(Constantes.Menu_Duplicar))
                        {
                            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                            oCreationPackage.UniqueID = Constantes.Menu_Duplicar;
                            oCreationPackage.String = Constantes.Menu_DuplicarDescripcion;
                            oCreationPackage.Position = 102;
                            oCreationPackage.Enabled = true;
                            oMenus.AddEx(oCreationPackage);
                        }
                    }
                }
                catch (Exception e)
                {
                    StatusMessageError("HandleRightClickEvent > BeforeAction > " + e.Message);
                }
            }
            else if (!menuInfo.BeforeAction)
            {
                try
                {
                    if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE || mForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
                    {
                        if (menuInfo.Row > 0)
                            Conexion.application.Menus.RemoveEx(Constantes.Menu_EliminarLinea);
                    }
                }
                catch (Exception e)
                {
                    StatusMessageError("HandleRightClickEvent > NotBeforeAction > " + e.Message);
                }
            }
            return result;
        }
        #endregion

        #region _METODOS_PROPIOS
        private void CalcularPorcentajesFormula()
        {
            oMatrix = mForm.Items.Item(MTX_FORMUL).Specific;
            oMatrix.FlushToDataSource();
            double totalA = 0;
            var checkA = !double.TryParse(oMatrix.Columns.Item("Col_3").ColumnSetting.SumValue, out totalA);
            for (int i = 0; i < dsFORMUL.Size; i++)
            {
                dsFORMUL.SetValue("U_EXP_PORC1", i, (checkA) ? "0" : ((double.Parse(dsFORMUL.GetValue("U_EXP_CAPA1", i)) * 100) / totalA).ToString());
            }
            oMatrix.LoadFromDataSource();
        }

        private void InstanciateComboRuta(SAPbouiCOM.ComboBox ComboBox, bool noExt = false)
        {
            while (ComboBox.ValidValues.Count != 0)
            {
                ComboBox.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }

            for (int i = 0; i < dsFRRUTA.Size; i++)
            {
                if (!noExt || (noExt && dsFRRUTA.GetValue("U_EXP_CODRUT", i).ToString() != Constantes.RT_EXTRUS))
                    ComboBox.ValidValues.Add(dsFRRUTA.GetValue("LineId", i).ToString(), dsFRRUTA.GetValue("U_EXP_NOMRUT", i).ToString());
            }
            ComboBox.Item.Enabled = true;
        }

        private string GetRutaDesc(string code, int tipoRef, string TipoRuta, string ruta = "")
        {
            int rutaLinea = string.IsNullOrEmpty(ruta) ? 0 : int.Parse(ruta);
            if (string.IsNullOrEmpty(code)) return "";
            if (int.Parse(code) == rutaLinea) return "";
            switch (tipoRef)
            {
                case Constantes.RutaOrigen:
                    if (int.Parse(code) > rutaLinea)
                    {
                        StatusMessageError(string.Format("No se puede poner una ruta posterior", TipoRuta, code));
                        return "";
                    }
                    break;
                case Constantes.RutaDestino:
                    if (int.Parse(code) < rutaLinea)
                    {
                        StatusMessageError(string.Format("No se puede poner una ruta anterior", TipoRuta, code));
                        return "";
                    }
                    break;
                default:
                    break;
            }
            string val = "";
            for (int i = 0; i < dsFRRUTA.Size; i++)
            {
                switch (tipoRef)
                {
                    case Constantes.RutaBase:
                        if (dsFRRUTA.GetValue("LineId", i).Equals(code) && (dsFRRUTA.GetValue("U_EXP_CODRUT", i).Trim().Equals(TipoRuta.Trim()))) val = dsFRRUTA.GetValue("U_EXP_NOMRUT", i);
                        break;
                    default:
                        if (dsFRRUTA.GetValue("LineId", i).Equals(code)) val = dsFRRUTA.GetValue("U_EXP_NOMRUT", i);
                        break;
                }
            }
            if (string.IsNullOrEmpty(val)) StatusMessageError(string.Format("No hay ruta definida para {0} con código {1}", TipoRuta, code));
            return val;
        }
        private void DeleteRow(int row, string ItemUID)
        {
            try
            {
                mForm.Freeze(true);
                switch (ItemUID)
                {
                    case MTX_FRRUTA:
                        if (Conexion.application.MessageBox("Se eliminará el detalle de la ruta que eliminará.", 1, "Continuar", "Cancelar", "") == 1)
                        {
                            ClearRuta(row, dsFRRUTA);
                            DeleteSpecificMatrix(ItemUID, dsFRRUTA, row, true);
                            DeleteSpecificMatrix(MTX_FORMUL, dsFORMUL, row, true);
                            ReordenarMatrix(row, MTX_FRRUTA); //ruta
                            ReordenarMatrix(row, "mtxForm"); //FORMULACIÓN

                            
                        }
                        break;

                    case MTX_INDUCT:
                    case MTX_FORMUL:
                        EliminarFilaMatrixEstandar(ItemUID, GetDS(ItemUID), row);
                        //DeleteSpecificMatrix(ItemUID, GetDS(ItemUID), row, true);
                        //ReordenarMatrix(row, "mtxForm"); //FORMULACIÓN
                        break;
                    default:
                        DeleteSpecificMatrix(ItemUID, GetDS(ItemUID), row); break;
                }
                ChangeUpdateMode();
                LimpiarReferencias(ItemUID, row);
            }
            catch (Exception ex)
            {
                StatusMessageError("DeleteRow() > " + ex.Message);
            }
            finally { mForm.Freeze(false); }
        }

        private void EliminarFilaMatrixEstandar(string itemUID, DBDataSource dBDataSource, int row)
        {
            try
            {
                mForm.Freeze(true);
                Matrix matrix = mForm.Items.Item(itemUID).Specific;
                matrix.DeleteRow(row);
                matrix.FlushToDataSource();
                matrix.LoadFromDataSource();
            }
            catch (Exception)
            {
                throw;
            }
            finally { mForm.Freeze(false); }
        }

        private void LimpiarReferencias(string itemUID, int row)
        {
            Matrix matrix = mForm.Items.Item(itemUID).Specific;
            string colLinea = itemUID == MTX_FRRUTA ? "#" : "Col_0";
            //string linea = matrix.GetCellSpecific(colLinea, row).Value.ToString();

            try
            {
                LimpiarOrigenDestino(MTX_FREXTR, row.ToString());
                LimpiarOrigenDestino(MTX_FRIMPR, row.ToString());
                LimpiarOrigenDestino(MTX_LAMINA, row.ToString());
                LimpiarOrigenDestino(MTX_FRSELL, row.ToString());
                LimpiarOrigenDestino(MTX_FRCORT, row.ToString());
                LimpiarOrigenDestino(MTX_FRHABI, row.ToString());
                LimpiarOrigenDestino(MTX_FRREBO, row.ToString());
                LimpiarOrigenDestino(MTX_FRSERV, row.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LimpiarOrigenDestino(string MTXID, string linea)
        {
            Matrix matrix = mForm.Items.Item(MTXID).Specific;

            string columnaReferencia = string.Empty;
            
            switch (MTXID)
            {
                case MTX_FREXTR: columnaReferencia = "Col_RDe"; break;
                case MTX_FRIMPR: 
                case MTX_LAMINA: 
                case MTX_FRSELL: 
                case MTX_FRCORT: 
                case MTX_FRHABI: 
                case MTX_FRREBO: 
                case MTX_FRSERV: columnaReferencia = "Col_2"; break;

                default:
                    break;
            }

            string refe = string.Empty;
            
            if(!string.IsNullOrEmpty(columnaReferencia))
            {
                for (int i = 1; i <= matrix.RowCount; i++)
                {
                    refe =  matrix.GetCellSpecific(columnaReferencia, i).Value.ToString();
                    if(refe == linea)
                        matrix.GetCellSpecific(columnaReferencia, i).Value = string.Empty;
                }
            }
        }

        private void ReordenarMatrix(int filaEliminada, string itemUID)
        {
            Matrix matrix = mForm.Items.Item(itemUID).Specific;
            bool reordenar = true; //SOLO SE REORDENA EN CASO SE ENCUENTRE LA RUTA EN LA MATRIX DESTINO

            //for (int i = 1; i <= matrix.RowCount; i++)
            //{
            //    int idRuta = Convert.ToInt32(matrix.GetCellSpecific("Col_0", i).Value); //CODIGO DE RUTA
            //    if(idRuta == filaEliminada)
            //    {
            //        reordenar = true;
            //        break;
            //    }
            //}

            if(reordenar)
            {
                for (int i = 1; i <= matrix.RowCount; i++)
                {
                    int idRuta = 0;
                    string columnaCodigoRuta = itemUID.Equals(MTX_FRRUTA) ? "#" : "Col_0";
                    string val = matrix.GetCellSpecific(columnaCodigoRuta, i).Value;
                    if(!string.IsNullOrEmpty(val))
                        idRuta = Convert.ToInt32(val); //CODIGO DE RUTA
                    
                    if (idRuta > filaEliminada)
                    {
                        matrix.GetCellSpecific(columnaCodigoRuta, i).Value = idRuta - 1;
                    }
                }       
            }

            matrix.FlushToDataSource();
            matrix.AutoResizeColumns();
        }

        private void AddRowRightClick(int row, string ItemUID)
        {
            try
            {
                switch (ItemUID)
                {
                    default:
                        AddSpecificMatrixNewLine(row, ItemUID, GetDS(ItemUID)); break;
                }
            }
            catch (Exception ex)
            {
                StatusMessageError("AddRow() > " + ex.Message);
            }
            finally { mForm.Freeze(false); }
        }
        private void AddRow(string ItemUID)
        {
            try
            {
                switch (ItemUID)
                {
                    default:
                        AddSpecificMatrix(ItemUID, GetDS(ItemUID)); break;
                }
            }
            catch (Exception ex)
            {
                StatusMessageError("AddRow() > " + ex.Message);
            }
            finally { mForm.Freeze(false); }
        }
        private bool DuplicateObject()
        {
            bool BubbleEvent = false;
            try
            {
                if (Conexion.application.MessageBox("Se tomará lo que esta actualmente, actualice el objeto si tiene cambios sin guardar", 1, "Continuar", "Cancelar", "") == 1)
                {
                    frmFormulacion formulacionRLM = new frmFormulacion(Conexion.formOpen);
                    //mForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;
                    formulacionRLM.dsOFRM.LoadFromXML(dsOFRM.GetAsXML());
                    string oldCode = dsOFRM.GetValue("Code", 0);
                    string newCode = oldCode + "C";
                    formulacionRLM.dsOFRM.SetValue("Code", 0, newCode);
                    formulacionRLM.dsFRRUTA.LoadFromXML(dsFRRUTA.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFORMUL.LoadFromXML(dsFORMUL.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsSUBPRD.LoadFromXML(dsSUBPRD.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsINDUCT.LoadFromXML(dsINDUCT.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFREXTR.LoadFromXML(dsFREXTR.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFRIMPR.LoadFromXML(dsFRIMPR.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsLAMINA.LoadFromXML(dsLAMINA.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFRSELL.LoadFromXML(dsFRSELL.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFRCORT.LoadFromXML(dsFRCORT.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFRHABI.LoadFromXML(dsFRHABI.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    formulacionRLM.dsFRREBO.LoadFromXML(dsFRREBO.GetAsXML().Replace(string.Format("<value>{0}</value>", oldCode), string.Format("<value>{0}</value>", newCode)));
                    BubbleEvent = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessageError("DuplicateObject() > " + ex.Message);
                BubbleEvent = false;
            }
            return BubbleEvent;
        }
        private bool AddSpecificMatrix(string matrix, SAPbouiCOM.DBDataSource ds, string aux = "", string codeRut = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(codeRut) && CheckDataSourceRuta(ds, codeRut, matrix)) return true;
                mForm.Freeze(true);
                oMatrix = mForm.Items.Item(matrix).Specific;               
                oMatrix.FlushToDataSource();
                if (oMatrix.RowCount == 0) ds.Clear();
                else
                {
                    switch (matrix)
                    {
                        case MTX_FREXTR:
                            if (String.IsNullOrEmpty(ds.GetValue("U_EXP_CODRUTDE", 0))) ds.Clear();
                            break;
                        default:
                            if (String.IsNullOrEmpty(ds.GetValue("U_EXP_CODRUT", 0))) ds.Clear();
                            break;
                    }
                }
                ds.InsertRecord(ds.Size);
                ds.SetValue("LineId", ds.Size -1, ds.Size.ToString());
                switch (matrix)
                {
                    case MTX_FORMUL:
                        ds.SetValue("U_EXP_CODRUT", ds.Size - 1, udRUTA.Value != null ? udRUTA.Value : "");
                        ds.SetValue("U_EXP_CAPA1", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA2", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA3", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA4", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA5", ds.Size - 1, "0");
                        break;
                    case MTX_INDUCT:
                        if (!string.IsNullOrEmpty(codeRut)) ds.SetValue("U_EXP_CODRUT", ds.Size - 1, codeRut);
                        if (!string.IsNullOrEmpty(aux)) ds.SetValue("U_EXP_INDCTR", ds.Size - 1, aux);
                        ds.SetValue("U_EXP_STATUS", ds.Size - 1, "Y");
                        break;
                    case MTX_FREXTR:
                        if (!string.IsNullOrEmpty(codeRut)) ds.SetValue("U_EXP_CODRUTDE", ds.Size - 1, codeRut);
                        if (!string.IsNullOrEmpty(aux)) ds.SetValue("U_EXP_NOMRUTDE", ds.Size - 1, aux);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(codeRut)) ds.SetValue("U_EXP_CODRUT", ds.Size - 1, codeRut);
                        if (!string.IsNullOrEmpty(aux)) ds.SetValue("U_EXP_NOMRUT", ds.Size - 1, aux);
                        break;
                }
                //ds.SetValue("U_Factor", ds.Size - 1, string.Empty);
                //ds.SetValue("U_FactReal", ds.Size - 1, string.Empty);
                oMatrix.LoadFromDataSource();
                oMatrix.AutoResizeColumns();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mForm.Freeze(false);
            }
        }

        private bool AddSpecificMatrixNewLine(int row, string matrix, SAPbouiCOM.DBDataSource ds, string aux = "", string codeRut = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(codeRut) && CheckDataSourceRuta(ds, codeRut, matrix)) return true;
                mForm.Freeze(true);
                oMatrix = mForm.Items.Item(matrix).Specific;
                oMatrix.FlushToDataSource();
                if (oMatrix.RowCount == 0) ds.Clear();
                else
                {
                    switch (matrix)
                    {
                        case MTX_FREXTR:
                            if (String.IsNullOrEmpty(ds.GetValue("U_EXP_CODRUTDE", 0))) ds.Clear();
                            break;
                        default:
                            if (String.IsNullOrEmpty(ds.GetValue("U_EXP_CODRUT", 0))) ds.Clear();
                            break;
                    }
                }
                
                if (matrix == MTX_FRRUTA &&  row > 0)
                {
                    ds.InsertRecord(row);
                    ds.SetValue("LineId", row, (row + 1).ToString());
                    ds.SetValue("U_EXP_CODRUT", row, "");
                    ds.SetValue("U_EXP_NOMRUT", row, "");
                    ds.SetValue("U_EXP_SUBPRD", row, "");
                    ds.SetValue("U_EXP_SCRAP", row, "");
                    ds.SetValue("U_EXP_REFILE", row, "");

                    if (oMatrix.RowCount > row)
                    {
                        for (int i = row + 1; i <= oMatrix.RowCount; i++)
                        {
                            ds.SetValue("LineId", i, (i + 1).ToString());
                        }
                    }
                }
                else {
                    ds.InsertRecord(ds.Size);
                    ds.SetValue("LineId", ds.Size - 1, ds.Size.ToString());
                }
                switch (matrix)
                {
                    case MTX_FORMUL:
                        ds.SetValue("U_EXP_CODRUT", ds.Size - 1, udRUTA.Value != null ? udRUTA.Value : "");
                        ds.SetValue("U_EXP_CAPA1", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA2", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA3", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA4", ds.Size - 1, "0");
                        ds.SetValue("U_EXP_CAPA5", ds.Size - 1, "0");
                        break;
                    case MTX_INDUCT:
                        if (!string.IsNullOrEmpty(codeRut)) ds.SetValue("U_EXP_CODRUT", ds.Size - 1, codeRut);
                        if (!string.IsNullOrEmpty(aux)) ds.SetValue("U_EXP_INDCTR", ds.Size - 1, aux);
                        ds.SetValue("U_EXP_STATUS", ds.Size - 1, "Y");
                        break;
                    case MTX_FREXTR:
                        if (!string.IsNullOrEmpty(codeRut)) ds.SetValue("U_EXP_CODRUTDE", ds.Size - 1, codeRut);
                        if (!string.IsNullOrEmpty(aux)) ds.SetValue("U_EXP_NOMRUTDE", ds.Size - 1, aux);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(codeRut)) ds.SetValue("U_EXP_CODRUT", row, codeRut);
                        if (!string.IsNullOrEmpty(aux)) ds.SetValue("U_EXP_NOMRUT", row, aux);
                        break;
                }
                //ds.SetValue("U_Factor", ds.Size - 1, string.Empty);
                //ds.SetValue("U_FactReal", ds.Size - 1, string.Empty);
                oMatrix.LoadFromDataSource();
                oMatrix.AutoResizeColumns();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mForm.Freeze(false);
            }
        }
        private void ClearRuta(int row, SAPbouiCOM.DBDataSource ds)
        {
            try
            {
                switch (ds.GetValue("U_EXP_CODRUT", row - 1))
                {
                    case Constantes.RT_EXTRUS:
                        DeleteSpecificMatrix(MTX_FREXTR, dsFREXTR, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FREXTR); //EXTRUCIÓN
                        break;
                    case Constantes.RT_IMPRES:
                        DeleteSpecificMatrix(MTX_FRIMPR, dsFRIMPR, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FRIMPR); //IMPRESION
                        break;
                    case Constantes.RT_LAMINA:
                        DeleteSpecificMatrix(MTX_LAMINA, dsLAMINA, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_LAMINA); //LAMINA
                        break;
                    case Constantes.RT_SELLAD:
                        DeleteSpecificMatrix(MTX_FRSELL, dsFRSELL, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FRSELL); //SELLADO
                        break;
                    case Constantes.RT_CORTE:
                        DeleteSpecificMatrix(MTX_FRCORT, dsFRCORT, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FRCORT); //CORTE
                        break;
                    case Constantes.RT_HABILI:
                        DeleteSpecificMatrix(MTX_FRHABI, dsFRHABI, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FRHABI); //HABILITADO
                        break;
                    case Constantes.RT_REBOBI:
                        DeleteSpecificMatrix(MTX_FRREBO, dsFRREBO, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FRREBO); //REBOBINADO
                        break;
                    case Constantes.RT_SERV:
                        DeleteSpecificMatrix(MTX_FRSERV, dsFRSERV, int.Parse(ds.GetValue("LineId", row - 1)), true);
                        ReordenarMatrix(row, MTX_FRSERV); //SERVICIO
                        break;
                }

                DeleteSpecificMatrix(MTX_INDUCT, dsINDUCT, int.Parse(ds.GetValue("LineId", row - 1)), true); //BORRAMOS EL INDUCTOR DE CUALQUIER ETAPA
                ReordenarMatrix(row, MTX_INDUCT); //INDUCTOR
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteSpecificMatrix(string matrix, SAPbouiCOM.DBDataSource ds, int row, bool isClear = false)
        {
            try
            {
                List<string> filasEliminables = new List<string>();
                oMatrix = mForm.Items.Item(matrix).Specific;
                oMatrix.FlushToDataSource();

                if (isClear)
                {
                    int tamani = ds.Size;
                    for (int i = 0; i < ds.Size; i++)
                    {
                        string codigoRuta = matrix.Equals(MTX_FREXTR) ? "U_EXP_CODRUTDE" : matrix.Equals(MTX_FRRUTA) ? "LineId" : "U_EXP_CODRUT";
                        string codigoRutaDS = ds.GetValue(codigoRuta, i);

                        if(codigoRutaDS == row.ToString())
                            filasEliminables.Add(ds.GetValue(codigoRuta, i));

                    }
                }

                foreach (string filaEliminable in filasEliminables)
                {
                    //string codigoRuta = matrix.Equals(MTX_FREXTR) ? "U_EXP_CODRUTDE" : matrix.Equals(MTX_FRRUTA) ? "LineId" : "U_EXP_CODRUT";
                    string columnaCodigoRuta = matrix.Equals(MTX_FREXTR) ? "Col_0" : matrix.Equals(MTX_FRRUTA) ? "#" : "Col_0";
                    for (int i = 1; i <= oMatrix.RowCount; i++)
                    {
                        string codigoRutaDS = oMatrix.GetCellSpecific(columnaCodigoRuta, i).Value;
                        if(codigoRutaDS == filaEliminable)
                        {
                            oMatrix.DeleteRow(i);
                            break;
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SAPbouiCOM.DBDataSource GetDS(string matrix)
        {
            switch (matrix)
            {
                case MTX_FRRUTA:
                    return dsFRRUTA;
                case MTX_FORMUL:
                    return dsFORMUL;
                case MTX_SUBPRD:
                    return dsSUBPRD;
                case MTX_INDUCT:
                    return dsINDUCT;
                case MTX_FREXTR:
                    return dsFREXTR;
                case MTX_FRIMPR:
                    return dsFRIMPR;
                case MTX_LAMINA:
                    return dsLAMINA;
                case MTX_FRSELL:
                    return dsFRSELL;
                case MTX_FRCORT:
                    return dsFRCORT;
                case MTX_FRHABI:
                    return dsFRHABI;
                case MTX_FRREBO:
                    return dsFRREBO;
                case MTX_FRSERV:
                    return dsFRSERV;
                default:
                    return null;
            }
        }
        #endregion
        private void ChangeUpdateMode()
        {
            if (mForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE)
                mForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
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