using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
//using Microsoft.VisualBasic.CompilerServices;

namespace Sokoban
{
    public abstract class IQAgent
    {
        private Random rand = new Random();
        
        public abstract void update(State s, Direction action, State sPrime, float reward);

        public float alpha { get; set; }
        public float discount { get; set; }
        public float epsilon { get; set; }

        protected abstract float getQValue(State s, Direction action);
        
        protected Dictionary<State, Dictionary<Direction, float>> qTable;

        protected void setQValue(State s, Direction action, float value)
        {
            ensureTableRow(s);
            qTable[s][action] = value;
        }

        protected void ensureTableRow(State s)
        {
            if (!qTable.ContainsKey(s))
            {
                qTable.Add(s, new Dictionary<Direction, float>());
            }
        }

        protected float computeValueFromQValues(State state)
        {
            Dictionary<Direction, float> mapping = getActionValueEntries(state);
            if (mapping.Count == 0)
            {
                return 0.0f;
            }
            
            return mapping.Values.Aggregate((v1, v2) => Math.Max(v1, v2));
        }

        protected Dictionary<Direction, float> getActionValueEntries(State s)
        {
            Dictionary<Direction, float> mapping = new Dictionary<Direction, float>();
            foreach (Direction action in s.getLegalActions())
            {
                mapping[action] = getQValue(s, action);
            }

            return mapping;
        }

        public Optional<Direction> computeAction(State state)
        {
            if (rand.Next(0, 1) < epsilon)
            {
                var legalActions = state.getLegalActions();
                return new Some<Direction>(legalActions[rand.Next(0, legalActions.Count)]);
            }

            return computeActionFromQValues(state);
        }

        protected Optional<Direction> computeActionFromQValues(State state)
        {
            var mapping = getActionValueEntries(state);
            if (mapping.Count == 0)
            {
                return new None<Direction>();
            }

            float maxQ = mapping.Values.Aggregate((a, b) => Math.Max(a, b));
            var choices = new List<Direction>(
                mapping
                    .Where(q => Math.Abs(q.Value - maxQ) < 0.001)
                    .Select(e => e.Key));
            
            return new Some<Direction>(choices[rand.Next(0, choices.Count())]);
        }

        protected IQAgent(float alpha, float discount, float epsilon)
        {
            
            this.qTable = new Dictionary<State, Dictionary<Direction, float>>();
        }
    }
    
    public class QAgent : IQAgent
    {
        
        public QAgent(float alpha, float discount, float epsilon) : base(alpha, discount, epsilon) {}

        protected override float getQValue(State s, Direction action)
        {
            ensureTableRow(s);
            float value = 0.0f;
            qTable[s].TryGetValue(action, out value);
            return value;
        }

        public override void update(State s, Direction action, State sPrime, float reward)
        {
            float oldQ = getQValue(s, action);
            float futureEstimate = computeValueFromQValues(sPrime);

            float newValue = (1.0f - alpha) * oldQ + alpha * (reward + discount * futureEstimate);

            setQValue(s, action, newValue);
        }
    }

    public class QFeatureAgent : IQAgent
    {

        private Dictionary<Feature, float> weights;

        public QFeatureAgent(float alpha, float discount, float epsilon) : base(alpha, discount, epsilon)
        {
            this.weights = new Dictionary<Feature, float>();
            weights.Add(Feature.SOLVED_BOXES, 0);
            weights.Add(Feature.UNSOLVED_BOXES, 0);
        }

        enum Feature
        {
            SOLVED_BOXES, UNSOLVED_BOXES
        }
        
        public override void update(State s, Direction action, State sPrime, float reward)
        {
            float difference = (reward + discount * computeValueFromQValues(sPrime)) - getQValue(s, action);
            Dictionary<Feature, float> alphaDiff = scalarProduct(extractFeatures(s), difference * alpha);
            weights = addVecs(weights, alphaDiff);
        }

        protected override float getQValue(State state, Direction action)
        {
            return dotProduct(weights, extractFeatures(state));
        }

        private static Dictionary<Feature, float> extractFeatures(State state)
        {
            Dictionary<Feature, float> vector = new Dictionary<Feature, float>();
            int numBoxes = state.boxes.Count;
            int placeBoxes = state.CountPlacedBoxes();
            vector.Add(Feature.SOLVED_BOXES, placeBoxes);
            vector.Add(Feature.UNSOLVED_BOXES, numBoxes - placeBoxes);
            return vector;
        }

        private static float dotProduct(Dictionary<Feature, float> a, Dictionary<Feature, float> b)
        {
            return a[Feature.SOLVED_BOXES] * b[Feature.SOLVED_BOXES]
                   + a[Feature.UNSOLVED_BOXES] * b[Feature.UNSOLVED_BOXES];
        }

        private static Dictionary<Feature, float> scalarProduct(Dictionary<Feature, float> vec, float f)
        {
            Dictionary<Feature, float> product = new Dictionary<Feature, float>();
            foreach (Feature key in vec.Keys)
            {
                product.Add(key, vec[key] * f);
            }

            return product;
        }

        private static Dictionary<Feature, float> addVecs(Dictionary<Feature, float> a, Dictionary<Feature, float> b)
        {
            Dictionary<Feature, float> sum = new Dictionary<Feature, float>();
            foreach (Feature key in a.Keys)
            {
                sum.Add(key, a[key] + b[key]);
            }

            return sum;
        }
    }
}