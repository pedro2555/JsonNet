# JsonNet
.NET Json Library provides a super simple JSON reading and writing implementation in C# (to use in any .NET enviromen).

Currently only supports reading a JSON stream into a .NET manageable objects neatly organized in Hashtable structures.

## How to use

### Parse JSON

JSON parsing is available with `Parser.ReadValue()`.

`ReadValue()` accepts:
  * an array of UTF-8 byte enconded JSON string characters (`byte[]`);
  * and an integer indicating where the JSON string starts in the array.
  
  A use example using a source string:
    
```
using JsonNet;

...

string JSON_string = "[ \"strings\", -12.35654,345 ,\r\n \t-234,12.3,{\"string\":\"value\",\"number\":-12.35654},[],true,false,null,]";
int position = 0;

object result = Parser.ReadValue(
  new UTF8Encoding().GetBytes(JSON_string),
  ref position);
```

### Compose JSON

TODO
