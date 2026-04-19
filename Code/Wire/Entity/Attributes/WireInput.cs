using System;

[CodeGenerator( CodeGeneratorFlags.WrapPropertySet | CodeGeneratorFlags.Instance, "OnWireInputWrapSet" )]
[AttributeUsage( AttributeTargets.Property)]
public class WireInputAttribute : Attribute
{
	private string name;

	public WireInputAttribute( string name) { this.name = name; }
}
