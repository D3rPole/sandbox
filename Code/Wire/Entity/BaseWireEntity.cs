using Sandbox;

public class WireConnection
{
	public BaseWireEntity TargetComponent;
	public PropertyDescription TargetField;
	public PropertyDescription OriginField;
}

public abstract class BaseWireEntity : Component, IWireEntity
{
	[Property]
	public List<WireConnection> wireConnections = new();
	[Property]
	public Dictionary<(Type type, string name), PropertyDescription> outputProperties
	{
		get
		{
			if ( field is null )
			{
				var properties = TypeLibrary.GetPropertyDescriptions( this, true ).Where( p => p.Attributes.Any( a => a is WireOutputAttribute ) );
				field = properties.ToDictionary(
					p => (p.PropertyType, p.Name),
					p => p
				);
			}
			return field;
		}
		set
		{
			field = value;
		}
	}

	[Property]
	public Dictionary<(Type type, string name), PropertyDescription> inputProperties
	{
		get
		{
			if ( field is null )
			{
				var properties = TypeLibrary.GetPropertyDescriptions( this, true ).Where( p => p.Attributes.Any( a => a is WireInputAttribute ) );
				field = properties.ToDictionary(
					p => (p.PropertyType, p.Name),
					p => p
				);
			}
			return field;
		}
		set
		{
			field = value;
		}
	}
	[Property]
	public abstract string Name { get; }

	public void OnWireOutputWrapSet<T>( WrappedPropertySet<T> p )
	{
		p.Setter( p.Value );

		var property = outputProperties[(typeof( T ), p.PropertyName)];
		var connections = wireConnections.FindAll( c => c.OriginField == property );
		foreach ( var connection in connections )
		{
			connection.TargetField.SetValue( connection.TargetComponent, p.Value );
		}

		Updated?.Invoke();
	}

	public void OnWireInputWrapSet<T>( WrappedPropertySet<T> p )
	{
		OnInputChanged( p.PropertyName );
		p.Setter( p.Value );
		Updated?.Invoke();
	}

	public event Action Updated;

	protected abstract void OnInputChanged( string fieldName );

	public Dictionary<string, object> GetInputs()
	{
		var properties = TypeLibrary.GetPropertyDescriptions( this, true ).Where( p => p.Attributes.Any( a => a is WireInputAttribute ) );
		return properties.ToDictionary(
			p => p.Name,
			p => p.GetValue( this )
		);
	}

	public Dictionary<string, object> GetOutputs()
	{
		var properties = TypeLibrary.GetPropertyDescriptions( this, true ).Where( p => p.Attributes.Any( a => a is WireOutputAttribute ) );
		return properties.ToDictionary(
			p => p.Name,
			p => p.GetValue( this )
		);
	}
}
