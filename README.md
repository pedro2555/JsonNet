# JsonNet

The main goal is to provided a functionality set very similiar to the BinaryFormatter class in .NET.
Allowing you to directly serialize and deserialize a .NET object into and from a JSON formatted stream.

A quick settings file, user preferences, anything can be dealt with as powerfull .NET objects, and have it effortlessly pressisted to disc, cloud, api, in JSON format with very little boilerplate code.

## Quick use guide

### MSDN Example

The follwing C# Console Application is an adaptation, with miminal tweaks required, of the MSDN example for the BinaryFormatter class.

```
using JsonSerializer;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;

public class App
{
    [STAThread]
    static void Main()
    {
        Serialize();
        Deserialize();
    }

    static void Serialize()
    {
        // Create a hashtable of values that will eventually be serialized.
        Hashtable addresses = new Hashtable();
        addresses.Add("Jeff", "123 Main Street, Redmond, WA 98052");
        addresses.Add("Fred", "987 Pine Road, Phila., PA 19116");
        addresses.Add("Mary", "PO Box 112233, Palo Alto, CA 94301");

        // To serialize the hashtable and its key/value pairs,  
        // you must first open a stream for writing. 
        // In this case, use a file stream.
        FileStream fs = new FileStream("DataFile.json", FileMode.Create);

        // Construct a Serializer and use it to serialize the data to the stream.
        Serializer serializer = new Serializer();
        try
        {
            serializer.Serialize(fs, addresses);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }


    static void Deserialize()
    {
        // Declare the hashtable reference.
        Hashtable addresses = null;

        // Open the file containing the data that you want to deserialize.
        FileStream fs = new FileStream("DataFile.json", FileMode.Open);
        try
        {
            Serializer serializer = new Serializer();

            // Deserialize the hashtable from the file and 
            // assign the reference to the local variable.
            addresses = (Hashtable)serializer.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }

        // To prove that the table deserialized correctly, 
        // display the key/value pairs.
        foreach (DictionaryEntry de in addresses)
        {
            Console.WriteLine("{0} lives at {1}.", de.Key, de.Value);
        }

        Console.Read();
    }
}
```
