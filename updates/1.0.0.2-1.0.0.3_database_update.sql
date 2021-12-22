# Adds `server_id` column to `CustomKits` table; required as of v1.0.0.3
ALTER TABLE `CustomKits` ADD COLUMN `server_id` VARCHAR(255) NULL AFTER `inventory`;