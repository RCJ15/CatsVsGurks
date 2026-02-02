using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class UnitDefinitionAttribute : Attribute
{
    private Team _team;
    public Team Team => _team;

    public UnitDefinitionAttribute(Team team)
    {
        _team = team;
    }
}
