// ====================================================================================================
//
// Cloud Code for AdminUpdateTemplates, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("DeckModule");

const ADMIN_USERNAMES = ["warren"];
const ADMIN_AUTH_KEY = "j10f74ndkuwn57gp";

const API = Spark.getGameDataService();
const player = Spark.getPlayer();

if (!ADMIN_USERNAMES.indexOf(player.getUserName()) < 0) {
    setScriptError("Forbidden.");
}

const authKey = Spark.getData().authKey;

if (authKey != ADMIN_AUTH_KEY) {
    setScriptError("Forbidden.");
}

Spark.getLog().info("Admin authorized: AdminUpdateTemplates.");

const templatesJson = Spark.getData().templatesJson;
// Validations.
templatesJson.forEach(function(templateJson) {
    const templateString = JSON.stringify(templateJson);
    
    const id = templateJson.id;
    const category = templateJson.category;
    const color = templateJson.color;
    const cost = templateJson.cost;
    const name = templateJson.name;
    if (id == null || (id.indexOf("C") < 0 && id.indexOf("B") < 0)) {
        setScriptError("Invalid id for template: " + templateString);
    } else if (category != 0 && category != 1) {
        setScriptError("Invalid category for template: " + templateString);
    } else if (color == null || color < 0 || color > 5) {
        setScriptError("Invalid color for template: " + templateString);
    } else if (cost == null) {
        setScriptError("Invalid cost for template: " + templateString);
    } else if (name == null || name.length <= 0) {
        setScriptError("Invalid name for template: " + templateString);
    }
    
    const attack = templateJson.attack;
    const health = templateJson.health;
    const abilities = templateJson.abilities;
        
    if (category === 0) {
        if (attack == null || attack < 0 || attack > 100) {
            setScriptError("Invalid attack for template: " + templateString);
        } else if (health == null || health <= 0 || health > 100) {
            setScriptError("Invalid health for template: " + templateString);
        } else if (abilities == null) {
            setScriptError("Invalid abilities for template: " + templateString);
        }
    } else if (category === 1) {
        if (attack != null) {
            setScriptError("Invalid attack for template: " + templateString);
        } else if (health != null) {
            setScriptError("Invalid health for template: " + templateString);
        } else if (abilities != null) {
            setScriptError("Invalid abilities for template: " + templateString);
        }
    }
});

// Updates.
templatesJson.forEach(function(templateJson) {
    const id = templateJson.id;
    const category = templateJson.category;
    const color = templateJson.color;
    const cost = templateJson.cost;
    const name = templateJson.name;
    const attack = templateJson.attack;
    const health = templateJson.health;
    const abilities = templateJson.abilities;
    
    var templateDataItem = API.getItem("Template", id).document();
    if (templateDataItem === null) {
        templateDataItem = API.createItem("Template", id);
    }
    
    const templateData = templateDataItem.getData();
    templateData.id = id;
    templateData.category = category;
    templateData.color = color;
    templateData.cost = cost;
    templateData.name = name;
    
    if (category === 0) {
        templateData.attack = attack;
        templateData.health = health;
        templateData.abilities = abilities;
    }
    
    const error = templateDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
});

setScriptSuccess();
