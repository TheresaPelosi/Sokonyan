using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    abstract class SimpleSearch
    {
        protected abstract Frontier getEmptyFrontier();

        protected abstract Heuristic getHeuristic();

        private int applyHeuristic(State state)
        {
            return this.getHeuristic().apply(state);
        }

        public List<Direction> solve(State state)
        {
            Frontier frontier = getEmptyFrontier();
            frontier.push(state, new List<Direction>(), applyHeuristic(state));
            return solve(frontier, new HashSet<State>());
        }

        private List<Direction> solve(Frontier frontier, HashSet<State> explored)
        {
            while (!frontier.isEmpty())
            {
                (State currentState, List<Direction> currentPath) = frontier.pop();
                if (explored.Contains(currentState) || !currentState.isSolvable())
                {
                    continue;
                }

                explored.Add(currentState);
                if (currentState.isGoalState())
                {
                    Debug.Log("Search completed.");
                    return currentPath;
                }

                foreach (KeyValuePair<Direction, State> entry in currentState.getSuccessors())
                {
                    State newState = entry.Value;
                    Direction dir = entry.Key;
                    if (explored.Contains(newState) && !currentState.isSolvable())
                    {
                        continue;
                    }

                    List<Direction> newPath = new List<Direction>(currentPath);
                    newPath.Add(dir);

                    frontier.push(newState, newPath, applyHeuristic(newState));
                }
            }

            return new List<Direction>();
        }
    }

    interface Frontier
    {
        void push(State state, List<Direction> path, int heuristicVal);
        (State, List<Direction>) pop();
        bool isEmpty();
    }

}