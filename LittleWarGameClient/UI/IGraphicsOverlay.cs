namespace LittleWarGameClient.UI
{
	public interface IGraphicsOverlay
	{
		public void InvokeUI(Action action);
		public void ChangeTransparencyKeyTo(System.Drawing.Color color);
		public void Close();
		public void SetLocationTo(Point newLocation);
		public void SetSizeTo(Size newSize);
		public void MakeVisible(bool option);
	}
}
