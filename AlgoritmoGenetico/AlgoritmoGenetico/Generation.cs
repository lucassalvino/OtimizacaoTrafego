using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using GeneticAlgorithm.Enuns;

namespace GeneticAlgorithm
{
    public class Generation<GeneType> : BaseEntity
    {
        public Generation(int numberOfIndividuals, int numberOfGenesOfIndividuals)
        {
            if (numberOfGenesOfIndividuals <= 0 || numberOfIndividuals <= 0)
                throw new Exception("The number of Genes of Individuals or number of individuals is smaller than zero");
            NumberOfIndividuals = numberOfIndividuals;
            NumberOfGenesOfIndividuals = numberOfGenesOfIndividuals;
            Environment = new Environment();
        }
        public List<Chromosome<GeneType>> Individuals { get; private set; }
        public int NumberOfIndividuals { get; set; }
        public int NumberOfGenesOfIndividuals { get; set; }
        public Environment Environment { get; set; }
        public int NumberOfGeneration { get; private set; } = 0;

        private Random Random = new Random((int)DateTime.Now.Ticks);
        public void AddIndividualChromosome(Chromosome<GeneType> chromosome)
        {
            if (Individuals == null)
                Individuals = new List<Chromosome<GeneType>>();
            if (chromosome == null)
            {
                Debug.WriteLine("The Chromosome is not defined");
                return;
            }
            if (Individuals.Count > NumberOfIndividuals)
            {
                Debug.WriteLine("The max number of Chromosome has been reached");
                return;
            } 
            if (Environment.AllowDuplicateChildren)
                Individuals.Add(chromosome);
            else
            {
                if (Individuals.Where(x => x.Id == chromosome.Id).FirstOrDefault() != null)
                {
                    Debug.WriteLine($"The Chromosome {chromosome.Id} already exists in the population");
                    return;
                }
                else
                {
                    Individuals.Add(chromosome);
                }
            }
        }
        private Guid GetIdOfRandomChromosome()
        {
            return Individuals[Random.Next() % Individuals.Count()].Id;
        }
        private Guid GetIdOfChromosomeRoulette()
        {
            double SumFitness = Individuals.Select(x => x.Fitness).Sum();
            int random = Random.Next() % (int)SumFitness;
            if (random <= 0)
                random = 1;
            double PartialSum = 0;
            foreach (var chromosome in Individuals)
            {
                PartialSum += chromosome.Fitness;
                if (PartialSum >= random)
                    return chromosome.Id;
            }
            if (Individuals.Count > 0)
                return GetIdOfRandomChromosome();
            return Guid.Empty;
        } 
        private void ValidateParentsId(Guid FatherId, Guid MotherId)
        {
            if (FatherId == null || MotherId == null)
                throw new NullReferenceException();
            if (FatherId == Guid.Empty || MotherId == Guid.Empty)
                throw new Exception("The Id of Father or Mother is empty");
        }
        private Chromosome<GeneType> GetChromosome(Guid Id)
        {
            return Individuals.Where(x => x.Id == Id).FirstOrDefault();
        }
        private Chromosome<GeneType> ExecuteCrossOver(Chromosome<GeneType> Father, Chromosome<GeneType> Mother)
        {
            switch (Environment.TypeCrossover)
            {
                case TypeCrossover.CrossOverOnePoint:
                    return Father.CrossOverOnePoint(Mother);
                case TypeCrossover.CrossOverTwoPoints:
                    return Father.CrossOverTwoPoints(Mother);
                default:
                    return Father.CrossOverOnePoint(Mother);
            }
        }
        public Generation<GeneType>GenerateNextGeneration(BaseEvaluation<GeneType> CalculatesEvaluation)
        {
            #region Validation
            if (Environment == null || Individuals == null)
                throw new NullReferenceException();
            if (Individuals.Count <= 0)
                throw new Exception("The number of indiciduals must be greater than zero");
            #endregion Validation

            UpdateFitnessOfGeneration(CalculatesEvaluation);
            var newGenration = new Generation<GeneType>(NumberOfIndividuals, NumberOfGenesOfIndividuals);
            newGenration.Environment = this.Environment;
            newGenration.NumberOfGeneration = this.NumberOfGeneration + 1;
            newGenration.Individuals = new List<Chromosome<GeneType>>();

            while (newGenration.Individuals.Count < NumberOfIndividuals)
            {
                var fatherId = GetIdOfChromosomeRoulette();
                var motherId = GetIdOfChromosomeRoulette();
                ValidateParentsId(fatherId, motherId);
                while (fatherId == motherId)
                {
                    motherId = GetIdOfRandomChromosome();
                    ValidateParentsId(fatherId, motherId);
                }
                #region CheckIfExecuteCrossover
                int randPercentage = Random.Next() % 100;
                if (randPercentage <= Environment.RateCrossover)
                {
                    var father = GetChromosome(fatherId);
                    var mother = GetChromosome(motherId);
                    newGenration.AddIndividualChromosome(ExecuteCrossOver(father, mother));
                    newGenration.AddIndividualChromosome(ExecuteCrossOver(mother, father));
                }
                else
                {
                    newGenration.AddIndividualChromosome(GetChromosome(fatherId));
                    newGenration.AddIndividualChromosome(GetChromosome(motherId));
                }
                #endregion CheckIfExecuteCrossover
            }
            #region ExecuteMutation
            int randPercentageMutation = Random.Next() % 100;
            if (randPercentageMutation <= Environment.RateMutation)
            {
                Debug.WriteLine("there was a mutation");
                int randomChromossomePosition = Random.Next() % Individuals.Count;
                Individuals[randomChromossomePosition].PerformsMutation(Random);
            }
            #endregion ExecuteMutation
            return newGenration;
        }
        private void UpdateFitnessOfGeneration(BaseEvaluation<GeneType> CalculatesEvaluation)
        {
            foreach(var chrmosome in Individuals)
            {
                chrmosome.Fitness = CalculatesEvaluation.CalculatesEvaluation(chrmosome);
            }
        }
    }
}