using GeneticAlgorithm.Enuns;
using System;

namespace GeneticAlgorithm
{
    public class Environment: BaseEntity
    {
        private float rateCrossover = 90;
        private float rateMutation = 5;
        /// <summary>
        /// The crossover rate defines whether or not the crossing will take place.
        /// It is recommended to use a high value (between 60% and 90%).
        /// Only values ​​between the range[0, 100] will be allowed.
        /// The default value is 90%
        /// </summary>
        public float RateCrossover 
        {
            get
            {
                return (rateCrossover);
            }
            set
            {
                if (value < 0 || value > 100)
                    throw new Exception("The Crossover rate must beteween the tange [0, 100]");
                rateCrossover = value;
            }
        }
        /// <summary>
        /// The mutation rate defines whether or not the mutation occurs.
        /// It is recommended to use a low value (less than 5%).
        ///  /// Only values ​​between the range[0, 100] will be allowed.
        /// The default value is 5%
        /// </summary>
        public float RateMutation
        {
            get
            {
                return (rateMutation);
            }
            set
            {
                if (value < 0 || value > 100)
                    throw new Exception("The rate mutation must beteween the range [0, 100]");
                rateMutation = value;
            }
        }
        public bool AllowDuplicateChildren { get; set; } = true;
        public TypeCrossover TypeCrossover { get; set; } = TypeCrossover.CrossOverOnePoint;
    }
}
