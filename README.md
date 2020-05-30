# VORP-Inventory
Inventory System

## Requirements
- [VORP CORE](https://github.com/VORPCORE/VORP-Core/releases)
- [VORP Inputs](https://github.com/VORPCORE/VORP-Inputs/releases)

## How to install
* [Download the lastest version of VORP Inventory](https://github.com/VORPCORE/VORP-Inventory/releases)
* Copy and paste ```vorp_inventory``` folder to ```resources/vorp_inventory```
* Add ```ensure vorp_inventory``` to your ```server.cfg``` file
* To change the language go to ```resources/vorp_inventory``` and change the default language
* Now you are ready!

## Features
* Unique weapons in order not to duplicate them.
* Each weapon has its own ammo and can have diferent type of ammo.
* Each weapon has its own modifications.
* When dropping or giving weapon you give it with all modifications and ammo.
* Also has use of items.

## API For Lua
For import the api on top of your server resource file
```vorpInventory = exports.vorp_inventory:vorp_inventoryApi()```
this will return a table to simply use inventory
* Uses:
* Quit weapon
``` vorpInventory.subWeapon(source,weaponId)```
* Create and give new Weapon with the name of weapon in capital leters
``` vorpInventory.addWeapon(source,weaponName)```
* Add an item with cuantity
``` vorpInventory.addItem(source,item,cuantity)```
* Sub item with cuantity
``` vorpInventory.subItem(source,item,subCuantity)```
* Returns item cuantity
``` vorpInventory.getItemCuantity(source,item)```