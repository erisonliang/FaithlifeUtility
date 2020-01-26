# DisposableUtility.DisposeWith&lt;T&gt; method

Adds the specified IDisposable object to *disposables* and returns it.

```csharp
public static T DisposeWith<T>(this T disposable, Disposables disposables)
    where T : IDisposable
```

| parameter | description |
| --- | --- |
| T | The type of the IDisposable object. |
| disposable | The IDisposable object. |
| disposables | A *disposables* that will dispose *disposable* when it is disposed. |

## Return Value

The IDisposable object that was added to *disposables*.

## See Also

* class [Disposables](../Disposables.md)
* class [DisposableUtility](../DisposableUtility.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->