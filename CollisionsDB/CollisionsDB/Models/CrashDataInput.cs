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

        public float BoolToFloat(bool input)
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
