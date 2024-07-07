using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Beneficiario
    /// </summary>
    public class BeneficiarioModel
    {
        public long Id { get; set; }
    
        [Required]
        public string CPF { get; set; }
        [Required]
        public string Nome{ get; set; }
        [Required]
        public long IdCliente { get; set; }



    }
}