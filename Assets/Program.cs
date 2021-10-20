using System;
using System.Collections.Generic;

namespace Sokoban
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = @"C:\Users\Zach\RiderProjects\Sokoban\Sokoban\level2.txt";
            runSearch(path, 0);
            runQ(path, 0);
        }

        public static List<Direction> runSearch(string levelFilePath, int index)
        {
            State initialState = new State(levelFilePath, index);
            AStar aStar = new AStar(new UnMatchedBoxesHeuristic());
            return aStar.solve(initialState);
        }

        public static List<Direction> runQ(string levelFilePath, int index)
        {
            State initialState = new State(levelFilePath, index);
            QFeatureAgent agent = new QFeatureAgent(0.8f, 0.4f, 0.75f);
            QHarness harness = new QHarness(agent, initialState);
            return harness.train(1000);
        }
    }




}