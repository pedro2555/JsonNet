# Example Usage

For a  class, class property, struct, or any other data structure, to be visible
to the JsonSerializer, it must have the `[JsonSerializable]` attribute.

```
using JsonSerializer;

[JsonSerializable]
public class MyClass
{
    public string Property1 { get; set; }
            
    public int Property2 { get; set; }

    public MyClass(string property1, int property2)
    {
        this.Property1 = property1;
        this.Property2 = property2;
    }
}
```

The attribute above is marking the whole class (all of it's public parameters)
as visible to the JsonSerializer. But you can also specify only specific
parameters, to do that just alter your class to set the `[JsonSerializable]`
attribute to every parameter you want to be serializable. The example class
below only exposes Property1 to the JsonSerializer

```
using JsonSerializer;

public class MyClass2
{
	[JsonSerializable]
    public string Property1 { get; set; }
            
    public int Property2 { get; set; }

    public MyClass2(string property1, int property2)
    {
        this.Property1 = property1;
        this.Property2 = property2;
    }
}
```

