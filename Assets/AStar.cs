using System.Collections.Generic;

namespace Sokoban
{
    class AStar : SimpleSearch
    {
        private Heuristic heuristic;

        public AStar(Heuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        protected override Frontier getEmptyFrontier()
        {
            return new Priority();
        }

        protected override Heuristic getHeuristic()
        {
            return this.heuristic;
        }
    }
    class Priority : Frontier
    {
        private PriorityQueue<(State, List<Direction>)> q;

        public Priority()
        {
            this.q = new PriorityQueue<(State, List<Direction>)>(isMinPriorityQueue: true);
        }

        public void push(State state, List<Direction> path, int heuristicVal)
        {
            q.Enqueue(heuristicVal + path.Count, (state, path));
        }

        public (State, List<Direction>) pop()
        {
            return q.Dequeue();
        }

        public bool isEmpty()
        {
            return q.Count == 0;
        }
    }
}