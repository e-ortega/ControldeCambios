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
            this.CambiosRequerimientos = new HashSet<CambiosRequerimiento>();
            this.CambiosRequerimientos1 = new HashSet<CambiosRequerimiento>();
            this.Sprints = new HashSet<Sprint>();
        }
    
        public string codigo { get; set; }
        public int version { get; set; }
        public System.DateTime creadoEn { get; set; }
        public string descripcion { get; set; }
        public string nombre { get; set; }
        public int prioridad { get; set; }
        public string observaciones { get; set; }
        public Nullable<int> esfuerzo { get; set; }
        public string estado { get; set; }
        public string creadoPor { get; set; }
        public string solicitadoPor { get; set; }
        public string proyecto { get; set; }
        public int modulo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CambiosRequerimiento> CambiosRequerimientos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CambiosRequerimiento> CambiosRequerimientos1 { get; set; }
        public virtual Modulo Modulo1 { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Usuario1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sprint> Sprints { get; set; }
    }
}
