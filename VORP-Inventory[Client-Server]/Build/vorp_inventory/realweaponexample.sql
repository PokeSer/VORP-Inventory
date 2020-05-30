-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versión del servidor:         10.4.11-MariaDB - mariadb.org binary distribution
-- SO del servidor:              Win64
-- HeidiSQL Versión:             10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Volcando estructura de base de datos para vorp
CREATE DATABASE IF NOT EXISTS `vorp` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;
USE `vorp`;

-- Volcando estructura para tabla vorp.loadout
CREATE TABLE IF NOT EXISTS `loadout` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `identifier` varchar(50) NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `ammo` longtext NOT NULL DEFAULT '{}',
  `components` longtext NOT NULL DEFAULT '[]',
  `dirtlevel` double DEFAULT 0,
  `mudlevel` double DEFAULT 0,
  `conditionlevel` double DEFAULT 0,
  `rustlevel` double DEFAULT 0,
  `used` tinyint(4) DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

-- Volcando datos para la tabla vorp.loadout: ~0 rows (aproximadamente)
/*!40000 ALTER TABLE `loadout` DISABLE KEYS */;
INSERT INTO `loadout` (`id`, `identifier`, `name`, `ammo`, `components`, `dirtlevel`, `mudlevel`, `conditionlevel`, `rustlevel`, `used`) VALUES
	(2, '', 'WEAPON_BOW', '{"AMMO_ARROW_DYNAMITE":10,"AMMO_ARROW_FIRE":20}', '["var1","var2"]', 0, 0, 0, 0, 0);
/*!40000 ALTER TABLE `loadout` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
