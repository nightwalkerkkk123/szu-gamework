using System;

namespace SugarRush.Core
{
    /// <summary>
    /// Simple reactive value wrapper for non-Unity data binding.
    /// </summary>
    [Serializable]
    public class ObservableValue<T>
    {
        [UnityEngine.SerializeField] private T _value;

        public event Action<T> OnChanged;

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                OnChanged?.Invoke(_value);
            }
        }

        public ObservableValue(T initialValue = default)
        {
            _value = initialValue;
        }

        public static implicit operator T(ObservableValue<T> observable) => observable.Value;
    }
}
