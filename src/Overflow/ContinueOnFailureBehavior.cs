namespace Overflow
{
    class ContinueOnFailureBehavior : OperationBehavior
    {
        public override BehaviorIntegrityMode IntegrityMode
        {
            get { return BehaviorIntegrityMode.MaintainsWorkflowIntegrity; }
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch { }
        }
    }
}
