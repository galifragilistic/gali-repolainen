using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        private static readonly char[] Genes = new[]
            {
                'a', 'b', 'c', 'd',
                'e', 'f', 'g', 'h', 
                'i', 'j', 'k', 'l', 
                'm', 'n', 'o', 'p', 
                'q', 'r', 's', 't', 
                'u', 'v', 'x', 'y',
                'z', ' '
            };
        private static readonly Random Rand = new Random();
        static void Main()
        {
            Console.WriteLine("Give a target string.");
            string targetString = Console.ReadLine();
            int dnaLenght = 0;
            if (targetString != null)
            {
                dnaLenght = targetString.Length;
            }
            
            // make a random dna strand, whose lenght is dnaLenght (this is the creature)
            char[] dna = CreateRandomDNA(dnaLenght);
            Console.WriteLine("\nFirst DNA strand: " + new string(dna));
            // ask the number of children a creature can have
            Console.WriteLine("\nHow many children can a creature have?\n(about 100 is recommended for fast results)");
            int amountOfChildren = int.Parse(Console.ReadLine());
            // ask the number of generations
            Console.WriteLine("\nHow many generations?");
            int generations = int.Parse(Console.ReadLine());
            int? perfectGeneration = null;
            int? closestGeneration = null;
            int mostSimilarities = 0;
            for (int i = 1; i <= generations; i++)
            {
                // make the children
                IList<string> children = CreateChildrenWithMutations(dna, amountOfChildren);
                // pick the child that is the most similar to the target string
                string bestChild = MostSimilarToTarget(children, targetString);
                if (bestChild == targetString)
                {
                    if (perfectGeneration == null) perfectGeneration = i;
                }
                else
                {
                    if (perfectGeneration == null)
                    {
                        int similarity = SimilarityToTarget(bestChild, targetString);
                        if (mostSimilarities <= similarity)
                        {
                            mostSimilarities = similarity;
                            closestGeneration = i;
                        }
                    }
                        
                }
                // print every 10th generation
                if (i % 5 == 0) Console.WriteLine(i + ". generation: " + bestChild);
                dna = bestChild.ToArray();
            }
            if (perfectGeneration != null)
                Console.WriteLine("\nA perfect match was first encountered in the " + perfectGeneration + ". generation");
            else
            {
                Console.WriteLine("\nNo perfect match was found. The closest was the " + closestGeneration + 
                    ". generation with " + mostSimilarities + " genes out of " + targetString.Length + " exactly the same.");
            }
            Console.ReadKey();
        }

        private static char[] CreateRandomDNA(int lenght)
        {
            var randomDNA = new char[lenght];
            for (int i = 0; i < randomDNA.Length; i++)
            {
                int randomIndex = Rand.Next(0, Genes.Length);
                randomDNA[i] = Genes[randomIndex];
            }
            return randomDNA;
        }
        private static IList<string> CreateChildrenWithMutations(char[] dna, int amount)
        {
            var children = new List<string>();
            for (int i = 0; i < amount; i++)
            {
                char[] child = Mutate(dna);
                children.Add(new string(child));
            }
            return children;
        }
        private static char[] Mutate(char[] dna)
        {
            // copy dna array to a new array
            var mutatedDNA = new char[dna.Length];
            Array.Copy(dna, mutatedDNA, dna.Length);

            int mutateIndex = Rand.Next(0, dna.Length);
            int randomGene = Rand.Next(0, Genes.Length);
            mutatedDNA[mutateIndex] = Genes[randomGene];

            return mutatedDNA;
        }
        private static string MostSimilarToTarget(IList<string> children, string targetString)
        {
            var similarities = new int[children.Count()];
            for (int i = 0; i < children.Count(); i++)
            {
                similarities[i] = SimilarityToTarget(children[i], targetString);
            }
            int max = MaxIndex(similarities);
            return children[max];
        }
        private static int SimilarityToTarget(string compareString, string targetString)
        {
            return targetString.Where((t, i) => t == compareString[i]).Count();
        }

        public static int MaxIndex<T>(IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            var maxIndex = -1;
            var maxValue = default(T);

            var index = 0;
            foreach (var value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }
    }
}
