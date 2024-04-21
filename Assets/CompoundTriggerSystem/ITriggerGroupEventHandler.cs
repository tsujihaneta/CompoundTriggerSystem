namespace CompoundTriggerSystem
{
	public interface ITriggerGroupEventHandler
	{
		void OnTriggerGroupEnter(TriggerGroup group);
		void OnTriggerGroupExit(TriggerGroup group);
	}
}
