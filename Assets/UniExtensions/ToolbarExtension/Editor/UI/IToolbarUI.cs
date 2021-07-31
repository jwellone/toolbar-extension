namespace UniExtensions.Editor.Toolbar
{
	public interface IToolbarUI
	{
		string Name { get; }

		void OnGUI();
	}
}
