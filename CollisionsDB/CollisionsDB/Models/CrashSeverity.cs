using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollisionsDB.Models
{
    public class CrashSeverity
    {
        [Required]
        [Key]
        public int CrashSeverityId { get; set; }

        public string Description { get; set; }
    }
}
