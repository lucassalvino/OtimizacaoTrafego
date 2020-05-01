using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    public class Chromosome<GeneType> : BaseEntity
    {
        public Guid Id { get; set; }
        public Chromosome(int maximumNumberGene)
        {
            if (maximumNumberGene <= 0)
                throw new Exception("The Maximum number of genes must to be bigger zero");
            NumberOfGene = maximumNumberGene;
            Id = Guid.NewGuid();
        }
        public List<GeneType> Genes { get; private set; }
        public int NumberOfGene { get; set; }
        public double Fitness { get; set; } = 0;
        public void AddGene(GeneType Gene)
        {
            #region Validations
            if (Genes == null)
                Genes = new List<GeneType>();
            if (Genes.Count > NumberOfGene)
                throw new Exception("Violation of Maximum Number of Genes");
            if (Gene == null)
                throw new NullReferenceException();
            #endregion Validations
            Genes.Add(Gene);
        }
        public Chromosome<GeneType> CrossOverOnePoint(Chromosome<GeneType> Mother)
        {
            #region Validations
            ParentSkillsForCrossing(this, Mother);
            #endregion Validations
            var child = new Chromosome<GeneType>(NumberOfGene);
            int pointPartition = new Random((int)DateTime.Now.Ticks).Next() % Genes.Count, i = 0;
            for (; i < pointPartition; i++)
                child.AddGene(Genes[i]);
            for (; i < Genes.Count; i++)
                child.AddGene(Mother.Genes[i]);
            return child;
        }
        public Chromosome<GeneType> CrossOverTwoPoints(Chromosome<GeneType> Mother)
        {
            #region Validations
            ParentSkillsForCrossing(this, Mother);
            #endregion Validations
            var child = new Chromosome<GeneType>(NumberOfGene);
            var rand = new Random((int)DateTime.Now.Ticks);
            int firstPointPartition = rand.Next() % Genes.Count,
                secondPointPartition = rand.Next() % Genes.Count, i = 0;

            #region ValidationOrder
            while (firstPointPartition == secondPointPartition && NumberOfGene >= 2)
                secondPointPartition = rand.Next() % Genes.Count;
            if (firstPointPartition > secondPointPartition)
            {
                var aux = secondPointPartition;
                secondPointPartition = firstPointPartition;
                firstPointPartition = aux;
            }
            #endregion ValidatioOrder

            for (; i < firstPointPartition; i++)
                child.AddGene(Genes[i]);
            for (; i < secondPointPartition; i++)
                child.AddGene(Mother.Genes[i]);
            for (; i < Genes.Count; i++)
                child.AddGene(Genes[i]);
            return child;
        }
        public void PerformsMutation(Random Random)
        {
            int firstGene = Random.Next() % NumberOfGene;
            int secondtGene = Random.Next() % NumberOfGene;
            while (secondtGene == firstGene)
                secondtGene = Random.Next() % NumberOfGene;
            var Backup = Genes[firstGene];
            Genes[firstGene] = Genes[secondtGene];
            Genes[secondtGene] = Backup;
        }
        private void ParentSkillsForCrossing(Chromosome<GeneType> Father, Chromosome<GeneType> Mother)
        {
            if (Father == null || Mother == null || Father.Genes == null || Mother.Genes == null)
                throw new NullReferenceException();
            if (Father.Genes.Count != NumberOfGene || Mother.Genes.Count != NumberOfGene)
                throw new Exception("The number of genes it's incomplet");
            if (Father.Genes.Count != Mother.Genes.Count)
                throw new Exception("The number of genes should to be equals");
        }
    }
}
