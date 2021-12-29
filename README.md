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

[Handlebars.Net](https://github.com/Handlebars-Net/Handlebars.Net) supports adding custom helper
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

### Query, QuerySingle, ConvertJsonStringToJToken and ConvertXmlStringToJToken

Query, QuerySingle, ConvertJsonStringToJToken and ConvertXmlStringToJToken should all work as
they do in Frends.Json with the addition of being able to deserialize more types of input
([see "All tasks" above](#all-tasks)).

## Documentation

Checkout [Frends.Json](https://github.com/Kungsbacka/Frends.Json) for documentation for most
tasks. Here you will find information about additional tasks and features that is not
present in Frends.Json.

### Anglebrackets in Handlebars templates and partials

Handlebars uses {{curley braces}} for expresions in a template. Since Frends also use Hanlebars
notation for mixing code elements with text, Xml, Json, etc, you have to use expression mode
with a verbatim string (@"") when you create Handlebars templates as to not confuse Frends.
This makes it impossible to mix in Frends code elements in Handlebars templates. One way around
this problem is to create the template in an expression block before the Handlebars task.

By switching to [[angle brackets]] for Handlebars you can now freely mix Handlebars expressions
and code elements with curly braces directly in the Handlebars task. Just change from expression
mode to text, Xml or Json and remove the verbatime string.

Handlebars.Net does not support angle brackets and there is no way to tell Handlebars.Net to
use angle brackets instead of curly braces. Before the template is sent to Handlebars for
compilation, angle brackets are replaced with curly braces. This is done using regular
expressions with balanced groups and a little bit of extra parsing. Handlebars.Net uses a
"real" parser and not regex, so don't expect angle brackets to behave exactly the same as
using curly braces directly. But it will work fine for most cases.

The angle brackets feature has to be enabled under Options. Here is an example of a template
that uses angle brackets.

```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
<Person>
    <FullName>[[firstname]] [[lastname]]</FullName>
    <Created>{{DateTime.Today.ToString("yyyy-MM-dd")}}</Created>
    <Source>{{#var.source}}</Source>
</Person>
```

Before it's compiled by Handlebars Frends will process all code elements and then all angle
brackets are replaced by curly braces.


```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
<Person>
    <FullName>{{firstname}} {{lastname}}</FullName>
    <Created>2021-12-01</Created>
    <Source>Active Directory</Source>
</Person>
```

### Custom Helper for Handlebars

```C#
new Action<System.IO.TextWriter, dynamic, object[]>((writer, context, arguments) =>
{
    // Use writer to ouput data
    // Context contains Json object
    // arguments contains whats supplied as argument when calling the helper
})
```

### Custom Block Helper for Handlebars

```C#
new Action<System.IO.TextWriter, dynamic, dynamic, object[]>((writer, options, context, arguments) =>
{
})
```

### Map

...

### ConvertXmlBytesToJToken

...
