CREATE TABLE `loadout` (
	`id` INT(11) NOT NULL AUTO_INCREMENT,
	`identifier` VARCHAR(50) NOT NULL,
	`name` VARCHAR(50) NULL DEFAULT NULL,
	`ammo` LONGTEXT NOT NULL,
	`components` LONGTEXT NOT NULL,
	PRIMARY KEY (`identifier`),
	INDEX `id` (`id`)
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;