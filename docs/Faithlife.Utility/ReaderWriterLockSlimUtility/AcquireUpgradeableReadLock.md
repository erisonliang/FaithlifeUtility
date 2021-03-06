# ReaderWriterLockSlimUtility.AcquireUpgradeableReadLock method

Enters an upgradeable read lock and returns a disposable object that, when disposed, will exit the lock.

```csharp
public static IDisposable AcquireUpgradeableReadLock(this ReaderWriterLockSlim theLock)
```

| parameter | description |
| --- | --- |
| theLock | The lock on which the upgradeable read lock should be entered. |

## Return Value

A disposable object that, when disposed, will exit the lock.

## See Also

* class [ReaderWriterLockSlimUtility](../ReaderWriterLockSlimUtility.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
