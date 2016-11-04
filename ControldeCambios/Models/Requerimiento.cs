
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
    
public partial class Requerimiento
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Requerimiento()
    {

        this.Requerimientos_Cri_Acep = new HashSet<Requerimientos_Cri_Acep>();

        this.Usuarios = new HashSet<Usuario>();

    }


    public int id { get; set; }

    public string codigo { get; set; }

    public Nullable<int> version { get; set; }

    public System.DateTime creadoEn { get; set; }

    public Nullable<System.DateTime> finalizaEn { get; set; }

    public string descripcion { get; set; }

    public string nombre { get; set; }

    public int prioridad { get; set; }

    public string observaciones { get; set; }

    public Nullable<int> esfuerzo { get; set; }

    public string estado { get; set; }

    public string creadoPor { get; set; }

    public string solicitadoPor { get; set; }

    public string proyecto { get; set; }

    public Nullable<int> modulo { get; set; }

    public byte[] imagen { get; set; }



    public virtual Estado_Requerimientos Estado_Requerimientos { get; set; }

    public virtual Modulo Modulo1 { get; set; }

    public virtual Proyecto Proyecto1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Requerimientos_Cri_Acep> Requerimientos_Cri_Acep { get; set; }

    public virtual Usuario Usuario { get; set; }

    public virtual Usuario Usuario1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Usuario> Usuarios { get; set; }

}

}
