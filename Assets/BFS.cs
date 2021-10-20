using System.Collections.Generic;

namespace Sokoban
{
    class BFS : SimpleSearch
    {
        protected override Frontier getEmptyFrontier()
        {
            return new BFSQueue();
        }

        protected override Heuristic getHeuristic()
        {
            return new NullHeuristic();
        }
    }

    class BFSQueue : Frontier
    {
        private Queue<(State, List<Direction>)> queue;

        public BFSQueue()
        {
            this.queue = new Queue<(State, List<Direction>)>();
        }

        public void push(State state, List<Direction> path, int hVal)
        {
            this.queue.Enqueue((state, path));
        }

        public (State, List<Direction>) pop()
        {
            return this.queue.Dequeue();
        }

        public bool isEmpty()
        {
            return this.queue.Count == 0;
        }
    }

}