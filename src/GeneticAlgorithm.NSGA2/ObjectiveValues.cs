using System.Collections;
using System.Collections.Generic;

namespace EvolutionaryAlgorithm.GeneticAlgorithm.NSGA2
{
    public class ObjectiveValues<T> : IObjectiveValues<T>
    {
        private IReadOnlyDictionary<T, double> _objectiveValues;
        private readonly Comparer _comparer = new Comparer();

        public ObjectiveValues(IReadOnlyDictionary<T, double> values)
        {
            _objectiveValues = values;
        }

        public double this[T key] => _objectiveValues[key];
        public IEnumerable<T> Keys => _objectiveValues.Keys;
        public IEnumerable<double> Values => _objectiveValues.Values;
        public int Count => _objectiveValues.Count;

        public int CompareTo(IObjectiveValues<T> other)
        {
            return _comparer.Compare(this, other);
        }

        public bool ContainsKey(T key) => _objectiveValues.ContainsKey(key);

        public IEnumerator<KeyValuePair<T, double>> GetEnumerator()
        {
            return _objectiveValues.GetEnumerator();
        }

        public bool TryGetValue(T key, out double value)
        {
            return _objectiveValues.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class Comparer : IComparer<IObjectiveValues<T>>
        {
            public int Compare(IObjectiveValues<T> x, IObjectiveValues<T> y)
            {
                var sign = 0;
                foreach (var key in x.Keys)
                {
                    var lsign = x[key].CompareTo(y[key]);
                    if (lsign == 0 || sign != 0 && sign != lsign) return 0;
                    sign = lsign;
                }

                return sign;
            }
        }
    }
}
