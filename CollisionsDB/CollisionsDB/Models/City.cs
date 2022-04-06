using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CollisionsDB
{
    public partial class City
    {
        [Key]
        [Required]
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
}
