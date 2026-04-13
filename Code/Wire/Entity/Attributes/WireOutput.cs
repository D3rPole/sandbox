using System;

[System.AttributeUsage( System.AttributeTargets.Property)]
public class WireInput : System.Attribute
{
	private string name;

	public WireInput(string name) { this.name = name; }
}
