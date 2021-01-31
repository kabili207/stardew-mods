namespace StardewMuffin
{
	class EventsApi
	{
		ModEntry modEntry;

		public EventsApi(ModEntry modEntry)
		{
			this.modEntry = modEntry;
		}
		

		public event EventHandler OnXPChanged
		{
			add => ME.LEE.OnXPChanged += value;
			remove => ME.LEE.OnXPChanged -= value;
		}
	}
}