using System;
using System.Collections.Generic;

namespace Sokoban
{
    public class QHarness
    {
        private State initial;
        private IQAgent agent;
        
        private static readonly float EMPTY_REWARD = -0.1f;
        private static readonly float INCREASED_BOX_REWARD = 20.0f;
        private static readonly float GOAL_STATE_REWARD = 1000.0f;
        private static readonly float DEADLOCK_REWARD = -500.0f;

        private static readonly int MAX_ACTIONS_PER_EPISODE = 1000;

        private static readonly float EP_DECAY_FACTOR = 0.9f;

        private readonly float defaultEpsilon;
        
        public QHarness(IQAgent agent, State initial)
        {
            this.initial = initial;
            this.agent = agent;
            this.defaultEpsilon = agent.epsilon;
        }

        public List<Direction> train(int numEpisodes)
        {
            State currentState = initial;
            List<Direction> solution = new List<Direction>();
            agent.epsilon = defaultEpsilon;
            for (int episode = 0; episode <= numEpisodes; ++episode)
            {
                if (episode == numEpisodes)
                {
                    agent.epsilon = 0.0f;
                }

                currentState = initial;
                
                Console.WriteLine("Starting episode {0}", episode);
                for (int move = 0; move < MAX_ACTIONS_PER_EPISODE; ++move)
                {
                    Optional<Direction> action = agent.computeAction(currentState);
                    if (episode == numEpisodes)
                    {
                        solution.Add(action.unwrap());
                    }
                    Optional<State> nextState = currentState.getSuccessor(action.unwrap());
                    float reward = CalculateReward(currentState, nextState.unwrap());
                    
                    currentState = nextState.unwrap();
                    agent.update(currentState, action.unwrap(), nextState.unwrap(), reward);

                    
                    if (ShouldExit(nextState.unwrap()))
                    {
                        if (episode == numEpisodes)
                        {
                            nextState.unwrap().print();
                        }
                        break;
                    }
                }

                agent.epsilon *= EP_DECAY_FACTOR;
            }

            return solution;
        }

        private bool ShouldExit(State state)
        {
            return state.isGoalState() || !state.isSolvable();
        }

        private float CalculateReward(State current, State next)
        {
            if (next.isGoalState())
            {
                return GOAL_STATE_REWARD;
            } else if (!next.isSolvable())
            {
                return DEADLOCK_REWARD;
            } else 
            {
                return EMPTY_REWARD + next.CountPlacedBoxes() * INCREASED_BOX_REWARD;
            }
        }
    }
}