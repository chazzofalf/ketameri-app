using System.Diagnostics;
using SkiaSharp;

namespace Symbs;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
	private Util? _util;
	Util Util => _util = _util ??  new Util();
	public SKBitmap MakeTransparency(SKBitmap bitmap)
	{
		return Util.MakeTransparency(bitmap);
	} 
	public void PostException(Exception e)
	{
		if (e is AggregateException ae)
		{
			ae.Flatten().InnerExceptions.Select(ie => (Action)(() => PostException(ie)))
			.ToList()
			.ForEach(exc_act => exc_act());
		}
		else
		{
			Debug.Print($"Exception Thrown: {e.GetType().FullName}");
			Debug.Print($"    Exception Message: {e.Message}");
			Debug.Print($"    StackTrace:");
			Debug.Print(string.Join("\n",e.StackTrace?.Split("\n")
			.Select(s => $"        {s}") ?? new [] {"Null Exception"}));
			Debug.Flush();
		}
	}
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}