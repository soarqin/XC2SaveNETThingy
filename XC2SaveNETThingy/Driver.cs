using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Driver : IXC2SaveObject
    {
        public static int SIZE = 0x5A0;
        public const int DRIVER_ARTS_COUNT = 526;

        public static readonly Dictionary<UInt16, string> IDNAMES = new Dictionary<UInt16, string>()
        {
            { 0, "(none)" },
            { 1, "Rex" },
            { 2, "Nia" },
            { 3, "Zeke" },
            { 4, "Tora" },
            { 5, "Vandham" },
            { 6, "Mòrag" },
            { 7, "Jin (Ch 1 Guest)" },
            { 8, "Malos (Ch 1 Guest)" },
            { 9, "Jin (Ch 8 Guest)" },
            { 10, "Unknown1" },
            { 11, "Unknown2" },
            { 12, "Unknown3" },
            { 13, "Unknown4" },
            { 14, "Unknown5" },
            { 15, "Unknown6" },
            { 16, "Unknown7" }
        };

        public static readonly Dictionary<int, string> STATUS = new Dictionary<int, string>()
        {
            { 0, "Not Active" },
            { 1, "In Party" },
            { 2, "In Mercenary Team" }
        };

        public static readonly Dictionary<int, string> SETBLADE = new Dictionary<int, string>()
        {
            { -1, "(none)" },
            { 0, "1st Blade" },
            { 1, "2nd Blade" },
            { 2, "3rd Blade" },
            { 3, "Unknown (Mythra+Rex only?)" }
        };

        public static readonly Dictionary<int, string> DRIVER_ARTS = new Dictionary<int, string>()
        {
            { 0, "(none)" },
            { 1, "Normal Attack 1 [Aegis Sword] (Rex)" },
            { 2, "Normal Attack 2 [Aegis Sword] (Rex)" },
            { 3, "Normal Attack 3 [Aegis Sword] (Rex)" },
            { 4, "Sword Bash [Aegis Sword] (Rex)" },
            { 5, "Double Spinning Edge [Aegis Sword] (Rex)" },
            { 6, "Anchor Shot [Aegis Sword] (Rex)" },
            { 7, "Rolling Smash [Aegis Sword] (Rex)" },
            { 8, "Normal Attack 1 [Catalyst Scimitar] (Rex)" },
            { 9, "Normal Attack 2 [Catalyst Scimitar] (Rex)" },
            { 10, "Normal Attack 3 [Catalyst Scimitar] (Rex)" },
            { 11, "Saber Slash [Catalyst Scimitar] (Rex)" },
            { 12, "Redemption [Catalyst Scimitar] (Rex)" },
            { 13, "Hydro Blast [Catalyst Scimitar] (Rex)" },
            { 14, "Water Flower [Catalyst Scimitar] (Rex)" },
            { 15, "Normal Attack 1 [Twin Rings] (Rex)" },
            { 16, "Normal Attack 2 [Twin Rings] (Rex)" },
            { 17, "Normal Attack 3 [Twin Rings] (Rex)" },
            { 18, "Normal Attack 1 [Twin Rings] (Nia)" },
            { 19, "Normal Attack 2 [Twin Rings] (Nia)" },
            { 20, "Normal Attack 3 [Twin Rings] (Nia)" },
            { 21, "Normal Attack 1 [Twin Rings] (Mòrag)" },
            { 22, "Normal Attack 2 [Twin Rings] (Mòrag)" },
            { 23, "Normal Attack 3 [Twin Rings] (Mòrag)" },
            { 24, "Normal Attack 1 [Twin Rings] (Zeke)" },
            { 25, "Normal Attack 2 [Twin Rings] (Zeke)" },
            { 26, "Normal Attack 3 [Twin Rings] (Zeke)" },
            { 27, "Wide Slash [Twin Rings] (Rex)" },
            { 28, "Strong Saucer [Twin Rings] (Rex)" },
            { 29, "Eightfold Edge [Twin Rings] (Rex)" },
            { 30, "Spin of Bravery [Twin Rings] (Rex)" },
            { 31, "Butterfly Blade [Twin Rings] (Nia)" },
            { 32, "Jaguar Slash [Twin Rings] (Nia)" },
            { 33, "Gemini Loop [Twin Rings] (Nia)" },
            { 34, "Healing Halo [Twin Rings] (Nia)" },
            { 35, "Halo Strike [Twin Rings] (Mòrag)" },
            { 36, "Inferno Wheel [Twin Rings] (Mòrag)" },
            { 37, "Twin Moonblade [Twin Rings] (Mòrag)" },
            { 38, "Revolution Flash [Twin Rings] (Mòrag)" },
            { 39, "Rapid Twist [Twin Rings] (Zeke)" },
            { 40, "Clear Divide [Twin Rings] (Zeke)" },
            { 41, "Righteous Guillotine [Twin Rings] (Zeke)" },
            { 42, "Shredder Spiral [Twin Rings] (Zeke)" },
            { 43, "Normal Attack 1 [Drill Shield] (Tora)" },
            { 44, "Normal Attack 2 [Drill Shield] (Tora)" },
            { 45, "Normal Attack 3 [Drill Shield] (Tora)" },
            { 46, "Steady Drill [Drill Shield] (Tora)" },
            { 47, "Big Boost [Drill Shield] (Tora)" },
            { 48, "Spinning Cutter [Drill Shield] (Tora)" },
            { 49, "Rigid Shield [Drill Shield] (Tora)" },
            { 50, "Normal Attack 1 [Mech Arms] (Tora)" },
            { 51, "Normal Attack 2 [Mech Arms] (Tora)" },
            { 52, "Normal Attack 3 [Mech Arms] (Tora)" },
            { 53, "Pow-Pow Cannon [Mech Arms] (Tora)" },
            { 54, "Dented Shield [Mech Arms] (Tora)" },
            { 55, "Scattershot [Mech Arms] (Tora)" },
            { 56, "Booming Buster [Mech Arms] (Tora)" },
            { 57, "Normal Attack 1 [Variable Saber] (Tora)" },
            { 58, "Normal Attack 2 [Variable Saber] (Tora)" },
            { 59, "Normal Attack 3 [Variable Saber] (Tora)" },
            { 60, "Swooshing Slash [Variable Saber] (Tora)" },
            { 61, "Speedy Sword [Variable Saber] (Tora)" },
            { 62, "Boom-Boom Laser [Variable Saber] (Tora)" },
            { 63, "Steady Beam [Variable Saber] (Tora)" },
            { 64, "Normal Attack 1 [Whipswords] (Rex)" },
            { 65, "Normal Attack 2 [Whipswords] (Rex)" },
            { 66, "Normal Attack 3 [Whipswords] (Rex)" },
            { 67, "Normal Attack 1 [Whipswords] (Mòrag)" },
            { 68, "Normal Attack 2 [Whipswords] (Mòrag)" },
            { 69, "Normal Attack 3 [Whipswords] (Mòrag)" },
            { 70, "Heat Rush [Whipswords] (Rex)" },
            { 71, "Storm Spin [Whipswords] (Rex)" },
            { 72, "Blazing Whip [Whipswords] (Rex)" },
            { 73, "Phantom Flame [Whipswords] (Rex)" },
            { 74, "Azure II: Hellfire [Whipswords] (Mòrag)" },
            { 75, "Azure II: Radiance [Whipswords] (Mòrag)" },
            { 76, "Azure II: Blaze [Whipswords] (Mòrag)" },
            { 77, "Azure II: Judgement [Whipswords] (Mòrag)" },
            { 78, "Normal Attack 1 [Big Bang Edge] (Rex)" },
            { 79, "Normal Attack 2 [Big Bang Edge] (Rex)" },
            { 80, "Normal Attack 3 [Big Bang Edge] (Rex)" },
            { 81, "Normal Attack 1 [Big Bang Edge] (Zeke)" },
            { 82, "Normal Attack 2 [Big Bang Edge] (Zeke)" },
            { 83, "Normal Attack 3 [Big Bang Edge] (Zeke)" },
            { 84, "Upper Edge [Big Bang Edge] (Rex)" },
            { 85, "Big Impact [Big Bang Edge] (Rex)" },
            { 86, "Electro Buster [Big Bang Edge] (Rex)" },
            { 87, "Voltiac Slash [Big Bang Edge] (Rex)" },
            { 88, "Stratospheric Tunder [Big Bang Edge] (Zeke)" },
            { 89, "Dynamic Spark Sword [Big Bang Edge] (Zeke)" },
            { 90, "Overload Thunder Beam [Big Bang Edge] (Zeke)" },
            { 91, "ULFS Max [Big Bang Edge] (Zeke)" },
            { 92, "Normal Attack 1 [Dual Scythes] (Rex)" },
            { 93, "Normal Attack 2 [Dual Scythes] (Rex)" },
            { 94, "Normal Attack 3 [Dual Scythes] (Rex)" },
            { 95, "Normal Attack 1 [Dual Scythes] (Vandham)" },
            { 96, "Normal Attack 2 [Dual Scythes] (Vandham)" },
            { 97, "Normal Attack 3 [Dual Scythes] (Vandham)" },
            { 98, "Cyclone Smash [Dual Scythes] (Rex)" },
            { 99, "Wild Scythe [Dual Scythes] (Rex)" },
            { 100, "Wing Smash [Dual Scythes] (Rex)" },
            { 101, "Vortex Edge [Dual Scythes] (Rex)" },
            { 102, "Muscle Slash [Dual Scythes] (Vandham)" },
            { 103, "Fast Twitch [Dual Scythes] (Vandham)" },
            { 104, "Muscle Uppercut [Dual Scythes] (Vandham)" },
            { 105, "Mega Muscle [Dual Scythes] (Vandham)" },
            { 106, "Normal Attack 1 [Greataxe] (Rex)" },
            { 107, "Normal Attack 2 [Greataxe] (Rex)" },
            { 108, "Normal Attack 3 [Greataxe] (Rex)" },
            { 109, "Normal Attack 1 [Greataxe] (Nia)" },
            { 110, "Normal Attack 2 [Greataxe] (Nia)" },
            { 111, "Normal ATtack 3 [Greataxe] (Nia)" },
            { 112, "Normal Attack 1 [Greataxe] (Mòrag)" },
            { 113, "Normal Attack 2 [Greataxe] (Mòrag)" },
            { 114, "Normal Attack 3 [Greataxe] (Mòrag)" },
            { 115, "Normal Attack 1 [Greataxe] (Zeke)" },
            { 116, "Normal Attack 2 [Greataxe] (Zeke)" },
            { 117, "Normal Attack 3 [Greataxe] (Zeke)" },
            { 118, "Strong Smash [Greataxe] (Rex)" },
            { 119, "Axe Twist [Greataxe] (Rex)" },
            { 120, "Power Swipe [Greataxe] (Rex)" },
            { 121, "Bold Tackle [Greataxe] (Rex)" },
            { 122, "Brutal Blade [Greataxe] (Nia)" },
            { 123, "Rhino Assault [Greataxe] (Nia)" },
            { 124, "Bear Smash [Greataxe] (Nia)" },
            { 125, "Shadow Slice [Greataxe] (Nia)" },
            { 126, "Rock Cleaver [Greataxe] (Mòrag)" },
            { 127, "Admiral Waltz [Greataxe] (Mòrag)" },
            { 128, "Gale Blade [Greataxe] (Mòrag)" },
            { 129, "Rising Dragon [Greataxe] (Mòrag)" },
            { 130, "Spinning Elbow [Greataxe] (Zeke)" },
            { 131, "Triumphal Axe [Greataxe] (Zeke)" },
            { 132, "Raging Charge [Greataxe] (Zeke)" },
            { 133, "Berserker Slash [Greataxe] (Zeke)" },
            { 134, "Normal Attack 1 [Megalance] (Rex)" },
            { 135, "Normal Attack 2 [Megalance] (Rex)" },
            { 136, "Normal Attack 3 [Megalance] (Rex)" },
            { 137, "Normal Attack 1 [Megalance] (Nia)" },
            { 138, "Normal Attack 2 [Megalance] (Nia)" },
            { 139, "Normal Attack 3 [Megalance] (Nia)" },
            { 140, "Normal Attack 1 [Megalance] (Mòrag)" },
            { 141, "Normal Attack 2 [Megalance] (Mòrag)" },
            { 142, "Normal Attack 3 [Megalance] (Mòrag)" },
            { 143, "Normal Attack 1 [Megalance] (Zeke)" },
            { 144, "Normal Attack 2 [Megalance] (Zeke)" },
            { 145, "Normal Attack 3 [Megalance] (Zeke)" },
            { 146, "Strong Horn [Megalance] (Rex)" },
            { 147, "Power Spike [Megalance] (Rex)" },
            { 148, "Heavy Hitter [Megalance] (Rex)" },
            { 149, "Feral Spin [Megalance] (Rex)" },
            { 150, "Vulture Peck [Megalance] (Nia)" },
            { 151, "Gyro Break [Megalance] (Nia)" },
            { 152, "Raptor Raid [Megalance] (Nia)" },
            { 153, "Discus Glaive [Megalance] (Nia)" },
            { 154, "Crown Splitter [Megalance] (Mòrag)" },
            { 155, "Spiral Uppercut [Megalance] (Mòrag)" },
            { 156, "Twin Dragons [Megalance] (Mòrag)" },
            { 157, "Windmill [Megalance] (Mòrag)" },
            { 158, "Brutal Swing [Megalance] (Zeke)" },
            { 159, "Meteor Lance [Megalance] (Zeke)" },
            { 160, "Powerhouse Smash [Megalance] (Zeke)" },
            { 161, "Supersonic Spin [Megalance] (Zeke)" },
            { 162, "Normal Attack 1 [Ether Cannon] (Rex)" },
            { 163, "Normal Attack 2 [Ether Cannon] (Rex)" },
            { 164, "Normal Attack 3 [Ether Cannon] (Rex)" },
            { 165, "Normal Attack 1 [Ether Cannon] (Nia)" },
            { 166, "Normal Attack 2 [Ether Cannon] (Nia)" },
            { 167, "Normal Attack 3 [Ether Cannon] (Nia)" },
            { 168, "Normal Attack 1 [Ether Cannon] (Mòrag)" },
            { 169, "Normal Attack 2 [Ether Cannon] (Mòrag)" },
            { 170, "Normal Attack 3 [Ether Cannon] (Mòrag)" },
            { 171, "Normal Attack 1 [Ether Cannon] (Zeke)" },
            { 172, "Normal Attack 2 [Ether Cannon] (Zeke)" },
            { 173, "Normal Attack 3 [Ether Cannon] (Zeke)" },
            { 174, "Daring Shot [Ether Cannon] (Rex)" },
            { 175, "Armor Crusher [Ether Cannon] (Rex)" },
            { 176, "Grenade Launcher [Ether Cannon] (Rex)" },
            { 177, "Wild Volley [Ether Cannon] (Rex)" },
            { 178, "Breach Shot [Ether Cannon] (Nia)" },
            { 179, "Leopard Blow [Ether Cannon] (Nia)" },
            { 180, "Cattle Driver [Ether Cannon] (Nia)" },
            { 181, "Pulse Fire [Ether Cannon] (Nia)" },
            { 182, "Ogre Slam [Ether Cannon] (Mòrag)" },
            { 183, "Woodcutter [Ether Cannon] (Mòrag)" },
            { 184, "Touch-Me-Not [Ether Cannon] (Mòrag)" },
            { 185, "Mist Buster [Ether Cannon] (Mòrag)" },
            { 186, "Burst Gunner [Ether Cannon] (Zeke)" },
            { 187, "Haywire Chainshot [Ether Cannon] (Zeke)" },
            { 188, "Blowback Cannon [Ether Cannon] (Zeke)" },
            { 189, "Laserpoint Bazooka [Ether Cannon] (Zeke)" },
            { 190, "Normal Attack 1 [Shield Hammer] (Rex)" },
            { 191, "Normal Attack 2 [Shield Hammer] (Rex)" },
            { 192, "Normal Attack 3 [Shield Hammer] (Rex)" },
            { 193, "Normal Attack 1 [Shield Hammer] (Nia)" },
            { 194, "Normal Attack 2 [Shield Hammer] (Nia)" },
            { 195, "Normal Attack 3 [Shield Hammer] (Nia)" },
            { 196, "Normal Attack 1 [Shield Hammer] (Mòrag)" },
            { 197, "Normal Attack 2 [Shield Hammer] (Mòrag)" },
            { 198, "Normal Attack 3 [Shield Hammer] (Mòrag)" },
            { 199, "Normal Attack 1 [Shield Hammer] (Zeke)" },
            { 200, "Normal Attack 2 [Shield Hammer] (Zeke)" },
            { 201, "Normal Attack 3 [Shield Hammer] (Zeke)" },
            { 202, "Iron Wall [Shield Hammer] (Rex)" },
            { 203, "Diving Swing [Shield Hammer] (Rex)" },
            { 204, "Power Hammer [Shield Hammer] (Rex)" },
            { 205, "Mighty Beat [Shield Hammer] (Rex)" },
            { 206, "Rumbling Smash [Shield Hammer] (Nia)" },
            { 207, "Elephant Press [Shield Hammer] (Nia)" },
            { 208, "Buckler Slam [Shield Hammer] (Nia)" },
            { 209, "Hammerhead [Shield Hammer] (Nia)" },
            { 210, "Vajra Blow [Shield Hammer] (Mòrag)" },
            { 211, "Battle Top [Shield Hammer] (Mòrag)" },
            { 212, "Mountain Crusher [Shield Hammer] (Mòrag)" },
            { 213, "Decimator [Shield Hammer] (Mòrag)" },
            { 214, "Brawl Swing [Shield Hammer] (Zeke)" },
            { 215, "Behemoth Uppercut [Shield Hammer] (Zeke)" },
            { 216, "Detonation Blow [Shield Hammer] (Zeke)" },
            { 217, "Impregnable Shield [Shield Hammer] (Zeke)" },
            { 218, "Normal Attack 1 [Chroma Katana] (Rex)" },
            { 219, "Normal Attack 2 [Chroma Katana] (Rex)" },
            { 220, "Normal Attack 3 [Chroma Katana] (Rex)" },
            { 221, "Normal Attack 1 [Chroma Katana] (Nia)" },
            { 222, "Normal Attack 2 [Chroma Katana] (Nia)" },
            { 223, "Normal Attack 3 [Chroma Katana] (Nia)" },
            { 224, "Normal Attack 1 [Chroma Katana] (Mòrag)" },
            { 225, "Normal Attack 2 [Chroma Katana] (Mòrag)" },
            { 226, "Normal Attack 3 [Chroma Katana] (Mòrag)" },
            { 227, "Normal Attack 1 [Chroma Katana] (Zeke)" },
            { 228, "Normal Attack 2 [Chroma Katana] (Zeke)" },
            { 229, "Normal Attack 3 [Chroma Katana] (Zeke)" },
            { 230, "Sword Uppercut [Chroma Katana] (Rex)" },
            { 231, "Feral Blade [Chroma Katana] (Rex)" },
            { 232, "Breakdown [Chroma Katana] (Rex)" },
            { 233, "Dual Slash [Chroma Katana] (Rex)" },
            { 234, "Deathbound [Chroma Katana] (Nia)" },
            { 235, "Scorpion Tail [Chroma Katana] (Nia)" },
            { 236, "Wolf Fang [Chroma Katana] (Nia)" },
            { 237, "Triple Flash [Chroma Katana] (Nia)" },
            { 238, "Aerial Slash [Chroma Katana] (Mòrag)" },
            { 239, "Eclipse Blade [Chroma Katana] (Mòrag)" },
            { 240, "Overfall [Chroma Katana] (Mòrag)" },
            { 241, "Quadruple Slash [Chroma Katana] (Mòrag)" },
            { 242, "Reverb Blade [Chroma Katana] (Zeke)" },
            { 243, "Psychic Sword [Chroma Katana] (Zeke)" },
            { 244, "Aerial Edge [Chroma Katana] (Zeke)" },
            { 245, "Shogun Slash [Chroma Katana] (Zeke)" },
            { 246, "Normal Attack 1 [Bitball] (Rex)" },
            { 247, "Normal Attack 2 [Bitball] (Rex)" },
            { 248, "Normal Attack 3 [Bitball] (Rex)" },
            { 249, "Normal Attack 1 [Bitball] (Nia)" },
            { 250, "Normal Attack 2 [Bitball] (Nia)" },
            { 251, "Normal Attack 3 [Bitball] (Nia)" },
            { 252, "Normal Attack 1 [Bitball] (Mòrag)" },
            { 253, "Normal Attack 2 [Bitball] (Mòrag)" },
            { 254, "Normal Attack 3 [Bitball] (Mòrag)" },
            { 255, "Normal Attack 1 [Bitball] (Zeke)" },
            { 256, "Normal Attack 2 [Bitball] (Zeke)" },
            { 257, "Normal Attack 3 [Bitball] (Zeke)" },
            { 258, "Touchdown [Bitball] (Rex)" },
            { 259, "Brave Throw [Bitball] (Rex)" },
            { 260, "String Shot [Bitball] (Rex)" },
            { 261, "Mighty Ball [Bitball] (Rex)" },
            { 262, "Dolphin Spin [Bitball] (Nia)" },
            { 263, "Healing Circle [Bitball] (Nia)" },
            { 264, "Falcon Turn [Bitball] (Nia)" },
            { 265, "Acrobatic Bomber [Bitball] (Nia)" },
            { 266, "Dragonbone Shot [Bitball] (Mòrag)" },
            { 267, "Celestial Flash [Bitball] (Mòrag)" },
            { 268, "Shadowrunner [Bitball] (Mòrag)" },
            { 269, "Gliding Swallow [Bitball] (Mòrag)" },
            { 270, "Rumble Pitch [Bitball] (Zeke)" },
            { 271, "Pulverizing Dunk [Bitball] (Zeke)" },
            { 272, "Shell Shot [Bitball] (Zeke)" },
            { 273, "Precipitous Impact [Bitball] (Zeke)" },
            { 274, "Normal Attack 1 [Knuckle Claws] (Rex)" },
            { 275, "Normal Attack 2 [Knuckle Claws] (Rex)" },
            { 276, "Normal Attack 3 [Knuckle Claws] (Rex)" },
            { 277, "Normal Attack 1 [Knuckle Claws] (Nia)" },
            { 278, "Normal Attack 2 [Knuckle Claws] (Nia)" },
            { 279, "Normal Attack 3 [Knuckle Claws] (Nia)" },
            { 280, "Normal Attack 1 [Knuckle Claws] (Mòrag)" },
            { 281, "Normal Attack 2 [Knuckle Claws] (Mòrag)" },
            { 282, "Normal Attack 3 [Knuckle Claws] (Mòrag)" },
            { 283, "Normal Attack 1 [Knuckle Claws] (Zeke)" },
            { 284, "Normal Attack 2 [Knuckle Claws] (Zeke)" },
            { 285, "Normal Attack 3 [Knuckle Claws] (Zeke)" },
            { 286, "Feral Uppercut [Knuckle Claws] (Rex)" },
            { 287, "Grand Smash [Knuckle Claws] (Rex)" },
            { 288, "Bullet Punch [Knuckle Claws] (Rex)" },
            { 289, "Mach Straight Punch [Knuckle Claws] (Rex)" },
            { 290, "Jackal Claw [Knuckle Claws] (Nia)" },
            { 291, "Comet Punch [Knuckle Claws] (Nia)" },
            { 292, "Deadly Twister [Knuckle Claws] (Nia)" },
            { 293, "Raptor Beat [Knuckle Claws] (Nia)" },
            { 294, "Armored Fist [Knuckle Claws] (Mòrag)" },
            { 295, "Takedown [Knuckle Claws] (Mòrag)" },
            { 296, "Sneak Hit [Knuckle Claws] (Mòrag)" },
            { 297, "Peerless Blow [Knuckle Claws] (Mòrag)" },
            { 298, "Hammer Lariat [Knuckle Claws] (Zeke)" },
            { 299, "Fury Dropkick [Knuckle Claws] (Zeke)" },
            { 300, "Dynamite Knuckle [Knuckle Claws] (Zeke)" },
            { 301, "Skysplitter Cross Chop [Knuckle Claws] (Zeke)" },
            { 302, "Normal Attack 1 [Broadsword] (Rex)" },
            { 303, "Normal Attack 2 [Broadsword] (Rex)" },
            { 304, "Normal Attack 3 [Broadsword] (Rex)" },
            { 305, "Sword Bash [Broadsword] (Rex)" },
            { 306, "Double Spinning Edge [Broadsword] (Rex)" },
            { 307, "Rolling Smash [Broadsword] (Rex)" },
            { 308, "Anchor Shot [Broadsword] (Rex)" },
            { 309, "Normal Attack 1 [Nodachi] (Jin Ch1 Guest)" },
            { 310, "Normal Attack 2 [Nodachi] (Jin Ch1 Guest)" },
            { 311, "Normal Attack 3 [Nodachi] (Jin Ch1 Guest)" },
            { 312, "Ultraslash [Nodachi] (Jin Ch1 Guest)" },
            { 313, "Skyward Slash [Nodachi] (Jin Ch1 Guest)" },
            { 314, "Empty Moment [Nodachi] (Jin Ch1 Guest)" },
            { 315, "Stunned Swallow [Nodachi] (Jin Ch1 Guest)" },
            { 316, "Normal Attack 1 [Sword Tonfa] (Malos Ch1 Guest)" },
            { 317, "Normal Attack 2 [Sword Tonfa] (Malos Ch1 Guest)" },
            { 318, "Normal Attack 3 [Sword Tonfa] (Malos Ch1 Guest)" },
            { 319, "Hammer Bash [Sword Tonfa] (Malos Ch1 Guest)" },
            { 320, "Sword Striike [Sword Tonfa] (Malos Ch1 Guest)" },
            { 321, "Spiral Savate [Sword Tonfa] (Malos Ch1 Guest)" },
            { 322, "Chakra Burst [Sword Tonfa] (Malos Ch1 Guest)" },
            { 323, "Normal Attack 1 [Nodachi] (Jin Ch8 Guest)" },
            { 324, "Normal Attack 2 [Nodachi] (Jin Ch8 Guest)" },
            { 325, "Normal Attack 3 [Nodachi] (Jin Ch8 Guest)" },
            { 326, "Ultraslash [Nodachi] (Jin Ch8 Guest)" },
            { 327, "Skyward Slash [Nodachi] (Jin Ch8 Guest)" },
            { 328, "Heavenly Disrupt [Nodachi] (Jin Ch8 Guest)" },
            { 329, "Stunned Swallow [Nodachi] (Jin Ch8 Guest)" },
            { 330, "Normal Attack 1 [Calamity Scythe] (Rex)" },
            { 331, "Normal Attack 2 [Calamity Scythe] (Rex)" },
            { 332, "Normal Attack 3 [Calamity Scythe] (Rex)" },
            { 333, "Normal Attack 1 [Calamity Scythe] (Nia)" },
            { 334, "Normal Attack 2 [Calamity Scythe] (Nia)" },
            { 335, "Normal Attack 3 [Calamity Scythe] (Nia)" },
            { 336, "Normal Attack 1 [Calamity Scythe] (Mòrag)" },
            { 337, "Normal Attack 2 [Calamity Scythe] (Mòrag)" },
            { 338, "Normal Attack 3 [Calamity Scythe] (Mòrag)" },
            { 339, "Normal Attack 1 [Calamity Scythe] (Zeke)" },
            { 340, "Normal Attack 2 [Calamity Scythe] (Zeke)" },
            { 341, "Normal Attack 3 [Calamity Scythe] (Zeke)" },
            { 342, "Revolt Smash [Calamity Scythe] (Rex)" },
            { 343, "Screw Reaper [Calamity Scythe] (Rex)" },
            { 344, "Rising Crescent [Calamity Scythe] (Rex)" },
            { 345, "Withering Assault [Calamity Scythe] (Rex)" },
            { 346, "Brutal Scythe [Calamity Scythe] (Nia)" },
            { 347, "Capra Assault [Calamity Scythe] (Nia)" },
            { 348, "Grizzly Smash [Calamity Scythe] (Nia)" },
            { 349, "Creeping Slicer [Calamity Scythe] (Nia)" },
            { 350, "Tempest Strike [Calamity Scythe] (Mòrag)" },
            { 351, "Sacred Beats's Blessing [Calamity Scythe] (Mòrag)" },
            { 352, "Cyclonic Blade [Calamity Scythe] (Mòrag)" },
            { 353, "Azure Dragon Storm [Calamity Scythe] (Mòrag)" },
            { 354, "Surprise Elbow [Calamity Scythe] (Zeke)" },
            { 355, "Careful Swing [Calamity Scythe] (Zeke)" },
            { 356, "Standing Ovation [Calamity Scythe] (Zeke)" },
            { 357, "Bewilder Slash [Calamity Scythe] (Zeke)" },
            { 358, "Normal Attack 1 [Cobra Bardiche] (Rex)" },
            { 359, "Normal Attack 2 [Cobra Bardiche] (Rex)" },
            { 360, "Normal Attack 3 [Cobra Bardiche] (Rex)" },
            { 361, "Normal Attack 1 [Cobra Bardiche] (Nia)" },
            { 362, "Normal Attack 2 [Cobra Bardiche] (Nia)" },
            { 363, "Normal Attack 3 [Cobra Bardiche] (Nia)" },
            { 364, "Normal Attack 1 [Cobra Bardiche] (Mòrag)" },
            { 365, "Normal Attack 2 [Cobra Bardiche] (Mòrag)" },
            { 366, "Normal Attack 3 [Cobra Bardiche] (Mòrag)" },
            { 367, "Normal Attack 1 [Cobra Bardiche] (Zeke)" },
            { 368, "Normal Attack 2 [Cobra Bardiche] (Zeke)" },
            { 369, "Normal Attack 3 [Cobra Bardiche] (Zeke)" },
            { 370, "Earth Splitter [Cobra Bardiche] (Rex)" },
            { 371, "Powerquake [Cobra Bardiche] (Rex)" },
            { 372, "Final Stone [Cobra Bardiche] (Rex)" },
            { 373, "Dust Up [Cobra Bardiche] (Rex)" },
            { 374, "Condor's Beak [Cobra Bardiche] (Nia)" },
            { 375, "Sand Break [Cobra Bardiche] (Nia)" },
            { 376, "Cobra Raid [Cobra Bardiche] (Nia)" },
            { 377, "Merciless Twister [Cobra Bardiche] (Nia)" },
            { 378, "Blunt Force [Cobra Bardiche] (Mòrag)" },
            { 379, "Heaven's Valor [Cobra Bardiche] (Mòrag)" },
            { 380, "Lunar Salutation [Cobra Bardiche] (Mòrag)" },
            { 381, "Twisted Thistle [Cobra Bardiche] (Mòrag)" },
            { 382, "Grudge Blow [Cobra Bardiche] (Zeke)" },
            { 383, "Heaven's Revenge [Cobra Bardiche] (Zeke)" },
            { 384, "Takedown Smash [Cobra Bardiche] (Zeke)" },
            { 385, "Whirlwind of Passion [Cobra Bardiche] (Zeke)" },
            { 386, "Normal Attack 1 [Infinity Fans] (Rex)" },
            { 387, "Normal Attack 2 [Infinity Fans] (Rex)" },
            { 388, "Normal Attack 3 [Infinity Fans] (Rex)" },
            { 389, "Normal Attack 1 [Infinity Fans] (Nia)" },
            { 390, "Normal Attack 2 [Infinity Fans] (Nia)" },
            { 391, "Normal Attack 3 [Infinity Fans] (Nia)" },
            { 392, "Normal Attack 1 [Infinity Fans] (Mòrag)" },
            { 393, "Normal Attack 2 [Infinity Fans] (Mòrag)" },
            { 394, "Normal Attack 3 [Infinity Fans] (Mòrag)" },
            { 395, "Normal Attack 1 [Infinity Fans] (Zeke)" },
            { 396, "Normal ATtack 2 [Infinity Fans] (Zeke)" },
            { 397, "Normal Attack 3 [Infinity Fans] (Zeke)" },
            { 398, "Opened Fan [Infinity Fans] (Rex)" },
            { 399, "Screw Cutter [Infinity Fans] (Rex)" },
            { 400, "Cross Edge [Infinity Fans] (Rex)" },
            { 401, "Brave Wing [Infinity Fans] (Rex)" },
            { 402, "Swallowtail [Infinity Fans] (Nia)" },
            { 403, "Scarecrow [Infinity Fans] (Nia)" },
            { 404, "Vicious Circle [Infinity Fans] (Nia)" },
            { 405, "Healing Trick [Infinity Fans] (Nia)" },
            { 406, "Albatross Cut [Infinity Fans] (Mòrag)" },
            { 407, "2,000 Wings [Infinity Fans] (Mòrag)" },
            { 408, "Black Wing Dance [Infinity Fans] (Mòrag)" },
            { 409, "Stoking the Fire [Infinity Fans] (Mòrag)" },
            { 410, "Twilight Twist [Infinity Fans] (Zeke)" },
            { 411, "Bold Division [Infinity Fans] (Zeke)" },
            { 412, "Gutsy Cutter [Infinity Fans] (Zeke)" },
            { 413, "Exquisite Spiral [Infinity Fans] (Zeke)" },
            { 414, "Normal Attack 1 [Brilliant Twinblades] (Rex)" },
            { 415, "Normal Attack 2 [Brilliant Twinblades] (Rex)" },
            { 416, "Normal Attack 3 [Brilliant Twinblades] (Rex)" },
            { 417, "Normal Attack 1 [Brilliant Twinblades] (Nia)" },
            { 418, "Normal Attack 2 [Brilliant Twinblades] (Nia)" },
            { 419, "Normal Attack 3 [Brilliant Twinblades] (Nia)" },
            { 420, "Normal Attack 1 [Brilliant Twinblades] (Mòrag)" },
            { 421, "Normal Attack 2 [Brilliant Twinblades] (Mòrag)" },
            { 422, "Normal Attack 3 [Brilliant Twinblades] (Mòrag)" },
            { 423, "Normal Attack 1 [Brilliant Twinblades] (Zeke)" },
            { 424, "Normal Attack 2 [Brilliant Twinblades] (Zeke)" },
            { 425, "Normal Attack 3 [Brilliant Twinblades] (Zeke)" },
            { 426, "Thunderclap [Brilliant Twinblades] (Rex)" },
            { 427, "Haphazard Throw [Brilliant Twinblades] (Rex)" },
            { 428, "Cutting Edge [Brilliant Twinblades] (Rex)" },
            { 429, "Rebel's Dagger [Brilliant Twinblades] (Rex)" },
            { 430, "Papillon Dance [Brilliant Twinblades] (Nia)" },
            { 431, "Cheetah Scratch [Brilliant Twinblades] (Nia)" },
            { 432, "Tricksy Loop [Brilliant Twinblades] (Nia)" },
            { 433, "Lustrous Heal [Brilliant Twinblades] (Nia)" },
            { 434, "Photon Slash [Brilliant Twinblades] (Mòrag)" },
            { 435, "Marigold in Bloom [Brilliant Twinblades] (Mòrag)" },
            { 436, "Flowing Shadow [Brilliant Twinblades] (Mòrag)" },
            { 437, "Mastery of Sky Sparks [Brilliant Twinblades] (Mòrag)" },
            { 438, "Electroshade Twist [Brilliant Twinblades] (Zeke)" },
            { 439, "Bewitching Divide [Brilliant Twinblades] (Zeke)" },
            { 440, "Thunderslam [Brilliant Twinblades] (Zeke)" },
            { 441, "Florid Spiral [Brilliant Twinblades] (Zeke)" },
            { 442, "Normal Attack 1 [Decimation Cannon] (Rex)" },
            { 443, "Normal Attack 2 [Decimation Cannon] (Rex)" },
            { 444, "Normal Attack 3 [Decimation Cannon] (Rex)" },
            { 445, "Normal Attack 1 [Decimation Cannon] (Nia)" },
            { 446, "Normal Attack 2 [Decimation Cannon] (Nia)" },
            { 447, "Normal Attack 3 [Decimation Cannon] (Nia)" },
            { 448, "Normal Attack 1 [Decimation Cannon] (Mòrag)" },
            { 449, "Normal Attack 2 [Decimation Cannon] (Mòrag)" },
            { 450, "Normal Attack 3 [Decimation Cannon] (Mòrag)" },
            { 451, "Normal Attack 1 [Decimation Cannon] (Zeke)" },
            { 452, "Normal Attack 2 [Decimation Cannon] (Zeke)" },
            { 453, "Normal Attack 3 [Decimation Cannon] (Zeke)" },
            { 454, "Critical Trigger [Decimation Cannon] (Rex)" },
            { 455, "Mighty Missile [Decimation Cannon] (Rex)" },
            { 456, "Grenade Burst [Decimation Cannon] (Rex)" },
            { 457, "Flamecaster [Decimation Cannon] (Rex)" },
            { 458, "Surprise Shot [Decimation Cannon] (Nia)" },
            { 459, "Porcupine Blow [Decimation Cannon] (Nia)" },
            { 460, "Mayfly [Decimation Cannon] (Nia)" },
            { 461, "Phosphorescent Flare [Decimation Cannon] (Nia)" },
            { 462, "Cruel Musket [Decimation Cannon] (Mòrag)" },
            { 463, "Thorn Assault [Decimation Cannon] (Mòrag)" },
            { 464, "Rhododendron [Decimation Cannon] (Mòrag)" },
            { 465, "Chimera Drop [Decimation Cannon] (Mòrag)" },
            { 466, "Breakthrough Cannon [Decimation Cannon] (Zeke)" },
            { 467, "Frenzied Fire [Decimation Cannon] (Zeke)" },
            { 468, "Stark Artillery [Decimation Cannon] (Zeke)" },
            { 469, "Crazed Bazooka [Decimation Cannon] (Zeke)" },
            { 470, "Normal Attack 1 [Rockrending Gauntlets] (Rex)" },
            { 471, "Normal Attack 2 [Rockrending Gauntlets] (Rex)" },
            { 472, "Normal Attack 3 [Rockrending Gauntlets] (Rex)" },
            { 473, "Normal Attack 1 [Rockrending Gauntlets] (Nia)" },
            { 474, "Normal Attack 2 [Rockrending Gauntlets] (Nia)" },
            { 475, "Normal Attack 3 [Rockrending Gauntlets] (Nia)" },
            { 476, "Normal Attack 1 [Rockrending Gauntlets] (Mòrag)" },
            { 477, "Normal Attack 2 [Rockrending Gauntlets] (Mòrag)" },
            { 478, "Normal Attack 3 [Rockrending Gauntlets] (Mòrag)" },
            { 479, "Normal Attack 1 [Rockrending Gauntlets] (Zeke)" },
            { 480, "Normal Attack 2 [Rockrending Gauntlets] (Zeke)" },
            { 481, "Normal Attack 3 [Rockrending Gauntlets] (Zeke)" },
            { 482, "Wild Slugger [Rockrending Gauntlets] (Rex)" },
            { 483, "Terror Smash [Rockrending Gauntlets] (Rex)" },
            { 484, "Rock Bullet [Rockrending Gauntlets] (Rex)" },
            { 485, "Giant Straight [Rockrending Gauntlets] (Rex)" },
            { 486, "Lion Claw [Rockrending Gauntlets] (Nia)" },
            { 487, "Meteor Punch [Rockrending Gauntlets] (Nia)" },
            { 488, "Raging Screw [Rockrending Gauntlets] (Nia)" },
            { 489, "Buffalo Beat [Rockrending Gauntlets] (Nia)" },
            { 490, "Armor Breaker [Rockrending Gauntlets] (Mòrag)" },
            { 491, "Bear Twist [Rockrending Gauntlets] (Mòrag)" },
            { 492, "Devastating Tremor [Rockrending Gauntlets] (Mòrag)" },
            { 493, "Earth Mover [Rockrending Gauntlets] (Mòrag)" },
            { 494, "Sublime Lariat [Rockrending Gauntlets] (Zeke)" },
            { 495, "Falling Dropkick [Rockrending Gauntlets] (Zeke)" },
            { 496, "Decisive Knuckle [Rockrending Gauntlets] (Zeke)" },
            { 497, "Sacrificial Chop [Rockrending Gauntlets] (Zeke)" },
            { 498, "Normal Attack 1 [Sword Tonfa] (Rex)" },
            { 499, "Normal Attack 2 [Sword Tonfa] (Rex)" },
            { 500, "Normal Attack 3 [Sword Tonfa] (Rex)" },
            { 501, "Normal Attack 1 [Sword Tonfa] (Nia)" },
            { 502, "Normal Attack 2 [Sword Tonfa] (Nia)" },
            { 503, "Normal ATtack 3 [Sword Tonfa] (Nia)" },
            { 504, "Normal Attack 1 [Sword Tonfa] (Mòrag)" },
            { 505, "Normal Attack 2 [Sword Tonfa] (Mòrag)" },
            { 506, "Normal Attack 3 [Sword Tonfa] (Mòrag)" },
            { 507, "Normal Attack 1 [Sword Tonfa] (Zeke)" },
            { 508, "Normal Attack 2 [Sword Tonfa] (Zeke)" },
            { 509, "Normal Attack 3 [Sword Tonfa] (Zeke)" },
            { 510, "Gale Upper [Sword Tonfa] (Rex)" },
            { 511, "Wind Smash [Sword Tonfa] (Rex)" },
            { 512, "Stormstarter [Sword Tonfa] (Rex)" },
            { 513, "Hurricane Punch [Sword Tonfa] (Rex)" },
            { 514, "Sabertooth Slash [Sword Tonfa] (Nia)" },
            { 515, "Updraft [Sword Tonfa] (Nia)" },
            { 516, "Whirlwind [Sword Tonfa] (Nia)" },
            { 517, "Universal Beat [Sword Tonfa] (Nia)" },
            { 518, "Blade of the Abyss [Sword Tonfa] (Mòrag)" },
            { 519, "Blunt Aggression [Sword Tonfa] (Mòrag)" },
            { 520, "Galeforce Blow [Sword Tonfa] (Mòrag)" },
            { 521, "No Love Lost [Sword Tonfa] (Mòrag)" },
            { 522, "Menacing Lariat [Sword Tonfa] (Zeke)" },
            { 523, "Hurricane Dropkick [Sword Tonfa] (Zeke)" },
            { 524, "Shuddering Tonfa [Sword Tonfa] (Zeke)" },
            { 525, "Wind Dragon Chop [Sword Tonfa] (Zeke)" }
        };

        public static readonly Dictionary<UInt16, string> DRIVER_SKILLS = new Dictionary<UInt16, string>()
        {
            { 0, "(none)" },
            { 1, "Reflex Response" },
            { 2, "Rapid Attack II" },
            { 3, "Picture of Health" },
            { 4, "Lively Lad" },
            { 5, "Topple Champ" },
            { 6, "Fight or Flight" },
            { 7, "Eyes on the Prize" },
            { 8, "Combat Boost" },
            { 9, "Child of Steel" },
            { 10, "Relentless Spirit" },
            { 11, "Rapid Attack I" },
            { 12, "Muscle Builder" },
            { 13, "Combo Enhance" },
            { 14, "Arts Chain" },
            { 15, "Drive to Win" },
            { 16, "Shadow Strike" },
            { 17, "Prediction" },
            { 18, "Inherit Muscle" },
            { 19, "Pre-empt Moves" },
            { 20, "Heart Recharge" },
            { 21, "Ether Studies" },
            { 22, "Rescue" },
            { 23, "Gramps' Blessing" },
            { 24, "Rapid Attack III" },
            { 25, "Untimate Relay" },
            { 26, "Further Ether Studies" },
            { 27, "Cheer Up" },
            { 28, "Herbalism" },
            { 29, "Solidarity" },
            { 30, "Gramps' Auspices" },
            { 31, "Catgirl" },
            { 32, "Big Cat" },
            { 33, "Feral Awakening III" },
            { 34, "Natural Selection" },
            { 35, "Hard Hitter" },
            { 36, "Focused Technique" },
            { 37, "Wild Life" },
            { 38, "Tigergirl" },
            { 39, "Sore Loser" },
            { 40, "Adrenaline" },
            { 41, "Kinetic Vision" },
            { 42, "Feral Awakening II" },
            { 43, "Tooth and Claw" },
            { 44, "Panther Pounce" },
            { 45, "Bestial Pursuit" },
            { 46, "Lithe Strength" },
            { 47, "Tiger's Eye" },
            { 48, "Transmit Power" },
            { 49, "Bear Knuckles" },
            { 50, "Road to Victory" },
            { 51, "Soothing Hand" },
            { 52, "Ether Talent" },
            { 53, "Herb Synthesis" },
            { 54, "Mutual Happiness" },
            { 55, "Feral Awakening I" },
            { 56, "Chance Recovery" },
            { 57, "Lucky Cat" },
            { 58, "Cheer Leader" },
            { 59, "Inner Strength" },
            { 60, "Lady of Luck" },
            { 61, "Raring to Go I" },
            { 62, "Spongy Bod" },
            { 63, "Health Bomb" },
            { 64, "Super Spongy Bod" },
            { 65, "Grin and Bear It" },
            { 66, "Keen Recovery" },
            { 67, "Restless Foot" },
            { 68, "Nopon Topple Arts" },
            { 69, "Look Over There!" },
            { 70, "Super Restless Foot" },
            { 71, "Good With Hands" },
            { 72, "Raring to Go III" },
            { 73, "Tiger Muscles" },
            { 74, "Nopon Chain Arts" },
            { 75, "Crazy With Hands" },
            { 76, "Forced Initiative" },
            { 77, "Nopon Chain Boost" },
            { 78, "Inherit Protein" },
            { 79, "Poppi Care" },
            { 80, "Mood Maker" },
            { 81, "Luckypon" },
            { 82, "Placebo" },
            { 83, "Raring to Go II" },
            { 84, "Double-Luckypon" },
            { 85, "Rapture" },
            { 86, "Nopon Rescue Service" },
            { 87, "Hidden Talent" },
            { 88, "Hope for the Best" },
            { 89, "Team Mascot" },
            { 90, "Burgeoning Talent...?" },
            { 91, "Tough Nut" },
            { 92, "Veteran" },
            { 93, "Guardian" },
            { 94, "Muscle Accelerator II" },
            { 95, "Last Man Standing" },
            { 96, "Power Lift" },
            { 97, "Quick Muscles" },
            { 98, "Recuperation" },
            { 99, "Pumped Dodge" },
            { 100, "Physical Fitness" },
            { 101, "10,000 Push-Ups" },
            { 102, "Burly Chain" },
            { 103, "Muscle Accelerator I" },
            { 104, "Beefcake Flurry" },
            { 105, "Elite Muscles" },
            { 106, "Knack for Battle" },
            { 107, "Anything for a Mate" },
            { 108, "Dependable Guy" },
            { 109, "Honed to Perfection" },
            { 110, "World's Stringest Man" },
            { 111, "Ether Powder" },
            { 112, "Muscle Accelerator III" },
            { 113, "Big Strong Hands" },
            { 114, "Ether Muscle" },
            { 115, "Serious Recovery" },
            { 116, "Thank the Heavens" },
            { 117, "Booming Aid" },
            { 118, "Buff Buff" },
            { 119, "Higher Powers" },
            { 120, "Greatest Work" },
            { 121, "Imperial Footwork" },
            { 122, "Basic Discipline" },
            { 123, "Initiave II" },
            { 124, "Martial Prowess" },
            { 125, "Ultimate Footwork" },
            { 126, "Instant Chain" },
            { 127, "Survival Training" },
            { 128, "Warrior's Resolve" },
            { 129, "Life's Flow" },
            { 130, "Imperial Pride" },
            { 131, "Arts Relay" },
            { 132, "Initiave I" },
            { 133, "Vigilance" },
            { 134, "Physical Discipline" },
            { 135, "Imperial Pursuit" },
            { 136, "Silent Bloodshed" },
            { 137, "Blade Alliance" },
            { 138, "Cool Focus" },
            { 139, "Battle Stance" },
            { 140, "Legendary Skill" },
            { 141, "Etherology" },
            { 142, "Alchemy" },
            { 143, "Charmed Life" },
            { 144, "Imperial First AId" },
            { 145, "Initiave III" },
            { 146, "Inspiration" },
            { 147, "Ultimate Etherology" },
            { 148, "Divination" },
            { 149, "Cheer Master" },
            { 150, "Golden Opportunity" },
            { 151, "Adamant Heart" },
            { 152, "Flash Step" },
            { 153, "Mach Prep II" },
            { 154, "Critical Pro" },
            { 155, "Air Extender" },
            { 156, "Natural Healing" },
            { 157, "Heart of Gold" },
            { 158, "Zeal Heal" },
            { 159, "Fleetfoot Flit" },
            { 160, "Second Wind" },
            { 161, "Mach Prep I" },
            { 162, "Brawn over Brain" },
            { 163, "Camraderie" },
            { 164, "Herculean Strength" },
            { 165, "Move Chaining" },
            { 166, "Feindish Pursuit" },
            { 167, "Professional Hit" },
            { 168, "Hotshot Hit" },
            { 169, "Obsidian Slash" },
            { 170, "A Hero's Work" },
            { 171, "Lucky Duck" },
            { 172, "Ether Prodigy" },
            { 173, "Tantal Courage" },
            { 174, "Mach Prep III" },
            { 175, "Histrionic Hero" },
            { 176, "Ether Autarch" },
            { 177, "Medicinal Maestro" },
            { 178, "Unleash Shining Justice" },
            { 179, "Raucous Cheer" },
            { 180, "As One" }
        };

        public class DriverSkill : IXC2SaveObject
        {
            public const int SIZE = 0xA;

            public UInt16[] IDs { get; set; }
            public UInt16 ColumnNo { get; set; }
            public UInt16 Level { get; set; }

            public DriverSkill(Byte[] data)
            {
                IDs = new UInt16[3];
                for (int i = 0; i < IDs.Length; i++)
                    IDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(i * 2, 2), 0);

                ColumnNo = BitConverter.ToUInt16(data.GetByteSubArray(0x6, 2), 0);
                Level = BitConverter.ToUInt16(data.GetByteSubArray(0x8, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                foreach (UInt16 u in IDs)
                    result.AddRange(BitConverter.GetBytes(u));

                result.AddRange(BitConverter.GetBytes(ColumnNo));
                result.AddRange(BitConverter.GetBytes(Level));

                if (result.Count != SIZE)
                {
                    string message = "DriverSkill: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

        public class Weapon : IXC2SaveObject
        {
            public static readonly Dictionary<int, string> NAMES = new Dictionary<int, string>()
            {
                { 0, "(none)"},
                { 1, "Aegis Sword"},
                { 2, "Catalyst Scimitar"},
                { 3, "Twin Rings"},
                { 4, "Drill Shield"},
                { 5, "Mech Arms"},
                { 6, "Variable Saber"},
                { 7, "Whipswords"},
                { 8, "Big Bang Edge"},
                { 9, "Dual Scythes"},
                { 10, "Greataxe"},
                { 11, "Megalance"},
                { 12, "Ether Cannon"},
                { 13, "Shield Hammer"},
                { 14, "Chroma Katana"},
                { 15, "Bitball"},
                { 16, "Knuckle Claws"},
                { 17, "Broadsword"},
                { 18, "Nodachi (Jin)"},
                { 19, "Sword Tonfa (Malos)"},
                { 20, "Calamity Scythe"},
                { 21, "Cobra Bardiche"},
                { 22, "Infinity Fans"},
                { 23, "Brilliant Twinblades"},
                { 24, "Decimation Cannon"},
                { 25, "Rockrending Gauntlets"},
                { 26, "Sword Tonfa"},
            };

            public const int SIZE = 0x14;

            public UInt32[] ArtIds { get; set; }
            public UInt32 WeaponPoints { get; set; }
            public UInt32 TotalWeaponPoints { get; set; }

            public Weapon(Byte[] data)
            {
                ArtIds = new UInt32[3];
                for (int i = 0; i < ArtIds.Length; i++)
                    ArtIds[i] = BitConverter.ToUInt32(data.GetByteSubArray(i * 4, 4), 0);

                WeaponPoints = BitConverter.ToUInt32(data.GetByteSubArray(0xC, 4), 0);
                TotalWeaponPoints = BitConverter.ToUInt32(data.GetByteSubArray(0x10, 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (UInt32 u in ArtIds)
                    result.AddRange(BitConverter.GetBytes(u));

                result.AddRange(BitConverter.GetBytes(WeaponPoints));
                result.AddRange(BitConverter.GetBytes(TotalWeaponPoints));

                if (result.Count != SIZE)
                {
                    string message = "Weapon: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

        public class Pouch : IXC2SaveObject
        {
            public const int SIZE = 0x8;

            public float Time { get; set; }
            public UInt16 ItemId { get; set; }
            public bool IsEnabled { get; set; }

            public Pouch(Byte[] data)
            {
                Time = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
                ItemId = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
                IsEnabled = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0) == 1;
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                result.AddRange(BitConverter.GetBytes(Time));
                result.AddRange(BitConverter.GetBytes(ItemId));
                result.AddRange(BitConverter.GetBytes((UInt16)(IsEnabled ? 1 : 0)));

                if (result.Count != SIZE)
                {
                    string message = "Pouch: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "BraveryPoints", 0x0 },
            { "BraveryLevel", 0x4 },
            { "TruthPoints", 0x8 },
            { "TruthLevel", 0xC },
            { "CompassionPoints", 0x10 },
            { "CompassionLevel", 0x14 },
            { "JusticePoints", 0x18 },
            { "JusticeLevel", 0x1C },
            { "IsActive", 0x20 },
            { "DriverId", 0x22 },
            { "SetBlade", 0x24 },
            { "EquippedBlades", 0x26 },
            { "gap_2C", 0x2C },
            { "SkillsRound1", 0x30 },
            { "SkillsRound2", 0x62 },
            { "Level", 0x94 },
            { "HpMax", 0x96 },
            { "Strength", 0x98 },
            { "PowEther", 0x9A },
            { "Dex", 0x9C },
            { "Agility", 0x9E },
            { "Luck", 0xA0 },
            { "PArmor", 0xA2 },
            { "EArmor", 0xA4 },
            { "CritRate", 0xA6 },
            { "GuardRate", 0xA7 },
            { "field_A8", 0xA8 },
            { "gap_AC", 0xAC },
            { "field_AF", 0xAF },
            { "Exp", 0xB0 },
            { "BattleExp", 0xB4 },
            { "SkillPoints", 0xB8 },
            { "TotalSkillPoints", 0xBC },
            { "Weapons", 0xC0 },
            { "gap_2DC", 0x2DC },
            { "AccessoryId2", 0x2E8 },
            { "gap_2EA", 0x2EA },
            { "AccessoryHandle2", 0x2EC },
            { "DriverArtLevels", 0x2F0 },
            { "gap_4FE", 0x4FE },
            { "AccessoryHandle0", 0x574 },
            { "AccessoryId0", 0x578 },
            { "gap_57A", 0x57A },
            { "AccessoryHandle1", 0x57C },
            { "AccessoryId1", 0x580 },
            { "gap_582", 0x582 },
            { "PouchInfo", 0x584 },
            { "gap_field_59C", 0x59C }
        };

        public UInt32 BraveryPoints { get; set; }
        public UInt32 BraveryLevel { get; set; }
        public UInt32 TruthPoints { get; set; }
        public UInt32 TruthLevel { get; set; }
        public UInt32 CompassionPoints { get; set; }
        public UInt32 CompassionLevel { get; set; }
        public UInt32 JusticePoints { get; set; }
        public UInt32 JusticeLevel { get; set; }
        public UInt16 IsActive { get; set; }
        public UInt16 ID { get; set; }
        public Int16 SetBlade { get; set; }
        public UInt16[] EquippedBlades { get; set; }
        public UInt32 Unk_0c2C { get; set; }
        public DriverSkill[] OvertSkills { get; set; }
        public DriverSkill[] HiddenSkills { get; set; }
        public UInt16 Level { get; set; }
        public UInt16 HP { get; set; }
        public UInt16 Strength { get; set; }
        public UInt16 Ether { get; set; }
        public UInt16 Dexterity { get; set; }
        public UInt16 Agility { get; set; }
        public UInt16 Luck { get; set; }
        public UInt16 PhysDef { get; set; }
        public UInt16 EtherDef { get; set; }
        public Byte CriticalRate { get; set; }
        public Byte GuardRate { get; set; }
        public UInt32 Unk_0xA8 { get; set; }
        public Byte[] Unk_0xAC { get; set; }
        public Byte Unk_0xAF { get; set; }
        public UInt32 BonusExp { get; set; }
        public UInt32 BattleExp { get; set; }
        public UInt32 CurrentSkillPoints { get; set; }
        public UInt32 TotalSkillPoints { get; set; }
        public Weapon[] Weapons { get; set; }
        public Byte[] Unk_0x02DC { get; set; }
        public UInt16 AccessoryId2 { get; set; }
        public Byte[] Unk_0x02EA { get; set; }
        public ItemHandle AccessoryHandle2 { get; set; }
        public Byte[] DriverArtLevels { get; set; }
        public Byte[] Unk_0x04FE { get; set; }
        public ItemHandle AccessoryHandle0 { get; set; }
        public UInt16 AccessoryId0 { get; set; }
        public UInt16 Unk_0x057A { get; set; }
        public ItemHandle AccessoryHandle1 { get; set; }
        public UInt16 AccessoryId1 { get; set; }
        public UInt16 Unk_0x0582 { get; set; }
        public Pouch[] PouchInfo { get; set; }
        public Byte[] Unk_0x059C { get; set; }

        public Driver(Byte[] data)
        {
            BraveryPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryPoints"], 4), 0);
            BraveryLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryLevel"], 4), 0);
            TruthPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthPoints"], 4), 0);
            TruthLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthLevel"], 4), 0);
            CompassionPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CompassionPoints"], 4), 0);
            CompassionLevel = BitConverter.ToUInt32( data.GetByteSubArray(LOC["CompassionLevel"], 4), 0);
            JusticePoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticePoints"], 4), 0);
            JusticeLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticeLevel"], 4), 0);
            IsActive = BitConverter.ToUInt16(data.GetByteSubArray(LOC["IsActive"], 2), 0);
            ID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["DriverId"], 2), 0);
            SetBlade = BitConverter.ToInt16(data.GetByteSubArray(LOC["SetBlade"], 2), 0);

            EquippedBlades = new UInt16[3];
            for (int i = 0; i < EquippedBlades.Length; i++)
                EquippedBlades[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EquippedBlades"] + (i * 2), 2), 0);

            Unk_0c2C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["gap_2C"], 4), 0);

            OvertSkills = new DriverSkill[5];
            for (int i = 0; i < OvertSkills.Length; i++)
                OvertSkills[i] = new DriverSkill(data.GetByteSubArray(LOC["SkillsRound1"] + (i * DriverSkill.SIZE), DriverSkill.SIZE));

            HiddenSkills = new DriverSkill[5];
            for (int i = 0; i < HiddenSkills.Length; i++)
                HiddenSkills[i] = new DriverSkill(data.GetByteSubArray(LOC["SkillsRound2"] + (i * DriverSkill.SIZE), DriverSkill.SIZE));

            Level = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Level"], 2), 0);
            HP = BitConverter.ToUInt16(data.GetByteSubArray(LOC["HpMax"], 2), 0);
            Strength = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Strength"], 2), 0);
            Ether = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PowEther"], 2), 0);
            Dexterity = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Dex"], 2), 0);
            Agility = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Agility"], 2), 0);
            Luck = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Luck"], 2), 0);
            PhysDef = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PArmor"], 2), 0);
            EtherDef = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EArmor"], 2), 0);
            CriticalRate = data[LOC["CritRate"]];
            GuardRate = data[LOC["GuardRate"]];
            Unk_0xA8 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_A8"], 4), 0);
            Unk_0xAC = data.GetByteSubArray(LOC["gap_AC"], 3);
            Unk_0xAF = data[LOC["field_AF"]];
            BonusExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Exp"], 4), 0);
            BattleExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BattleExp"], 4), 0);
            CurrentSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["SkillPoints"], 4), 0);
            TotalSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TotalSkillPoints"], 4), 0);

            Weapons = new Weapon[27];
            for (int i = 0; i < Weapons.Length; i++)
                Weapons[i] = new Weapon(data.GetByteSubArray(LOC["Weapons"] + (i * Weapon.SIZE), Weapon.SIZE));

            Unk_0x02DC = data.GetByteSubArray(LOC["gap_2DC"], 12);
            AccessoryId2 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId2"], 2), 0);
            Unk_0x02EA = data.GetByteSubArray(LOC["gap_2EA"], 2);
            AccessoryHandle2 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle2"], ItemHandle32.SIZE));
            DriverArtLevels = data.GetByteSubArray(LOC["DriverArtLevels"], 526);
            Unk_0x04FE = data.GetByteSubArray(LOC["gap_4FE"], 118);
            AccessoryHandle0 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle0"], ItemHandle32.SIZE));
            AccessoryId0 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId0"], 2), 0);
            Unk_0x057A = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_57A"], 2), 0);
            AccessoryHandle1 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle1"], ItemHandle32.SIZE));
            AccessoryId1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId1"], 2), 0);
            Unk_0x0582 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_582"], 2), 0);

            PouchInfo = new Pouch[3];
            for (int i = 0; i < PouchInfo.Length; i++)
                PouchInfo[i] = new Pouch(data.GetByteSubArray(LOC["PouchInfo"] + (i * Pouch.SIZE), Pouch.SIZE));

            Unk_0x059C = data.GetByteSubArray(LOC["gap_field_59C"], 4);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(BraveryPoints));
            result.AddRange(BitConverter.GetBytes(BraveryLevel));
            result.AddRange(BitConverter.GetBytes(TruthPoints));
            result.AddRange(BitConverter.GetBytes(TruthLevel));
            result.AddRange(BitConverter.GetBytes(CompassionPoints));
            result.AddRange(BitConverter.GetBytes(CompassionLevel));
            result.AddRange(BitConverter.GetBytes(JusticePoints));
            result.AddRange(BitConverter.GetBytes(JusticeLevel));
            result.AddRange(BitConverter.GetBytes(IsActive));
            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(BitConverter.GetBytes(SetBlade));

            foreach (UInt16 u in EquippedBlades)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(BitConverter.GetBytes(Unk_0c2C));

            foreach (DriverSkill ds in OvertSkills)
                result.AddRange(ds.ToRawData());

            foreach (DriverSkill ds in HiddenSkills)
                result.AddRange(ds.ToRawData());

            result.AddRange(BitConverter.GetBytes(Level));
            result.AddRange(BitConverter.GetBytes(HP));
            result.AddRange(BitConverter.GetBytes(Strength));
            result.AddRange(BitConverter.GetBytes(Ether));
            result.AddRange(BitConverter.GetBytes(Dexterity));
            result.AddRange(BitConverter.GetBytes(Agility));
            result.AddRange(BitConverter.GetBytes(Luck));
            result.AddRange(BitConverter.GetBytes(PhysDef));
            result.AddRange(BitConverter.GetBytes(EtherDef));
            result.Add(CriticalRate);
            result.Add(GuardRate);
            result.AddRange(BitConverter.GetBytes(Unk_0xA8));
            result.AddRange(Unk_0xAC);
            result.Add(Unk_0xAF);
            result.AddRange(BitConverter.GetBytes(BonusExp));
            result.AddRange(BitConverter.GetBytes(BattleExp));
            result.AddRange(BitConverter.GetBytes(CurrentSkillPoints));
            result.AddRange(BitConverter.GetBytes(TotalSkillPoints));

            foreach (Weapon w in Weapons)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x02DC);
            result.AddRange(BitConverter.GetBytes(AccessoryId2));
            result.AddRange(Unk_0x02EA);
            result.AddRange(AccessoryHandle2.ToRawData());
            result.AddRange(DriverArtLevels);
            result.AddRange(Unk_0x04FE);
            result.AddRange(AccessoryHandle0.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId0));
            result.AddRange(BitConverter.GetBytes(Unk_0x057A));
            result.AddRange(AccessoryHandle1.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId1));
            result.AddRange(BitConverter.GetBytes(Unk_0x0582));

            foreach (Pouch p in PouchInfo)
                result.AddRange(p.ToRawData());

            result.AddRange(Unk_0x059C);

            if (result.Count != SIZE)
            {
                string message = "Driver: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            if (IDNAMES.ContainsKey(ID))
                return ID + ": " + IDNAMES[ID];
            else
                return "INVALID ID";
        }
    }
}
