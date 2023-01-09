using System;

public class DescriptionAttribute : Attribute
{
    private string _description;
    public string Description
    {
        get => _description;
    }

    public DescriptionAttribute(string description)
    {
        _description = description;
    }
}
