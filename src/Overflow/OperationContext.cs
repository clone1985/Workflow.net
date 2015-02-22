using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overflow
{
    class OperationContext
    {
        private readonly Dictionary<Type, object> _values = new Dictionary<Type, object>(); 

        public void RegisterOutputHandlers(IOperation operation)
        {
            var outputOperationInterfaces = operation.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IOutputOperation<>));
            foreach (var outputOperationType in outputOperationInterfaces)
                RegisterOutputHandler(operation, outputOperationType);
        }

        private void RegisterOutputHandler(IOperation operation, Type outputOperationType)
        {
            var outputType = outputOperationType.GetGenericArguments()[0];
            var registerHandlerMethod = outputOperationType.GetMethod("Output");

            var outputHandler = CreateOutputHandler(outputType);
            registerHandlerMethod.Invoke(operation, new object[] { outputHandler });
        }

        private Delegate CreateOutputHandler(Type type)
        {
            var method = typeof (OperationContext).GetMethod("OnOutput", BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethod = method.MakeGenericMethod(type);
            var actionT = typeof(Action<>).MakeGenericType(type);
            return Delegate.CreateDelegate(actionT, this, genericMethod);
        }

        private void OnOutput<TOutput>(TOutput output)
        {
            var key = typeof (TOutput);
            if (_values.ContainsKey(key))
                _values.Remove(key);

            _values.Add(key, output);
        }

        public void ProvideInputs(IOperation operation)
        {
            var inputOperationInterfaces = operation.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IInputOperation<>));
            foreach (var inputOperationType in inputOperationInterfaces)
                ProvideInput(operation, inputOperationType);
        }

        private void ProvideInput(IOperation operation, Type inputOperationType)
        {
            var inputType = inputOperationType.GetGenericArguments()[0];
            var provideInputMethod = inputOperationType.GetMethod("Input");

            var input = GetInput(inputType);
            provideInputMethod.Invoke(operation, new []{ input });
        }

        private object GetInput(Type inputType)
        {
            if (!_values.ContainsKey(inputType)) return null;

            return _values[inputType];
        }
    }
}
