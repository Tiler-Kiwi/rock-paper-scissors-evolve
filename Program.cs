using System;
using System.Collections.Generic;

namespace rockpaperscissorstrials
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			
			GenerationMatches fights = new GenerationMatches(300,100,100,100,100);
			fights.PlayItOut();
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
	
	public enum Choice
	{
		NONE = 0,
		Rock = 1,
		Paper = 2,
		Scissors = 3
	}
	
	public interface Strategem
	{
		Choice GetChoice();
		int ID{get;}
	}
	
	public class AlwaysRock:Strategem
	{
		public Choice GetChoice()
		{
			return Choice.Rock;
		}
		public int ID{get{return 1;}}
	}
	public class AlwaysPaper:Strategem
	{
		public Choice GetChoice()
		{
			return Choice.Paper;
		}
		public int ID{get{return 2;}}
	}
	
	public class AlwaysScissors:Strategem
	{
		public Choice GetChoice()
		{
			return Choice.Scissors;
		}
		public int ID{get{return 3;}}
	}
	
	public class RandomChoice:Strategem
	{
		private static Random RNG = new Random();
		public Choice GetChoice()
		{
			int num = RNG.Next(1,4);
			//Console.WriteLine("DEBUG: CHOSE {0}",(Choice)num);
			return (Choice)num;
		}
		public int ID{get{return 0;}}
	}
	
	public static class MatchUp
	{
		private static Random RNG = new Random();
		
		static int RockBeatsPaperChance = 40;
		
		public static Tuple<decimal, decimal> Match(Strategem a, Strategem b)
		{	
			Choice A = a.GetChoice();
			Choice B = b.GetChoice();
			return  Match(A, B);
		}
		public static Tuple<decimal, decimal> Match(Choice A, Choice B)
		{
			if(A==B)
			{
			int WinTies = RNG.Next(0,2);
			return new Tuple<decimal, decimal>(WinTies, 1-WinTies);	
			}
			if(A == Choice.Rock)
			{
				switch(B)
				{
					case Choice.Scissors:
						return new Tuple<decimal, decimal>(1,0);
					case Choice.Paper:
						if(RNG.Next(1,101)>(RockBeatsPaperChance))
						{
						return new Tuple<decimal, decimal>(0,1);
						}
						return new Tuple<decimal, decimal>(1,0);
				}
			}
			if(A == Choice.Scissors)
			{
				switch(B)
				{
					case Choice.Rock:
						return new Tuple<decimal, decimal>(0, 1);
					case Choice.Paper:
						return new Tuple<decimal, decimal>(1,0);
				}
			}
			if(A==Choice.Paper)
			{
				switch(B)
				{
					case Choice.Rock:
						if(RNG.Next(1,101)>(RockBeatsPaperChance))
						{
						return new Tuple<decimal, decimal>(1,0);
						}
						return new Tuple<decimal,decimal>(0,1);
					case Choice.Scissors:
						return new Tuple<decimal, decimal>(0,1);
				}
			}
			throw new Exception("fuck");
		}
	}
	
	public class GenerationMatches
	{
		public int Generations;
		public int Randoms;
		public int Rocks;
		public int Papers;
		public int Scissors;
		
		int RandMin{get{return Randoms==0?0:1;}}
		int RockMin{get{return Rocks==0?0:1;}}
		int PaperMin{get{return Papers==0?0:1;}}
		int ScisMin{get{return Scissors==0?0:1;}}
		
		
		
		public GenerationMatches(int gens, int rand, int rock, int paper, int scis)
		{
			Generations = gens;
			Randoms = rand;
			Rocks = rock;
			Papers = paper;
			Scissors = scis;
		}
		
		private class Pop
		{
			public Strategem strat;
			public decimal score;
			public Pop(Strategem s)
			{
				strat = s;
			}
		}
		
		public void PlayItOut()
		{
			List<Pop> Population = new List<Pop>();
			for(int i=0;i<Randoms;i++)
			{
				Strategem Cur = new RandomChoice();
				Population.Add(new Pop(Cur));
			}
			for(int i=0;i<Rocks;i++)
			{
				Strategem Cur = new AlwaysRock();
				Population.Add(new Pop(Cur));
			}
			for(int i=0;i<Papers;i++)
			{
				Strategem Cur = new AlwaysPaper();
				Population.Add(new Pop(Cur));
			}
			for(int i=0;i<Scissors;i++)
			{
				Strategem Cur = new AlwaysScissors();
				Population.Add(new Pop(Cur));
			}
			
			for(int i=0;i<Generations;i++)
			{
			double RockUse = 0;
			double PaperUse = 0;
			double ScissorsUse = 0;
				for(int k=0;k<Population.Count;k++)
				{
					for(int j=k+1;j<Population.Count;j++)
					{
						Choice K = Population[k].strat.GetChoice();
						Choice J = Population[j].strat.GetChoice();
						Tuple<decimal, decimal> Score = MatchUp.Match(K,J);
						Population[k].score += Score.Item1;
						Population[j].score += Score.Item2;
						
						switch(K)
						{
							case Choice.Rock:
								RockUse++;
								break;
							case Choice.Paper:
								PaperUse++;
								break;
							case Choice.Scissors:
								ScissorsUse++;
								break;
						}
						switch(J)
						{
							case Choice.Rock:
								RockUse++;
								break;
							case Choice.Paper:
								PaperUse++;
								break;
							case Choice.Scissors:
								ScissorsUse++;
								break;
						}	
					}
				}
				
				Population.Sort(delegate(Pop A, Pop B)
				                {
				                	if(A.score > B.score){return 1;}
				                	if(A.score == B.score){return 0;}
				                	return -1;
				                });
				
				double PopThird = Population.Count * 0.2;
				for(int k=0; k<PopThird; k++)
				{
					Population.RemoveAt(0);
				}
				List<Pop> NewPops = new List<GenerationMatches.Pop>();
				for(int k=0;k<PopThird;k++)
				{
					NewPops.Add(new Pop(Population[Population.Count-1-k].strat));
				}
				for(int k=0;k<NewPops.Count;k++)
				{
					Population.Add(NewPops[k]);
				}
				
				int RandomCount = 0;
				int RockCount = 0;
				int PaperCount = 0;
				int ScissorsCount = 0;
				for(int k=0;k<Population.Count;k++)
				{
					switch(Population[k].strat.ID)
					{
						case 0:
							RandomCount++;
							break;
						case 1:
							RockCount++;
							break;
						case 2:
							PaperCount++;
							break;
						case 3:
							ScissorsCount++;
							break;
					}
					Population[k].score =0;
				}
				if(RandomCount<RandMin){Population[0]=(new Pop(new RandomChoice()));}
				if(PaperCount<PaperMin){Population[1]=(new Pop(new AlwaysPaper()));}
				if(RockCount<RockMin){Population[2]=(new Pop(new AlwaysRock()));}
				if(ScissorsCount<ScisMin){Population[3]=(new Pop(new AlwaysScissors()));}
				
				double TotalUse = RockUse+PaperUse+ScissorsUse;
				
				Console.Write(" Gen:{0,-5} Pops: Ra-{1,3} Ro-{2,-3} Pa-{3,-3} Sc-{4,-3}", i, RandomCount, RockCount, PaperCount, ScissorsCount);
				Console.WriteLine(" Choices: Ro-{0,-2}% Pa-{1,-2}% Sc-{2,-2}%", Math.Round(RockUse/TotalUse,2)*100, Math.Round(PaperUse/TotalUse,2)*100, Math.Round(ScissorsUse/TotalUse,2)*100);
			}
		}
	}
}