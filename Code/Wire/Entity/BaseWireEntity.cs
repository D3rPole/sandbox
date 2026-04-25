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
	public List<WireConnection> WireOutConnections = new();
	[Property]
	public List<WireConnection> WireInConnections = new();
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

	private List<(string PropertyName, string PropertyType)> dirty = new();

	protected override void OnFixedUpdate()
	{
		dirty.Clear();
		base.OnFixedUpdate();
	}

	protected override void OnDestroy()
	{
		DisconnectAllWireConnections();
		base.OnDestroy();
	}

	public void OnWireOutputWrapSet<T>( WrappedPropertySet<T> p )
	{
		p.Setter( p.Value );

		var property = outputProperties[(typeof( T ), p.PropertyName)];
		var connections = WireOutConnections.FindAll( c => c.OriginField == property );
		foreach ( var connection in connections )
		{
			connection.TargetField.SetValue( connection.TargetComponent, p.Value );
		}

		Updated?.Invoke();
	}

	public int DisconnectAllWireConnections()
	{
		int removed = 0;
		foreach ( var connection in WireOutConnections )
		{
			removed += connection.TargetComponent.WireInConnections.RemoveAll( c => c.TargetComponent == this );
		}
		foreach ( var connection in WireInConnections )
		{
			removed += connection.TargetComponent.WireOutConnections.RemoveAll( c => c.TargetComponent == this );
		}
		if ( WireInConnections.Count > 0 )
		{
			removed += WireInConnections.Count;
			WireInConnections.Clear();
		}
		if ( WireOutConnections.Count > 0 )
		{
			removed += WireOutConnections.Count;
			WireOutConnections.Clear();
		}
		return removed;
	}

	/// <summary>
	/// Removes entity connection including out/ingoing from other entities
	/// </summary>
	/// <param name="connection"></param>
	public void DisconnectWireConnection( WireConnection connection )
	{
		connection.TargetComponent.WireInConnections.RemoveAll( c =>
			c.TargetComponent == this &&
			c.TargetField.Name == connection.OriginField.Name &&
			c.TargetField.PropertyType == connection.OriginField.PropertyType &&
			c.OriginField.Name == connection.TargetField.Name &&
			c.OriginField.PropertyType == connection.TargetField.PropertyType );

		connection.TargetComponent.WireOutConnections.RemoveAll( c =>
			c.TargetComponent == this &&
			c.TargetField.Name == connection.OriginField.Name &&
			c.TargetField.PropertyType == connection.OriginField.PropertyType &&
			c.OriginField.Name == connection.TargetField.Name &&
			c.OriginField.PropertyType == connection.TargetField.PropertyType );

		WireOutConnections.Remove( connection );
		WireInConnections.Remove( connection );
	}

	public void OnWireInputWrapSet<T>( WrappedPropertySet<T> p )
	{
		var name = p.PropertyName;
		var type = p.TypeName;
		if ( dirty.Exists( t => t.PropertyName == name && t.PropertyType == type ) )
			return; // already set this tick, avoid multiple inputs / infinite loops
		dirty.Add( (name, type) );
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
