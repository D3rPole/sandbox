using System;

[CodeGenerator( CodeGeneratorFlags.WrapPropertySet | CodeGeneratorFlags.Instance, "OnWireOutputWrapSet" )]
[System.AttributeUsage(System.AttributeTargets.Property)]
public class WireOutputAttribute : System.Attribute
{
	private string name;

	public WireOutputAttribute( string name) { this.name = name; }
}
