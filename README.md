# Frends.Kungsbacka.Json

This is a drop in replacement for Frends.Json.

Additional features/changes:

* Frends.Json has a method for converting to JToken. Currently it supports string and
  JToken. Frends.Kungsbacka.Json adds support for any deserializable object. This includes
  `CaseInsensitivePropertyTree` which is the object Frends use to deserialize data coming
  from another process (i.g. invoking a sub process). `CaseInsensitivePropertyTree` is also
  used for trigger parameters. This removes the need to manually deserialize data passed
  between processes before passing it to a Json task.
* Adds support for using [[angle brackets]] instead of {{curly braces}} in Handlebars
  templates and partials. The angle brackets gets replaced with curly braces before
  the template is passed to Handlebars. This means that the template or partial no longer
  has to be an expression with a verbatim string (@"template"), but can be text instead.
  This makes it possible to use Frends expression syntax directly inside templates without
  adding an extra task to first create the template.
  Please note that this feature is experimental. More tests are needed.
* Adds support for custom helpers in a Handlebars task. Custom helpers are declared inside
  a C# statement and assigned to a variable that can then be referenced in a Handlebars task.
* Introducing a new task called Map that can create a new `JObject` by querying an existing
  `JObject`. It can handle defaults if a property does not exist and do simple transformations.
  Future versions of the Map task will allow for adding custom transformations.
* `ConvertXmlStringToJToken` has got a new sibling task called `ConvertXmlBytesToJToken`. It's
   useful when you can't know the Xml encoding without parsing the Xml declaration. Using this
   task you don't have to convert the Xml content to a string before converting it to Json.
   Instead you let `System.Xml.XmlDocument` figure out the encoding.
* `Query`, `QuerySingle`, `ConvertJsonStringToJToken` and `ConvertXmlStringToJToken` should all
   work as they do in Frends.Json with the addition of being able to deserialize more types of
   input (see first point).
