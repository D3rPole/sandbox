
using Sandbox.UI;
using Sandbox.Wire.UI.WireTool;

[Title( "Wire" )]
[Icon( "🔋" )]
[ClassName( "wiretool" )]
[Group( "Wire" )]
public class WireTool : ToolMode
{
	public override string Description => "Tool to wire entities";
	public override string PrimaryAction => "Wire component";
	public override string SecondaryAction => step == 0 ? "Disconnect all wire connections" : step == 1 ? "Disconnect wire connection" : null;
	public override string ReloadAction => inputEntity is null && outputEntity is null ? null : "Abort wiring";
	public override bool AbsorbMouseInput => propertySelection?.Entity is not null && (inputEntity is not null && inputPropertyDescription is null || outputEntity is not null && outputPropertyDescription is null);
	public override bool DisallowWeaponSwitching => propertySelection?.Entity is not null && (inputEntity is not null && inputPropertyDescription is null || outputEntity is not null && outputPropertyDescription is null);
	private GameObject gui;
	private PropertySelection propertySelection => (gui?.Components.FirstOrDefault( c => c is PropertySelection ) as PropertySelection);

	private BaseWireEntity inputEntity;
	private PropertyDescription inputPropertyDescription;

	private BaseWireEntity outputEntity;
	private PropertyDescription outputPropertyDescription;
	private int step;

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		if ( !select.IsValid() ) return;

		var wireEntity = CheckForWireComponent( select );

		if ( wireEntity is null )
		{
			if ( gui is not null )
			{
				propertySelection.Entity = null;
			}
		}
		else
		{
			if ( gui is null )
			{
				gui = new GameObject( true, "Wire" );
				gui.Flags = GameObjectFlags.NotSaved | GameObjectFlags.NotNetworked;
				var screenPanel = gui.Components.Create<ScreenPanel>();
				var propertySelection = gui.Components.Create<PropertySelection>();
				Scene.Root.Children.Add( gui );
			}
			if(propertySelection.Entity != wireEntity )
			{
				propertySelection.SelectedProperty = -1;
				propertySelection.Entity = wireEntity;
			}
			
			if ( inputEntity is null || inputPropertyDescription is null )
			{
				propertySelection?.ShowOutputs = false;
			}
			else
			{
				propertySelection.ShowOutputs = true;
			}
		}

		if ( Input.Pressed( "attack1" ) )
		{
			if ( wireEntity is null )
				return;

			switch ( step )
			{
				case 0:
					inputEntity = wireEntity;
					propertySelection.SelectedProperty = 0;
					this.ShootEffects( select );
					step = 1;
					return;
				case 1:
					{
						var property = propertySelection.GetSelection();
						if ( property is null )
							return;
						inputPropertyDescription = property?.Item2;
						propertySelection.SelectedProperty = -1;
						step = 2;
						this.ShootEffects( select );
						return;
					}
				case 2:
					if(wireEntity == inputEntity)
					{
						Notices.AddNotice( "warning", "#e55", $"Unable to wire an entity to itself!".Trim(), 5 );
						return;
					}
					outputEntity = wireEntity;
					propertySelection.SelectedProperty = 0;
					this.ShootEffects( select );
					step = 3;
					return;
				case 3:
					{
						var property = propertySelection.GetSelection();
						if ( property is null )
							return;
						outputPropertyDescription = property?.Item2;
						outputEntity.WireOutConnections.Add(new WireConnection()
						{
							TargetComponent = inputEntity,
							TargetField = inputPropertyDescription,
							OriginField = outputPropertyDescription
						} );
						inputEntity.WireInConnections.Add(new WireConnection()
						{
							TargetComponent = outputEntity,
							TargetField = outputPropertyDescription,
							OriginField = inputPropertyDescription
						} );
						propertySelection.SelectedProperty = -1;
						// Reset before finishing
						ShootEffects( select );
						Reset();
						return;
					}
			}

		}
		else if ( Input.Pressed( "attack2" ) )
		{
			if(wireEntity is null || propertySelection is null)
				return;
			if(step > 1)
			{
				Notices.AddNotice( "warning", "#e55", $"Unable to unwire an entitiy in the middle of a wiring process!".Trim(), 5 );
				return;
			}

			if(step == 0 )
			{
				int removed = wireEntity.DisconnectAllWireConnections();
				if(removed > 0 )
				{
					Notices.AddNotice( "cached", "#3273eb", $"Disconnected all wire connections from {wireEntity.Name}.", 5 );
					ShootEffects( select );
					Reset();
					return;
				}
				Notices.AddNotice( "warning", "#ea5", $"No wire connections to disconnect.", 5 );
			}
			else if(step == 1 )
			{
				var property = propertySelection.GetSelection();
				if(property is null)
					return;
				var wireConnection = property?.Item1.WireInConnections.FirstOrDefault(c => c.OriginField.Name == property?.Item2.Name && c.OriginField.PropertyType == property?.Item2.PropertyType);
				if(wireConnection is null )
				{
					Notices.AddNotice( "warning", "#e55", $"Could not disconnect wire! (Couldn't find wire connection)", 5 );
					return;
				}
				property?.Item1.DisconnectWireConnection(wireConnection);
				Notices.AddNotice( "cached", "#3273eb", $"Input {property?.Item2.Name} disconnected.", 5 );
				ShootEffects( select );
				Reset();
			}
		}
		else if ( Input.Pressed( "reload" ) )
		{
			// Reset entire process
			if ( inputEntity is null && outputEntity is null )
				return;
			Reset();
			Toolgun.SpinCoil();
		}
		else if ( Input.MouseWheel.y != 0 )
		{
			if ( DisallowWeaponSwitching )
			{
				propertySelection?.MoveSlot( -(int)Input.MouseWheel.y );
			}
		}
	}

	private void Reset()
	{
		inputEntity = null;
		inputPropertyDescription = null;
		outputEntity = null;
		outputPropertyDescription = null;
		step = 0;
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		Reset();
	}

	private BaseWireEntity? CheckForWireComponent( SelectionPoint traceSelect )
	{
		if ( traceSelect.GameObject is null )
			return null;

		BaseWireEntity? baseWireEntity = traceSelect.GameObject.Components.FirstOrDefault( c => c is BaseWireEntity ) as BaseWireEntity;

		if ( baseWireEntity is null )
			return null;

		return baseWireEntity;
	}
}
