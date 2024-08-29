using AddonListaMaterialesYrutas.commons;
using AddonListaMaterialesYrutas.view;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AddonListaMaterialesYrutas.conexion
{
    public class Conexion
    {
        public static SAPbobsCOM.Company company;
        public static SAPbouiCOM.Application application;
        public static readonly Dictionary<string, IForm> formOpen;
        static Conexion()
        {
            formOpen = new Dictionary<string, IForm>();
        }
        public Conexion()
        {
            try
            {
                application = instanciarAplicacion();
                company = InstanciarCompania();
                InicializarFiltros();
                application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(Application_AppEvent);
                application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(Application_MenuEvent);
                application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(Application_ItemEvent);
                application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(Application_FormDataEvent);
                application.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(Application_RightClickEvent);

                //LAYOUT
                application.LayoutKeyEvent += new SAPbouiCOM._IApplicationEvents_LayoutKeyEventEventHandler(Application_LayoutKeyEvent);
                CrearMenu();
            }
            catch (Exception e)
            {
                application.MessageBox("Conexion: " + e.Message);
            }
        }

        private void Application_LayoutKeyEvent(ref SAPbouiCOM.LayoutKeyInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //NO QUISE TOCAR TU INTERFAZ(IForm) PARA QUE ACEPTARA ESTE EVENTO, ASI QUE ESTE EVENTO AISLADO LO MANEJO SOLO ACA (DAVID)
            if(eventInfo.BeforeAction && eventInfo.FormUID.Contains(FormName.FORMUL_RLM))
            {
                SAPbouiCOM.Form form = Conexion.application.Forms.ActiveForm; 

                string codigo = form.DataSources.DBDataSources.Item("@EXP_OFRM").GetValue("Code", 0);
                if (!string.IsNullOrEmpty(codigo))
                    eventInfo.LayoutKey = codigo;
                else BubbleEvent = false;
            }
        }

        private SAPbouiCOM.Application instanciarAplicacion()
        {
            var guiApi = new SAPbouiCOM.SboGuiApi();
            guiApi.Connect(Environment.GetCommandLineArgs().GetValue(1).ToString());
            return guiApi.GetApplication();
        }
        private SAPbobsCOM.Company InstanciarCompania()
        {
            try
            {
                return application.Company.GetDICompany();
            }
            catch (Exception e)
            {
                application.MessageBox(e.Message);
            }
            return null;
        }

        private void InicializarFiltros()
        {
            SAPbouiCOM.EventFilters filtros = new SAPbouiCOM.EventFilters();
            SAPbouiCOM.EventFilter filtroMenu = filtros.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
            SAPbouiCOM.EventFilter filtroItem = filtros.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);

            SAPbouiCOM.EventFilter filtroLayout = filtros.Add(SAPbouiCOM.BoEventTypes.et_PRINT_LAYOUT_KEY);
            filtroLayout.AddEx(FormName.FORMUL_RLM);
            filtroItem.AddEx(FormName.FORMUL_RLM);
            filtroItem.AddEx(FormName.LISTA_MATERIALES);
            filtroItem.AddEx(FormName.ORDEN_FABRICACION);
            filtroItem.AddEx("FRMOBS");
            filtroItem.AddEx("140");
            filtroItem.AddEx("42");

            SAPbouiCOM.EventFilter filtroDoubleClick = filtros.Add(SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK);
            filtroDoubleClick.AddEx(FormName.FORMUL_RLM);

            SAPbouiCOM.EventFilter filtroCFL = filtros.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
            filtroCFL.AddEx(FormName.FORMUL_RLM);

            SAPbouiCOM.EventFilter filterMatrixLink = filtros.Add(SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED);
            filterMatrixLink.AddEx(FormName.FORMUL_RLM);

            SAPbouiCOM.EventFilter filterCombo = filtros.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
            filterCombo.AddEx(FormName.FORMUL_RLM);

            SAPbouiCOM.EventFilter filterLostFocus = filtros.Add(SAPbouiCOM.BoEventTypes.et_LOST_FOCUS);
            filterLostFocus.AddEx(FormName.FORMUL_RLM);

            SAPbouiCOM.EventFilter filterFormLoad = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
            filterFormLoad.AddEx(FormName.LISTA_MATERIALES);
            filterFormLoad.AddEx(FormName.ORDEN_FABRICACION);
            filterFormLoad.AddEx(FormName.FORMUL_RLM);
            SAPbouiCOM.EventFilter filterFormDataLoad = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD);
            filterFormDataLoad.AddEx(FormName.FORMUL_RLM);

            //filterFormLoad.AddEx("0");
            SAPbouiCOM.EventFilter filterAddData = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD);
            filterAddData.AddEx(FormName.FORMUL_RLM);

            SAPbouiCOM.EventFilter filterRightClick = filtros.Add(SAPbouiCOM.BoEventTypes.et_RIGHT_CLICK);
            filterRightClick.AddEx(FormName.FORMUL_RLM);

            //SAPbouiCOM.EventFilter filterClose = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
            //filterClose.AddEx(FormName.DATOS_DEL_VIAJE);

            application.SetFilter(filtros);
        }

        //Eventos de aplicación
        void Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            try
            {
                BubbleEvent = true;

                if (formOpen.ContainsKey(FormUID))
                {
                    BubbleEvent = formOpen[FormUID].HandleItemEvents(pVal);
                }

                switch (pVal.FormTypeEx)
                {
                    case FormName.LISTA_MATERIALES:
                        BubbleEvent = new frmOITT().HandleItemEvents(pVal);
                        break;
                    case FormName.ORDEN_FABRICACION:
                        BubbleEvent = new frmOWOR().HandleItemEvents(pVal);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {
                BubbleEvent = true;
            }
        }

        void Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    company.Disconnect();
                    Environment.Exit(0);
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    company.Disconnect();
                    Environment.Exit(0);
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    company.Disconnect();
                    Environment.Exit(0);
                    break;
            }
        }

        void Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            var result = true;
            if (pVal.BeforeAction)
            {
                try
                {
                    switch (pVal.MenuUID)
                    {
                        case FormName.FORMUL_RLM:
                            frmFormulacion formulacionRLM = new frmFormulacion(formOpen);
                            break;

                        case "1283": //MENU ELIMINAR

                            SAPbouiCOM.Form form = Conexion.application.Forms.ActiveForm;
                            if(form.TypeEx == FormName.FORMUL_RLM)
                            {
                                int rpta = Conexion.application.MessageBox("Está a punto de eliminar la fórmula. ¿Desea continuar?", 2, "Sí", "No");
                                result = rpta == 1;
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    application.MessageBox(e.Message);
                }
            }
            try
            {
                // Control "Crear" de la barra de herramientas || Control "Buscar" de la barra de herramientas
                if (pVal.MenuUID == Constantes.Menu_Crear || pVal.MenuUID == Constantes.Menu_Buscar
                    || pVal.MenuUID == Constantes.Registro_Datos_Anterior || pVal.MenuUID == Constantes.Registro_Datos_Siguiente
                    || pVal.MenuUID == Constantes.Primer_Registro_Datos || pVal.MenuUID == Constantes.Ultimo_Registro_Datos)
                {
                    var mForm = application.Forms.ActiveForm;
                    if (formOpen.ContainsKey(mForm.UniqueID))
                        result = formOpen[mForm.UniqueID].HandleMenuDataEvents(pVal);
                }

                //Controles basados en el menu "Click derecho"
                if (pVal.MenuUID == Constantes.Menu_AgregarLinea || pVal.MenuUID == Constantes.Menu_EliminarLinea || pVal.MenuUID == Constantes.Menu_Cancelar || pVal.MenuUID == Constantes.Menu_Duplicar)
                {
                    if (pVal.BeforeAction)
                    {
                        var mForm = application.Forms.ActiveForm;
                        if (formOpen.ContainsKey(mForm.UniqueID))
                            result = formOpen[mForm.UniqueID].HandleMenuDataEvents(pVal);
                    }
                }
            }
            catch (Exception e)
            {
                application.MessageBox(e.Message);
            }
            BubbleEvent = result;
        }

        void Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            try
            {
                BubbleEvent = true;

                if (formOpen.ContainsKey(BusinessObjectInfo.FormUID))
                {
                    BubbleEvent = formOpen[BusinessObjectInfo.FormUID].HandleFormDataEvents(BusinessObjectInfo);
                }
                else
                {
                    switch (BusinessObjectInfo.FormTypeEx)
                    {

                          default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                BubbleEvent = true;
            }
        }

        void Application_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = formOpen[eventInfo.FormUID].HandleRightClickEvent(eventInfo);
        }

        //Creación de menú
        private void CrearMenu(System.Drawing.Bitmap imageBMP = null)
        {
            SAPbouiCOM.Form frmEps = application.Forms.GetFormByTypeAndCount(169, 1);
            frmEps.Freeze(true);
            try
            {
                application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + "Cargando opciones de menú", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_None);

                XmlDocument xmlMenu = new XmlDocument();
                xmlMenu.LoadXml(AddonListaMaterialesYrutas.Properties.Resources.Menu);
                application.LoadBatchActions(xmlMenu.InnerXml);
            }
            catch (Exception e)
            {
                application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + e.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                frmEps.Freeze(false);
                frmEps.Update();
            }
        }

        public static void AddForm(string UID, IForm newForm)
        {
            formOpen.Add(UID, newForm);
        }
    }
}