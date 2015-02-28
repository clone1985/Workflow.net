using System;
using System.Linq;

namespace Overflow
{
    class OperationBehaviorAttributeBuilder : IOperationBehaviorBuilder
    {
        public IOperation ApplyBehavior(IOperation operation, WorkflowConfiguration configuration)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            var decoratorAttributes = operation.GetType().GetCustomAttributes(typeof(OperationBehaviorAttribute), inherit: false);
            if (decoratorAttributes.Length == 0) return operation;

            foreach (var decoratorAttribute in decoratorAttributes.OfType<OperationBehaviorAttribute>())
                operation = decoratorAttribute.AddBehavior(operation);

            return operation;
        }


    }
}