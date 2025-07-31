namespace WatcherEye.Core.Exceptions;

public class WatcherEyeCoreException : Exception
{
	public WatcherEyeCoreException(string message) : base(message) { }
	public WatcherEyeCoreException(string message, Exception inner) : base(message, inner){}
}