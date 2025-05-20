-- Add PaymentLinkId column to orders table
ALTER TABLE orders ADD COLUMN payment_link_id VARCHAR(255) NULL;
