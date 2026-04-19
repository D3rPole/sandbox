using System;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class WireOutput : System.Attribute
{
	private string name;

	public WireOutput( string name) { this.name = name; }
}
