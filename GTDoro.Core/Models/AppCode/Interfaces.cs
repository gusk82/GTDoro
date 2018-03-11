using System;

public interface ILoggable
{
    DateTime? Start { get; }
    DateTime? StartLocal { get; }
}
