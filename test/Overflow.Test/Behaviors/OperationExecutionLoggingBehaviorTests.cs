using System;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test.Behaviors
{
    public class OperationExecutionLoggingBehaviorTests
    {
        [Theory, AutoMoqData]
        public void The_behavior_has_logging_level_precedence(IWorkflowLogger logger)
        {
            var sut = new OperationExecutionLoggingBehavior(logger);

            Assert.Equal(BehaviorPrecedence.Logging, sut.Precedence);
        }

        [Theory, AutoMoqData]
        public void Executing_the_behavior_logs_the_start_and_end_of_the_operation(FakeWorkflowLogger logger, IOperation innerOperation)
        {
            var sut = new OperationExecutionLoggingBehavior(logger).Attach(innerOperation);

            sut.Execute();

            Assert.Equal(1, logger.StartedOperations.Count);
            Assert.Equal(innerOperation, logger.StartedOperations[0]);
            Assert.Equal(1, logger.FinishedOperations.Count);
            Assert.Equal(innerOperation, logger.FinishedOperations[0]);
        }

        [Theory, AutoMoqData]
        public void The_innermost_operation_is_logged(FakeWorkflowLogger logger, IOperation innerOperation)
        {
            var sut = new OperationExecutionLoggingBehavior(logger).Attach(new FakeOperationBehavior().Attach(innerOperation));

            sut.Execute();

            Assert.Equal(innerOperation, logger.StartedOperations[0]);
            Assert.Equal(innerOperation, logger.FinishedOperations[0]);
        }

        [Theory, AutoMoqData]
        public void Start_and_finish_are_logged_in_case_of_failure(FakeWorkflowLogger logger)
        {
            var innerOperation = new FakeOperation { ThrowOnExecute = new Exception() };
            var sut = new OperationExecutionLoggingBehavior(logger).Attach(innerOperation);

            try { sut.Execute(); }
            catch { }

            Assert.Equal(1, logger.StartedOperations.Count);
            Assert.Equal(1, logger.FinishedOperations.Count);
        }
    }
}