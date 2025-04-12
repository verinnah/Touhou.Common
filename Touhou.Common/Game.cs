using System.Diagnostics.CodeAnalysis;

namespace Touhou.Common;

/// <summary>
/// Enumeration containing the official Touhou games.
/// </summary>
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Adding a zero value makes no sense in this enum")]
public enum Game
{
	/// <summary>
	/// Highly Responsive to Prayers.
	/// </summary>
	HRtP = 1,
	/// <summary>
	/// Story of Eastern Wonderland.
	/// </summary>
	SoEW = 2,
	/// <summary>
	/// Phantasmagoria of Dimensional Dream.
	/// </summary>
	PoDD = 3,
	/// <summary>
	/// Lotus Land Story.
	/// </summary>
	LLS = 4,
	/// <summary>
	/// Mystic Square.
	/// </summary>
	MS = 5,
	/// <summary>
	/// The Embodiment of Scarlet Devil.
	/// </summary>
	EoSD = 6,
	/// <summary>
	/// Perfect Cherry Blossom.
	/// </summary>
	PCB = 7,
	/// <summary>
	/// Immaterial and Missing Power.
	/// </summary>
	IaMP = 75,
	/// <summary>
	/// Impersishable Night.
	/// </summary>
	IN = 8,
	/// <summary>
	/// Phantasmagoria of Flower View.
	/// </summary>
	PoFV = 9,
	/// <summary>
	/// Shoot the Bullet.
	/// </summary>
	StB = 95,
	/// <summary>
	/// Mountain of Faith.
	/// </summary>
	MoF = 10,
	/// <summary>
	/// Scarlet Weather Rhapsody.
	/// </summary>
	SWR = 105,
	/// <summary>
	/// Subterranean Animism.
	/// </summary>
	SA = 11,
	/// <summary>
	/// Undefined Fantastic Object.
	/// </summary>
	UFO = 12,
	/// <summary>
	/// Touhou Hisotensoku.
	/// </summary>
	Hiso = 123,
	/// <summary>
	/// Double Spoiler.
	/// </summary>
	DS = 125,
	/// <summary>
	/// Great Fairy Wars.
	/// </summary>
	GFW = 128,
	/// <summary>
	/// Ten Desires.
	/// </summary>
	TD = 13,
	/// <summary>
	/// Hopeless Masquerade.
	/// </summary>
	HM = 135,
	/// <summary>
	/// Double Dealing Character.
	/// </summary>
	DDC = 14,
	/// <summary>
	/// Impossible Spell Card.
	/// </summary>
	ISC = 143,
	/// <summary>
	/// Urban Legend in Limbo.
	/// </summary>
	ULiL = 145,
	/// <summary>
	/// Legacy of Lunatic Kingdom.
	/// </summary>
	LoLK = 15,
	/// <summary>
	/// Antinomy of Common Flowers.
	/// </summary>
	AoCF = 155,
	/// <summary>
	/// Hidden Star in Four Seasons.
	/// </summary>
	HSiFS = 16,
	/// <summary>
	/// Violet Detector.
	/// </summary>
	VD = 165,
	/// <summary>
	/// Wily Beast and Weakest Creature.
	/// </summary>
	WBaWC = 17,
	/// <summary>
	/// Touhou Gouyoku Ibun.
	/// </summary>
	GI = 175,
	/// <summary>
	/// Unconnected Marketeers.
	/// </summary>
	UM = 18,
	/// <summary>
	/// 100th Black Market.
	/// </summary>
	HBM = 185,
	/// <summary>
	/// Unfinished Dream of All Living Ghost.
	/// </summary>
	UDoALG = 19,
	/// <summary>
	/// Fossilized Wonders
	/// </summary>
	FW = 20
}
