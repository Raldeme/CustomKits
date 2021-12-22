# Adds `server_id` column to `Kits` table; required as of v1.0.0.3
ALTER TABLE `Kits` ADD COLUMN `server_id` VARCHAR(255) NULL AFTER `inventory`;