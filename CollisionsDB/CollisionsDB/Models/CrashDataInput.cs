using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollisionsDB.Models
{
    public class CrashDataInput
    {
        public float Milepoint { get; set; }
        public float LatUtmY { get; set; }
        public float LongUtmX { get; set; }
        public bool PedestrianInvolved { get; set;  }
        public bool BicyclistInvolved { get; set; }
        public bool MotorcycleInvolved { get; set; }
        public bool OverturnRollover { get; set; }
        public bool TeenageDriverInvolved { get; set;  }
        public bool NightDarkCondition { get; set; }

        // converts collision object into CrashDataInput for the ML
        public static CrashDataInput CollisionToMLInput(Collision collision)
        {
            return new CrashDataInput
            {
                Milepoint = (float)collision.Milepoint,
                LatUtmY = (float)collision.LatUtmY,
                LongUtmX = (float)collision.LongUtmX,
                // convert int to bool
                PedestrianInvolved = collision.PedestrianInvolved < 1 ? false : true,
                BicyclistInvolved = collision.BicyclistInvolved < 1 ? false : true,
                MotorcycleInvolved = collision.MotorcycleInvolved < 1 ? false : true,
                OverturnRollover = collision.OverturnRollover < 1 ? false : true,
                TeenageDriverInvolved = collision.TeenageDriverInvolved < 1 ? false : true,
                NightDarkCondition = collision.NightDarkCondition < 1 ? false : true,
            };
        }

        public static float BoolToFloat(bool input)
        {
            if(input == true)
            {
                return 1f;
            }
            else
            {
                return 0;
            }
        }

        public CrashDataInput Clone()
        {
            return (CrashDataInput)this.MemberwiseClone();
        }

        public Tensor<float> AsTensor()
        {

            float[] data = new float[]
            {
            Milepoint, LatUtmY, LongUtmX, BoolToFloat(PedestrianInvolved), BoolToFloat(BicyclistInvolved), BoolToFloat(MotorcycleInvolved), BoolToFloat(OverturnRollover), BoolToFloat(TeenageDriverInvolved), BoolToFloat(NightDarkCondition)
            };
            int[] dimensions = new int[] { 1, 9 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}
