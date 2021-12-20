# Frends.Kungsbacka.Json

This is a replacement for Frends.Json.

## New and changed tasks

### All tasks

Frends.Json has a method for converting input to JToken. Currently it supports string and JToken.
Frends.Kungsbacka.Json changes this slightly to support all objects where `ToString()` returns
the object serialized as Json. This includes `CaseInsensitivePropertyTree` which is the object
Frends use to deserialize data coming from another process (i.g. invoking a sub process).
`CaseInsensitivePropertyTree` is also used for trigger parameters. This removes the need to
manually deserialize data passed between processes before passing it to a Json task.

### Validate task

Validate task has switched from using [Json.NET Schema](https://www.newtonsoft.com/jsonschema)
to [NJsonSchema](https://github.com/RicoSuter/NJsonSchema).

### Handlebars task

Handlebars now supports using [[angle brackets]] instead of {{curly braces}} in
Handlebars templates and partials. The angle brackets gets replaced with curly braces before
the template is passed to Handlebars. When using angle brackets the template or partial no longer
has to be an expression with a verbatim string (@"template"), but can be text instead. This opens
up the possibility to use Frends expression syntax directly inside templates without adding an extra
task to create the template. The feature relies on regex with balanced groups and does not use
a full parser. It supports escaping, but there will likely be corner cases that will fail.

[Handlebars.NET](https://github.com/Handlebars-Net/Handlebars.Net) supports adding custom helper
functions. This is now exposed in the Handlebars task. Custom helpers are declared inside a C#
statement and assigned to a variable that can then be referenced in a Handlebars task.

### New Map task

Introducing a new task called Map that can create a new `JObject` by querying an existing `JObject`.
It can handle defaults if a property does not exist and do simple transformations. Future versions
of the Map task will allow for adding custom transformations.

### New ConvertXmlBytesToJToken task

ConvertXmlStringToJToken has got a new sibling task called ConvertXmlBytesToJToken. It's
useful when you can't know the Xml encoding without parsing the Xml declaration. Using this
task you don't have to convert the Xml content to a string before converting it to Json.
Instead you let `System.Xml.XmlDocument` figure out the encoding.

### Query, QuerySingle, 

Query, QuerySingle, ConvertJsonStringToJToken and ConvertXmlStringToJToken should all work as
they do in Frends.Json with the addition of being able to deserialize more types of input
([see "All tasks" above](#all-tasks)).
