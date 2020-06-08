using System;
using TekHow.Core.Extensions;

public static class UniqueId
{
    private static string _currentId = "";
    public static string NewId()
    {
        _currentId = Guid.NewGuid().ToString().ToMd5String();
        return _currentId;
    }
}