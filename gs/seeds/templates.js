const testUtils = require('./seedUtils');

const TEMPLATES_JSON = [
  {
    "abilities": [
      0,
      2
    ],
    "cost": 50,
    "color": 3,
    "attack": 30,
    "name": "Poseidon's Handmaiden",
    "health": 60,
    "id": "B0",
    "category": 0,
  },
  {
    "abilities": [
      12
    ],
    "cost": 60,
    "color": 1,
    "attack": 50,
    "name": "Bombshell Bombadier",
    "health": 40,
    "id": "B1",
    "category": 0,
  },
  {
    "abilities": [
      17
    ],
    "cost": 50,
    "color": 0,
    "attack": 80,
    "name": "Hellbringer",
    "health": 70,
    "id": "B2",
    "category": 0,
  },
  {
    "abilities": [
      36
    ],
    "cost": 100,
    "color": 4,
    "attack": 60,
    "name": "Krul, Phantom Skullcrusher",
    "health": 80,
    "id": "B3",
    "category": 0,
  },
  {
    "abilities": [
      51
    ],
    "cost": 70,
    "color": 1,
    "attack": 60,
    "name": "Fireborn Menace",
    "health": 60,
    "id": "B4",
    "category": 0,
  },
  {
    "abilities": [
      31
    ],
    "cost": 70,
    "color": 4,
    "attack": 60,
    "name": "POWER SIPHONER",
    "health": 50,
    "id": "B5",
    "category": 0,
  },
  {
    "abilities": [
      29
    ],
    "cost": 80,
    "color": 5,
    "attack": 40,
    "name": "Thunderous Desperado",
    "health": 60,
    "id": "B6",
    "category": 0,
  },
  {
    "abilities": [
      2,
      46
    ],
    "cost": 0,
    "color": 5,
    "attack": 0,
    "name": "Angelic Egg",
    "health": 10,
    "id": "B7",
    "category": 0,
  },
  {
    "abilities": [
      7
    ],
    "cost": 20,
    "color": 3,
    "attack": 20,
    "name": "Marshwater Squealer",
    "health": 30,
    "id": "C0",
    "category": 0,
  },
  {
    "abilities": [
      0,
      8
    ],
    "cost": 40,
    "color": 3,
    "attack": 30,
    "name": "Waterborne Razorback",
    "health": 60,
    "id": "C1",
    "category": 0,
  },
  {
    "abilities": [
      12
    ],
    "cost": 60,
    "color": 1,
    "attack": 50,
    "name": "Bombshell Bombadier",
    "health": 40,
    "id": "C10",
    "category": 0,
  },
  {
    "abilities": [
      1
    ],
    "cost": 30,
    "color": 2,
    "attack": 30,
    "name": "Taji the Fearless",
    "health": 50,
    "id": "C11",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 20,
    "color": 0,
    "attack": 10,
    "name": "Unkindled Junior",
    "health": 20,
    "id": "C12",
    "category": 0,
  },
  {
    "abilities": [
      16
    ],
    "cost": 40,
    "color": 1,
    "attack": 30,
    "name": "Flamebelcher",
    "health": 30,
    "id": "C13",
    "category": 0,
  },
  {
    "abilities": [
      51
    ],
    "cost": 70,
    "color": 1,
    "attack": 60,
    "name": "Fireborn Menace",
    "health": 60,
    "id": "C14",
    "category": 0,
  },
  {
    "cost": 50,
    "color": 3,
    "name": "Brr Brr Blizzard",
    "id": "C15",
    "category": 1,
  },
  {
    "cost": 40,
    "color": 3,
    "name": "Widespread Frostbite",
    "id": "C16",
    "category": 1,
  },
  {
    "abilities": [
      1
    ],
    "cost": 20,
    "color": 0,
    "attack": 20,
    "name": "Te'a Greenleaf",
    "health": 30,
    "id": "C17",
    "category": 0,
  },
  {
    "abilities": [
      1,
      5
    ],
    "cost": 50,
    "color": 2,
    "attack": 40,
    "name": "Nessa, Nature's Champion",
    "health": 60,
    "id": "C18",
    "category": 0,
  },
  {
    "cost": 20,
    "color": 0,
    "name": "Touch of Zeus",
    "id": "C19",
    "category": 1,
  },
  {
    "abilities": [
      10
    ],
    "cost": 20,
    "color": 1,
    "attack": 20,
    "name": "Firebug Catelyn",
    "health": 10,
    "id": "C2",
    "category": 0,
  },
  {
    "cost": 50,
    "color": 5,
    "name": "Shields Up!",
    "id": "C20",
    "category": 1,
  },
  {
    "cost": 20,
    "color": 3,
    "name": "Deep Freeze",
    "id": "C21",
    "category": 1,
  },
  {
    "cost": 50,
    "color": 0,
    "name": "Greedy Fingers",
    "id": "C22",
    "category": 1,
  },
  {
    "cost": 40,
    "color": 4,
    "name": "Silence the Meek",
    "id": "C23",
    "category": 1,
  },
  {
    "cost": 30,
    "color": 0,
    "name": "Rally to the Queen",
    "id": "C24",
    "category": 1,
  },
  {
    "cost": 50,
    "color": 0,
    "name": "Death Note",
    "id": "C25",
    "category": 1,
  },
  {
    "cost": 20,
    "color": 0,
    "name": "Bombs Away",
    "id": "C26",
    "category": 1,
  },
  {
    "cost": 50,
    "color": 0,
    "name": "Battle Royale",
    "id": "C27",
    "category": 1,
  },
  {
    "cost": 40,
    "color": 4,
    "name": "Grave-digging",
    "id": "C28",
    "category": 1,
  },
  {
    "abilities": [
      2
    ],
    "cost": 20,
    "color": 3,
    "attack": 10,
    "name": "Bubble Squirter",
    "health": 30,
    "id": "C29",
    "category": 0,
  },
  {
    "abilities": [
      4
    ],
    "cost": 20,
    "color": 2,
    "attack": 10,
    "name": "Blessed Newborn",
    "health": 10,
    "id": "C3",
    "category": 0,
  },
  {
    "abilities": [
      0
    ],
    "cost": 40,
    "color": 3,
    "attack": 20,
    "name": "Swift Shellback",
    "health": 50,
    "id": "C30",
    "category": 0,
  },
  {
    "abilities": [
      2,
      8
    ],
    "cost": 60,
    "color": 3,
    "attack": 30,
    "name": "Sentient Seaking",
    "health": 80,
    "id": "C31",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 20,
    "color": 0,
    "attack": 30,
    "name": "Crystal Snapper",
    "health": 30,
    "id": "C32",
    "category": 0,
  },
  {
    "abilities": [
      21
    ],
    "cost": 70,
    "color": 4,
    "attack": 30,
    "name": "Battleclad Gasdon",
    "health": 70,
    "id": "C33",
    "category": 0,
  },
  {
    "abilities": [
      22
    ],
    "cost": 30,
    "color": 0,
    "attack": 40,
    "name": "Redhaired Paladin",
    "health": 30,
    "id": "C34",
    "category": 0,
  },
  {
    "abilities": [
      23
    ],
    "cost": 70,
    "color": 1,
    "attack": 50,
    "name": "Firesworn Godblade",
    "health": 70,
    "id": "C35",
    "category": 0,
  },
  {
    "abilities": [
      24
    ],
    "cost": 20,
    "color": 4,
    "attack": 40,
    "name": "Ritual Hatchling",
    "health": 30,
    "id": "C36",
    "category": 0,
  },
  {
    "abilities": [
      17
    ],
    "cost": 50,
    "color": 0,
    "attack": 80,
    "name": "Hellbringer",
    "health": 70,
    "id": "C37",
    "category": 0,
  },
  {
    "abilities": [
      1,
      4
    ],
    "cost": 30,
    "color": 2,
    "attack": 20,
    "name": "Hoofed Lush",
    "health": 20,
    "id": "C38",
    "category": 0,
  },
  {
    "abilities": [
      25
    ],
    "cost": 50,
    "color": 5,
    "attack": 40,
    "name": "Dionysian Tosspot",
    "health": 30,
    "id": "C39",
    "category": 0,
  },
  {
    "abilities": [
      15
    ],
    "cost": 20,
    "color": 1,
    "attack": 20,
    "name": "Young Kyo",
    "health": 20,
    "id": "C4",
    "category": 0,
  },
  {
    "abilities": [
      26
    ],
    "cost": 30,
    "color": 3,
    "attack": 20,
    "name": "Seahorse Squire",
    "health": 30,
    "id": "C40",
    "category": 0,
  },
  {
    "abilities": [
      27
    ],
    "cost": 60,
    "color": 3,
    "attack": 40,
    "name": "Trident Battlemage",
    "health": 60,
    "id": "C41",
    "category": 0,
  },
  {
    "abilities": [
      20
    ],
    "cost": 30,
    "color": 4,
    "attack": 40,
    "name": "Sneerblade",
    "health": 30,
    "id": "C42",
    "category": 0,
  },
  {
    "abilities": [
      5,
      20
    ],
    "cost": 60,
    "color": 4,
    "attack": 70,
    "name": "Kronos, Timewarp Kingpin",
    "health": 60,
    "id": "C43",
    "category": 0,
  },
  {
    "abilities": [
      28
    ],
    "cost": 30,
    "color": 5,
    "attack": 20,
    "name": "Lux",
    "health": 30,
    "id": "C44",
    "category": 0,
  },
  {
    "abilities": [
      29
    ],
    "cost": 80,
    "color": 5,
    "attack": 40,
    "name": "Thunderous Desperado",
    "health": 60,
    "id": "C45",
    "category": 0,
  },
  {
    "abilities": [
      5,
      6
    ],
    "cost": 50,
    "color": 4,
    "attack": 40,
    "name": "Cereboarus",
    "health": 40,
    "id": "C46",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 0,
    "color": 0,
    "attack": 10,
    "name": "Guppea",
    "health": 10,
    "id": "C47",
    "category": 0,
  },
  {
    "abilities": [
      0
    ],
    "cost": 30,
    "color": 3,
    "attack": 20,
    "name": "Rhynokarp",
    "health": 30,
    "id": "C48",
    "category": 0,
  },
  {
    "abilities": [
      1,
      21
    ],
    "cost": 30,
    "color": 2,
    "attack": 10,
    "name": "Pricklepillar",
    "health": 30,
    "id": "C49",
    "category": 0,
  },
  {
    "abilities": [
      4
    ],
    "cost": 30,
    "color": 3,
    "attack": 20,
    "name": "Wave Charmer",
    "health": 30,
    "id": "C5",
    "category": 0,
  },
  {
    "abilities": [
      1,
      30
    ],
    "cost": 60,
    "color": 2,
    "attack": 40,
    "name": "Adderspine Weevil",
    "health": 60,
    "id": "C50",
    "category": 0,
  },
  {
    "abilities": [
      18
    ],
    "cost": 30,
    "color": 0,
    "attack": 10,
    "name": "Thief of Night",
    "health": 40,
    "id": "C51",
    "category": 0,
  },
  {
    "abilities": [
      31
    ],
    "cost": 70,
    "color": 4,
    "attack": 60,
    "name": "POWER SIPHONER",
    "health": 50,
    "id": "C52",
    "category": 0,
  },
  {
    "abilities": [
      20
    ],
    "cost": 40,
    "color": 0,
    "attack": 20,
    "name": "Lil' Rusty",
    "health": 50,
    "id": "C53",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 30,
    "color": 1,
    "attack": 40,
    "name": "CHAR-BOT-451",
    "health": 20,
    "id": "C54",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 30,
    "color": 0,
    "attack": 30,
    "name": "MegaPUNK",
    "health": 40,
    "id": "C55",
    "category": 0,
  },
  {
    "cost": 80,
    "color": 1,
    "name": "Raze to Ashes",
    "id": "C56",
    "category": 1,
  },
  {
    "abilities": [
      34
    ],
    "cost": 30,
    "color": 4,
    "attack": 20,
    "name": "Dusk Dweller",
    "health": 10,
    "id": "C57",
    "category": 0,
  },
  {
    "abilities": [
      35
    ],
    "cost": 60,
    "color": 4,
    "attack": 40,
    "name": "Talusreaver",
    "health": 50,
    "id": "C58",
    "category": 0,
  },
  {
    "abilities": [
      36
    ],
    "cost": 100,
    "color": 4,
    "attack": 60,
    "name": "Krul, Phantom Skullcrusher",
    "health": 80,
    "id": "C59",
    "category": 0,
  },
  {
    "abilities": [
      0,
      2
    ],
    "cost": 50,
    "color": 3,
    "attack": 30,
    "name": "Poseidon's Handmaiden",
    "health": 60,
    "id": "C6",
    "category": 0,
  },
  {
    "abilities": [
      37
    ],
    "cost": 20,
    "color": 3,
    "attack": 20,
    "name": "Blue Gipsy V3",
    "health": 30,
    "id": "C60",
    "category": 0,
  },
  {
    "abilities": [
      38
    ],
    "cost": 60,
    "color": 3,
    "attack": 50,
    "name": "Frostland THRASHER-8",
    "health": 60,
    "id": "C61",
    "category": 0,
  },
  {
    "abilities": [
      40
    ],
    "cost": 20,
    "color": 5,
    "attack": 10,
    "name": "PAL_V1",
    "health": 30,
    "id": "C62",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 60,
    "color": 5,
    "attack": 30,
    "name": "Cyber Sentinel",
    "health": 50,
    "id": "C63",
    "category": 0,
  },
  {
    "abilities": [
      1
    ],
    "cost": 30,
    "color": 0,
    "attack": 30,
    "name": "Armored Warden",
    "health": 40,
    "id": "C64",
    "category": 0,
  },
  {
    "abilities": [
      5
    ],
    "cost": 30,
    "color": 4,
    "attack": 30,
    "name": "Graveyard Guardian",
    "health": 40,
    "id": "C65",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 10,
    "color": 0,
    "attack": 10,
    "name": "Arc Knight",
    "health": 30,
    "id": "C66",
    "category": 0,
  },
  {
    "abilities": [
      1,
      41
    ],
    "cost": 80,
    "color": 2,
    "attack": 40,
    "name": "Terratank",
    "health": 90,
    "id": "C67",
    "category": 0,
  },
  {
    "abilities": [
      33
    ],
    "cost": 10,
    "color": 4,
    "attack": 10,
    "name": "Culyssa",
    "health": 10,
    "id": "C68",
    "category": 0,
  },
  {
    "abilities": [
      42
    ],
    "cost": 70,
    "color": 3,
    "attack": 40,
    "name": "Sabre, Crystalline Dragon",
    "health": 80,
    "id": "C69",
    "category": 0,
  },
  {
    "abilities": [
      13
    ],
    "cost": 30,
    "color": 1,
    "attack": 30,
    "name": "Emberkitty",
    "health": 40,
    "id": "C7",
    "category": 0,
  },
  {
    "abilities": [
      43
    ],
    "cost": 20,
    "color": 1,
    "attack": 10,
    "name": "Firesmith Apprentice",
    "health": 30,
    "id": "C70",
    "category": 0,
  },
  {
    "abilities": [
      44
    ],
    "cost": 40,
    "color": 1,
    "attack": 20,
    "name": "Forgemech",
    "health": 20,
    "id": "C71",
    "category": 0,
  },
  {
    "abilities": [
      45
    ],
    "cost": 30,
    "color": 5,
    "attack": 40,
    "name": "Lighthunter",
    "health": 30,
    "id": "C72",
    "category": 0,
  },
  {
    "abilities": [
      45
    ],
    "cost": 70,
    "color": 5,
    "attack": 60,
    "name": "Bringer of Dawn",
    "health": 60,
    "id": "C73",
    "category": 0,
  },
  {
    "abilities": [],
    "cost": 40,
    "color": 4,
    "attack": 60,
    "name": "Abyssal Eel",
    "health": 30,
    "id": "C74",
    "category": 0,
  },
  {
    "abilities": [
      0,
      45
    ],
    "cost": 70,
    "color": 5,
    "attack": 50,
    "name": "Emilia Airheart",
    "health": 70,
    "id": "C75",
    "category": 0,
  },
  {
    "abilities": [
      2,
      46
    ],
    "cost": 0,
    "color": 5,
    "attack": 0,
    "name": "Angelic Egg",
    "health": 10,
    "id": "C76",
    "category": 0,
  },
  {
    "abilities": [
      49
    ],
    "cost": 0,
    "color": 4,
    "attack": 0,
    "name": "Cursed Egg",
    "health": 40,
    "id": "C77",
    "category": 0,
  },
  {
    "abilities": [
      0
    ],
    "cost": 10,
    "color": 3,
    "attack": 10,
    "name": "Pearl Nymph",
    "health": 10,
    "id": "C78",
    "category": 0,
  },
  {
    "abilities": [
      19
    ],
    "cost": 60,
    "color": 3,
    "attack": 40,
    "name": "Tara, Swan Princess",
    "health": 50,
    "id": "C79",
    "category": 0,
  },
  {
    "abilities": [
      14
    ],
    "cost": 50,
    "color": 1,
    "attack": 50,
    "name": "Firestrided Tigress",
    "health": 40,
    "id": "C8",
    "category": 0,
  },
  {
    "abilities": [
      42
    ],
    "cost": 10,
    "color": 3,
    "attack": 10,
    "name": "Frostspore",
    "health": 10,
    "id": "C80",
    "category": 0,
  },
  {
    "abilities": [
      1,
      2
    ],
    "cost": 60,
    "color": 2,
    "attack": 40,
    "name": "Temple Guardian",
    "health": 60,
    "id": "C9",
    "category": 0,
  },
];

