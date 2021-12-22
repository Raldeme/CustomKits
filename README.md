# Simple CustomKits

*Allows players to save and retrieve their entire inventory - or individual items - to a server-side MySQL database.*

This plugin was developed using [Rocket Mod](https://rocketmod.net/) libraries for the [Steam](http://store.steampowered.com/) game [Unturned](http://store.steampowered.com/app/304930/).


**Current Release :**
- Simple CustomKits v1.0.0.4

**How to Install:**

***CustomKits Rocket Plugin***
1. Compile this project
2. Copy the compiled `CustomKits.dll` to your Rocket Mod plugin directory
3. Start/stop your server to generate `./plugins/CustomKits/CustomKits.configuration.xml`
4. Edit `CustomKits.configuration.xml` and configure MySQL database settings
5. Add a Rocket Mod permission for the /CustomKits & /Kit command by adding it to your `Permissions.config.xml`
    - *Example*: 
        - `<Permission Cooldown="0">Kits</Permission>`
        - `<Permission Cooldown="0">Kit</Permission>`
6. Start Unturned Server


---
* modified by Raldeme
* original author: Nexis (steam:iamtwidget) <[nexis@nexisrealms.com](mailto:nexis@nexisrealms.com)>*