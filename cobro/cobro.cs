using Req_Utility.Clases;
using Req_Utility.Controles;
using Req_Utility.Formulario;
using Req_Utility.ControlesObj;
using Softech.Administrativo.BusinessObjects;
using Softech.Administrativo.DefinicionBO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace Requerimientos
{
    public class cobro
    {
        public static Dictionary<string, object> parametros = new Dictionary<string, object>();
        public object ObjGlobalFormaUIPCMetodo = new object();
        public object ObjGlobalPForma = new object();
        private object ObjGlobalFormaUIPC = new object();
        public CobroABO objFrmCobro = new CobroABO();
        public List<ICobroDocRenglonABO> objFrmCobroDocRenglon = new List<ICobroDocRenglonABO>();
        private string pk_Doc_Num = string.Empty;
        private string pkCo_Mone = string.Empty;
        private DateTime pkFec_Emis;

        public bool AntesDeIniciarPantalla(object forma, object FormaUIPC)
        {
            ObjGlobalFormaUIPCMetodo = FormaUIPC;
            ObjGlobalFormaUIPC = forma;
            ObjGlobalPForma = forma;

            return true;
        }

        public bool DespuesDeGuardar()
        {

            objFrmCobro = (CobroABO)ObjGlobalFormaUIPCMetodo.GetType().GetMethod("ObtenerObjetoReq").Invoke(ObjGlobalFormaUIPCMetodo, new Object[0]);
            pk_Doc_Num = objFrmCobro.Pk_Cob_Num;
            pkFec_Emis = objFrmCobro.Fecha;

            //   MessageBox.Show("Informacion del cobro: " + pk_Doc_Num.ToString() + "Fecha: " + pkFec_Emis.ToString() , "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            objFrmCobroDocRenglon = (List<ICobroDocRenglonABO>)ObjGlobalFormaUIPCMetodo.GetType().GetMethod("ObtenerObjetoRenglonesReq").Invoke(ObjGlobalFormaUIPCMetodo, new Object[0]);

            if (objFrmCobroDocRenglon.Count > 0) {                 
                foreach (CobroDocRenglonABO Renglones in objFrmCobroDocRenglon)
                {                    
                    if (Renglones.Co_Tipo_Doc.ToString() == "IVAN" || Renglones.Co_Tipo_Doc.ToString() == "IVAP" || Renglones.Co_Tipo_Doc.ToString() == "ISLR" || Renglones.Co_Tipo_Doc.ToString() == "ADEL")
                    {
                        StringBuilder stringBuilder1 = new StringBuilder();
                        stringBuilder1.Append("select dbo.TasaAUnaFecha('US$',1,CONVERT(datetime, CONVERT(varchar(10), '" + pkFec_Emis.ToString("dd/MM/yyyy") + "', 103), 103))");
                        DataTable DTTasaAUnaFecha = (DataTable)this.ObjGlobalFormaUIPCMetodo.GetType().GetMethod("EjecutarConsulta").Invoke(this.ObjGlobalFormaUIPCMetodo, new object[2]
                        {
                                    (object) stringBuilder1,
                                    (object) false
                        });

                        MessageBox.Show(Renglones.Co_Tipo_Doc.ToString() + " " + Renglones.Nro_Doc.ToString());
                    
                        StringBuilder stringBuilder2 = new StringBuilder();
                        stringBuilder2.Append("UPDATE saDocumentoVenta SET campo3 = '" + DTTasaAUnaFecha.Rows[0][0].ToString().Trim() + "' WHERE co_tipo_doc = '" + Renglones.Co_Tipo_Doc.ToString() + "' AND nro_doc = '" + Renglones.Nro_Doc.ToString() + "'");
                        DataTable DTTasaAUnaFechaU = (DataTable)this.ObjGlobalFormaUIPCMetodo.GetType().GetMethod("EjecutarConsulta").Invoke(this.ObjGlobalFormaUIPCMetodo, new object[2]
                        {
                            (object) stringBuilder2,
                            (object) false
                        });
                        MessageBox.Show("Se ha actualizado el tipo de cambio de US$ en el campo adicional Nro. 3 del documento tipo " + Renglones.Co_Tipo_Doc.ToString() + ". Nro.: " + Renglones.Nro_Doc.ToString() + ". Tasa: " +  DTTasaAUnaFecha.Rows[0][0].ToString(), "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }
            }

            return true;
        }
    }
}
