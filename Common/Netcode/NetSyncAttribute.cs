﻿using System;

namespace Macrocosm.Common.Netcode
{
	/// <summary> 
	/// This attribute is used for network syncing of 
	/// <see cref="Drawing.Particles.Particle"> Particle </see>, 
	/// <see cref="Content.Rockets.Rocket"> Rocket </see>,
	/// <see cref="Content.Rockets.LaunchPads.LaunchPad"> LaunchPad </see>
	/// and <b>(TO FIX!!!, currently not working)</b> ModNPC and ModProjectile 
	/// <b>fields</b> (not auto-properties) marked with this attribute.
	/// For ModNPC and ModProjectile, this should be used only after filling up the entirety of the already existing <c>ai[]</c> array.
	/// The fields are synced whenever:  <list type="bullet">
	///		<item> <c><see cref="Drawing.Particles.Particle.NetSync(int)"> Particle.NetSync() </see></c> is called </item>
	///		<item> <c><see cref="Content.Rockets.Rocket.NetSync(int)"> Rocket.NetSync() </see></c> is called </item>
	///		<item> <c><see cref="Content.Rockets.LaunchPads.LaunchPad.NetSync(int)"> LaunchPad.NetSync() </see></c> is called </item>
	///		<item> <c>NPC.netUpdate</c> or <c>Projectile.netUpdate</c> are set to true, or the corresponding NetMessage is sent </item>
	/// </list> </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NetSyncAttribute : Attribute { }
}
