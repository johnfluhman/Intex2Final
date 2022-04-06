using System;
using System.ComponentModel.DataAnnotations;

namespace CollisionsDB.Models
{
    public class Collision
    {
        [Key]
        [Required]
        public long CRASH_ID { get; set; }

        [Required(ErrorMessage = "Please enter the Date the crash occurred on")]
        public string CRASH_DATETIME { get; set; }

        public int ROUTE { get; set; }

        public double MILEPOINT { get; set; }

        public string LAT_UTM_Y { get; set; }

        public string LONG_UTM_X { get; set; }

        [Required(ErrorMessage = "Please enter the Road Name the crash occurred on")]
        public string MAIN_ROAD_NAME { get; set; }

        [Required(ErrorMessage = "Please enter a value between 1 and 5")]
        [Range(1, 5)]
        public int CRASH_SEVERITY_ID { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int WORK_ZONE_RELATED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int PEDESTRIAN_INVOLVED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int BICYCLIST_INVOLVED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int MOTORCYCLE_INVOLVED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int IMPROPER_RESTRAINT { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int UNRESTRAINED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int DUI { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int INTERSECTION_RELATED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int WILD_ANIMAL_RELATED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int DOMESTIC_ANIMAL_RELATED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int OVERTURN_ROLLOVER { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int COMMERCIAL_MOTOR_VEH_INVOLVED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int TEENAGE_DRIVER_INVOLVED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int OLDER_DRIVER_INVOLVED { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int NIGHT_DARK_CONDITION { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int SINGLE_VEHICLE { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int DISTRACTED_DRIVING { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int DROWSY_DRIVING { get; set; }

        [Required, Range(0, 1, ErrorMessage = "Please Select either 'TRUE' or 'FALSE'")]
        public int ROADWAY_DEPARTURE { get; set; }

        //Foreign Key Relationships
        [Required(ErrorMessage = "Please enter the City the crash occurred in")]
        public int CITY_ID { get; set; }
        public City City { get; set; }

        [Required(ErrorMessage = "Please enter the County the crash occurred in")]
        public int COUNTY_ID { get; set; }
        public County County { get; set; }
    }
}

