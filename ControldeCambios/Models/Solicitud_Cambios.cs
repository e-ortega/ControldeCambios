
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace ControldeCambios.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Solicitud_Cambios
{

    public int id { get; set; }

    public int req1 { get; set; }

    public Nullable<int> req2 { get; set; }

    public string razon { get; set; }

    public string solicitadoPor { get; set; }

    public System.DateTime solicitadoEn { get; set; }

    public string aprobadoPor { get; set; }

    public Nullable<System.DateTime> aprobadoEn { get; set; }

    public string estado { get; set; }

    public string tipo { get; set; }

    public string comentario { get; set; }

    public string proyecto { get; set; }



    public virtual Estado_Solicitud Estado_Solicitud { get; set; }

    public virtual Proyecto Proyecto1 { get; set; }

    public virtual Requerimiento Requerimiento { get; set; }

    public virtual Requerimiento Requerimiento1 { get; set; }

    public virtual Tipo_Solicitud Tipo_Solicitud { get; set; }

    public virtual Usuario Usuario { get; set; }

    public virtual Usuario Usuario1 { get; set; }

}

}
