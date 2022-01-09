using System;
using System.Collections.Generic;
using System.Linq;

namespace NaturalDams
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) => ShowError((Exception)eventArgs.ExceptionObject);

            try
            {
                var landscape = new Landscape(args.Select(Int32.Parse));
                landscape.Fill();
                Console.WriteLine($"Total water: {landscape.TotalWater}");
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            Console.ReadLine();
        }

        private static void ShowError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("" + exception);
        }
    }

    public class Terrain 
    {
        public static Terrain Create(int height) => new Terrain(height);
        
        public Terrain(int height)
        {
            Height = height;
        }

        public void AddWater(int amount)
        {
            WaterLevel += amount;
            // all my water drained off
            if (WaterLevel < 0) WaterLevel = 0;
        }

        public override string ToString()
        {
            return $"Terrain(Height={Height},Water={WaterLevel},Current={CurrentLevel})";
        }

        public int Height { get; }
        public int WaterLevel { get; private set; }
        public int CurrentLevel => WaterLevel + Height;
    }

    public class Landscape
    {
        public Landscape(IEnumerable<int> heights)
        {
            _terrain = heights.Select(Terrain.Create).ToList();
        }

        public void Fill()
        {
            if (_terrain.Count > 1)
                AddWater(0, 0);
        }

        private void AddWater(int index, int highest)
        {
            // whatever's at the "edge" falls off
            if (index >= _terrain.Count - 1)
                return;

            // safe indexing b/c of previous check
            var terrain = _terrain[index];
            var east = _terrain[index + 1];

            // get the highest I've seen so far
            highest = Math.Max(highest, terrain.Height);

            // add that much water (any more would run off)
            terrain.AddWater(highest - terrain.Height);
            
            // go to the next terrain
            AddWater(index + 1, highest);

            // slough off water to east
            if (terrain.CurrentLevel > east.CurrentLevel)
            {
                // this is a negative number, removing as much water as it can
                terrain.AddWater(east.CurrentLevel - terrain.CurrentLevel);
            }
        }
        
        public int TotalWater => _terrain.Sum(t => t.WaterLevel);

        private IList<Terrain> _terrain;
    }
}