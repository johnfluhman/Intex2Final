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
        public string CrashDatetime { get; set; }
        public int? Route { get; set; }
        public double? Milepoint { get; set; }
        public string LatUtmY { get; set; }
        public string LongUtmX { get; set; }
        public string MainRoadName { get; set; }
        public int CrashSeverityId { get; set; }
        public int WorkZoneRelated { get; set; }
        public int PedestrianInvolved { get; set; }
        public int BicyclistInvolved { get; set; }
        public int MotorcycleInvolved { get; set; }
        public int ImproperRestraint { get; set; }
        public int Unrestrained { get; set; }
        public int Dui { get; set; }
        public int IntersectionRelated { get; set; }
        public int WildAnimalRelated { get; set; }
        public int DomesticAnimalRelated { get; set; }
        public int OverturnRollover { get; set; }
        public int CommercialMotorVehInvolved { get; set; }
        public int TeenageDriverInvolved { get; set; }
        public int OlderDriverInvolved { get; set; }
        public int NightDarkCondition { get; set; }
        public int SingleVehicle { get; set; }
        public int DistractedDriving { get; set; }
        public int DrowsyDriving { get; set; }
        public int RoadwayDeparture { get; set; }
        public int CityId { get; set; }
        public int CountyId { get; set; }

        public virtual City City { get; set; }
        public virtual County County { get; set; }
    }
}
