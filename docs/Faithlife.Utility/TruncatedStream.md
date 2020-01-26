# TruncatedStream class

[`TruncatedStream`](TruncatedStream.md) is a read-only stream wrapper that will not read past the specified length.

```csharp
public sealed class TruncatedStream : WrappingStreamBase
```

## Public Members

| name | description |
| --- | --- |
| [TruncatedStream](TruncatedStream/TruncatedStream.md)(…) | Creates a new truncated stream. |
| override [CanWrite](TruncatedStream/CanWrite.md) { get; } | Returns false; writes are not supported. |
| override [Length](TruncatedStream/Length.md) { get; } | Returns the (truncated) length of the stream. |
| override [Position](TruncatedStream/Position.md) { get; set; } | The current position in the stream. |
| override [Flush](TruncatedStream/Flush.md)() | Throws an exception; writes are not supported. |
| override [Read](TruncatedStream/Read.md)(…) | Reads from the stream. |
| override [ReadAsync](TruncatedStream/ReadAsync.md)(…) | Reads from the stream asynchronously. |
| override [ReadByte](TruncatedStream/ReadByte.md)() | Reads a byte from the stream. |
| override [Seek](TruncatedStream/Seek.md)(…) | Changes the current position in the stream. |
| override [SetLength](TruncatedStream/SetLength.md)(…) | Throws an exception; writes are not supported. |
| override [Write](TruncatedStream/Write.md)(…) | Throws an exception; writes are not supported. |
| override [WriteAsync](TruncatedStream/WriteAsync.md)(…) | Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests. |

## See Also

* class [WrappingStreamBase](WrappingStreamBase.md)
* namespace [Faithlife.Utility](../Faithlife.Utility.md)
* [TruncatedStream.cs](https://github.com/Faithlife/FaithlifeUtility/tree/master/src/Faithlife.Utility/TruncatedStream.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->