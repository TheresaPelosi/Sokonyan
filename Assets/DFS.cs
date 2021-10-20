using System.Collections.Generic;

namespace Sokoban
{
    class DFS : SimpleSearch
    {
        protected override Frontier getEmptyFrontier()
        {
            return new DFSStack();
        }

        protected override Heuristic getHeuristic()
        {
            return new NullHeuristic();
        }
    }

    class DFSStack : Frontier
    {
        private Stack<(State, List<Direction>)> stack;

        public DFSStack()
        {
            this.stack = new Stack<(State, List<Direction>)>();
        }

        public void push(State state, List<Direction> path, int hVal)
        {
            stack.Push((state, path));
        }

        public (State, List<Direction>) pop()
        {
            return this.stack.Pop();
        }

        public bool isEmpty()
        {
            return this.stack.Count == 0;
        }
    }

}