namespace jwellone.Toolbar.Editor
{
	public interface IToolbarUI
	{
		string Name { get; }

		void OnGUI();
	}
}
