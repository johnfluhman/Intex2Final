using CollisionsDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CollisionsDB
{
    public partial class Collision
    {
        [Key]
        [Required]
        public long CrashId { get; set; }
        [Required, RegularExpression(@"^\d+\/\d+\/\d{4}\s\d+\:\d+", ErrorMessage = "Please enter date in the following format: M/DD/YYYY HH:MM")]
        public string CrashDatetime { get; set; }
        [Required]
        public int? Route { get; set; }
        [Required]
        public double? Milepoint { get; set; }
        [Required]
        public double? LatUtmY { get; set; }
        [Required]
        public double? LongUtmX { get; set; }
        [Required]
        public string MainRoadName { get; set; }

        [Required, Range(1, 5, ErrorMessage = "Please select a value")]
        public int CrashSeverityId { get; set; }
        public CrashSeverity CrashSeverity { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int WorkZoneRelated { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int PedestrianInvolved { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int BicyclistInvolved { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int MotorcycleInvolved { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int ImproperRestraint { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int Unrestrained { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int Dui { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int IntersectionRelated { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int WildAnimalRelated { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int DomesticAnimalRelated { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int OverturnRollover { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int CommercialMotorVehInvolved { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int TeenageDriverInvolved { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int OlderDriverInvolved { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int NightDarkCondition { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int SingleVehicle { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int DistractedDriving { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int DrowsyDriving { get; set; }
        [Required, Range(0, 1, ErrorMessage = "Please select a value")]
        public int RoadwayDeparture { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public int CountyId { get; set; }

        public virtual City City { get; set; }
        public virtual County County { get; set; }
    }
}
