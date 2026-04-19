using Sandbox;

public abstract class BaseWireEntity:  Component, IWireEntity
{
	public abstract string Name { get; }

	public void OnWireInputWrapSet<T>( WrappedPropertySet<T> p )
	{
		OnInputChanged( p.PropertyName );
		p.Setter( p.Value );
		Updated?.Invoke();
	}

	public event Action Updated;

	protected abstract void OnInputChanged(string fieldName);

	public Dictionary<string, object> GetInputs()
	{
		var properties = TypeLibrary.GetPropertyDescriptions(this, true).Where(p => p.Attributes.Any(a => a is WireInputAttribute));
		return properties.ToDictionary(
			p => p.Name,
			p => p.GetValue(this)
		);
	}

	public Dictionary<string, object> GetOutputs()
	{
		var properties = TypeLibrary.GetPropertyDescriptions(this, true).Where(p => p.Attributes.Any(a => a is WireOutput));
		return properties.ToDictionary(
			p => p.Name,
			p => p.GetValue(this)
		);
	}
}
