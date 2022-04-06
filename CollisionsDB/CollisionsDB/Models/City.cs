using System;
using System.ComponentModel.DataAnnotations;

namespace CollisionsDB.Models
{
    public class City
    {
        [Key, Required]
        public int CITY_ID { get; set; }
        [Required]
        public string CityName { get; set; }
    }
}
