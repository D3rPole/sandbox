using System;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class WireGate : System.Attribute
{
	private string name;

	public WireGate( string name) { this.name = name; }
}