TEMPLATES_JSON.forEach(function(templateJson) {
  const templateString = JSON.stringify(templateJson);

  const id = templateJson.id;
  const category = templateJson.category;
  const color = templateJson.color;
  const cost = templateJson.cost;
  const name = templateJson.name;
  if (id == null || (id.indexOf("C") < 0 && id.indexOf("B") < 0)) {
    console.log("Invalid id for template: " + templateString);
  } else if (category != 0 && category != 1) {
    console.log("Invalid category for template: " + templateString);
  } else if (color == null || color < 0 || color > 5) {
    console.log("Invalid color for template: " + templateString);
  } else if (cost == null) {
    console.log("Invalid cost for template: " + templateString);
  } else if (name == null || name.length <= 0) {
    console.log("Invalid name for template: " + templateString);
  }

  const attack = templateJson.attack;
  const health = templateJson.health;
  const abilities = templateJson.abilities;

  if (category === 0) {
    if (attack == null || attack < 0 || attack > 100) {
      console.log("Invalid attack for template: " + templateString);
    } else if (health == null || health <= 0 || health > 100) {
      console.log("Invalid health for template: " + templateString);
    } else if (abilities == null) {
      console.log("Invalid abilities for template: " + templateString);
    }
  } else if (category === 1) {
    if (attack != null) {
      console.log("Invalid attack for template: " + templateString);
    } else if (health != null) {
      console.log("Invalid health for template: " + templateString);
    } else if (abilities != null) {
      console.log("Invalid abilities for template: " + templateString);
    }
  }
});

const gamesparks = testUtils.gamesparks;
testUtils.initGS(function() {
  gamesparks.sendWithData(
    "LogEventRequest",
    {
      eventKey: "AdminUpdateTemplates",
      authKey: "j10f74ndkuwn57gp",
      templatesJson: TEMPLATES_JSON,
    },
    function(response) {
      console.log(response);
    }
  );
});
