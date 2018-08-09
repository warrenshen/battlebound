// ====================================================================================================
//
// Cloud Code for ChallengeCardModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeMovesModule");

const CARD_COLOR_NEUTRAL = 0;
const CARD_COLOR_FIRE = 1;
const CARD_COLOR_EARTH = 2;
const CARD_COLOR_WATER = 3;
const CARD_COLOR_DARK = 4;
const CARD_COLOR_LIGHT = 5;

const CARD_NAME_FIREBUG_CATELYN = "Firebug Catelyn";
const CARD_NAME_MARSHWATER_SQUEALER = "Marshwater Squealer";
const CARD_NAME_WATERBORNE_RAZORBACK = "Waterborne Razorback";
const CARD_NAME_BLESSED_NEWBORN = "Blessed Newborn";
const CARD_NAME_YOUNG_KYO = "Young Kyo";
const CARD_NAME_WAVE_CHARMER = "Wave Charmer";
const CARD_NAME_POSEIDONS_HANDMAIDEN = "Poseidon's Handmaiden";
const CARD_NAME_EMBERKITTY = "Emberkitty";
const CARD_NAME_FIRESTRIDED_TIGRESS = "Firestrided Tigress";
const CARD_NAME_TEMPLE_GUARDIAN = "Temple Guardian";
const CARD_NAME_BOMBSHELL_BOMBADIER = "Bombshell Bombadier";
const CARD_NAME_TAJI_THE_FEARLESS = "Taji the Fearless";
const CARD_NAME_UNKINDLED_JUNIOR = "Unkindled Junior";
const CARD_NAME_FLAMEBELCHER = "Flamebelcher";
const CARD_NAME_FIREBORN_MENACE = "Fireborn Menace";
const CARD_NAME_TEA_GREENLEAF = "Te'a Greenleaf";
const CARD_NAME_NESSA_NATURES_CHAMPION = "Nessa, Nature's Champion";
const CARD_NAME_BUBBLE_SQUIRTER = "Bubble Squirter";
const CARD_NAME_SWIFT_SHELLBACK = "Swift Shellback";
const CARD_NAME_SENTIENT_SEAKING = "Sentient Seaking";
const CARD_NAME_CRYSTAL_SNAPPER = "Crystal Snapper";
const CARD_NAME_BATTLECLAD_GASDON = "Battleclad Gasdon";
const CARD_NAME_REDHAIRED_PALADIN = "Redhaired Paladin";
const CARD_NAME_FIRESWORN_GODBLADE = "Firesworn Godblade";
const CARD_NAME_RITUAL_HATCHLING = "Ritual Hatchling";
const CARD_NAME_HELLBRINGER = "Hellbringer";
const CARD_NAME_HOOFED_LUSH = "Hoofed Lush";
const CARD_NAME_DIONYSIAN_TOSSPOT = "Dionysian Tosspot";
const CARD_NAME_SEAHORSE_SQUIRE = "Seahorse Squire";
const CARD_NAME_TRIDENT_BATTLEMAGE = "Trident Battlemage";
const CARD_NAME_SNEERBLADE = "Sneerblade";
const CARD_NAME_TIMEWARP_KINGPIN = "Timewarp Kingpin";
const CARD_NAME_LUX = "Lux";
const CARD_NAME_THUNDEROUS_DESPERADO = "Thunderous Desperado";
const CARD_NAME_CEREBOAROUS = "Cereboarus";
const CARD_NAME_GUPPEA = "Guppea";
const CARD_NAME_RHYNOKARP = "Rhynokarp";
const CARD_NAME_PRICKLEPILLAR = "Pricklepillar";
const CARD_NAME_ADDERSPINE_WEEVIL = "Adderspine Weevil";
const CARD_NAME_THIEF_OF_NIGHT = "Thief of Night";
const CARD_NAME_POWER_SIPHONER = "POWER Siphoner";
const CARD_NAME_LIL_RUSTY = "Lil' Rusty";
const CARD_NAME_INFERNO_902 = "INFERNO-902";
const CARD_NAME_CHAR_BOT_451 = "CHAR-BOT-451";
const CARD_NAME_MEGAPUNK = "MegaPUNK";
const CARD_NAME_BLUE_GIPSY_V3 = "Blue Gipsy V3";
const CARD_NAME_FROSTLAND_THRASHER_8 = "Frostland THRASHER-8";
const CARD_NAME_CYBER_SENTINEL = "Cyber Sentinel";
const CARD_NAME_PAL_V1 = "PAL_V1";
const CARD_NAME_FORTRESS_KNIGHT = "Fortress Knight";
const CARD_NAME_TERRATANK = "Terratank";
const CARD_NAME_CULYSSA = "Culyssa";
const CARD_NAME_SABRE_CRYSTALLINE_DRAGON = "Sabre, Crystalline Dragon";
const CARD_NAME_SAPLET = "Saplet";
const CARD_NAME_FIRESMITH_APPRENTICE = "Firesmith Apprentice";
const CARD_NAME_FORGEMECH = "Forgemech";
const CARD_NAME_LIGHTHUNTER = "Lighthunter";
const CARD_NAME_BRINGER_OF_DAWN = "Bringer of Dawn";
const CARD_NAME_ABYSSAL_EEL = "Abyssal Eel";
const CARD_NAME_EMILIA_AIRHEART = "Emilia Airheart";
const CARD_NAME_PEARL_NYMPH = "Pearl Nymph";
const CARD_NAME_TARA_SWAN_PRINCESS = "Tara, Swan Princess";
const CARD_NAME_FROSTSPORE = "Frostspore";

const CARD_NAME_ANGELIC_EGG = "Angelic Egg";
const CARD_NAME_CURSED_EGG = "Cursed Egg";
const CARD_NAME_FOXY_APPRENTICE = "Foxy Apprentice";
