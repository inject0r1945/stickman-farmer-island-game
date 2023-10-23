namespace HCGame.Utils
{
    public class LazyValue<T>
    {
        private T _value;
        private bool _isInitialized;
        private InitializerDelegate _initializer;

        public delegate T InitializerDelegate();

        public LazyValue(InitializerDelegate initializer)
        {
            _initializer = initializer;
        }

        public T Value
        {
            get
            {
                ForceInit();
                return _value;
            }
            set
            {
                _isInitialized = true;
                _value = value;
            }
        }

        public void ForceInit()
        {
            if (!_isInitialized)
            {
                _value = _initializer();
                _isInitialized = true;
            }
        }
    }
}