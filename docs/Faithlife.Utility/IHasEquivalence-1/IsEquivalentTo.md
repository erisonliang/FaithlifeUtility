# IHasEquivalence&lt;T&gt;.IsEquivalentTo method

Determines whether the object is equivalent to the specified object.

```csharp
public bool IsEquivalentTo(T other)
```

| parameter | description |
| --- | --- |
| other | The specified object. |

## Return Value

True if the object is equivalent to the specified object.

## Remarks

As with equality (Object.Equals), equivalence should be reflexive, symmetric, transitive, and consistent (as long as the objects in question are not modified). An object should never be equivalent to null.

## See Also

* interface [IHasEquivalence&lt;T&gt;](../IHasEquivalence-1.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
