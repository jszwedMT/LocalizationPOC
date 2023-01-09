# LocalizationPOC
this is a proof of concept of a Unity Localization system consisting of a Editor, for modifying the Localization values.
To open the Localization window, go to Window > Localization Editor

![Capture](https://user-images.githubusercontent.com/108739402/211246405-9a141f51-8a27-43ad-9ea5-21a27d056705.PNG)

You'll be presented with a Editor window containing columns representing each of the languages, and rows representing the values, and keys.

![Capture](https://user-images.githubusercontent.com/108739402/211245971-ad27a0af-0c19-4744-aec9-4da2f08c54b2.PNG)

To add new definitions to the localization system, under the name space Utilities.Localization add a new enum to the Partial Localized class. In doing so, your localizations will be populated with a new section, and keys.
```csharp
namespace Utilities.Localization
{
    public partial class Localized
    {
        public enum Test
        {
            [Description("This is an example test for a localization key")]Test1,
            Test2,
            Test3
        }
    }
}
```

Description attributes are field attributes with the intention to be assigned to Enums. In doing so, will add a tool tip to the localization editor window.

![Capture](https://user-images.githubusercontent.com/108739402/211245891-8a2e360c-d22a-455c-b0a9-ca99249fb2a6.PNG)

To add new languages to the localization system, go to Localized.cs, and add the new language to the following enum, where the named value is the reference key to that language, and the Display name attribute is how the name will appear in the editor window. If there is no Display attribute it will display as the Enums name.
```cs
        public enum Languages
        {
            [DisplayName("Keys")] Keys_Doc,
            [DisplayName("English NA")] English_NA,
            [DisplayName("Dutch")] Dutch,
            [DisplayName("German")] German,
            [DisplayName("French")] French,
            [DisplayName("Spanish")] Spanish,
            [DisplayName("Italian")] Italian,
        }
```

Selecting a language at runtime can be done with the function
```csharp
Localized.Instance.SetLanguage(Localized.Languages.English)
```

Grabbing a localized definition at runtime can be done with the following function (from the test example above)
```csharp
Localized.Test.Localize();
```

For more complex variations of localization, localizations can support string formatting. The following example 
```csharp
namespace Utilities.Localization
{
    public partial class Localized
    {
        public enum Animals
        {
            Dog,
        }
        
        public enum Questions
        {
            WhereIsMy_X
        }
    }
}
```

![Capture](https://user-images.githubusercontent.com/108739402/211247105-7086d622-5e32-4229-a3cf-fdaf1f7efba8.PNG)

would result in "Where is my Dog"
```csharp
Localized.Instance.SetLanguage(Localized.Languages.English)
Localized.Questions.WhereIsMy_X.Localize(Localized.Animals.Dog);
```
and "¿Dónde está mi Perro?"
```csharp
Localized.Instance.SetLanguage(Localized.Languages.Spanish)
Localized.Questions.WhereIsMy_X.Localize(Localized.Animals.Dog);
```
