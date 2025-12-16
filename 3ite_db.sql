-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.4.32-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.13.0.7147
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for 3ite_db
CREATE DATABASE IF NOT EXISTS `3ite_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `3ite_db`;

-- Dumping structure for table 3ite_db.tbl_cart
CREATE TABLE IF NOT EXISTS `tbl_cart` (
  `cart_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`cart_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `tbl_cart_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_cart: ~0 rows (approximately)
DELETE FROM `tbl_cart`;

-- Dumping structure for table 3ite_db.tbl_cart_items
CREATE TABLE IF NOT EXISTS `tbl_cart_items` (
  `cart_item_id` int(11) NOT NULL AUTO_INCREMENT,
  `cart_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `quantity` int(11) NOT NULL DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`cart_item_id`),
  KEY `cart_id` (`cart_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `tbl_cart_items_ibfk_1` FOREIGN KEY (`cart_id`) REFERENCES `tbl_cart` (`cart_id`) ON DELETE CASCADE,
  CONSTRAINT `tbl_cart_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `tbl_products` (`product_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_cart_items: ~0 rows (approximately)
DELETE FROM `tbl_cart_items`;

-- Dumping structure for table 3ite_db.tbl_images
CREATE TABLE IF NOT EXISTS `tbl_images` (
  `image_id` int(11) NOT NULL AUTO_INCREMENT,
  `image_path` varchar(255) NOT NULL,
  `alt_text` varchar(255) DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`image_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_images: ~5 rows (approximately)
DELETE FROM `tbl_images`;
INSERT INTO `tbl_images` (`image_id`, `image_path`, `alt_text`, `is_active`, `created_at`, `updated_at`) VALUES
	(1, '/Assets/Images/Products/ff130cde-416d-4d23-a67a-8058b665611b_Jake.jpg', 'Jake the Dog', 1, '2025-12-17 03:12:24', '2025-12-17 03:12:24'),
	(2, '/Assets/Images/Products/7afb0c7d-c1e7-41f6-aa8e-a54d9a082249_Finn.jpg', 'Finn the Human', 1, '2025-12-17 04:09:23', '2025-12-17 04:09:23'),
	(3, '/Assets/Images/Products/cc5e86de-b9f5-4637-b3e9-bd55b137f430_Bubblegum.jpg', 'Princess Bubblegum', 1, '2025-12-17 05:15:10', '2025-12-17 05:15:10'),
	(4, '/Assets/Images/Products/3be56d90-344a-4965-8c1a-1f3066f08888_Rose Bouquet.png', 'Red Romance', 1, '2025-12-17 05:18:59', '2025-12-17 05:18:59'),
	(5, '/Assets/Images/Products/c94d53b1-cb2e-4147-b5fc-2a7d0c282e7f_Tulips Arrange.png', 'Enchanted Purple', 1, '2025-12-17 05:21:48', '2025-12-17 05:21:48'),
	(6, '/Assets/Images/Products/bebaf6c9-d78b-4967-abd0-67620d2ad283_Adventure Time Flowers.png', 'Adventure Flowers', 1, '2025-12-17 06:15:05', '2025-12-17 06:15:05');

-- Dumping structure for table 3ite_db.tbl_orderitems
CREATE TABLE IF NOT EXISTS `tbl_orderitems` (
  `order_item_id` int(11) NOT NULL AUTO_INCREMENT,
  `order_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `product_name` varchar(255) NOT NULL,
  `quantity` int(11) NOT NULL DEFAULT 1,
  `price_at_purchase` decimal(10,2) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`order_item_id`),
  KEY `order_id` (`order_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `tbl_orderitems_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `tbl_orders` (`order_id`) ON DELETE CASCADE,
  CONSTRAINT `tbl_orderitems_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `tbl_products` (`product_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_orderitems: ~5 rows (approximately)
DELETE FROM `tbl_orderitems`;
INSERT INTO `tbl_orderitems` (`order_item_id`, `order_id`, `product_id`, `product_name`, `quantity`, `price_at_purchase`, `created_at`) VALUES
	(1, 1, 2, 'Finn the Human', 1, 1000.00, '2025-12-17 05:40:52'),
	(2, 1, 4, 'Red Romance', 1, 800.00, '2025-12-17 05:40:52'),
	(3, 2, 1, 'Jake the Dog', 1, 500.00, '2025-12-17 06:00:13'),
	(4, 3, 3, 'Princess Bubblegum', 2, 700.50, '2025-12-17 06:01:03'),
	(5, 3, 5, 'Enchanted Purple', 2, 1700.00, '2025-12-17 06:01:03'),
	(6, 4, 6, 'Adventure Flowers', 1, 250.00, '2025-12-17 06:15:47');

-- Dumping structure for table 3ite_db.tbl_orders
CREATE TABLE IF NOT EXISTS `tbl_orders` (
  `order_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `order_status` enum('Pending','Processing','Shipped','Delivered','Cancelled') DEFAULT 'Pending',
  `shipping_status` enum('Pending','InTransit','Delivered') DEFAULT 'Pending',
  `total_amount` decimal(10,2) NOT NULL DEFAULT 0.00,
  `shipping_address` text NOT NULL,
  `recipient_name` varchar(200) NOT NULL,
  `recipient_phone` varchar(20) NOT NULL,
  `payment_method` enum('COD','GCash','Bank Transfer','Credit Card') DEFAULT 'COD',
  `payment_status` enum('Pending','Paid','Failed') DEFAULT 'Pending',
  `estimated_delivery` date DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`order_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `tbl_orders_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_orders: ~3 rows (approximately)
DELETE FROM `tbl_orders`;
INSERT INTO `tbl_orders` (`order_id`, `user_id`, `order_status`, `shipping_status`, `total_amount`, `shipping_address`, `recipient_name`, `recipient_phone`, `payment_method`, `payment_status`, `estimated_delivery`, `created_at`, `updated_at`) VALUES
	(1, 5, 'Pending', 'Pending', 1850.00, '109 yes st., qqweqwe, 0000', 'Andi Pascual', '0090990', '', 'Pending', '2025-12-20', '2025-12-17 05:40:51', '2025-12-17 06:05:36'),
	(2, 5, 'Pending', 'Pending', 550.00, 'yes st 42, City, 000', 'Andi Pascual', '0000', '', 'Pending', '2025-12-20', '2025-12-17 06:00:13', '2025-12-17 06:05:31'),
	(3, 5, 'Pending', 'Pending', 4851.00, 'my yes st. 1075, Manila, 0000', 'Andi Pascual', '0090990', '', 'Pending', '2025-12-20', '2025-12-17 06:01:03', '2025-12-17 06:05:25'),
	(4, 3, 'Pending', 'Pending', 300.00, 'Bulacan State University, Quezon City, 0000', 'Lara Dy', '000000000', 'GCash', 'Pending', '2025-12-20', '2025-12-17 06:15:47', '2025-12-17 06:15:47');

-- Dumping structure for table 3ite_db.tbl_productcategory
CREATE TABLE IF NOT EXISTS `tbl_productcategory` (
  `category_id` int(11) NOT NULL AUTO_INCREMENT,
  `category_name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_productcategory: ~4 rows (approximately)
DELETE FROM `tbl_productcategory`;
INSERT INTO `tbl_productcategory` (`category_id`, `category_name`, `description`, `is_active`, `created_at`, `updated_at`) VALUES
	(4, 'Roses', 'Beautiful red roses', 1, '2025-12-17 02:59:06', '2025-12-17 02:59:06'),
	(5, 'Tulips', 'Colorful tulips', 1, '2025-12-17 02:59:06', '2025-12-17 02:59:06'),
	(6, 'Lilies', 'Elegant lilies', 1, '2025-12-17 02:59:06', '2025-12-17 02:59:06'),
	(7, 'Mixed Bouquets', 'Assorted flower arrangements', 1, '2025-12-17 02:59:06', '2025-12-17 02:59:06');

-- Dumping structure for table 3ite_db.tbl_products
CREATE TABLE IF NOT EXISTS `tbl_products` (
  `product_id` int(11) NOT NULL AUTO_INCREMENT,
  `category_id` int(11) DEFAULT NULL,
  `image_id` int(11) DEFAULT NULL,
  `name` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `price` decimal(10,2) NOT NULL DEFAULT 0.00,
  `stock_quantity` int(11) DEFAULT 0,
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`product_id`),
  KEY `category_id` (`category_id`),
  KEY `image_id` (`image_id`),
  CONSTRAINT `tbl_products_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `tbl_productcategory` (`category_id`) ON DELETE SET NULL,
  CONSTRAINT `tbl_products_ibfk_2` FOREIGN KEY (`image_id`) REFERENCES `tbl_images` (`image_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_products: ~6 rows (approximately)
DELETE FROM `tbl_products`;
INSERT INTO `tbl_products` (`product_id`, `category_id`, `image_id`, `name`, `description`, `price`, `stock_quantity`, `is_active`, `created_at`, `updated_at`) VALUES
	(1, 4, 1, 'Jake the Dog', 'This is Jake the dog', 500.00, 3, 1, '2025-12-17 03:12:24', '2025-12-17 06:00:13'),
	(2, 6, 2, 'Finn the Human', 'this is Finn the human', 1000.00, 5, 1, '2025-12-17 04:09:23', '2025-12-17 05:54:19'),
	(3, 7, 3, 'Princess Bubblegum', 'this is princess bubblegum', 700.50, 8, 1, '2025-12-17 05:15:11', '2025-12-17 06:01:03'),
	(4, 4, 4, 'Red Romance', 'A beautifully arranged bouquet of red roses and white filler flowers, wrapped in elegant black and gold paper and finished with a bright red ribbon, placed on a soft white surface for a romantic and classy presentation.', 800.00, 9, 1, '2025-12-17 05:18:59', '2025-12-17 06:12:08'),
	(5, 5, 5, 'Enchanted Purple', 'A fresh and elegant bouquet of purple and white tulips, wrapped in soft beige and white layers with a delicate lavender ribbon, creating a calm and charming floral arrangement.', 1700.00, 18, 1, '2025-12-17 05:21:48', '2025-12-17 06:01:03'),
	(6, 7, 6, 'Adventure Flowers', 'This is an Adventure Time Flower Crochette', 250.00, 3, 1, '2025-12-17 06:15:05', '2025-12-17 06:15:47');

-- Dumping structure for table 3ite_db.tbl_sessions
CREATE TABLE IF NOT EXISTS `tbl_sessions` (
  `session_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `action` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`session_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `tbl_sessions_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_sessions: ~0 rows (approximately)
DELETE FROM `tbl_sessions`;

-- Dumping structure for table 3ite_db.tbl_users
CREATE TABLE IF NOT EXISTS `tbl_users` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(150) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `first_name` varchar(100) NOT NULL,
  `last_name` varchar(100) NOT NULL,
  `phone_number` varchar(20) DEFAULT NULL,
  `role` enum('admin','customer') NOT NULL DEFAULT 'customer',
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table 3ite_db.tbl_users: ~2 rows (approximately)
DELETE FROM `tbl_users`;
INSERT INTO `tbl_users` (`user_id`, `email`, `password_hash`, `first_name`, `last_name`, `phone_number`, `role`, `is_active`, `created_at`, `updated_at`) VALUES
	(3, 'admin@test.com', 'Admin123', 'Lara', 'Dy', '', 'admin', 1, '2025-12-16 23:19:41', '2025-12-16 23:20:51'),
	(5, 'user@test.com', 'User123', 'Andi', 'Pascual', '', 'customer', 1, '2025-12-16 23:21:45', '2025-12-16 23:21:45');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
