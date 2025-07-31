namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public static class TypeCaster
{
	public static bool TryCast<TResult>(this object? source, out TResult result)
	{
		result = default;
		if (source is TResult castedResult)
		{
			result = castedResult;
			return true;
		}

		return false;
	}
}