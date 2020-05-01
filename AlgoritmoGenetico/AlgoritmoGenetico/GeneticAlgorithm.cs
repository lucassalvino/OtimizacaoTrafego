using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithm<GeneType>
    {
        public GeneticAlgorithm(int numberOfGenerations, BaseEvaluation<GeneType> Evaluation)
        {
            if (numberOfGenerations <= 0)
                throw new Exception("The number of the generations it is invalid");
            if (Evaluation == null)
                throw new Exception("The Evaluation it's not defined");
            NumberOfGenerations = numberOfGenerations;
            this.Evaluation = Evaluation;
        }
        public List<Generation<GeneType>> Population { get; set; } = null;
        public int NumberOfGenerations { get; set; }
        private BaseEvaluation<GeneType> Evaluation = null;
        public void DefineInitialPopulation(int numberOfIndividuals, int numberOfGenesOfIndividuals, List<Chromosome<GeneType>> PopulationInitial)
        {
            if (numberOfIndividuals <= 0)
                throw new Exception("The number of individuals it's invalid");
            if (numberOfGenesOfIndividuals <= 0)
                throw new Exception("The number of genes of individuals it's invalid");
            if (PopulationInitial == null || PopulationInitial.Count <= 0)
                throw new Exception("The population initial has not been set");
            if (PopulationInitial.Count != numberOfIndividuals)
                throw new Exception("The number of the individuals not is equals to the number of the individuals of population initial");
            Generation<GeneType> p = new Generation<GeneType>(numberOfIndividuals, numberOfGenesOfIndividuals);
            PopulationInitial.ForEach(x => p.AddIndividualChromosome(x));
            Population = new List<Generation<GeneType>> { p };
        }
        public void Run()
        {
            if (Population == null || Population.Count <= 0)
                throw new Exception("the initial population has not been defined");
            if (NumberOfGenerations <= 0)
                throw new Exception("the number of the generations must be greater than zero");
            for (int i = 0; i < NumberOfGenerations; i++)
                Population.Add(Population.Last().GenerateNextGeneration(Evaluation));
        }

        public Chromosome<GeneType> GetBestChromosome()
        {
            if (Population == null || Population.Count == 0)
                throw new Exception("The genetic algorithm hass not been executed");
            Chromosome<GeneType> ValueReturn = null;
            double Evaluation = double.MinValue;
            Population.ForEach(x =>
            {
                x.Individuals.ForEach(y => {
                    if(y.Fitness > Evaluation)
                    {
                        Evaluation = y.Fitness;
                        ValueReturn = y;
                    }
                });
            });
            return ValueReturn;
        }
    }
}